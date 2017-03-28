using LinqKit;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormAzureLogQuery.SourceCode
{
    public class InputProcessor
    {
        private AzureLog azureLog = new AzureLog();
        private CloudTable table; 
        private TableContinuationToken token;
        private FilterSet filter; 
        private List<List<LogEntity>> allLogs = new List<List<LogEntity>>();
        private bool newlyUsedQuery = true;
        private int queryIndex = 0;
        //listOfQueries can contain max of 3 queries for start, within and end. 
        List<TableQuery<LogEntity>> listOfQueries; //contains a max of 3 queries to be executed based on specified start, and end date
        private int queryLimit = 100; //each query contains a max of 100 records, same for each page

        //getters
        public bool NewlyUsedQuery { get { return newlyUsedQuery; } }
        public int QueryIndex { get { return queryIndex; } }
        public List<TableQuery<LogEntity>> QueryList {get { return listOfQueries; } }
        public TableContinuationToken QueryToken { get { return token;  } }
        public List<List<LogEntity>> AllLogs { get { return allLogs;  } }
        
        public int GetTotalLogCount()
        {
            if (this.allLogs == null || this.allLogs.Count == 0)
                return 0;

            int totalPages = this.allLogs.Count; 
            int lastItemCount = this.allLogs[totalPages - 1].Count;
            int totalRecords = this.queryLimit * (totalPages - 1) + lastItemCount;

            return totalRecords; 
        }

        public int GetLastLogIndx()
        {
            return this.allLogs.Count - 1;  //return index of the last element in the list 
        }

        public List<LogEntity> GetLogAtIndex(int indx)
        {
            return this.allLogs[indx]; 
        }

        public InputProcessor(DateTime startDate, DateTime endDate, CloudTable table, FilterSet filter)
        {
            Initialise(startDate, endDate, table, filter); 
        }

        private void Initialise(DateTime startDate, DateTime endDate, CloudTable table, FilterSet filter)
        {
            DatePair startPair = GetParsedDate(startDate);
            DatePair endPair = GetParsedDate(endDate);
               
            //Start the Queries
            this.listOfQueries = azureLog.GetAllQueries(startPair.TimeToMinutes, startPair.TimeToSeconds, endPair.TimeToMinutes, endPair.TimeToSeconds);
            this.table = table;
            this.filter = filter; 
        } 

        /// <summary>
        /// Retrieve data from the cloud, and return a list of data with the size of the specified row limti.
        /// Default: this.rowLimit = 100
        /// </summary>
        /// <returns></returns>
        public async Task<List<LogEntity>> RetrieveData()
        {
            TableQuery<LogEntity> query = listOfQueries[queryIndex];
            query.TakeCount = this.queryLimit;
            List<LogEntity> result = await GetDataFromAzure(query);
            
            if (this.filter != null) //Used filter
                result = FilterResults(result); 
            
            if (this.token == null)  //The current query has been done, move to the next one
            {
                SetToNextQuery();
            }           
            
            //For example, if queryLimit=100, and requestedDataCount = 50, it will get another 50 records, so each 
            //list of LogEntity has the same number of elements, until no more records can be retrieved. 
                  
            bool endOfAQuery = this.newlyUsedQuery == false && this.token == null;  
            bool lastQueryInList = (this.listOfQueries.Count - 1) == this.queryIndex;
            bool noMoreQuery = endOfAQuery && lastQueryInList; 
            while (!noMoreQuery && result.Count < this.queryLimit)
            {
                //get remaining data
                int remainingCount = this.queryLimit - result.Count; //No need to check for zero, as the statement of the while loop has done that
                List<LogEntity> remainData = await GetRemainingData(remainingCount);
                if (remainData != null)
                {
                    if (this.filter != null)
                        remainData = FilterResults(remainData); 
                    result.AddRange(remainData);
                }

                if (this.token == null) //token is null when no more item to be retrieved from that particular query
                {
                    SetToNextQuery(); 
                }
                //re-set so to determine whether to go through the next while loop
                
                //update noMoreRemaining, before executing the next loop
                endOfAQuery = this.newlyUsedQuery == false && this.token == null;
                lastQueryInList = (this.listOfQueries.Count - 1) == this.queryIndex;
                noMoreQuery = endOfAQuery && lastQueryInList; 
            }
            
            this.allLogs.Add(result); 
            return result; 
        }

        /// <summary>
        /// This method will only be called at the a query, when query result is less than the query limit. 
        /// </summary>
        /// <param name="currentCount"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private async Task<List<LogEntity>> GetRemainingData(int remainCount) { 
            //Retrieve remaining data from the cloud 
            TableQuery<LogEntity> query = this.listOfQueries[this.queryIndex];

            query.TakeCount = remainCount; 
            List<LogEntity> remainingData = await GetDataFromAzure(query);

            return remainingData; 
        }

        /// <summary>
        /// Use the next query in the listOfQueries
        /// </summary>
        private void SetToNextQuery()
        {
            int nextTargetIndx = this.queryIndex + 1;

            bool isValidIndx = nextTargetIndx <= (listOfQueries.Count - 1);

            if (isValidIndx) 
            {
                //set to the next query
                ++this.queryIndex;
                this.newlyUsedQuery = true; //start of new query
            }  


            //Now firstQuery is reset to true, as the next query is being used for the first time. 
            //Do not need to reset this.token to null because this method is call onlyed when the end of query result is reached.
            //i.e. when this.token = null
        }

        /// <summary>
        /// Get the data from the Azure,and also updating the token property 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private async Task<List<LogEntity>> GetDataFromAzure(TableQuery<LogEntity> query)
        {
            if (this.newlyUsedQuery || (!this.newlyUsedQuery && this.token != null))
            {
                ResultPair  resultPair = await azureLog.GetResultsAsync(query, this.table, this.token, this.newlyUsedQuery);
                this.token = resultPair.token; //update token, so can be used in other methods in this class
                this.newlyUsedQuery = false;
              
                return resultPair.logs; 
            }

            return null;
        }

        private List<LogEntity> FilterResults(List<LogEntity> result)
        {        
            IQueryable<LogEntity> qResult = result.AsQueryable();
            List<LogEntity> filteredResults = new List<LogEntity>();

            //Filter on keyword
            if (!String.IsNullOrEmpty(this.filter.Keyword))
                 qResult = qResult.Where(x => x.Message.Contains(this.filter.Keyword));

            //Filter on levels
            if (this.filter.Levels != null 
                && this.filter.Levels.Count != 0
                && this.filter.Levels.Count != this.filter.LevelTotalCount) //Do not need to filter data when all checkboxes are ticked. 
            {                
                //Use expression tree
                var predicate = PredicateBuilder.False<LogEntity>(); 
                foreach (string level in this.filter.Levels)
                {
                    predicate = predicate.Or(l => l.Level.Equals(level));
                }
                qResult = qResult.Where(predicate);  
            }

            return qResult.ToList(); 
        }

        private DatePair GetParsedDate(DateTime inputDate)
        {
            DatePair pair = new DatePair(inputDate.ToString("yyyy-MM-dd HH:mm"), inputDate.Second.ToString());
            return pair;
        }
    }

    class DatePair
    {
        private string timeToMinutes;
        private string timeToSeconds;

        public string TimeToMinutes { get { return this.timeToMinutes; } }
        public string TimeToSeconds { get { return this.timeToSeconds; } }

        public DatePair(string timeToMinutes, string timeToSeconds)
        {
            this.timeToMinutes = timeToMinutes;
            this.timeToSeconds = timeToSeconds;
        }

    }

}
