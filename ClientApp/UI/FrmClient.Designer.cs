namespace ClientApp.UI
{
    partial class FrmClient
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnLoadHistory = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblLogHeader = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstLog = new System.Windows.Forms.ListBox();
            // Alert controls
            this.grpAlert = new System.Windows.Forms.GroupBox();
            this.chkAlertEnable = new System.Windows.Forms.CheckBox();
            this.chkAlertCreated = new System.Windows.Forms.CheckBox();
            this.chkAlertDeleted = new System.Windows.Forms.CheckBox();
            this.chkAlertModified = new System.Windows.Forms.CheckBox();
            this.chkAlertRenamed = new System.Windows.Forms.CheckBox();
            this.grpAlert.SuspendLayout();
            this.SuspendLayout();

            // ── btnConnect ───────────────────────────────────────────────────
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.btnConnect.Location = new System.Drawing.Point(465, 9);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(110, 42);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // ── btnDisconnect ────────────────────────────────────────────────
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
            this.btnDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.btnDisconnect.Location = new System.Drawing.Point(465, 57);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(110, 42);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);

            // ── txtIP ────────────────────────────────────────────────────────
            this.txtIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtIP.Location = new System.Drawing.Point(90, 33);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(160, 27);
            this.txtIP.TabIndex = 2;
            this.txtIP.Text = "127.0.0.1";

            // ── txtPort ──────────────────────────────────────────────────────
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtPort.Location = new System.Drawing.Point(334, 33);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(110, 27);
            this.txtPort.TabIndex = 3;

            // ── lblIP ────────────────────────────────────────────────────────
            this.lblIP.AutoSize = true;
            this.lblIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblIP.Location = new System.Drawing.Point(25, 36);
            this.lblIP.Name = "lblIP";
            this.lblIP.Text = "IP";

            // ── lblPort ──────────────────────────────────────────────────────
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblPort.Location = new System.Drawing.Point(284, 36);
            this.lblPort.Name = "lblPort";
            this.lblPort.Text = "Port";

            // ── lblLogHeader ─────────────────────────────────────────────────
            this.lblLogHeader.AutoSize = true;
            this.lblLogHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblLogHeader.ForeColor = System.Drawing.Color.DimGray;
            this.lblLogHeader.Location = new System.Drawing.Point(25, 108);
            this.lblLogHeader.Name = "lblLogHeader";
            this.lblLogHeader.Text = "TIME        ACTION      FILE NAME";

            // ── lstLog ───────────────────────────────────────────────────────
            this.lstLog.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.ItemHeight = 19;
            this.lstLog.Location = new System.Drawing.Point(25, 130);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(550, 190);
            this.lstLog.TabIndex = 4;

            // ── grpAlert (GroupBox cảnh báo) ─────────────────────────────────
            this.grpAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.grpAlert.Location = new System.Drawing.Point(25, 330);
            this.grpAlert.Name = "grpAlert";
            this.grpAlert.Size = new System.Drawing.Size(550, 55);
            this.grpAlert.TabIndex = 10;
            this.grpAlert.TabStop = false;
            this.grpAlert.Text = "Cảnh báo";

            // chkAlertEnable — master toggle
            this.chkAlertEnable.AutoSize = true;
            this.chkAlertEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.chkAlertEnable.ForeColor = System.Drawing.Color.DarkRed;
            this.chkAlertEnable.Location = new System.Drawing.Point(10, 25);
            this.chkAlertEnable.Name = "chkAlertEnable";
            this.chkAlertEnable.Text = "Bật";
            this.chkAlertEnable.CheckedChanged += new System.EventHandler(this.chkAlertEnable_CheckedChanged);

            // chkAlertCreated
            this.chkAlertCreated.AutoSize = true;
            this.chkAlertCreated.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.chkAlertCreated.Location = new System.Drawing.Point(75, 25);
            this.chkAlertCreated.Name = "chkAlertCreated";
            this.chkAlertCreated.Text = "[+] Created";
            this.chkAlertCreated.CheckedChanged += new System.EventHandler(this.chkAlertCreated_CheckedChanged);

            // chkAlertDeleted
            this.chkAlertDeleted.AutoSize = true;
            this.chkAlertDeleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.chkAlertDeleted.Location = new System.Drawing.Point(185, 25);
            this.chkAlertDeleted.Name = "chkAlertDeleted";
            this.chkAlertDeleted.Text = "[-] Deleted";
            this.chkAlertDeleted.CheckedChanged += new System.EventHandler(this.chkAlertDeleted_CheckedChanged);

            // chkAlertModified
            this.chkAlertModified.AutoSize = true;
            this.chkAlertModified.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.chkAlertModified.Location = new System.Drawing.Point(295, 25);
            this.chkAlertModified.Name = "chkAlertModified";
            this.chkAlertModified.Text = "[~] Modified";
            this.chkAlertModified.CheckedChanged += new System.EventHandler(this.chkAlertModified_CheckedChanged);

            // chkAlertRenamed
            this.chkAlertRenamed.AutoSize = true;
            this.chkAlertRenamed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.chkAlertRenamed.Location = new System.Drawing.Point(415, 25);
            this.chkAlertRenamed.Name = "chkAlertRenamed";
            this.chkAlertRenamed.Text = "[→] Renamed";
            this.chkAlertRenamed.CheckedChanged += new System.EventHandler(this.chkAlertRenamed_CheckedChanged);

            this.grpAlert.Controls.Add(this.chkAlertEnable);
            this.grpAlert.Controls.Add(this.chkAlertCreated);
            this.grpAlert.Controls.Add(this.chkAlertDeleted);
            this.grpAlert.Controls.Add(this.chkAlertModified);
            this.grpAlert.Controls.Add(this.chkAlertRenamed);

            // ── btnLoadHistory ────────────────────────────────────────────────
            this.btnLoadHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.btnLoadHistory.Location = new System.Drawing.Point(330, 396);
            this.btnLoadHistory.Name = "btnLoadHistory";
            this.btnLoadHistory.Size = new System.Drawing.Size(120, 30);
            this.btnLoadHistory.TabIndex = 7;
            this.btnLoadHistory.Text = "Load History";
            this.btnLoadHistory.UseVisualStyleBackColor = true;
            this.btnLoadHistory.Click += new System.EventHandler(this.btnLoadHistory_Click);

            // ── btnClear ─────────────────────────────────────────────────────
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.btnClear.Location = new System.Drawing.Point(460, 396);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 30);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            // ── lblStatus ────────────────────────────────────────────────────
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(22, 402);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Waiting...";

            // ── FrmClient ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 440);
            this.Name = "FrmClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client Monitor";

            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblLogHeader);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.grpAlert);
            this.Controls.Add(this.btnLoadHistory);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblStatus);

            this.grpAlert.ResumeLayout(false);
            this.grpAlert.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnLoadHistory;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblLogHeader;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.GroupBox grpAlert;
        private System.Windows.Forms.CheckBox chkAlertEnable;
        private System.Windows.Forms.CheckBox chkAlertCreated;
        private System.Windows.Forms.CheckBox chkAlertDeleted;
        private System.Windows.Forms.CheckBox chkAlertModified;
        private System.Windows.Forms.CheckBox chkAlertRenamed;
    }
}