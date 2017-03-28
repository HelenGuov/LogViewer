using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinFormAzureLogQuery.SourceCode;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LinqKit; 

namespace Tests
{
    /// <summary>
    /// This class focuses testing the Filter functionality. Ignore DateTime inputs. 
    /// </summary>
    [TestClass]
    public class FilterTest
    {
        /// <summary>
        /// This tests filter only on ONE selected level. In this case, selected level is DEBUG. 
        /// </summary>
        [TestMethod]
        public void FilterOnlyDebugLevel()
        {
            DateTime start = new DateTime(2016, 04, 20, 11, 34, 37, 146);
            DateTime end = new DateTime(2016, 4, 20, 11, 35, 29, 032); 

            FilterSet filter = new FilterSet();
            filter.Levels = new List<String>(); 
            filter.Levels.Add("DEBUG");
            List<LogEntity> data = Exec(start, end, filter).Result;
            
            //There are 5 entities contain DEBUG level.
            int expectedCount = 5; 
            string failureMsgListSize = CheckListSize(data, expectedCount);

            if (!String.IsNullOrEmpty(failureMsgListSize))
            { 
                Assert.Fail(String.Format("Expected {0} items, but found {1}", expectedCount, failureMsgListSize));
            }
            //else{
            //    //check the actual content in the list
            //    foreach (LogEntity logItem in data) {
            //        if (!logItem.Level.Equals("DEBUG")) {
            //            int itemNo = data.IndexOf(logItem); 
            //            Assert.Fail(String.Format("Item {0}: Expected DEBUG, but found {1}", itemNo, logItem.Level); 
            //        }
            //    }
            //}
            
        }

        [TestMethod]
        public void FilterMoreThanOneLevel()
        {
            DateTime start = new DateTime(2016, 04, 20, 11, 34, 37, 146);
            DateTime end = new DateTime(2016, 4, 20, 11, 35, 29, 032);

            FilterSet filter = new FilterSet();
            filter.Levels = new List<String>();
            filter.Levels.Add("DEBUG");  //expected 5 items
            filter.Levels.Add("ERROR");  //expected 4 items 

            List<LogEntity> data = Exec(start, end, filter).Result;

            //total expected 9 items
            int expectedCount = 9;
            string failureMsg = CheckListSize(data, expectedCount);

            if (!String.IsNullOrEmpty(failureMsg))
                Assert.Fail(String.Format("Expected {0} items, but found {1}", expectedCount, failureMsg)); 
        }


        #region private methods
        private string CheckListSize(List<LogEntity> resultedata, int expectedCount)
        {
            string message = "";

            if (resultedata == null) 
            {
                message = "null";
            }
            else if (resultedata.Count != expectedCount)
            {
                message = String.Format("{0} items", resultedata.Count); 
            }
       
            return message; 
        }

        private async Task<List<LogEntity>> Exec(DateTime startDate, DateTime endDate, FilterSet filter)
        {
            string tableName = "Table1"; 
            AzureTableConnection storage = new AzureTableConnection(tableName);
            CloudTable table = storage.GetTable();

            InputProcessor processor = new InputProcessor(startDate, endDate, table);
            List<LogEntity> data = await processor.RetrieveData(filter);

            return data;
        }
        #endregion
    }
}
