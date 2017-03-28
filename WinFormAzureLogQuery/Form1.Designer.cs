using System.Windows.Forms;
namespace WinFormAzureLogQuery
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.searchButton = new System.Windows.Forms.Button();
            this.tableNameTextBox = new System.Windows.Forms.TextBox();
            this.tableNameLabel = new System.Windows.Forms.Label();
            this.startDateLabel = new System.Windows.Forms.Label();
            this.endDateLabel = new System.Windows.Forms.Label();
            this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.resultListView = new System.Windows.Forms.ListView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.nextButton = new System.Windows.Forms.Button();
            this.prevButton = new System.Windows.Forms.Button();
            this.keywordLabel = new System.Windows.Forms.Label();
            this.keywordText = new System.Windows.Forms.TextBox();
            this.filterGroup = new System.Windows.Forms.GroupBox();
            this.fatalCheck = new System.Windows.Forms.CheckBox();
            this.errorCheck = new System.Windows.Forms.CheckBox();
            this.warnCheck = new System.Windows.Forms.CheckBox();
            this.infoCheck = new System.Windows.Forms.CheckBox();
            this.debugCheck = new System.Windows.Forms.CheckBox();
            this.levelLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.lblPage = new System.Windows.Forms.Label();
            this.lblPageNumber = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.filterGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // startDateTimePicker
            // 
            this.startDateTimePicker.AllowDrop = true;
            this.startDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.startDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDateTimePicker.Location = new System.Drawing.Point(86, 77);
            this.startDateTimePicker.Name = "startDateTimePicker";
            this.startDateTimePicker.Size = new System.Drawing.Size(156, 20);
            this.startDateTimePicker.TabIndex = 1;
            this.startDateTimePicker.Value = new System.DateTime(2016, 3, 9, 13, 0, 0, 0);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(86, 309);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 37);
            this.searchButton.TabIndex = 2;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.search_Click);
            // 
            // tableNameTextBox
            // 
            this.tableNameTextBox.Location = new System.Drawing.Point(86, 21);
            this.tableNameTextBox.Name = "tableNameTextBox";
            this.tableNameTextBox.Size = new System.Drawing.Size(200, 20);
            this.tableNameTextBox.TabIndex = 7;
            // 
            // tableNameLabel
            // 
            this.tableNameLabel.AutoSize = true;
            this.tableNameLabel.Location = new System.Drawing.Point(18, 24);
            this.tableNameLabel.Name = "tableNameLabel";
            this.tableNameLabel.Size = new System.Drawing.Size(65, 13);
            this.tableNameLabel.TabIndex = 8;
            this.tableNameLabel.Text = "Table Name";
            // 
            // startDateLabel
            // 
            this.startDateLabel.AutoSize = true;
            this.startDateLabel.Location = new System.Drawing.Point(18, 77);
            this.startDateLabel.Name = "startDateLabel";
            this.startDateLabel.Size = new System.Drawing.Size(55, 13);
            this.startDateLabel.TabIndex = 9;
            this.startDateLabel.Text = "Start Date";
            // 
            // endDateLabel
            // 
            this.endDateLabel.AutoSize = true;
            this.endDateLabel.Location = new System.Drawing.Point(21, 109);
            this.endDateLabel.Name = "endDateLabel";
            this.endDateLabel.Size = new System.Drawing.Size(52, 13);
            this.endDateLabel.TabIndex = 10;
            this.endDateLabel.Text = "End Date";
            // 
            // endDateTimePicker
            // 
            this.endDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.endDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDateTimePicker.Location = new System.Drawing.Point(86, 103);
            this.endDateTimePicker.Name = "endDateTimePicker";
            this.endDateTimePicker.Size = new System.Drawing.Size(156, 20);
            this.endDateTimePicker.TabIndex = 11;
            // 
            // resultListView
            // 
            this.resultListView.FullRowSelect = true;
            this.resultListView.Location = new System.Drawing.Point(316, 21);
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(858, 385);
            this.resultListView.TabIndex = 15;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(756, 412);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(62, 35);
            this.nextButton.TabIndex = 17;
            this.nextButton.Text = "▶";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // prevButton
            // 
            this.prevButton.Location = new System.Drawing.Point(672, 412);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(62, 35);
            this.prevButton.TabIndex = 16;
            this.prevButton.Text = "◀";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click_1);
            // 
            // keywordLabel
            // 
            this.keywordLabel.AutoSize = true;
            this.keywordLabel.Location = new System.Drawing.Point(6, 27);
            this.keywordLabel.Name = "keywordLabel";
            this.keywordLabel.Size = new System.Drawing.Size(54, 13);
            this.keywordLabel.TabIndex = 19;
            this.keywordLabel.Text = "Keyword: ";
            // 
            // keywordText
            // 
            this.keywordText.Location = new System.Drawing.Point(74, 19);
            this.keywordText.Name = "keywordText";
            this.keywordText.Size = new System.Drawing.Size(200, 20);
            this.keywordText.TabIndex = 20;
            // 
            // filterGroup
            // 
            this.filterGroup.Controls.Add(this.fatalCheck);
            this.filterGroup.Controls.Add(this.errorCheck);
            this.filterGroup.Controls.Add(this.warnCheck);
            this.filterGroup.Controls.Add(this.infoCheck);
            this.filterGroup.Controls.Add(this.debugCheck);
            this.filterGroup.Controls.Add(this.keywordText);
            this.filterGroup.Controls.Add(this.levelLabel);
            this.filterGroup.Controls.Add(this.keywordLabel);
            this.filterGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.filterGroup.Location = new System.Drawing.Point(12, 147);
            this.filterGroup.Name = "filterGroup";
            this.filterGroup.Size = new System.Drawing.Size(285, 145);
            this.filterGroup.TabIndex = 25;
            this.filterGroup.TabStop = false;
            this.filterGroup.Text = "Filter";
            // 
            // fatalCheck
            // 
            this.fatalCheck.AutoSize = true;
            this.fatalCheck.Location = new System.Drawing.Point(190, 84);
            this.fatalCheck.Name = "fatalCheck";
            this.fatalCheck.Size = new System.Drawing.Size(59, 17);
            this.fatalCheck.TabIndex = 31;
            this.fatalCheck.Text = "FATAL";
            this.fatalCheck.UseVisualStyleBackColor = true;
            // 
            // errorCheck
            // 
            this.errorCheck.AutoSize = true;
            this.errorCheck.Location = new System.Drawing.Point(190, 60);
            this.errorCheck.Name = "errorCheck";
            this.errorCheck.Size = new System.Drawing.Size(65, 17);
            this.errorCheck.TabIndex = 30;
            this.errorCheck.Text = "ERROR";
            this.errorCheck.UseVisualStyleBackColor = true;
            // 
            // warnCheck
            // 
            this.warnCheck.AutoSize = true;
            this.warnCheck.Location = new System.Drawing.Point(74, 107);
            this.warnCheck.Name = "warnCheck";
            this.warnCheck.Size = new System.Drawing.Size(60, 17);
            this.warnCheck.TabIndex = 29;
            this.warnCheck.Text = "WARN";
            this.warnCheck.UseVisualStyleBackColor = true;
            // 
            // infoCheck
            // 
            this.infoCheck.AutoSize = true;
            this.infoCheck.Location = new System.Drawing.Point(74, 84);
            this.infoCheck.Name = "infoCheck";
            this.infoCheck.Size = new System.Drawing.Size(51, 17);
            this.infoCheck.TabIndex = 28;
            this.infoCheck.Text = "INFO";
            this.infoCheck.UseVisualStyleBackColor = true;
            // 
            // debugCheck
            // 
            this.debugCheck.AutoSize = true;
            this.debugCheck.Location = new System.Drawing.Point(74, 60);
            this.debugCheck.Name = "debugCheck";
            this.debugCheck.Size = new System.Drawing.Size(64, 17);
            this.debugCheck.TabIndex = 27;
            this.debugCheck.Text = "DEBUG";
            this.debugCheck.UseVisualStyleBackColor = true;
            // 
            // levelLabel
            // 
            this.levelLabel.AutoSize = true;
            this.levelLabel.Location = new System.Drawing.Point(6, 60);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(39, 13);
            this.levelLabel.TabIndex = 26;
            this.levelLabel.Text = "Level: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(316, 412);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Record count";
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Location = new System.Drawing.Point(395, 413);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(13, 13);
            this.lblRecordCount.TabIndex = 27;
            this.lblRecordCount.Text = "0";
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(316, 443);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(38, 13);
            this.lblPage.TabIndex = 28;
            this.lblPage.Text = "Page: ";
            // 
            // lblPageNumber
            // 
            this.lblPageNumber.AutoSize = true;
            this.lblPageNumber.Location = new System.Drawing.Point(352, 443);
            this.lblPageNumber.Name = "lblPageNumber";
            this.lblPageNumber.Size = new System.Drawing.Size(13, 13);
            this.lblPageNumber.TabIndex = 29;
            this.lblPageNumber.Text = "1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 468);
            this.Controls.Add(this.lblPageNumber);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filterGroup);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.resultListView);
            this.Controls.Add(this.endDateTimePicker);
            this.Controls.Add(this.endDateLabel);
            this.Controls.Add(this.startDateLabel);
            this.Controls.Add(this.tableNameLabel);
            this.Controls.Add(this.tableNameTextBox);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.startDateTimePicker);
            this.Name = "Form1";
            this.Text = " ";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.filterGroup.ResumeLayout(false);
            this.filterGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker startDateTimePicker;
        private System.Windows.Forms.Button searchButton;
        private TextBox tableNameTextBox;
        private Label tableNameLabel;
        private Label startDateLabel;
        private Label endDateLabel;
        private DateTimePicker endDateTimePicker;
        private ListView resultListView;
        private ErrorProvider errorProvider1;
        private Button nextButton;
        private Button prevButton;
        private Label keywordLabel;
        private TextBox keywordText;
        private GroupBox filterGroup;
        private Label levelLabel;
        private CheckBox errorCheck;
        private CheckBox warnCheck;
        private CheckBox infoCheck;
        private CheckBox debugCheck;
        private CheckBox fatalCheck;
        private Label lblRecordCount;
        private Label label1;
        private Label lblPage;
        private Label lblPageNumber;
    }
}

