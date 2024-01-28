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
        private static readonly string END_MSG = "end";

        /// <summary>
        /// Names的根节点
        /// </summary>
        private Names names = null;
        /// <summary>
        /// 是否确认过提示信息
        /// </summary>
        internal bool ischecked = false;

        /// <summary>
        /// lable "集号" 的宽度，在Form1初始化时获取
        /// </summary>
        private readonly int lable_num_width;

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
        private static readonly int pl_hi = 20;

        /// <summary>
        /// 手动修改pannel内视频文件label名字
        /// </summary>
        private static readonly string name_video_lable = "lable_video";

        /// <summary>
        /// 手动修改pannel内字幕文件label名字
        /// </summary>
        private static readonly string name_sub_lable = "lable_sub";

        /// <summary>
        /// Form1初始化，隐藏panel_regex，设置lable_num_width
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            textBox_path.Text = System.Environment.CurrentDirectory;
            //this.textBox_path.Text = "D:\\aaa\\aaa\\aab";
            SetPanelRegexVisible(false);
            lable_num_width = label_video_num.Width;
            TextBox_Ext_Size();
            textBox_subExt.Text = Extentions.GetExts(Extentions.SUB);
            textBox_videoExt.Text = Extentions.GetExts(Extentions.VIDEO);
        }

        /// <summary>
        /// 显示/隐藏 panel_regex
        /// </summary>
        /// <param name="visible"></param>
        private void SetPanelRegexVisible(bool visible)
        {
            if (visible)
            {
                panel_regex.Visible = true;//设置可见
                panel_name.Top = panel_regex.Bottom;//设置panel_name的顶部位置
                panel_name.Height = statusStrip1.Top - panel_name.Top;//设置panel_name的高度
                Reset_regex_size();
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
        private void Button_path_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox_path.Text = fbd.SelectedPath;
            }
        }

        /// <summary>
        /// 对names里的文件名 执行重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_doRename_Click(object sender, EventArgs e)
        {
            if (names == null)
            {
                _ = MessageBox.Show("请先获取文件");
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
            SetClickable(false);
            toolStripProgressBar1.Maximum = names.GetVideoCount();
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
            {
                toolStripProgressBar1.Value = e.ProgressPercentage;
            }
            object msg = e.UserState;
            if (msg != null)
            {
                if (msg.ToString().Equals(END_MSG))
                {
                    SetClickable(true);
                    toolStripStatusLabel1.Text = "修改完成";
                    //this.button_name_Click(null, null);
                    Button_name2_Click(sender, null);
                }
                else
                {
                    toolStripStatusLabel1.Text = msg.ToString();

                    //this.toolStripStatusLabel1.ToolTipText = this.toolStripStatusLabel1.Text;
                }
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            int c = 0;
            //Renamer.Rename(names, bgWorker);
            Renamer.ClearRedoDic();
            foreach (object panel in panel1.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(panel))
                {
                    FileInfo video = null;
                    LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
                    foreach (object var in (panel as Panel).Controls)
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
                                _ = subs.AddLast((FileInfo)label.Tag);
                            }
                        }
                    }
                    if (video != null && subs.Count != 0)
                    {
                        bgWorker.ReportProgress(++c, video.Name);
                        Renamer.RenameSubs(video, subs, textBox_delimiter.Text);
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

        private void SetClickable(bool p)
        {
            button_path.Enabled = p;
            //this.button_name.Enabled = p;
            button_doRename.Enabled = p;
            button_name2.Enabled = p;
            textBox_path.Enabled = p;
            //throw new NotImplementedException();
        }



        internal bool DoCheckMessage()
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("执行前请先备份字幕文件，以免识别错误导致字幕文件混乱\n撤销功能只能撤销上一次改名结果\n本消息只提示一次", "确认信息", messButton);
            if (dr == DialogResult.OK)
            {
                ischecked = true;
                return true;
            }
            return false;
        }

        private void Button_regex_panel_Click_1(object sender, EventArgs e)
        {
            SetPanelRegexVisible(!panel_regex.Visible);
            SetExtText_BoxVisable(!panel_regex.Visible);
            button_regex_panel.Text = panel_regex.Visible == false ? "↓" : "↑";
        }




        private void Form1_Resize(object sender, EventArgs e)
        {
            //Size size;
            Reset_regex_size();
            TextBox_Ext_Size();
            //size = textBox_video_right.Size;
            //size.Width = groupBox_video.Right - textBox_video_right.Left;


            //size = textBox_video_left.Size;
            //size.Width = label_video_num.Left - groupBox_video.Left; 


        }


        private void Panel1_resize(object sender, EventArgs e)
        {
            foreach (object p in panel1.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(p))
                {
                    (p as Panel).Width = panel1.Width - 6;
                }
            }
        }

        /// <summary>
        /// 调整正则相关textBox_video_right、textBox_video_right
        /// textBox_sub_right、textBox_sub_left的大小
        /// </summary>
        private void Reset_regex_size()
        {
            textBox_video_right.Left = (groupBox_video.Width / 2) + (lable_num_width / 2) + 4;
            textBox_video_right.Width = groupBox_video.Right - textBox_video_right.Left - 6;
            textBox_video_left.Width = (groupBox_video.Width / 2) - ((lable_num_width / 2) + 10) - groupBox_video.Left;
            label_video_num.Left = (groupBox_video.Width / 2) - (lable_num_width / 2);


            textBox_sub_right.Left = (groupBox_sub.Width / 2) + (lable_num_width / 2) + 4;
            textBox_sub_right.Width = groupBox_sub.Right - textBox_sub_right.Left - 6;
            textBox_sub_left.Width = (groupBox_sub.Width / 2) - ((lable_num_width / 2) + 10) - groupBox_sub.Left;
            label_sub_num.Left = (groupBox_sub.Width / 2) - (lable_num_width / 2);
        }

        /// <summary>
        /// 调整textBox_videoExt、textBox_subExt的size
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void TextBox_Ext_Size()
        {
            textBox_subExt.Left = ((button_doRename.Right + 6 + button_redo.Left - 6) / 2) + 3;
            textBox_subExt.Width = button_redo.Left - textBox_subExt.Left - 6;

            textBox_videoExt.Width = textBox_subExt.Width;


            //throw new NotImplementedException();
        }

        private void Button_Autotransfer_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 按textBox_path里的路径获取文件名，生成Names对象。
        /// panel_regex.Visible是true，则会按照指定的正则表达式获取文件名，同时忽略所有子文件夹。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_name2_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            DirectoryInfo dInfo = new DirectoryInfo(textBox_path.Text);
            names = !panel_regex.Visible
                ? new Names(dInfo)
                : new Names(dInfo, textBox_video_left.Text, textBox_video_right.Text, textBox_sub_left.Text, textBox_sub_right.Text);

            LoadNames(names);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (NumberResolver.Reslove(names))
            {
                names.Reslovered = true;
                LoadNames(names);
            }
            else
            {
                _ = MessageBox.Show("分析失败，不要问为什么，就是失败了");
            }
        }



        private void Button2_Click_1(object sender, EventArgs e)
        {
            if (Renamer.IsRedoAvailable() == false)
            {
                _ = MessageBox.Show("没有操作记录");
                return;
            }

            if (Renamer.Redo())
            {
                DialogResult dr = MessageBox.Show("撤销成功");
                if (dr == DialogResult.OK)
                {
                    Button_name2_Click(null, null);
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("撤销失败，重来吧");
                if (dr == DialogResult.OK)
                {
                    Button_name2_Click(null, null);
                }
            }
        }


        private void LoadNames(Names names)
        {
            if (names == null)
            {
                return;
            }

            panel1.Controls.Clear();
            if (names.IsRegex)
            {
                LoadNames_Regex(names);
            }
            else if (names.Reslovered)
            {
                LoadNames_Reslobered(names);
            }
            else
            {
                LoadNames_normal(names);
            }
        }


        private void LoadNames_normal(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (FileInfo var in names.subs)
            {
                _ = allsubs.AddLast(var);
            }
            foreach (Video video in names.videos)
            {
                string num = Renamer.GetVideoNumber(video.file);
                Panel panel = GetNewChildPanel();
                Label label_v = NewFileLable(video.file.Name, name_video_lable, video.file);
                AddNewSubLable(panel, label_v);
                AddChildrenPanel(panel);
                if (num != null)
                {
                    LinkedList<FileInfo> subs = Renamer.GetSubList(names, num);
                    foreach (FileInfo sub in subs)
                    {
                        Label label_s = NewFileLable(sub.Name, name_sub_lable, sub);
                        AddNewSubLable(panel, label_s);
                        _ = allsubs.Remove(sub);
                    }
                }
            }


            Panel panel_1 = GetNewChildPanel();
            Label label_v1 = NewFileLable("不改名字幕文件", name_video_lable, null);
            AddNewSubLable(panel_1, label_v1);
            AddChildrenPanel(panel_1);
            foreach (FileInfo sub in allsubs)
            {
                Label label_s = NewFileLable(sub.Name, name_sub_lable, sub);
                AddNewSubLable(panel_1, label_s);
            }

        }


        private void LoadNames_Reslobered(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (FileInfo var in names.subs)
            {
                _ = allsubs.AddLast(var);
            }

            foreach (Video video in names.videos)
            {
                string num = video.num; ;
                if (num == null || num == "")
                {
                    continue;
                }
                LinkedList<FileInfo> subs = Renamer.GetSubList(names, num);
                Panel panel = GetNewChildPanel();
                Label label_v = NewFileLable(video.file.Name, name_video_lable, video.file);
                AddNewSubLable(panel, label_v);

                foreach (FileInfo sub in subs)
                {
                    Label label_s = NewFileLable(sub.Name, name_sub_lable, sub);
                    AddNewSubLable(panel, label_s);
                    _ = allsubs.Remove(sub);
                }
                AddChildrenPanel(panel);
            }

            Panel panel_1 = GetNewChildPanel();
            Label label_v1 = NewFileLable("不改名字幕文件", name_video_lable, null);
            AddNewSubLable(panel_1, label_v1);
            AddChildrenPanel(panel_1);
            foreach (FileInfo sub in allsubs)
            {
                Label label_s = NewFileLable(sub.Name, name_sub_lable, sub);
                AddNewSubLable(panel_1, label_s);
            }
        }

        private void LoadNames_Regex(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (FileInfo var in names.subs)
            {
                _ = allsubs.AddLast(var);
            }


            Dictionary<FileInfo, string> videoDic = Renamer.GetDic(names.GetVideoFileList(), names.GetVideoReplasePattern());
            Dictionary<FileInfo, string> subDic = Renamer.GetDic(names.subs, names.GetSubReplasePattern());

            foreach (FileInfo video in videoDic.Keys)
            {
                Panel panel = GetNewChildPanel();
                LinkedList<FileInfo> subs = Renamer.GetSubList(subDic, videoDic[video]);
                Label label_v = NewFileLable(video.Name, name_video_lable, video);
                AddNewSubLable(panel, label_v);
                foreach (FileInfo sub in subs)
                {
                    Label label_s = NewFileLable(sub.Name, name_sub_lable, sub);
                    AddNewSubLable(panel, label_s);
                    _ = allsubs.Remove(sub);
                }
                AddChildrenPanel(panel);
            }

            Panel panel_1 = GetNewChildPanel();
            Label label_v1 = NewFileLable("不改名字幕文件", name_video_lable, null);
            AddNewSubLable(panel_1, label_v1);
            AddChildrenPanel(panel_1);
            foreach (FileInfo sub in allsubs)
            {
                Label label_s = NewFileLable(sub.Name, name_sub_lable, sub);
                AddNewSubLable(panel_1, label_s);
            }
        }

        private void AddChildrenPanel(Panel panel)
        {
            int buttom = 0;
            foreach (object pan in panel1.Controls)
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
            panel.Width = panel1.Width;
            panel1.Controls.Add(panel);
            //this.flowLayoutPanel1.Controls.Add(panel);
        }

        private Panel GetNewChildPanel()
        {
            int pl_wi = panel1.Size.Width;
            Panel panel = new Panel
            {
                BackColor = SystemColors.ActiveCaption,
                Size = new Size(pl_wi, pl_hi),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AllowDrop = true,
                AutoSize = true
            };
            //pannel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            //panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            panel.AutoSizeChanged += new EventHandler(PanelAutoSizeChanged);
            panel.Margin = new Padding(3);
            panel.Padding = new Padding(3);
            panel.DragDrop += new DragEventHandler(DragDrop_Panel);
            panel.DragEnter += new DragEventHandler(DragEnter_Panel);
            panel.DragLeave += new EventHandler(DragLeave_Panel);
            return panel;
        }

        private void PanelAutoSizeChanged(object sender, EventArgs e)
        {
            (sender as Panel).Width = panel1.Width;
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
                        RemoveSubLable((Panel)dragTraget.Parent, dragTraget);
                        AddNewSubLable(sender as Panel, dragTraget);
                        //MessageBox.Show((string)e.Data.GetData(DataFormats.StringFormat));
                    }
                }
            }
          //setDragTraget(null);
          (sender as Panel).BackColor = SystemColors.ActiveCaption;
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
                        (sender as Panel).BackColor = SystemColors.Highlight;
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
                (sender as Panel).BackColor = SystemColors.ActiveCaption;
            }
        }


        private void AddNewSubLable(Panel panel, Label lable)
        {
            Label video = null;
            int buttom = 0;
            bool resize = false;
            foreach (object lab in panel.Controls)
            {
                if (typeof(Label).IsInstanceOfType(lab))
                {
                    if ((lab as Label).Name == name_video_lable)
                    {
                        video = lab as Label;
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
                    panel.Controls.Add(lable);
                    resize = true;
                }
            }
            else if (lable.Name == name_sub_lable)
            {
                lable.Location = video != null ? new Point(video.Right + 3, buttom + 3) : new Point((panel.Width / 2) + 3, buttom + 3);
                panel.Controls.Add(lable);
                buttom = lable.Bottom + 3;
                resize = true;
            }

            if (resize)
            {
                panel.Height = buttom;
                ResizeChildPanel(panel.Parent);
            }
        }

        private void RemoveSubLable(Panel panel, Label lable)
        {
            panel.Controls.Remove(lable);

            LinkedList<Label> list = new LinkedList<Label>();
            int bottom = 3;
            foreach (object lab in panel.Controls)
            {
                if (typeof(Label).IsInstanceOfType(lab))
                {
                    if ((lab as Label).Name == name_sub_lable)
                    {
                        _ = list.AddLast(lab as Label);
                    }
                }
            }
            foreach (Label lab in list)
            {
                lab.Top = bottom;
                bottom = lab.Bottom + 3;
            }
            panel.Height = bottom;
            ResizeChildPanel(panel.Parent);

        }
        private void ResizeChildPanel(object sender)
        {
            if (!typeof(Panel).IsInstanceOfType(sender))
            {
                return;
            }
            Panel panel = sender as Panel;
            int butt = 3 + panel.AutoScrollPosition.Y;

            foreach (object p in panel.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(p))
                {
                    (p as Panel).Location = new Point(3, butt);
                    butt += (p as Panel).Height + 3;
                }
            }
        }

        private Label NewFileLable(string text, string name, FileInfo file)
        {
            Label lable = new Label
            {
                BackColor = SystemColors.ControlLightLight,
                Text = text,
                AutoSize = true,
                Margin = new Padding(3),
                Padding = new Padding(3),
                Name = name,
                Tag = file
            };
            if (name == name_sub_lable)
            {
                lable.MouseDown += new MouseEventHandler(SubLable_MouseDown);
                lable.MouseUp += new MouseEventHandler(SubLable_MouseUp);
                lable.MouseMove += new MouseEventHandler(SubLable_MouseMove);
                lable.QueryContinueDrag += new QueryContinueDragEventHandler(SubLable_QueryContinueDrag);
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
                _ = dragTraget.DoDragDrop(dragTraget.Text, DragDropEffects.Move);
                SetDragTraget(null);
            }
        }

        private void SubLable_MouseUp(object sender, MouseEventArgs e)
        {
            //手动调整窗口时debug用
            //this.label1.Text = "MouseUp";
            SetDragTraget(null);
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
                    SetDragTraget(null);
                }
                SetDragTraget(sender as Label);
                //label.DoDragDrop(label.Text, DragDropEffects.Move);
            }
        }

        private void SetDragTraget(Label label)
        {
            if (label == null)
            {
                if (dragTraget != null)
                {
                    dragTraget.BackColor = SystemColors.ControlLightLight;
                }
                dragTraget = null;
                toolStripStatusLabel1.Text = null;
            }
            else
            {
                dragTraget = label;
                toolStripStatusLabel1.Text = dragTraget.Text;
                dragTraget.BackColor = SystemColors.MenuHighlight;
            }

        }


        private void SubLable_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

            Form f = (sender as Control).FindForm();
            Point screenLocation_Panel1 = PointToScreen(panel1.Location);
            Point screenOffset = SystemInformation.WorkingArea.Location;

            //手动调整窗口时debug用
            //this.label1.Text = "m (" + (Control.MousePosition.X - screenLocation_Panel1.X) + "," + (Control.MousePosition.Y - screenLocation_Panel1.Y) + ") L ("
            //    + panel1.AutoScrollPosition.X + "," + panel1.AutoScrollPosition.Y + ") " + panel1.Height + " d "+ dragTraget.ToString();


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
                SetDragTraget(null);
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

        private TreeNode GetNodeByNames(Names names)
        {
            TreeNode root = new TreeNode(names.path);

            TreeNode videos = new TreeNode("videos");
            LinkedList<string> video_nodes = GetVideoStringList(names);
            AddNodes(videos, video_nodes);
            _ = root.Nodes.Add(videos);

            TreeNode subs = new TreeNode("subs");
            LinkedList<string> sub_nodes = GetSubStringList(names);
            AddNodes(subs, sub_nodes);
            _ = root.Nodes.Add(subs);


            TreeNode directories = new TreeNode("directories");
            AddNodes(directories, names.names);
            _ = root.Nodes.Add(directories);
            return root;
        }

        private LinkedList<string> GetVideoStringList(Names names)
        {
            LinkedList<string> result = new LinkedList<string>();

            foreach (Video video in names.videos)
            {
                if (names.IsRegex)
                {
                    string str = Regex.Replace(video.file.Name, names.GetVideoReplasePattern(), "");
                    _ = result.AddLast(video.file.Name + "  ---(" + str + ")");
                }
                else
                {
                    _ = names.Reslovered ? result.AddLast(video.file.Name + "  ---(" + video.num + ")") : result.AddLast(video.file.Name);
                }
            }
            return result;
        }


        private LinkedList<string> GetSubStringList(Names names)
        {
            LinkedList<string> result = new LinkedList<string>();
            foreach (FileInfo sub in names.subs)
            {
                if (names.IsRegex)
                {
                    string str = Regex.Replace(sub.Name, names.GetSubReplasePattern(), "");
                    _ = result.AddLast(sub.Name + "  ---(" + str + ")");
                }
                else
                {
                    _ = result.AddLast(sub.Name);
                }
            }
            return result;
        }

        private void AddNodes(TreeNode directories, LinkedList<Names> names)
        {
            foreach (Names name in names)
            {
                TreeNode node = GetNodeByNames(name);
                _ = directories.Nodes.Add(node);
            }
        }

        private void AddNodes(TreeNode videos, LinkedList<string> list)
        {
            foreach (string item in list)
            {
                _ = videos.Nodes.Add(item);
            }
        }

        private void TextBox_videoExt_TextChanged(object sender, EventArgs e)
        {
            TextBox videoExt = (TextBox)sender;
            Extentions.SetExts(videoExt.Text, Extentions.VIDEO);
        }

        private void TextBox_subExt_TextChanged(object sender, EventArgs e)
        {
            TextBox subExt = (TextBox)sender;
            Extentions.SetExts(subExt.Text, Extentions.SUB);
        }

        private void SetExtText_BoxVisable(bool visable)
        {
            textBox_subExt.Visible = visable;
            textBox_videoExt.Visible = visable;
        }
    }
}