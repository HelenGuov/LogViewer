using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinFormAzureLogQuery.SourceCode;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// Summary description for InputProcessorTest
    /// </summary>
    [TestClass]
    public class InputProcessorTest
    {
        #region Check force the program to execute queries multiple within the same call 

        /// <summary>
        /// Test case: 2016-03-23 11:23:17
        /// query 1: partitionkey eq 2016-03-23 11:23 rowkey gte 17
        /// query 2: partitionkey gt 2016-03-23 11:23 AND rowkey lt <datetime.Now>
        /// query 3: partitionkey eq <yyyy-MM-dd HH:mm> now AND rowkey lte <datetime.Now.Sec>
        /// There are only results for query 1 and 2. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCallRemainingOnce()
        {
            string tableName = "Table1"; 
            AzureTableConnection storage = new AzureTableConnection(tableName);
            CloudTable table = storage.GetTable();
            DateTime start = new DateTime(2016, 03, 23, 11, 23, 17, 0);
            DateTime end = DateTime.Now;
            InputProcessor processor = new InputProcessor(start, end, table);

            List<LogEntity> logs = await processor.RetrieveData();          
        }

        /// <summary>
        /// Query 1: get 17 records (need 83 more to make 100)
        /// Query 2: get 35 records (48 more) 
        /// Query 3: got 16 records 
        /// all 3 queries above do not add up to 100, but call to RetrieveData() will be returned, as
        /// no all queries have been used in the listOfQuries in the InputProcessor class. 
        /// </summary> 
        /// <returns></returns>
        [TestMethod]
        public async Task TestCallRemainingTwice()
        {
            string tableName = "Table1";
            AzureTableConnection storage = new AzureTableConnection(tableName);
            CloudTable table = storage.GetTable();
            DateTime start = new DateTime(2016, 03, 23, 11, 23, 17, 0);
            DateTime end = new DateTime(2016, 03, 29, 11, 47, 52, 0);
            InputProcessor processor = new InputProcessor(start, end, table);

            List<LogEntity> logs = await processor.RetrieveData();
        }
        #endregion

        #region Check empty results. If empty list is returned, 
        //Purpose: Ensuring that any list in between is not empty
            //case 1: InputProcessor.RetrieveData() returns empty list when clicked on Search.
            //i.e. all queries return empty list 
            //query1: empty, query2: empty, query3: empty
             
            //case 2: query1: not empty, query2: empty, query3: empty 
            //case 3: query2: 
        #endregion

         

    }
}
