using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace WinFormAzureLogQuery.SourceCode
{
    public class AzureLog
    {
        /// <summary>
        /// Retrieve log data from the Azure Table service in an asynchronous manner
        /// </summary>
        /// <param name="tableQuery"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task<ResultPair> GetResultsAsync(TableQuery<LogEntity> tableQuery, CloudTable table, TableContinuationToken token, bool firstQuery)
        { 
            TableQuerySegment<LogEntity> tableQueryResult = null; 

            if (firstQuery || (!firstQuery && token != null))
            {
                tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, token);
                token = tableQueryResult.ContinuationToken;

            }

            return new ResultPair(tableQueryResult.Results, token);      
        }


        #region helper methods
            /// <summary>
            /// Get Time differences in minutes between the start and end partition Key.
            /// i.e "2016-03-18 15:41" is the partition key. 
            /// </summary>
            /// <param name="startDtPartKey"></param>
            /// <param name="endDtPartKey"></param>
            /// <returns></returns>
            private double GetTimeDiff(string startDtPartKey, string endDtPartKey) {
                DateTime dt = DateTime.Parse(startDtPartKey);
                DateTime dt2 = DateTime.Parse(endDtPartKey);
                TimeSpan diff = dt2 - dt;
                return diff.TotalMinutes;  
            }

            /// <summary>
            /// Get number of queries depending on the start and end time. Types of queries are start, Within, End
            /// Refer to TestPartitionDiffMoreThanTwo() in UnitTest1.cs in the Tests project. 
            /// </summary>
            /// <param name="startDatePartKey"></param>
            /// <param name="startDateRowKey"></param>
            /// <param name="endDatePartKey"></param>
            /// <param name="endDateRowKey"></param>
            /// <returns></returns>
            public List<TableQuery<LogEntity>> GetAllQueries(
                string startDatePartKey, string startDateRowKey,
                string endDatePartKey, string endDateRowKey)
            {
                //Use for filter on row key when it is at greater or less than. 
                string maxEndRowKey = (Int32.Parse(endDateRowKey) + 1).ToString();  //RowKey < maxEnd, take all row keys that are up to maxEndSec
                double minutesDiff = GetTimeDiff(startDatePartKey, endDatePartKey);

                TableQuery<LogEntity> queryStart = new TableQuery<LogEntity>();
                TableQuery<LogEntity> queryWithin = new TableQuery<LogEntity>();
                TableQuery<LogEntity> queryEnd = new TableQuery<LogEntity>();
                List<TableQuery<LogEntity>> listOfQueries = new List<TableQuery<LogEntity>>(); 

                if (startDatePartKey.Equals(endDatePartKey) && startDateRowKey.Equals(endDateRowKey))
                {
                    // both start and end date are the same
                    string filterOnPartRowKey =
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition(
                                "PartitionKey", QueryComparisons.Equal, startDatePartKey), 
                            TableOperators.And, 
                            TableQuery.GenerateFilterCondition(
                                "RowKey", QueryComparisons.Equal, startDateRowKey));

                    queryStart.FilterString = filterOnPartRowKey;

                    // queryWithin is not used
                    // queryEnd is not used
                }
                else if (minutesDiff == 0)
                {
                    // Same Partition

                    // Create a filter for the seconds (ie row key)
                    string filterOnRowKeys =
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition(
                                "RowKey", QueryComparisons.GreaterThanOrEqual, startDateRowKey),
                            TableOperators.And, 
                            TableQuery.GenerateFilterCondition(
                                "RowKey", QueryComparisons.LessThanOrEqual, endDateRowKey));

                    // Combine the date+hour with seconds filter
                    string totalCombinedFilters =
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition(
                                "PartitionKey", QueryComparisons.Equal, startDatePartKey),
                            TableOperators.And, 
                                filterOnRowKeys);

                    queryStart.FilterString = totalCombinedFilters;               
                }
                else if (minutesDiff >= 1.0) //end minute > start minute = 1 
                {
                    // Different partitions

                    //start date 
                    string filterOnStartPartKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, startDatePartKey); 
                    string filterOnStartRowKey = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startDateRowKey);
                    string combinedStartFilters = TableQuery.CombineFilters(filterOnStartPartKey, TableOperators.And, filterOnStartRowKey);
                    queryStart.FilterString = combinedStartFilters;

                    //end date 
                    string filterOnEndPartKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, endDatePartKey);
                    // maxEndRowKey is 1 second greater than the user specified end time
                    // e.g. user asks for 16:46:20, end time would be "21" seconds
                    string filterOnEndRowKey = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, maxEndRowKey);
                    string combinedEndFilters = TableQuery.CombineFilters(filterOnEndPartKey, TableOperators.And, filterOnEndRowKey);
                    queryEnd.FilterString = combinedEndFilters;

                    if (minutesDiff == 2) {                
                        //the single partition that is greater than start date and less than end date
                        DateTime middlePartition = DateTime.Parse(startDatePartKey);
                        middlePartition = middlePartition.AddMinutes(1);
                        string middlePartitionStr = middlePartition.ToString("yyyy-MM-dd HH:mm");
                        string filterOnMiddlePartition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, middlePartitionStr);
                        queryWithin.FilterString = filterOnMiddlePartition; 
                    }
                    else if (minutesDiff > 2) {
                        string filterOnMidStart = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, startDatePartKey);
                        string filterOnMidEnd = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, endDatePartKey);
                        string combinedMidRangePartition = TableQuery.CombineFilters(filterOnMidStart, TableOperators.And, filterOnMidEnd); 
                        queryWithin.FilterString = combinedMidRangePartition; 
                    }
                }
                
                

                if (queryStart.FilterString != null) {
                    listOfQueries.Add(queryStart);
                }
                if (queryWithin.FilterString != null)
                {
                    listOfQueries.Add(queryWithin);
                }
                if (queryEnd.FilterString != null)
                {
                    listOfQueries.Add(queryEnd); 
                }

                return listOfQueries; 
            }
        #endregion
    }
}
