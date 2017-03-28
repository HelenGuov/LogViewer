using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormAzureLogQuery.SourceCode
{
    public class AzureTableConnection
    {
        private string connectionString; 
        private string tableName;
        private string ACCOUNT_KEY = "Tbs8YD99tId2fWak6Ff6KCjz6oJXhOljsBu1USpDcadpmJhUVX50cKZugRU0zOyIZpbHp/nVMaNj9bRqvDNEEQ==";
        private string ACCOUNT_NAME = "htest";  // Azure Account name

        public   AzureTableConnection(string tableName) {
            this.connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", this.ACCOUNT_NAME, this.ACCOUNT_KEY); 
            this.tableName = tableName; 
        }

        public CloudTable GetTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.connectionString);
            // Create the table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(this.tableName);
           
            return table;
        }

        
    }
}
