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
        static string END_MSG = "end";
        Names names = null;
        bool ischecked = false;
        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = System.Environment.CurrentDirectory;
            //this.textBox1.Text = "D:\\aaa\\aaa";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.toolStripProgressBar1.Value = 0;
            DirectoryInfo dInfo = new DirectoryInfo(this.textBox1.Text);
            names = new Names(dInfo);
            names.setTreeView(this.treeView1);

        }

        private void button3_Click(object sender, EventArgs e)
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
                    this.button2_Click(null, null);
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
            this.button1.Enabled = p;
            this.button2.Enabled = p;
            this.button3.Enabled = p;
            this.textBox1.Enabled = p;
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


    }
}
