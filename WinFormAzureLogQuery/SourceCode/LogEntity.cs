using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormAzureLogQuery.SourceCode
{
    public class LogEntity : TableEntity
    {
        public LogEntity(string groupKey, string rowKey)
        {

            this.PartitionKey = groupKey;
            this.RowKey = rowKey;
        }

        public LogEntity() { }

        public string Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    } 
}
