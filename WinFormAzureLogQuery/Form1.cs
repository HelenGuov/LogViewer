using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormAzureLogQuery.SourceCode;

namespace WinFormAzureLogQuery
{
    public partial class Form1 : Form
    {
        private InputProcessor processor;
        private int currentPageIndx = 0;  //Each page is the index of the items in the InputProcessor.QueryList
        private bool acceptAsyncCall = true;

        public Form1()
        {
            InitializeComponent();
            TickAllLevels();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                FetchNextPage();
                return true;
            }
            else if (keyData == Keys.Left)
            {
                FetchPrevPage();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void search_Click(object sender, EventArgs e)
        {
            ResetFormState();
            DisableAllButtons();
            string tableName = this.tableNameTextBox.Text;
            if (String.IsNullOrEmpty(tableName))
            {
                this.errorProvider1.SetError(this.tableNameTextBox, "Please enter the table name");
                return;
            }

            AzureTableConnection storage = new AzureTableConnection(tableName);
            CloudTable table = storage.GetTable();

            //Check for table existence 
            bool tableExists = table.Exists();
            if (!tableExists)
            {
                this.errorProvider1.SetError(this.tableNameTextBox, "Table (" + tableName + ") cannot be found");
                return;
            }

            
            this.errorProvider1.Clear(); //If there was an error previously, Clear error message.
            
            FilterSet filteredInput = GetFilters();
            this.processor = new InputProcessor(
                this.startDateTimePicker.Value, this.endDateTimePicker.Value, table, filteredInput);

            this.searchButton.Enabled = false;
            List<LogEntity> logs = await this.processor.RetrieveData();

            this.resultListView.Clear(); //Clear prev data if any
            AddLogDataToUIList(logs);
            ChangeNextButtonState();

            UpdateRecordCount(processor.GetTotalLogCount());
            this.searchButton.Enabled = true;
        }

        private async void nextButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons(); 
            await FetchNextPage();
            Debug.WriteLine("return to next click"); 
            UpdatePageNumber();
            this.searchButton.Enabled = true;
        }

        private void prevButton_Click_1(object sender, EventArgs e)
        {
            DisableAllButtons(); 
            FetchPrevPage();
            Debug.WriteLine("return to prev click handler"); 
            UpdatePageNumber();
            this.searchButton.Enabled = true; 
        }

        private async Task FetchNextPage()
        {
            if (!this.acceptAsyncCall) return;

            this.acceptAsyncCall = false; 
            //set to the next page
            int nextPageIndx = this.currentPageIndx + 1;
            bool completedQuery = !this.processor.NewlyUsedQuery && this.processor.QueryToken == null;
            bool lastQuery = this.processor.QueryIndex == (this.processor.QueryList.Count - 1);

            int lastPageIndx = this.processor.GetLastLogIndx();
            if (nextPageIndx <= lastPageIndx)
            {
                this.currentPageIndx = nextPageIndx;
                List<LogEntity> logData = this.processor.GetLogAtIndex(this.currentPageIndx); 
                this.resultListView.Clear();
                AddLogDataToUIList(logData); 
            }
            else if (nextPageIndx > lastPageIndx && !lastQuery && !completedQuery) 
            {   //It's possible to request more data 

                List<LogEntity> logs = await this.processor.RetrieveData();
                if (logs.Count == 0) return; //No need to update UI, no record is found

                this.resultListView.Clear();
                AddLogDataToUIList(logs);
                this.currentPageIndx = nextPageIndx;
                Debug.WriteLine("In fetch next page, change page to: {0} ", this.currentPageIndx); 
            }
     
            ChangeNextButtonState();
            ChangePrevbuttonState();
            UpdateRecordCount(processor.GetTotalLogCount());
            

            this.acceptAsyncCall = true;
            
            Debug.WriteLine("End of fetch next"); 
        }

        private void FetchPrevPage()
        {
            if (!this.acceptAsyncCall) return; //abort page when the page is currently loaded 

            this.acceptAsyncCall = false; 
            int prevPage = this.currentPageIndx - 1;
            if (prevPage >= 0)
            {
                this.currentPageIndx = prevPage;
                Debug.WriteLine("In next page, page num is " + this.currentPageIndx); 
                this.resultListView.Clear();
                AddLogDataToUIList(this.processor.AllLogs[this.currentPageIndx]);
                this.acceptAsyncCall = true; 
            }

            ChangeNextButtonState();
            ChangePrevbuttonState();

            UpdateRecordCount(processor.GetTotalLogCount());
            this.acceptAsyncCall = true;
            Debug.WriteLine("end of prev page"); 
        }

        #region UI 

        private void TickAllLevels()
        {
            this.debugCheck.Checked = true;
            this.infoCheck.Checked = true;
            this.errorCheck.Checked = true;
            this.fatalCheck.Checked = true;
            this.warnCheck.Checked = true; 
        }

        private void AddLogDataToUIList(List<LogEntity> logs)
        {
            foreach (LogEntity log in  logs)
            {
                //string logDate = log.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"); 
                string rowKeyPattern = @"(\d\d.\d\d\d)-(.)*";
                Regex regex = new Regex(rowKeyPattern);
                Match m = regex.Match(log.RowKey);
                string rowTime = m.Groups[1].Value;
                string logDate = String.Format("{0}:{1}", log.PartitionKey, rowTime);

                String[] rowItems = { logDate, log.Thread, log.Level, log.Logger, log.Message };
                ListViewItem viewItem = new ListViewItem(rowItems);
                this.resultListView.Items.Add(viewItem);
            }
            MakeColumns();

            this.resultListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            this.resultListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void MakeColumns()
        {
            // Create colum ns for the items and subitems.
            // Width of -2 indicates auto-size.
            this.resultListView.Columns.Add("Date", -2, HorizontalAlignment.Left);
            this.resultListView.Columns.Add("Thread", -2, HorizontalAlignment.Left);
            this.resultListView.Columns.Add("Level", -2, HorizontalAlignment.Left);
            this.resultListView.Columns.Add("Logger", -2, HorizontalAlignment.Left);
            this.resultListView.Columns.Add("Message", -2, HorizontalAlignment.Left);
            this.resultListView.Columns.Add("Exception", -2, HorizontalAlignment.Left);
        }

        private void UpdateRecordCount(int dataCount)
        {
            if (dataCount == 0)
                lblRecordCount.Text = "No data";
            else
                lblRecordCount.Text = dataCount.ToString();
        }

        private void UpdatePageNumber()
        {
           // this.lblPageNumber.Text = (this.currentPageIndx + 1).ToString(); 
            this.lblPageNumber.Text = this.currentPageIndx.ToString();
        }

        /// <summary>
        /// Manipulate the Filter object according to user inputs for the Filter group controls
        /// </summary>
        /// <returns></returns>
        private FilterSet GetFilters()
        {
            FilterSet filter = new FilterSet();

            //keyword filter
            filter.Keyword = this.keywordText.Text;
            filter.Levels = new List<String>();

            //level CheckBox filter
            if (debugCheck.Checked)
                filter.Levels.Add(debugCheck.Text); 

            if (infoCheck.Checked)
                filter.Levels.Add(infoCheck.Text);

            if (warnCheck.Checked)
                filter.Levels.Add(warnCheck.Text);

            if (errorCheck.Checked)
                filter.Levels.Add(errorCheck.Text);

            if (fatalCheck.Checked)
                filter.Levels.Add(fatalCheck.Text);
      
            return filter;
        }


        #endregion

        #region Set button states
        private void ChangeNextButtonState()
        {
            bool reachedEndOfQueries = this.processor.QueryIndex == (this.processor.QueryList.Count() - 1);
            bool allResultsObtained = reachedEndOfQueries && this.processor.QueryToken == null && (this.processor.NewlyUsedQuery == false);
            bool lastPage = this.currentPageIndx == (this.processor.AllLogs.Count - 1);
            if (this.processor.AllLogs.Count == 0)
            {
                //no record 
                this.nextButton.Enabled = false;
            }
            else if (allResultsObtained && lastPage)
            {
                //Nothing to request, so button is disable
                this.nextButton.Enabled = false;
            }
            else
            {
                //pass the above 2 conditions, so enable the Next button 
                this.nextButton.Enabled = true;
            }
        }

        private void ChangePrevbuttonState()
        {
            Debug.WriteLine(this.currentPageIndx); 
            if (this.processor.AllLogs.Count == 0)
            {
                //no record
                this.prevButton.Enabled = false;
            }
            else if (this.currentPageIndx == 0)
            {
                //first page
                this.prevButton.Enabled = false;
            }
            else
            {
                this.prevButton.Enabled = true;
            }
        }
        
        private void ResetFormState()
        {
            this.prevButton.Enabled = false;
            this.nextButton.Enabled = false;
            this.lblPageNumber.Text = "0"; 
            
            this.currentPageIndx = 0;        
            this.acceptAsyncCall = true;
        }
        

        /// <summary>
        /// Disable all buttons so for async purposes 
        /// </summary>
        private void DisableAllButtons()
        {
            this.searchButton.Enabled = false;
            this.nextButton.Enabled = false;
            this.prevButton.Enabled = false;
        }

        private void EnableAllButtons()
        {
            this.searchButton.Enabled = true;
            this.nextButton.Enabled = true;
            this.prevButton.Enabled = true; 
        }
        #endregion




  
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
