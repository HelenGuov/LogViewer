using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinFormAzureLogQuery.SourceCode;
using System.IO;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.Table;
using System.Xml;


namespace Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        ///<summary>
        ///Test case for same start date and end date. it should return one record
        ///</summary>
        public void TestSamePartitionSameRowKey()
        {
            List<LogEntity> allLogs = Exec("2016-03-18 15:41", "16", "2016-03-18 15:41", "16", "samePartitionKeySameRowKey.txt");
            DateTimeOffset fromDt = new DateTimeOffset(new DateTime(2016, 3, 18, 15, 41, 16));
            DateTimeOffset toDt = new DateTimeOffset(new DateTime(2016, 3, 18, 15, 41, 16));
            VerifyResults(allLogs, fromDt, toDt); 
        }

        [TestMethod]
        ///<summary>
        ///Test for date time range within the same partition 
        ///</summary>
        public void TestSamePartition()
        {
            List<LogEntity> allLogs = Exec("2016-03-23 11:19", "04", "2016-03-23 11:19", "04", "samePartitionKey.txt");
            DateTimeOffset fromDate = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 19, 4));
            DateTimeOffset toDate = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 19, 4)); 
            VerifyResults(allLogs, fromDate, toDate); 

            //DateTime dt2 = DateTime.Parse("2016-03-18 15:41");
            //TimeSpan diff = dt2 - dt;
            //double minutes = diff.TotalMinutes;  
        }

        /// <summary>
        /// Test case:
        /// The minute in te startDAtePartitionKey in 1 greater than the minute in the endDatePartitionKey
        /// Difference in minute = 42 - 41 = 1 minute
        /// </summary>
        [TestMethod]
        public void TestPartitionDifferenceByOne()
        {        
            //Query on 2 partitions
            List<LogEntity> allLogs = Exec("2016-03-23 11:23", "16", "2016-03-23 11:24", "20", "partitionDiffByONe.txt");     
            DateTimeOffset fromDt = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 23, 16)); 
            DateTimeOffset toDt = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 24, 20));

            VerifyResults(allLogs, fromDt, toDt); 
        }

        /// <summary>
        /// Test case:
        /// minute in the startDate partitionKey is 2 greater than endDatePartitionKey
        /// Difference in minutes = startDateMinute - EndDateMinute = 2 minutes 
        /// </summary>
        [TestMethod]
        public void TestPartitionDifferenceByTwo()
        {
            //Query on 3 paritions
            //1st partition = startDate partition key
            //2nd partition = startDate partitionKey + 1
            //last partition = endDate partitionkKey
            List<LogEntity> allLogs = Exec("2016-03-23 11:22", "16", "2016-03-23 11:24", "20", "partitionDiffByTwo.txt");
            DateTimeOffset fromDt = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 22, 16));
            DateTimeOffset toDt = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 24, 20));
            VerifyResults(allLogs, fromDt, toDt); 
        }

        /// <summary>
        /// Test case:
        /// startDate = "2016-03-18 15:41", "16"
        /// endDate = "2016-03-18 15:50", "20"
        /// Difference in minutes = 50 - 41 = 9 minutes > 2mins 
        /// </summary>
        [TestMethod]
        public void TestPartitionDiffMoreThanTwo()
        {
            //Query x3
            //1st partition/start: "2016-03-18 15:41"
            //2nd partition/within:  "2016-03-18 15:50" < query > "2016-03-18 15:41"
            //last partition/end: "2016-03-18 15:50"
            List<LogEntity> allLogs = Exec("2016-03-18 11:19", "50", "2016-03-23 11:24", "28", "partitionDiffMoreThanTwo.txt");
            DateTimeOffset fromDt = new DateTimeOffset(new DateTime(2016, 3, 18, 11, 19, 50));
            DateTimeOffset toDt = new DateTimeOffset(new DateTime(2016, 3, 23, 11, 24, 28));
            VerifyResults(allLogs, fromDt, toDt); 
        }
        
        /// <summary>
        /// Method for executing the source code 
        /// </summary>
        /// <param name="startPartitionKey"></param>
        /// <param name="startRowKey"></param>
        /// <param name="endPartitionKey"></param>
        /// <param name="endRowKey"></param>
        /// <param name="fileName"></param>
        public static List<LogEntity> Exec(string startPartitionKey, string startRowKey, string endPartitionKey, string endRowKey, string fileName)
        {
            //TODO: add UI for the below 3 fields. 
         
            string tableName = "table1";

            AzureTableConnection storage = new AzureTableConnection(tableName);
            AzureLog result = new AzureLog(); 
            CloudTable table = storage.GetTable();
            
            //If table is null return error to the user. Abort the query
            List<TableQuery<LogEntity>> listOfQueries = result.GetAllQueries(startPartitionKey, startRowKey, endPartitionKey, endRowKey);
            
            List<LogEntity> allLogs = new List<LogEntity>(); 

            foreach (TableQuery<LogEntity> aQuery in listOfQueries)
            {
                aQuery.TakeCount = 100; 
                TableContinuationToken token = null;
                bool first = true;

                do
                {
                    ResultPair pair = result.GetResultsAsync(aQuery, table, token, first).Result;
                    first = false;
                    token = pair.token;
                    allLogs.AddRange(pair.logs);
                }
                while (token != null); 
            }
            //List<LogEntity> logs = await result.StartQueryAsync("2016-03-18 15:41", "16", "2016-03-18 15:41", "20"); 
            //List<LogEntity> logs = result.StartQueryAsync(startPartitionKey, startRowKey, endPartitionKey, endRowKey).Result;

            //WriteLogToFile(startPartitionKey, startRowKey, endPartitionKey, endRowKey, fileName, allLogs); 
            return allLogs; 
        }

        //TODO
        private static void VerifyResults(List<LogEntity> logResults, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            DateTimeOffset maxEndDate = toDate.AddMinutes(1.00); 
            foreach (LogEntity log in logResults)
            {
                DateTimeOffset localLogTime = log.Timestamp.ToLocalTime();

                if (!(localLogTime >= fromDate && localLogTime < maxEndDate))
                {
                    string failedItem = String.Format("Local Time: {0}, UTC: {1}, Msg: {2}", localLogTime, log.Timestamp, log.Message);
                    Assert.Fail(String.Format("{0}.{3}Expect time between {1} and {2}", failedItem, fromDate, toDate, Environment.NewLine)); 
                }
            }
        }
        private static void WriteLogToFile(string startPartitionKey, string startRowKey, string endPartitionKey, string endRowKey, string fileName, List<LogEntity> logs)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\Users\Hiang\Dropbox\IndustryConnect\06_Logging\WinFormAzureLogQuery\TestResults" + fileName))
            {
                writer.WriteLine(String.Format("Test Case: StartDate: {0}:{1}, EndDate: {2}:{3}{4}", startPartitionKey, startRowKey, endPartitionKey, endRowKey, System.Environment.NewLine));
                foreach (LogEntity log in logs)
                {
                    writer.WriteLine(String.Format("{0} {1} {2} {3} {4}", log.PartitionKey, log.RowKey, log.Logger, log.Message, log.Exception));
                }
            };
        }

       

         
    }
}
