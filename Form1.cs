using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly int label_num_width;

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
        private static readonly string NAME_VIDEO_LABEL = "lable_video";

        /// <summary>
        /// 手动修改pannel内字幕文件label名字
        /// </summary>
        private static readonly string NAME_SUB_LABEL = "lable_sub";


        private static readonly Color COLOR_VIDEOLABEL = SystemColors.ControlLightLight;

        private static readonly Color COLOR_SUBLABEL = SystemColors.ControlLight;

        private static readonly Color COLOR_SUBLABEL_HIGHLIHGT = SystemColors.Highlight;

        private static readonly Color COLOR_CHILD_PANAL = SystemColors.ActiveCaption;

        private static readonly Color COLOR_CHILD_PANAL_HIGHLIHGT = SystemColors.Highlight;

        /// <summary>
        /// Form1初始化，隐藏panel_regex，设置lable_num_width
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            textBox_path.Text = Environment.CurrentDirectory;
#if DEBUG
            this.textBox_path.Text = "D:\\anime\\[Kamigami&VCB-Studio] Yahari Ore no Seishun Lovecome wa Machigatte Iru. [Ma10p_1080p]";
#endif         
            SetPanelRegexVisible(false);
            label_num_width = label_video_num.Width;
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
                _ = MessageBox.Show(Resource.pls_load_file);
                return;
            }
#if ENABLE_CHECK_MESSAGE // 编译符号控制：定义则保留检查逻辑，未定义则直接跳过
            if (!ischecked)
            {
                if (!DoCheckMessage())
                {
                    return;
                }
            }
#endif
            DoRename();
        }

        private void DoRename()
        {
            SetClickable(false);
            toolStripProgressBar1.Maximum = names != null ? names.GetVideoCount() : 1;
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
                string str = msg.ToString();
                if (str != null && str.Equals(END_MSG))
                {
                    SetClickable(true);
                    toolStripStatusLabel1.Text = Resource.rename_complete;
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
            foreach (object panel in panel_filelist.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(panel))
                {
                    if (panel is Panel _p)
                    {
                        FileInfo video = null;
                        LinkedList<FileInfo> subs = new LinkedList<FileInfo>();

                        foreach (object var in _p.Controls)
                        {
                            if (typeof(Label).IsInstanceOfType(var))
                            {
                                if (var is Label label && label.Tag is FileInfo _fi)
                                {
                                    if (label.Name == NAME_VIDEO_LABEL)
                                    {
                                        video = _fi;
                                    }
                                    else if (label.Name == NAME_SUB_LABEL)
                                    {
                                        _ = subs.AddLast(_fi);
                                    }
                                }
                            }
                        }
                        if (video != null && subs.Count != 0 && bgWorker != null)
                        {
                            bgWorker.ReportProgress(++c, video.Name);
                            Renamer.RenameSubs(video, subs, textBox_delimiter.Text);
                        }
                    }
                }
            }
            if (bgWorker != null) bgWorker.ReportProgress(-1, END_MSG);
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
            DialogResult dr = MessageBox.Show(Resource.check_message_string, Resource.check_message, messButton);
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
            button_regex_panel.Text = panel_regex.Visible == false ? Resource.rgx_down :Resource.rgx_up;
        }



        private void Panel1_resize(object sender, EventArgs e)
        {
            foreach (object p in panel_filelist.Controls)
            {
                if (p is Panel _p)
                {
                    _p.Width = panel_filelist.Width - 6;
                }
            }
        }

        /// <summary>
        /// 调整正则相关textBox_video_right、textBox_video_right
        /// textBox_sub_right、textBox_sub_left的大小
        /// </summary>
        private void Reset_regex_size()
        {
            textBox_video_right.Left = (groupBox_video.Width / 2) + (label_num_width / 2) + 4;
            textBox_video_right.Width = groupBox_video.Right - textBox_video_right.Left - 6;
            textBox_video_left.Width = (groupBox_video.Width / 2) - ((label_num_width / 2) + 10) - groupBox_video.Left;
            label_video_num.Left = (groupBox_video.Width / 2) - (label_num_width / 2);


            textBox_sub_right.Left = (groupBox_sub.Width / 2) + (label_num_width / 2) + 4;
            textBox_sub_right.Width = groupBox_sub.Right - textBox_sub_right.Left - 6;
            textBox_sub_left.Width = (groupBox_sub.Width / 2) - ((label_num_width / 2) + 10) - groupBox_sub.Left;
            label_sub_num.Left = (groupBox_sub.Width / 2) - (label_num_width / 2);
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

        private void Button_Reslove_Click(object sender, EventArgs e)
        {
            if (names != null && NumberResolver.Reslove(names))
            {
                names.Reslovered = true;
                LoadNames(names);
            }
            else
            {
                _ = MessageBox.Show(Resource.reslove_fail);
            }
        }



        private void Button_Redo_Click(object sender, EventArgs e)
        {
            if (Renamer.IsRedoAvailable() == false)
            {
                _ = MessageBox.Show(Resource.no_rename_record);
                return;
            }

            if (Renamer.Redo())
            {
                Button_name2_Click(null, null);
                _ = MessageBox.Show(Resource.revoke_successed);
            }
            else
            {
                Button_name2_Click(null, null);
                _ = MessageBox.Show(Resource.revoke_fail);
            }
        }


        private void LoadNames(Names names)
        {
            if (names == null)
            {
                return;
            }

            panel_filelist.Controls.Clear();
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
            // ===== 步骤1.1：临时变量 - 提取视频列表，用于批量计算集号 =====
            var videoList = names.videos.ToList();
            // ===== 步骤1.2：临时变量 - 生成二维数字数组（仅方法内使用） =====
            // var tempNumber2DArray = NumberResolver.ExtractVideoNumber2DArray(names.videos.ToList());
            var tempVideo2DArray = NumberResolver.ExtractFileName2DArray(videoList);
            // ===== 步骤1.3：临时变量 - 筛选集号列索引（仅方法内使用） =====
            // int tempTargetColIdx = NumberResolver.FindLeftUniqueColumn(tempNumber2DArray);
            int tempVideoColIdx = NumberResolver.FindHighestMODColumn(tempVideo2DArray);
            // ========== 步骤1.4：计算每个视频的最终集号，存入names.videos.num ==========
            for (int i = 0; i < videoList.Count; i++)
            {
                var video = videoList[i];
                string finalEpisodeStr = null; // 默认无集号
                // 仅当列索引有效，且当前视频行有该列数据时，计算集号
                if (tempVideoColIdx >= 0)
                {
                    var episodeStrList = tempVideo2DArray[i];
                    if (tempVideoColIdx < episodeStrList.Count)
                    {
                        // 直接取原始字符串集号，不转数字
                        finalEpisodeStr = episodeStrList[tempVideoColIdx];
                    }
                }
                // 仅存储最终集号到Names（临时变量丢弃）
                video.num = finalEpisodeStr;
            }

            // ===== 步骤2.1：临时变量 - 提取视频列表，用于批量计算集号 =====
            var subList = names.subs.ToList();
            // ===== 步骤2.2：临时变量 - 生成二维数字数组（仅方法内使用） =====
            // var tempNumber2DArray = NumberResolver.ExtractVideoNumber2DArray(names.videos.ToList());
            var tempSub2DArray = NumberResolver.ExtractFileName2DArray(subList);
            // ===== 步骤2.3：临时变量 - 筛选集号列索引（仅方法内使用） =====
            // int tempTargetColIdx = NumberResolver.FindLeftUniqueColumn(tempNumber2DArray);
            int tempSubColIdx = NumberResolver.FindHighestMODColumn(tempSub2DArray);
            // ========== 步骤2.4：计算每个视频的最终集号，存入names.subs.num ==========
            for (int i = 0; i < subList.Count; i++)
            {
                var sub = subList[i];
                string finalEpisodeStr = null; // 默认无集号
                // 仅当列索引有效，且当前视频行有该列数据时，计算集号
                if (tempSubColIdx >= 0)
                {
                    var episodeStrList = tempSub2DArray[i];
                    if (tempSubColIdx < episodeStrList.Count)
                    {
                        // 直接取原始字符串集号，不转数字
                        finalEpisodeStr = episodeStrList[tempSubColIdx];
                    }
                }
                // 仅存储最终集号到Names（临时变量丢弃）
                sub.num = finalEpisodeStr;
            }


            // ========== 步骤3：处理界面渲染（基于已有文件+新计算的集号） ==========
            panel_filelist.Controls.Clear();
            var allSubs = File.FileListTOFileInfoList(names.subs); // 复制字幕列表，用于标记已匹配
            // 渲染每个视频及匹配的字幕
            foreach (File video in names.videos)
            {
                // 从 video.num 读取原始集号字符串
                string episodeNum = video.num;
                // 创建视频面板
                Panel videoPanel = CreateNewChildPanel();
                Label videoLabel = CreateNewFileLable(video.file.Name, NAME_VIDEO_LABEL, video.file);
                AddNewSubLable(videoPanel, videoLabel);

                // 匹配字幕（复用原有GetSubList逻辑）
                if (!string.IsNullOrEmpty(episodeNum))
                {
                    LinkedList<FileInfo> matchedSubs = Renamer.GetSubListByNum(names, episodeNum);
                    foreach (FileInfo sub in matchedSubs)
                    {
                        Label subLabel = CreateNewFileLable(sub.Name, NAME_SUB_LABEL, sub);
                        AddNewSubLable(videoPanel, subLabel);
                        allSubs.Remove(sub);
                    }
                }
                AddChildrenPanel(videoPanel);
            }

            // 渲染未匹配的字幕
            Panel unMatchedPanel = CreateNewChildPanel();
            Label unMatchedTitle = CreateNewFileLable(Resource.other_sub_filename, NAME_VIDEO_LABEL, null);
            AddNewSubLable(unMatchedPanel, unMatchedTitle);
            AddChildrenPanel(unMatchedPanel);
            foreach (FileInfo sub in allSubs)
            {
                Label subLabel = CreateNewFileLable(sub.Name, NAME_SUB_LABEL, sub);
                AddNewSubLable(unMatchedPanel, subLabel);
            }

        }




        private void LoadNames_Reslobered(Names names)
        {
            LinkedList<FileInfo> allsubs = new LinkedList<FileInfo>();
            foreach (File var in names.subs)
            {
                _ = allsubs.AddLast(var.file);
            }

            foreach (Video video in names.videos)
            {
                string num = video.num;
                if (num == null || num == "")
                {
                    continue;
                }
                LinkedList<FileInfo> subs = Renamer.GetSubList(names, num);
                Panel panel = CreateNewChildPanel();
                Label label_v = CreateNewFileLable(video.file.Name, NAME_VIDEO_LABEL, video.file);
                AddNewSubLable(panel, label_v);

                foreach (FileInfo sub in subs)
                {
                    Label label_s = CreateNewFileLable(sub.Name, NAME_SUB_LABEL, sub);
                    AddNewSubLable(panel, label_s);
                    _ = allsubs.Remove(sub);
                }
                AddChildrenPanel(panel);
            }

            Panel panel_1 = CreateNewChildPanel();
            Label label_v1 = CreateNewFileLable(Resource.other_sub_filename, NAME_VIDEO_LABEL, null);
            AddNewSubLable(panel_1, label_v1);
            AddChildrenPanel(panel_1);
            foreach (FileInfo sub in allsubs)
            {
                Label label_s = CreateNewFileLable(sub.Name, NAME_SUB_LABEL, sub);
                AddNewSubLable(panel_1, label_s);
            }
        }

        private void LoadNames_Regex(Names names)
        {
            LinkedList<FileInfo> allsubs = File.FileListTOFileInfoList(names.subs);

            Dictionary<FileInfo, string> videoDic = Renamer.GetDic(File.FileListTOFileInfoList(names.videos), names.GetVideoReplasePattern());
            Dictionary<FileInfo, string> subDic = Renamer.GetDic(File.FileListTOFileInfoList(names.subs), names.GetSubReplasePattern());

            foreach (FileInfo video in videoDic.Keys)
            {
                Panel panel = CreateNewChildPanel();
                LinkedList<FileInfo> subs = Renamer.GetSubList(subDic, videoDic[video]);
                Label label_v = CreateNewFileLable(video.Name, NAME_VIDEO_LABEL, video);
                AddNewSubLable(panel, label_v);
                foreach (FileInfo sub in subs)
                {
                    Label label_s = CreateNewFileLable(sub.Name, NAME_SUB_LABEL, sub);
                    AddNewSubLable(panel, label_s);
                    _ = allsubs.Remove(sub);
                }
                AddChildrenPanel(panel);
            }

            Panel panel_1 = CreateNewChildPanel();
            Label label_v1 = CreateNewFileLable(Resource.other_sub_filename, NAME_VIDEO_LABEL, null);
            AddNewSubLable(panel_1, label_v1);
            AddChildrenPanel(panel_1);
            foreach (FileInfo sub in allsubs)
            {
                Label label_s = CreateNewFileLable(sub.Name, NAME_SUB_LABEL, sub);
                AddNewSubLable(panel_1, label_s);
            }
        }

        private void AddChildrenPanel(Panel panel)
        {
            int buttom = 0;
            foreach (object pan in panel_filelist.Controls)
            {
                if (pan is Panel _pan)
                {
                    if (buttom < _pan.Bottom)
                    {
                        buttom = _pan.Bottom;
                    }
                }
            }
            panel.Top = buttom + 3;
            panel.Left = 3;
            panel.Width = panel_filelist.Width;
            panel_filelist.Controls.Add(panel);
        }

        private Panel CreateNewChildPanel()
        {
            int pl_wi = panel_filelist.Size.Width;
            Panel panel = new Panel
            {
                BackColor = COLOR_CHILD_PANAL,
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
            if (sender is Panel _s)
                _s.Width = panel_filelist.Width;
        }

        private void DragDrop_Panel(object sender, DragEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripProgressBar1.Text = "DragDrop";
#endif         
            if (sender is Panel _s)
            {
                if (dragTraget != null)
                {
                    if (!_s.Controls.Contains(dragTraget))
                    {
                        //e.Effect = DragDropEffects.Move;
                        if (dragTraget.Parent is Panel _p)
                            RemoveSubLable(_p, dragTraget);
                        AddNewSubLable(_s, dragTraget);
                    }
                }

                //setDragTraget(null);
                _s.BackColor = COLOR_CHILD_PANAL;
            }
        }

        private void DragEnter_Panel(object sender, DragEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripProgressBar1.Text = "DragEnter";
#endif
            if (sender is Panel _s)
            {
                if (dragTraget != null)
                {
                    if (!_s.Controls.Contains(dragTraget))
                    {
                        e.Effect = DragDropEffects.Move;
                        _s.BackColor = COLOR_CHILD_PANAL_HIGHLIHGT;
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
            if (sender is Panel _s)
            {
                _s.BackColor = COLOR_CHILD_PANAL;
            }
        }


        private void AddNewSubLable(Panel panel, Label lable)
        {
            Label video = null;
            int buttom = 0;
            bool resize = false;
            foreach (object lab in panel.Controls)
            {
                if (lab is Label _lab)
                {
                    //if (_lab.Name == NAME_VIDEO_LABEL)
                    //{
                    //    video = lab as Label;
                    //}
                    //else if (_lab.Name == NAME_SUB_LABEL)
                    //{
                        if (buttom < _lab.Bottom)
                        {
                            buttom = _lab.Bottom;
                        }
                    //}
                }
            }
            if (lable.Name == NAME_VIDEO_LABEL)
            {
                if (video == null)
                {
                    lable.Location = new Point(3, 3);
                    panel.Controls.Add(lable);
                    resize = true;
                }
            }
            else if (lable.Name == NAME_SUB_LABEL)
            {
                lable.Location = video != null ? new Point(video.Left + 15, buttom + 3) : new Point(panel.Left + 18, buttom + 3);
                panel.Controls.Add(lable);
                buttom = lable.Bottom + 3;
                resize = true;
            }

            if (resize)
            {
                panel.Height = buttom;
                if (panel.Parent != null) ResizeChildPanel(panel.Parent);
            }
        }

        private void RemoveSubLable(Panel panel, Label lable)
        {
            panel.Controls.Remove(lable);

            LinkedList<Label> list = new LinkedList<Label>();
            Label video = null;
            
            foreach (object lab in panel.Controls)
            {
                if (lab is Label _lab)
                {
                    if (_lab.Name == NAME_SUB_LABEL)
                    {
                        _ = list.AddLast(_lab);
                    }
                    else if (_lab.Name == NAME_VIDEO_LABEL)
                    {
                        video = _lab;
                    }
                }
            }

            int bottom = 3 + video.Bottom;

            foreach (Label lab in list)
            {
                lab.Top = bottom;
                bottom = lab.Bottom + 3;
            }
            panel.Height = bottom;
            if (panel.Parent != null) ResizeChildPanel(panel.Parent);

        }
        private static void ResizeChildPanel(object sender)
        {
            if (sender is Panel _s)
            {
                int butt = 3 + _s.AutoScrollPosition.Y;
                foreach (object p in _s.Controls)
                {
                    if (p is Panel _p)
                    {
                        _p.Location = new Point(3, butt);
                        butt += _p.Height + 3;
                    }
                }
            }

        }

        private Label CreateNewFileLable(string text, string name, FileInfo file)
        {

            if (name == NAME_VIDEO_LABEL)
            {
                Label lable = new Label
                {
                    BackColor = COLOR_VIDEOLABEL,
                    Text = text,
                    AutoSize = true,
                    Margin = new Padding(3),
                    Padding = new Padding(3),
                    Name = name,
                    Tag = file
                };
                return lable;
            }
            else if (name == NAME_SUB_LABEL)
            {
                Label lable = new Label
                {
                    BackColor = COLOR_SUBLABEL,
                    Text = text,
                    AutoSize = true,
                    Margin = new Padding(3),
                    Padding = new Padding(3),
                    Name = name,
                    Tag = file
                };
                lable.MouseDown += new MouseEventHandler(SubLable_MouseDown);
                lable.MouseUp += new MouseEventHandler(SubLable_MouseUp);
                lable.MouseMove += new MouseEventHandler(SubLable_MouseMove);
                lable.QueryContinueDrag += new QueryContinueDragEventHandler(SubLable_QueryContinueDrag);
                return lable;
            }
            else
            {
                return null;
            }
            
        }

        private void SubLable_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragTraget != null)
            {
#if DEBUG
                //手动调整窗口时debug用
                this.toolStripStatusLabel1.Text = "MouseMove (" + e.X + "," + e.Y + ")";
#endif
                _ = dragTraget.DoDragDrop(dragTraget.Text, DragDropEffects.Move);
                SetDragTraget(null);
            }
        }

        private void SubLable_MouseUp(object sender, MouseEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripStatusLabel1.Text = "MouseUp";
#endif
            SetDragTraget(null);
        }


        private void SubLable_MouseDown(object sender, MouseEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripStatusLabel1.Text = "MouseDown";
#endif
            if (typeof(Label).IsInstanceOfType(sender))
            {
                if (dragTraget != null)
                {
                    SetDragTraget(null);
                }
                SetDragTraget(sender as Label);
            }
        }

        private void SetDragTraget(Label label)
        {
            if (label == null)
            {
                if (dragTraget != null)
                {
                    dragTraget.BackColor = COLOR_SUBLABEL;
                }
                dragTraget = null;
                toolStripStatusLabel1.Text = null;
            }
            else
            {
                dragTraget = label;
                toolStripStatusLabel1.Text = dragTraget.Text;
                dragTraget.BackColor = COLOR_SUBLABEL_HIGHLIHGT;
            }

        }


        private void SubLable_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (sender is Control _s)
            {
                Form f = _s.FindForm();
                if (f != null)
                {
                    Point screenLocation_Panel1 = PointToScreen(panel_filelist.Location);
                    Point screenOffset = SystemInformation.WorkingArea.Location;

#if DEBUG
                    //手动调整窗口时debug用
                    this.toolStripStatusLabel1.Text = "m (" + (Control.MousePosition.X - screenLocation_Panel1.X) + "," + (Control.MousePosition.Y - screenLocation_Panel1.Y) + ") L ("
                        + panel_filelist.AutoScrollPosition.X + "," + panel_filelist.AutoScrollPosition.Y + ") " + panel_filelist.Height + " d " + dragTraget.ToString();
#endif
                    //(this.Height - this.ClientRectangle.Height)

                    // Cancel the drag if the mouse moves off the form. The screenOffset
                    // takes into account any desktop bands that may be at the top or left
                    // side of the screen.
                    if (((Control.MousePosition.X - screenOffset.X) < f.DesktopBounds.Left) ||
                        ((Control.MousePosition.X - screenOffset.X) > f.DesktopBounds.Right) ||
                        ((Control.MousePosition.Y - screenOffset.Y) < f.DesktopBounds.Top) ||
                        ((Control.MousePosition.Y - screenOffset.Y) > f.DesktopBounds.Bottom))
                    {
#if DEBUG
                        //手动调整窗口时debug用
                        toolStripStatusLabel1.Text = "cancel";
#endif
                        e.Action = DragAction.Cancel;
                        SetDragTraget(null);

                    }
                    else if (Control.MousePosition.Y - screenLocation_Panel1.Y < 15)
                    {
#if DEBUG
                        //手动调整窗口时debug用
                        toolStripStatusLabel1.Text = (DateTime.Now - scrolltime).TotalMilliseconds.ToString();
#endif

                        if ((DateTime.Now - scrolltime).TotalMilliseconds > 5)
                        {
                            panel_filelist.AutoScrollPosition = new Point(0, -panel_filelist.AutoScrollPosition.Y - 4);
                            scrolltime = DateTime.Now;
                        }

                    }
                    else if (Control.MousePosition.Y - screenLocation_Panel1.Y > panel_filelist.Height - 15)
                    {
                        if ((DateTime.Now - scrolltime).TotalMilliseconds > 5)
                        {
                            panel_filelist.AutoScrollPosition = new Point(0, -panel_filelist.AutoScrollPosition.Y + 4);
                            scrolltime = DateTime.Now;
                        }
                    }
                }
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