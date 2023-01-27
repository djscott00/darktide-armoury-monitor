
namespace Darktide_Armoury_Monitor
{
    partial class MainForm
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
            if (disposing && (components != null)) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_results = new System.Windows.Forms.TextBox();
            this.chk_qcMode = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_totalChecks = new System.Windows.Forms.Label();
            this.lbl_totalChecksWithHits = new System.Windows.Forms.Label();
            this.lbl_totalHits = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chk_pushToLogSnag = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lbl_nextRunTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(259, 301);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 39);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Location = new System.Drawing.Point(380, 301);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(75, 39);
            this.btn_stop.TabIndex = 1;
            this.btn_stop.Text = "Stop";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Results Log:";
            // 
            // txt_results
            // 
            this.txt_results.BackColor = System.Drawing.SystemColors.Window;
            this.txt_results.Location = new System.Drawing.Point(110, 85);
            this.txt_results.Multiline = true;
            this.txt_results.Name = "txt_results";
            this.txt_results.ReadOnly = true;
            this.txt_results.Size = new System.Drawing.Size(556, 120);
            this.txt_results.TabIndex = 3;
            // 
            // chk_qcMode
            // 
            this.chk_qcMode.AutoSize = true;
            this.chk_qcMode.Location = new System.Drawing.Point(110, 259);
            this.chk_qcMode.Name = "chk_qcMode";
            this.chk_qcMode.Size = new System.Drawing.Size(71, 17);
            this.chk_qcMode.TabIndex = 4;
            this.chk_qcMode.Text = "QC Mode";
            this.toolTip1.SetToolTip(this.chk_qcMode, "If checked, will not run headless.");
            this.chk_qcMode.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Total Checks:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Total Checks with Hits:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(535, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Total Hits:";
            // 
            // lbl_totalChecks
            // 
            this.lbl_totalChecks.AutoSize = true;
            this.lbl_totalChecks.Location = new System.Drawing.Point(186, 20);
            this.lbl_totalChecks.Name = "lbl_totalChecks";
            this.lbl_totalChecks.Size = new System.Drawing.Size(13, 13);
            this.lbl_totalChecks.TabIndex = 8;
            this.lbl_totalChecks.Text = "0";
            // 
            // lbl_totalChecksWithHits
            // 
            this.lbl_totalChecksWithHits.AutoSize = true;
            this.lbl_totalChecksWithHits.Location = new System.Drawing.Point(429, 20);
            this.lbl_totalChecksWithHits.Name = "lbl_totalChecksWithHits";
            this.lbl_totalChecksWithHits.Size = new System.Drawing.Size(13, 13);
            this.lbl_totalChecksWithHits.TabIndex = 9;
            this.lbl_totalChecksWithHits.Text = "0";
            // 
            // lbl_totalHits
            // 
            this.lbl_totalHits.AutoSize = true;
            this.lbl_totalHits.Location = new System.Drawing.Point(596, 20);
            this.lbl_totalHits.Name = "lbl_totalHits";
            this.lbl_totalHits.Size = new System.Drawing.Size(13, 13);
            this.lbl_totalHits.TabIndex = 10;
            this.lbl_totalHits.Text = "0";
            // 
            // chk_pushToLogSnag
            // 
            this.chk_pushToLogSnag.AutoSize = true;
            this.chk_pushToLogSnag.Checked = true;
            this.chk_pushToLogSnag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_pushToLogSnag.Location = new System.Drawing.Point(110, 226);
            this.chk_pushToLogSnag.Name = "chk_pushToLogSnag";
            this.chk_pushToLogSnag.Size = new System.Drawing.Size(169, 17);
            this.chk_pushToLogSnag.TabIndex = 11;
            this.chk_pushToLogSnag.Text = "Push Notifications to LogSnag";
            this.toolTip1.SetToolTip(this.chk_pushToLogSnag, "If checked, will push notifications your Log Snag channel (see config.xml for inp" +
        "uts)");
            this.chk_pushToLogSnag.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(257, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Next Run Time:";
            // 
            // lbl_nextRunTime
            // 
            this.lbl_nextRunTime.AutoSize = true;
            this.lbl_nextRunTime.Location = new System.Drawing.Point(344, 58);
            this.lbl_nextRunTime.Name = "lbl_nextRunTime";
            this.lbl_nextRunTime.Size = new System.Drawing.Size(13, 13);
            this.lbl_nextRunTime.TabIndex = 13;
            this.lbl_nextRunTime.Text = "0";
            this.lbl_nextRunTime.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 371);
            this.Controls.Add(this.lbl_nextRunTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chk_pushToLogSnag);
            this.Controls.Add(this.lbl_totalHits);
            this.Controls.Add(this.lbl_totalChecksWithHits);
            this.Controls.Add(this.lbl_totalChecks);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chk_qcMode);
            this.Controls.Add(this.txt_results);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.btn_start);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Darktide Armoury Scraper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_results;
        private System.Windows.Forms.CheckBox chk_qcMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_totalChecks;
        private System.Windows.Forms.Label lbl_totalChecksWithHits;
        private System.Windows.Forms.Label lbl_totalHits;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chk_pushToLogSnag;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_nextRunTime;
    }
}

