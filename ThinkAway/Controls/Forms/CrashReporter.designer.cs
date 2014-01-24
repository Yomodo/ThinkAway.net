namespace ThinkAway.Controls.Forms
{
    partial class CrashReporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashReporter));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBoxErr = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkHelper = new System.Windows.Forms.CheckBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.lnkWeb = new System.Windows.Forms.LinkLabel();
            this.lnkHelper = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxErr)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Controls.Add(this.pictureBoxErr);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(375, 102);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "抱歉,程序运行时出现异常.";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 56);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(363, 40);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // pictureBoxErr
            // 
            this.pictureBoxErr.Location = new System.Drawing.Point(6, 20);
            this.pictureBoxErr.Name = "pictureBoxErr";
            this.pictureBoxErr.Size = new System.Drawing.Size(32, 30);
            this.pictureBoxErr.TabIndex = 7;
            this.pictureBoxErr.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1025, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "An exception occured in a component of Little Disk Cleaner. We have created an er" +
    "ror report that you can send us. We will treat this report as confidential and a" +
    "nonymous.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "程序名称:{0}";
            // 
            // btnReport
            // 
            this.btnReport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnReport.Location = new System.Drawing.Point(235, 120);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(72, 23);
            this.btnReport.TabIndex = 2;
            this.btnReport.Text = "提交(&R)";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(313, 120);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "关闭(&C)";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.buttonDontSend_Click);
            // 
            // chkHelper
            // 
            this.chkHelper.AutoSize = true;
            this.chkHelper.Location = new System.Drawing.Point(10, 123);
            this.chkHelper.Name = "chkHelper";
            this.chkHelper.Size = new System.Drawing.Size(216, 16);
            this.chkHelper.TabIndex = 3;
            this.chkHelper.Text = "我愿意帮助程序作者发现并解决问题";
            this.chkHelper.UseVisualStyleBackColor = true;
            this.chkHelper.CheckedChanged += new System.EventHandler(this.ChkHelperCheckedChanged);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 149);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(375, 66);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // lnkWeb
            // 
            this.lnkWeb.AutoSize = true;
            this.lnkWeb.Location = new System.Drawing.Point(14, 224);
            this.lnkWeb.Name = "lnkWeb";
            this.lnkWeb.Size = new System.Drawing.Size(53, 12);
            this.lnkWeb.TabIndex = 5;
            this.lnkWeb.TabStop = true;
            this.lnkWeb.Text = "官方网站";
            // 
            // lnkHelper
            // 
            this.lnkHelper.AutoSize = true;
            this.lnkHelper.Location = new System.Drawing.Point(73, 224);
            this.lnkHelper.Name = "lnkHelper";
            this.lnkHelper.Size = new System.Drawing.Size(53, 12);
            this.lnkHelper.TabIndex = 5;
            this.lnkHelper.TabStop = true;
            this.lnkHelper.Text = "寻求帮助";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(334, 224);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(53, 12);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "隐私条款";
            // 
            // CrashReporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 248);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lnkHelper);
            this.Controls.Add(this.lnkWeb);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.chkHelper);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 276);
            this.MinimumSize = new System.Drawing.Size(400, 176);
            this.Name = "CrashReporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BugReport";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ErrorDlg_FormClosed);
            this.Load += new System.EventHandler(this.BugReportLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxErr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkHelper;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.LinkLabel lnkWeb;
        private System.Windows.Forms.LinkLabel lnkHelper;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.PictureBox pictureBoxErr;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}