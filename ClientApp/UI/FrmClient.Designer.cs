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
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblLogHeader = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.SuspendLayout();

            // ── btnConnect ───────────────────────────────────────────────────
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.btnConnect.Location = new System.Drawing.Point(343, 21);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 42);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // ── btnDisconnect ────────────────────────────────────────────────
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
            this.btnDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.btnDisconnect.Location = new System.Drawing.Point(458, 21);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(110, 42);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);

            // ── lblIP ────────────────────────────────────────────────────────
            this.lblIP.AutoSize = true;
            this.lblIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F,
                                        System.Drawing.FontStyle.Bold);
            this.lblIP.Location = new System.Drawing.Point(25, 36);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(30, 20);
            this.lblIP.Text = "IP";

            // ── txtIP ────────────────────────────────────────────────────────
            this.txtIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtIP.Location = new System.Drawing.Point(90, 33);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(160, 27);
            this.txtIP.TabIndex = 2;
            this.txtIP.Text = "127.0.0.1";

            // ── lblPort ──────────────────────────────────────────────────────
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F,
                                          System.Drawing.FontStyle.Bold);
            this.lblPort.Location = new System.Drawing.Point(265, 36);
            this.lblPort.Name = "lblPort";
            this.lblPort.Text = "Port";

            // ── txtPort ──────────────────────────────────────────────────────
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.txtPort.Location = new System.Drawing.Point(315, 33);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(80, 27);  // ngắn hơn txtIP vì chỉ cần port
            this.txtPort.TabIndex = 3;

            // ── lblLogHeader ─────────────────────────────────────────────────
            this.lblLogHeader.AutoSize = true;
            this.lblLogHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F,
                                               System.Drawing.FontStyle.Bold);
            this.lblLogHeader.ForeColor = System.Drawing.Color.DimGray;
            this.lblLogHeader.Location = new System.Drawing.Point(25, 83);
            this.lblLogHeader.Name = "lblLogHeader";
            this.lblLogHeader.Text = "TIME        ACTION      FILE NAME";

            // ── lstLog ───────────────────────────────────────────────────────
            this.lstLog.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 18;
            this.lstLog.Location = new System.Drawing.Point(25, 105);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(550, 234);
            this.lstLog.TabIndex = 4;
            this.lstLog.HorizontalScrollbar = true;

            // ── btnClear ─────────────────────────────────────────────────────
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.btnClear.Location = new System.Drawing.Point(509, 350);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(66, 30);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            // ── lblStatus ────────────────────────────────────────────────────
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(22, 357);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Waiting...";

            // ── FrmClient ────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Text = "Client Monitor";
            this.Name = "FrmClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblLogHeader);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblStatus);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblLogHeader;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstLog;
    }
}