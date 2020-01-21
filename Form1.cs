using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SubRenamer
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
        internal bool ischecked = false;
        /// <summary>
        /// lable "集号" 的宽度，在Form1初始化时获取
        /// </summary>
        int lable_num_width;

        /// <summary>
        /// 手动修改pannel拖动目标
        /// </summary>
        private Label dragTraget;
        /// <summary>
        /// 拖动时滚动延迟时间
        /// </summary>
        private DateTime scrolltime;

        /// <summary>
        /// 手动修改pannel内label高度
        /// </summary>
        static int pl_hi = 20;
        /// <summary>
        /// 手动修改pannel内视频文件label名字
        /// </summary>
        static readonly string name_video_lable = "lable_video";
        /// <summary>
        /// 手动修改pannel内字幕文件label名字
        /// </summary>
        static readonly string name_sub_lable = "lable_sub";

        /// <summary>
        /// Form1初始化，隐藏panel_regex，设置lable_num_width
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            this.textBox_path.Text = System.Environment.CurrentDirectory;
            //this.textBox_path.Text = "D:\\aaa\\aaa\\aab";
            setPanelRegexVisible(false);
            lable_num_width = label_video_num.Width;
            treeView1.Visible = false;
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
        internal void button_name_Click(object sender, EventArgs e)
        {
            this.toolStripProgressBar1.Value = 0;
            DirectoryInfo dInfo = new DirectoryInfo(this.textBox_path.Text);
            if (!panel_regex.Visible)
            {
                names = new Names(dInfo, true);
            }
            else
            {
                names = new Names(dInfo, textBox_video_left.Text, textBox_video_right.Text, textBox_sub_left.Text, textBox_sub_right.Text);
            }
            setTreeView(this.treeView1, names);

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
            //if (!ischecked)
            //{
            //    if (!DoCheckMessage())
            //    {
            //        return;
            //    }
            //}
            DoRename();
        }

        private void DoRename()
        {
            //this.toolStripStatusLabel1.Text = "正在运行...";
            setClickable(false);
            this.toolStripProgressBar1.Maximum = names.getVideoCount();
            //this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            //this.backgroundWorker1.WorkerReportsProgress = true;
            //this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
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
            if (msg != null)
            {
                if (msg.ToString().Equals(END_MSG))
                {
                    setClickable(true);
                    this.toolStripStatusLabel1.Text = "修改完成";
                    //this.button_name_Click(null, null);
                    button_name2_Click(sender, null);
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
            int c = 0;
            //Renamer.Rename(names, bgWorker);
            Renamer.clearRedoDic();
            foreach (var panel in this.panel1.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(panel))
                {
                    FileInfo video = null;
                    LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
                    foreach (var var in (panel as Panel).Controls)
                    {
                        if (typeof(Label).IsInstanceOfType(var))
                        {
                            Label label = var as Label;
                            if (label.Name == name_video_lable)
                            {
                                video = (FileInfo)label.Tag;
                            }
                            else if (label.Name == name_sub_lable)
                            {
                                subs.AddLast((FileInfo)label.Tag);
                            }
                        }
                    }
                    if (video != null && subs.Count != 0)
                    {
                        bgWorker.ReportProgress(++c, video.Name);
                        Renamer.renameSubs(video, subs);
                    }
                }
            }


            bgWorker.ReportProgress(-1, END_MSG);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //BackgroundWorker bgWorker = sender as BackgroundWorker;
            //bgWorker.ReportProgress(-1, END_MSG);

        }

        private void setClickable(bool p)
        {
            this.button_path.Enabled = p;
            //this.button_name.Enabled = p;
            this.button_doRename.Enabled = p;
            this.button_name2.Enabled = p;
            this.textBox_path.Enabled = p;
            //throw new NotImplementedException();
        }



        internal bool DoCheckMessage()
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("执行前请先备份字幕文件，以免识别错误导致字幕文件混乱\n撤销功能只能撤销上一次改名结果\n本消息只提示一次", "确认信息", messButton);
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
            textBox_video_right.Left = groupBox_video.Width / 2 + (lable_num_width / 2 + 4);
            textBox_video_right.Width = groupBox_video.Right - textBox_video_right.Left - 6;
            textBox_video_left.Width = groupBox_video.Width / 2 - (lable_num_width / 2 + 10) - groupBox_video.Left;
            label_video_num.Left = groupBox_video.Width / 2 - lable_num_width / 2;


            textBox_sub_right.Left = groupBox_sub.Width / 2 + (lable_num_width / 2 + 4);
            textBox_sub_right.Width = groupBox_sub.Right - textBox_sub_right.Left - 6;
            textBox_sub_left.Width = groupBox_sub.Width / 2 - (lable_num_width / 2 + 10) - groupBox_sub.Left;
            label_sub_num.Left = groupBox_sub.Width / 2 - lable_num_width / 2;
        }

        private void button_autotransfer_Click(object sender, EventArgs e)
        {
            TransferChar(textBox_video_left, textBox_video_right, textBox_sub_left, textBox_sub_right);
        }

        private static void TransferChar(params TextBox[] textBoxs)
        {
            foreach (TextBox textBox in textBoxs)
            {
                string str = textBox.Text;
                str = str.Replace("\\", "\\\\");
                str = str.Replace("]", "\\]");
                str = str.Replace("[", "\\[");
                str = str.Replace("}", "\\}");
                str = str.Replace("{", "\\{");
                str = str.Replace(")", "\\)");
                str = str.Replace("(", "\\(");
                str = str.Replace("^", "\\^");
                str = str.Replace("$", "\\$");
                str = str.Replace("|", "\\|");
                str = str.Replace("*", "\\*");
                str = str.Replace("+", "\\+");
                str = str.Replace(".", "\\.");
                str = str.Replace("?", "\\?");
                textBox.Text = str;
                //string patt = "[\\\\\\[\\]\\(\\)\\{\\}^$|*+?]";
                //MessageBox.Show(patt);
                //MatchCollection res = Regex.Matches(str, patt);
                //string aaa = "";
                //foreach (Match v in res)
                //{
                //    aaa += v.Value + "\n";
                //}
                //MessageBox.Show(aaa);
            }
        }

        private void button_name2_Click(object sender, EventArgs e)
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

            //使用treeView显示文件名时的代码
            //setTreeView(this.treeView1, names);

            loadNames(names);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (NumberResolver.Reslove(names))
            {
                names.reslovered = true;
                setTreeView(this.treeView1, names);
                loadNames(names);
            }
            else
            {
                MessageBox.Show("分析失败，不要问为什么，就是失败了");
            }
        }



        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Renamer.isRedoAvailable() == false)
            {
                MessageBox.Show("没有操作记录");
                return;
            }
            //Form2 fm2 = new Form2();
            //fm2.setNames(names, this);
            //this.Hide();
            //fm2.Show();
            //loadNames(names);
            if (Renamer.Redo())
            {
                DialogResult dr = MessageBox.Show("撤销成功");
                if(dr == DialogResult.OK)
                {
                    button_name2_Click(null,null);
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("撤销失败，重来吧");
                if (dr == DialogResult.OK)
                {
                    button_name2_Click(null, null);
                }
            }
        }


        private void loadNames(Names names)
        {
            if (names == null) return;

            this.panel1.Controls.Clear();
            if (names.isRegex)
            {
                loadNames_Regex(names);
            }
            else if (names.reslovered)
            {
                loadNames_Reslobered(names);
            }
            else
            {
                loadNames_normal(names);
            }
        }


        private void loadNames_normal(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (var var in names.subs)
            {
                allsubs.AddLast(var);
            }
            foreach (var video in names.videos)
            {
                string num = Renamer.getVideoNumber(video.file);
                Panel panel = getNewChildPanel();
                Label label_v = newFileLable(video.file.Name, name_video_lable, video.file);
                addNewSubLable(panel, label_v);
                addChildrenPanel(panel);
                if (num != null)
                {
                    LinkedList<FileInfo> subs = Renamer.getSubList(names, num);
                    foreach (FileInfo sub in subs)
                    {
                        Label label_s = newFileLable(sub.Name, name_sub_lable, sub);
                        addNewSubLable(panel, label_s);
                        allsubs.Remove(sub);
                    }
                }
            }


            Panel panel1 = getNewChildPanel();
            Label label_v1 = newFileLable("不改名字幕文件", name_video_lable, null);
            addNewSubLable(panel1, label_v1);
            addChildrenPanel(panel1);
            foreach (var sub in allsubs)
            {
                Label label_s = newFileLable(sub.Name, name_sub_lable, sub);
                addNewSubLable(panel1, label_s);
            }
        }


        private void loadNames_Reslobered(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (var var in names.subs)
            {
                allsubs.AddLast(var);
            }

            foreach (var video in names.videos)
            {
                string num = video.num; ;
                if (num == null || num == "")
                {
                    continue;
                }
                LinkedList<FileInfo> subs = Renamer.getSubList(names, num);
                Panel panel = getNewChildPanel();
                Label label_v = newFileLable(video.file.Name, name_video_lable, video.file);
                addNewSubLable(panel, label_v);

                foreach (FileInfo sub in subs)
                {
                    Label label_s = newFileLable(sub.Name, name_sub_lable, sub);
                    addNewSubLable(panel, label_s);
                    allsubs.Remove(sub);
                }
                addChildrenPanel(panel);
            }

            Panel panel1 = getNewChildPanel();
            Label label_v1 = newFileLable("不改名字幕文件", name_video_lable, null);
            addNewSubLable(panel1, label_v1);
            addChildrenPanel(panel1);
            foreach (var sub in allsubs)
            {
                Label label_s = newFileLable(sub.Name, name_sub_lable, sub);
                addNewSubLable(panel1, label_s);
            }
        }

        private void loadNames_Regex(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (var var in names.subs)
            {
                allsubs.AddLast(var);
            }


            Dictionary<FileInfo, string> videoDic = Renamer.getDic(names.getVideoFileList(), names.getVideoReplasePattern());
            Dictionary<FileInfo, string> subDic = Renamer.getDic(names.subs, names.getSubReplasePattern());
            
            foreach (var video in videoDic.Keys)
            {
                Panel panel = getNewChildPanel();
                LinkedList<FileInfo> subs = Renamer.getSubList(subDic, videoDic[video]);
                Label label_v = newFileLable(video.Name, name_video_lable, video);
                addNewSubLable(panel, label_v);
                foreach (FileInfo sub in subs)
                {
                    Label label_s = newFileLable(sub.Name, name_sub_lable, sub);
                    addNewSubLable(panel, label_s);
                    allsubs.Remove(sub);
                }
                addChildrenPanel(panel);
            }

            Panel panel1 = getNewChildPanel();
            Label label_v1 = newFileLable("不改名字幕文件", name_video_lable, null);
            addNewSubLable(panel1, label_v1);
            addChildrenPanel(panel1);
            foreach (var sub in allsubs)
            {
                Label label_s = newFileLable(sub.Name, name_sub_lable, sub);
                addNewSubLable(panel1, label_s);
            }
        }

        private void addChildrenPanel(Panel panel)
        {
            int buttom = 0;
            foreach (var pan in this.panel1.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(pan))
                {
                    if (buttom < (pan as Panel).Bottom)
                    {
                        buttom = (pan as Panel).Bottom;
                    }
                }
            }
            panel.Top = buttom + 3;
            panel.Left = 3;
            this.panel1.Controls.Add(panel);
            //this.flowLayoutPanel1.Controls.Add(panel);
        }

        private Panel getNewChildPanel()
        {
            int pl_wi = this.panel1.Size.Width - 30;
            Panel panel = new Panel();
            panel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            panel.Size = new System.Drawing.Size(pl_wi, pl_hi);
            panel.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            panel.AllowDrop = true;
            panel.AutoSize = true;
            //pannel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            //panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            panel.AutoSizeChanged += new EventHandler(panelAutoSizeChanged);
            panel.Margin = new System.Windows.Forms.Padding(3);
            panel.Padding = new System.Windows.Forms.Padding(3);
            panel.DragDrop += new DragEventHandler(DragDrop_Panel);
            panel.DragEnter += new DragEventHandler(DragEnter_Panel);
            panel.DragLeave += new EventHandler(DragLeave_Panel);
            return panel;
        }

        private void panelAutoSizeChanged(object sender, EventArgs e)
        {
            (sender as Panel).Width = this.panel1.Width - 30;
        }

        private void DragDrop_Panel(object sender, DragEventArgs e)
        {
            //手动调整窗口时debug用
            //this.label1.Text = "DragDrop";
            if (dragTraget != null)
            {
                if (typeof(Panel).IsInstanceOfType(sender))
                {
                    if (!(sender as Panel).Controls.Contains(dragTraget))
                    {
                        //e.Effect = DragDropEffects.Move;
                        removeSubLable((Panel)dragTraget.Parent, dragTraget);
                        addNewSubLable((sender as Panel), dragTraget);
                        //MessageBox.Show((string)e.Data.GetData(DataFormats.StringFormat));
                    }
                }
            }
          //setDragTraget(null);
          (sender as Panel).BackColor = System.Drawing.SystemColors.ActiveCaption;
            //dragTraget = null;
            //this.label_file.Text = null;
        }

        private void DragEnter_Panel(object sender, DragEventArgs e)
        {
            //手动调整窗口时debug用
            //this.label1.Text = "DragEnter";
            if (dragTraget != null)
            {
                if (typeof(Panel).IsInstanceOfType(sender))
                {
                    if (!(sender as Panel).Controls.Contains(dragTraget))
                    {
                        e.Effect = DragDropEffects.Move;
                        (sender as Panel).BackColor = System.Drawing.SystemColors.Highlight;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
        }


        private void DragLeave_Panel(object sender, EventArgs e)
        {
            if (typeof(Panel).IsInstanceOfType(sender))
            {
                (sender as Panel).BackColor = System.Drawing.SystemColors.ActiveCaption;
            }
        }


        private void addNewSubLable(Panel panel, Label lable)
        {
            Label video = null;
            int buttom = 0;
            Boolean resize = false;
            foreach (var lab in (panel as Panel).Controls)
            {
                if (typeof(Label).IsInstanceOfType(lab))
                {
                    if ((lab as Label).Name == name_video_lable)
                    {
                        video = (lab as Label);
                    }
                    else if ((lab as Label).Name == name_sub_lable)
                    {
                        if (buttom < (lab as Label).Bottom)
                        {
                            buttom = (lab as Label).Bottom;
                        }
                    }
                }
            }
            if (lable.Name == name_video_lable)
            {
                if (video == null)
                {
                    lable.Location = new Point(3, 3);
                    (panel as Panel).Controls.Add(lable);
                    resize = true;
                }
            }
            else if (lable.Name == name_sub_lable)
            {
                if (video != null)
                {
                    lable.Location = new Point(video.Right + 3, buttom + 3);
                }
                else
                {
                    lable.Location = new Point(panel.Width / 2 + 3, buttom + 3);
                }
                (panel as Panel).Controls.Add(lable);
                buttom = lable.Bottom + 3;
                resize = true;
            }

            if (resize)
            {
                panel.Height = buttom;
                replaceChildPanel(panel.Parent);
            }
        }

        private void removeSubLable(Panel panel, Label lable)
        {
            (panel as Panel).Controls.Remove(lable);

            LinkedList<Label> list = new LinkedList<Label>();
            int bottom = 3;
            foreach (var lab in (panel as Panel).Controls)
            {
                if (typeof(Label).IsInstanceOfType(lab))
                {
                    if ((lab as Label).Name == name_sub_lable)
                    {
                        list.AddLast(lab as Label);
                    }
                }
            }
            foreach (Label lab in list)
            {
                lab.Top = bottom;
                bottom = lab.Bottom + 3;
            }
            panel.Height = bottom;
            replaceChildPanel(panel.Parent);

        }
        private void replaceChildPanel(object sender)
        {
            if (!typeof(Panel).IsInstanceOfType(sender))
            {
                return;
            }
            Panel panel = (sender as Panel);
            int butt = 3 + panel.AutoScrollPosition.Y;
            //foreach (var p in panel.Controls)
            //{
            //    if (typeof(Panel).IsInstanceOfType(p))
            //    {
            //        if(butt>(p as Panel).Location.Y)
            //        {
            //            butt = (p as Panel).Location.Y; 
            //        }
            //    }
            //}
            foreach (var p in panel.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(p))
                {
                    (p as Panel).Location = new Point(3, butt);
                    butt += (p as Panel).Height + 3;
                }
            }
        }
        private Label newFileLable(String text, String name)
        {
            return newFileLable(text, name, null);
        }

        private Label newFileLable(String text, String name,FileInfo file)
        {
            Label lable = new Label();
            lable.BackColor = System.Drawing.SystemColors.ControlLightLight;
            lable.Text = text;
            lable.AutoSize = true;
            lable.Margin = new System.Windows.Forms.Padding(3);
            lable.Padding = new System.Windows.Forms.Padding(3);
            lable.Name = name;
            lable.Tag = file;
            if(name == name_sub_lable)
            {
                lable.MouseDown += new MouseEventHandler(this.SubLable_MouseDown);
                lable.MouseUp += new MouseEventHandler(this.SubLable_MouseUp);
                lable.MouseMove += new MouseEventHandler(this.SubLable_MouseMove);
                lable.QueryContinueDrag += new QueryContinueDragEventHandler(this.SubLable_QueryContinueDrag);
                //lable.GiveFeedback += new GiveFeedbackEventHandler(this.SubLable_GiveFeedback);

            }
            return lable;
        }

        private void SubLable_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragTraget != null)
            {
                //手动调整窗口时debug用
                //this.label1.Text = "MouseMove (" + e.X + "," + e.Y + ")";
                dragTraget.DoDragDrop(dragTraget.Text, DragDropEffects.Move);
                setDragTraget(null);
            }
        }

        private void SubLable_MouseUp(object sender, MouseEventArgs e)
        {
            //手动调整窗口时debug用
            //this.label1.Text = "MouseUp";
            setDragTraget(null);
            //dragTraget = null;
            //this.label_file.Text = null;
        }


        private void SubLable_MouseDown(object sender, MouseEventArgs e)
        {
            //手动调整窗口时debug用
            //this.label1.Text = "MouseDown";
            if (typeof(Label).IsInstanceOfType(sender))
            {
                if (dragTraget != null)
                {
                    setDragTraget(null);
                }
                setDragTraget(sender as Label);
                //label.DoDragDrop(label.Text, DragDropEffects.Move);
            }
        }

        private void setDragTraget(Label label)
        {
            if (label == null)
            {
                if (dragTraget != null)
                {
                    dragTraget.BackColor = System.Drawing.SystemColors.ControlLightLight;
                }
                dragTraget = null;
                this.toolStripStatusLabel1.Text = null;
            }
            else
            {
                dragTraget = label;
                this.toolStripStatusLabel1.Text = dragTraget.Text;
                dragTraget.BackColor = System.Drawing.SystemColors.MenuHighlight;
            }

        }


        private void SubLable_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

            Form f = (sender as Control).FindForm();
            Point screenLocation_Panel1 = PointToScreen(panel1.Location);
            Point screenOffset = SystemInformation.WorkingArea.Location;

            //手动调整窗口时debug用
            //this.label1.Text = "m (" + (Control.MousePosition.X - screenLocation_Panel1.X) + "," + (Control.MousePosition.Y - screenLocation_Panel1.Y) + ") L ("
            //    + panel1.AutoScrollPosition.X + "," + panel1.AutoScrollPosition.Y + ") " + panel1.Height;
            
            
            //(this.Height - this.ClientRectangle.Height)



            // Cancel the drag if the mouse moves off the form. The screenOffset
            // takes into account any desktop bands that may be at the top or left
            // side of the screen.
            if (((Control.MousePosition.X - screenOffset.X) < f.DesktopBounds.Left) ||
                ((Control.MousePosition.X - screenOffset.X) > f.DesktopBounds.Right) ||
                ((Control.MousePosition.Y - screenOffset.Y) < f.DesktopBounds.Top) ||
                ((Control.MousePosition.Y - screenOffset.Y) > f.DesktopBounds.Bottom))
            {
                //手动调整窗口时debug用
                //label1.Text = "cancel";
                e.Action = DragAction.Cancel;
                setDragTraget(null);
                //dragTraget = null;
                //this.label_file.Text = null;
            }
            else if (Control.MousePosition.Y - screenLocation_Panel1.Y < 15)
            {
                //scrolltime = DateTime.Now;

                //手动调整窗口时debug用
                //label1.Text = (DateTime.Now - scrolltime).TotalMilliseconds.ToString();

                //if (panel1.AutoScrollPosition.Y < 0)
                //{
                if ((DateTime.Now - scrolltime).TotalMilliseconds > 5)
                {
                    panel1.AutoScrollPosition = new Point(0, -panel1.AutoScrollPosition.Y - 4);
                    scrolltime = DateTime.Now;
                }
                //}
            }
            else if (Control.MousePosition.Y - screenLocation_Panel1.Y > panel1.Height - 15)
            {
                if ((DateTime.Now - scrolltime).TotalMilliseconds > 5)
                {
                    panel1.AutoScrollPosition = new Point(0, -panel1.AutoScrollPosition.Y + 4);
                    scrolltime = DateTime.Now;
                }
            }

        }


        internal void setTreeView(System.Windows.Forms.TreeView treeView, Names names)
        {
            treeView.Nodes.Clear();
            TreeNode node = getNodeByNames(names);
            treeView.Nodes.Add(node);
            node.Expand();
        }


        private TreeNode getNodeByNames(Names names)
        {
            TreeNode root = new TreeNode(names.path);

            TreeNode videos = new TreeNode("videos");
            LinkedList<string> video_nodes = getVideoStringList(names);
            addNodes(videos, video_nodes);
            root.Nodes.Add(videos);

            TreeNode subs = new TreeNode("subs");
            LinkedList<string> sub_nodes = getSubStringList(names);
            addNodes(subs, sub_nodes);
            root.Nodes.Add(subs);


            TreeNode directories = new TreeNode("directories");
            addNodes(directories, names.names);
            root.Nodes.Add(directories);
            return root;
        }

        private LinkedList<string> getVideoStringList(Names names)
        {
            LinkedList<string> result = new LinkedList<string>();
            //string[] strs = getStrArray(videos);

            //for(int i = 0; i < strs.Length; i++)
            //{
            //    if (this.isRegex)
            //    {
            //        string str = Regex.Replace(strs[i], getVideoReplasePattern(), "");
            //        result.AddLast(strs[i] + "  ---(" + str + ")");
            //    }
            //    else if (reslovered)
            //    {
            //        result.AddLast(strs[i] + "  ---(" + videos_num[i] + ")"); 
            //    }
            //    else
            //    {
            //        result.AddLast(strs[i]);
            //    }
            //}

            foreach (var video in names.videos)
            {
                if (names.isRegex)
                {
                    string str = Regex.Replace(video.file.Name, names.getVideoReplasePattern(), "");
                    result.AddLast(video.file.Name + "  ---(" + str + ")");
                }
                else if (names.reslovered)
                {
                    result.AddLast(video.file.Name + "  ---(" + video.num + ")");
                }
                else
                {
                    result.AddLast(video.file.Name);
                }
            }
            return result;
        }


        private LinkedList<string> getSubStringList(Names names)
        {
            LinkedList<string> result = new LinkedList<string>();
            foreach (var sub in names.subs)
            {
                if (names.isRegex)
                {
                    string str = Regex.Replace(sub.Name, names.getSubReplasePattern(), "");
                    result.AddLast(sub.Name + "  ---(" + str + ")");
                }
                else
                {
                    result.AddLast(sub.Name);
                }
            }
            return result;
        }

        private void addNodes(TreeNode directories, LinkedList<Names> names)
        {
            foreach (var name in names)
            {
                TreeNode node = getNodeByNames(name);
                directories.Nodes.Add(node);
            }
        }

        private void addNodes(TreeNode videos, LinkedList<string> list)
        {
            foreach (var item in list)
            {
                videos.Nodes.Add(item);
            }
        }

        private void addNodes(TreeNode videos, LinkedList<FileInfo> list)
        {
            foreach (var item in list)
            {
                videos.Nodes.Add(item.Name);
            }
        }
    }
}
