using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormAzureLogQuery.SourceCode
{
    public class ResultPair
    {
        public List<LogEntity> logs { get; set; }
        public TableContinuationToken token {get; set; }

        public ResultPair(List<LogEntity> logs, TableContinuationToken token)
        {
            this.logs = logs;
            this.token = token; 
        }
    }
}
