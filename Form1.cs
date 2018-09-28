using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 重命名工作结束时返回的表记
        /// </summary>
        static string END_MSG = "end";
        /// <summary>
        /// Names的根节点
        /// </summary>
        Names names = null;
        /// <summary>
        /// 是否确认过提示信息
        /// </summary>
        bool ischecked = false;
        /// <summary>
        /// lable "集号" 的宽度，在Form1初始化时获取
        /// </summary>
        int lable_num_width;
        /// <summary>
        /// Form1初始化，隐藏panel_regex，设置lable_num_width
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            this.textBox_path.Text = System.Environment.CurrentDirectory;
            //this.textBox1.Text = "D:\\aaa\\aaa";
            setPanelRegexVisible(false);
            lable_num_width = label_video_num.Width ;
        }

        /// <summary>
        /// 显示/隐藏 panel_regex
        /// </summary>
        /// <param name="visible"></param>
        void setPanelRegexVisible(bool visible)
        {
            if (visible)
            {
                panel_regex.Visible = true;//设置可见
                panel_name.Top = panel_regex.Bottom;//设置panel_name的顶部位置
                panel_name.Height = statusStrip1.Top - panel_name.Top;//设置panel_name的高度
                reset_regex_size();
            }
            else
            {
                panel_regex.Visible = false;
                panel_name.Top = panel_path.Bottom;
                panel_name.Height = statusStrip1.Top - panel_name.Top;
            }
        }
        /// <summary>
        /// 开启选择路径的对话框，选择完后设置到textBox_path里
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_path_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.textBox_path.Text = fbd.SelectedPath;
            }
        }
        /// <summary>
        /// 按textBox_path里的路径获取文件名，生成Names对象。
        /// panel_regex.Visible是true，则会按照指定的正则表达式获取文件名，同时忽略所有子文件夹。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_name_Click(object sender, EventArgs e)
        {
            this.toolStripProgressBar1.Value = 0;
            DirectoryInfo dInfo = new DirectoryInfo(this.textBox_path.Text);
            if (!panel_regex.Visible)
            {
                names = new Names(dInfo);
            }
            else
            {
                names = new Names(dInfo, textBox_video_left.Text, textBox_video_right.Text, textBox_sub_left.Text, textBox_sub_right.Text);
            }
            names.setTreeView(this.treeView1);

        }
        /// <summary>
        /// 对names里的文件名 执行重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_doRename_Click(object sender, EventArgs e)
        {
            if (names == null)
            {
                MessageBox.Show("请先获取文件");
                return;
            }
            if (!ischecked)
            {
                if (!DoCheckMessage())
                {
                    return;
                }
            }
            DoRename();
        }

        private void DoRename()
        {
            //this.toolStripStatusLabel1.Text = "正在运行...";
            setClickable(false);
            this.toolStripProgressBar1.Maximum = names.getVideoCount();
            this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            this.backgroundWorker1.RunWorkerAsync();

            //Renamer.Rename(names);
            //this.maskedTextBox1.Text = "修改完成";
            //this.button2_Click(null, null);
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
            {
                this.toolStripProgressBar1.Value = e.ProgressPercentage;
            }
            object msg = e.UserState;
            if (msg != null )
            {
                if (msg.ToString().Equals(END_MSG))
                {
                    setClickable(true);
                    this.toolStripStatusLabel1.Text = "修改完成";
                    this.button_name_Click(null, null);
                }
                else
                {
                    this.toolStripStatusLabel1.Text = msg.ToString();
                    
                    //this.toolStripStatusLabel1.ToolTipText = this.toolStripStatusLabel1.Text;
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            Renamer.Rename(names,bgWorker);
            bgWorker.ReportProgress(-1, END_MSG);
        }

        private void setClickable(bool p)
        {
            this.button_path.Enabled = p;
            this.button_name.Enabled = p;
            this.button_doRename.Enabled = p;
            this.textBox_path.Enabled = p;
            //throw new NotImplementedException();
        }



        private bool DoCheckMessage()
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("执行前请先备份字幕文件，以免识别错误导致字幕文件混乱\n本消息只提示一次", "确认信息", messButton);
            if (dr == DialogResult.OK)
            {
                this.ischecked = true;
                return true;
            }
            return false;
        }

        private void button_regex_panel_Click_1(object sender, EventArgs e)
        {
            setPanelRegexVisible(!panel_regex.Visible);
            if (panel_regex.Visible == false)
            {
                button_regex_panel.Text = "↓";
            }
            else
            {
                button_regex_panel.Text = "↑";
            }
        }




        private void Form1_Resize(object sender, EventArgs e)
        {
            //Size size;
            reset_regex_size();

            //size = textBox_video_right.Size;
            //size.Width = groupBox_video.Right - textBox_video_right.Left;


            //size = textBox_video_left.Size;
            //size.Width = label_video_num.Left - groupBox_video.Left; 


        }

        private void reset_regex_size()
        {
            textBox_video_right.Left = groupBox_video.Width / 2 + (lable_num_width/2+4) ;
            textBox_video_right.Width = groupBox_video.Right - textBox_video_right.Left - 6;
            textBox_video_left.Width = groupBox_video.Width / 2 - (lable_num_width / 2 + 10) - groupBox_video.Left;
            label_video_num.Left = groupBox_video.Width / 2 - lable_num_width / 2;


            textBox_sub_right.Left = groupBox_sub.Width / 2 + (lable_num_width / 2 + 4);
            textBox_sub_right.Width = groupBox_sub.Right - textBox_sub_right.Left - 6 ;
            textBox_sub_left.Width = groupBox_sub.Width / 2 - (lable_num_width / 2 + 10) - groupBox_sub.Left;
            label_sub_num.Left = groupBox_sub.Width / 2 - lable_num_width / 2;
        }
    }
}
