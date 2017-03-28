using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinFormAzureLogQuery;
using System.IO; 

namespace WinFormAzureLogQuery.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]     
        public void SamePartitionTest()
        {
            Exec("2016-03-18 15:41", "16", "2016-03-18 15:41", "20", "samePartition.txt"); 
        }


        #region code for executing the query
        public static void Exec(string startPartitionKey, string startRowKey, string endPartitionKey, string endRowKey, string fileName)
        {
            //TODO: add UI for the below 3 fields. 
            string accountKey = "Tbs8YD99tId2fWak6Ff6KCjz6oJXhOljsBu1USpDcadpmJhUVX50cKZugRU0zOyIZpbHp/nVMaNj9bRqvDNEEQ==";
            string accountName = "htest";
            string tableName = "table1";

            AzureTable storage = new AzureTable(accountName, accountKey, tableName);
            LogResult result = new LogResult(storage);
            //List<LogEntity> logs = await result.StartQueryAsync("2016-03-18 15:41", "16", "2016-03-18 15:41", "20"); 
            List<LogEntity> logs = result.StartQueryAsync(startPartitionKey, startRowKey, endPartitionKey, endRowKey).Result;

            using (StreamWriter writer = new StreamWriter(@"C:\Users\Hiang\Dropbox\IndustryConnect\06_Logging\TestResults\" + fileName))
            {
                foreach (LogEntity log in logs)
                {
                    writer.WriteLine(String.Format("{0} {1} {2} {3} {4}", log.PartitionKey, log.RowKey, log.Logger, log.Message, log.Exception));
                }
            };

        }
        #endregion 
    }
}
