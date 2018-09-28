namespace WindowsFormsApplication2
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.button_path = new System.Windows.Forms.Button();
            this.button_name = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button_doRename = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel_regex = new System.Windows.Forms.Panel();
            this.groupBox_sub = new System.Windows.Forms.GroupBox();
            this.textBox_sub_right = new System.Windows.Forms.TextBox();
            this.label_sub_num = new System.Windows.Forms.Label();
            this.textBox_sub_left = new System.Windows.Forms.TextBox();
            this.groupBox_video = new System.Windows.Forms.GroupBox();
            this.textBox_video_right = new System.Windows.Forms.TextBox();
            this.label_video_num = new System.Windows.Forms.Label();
            this.textBox_video_left = new System.Windows.Forms.TextBox();
            this.panel_name = new System.Windows.Forms.Panel();
            this.button_regex_panel = new System.Windows.Forms.Button();
            this.panel_path = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_regex.SuspendLayout();
            this.groupBox_sub.SuspendLayout();
            this.groupBox_video.SuspendLayout();
            this.panel_name.SuspendLayout();
            this.panel_path.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_path
            // 
            this.textBox_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_path.Location = new System.Drawing.Point(6, 6);
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.Size = new System.Drawing.Size(282, 21);
            this.textBox_path.TabIndex = 0;
            // 
            // button_path
            // 
            this.button_path.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_path.Location = new System.Drawing.Point(294, 5);
            this.button_path.Name = "button_path";
            this.button_path.Size = new System.Drawing.Size(75, 23);
            this.button_path.TabIndex = 1;
            this.button_path.Text = "选择路径";
            this.button_path.UseVisualStyleBackColor = true;
            this.button_path.Click += new System.EventHandler(this.button_path_Click);
            // 
            // button_name
            // 
            this.button_name.Location = new System.Drawing.Point(6, 6);
            this.button_name.Name = "button_name";
            this.button_name.Size = new System.Drawing.Size(75, 23);
            this.button_name.TabIndex = 2;
            this.button_name.Text = "获取文件";
            this.button_name.UseVisualStyleBackColor = true;
            this.button_name.Click += new System.EventHandler(this.button_name_Click);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(6, 33);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(363, 115);
            this.treeView1.TabIndex = 3;
            // 
            // button_doRename
            // 
            this.button_doRename.Location = new System.Drawing.Point(87, 6);
            this.button_doRename.Name = "button_doRename";
            this.button_doRename.Size = new System.Drawing.Size(115, 23);
            this.button_doRename.TabIndex = 4;
            this.button_doRename.Text = "修改字幕文件名";
            this.button_doRename.UseVisualStyleBackColor = true;
            this.button_doRename.Click += new System.EventHandler(this.button_doRename_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 306);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(387, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.AutoToolTip = true;
            this.toolStripStatusLabel1.IsLink = true;
            this.toolStripStatusLabel1.Margin = new System.Windows.Forms.Padding(11, 3, 0, 2);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(259, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.panel_regex);
            this.panel1.Controls.Add(this.panel_name);
            this.panel1.Controls.Add(this.panel_path);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 328);
            this.panel1.TabIndex = 6;
            // 
            // panel_regex
            // 
            this.panel_regex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_regex.Controls.Add(this.groupBox_sub);
            this.panel_regex.Controls.Add(this.groupBox_video);
            this.panel_regex.Location = new System.Drawing.Point(6, 46);
            this.panel_regex.Name = "panel_regex";
            this.panel_regex.Size = new System.Drawing.Size(375, 100);
            this.panel_regex.TabIndex = 6;
            // 
            // groupBox_sub
            // 
            this.groupBox_sub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_sub.Controls.Add(this.textBox_sub_right);
            this.groupBox_sub.Controls.Add(this.label_sub_num);
            this.groupBox_sub.Controls.Add(this.textBox_sub_left);
            this.groupBox_sub.Location = new System.Drawing.Point(0, 50);
            this.groupBox_sub.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox_sub.Name = "groupBox_sub";
            this.groupBox_sub.Size = new System.Drawing.Size(375, 50);
            this.groupBox_sub.TabIndex = 1;
            this.groupBox_sub.TabStop = false;
            this.groupBox_sub.Text = "字幕";
            // 
            // textBox_sub_right
            // 
            this.textBox_sub_right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sub_right.Location = new System.Drawing.Point(208, 21);
            this.textBox_sub_right.Name = "textBox_sub_right";
            this.textBox_sub_right.Size = new System.Drawing.Size(161, 21);
            this.textBox_sub_right.TabIndex = 5;
            // 
            // label_sub_num
            // 
            this.label_sub_num.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label_sub_num.AutoSize = true;
            this.label_sub_num.Location = new System.Drawing.Point(173, 24);
            this.label_sub_num.Name = "label_sub_num";
            this.label_sub_num.Size = new System.Drawing.Size(29, 12);
            this.label_sub_num.TabIndex = 4;
            this.label_sub_num.Text = "集号";
            // 
            // textBox_sub_left
            // 
            this.textBox_sub_left.Location = new System.Drawing.Point(6, 21);
            this.textBox_sub_left.Name = "textBox_sub_left";
            this.textBox_sub_left.Size = new System.Drawing.Size(160, 21);
            this.textBox_sub_left.TabIndex = 3;
            // 
            // groupBox_video
            // 
            this.groupBox_video.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_video.Controls.Add(this.textBox_video_right);
            this.groupBox_video.Controls.Add(this.label_video_num);
            this.groupBox_video.Controls.Add(this.textBox_video_left);
            this.groupBox_video.Location = new System.Drawing.Point(0, 0);
            this.groupBox_video.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox_video.Name = "groupBox_video";
            this.groupBox_video.Size = new System.Drawing.Size(375, 50);
            this.groupBox_video.TabIndex = 0;
            this.groupBox_video.TabStop = false;
            this.groupBox_video.Text = "视频";
            // 
            // textBox_video_right
            // 
            this.textBox_video_right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_video_right.Location = new System.Drawing.Point(208, 21);
            this.textBox_video_right.Name = "textBox_video_right";
            this.textBox_video_right.Size = new System.Drawing.Size(161, 21);
            this.textBox_video_right.TabIndex = 2;
            // 
            // label_video_num
            // 
            this.label_video_num.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label_video_num.AutoSize = true;
            this.label_video_num.Location = new System.Drawing.Point(173, 24);
            this.label_video_num.Name = "label_video_num";
            this.label_video_num.Size = new System.Drawing.Size(29, 12);
            this.label_video_num.TabIndex = 1;
            this.label_video_num.Text = "集号";
            // 
            // textBox_video_left
            // 
            this.textBox_video_left.Location = new System.Drawing.Point(6, 21);
            this.textBox_video_left.Name = "textBox_video_left";
            this.textBox_video_left.Size = new System.Drawing.Size(160, 21);
            this.textBox_video_left.TabIndex = 0;
            // 
            // panel_name
            // 
            this.panel_name.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_name.Controls.Add(this.button_regex_panel);
            this.panel_name.Controls.Add(this.button_name);
            this.panel_name.Controls.Add(this.treeView1);
            this.panel_name.Controls.Add(this.button_doRename);
            this.panel_name.Location = new System.Drawing.Point(6, 152);
            this.panel_name.Name = "panel_name";
            this.panel_name.Size = new System.Drawing.Size(375, 151);
            this.panel_name.TabIndex = 5;
            // 
            // button_regex_panel
            // 
            this.button_regex_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_regex_panel.Location = new System.Drawing.Point(329, 4);
            this.button_regex_panel.Name = "button_regex_panel";
            this.button_regex_panel.Size = new System.Drawing.Size(23, 23);
            this.button_regex_panel.TabIndex = 5;
            this.button_regex_panel.Text = "↓";
            this.button_regex_panel.UseVisualStyleBackColor = true;
            this.button_regex_panel.Click += new System.EventHandler(this.button_regex_panel_Click_1);
            // 
            // panel_path
            // 
            this.panel_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_path.Controls.Add(this.button_path);
            this.panel_path.Controls.Add(this.textBox_path);
            this.panel_path.Location = new System.Drawing.Point(6, 6);
            this.panel_path.Name = "panel_path";
            this.panel_path.Size = new System.Drawing.Size(375, 33);
            this.panel_path.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(387, 328);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "SubRenamer";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel_regex.ResumeLayout(false);
            this.groupBox_sub.ResumeLayout(false);
            this.groupBox_sub.PerformLayout();
            this.groupBox_video.ResumeLayout(false);
            this.groupBox_video.PerformLayout();
            this.panel_name.ResumeLayout(false);
            this.panel_path.ResumeLayout(false);
            this.panel_path.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.Button button_path;
        private System.Windows.Forms.Button button_name;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button_doRename;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel_path;
        private System.Windows.Forms.Panel panel_name;
        private System.Windows.Forms.Panel panel_regex;
        private System.Windows.Forms.GroupBox groupBox_sub;
        private System.Windows.Forms.GroupBox groupBox_video;
        private System.Windows.Forms.TextBox textBox_video_left;
        private System.Windows.Forms.TextBox textBox_sub_right;
        private System.Windows.Forms.Label label_sub_num;
        private System.Windows.Forms.TextBox textBox_sub_left;
        private System.Windows.Forms.TextBox textBox_video_right;
        private System.Windows.Forms.Label label_video_num;
        private System.Windows.Forms.Button button_regex_panel;
    }
}

