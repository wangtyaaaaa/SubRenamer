using System;
using System.Windows.Forms;

namespace SubRenamer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.button_path = new System.Windows.Forms.Button();
            this.button_doRename = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.panel_root = new System.Windows.Forms.Panel();
            this.panel_regex = new System.Windows.Forms.Panel();
            this.button_autotransfer = new System.Windows.Forms.Button();
            this.groupBox_sub = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_sub_left = new System.Windows.Forms.TextBox();
            this.label_sub_num = new System.Windows.Forms.Label();
            this.textBox_sub_right = new System.Windows.Forms.TextBox();
            this.groupBox_video = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_video = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_video_right = new System.Windows.Forms.TextBox();
            this.textBox_video_left = new System.Windows.Forms.TextBox();
            this.label_video_num = new System.Windows.Forms.Label();
            this.panel_name = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button_name2 = new System.Windows.Forms.Button();
            this.textBox_subExt = new System.Windows.Forms.TextBox();
            this.textBox_videoExt = new System.Windows.Forms.TextBox();
            this.button_regex_panel = new System.Windows.Forms.Button();
            this.button_resolve = new System.Windows.Forms.Button();
            this.button_redo = new System.Windows.Forms.Button();
            this.panel_filelist = new System.Windows.Forms.Panel();
            this.panel_path = new System.Windows.Forms.Panel();
            this.label_delimiter = new System.Windows.Forms.Label();
            this.textBox_delimiter = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1.SuspendLayout();
            this.panel_root.SuspendLayout();
            this.panel_regex.SuspendLayout();
            this.groupBox_sub.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox_video.SuspendLayout();
            this.tableLayoutPanel_video.SuspendLayout();
            this.panel_name.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel_path.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_path
            // 
            resources.ApplyResources(this.textBox_path, "textBox_path");
            this.textBox_path.Name = "textBox_path";
            // 
            // button_path
            // 
            resources.ApplyResources(this.button_path, "button_path");
            this.button_path.Name = "button_path";
            this.button_path.UseVisualStyleBackColor = true;
            this.button_path.Click += new System.EventHandler(this.Button_path_Click);
            // 
            // button_doRename
            // 
            resources.ApplyResources(this.button_doRename, "button_doRename");
            this.button_doRename.Name = "button_doRename";
            this.button_doRename.UseVisualStyleBackColor = true;
            this.button_doRename.Click += new System.EventHandler(this.Button_doRename_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.AutoToolTip = true;
            this.toolStripStatusLabel1.IsLink = true;
            this.toolStripStatusLabel1.Margin = new System.Windows.Forms.Padding(11, 3, 0, 2);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripProgressBar1, "toolStripProgressBar1");
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            // 
            // panel_root
            // 
            resources.ApplyResources(this.panel_root, "panel_root");
            this.panel_root.Controls.Add(this.panel_regex);
            this.panel_root.Controls.Add(this.panel_name);
            this.panel_root.Controls.Add(this.panel_path);
            this.panel_root.Name = "panel_root";
            // 
            // panel_regex
            // 
            resources.ApplyResources(this.panel_regex, "panel_regex");
            this.panel_regex.Controls.Add(this.button_autotransfer);
            this.panel_regex.Controls.Add(this.groupBox_sub);
            this.panel_regex.Controls.Add(this.groupBox_video);
            this.panel_regex.Name = "panel_regex";
            // 
            // button_autotransfer
            // 
            resources.ApplyResources(this.button_autotransfer, "button_autotransfer");
            this.button_autotransfer.Name = "button_autotransfer";
            this.button_autotransfer.UseVisualStyleBackColor = true;
            this.button_autotransfer.Click += new System.EventHandler(this.Button_Autotransfer_Click);
            // 
            // groupBox_sub
            // 
            resources.ApplyResources(this.groupBox_sub, "groupBox_sub");
            this.groupBox_sub.Controls.Add(this.tableLayoutPanel1);
            this.groupBox_sub.Name = "groupBox_sub";
            this.groupBox_sub.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.textBox_sub_left, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_sub_num, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_sub_right, 2, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // textBox_sub_left
            // 
            resources.ApplyResources(this.textBox_sub_left, "textBox_sub_left");
            this.textBox_sub_left.Name = "textBox_sub_left";
            // 
            // label_sub_num
            // 
            resources.ApplyResources(this.label_sub_num, "label_sub_num");
            this.label_sub_num.Name = "label_sub_num";
            // 
            // textBox_sub_right
            // 
            resources.ApplyResources(this.textBox_sub_right, "textBox_sub_right");
            this.textBox_sub_right.Name = "textBox_sub_right";
            // 
            // groupBox_video
            // 
            resources.ApplyResources(this.groupBox_video, "groupBox_video");
            this.groupBox_video.Controls.Add(this.tableLayoutPanel_video);
            this.groupBox_video.Name = "groupBox_video";
            this.groupBox_video.TabStop = false;
            // 
            // tableLayoutPanel_video
            // 
            resources.ApplyResources(this.tableLayoutPanel_video, "tableLayoutPanel_video");
            this.tableLayoutPanel_video.Controls.Add(this.textBox_video_right, 2, 0);
            this.tableLayoutPanel_video.Controls.Add(this.textBox_video_left, 0, 0);
            this.tableLayoutPanel_video.Controls.Add(this.label_video_num, 1, 0);
            this.tableLayoutPanel_video.Name = "tableLayoutPanel_video";
            // 
            // textBox_video_right
            // 
            resources.ApplyResources(this.textBox_video_right, "textBox_video_right");
            this.textBox_video_right.Name = "textBox_video_right";
            // 
            // textBox_video_left
            // 
            resources.ApplyResources(this.textBox_video_left, "textBox_video_left");
            this.textBox_video_left.Name = "textBox_video_left";
            // 
            // label_video_num
            // 
            resources.ApplyResources(this.label_video_num, "label_video_num");
            this.label_video_num.Name = "label_video_num";
            // 
            // panel_name
            // 
            resources.ApplyResources(this.panel_name, "panel_name");
            this.panel_name.Controls.Add(this.tableLayoutPanel2);
            this.panel_name.Controls.Add(this.panel_filelist);
            this.panel_name.Name = "panel_name";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.button_name2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox_subExt, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_doRename, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox_videoExt, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_regex_panel, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_resolve, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_redo, 4, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // button_name2
            // 
            resources.ApplyResources(this.button_name2, "button_name2");
            this.button_name2.Name = "button_name2";
            this.button_name2.UseVisualStyleBackColor = true;
            this.button_name2.Click += new System.EventHandler(this.Button_name2_Click);
            // 
            // textBox_subExt
            // 
            resources.ApplyResources(this.textBox_subExt, "textBox_subExt");
            this.textBox_subExt.Name = "textBox_subExt";
            this.toolTip1.SetToolTip(this.textBox_subExt, resources.GetString("textBox_subExt.ToolTip"));
            this.textBox_subExt.TextChanged += new System.EventHandler(this.TextBox_subExt_TextChanged);
            // 
            // textBox_videoExt
            // 
            resources.ApplyResources(this.textBox_videoExt, "textBox_videoExt");
            this.textBox_videoExt.Name = "textBox_videoExt";
            this.toolTip1.SetToolTip(this.textBox_videoExt, resources.GetString("textBox_videoExt.ToolTip"));
            this.textBox_videoExt.TextChanged += new System.EventHandler(this.TextBox_videoExt_TextChanged);
            // 
            // button_regex_panel
            // 
            resources.ApplyResources(this.button_regex_panel, "button_regex_panel");
            this.button_regex_panel.Name = "button_regex_panel";
            this.button_regex_panel.UseVisualStyleBackColor = true;
            this.button_regex_panel.Click += new System.EventHandler(this.Button_regex_panel_Click_1);
            // 
            // button_resolve
            // 
            resources.ApplyResources(this.button_resolve, "button_resolve");
            this.button_resolve.Name = "button_resolve";
            this.button_resolve.UseVisualStyleBackColor = true;
            this.button_resolve.Click += new System.EventHandler(this.Button_Resolve_Click);
            // 
            // button_redo
            // 
            resources.ApplyResources(this.button_redo, "button_redo");
            this.button_redo.Name = "button_redo";
            this.button_redo.UseVisualStyleBackColor = true;
            this.button_redo.Click += new System.EventHandler(this.Button_Redo_Click);
            // 
            // panel_filelist
            // 
            resources.ApplyResources(this.panel_filelist, "panel_filelist");
            this.panel_filelist.BackColor = System.Drawing.SystemColors.Window;
            this.panel_filelist.Name = "panel_filelist";
            this.panel_filelist.Resize += new System.EventHandler(this.Panel1_resize);
            // 
            // panel_path
            // 
            resources.ApplyResources(this.panel_path, "panel_path");
            this.panel_path.Controls.Add(this.label_delimiter);
            this.panel_path.Controls.Add(this.textBox_delimiter);
            this.panel_path.Controls.Add(this.button_path);
            this.panel_path.Controls.Add(this.textBox_path);
            this.panel_path.Name = "panel_path";
            // 
            // label_delimiter
            // 
            resources.ApplyResources(this.label_delimiter, "label_delimiter");
            this.label_delimiter.Name = "label_delimiter";
            // 
            // textBox_delimiter
            // 
            resources.ApplyResources(this.textBox_delimiter, "textBox_delimiter");
            this.textBox_delimiter.Name = "textBox_delimiter";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel_root);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel_root.ResumeLayout(false);
            this.panel_regex.ResumeLayout(false);
            this.groupBox_sub.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox_video.ResumeLayout(false);
            this.tableLayoutPanel_video.ResumeLayout(false);
            this.tableLayoutPanel_video.PerformLayout();
            this.panel_name.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel_path.ResumeLayout(false);
            this.panel_path.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }




        #endregion

        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.Button button_path;
        private System.Windows.Forms.Button button_doRename;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Panel panel_root;
        private System.Windows.Forms.Panel panel_path;
        private System.Windows.Forms.Panel panel_name;
        private System.Windows.Forms.Panel panel_regex;
        private System.Windows.Forms.GroupBox groupBox_sub;
        private System.Windows.Forms.GroupBox groupBox_video;
        private System.Windows.Forms.TextBox textBox_sub_right;
        private System.Windows.Forms.Label label_sub_num;
        private System.Windows.Forms.TextBox textBox_sub_left;
        private System.Windows.Forms.Button button_regex_panel;
        private System.Windows.Forms.Button button_autotransfer;
        private System.Windows.Forms.Button button_name2;
        private System.Windows.Forms.Button button_resolve;
        private System.Windows.Forms.Button button_redo;
        private System.Windows.Forms.Panel panel_filelist;
        private System.Windows.Forms.TextBox textBox_subExt;
        private System.Windows.Forms.TextBox textBox_videoExt;
        private System.Windows.Forms.ToolTip toolTip1;
        private Label label_delimiter;
        private TextBox textBox_delimiter;
        private TableLayoutPanel tableLayoutPanel_video;
        private TextBox textBox_video_left;
        private Label label_video_num;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textBox_video_right;
        private TableLayoutPanel tableLayoutPanel2;
    }
}

