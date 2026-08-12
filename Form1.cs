using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SubRenamer
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// Names的根节点
        /// </summary>
        private Names names = null;

#if DEBUG
        private int debug_drop_leave;
#endif
#if ENABLE_CHECK_MESSAGE 
        /// <summary>
        /// 是否确认过提示信息
        /// </summary>
        internal bool ischecked = false;
#endif

        /// <summary>
        /// label "集号" 的宽度，在Form1初始化时获取
        /// </summary>
        private readonly int label_num_width;

        /// <summary>
        /// 手动修改pannel拖动目标
        /// </summary>
        private Label dragSubTraget;
        /// <summary>
        /// 拖动时滚动延迟时间
        /// </summary>
        private DateTime scrolltime;


        private bool _isExternalFolderDrag = false;
        //private Point _tipPos;

        /// <summary>
        /// 保存被临时禁用的子控件及其原始 AllowDrop 值
        /// </summary>
        private Dictionary<Control, bool> _savedAllowDrop = new Dictionary<Control, bool>();



        /// <summary>
        /// 手动修改pannel内label高度
        /// </summary>
        private static readonly int pl_hi = 20;

        /// <summary>
        /// 手动修改pannel内视频文件label名字
        /// </summary>
        private static readonly string NAME_VIDEO_LABEL = "label_video";

        /// <summary>
        /// 手动修改pannel内字幕文件label名字
        /// </summary>
        private static readonly string NAME_SUB_LABEL = "label_sub";


        private static readonly Color COLOR_VIDEOLABEL = SystemColors.ControlLightLight;

        private static readonly Color COLOR_SUBLABEL = SystemColors.ControlLight;

        private static readonly Color COLOR_SUBLABEL_HIGHLIHGT = SystemColors.Highlight;

        private static readonly Color COLOR_CHILD_PANAL = SystemColors.ActiveCaption;

        private static readonly Color COLOR_CHILD_PANAL_HIGHLIHGT = SystemColors.Highlight;

        private static readonly Color COLOR_WARNING = Color.LightPink;

        private static readonly Color COLOR_NORMAL = SystemColors.ControlLightLight;

        /// <summary>
        /// Form1初始化，隐藏panel_regex，设置label_num_width
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            textBox_path.Text = Environment.CurrentDirectory;
#if DEBUG
            this.textBox_path.Text = "C:\\aaa\\bbb";
#endif         
            SetPanelRegexVisible(false);
            UpdateButtonRevokeClickable();
            label_num_width = label_video_num.Width;
            TextBox_Ext_Size();
        }

        /// <summary>
        /// 显示/隐藏 panel_regex
        /// </summary>
        /// <param name="visible"></param>
        private void SetPanelRegexVisible(bool visible)
        {
            if (visible)
            {
                tableLayoutPanel_ext.Visible = false;
                panel_regex.Visible = true;//设置可见

                panel_regex.Top = panel_path.Bottom + 3;

                panel_name.Top = panel_regex.Bottom;//设置panel_name的顶部位置
                panel_name.Height = statusStrip1.Top - panel_name.Top;//设置panel_name的高度
                Reset_regex_size();
            }
            else
            {
                panel_regex.Visible = false;
                tableLayoutPanel_ext.Visible = true;
                panel_name.Top = tableLayoutPanel_ext.Bottom + 3;
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
                toolStripStatusLabel1.Text = str;
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = sender as BackgroundWorker;
            int c = 0;
            Renamer.ClearRedoDic();
            foreach (object panel in panel_filelist.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(panel))
                {
                    if (panel is Panel _p)
                    {
                        FileInfo video = null;
                        List<FileInfo> subs = new List<FileInfo>();

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
                                        subs.Add(_fi);
                                    }
                                }
                            }
                        }
                        if (video != null && subs.Count > 0 && bgWorker != null)
                        {
                            bgWorker.ReportProgress(++c, video.Name);
                            Renamer.RenameSubs(video, subs, textBox_delimiter.Text);
                        }
                    }
                }
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetClickable(true);
            toolStripStatusLabel1.Text = Resource.rename_complete;
            Button_name2_Click(null, null);
            //MessageBox.Show("RunWorkerCompleted");
        }

        private void SetClickable(bool p)
        {
            button_path.Enabled = p;
            //this.button_name.Enabled = p;
            button_doRename.Enabled = p;
            button_name2.Enabled = p;
            textBox_path.Enabled = p;

            UpdateButtonRevokeClickable();
        }


#if ENABLE_CHECK_MESSAGE
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
#endif


        private void Button_regex_panel_Click_1(object sender, EventArgs e)
        {
            SetPanelRegexVisible(!panel_regex.Visible);
            SetExtText_BoxVisable(!panel_regex.Visible);
            button_regex_panel.Text = panel_regex.Visible == false ? Resource.rgx_down : Resource.rgx_up;
        }



        private void Panel1_resize(object sender, EventArgs e)
        {
            Panel panel_filelist = sender as Panel;
            if (panel_filelist == null) return;

            int scrollW = panel_filelist.VerticalScroll.Visible
                ? SystemInformation.VerticalScrollBarWidth
                : 0;
            int validWidth = panel_filelist.Width - scrollW - 6;

            foreach (Control c in panel_filelist.Controls)
            {
                if (c is Panel itemPanel)
                {
                    itemPanel.Width = validWidth;

                    foreach (Control subC in itemPanel.Controls)
                    {
                        if (subC is Label lbl)
                        {
                            if (lbl.Name == NAME_VIDEO_LABEL)
                            {
                                lbl.Width = itemPanel.Width - 6;
                            }
                            else if (lbl.Name == NAME_SUB_LABEL)
                            {
                                lbl.Width = itemPanel.Width - 6 - 15;
                            }
                        }
                    }
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
            textBox_subExt.Left = ((button_doRename.Right + 6 + button_revoke.Left - 6) / 2) + 3;
            textBox_subExt.Width = button_revoke.Left - textBox_subExt.Left - 6;

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
            Extentions.SetExts(textBox_videoExt.Text, Extentions.VIDEO);
            Extentions.SetExts(textBox_subExt.Text, Extentions.SUB);
            toolStripProgressBar1.Value = 0;
            DirectoryInfo dInfo = new DirectoryInfo(textBox_path.Text);
            names = !panel_regex.Visible
                ? new Names(dInfo)
                : new Names(dInfo, textBox_video_left.Text, textBox_video_right.Text, textBox_sub_left.Text, textBox_sub_right.Text);

            LoadNames(names);
            if(sender != null)
                toolStripStatusLabel1.Text = Resource.load_path_complete + " [" + textBox_path.Text + "]";
        }

        private void Button_Resolve_Click(object sender, EventArgs e)
        {
            if (names != null && NumberResolver.Resolve(names))
            {
                names.Resolved = true;
                LoadNames(names);
            }
            else
            {
                _ = MessageBox.Show(Resource.resolve_fail);
            }
        }

        internal void UpdateButtonRevokeClickable()
        {
            button_revoke.Enabled = Renamer.IsRedoAvailabel();
        }

        private void Button_Revoke_Click(object sender, EventArgs e)
        {
            if (Renamer.IsRedoAvailabel() == false)
            {
                _ = MessageBox.Show(Resource.no_rename_record);
                return;
            }

            if (Renamer.Revoke())
            {
                Button_name2_Click(null, null);
                _ = MessageBox.Show(Resource.revoke_successed);
            }
            else
            {
                Button_name2_Click(null, null);
                _ = MessageBox.Show(Resource.revoke_fail);
            }

            UpdateButtonRevokeClickable();
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
            else if (names.Resolved)
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
            // 视频按单组统一计算集号
            NumberResolver.ResolveVSFileList(names.videos);

            // 字幕用分组的方式，按组计算集号
            double.TryParse(textBox_min_match_rate.Text, out double _r);
            NumberResolver.ResolveVSFileListBYGroup(names.subs, _r);

            // ========== 步骤3：处理界面渲染（基于已有文件+新计算的集号） ==========
            // 清理现有界面
            panel_filelist.Controls.Clear();

            //匹配视频字幕文件
            var groups = Renamer.GetPairedVSFileGroups(names.videos, names.subs);

            // 渲染每个视频及匹配的字幕
            SetFileListUI(groups);
        }

        /// <summary>
        /// 按分好组的文件添加界面控件
        /// </summary>
        /// <param name="groups"></param>
        private void SetFileListUI(List<PairedVSFileGroup> groups)
        {
            foreach (var group in groups)
            {
                var video = group.Video;
                var video_name = video != null ? video.File.Name : Resource.other_sub_filename;
                var video_file = video?.File;
                // 创建组面板
                Panel videoPanel = CreateNewChildPanel();
                // 创建视频标签
                Label videoLabel = CreateNewFileLabel(video_name, NAME_VIDEO_LABEL, video_file);
                //视频标签添加到面板中
                AddNewSubLabel(videoPanel, videoLabel);

                foreach (var sub in group.Subs)
                {
                    // 创建字幕标签
                    Label subLabel = CreateNewFileLabel(sub.File.Name, NAME_SUB_LABEL, sub.File);
                    AddNewSubLabel(videoPanel, subLabel);
                }

                AddChildrenPanel(videoPanel);
            }
        }

        private void LoadNames_Reslobered(Names names)
        {
            List<FileInfo> allsubs = new List<FileInfo>();
            foreach (VSFile var in names.subs)
            {
                allsubs.Add(var.File);
            }

            foreach (Video video in names.videos)
            {
                string num = video.Num;
                if (num == null || num == "")
                {
                    continue;
                }
                List<FileInfo> subs = Renamer.GetSubList(names, num);
                Panel panel = CreateNewChildPanel();
                Label label_v = CreateNewFileLabel(video.File.Name, NAME_VIDEO_LABEL, video.File);
                AddNewSubLabel(panel, label_v);

                foreach (FileInfo sub in subs)
                {
                    Label label_s = CreateNewFileLabel(sub.Name, NAME_SUB_LABEL, sub);
                    AddNewSubLabel(panel, label_s);
                    _ = allsubs.Remove(sub);
                }
                AddChildrenPanel(panel);
            }

            Panel panel_1 = CreateNewChildPanel();
            Label label_v1 = CreateNewFileLabel(Resource.other_sub_filename, NAME_VIDEO_LABEL, null);
            AddNewSubLabel(panel_1, label_v1);
            AddChildrenPanel(panel_1);
            foreach (FileInfo sub in allsubs)
            {
                Label label_s = CreateNewFileLabel(sub.Name, NAME_SUB_LABEL, sub);
                AddNewSubLabel(panel_1, label_s);
            }
        }

        private void LoadNames_Regex(Names names)
        {
 
            NumberResolver.ResolveVSFileListBYRegex(names.videos, names.GetVideoReplasePattern());
            NumberResolver.ResolveVSFileListBYRegex(names.subs, names.GetSubReplasePattern());

            // 清理现有界面
            panel_filelist.Controls.Clear();

            //匹配视频字幕文件
            var groups = Renamer.GetPairedVSFileGroups(names.videos, names.subs);

            // 渲染每个视频及匹配的字幕
            SetFileListUI(groups);
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
                AutoSize = false,
                MinimumSize = Size.Empty,
                MaximumSize = Size.Empty
            };
            //pannel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            //panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            panel.Margin = new Padding(3);
            panel.Padding = new Padding(3);
            panel.DragDrop += new DragEventHandler(DragDrop_Panel);
            panel.DragEnter += new DragEventHandler(DragEnter_Panel);
            panel.DragLeave += new EventHandler(DragLeave_Panel);
            return panel;
        }

        private void DragDrop_Panel(object sender, DragEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripProgressBar1.Text = "DragDrop";
#endif
            if (sender is Panel _s)
            {
                if (dragSubTraget != null)
                {
                    if (!_s.Controls.Contains(dragSubTraget))
                    {
                        //e.Effect = DragDropEffects.Move;
                        if (dragSubTraget.Parent is Panel _p)
                            RemoveSubLabel(_p, dragSubTraget);
                        AddNewSubLabel(_s, dragSubTraget);
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
                if (dragSubTraget != null)
                {
                    if (!_s.Controls.Contains(dragSubTraget))
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


        private void AddNewSubLabel(Panel panel, Label label)
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
            if (label.Name == NAME_VIDEO_LABEL)
            {
                if (video == null)
                {
                    label.Location = new Point(3, 3);
                    panel.Controls.Add(label);
                    resize = true;
                }
            }
            else if (label.Name == NAME_SUB_LABEL)
            {
                label.Location = video != null ? new Point(video.Left + 15, buttom + 3) : new Point(panel.Left + 18, buttom + 3);
                panel.Controls.Add(label);
                buttom = label.Bottom + 3;
                resize = true;
            }

            if (resize)
            {
                panel.Height = buttom;
                if (panel.Parent != null) ResizeChildPanel(panel.Parent);
            }
        }

        private void RemoveSubLabel(Panel panel, Label label)
        {
            panel.Controls.Remove(label);

            List<Label> list = new List<Label>();
            Label video = null;

            foreach (object lab in panel.Controls)
            {
                if (lab is Label _lab)
                {
                    if (_lab.Name == NAME_SUB_LABEL)
                    {
                        list.Add(_lab);
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

        private Label CreateNewFileLabel(string text, string name, FileInfo file)
        {
            int child_panel_width = panel_filelist.Width - 6;
            int textW,textH = 23;
            Label label = null;
            if (name == NAME_VIDEO_LABEL)
            {
                label = new Label
                {
                    BackColor = COLOR_VIDEOLABEL,
                    Text = text,
                    AutoSize = false,
                    Margin = new Padding(3),
                    Padding = new Padding(3),
                    Name = name,
                    Tag = file,
                    TextAlign = ContentAlignment.TopLeft,
                    AutoEllipsis = true,
                    MinimumSize = Size.Empty,
                    Size = new Size(child_panel_width - 6, 23)
                };
            }
            else if (name == NAME_SUB_LABEL)
            {
                label = new Label
                {
                    BackColor = COLOR_SUBLABEL,
                    Text = text,
                    AutoSize = false,
                    Margin = new Padding(3),
                    Padding = new Padding(3),
                    Name = name,
                    Tag = file,
                    TextAlign = ContentAlignment.TopLeft,
                    AutoEllipsis = true,
                    MinimumSize = Size.Empty,
                    Size = new Size(child_panel_width - 6 - 15, 23)
                };
                label.MouseDown += new MouseEventHandler(SubLabel_MouseDown);
                label.MouseUp += new MouseEventHandler(SubLabel_MouseUp);
                label.MouseMove += new MouseEventHandler(SubLabel_MouseMove);
                label.QueryContinueDrag += new QueryContinueDragEventHandler(SubLabel_QueryContinueDrag);
            }
            else
            {
                return null;
            }
            textW = TextRenderer.MeasureText(text, label.Font).Width;

            label.MaximumSize = new Size(textW + 6, textH);

            return label;

        }

        private void SubLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragSubTraget != null)
            {
#if DEBUG
                //手动调整窗口时debug用
                this.toolStripStatusLabel1.Text = "MouseMove (" + e.X + "," + e.Y + ")";
#endif
                _ = dragSubTraget.DoDragDrop(dragSubTraget.Text, DragDropEffects.Move);
                SetDragTraget(null);
            }
        }

        private void SubLabel_MouseUp(object sender, MouseEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripStatusLabel1.Text = "MouseUp";
#endif
            SetDragTraget(null);
        }


        private void SubLabel_MouseDown(object sender, MouseEventArgs e)
        {
#if DEBUG
            //手动调整窗口时debug用
            this.toolStripStatusLabel1.Text = "MouseDown";
#endif
            if (typeof(Label).IsInstanceOfType(sender))
            {
                if (dragSubTraget != null)
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
                if (dragSubTraget != null)
                {
                    dragSubTraget.BackColor = COLOR_SUBLABEL;
                }
                dragSubTraget = null;
                toolStripStatusLabel1.Text = null;
            }
            else
            {
                dragSubTraget = label;
                toolStripStatusLabel1.Text = dragSubTraget.Text;
                dragSubTraget.BackColor = COLOR_SUBLABEL_HIGHLIHGT;
            }

        }


        private void SubLabel_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (sender is Control _s)
            {
                Form f = _s.FindForm();
                if (f != null)
                {
                    Point screenLocation_Panel1 = panel_filelist.PointToScreen(Point.Empty);
                    Point screenOffset = SystemInformation.WorkingArea.Location;

#if DEBUG
                    //手动调整窗口时debug用
                    this.toolStripStatusLabel1.Text = "m (" + (Control.MousePosition.X - screenLocation_Panel1.X) + "," + (Control.MousePosition.Y - screenLocation_Panel1.Y) + ") L ("
                        + panel_filelist.AutoScrollPosition.X + "," + panel_filelist.AutoScrollPosition.Y + ") " + panel_filelist.Height.ToString();
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



        private void SetExtText_BoxVisable(bool visable)
        {
            textBox_subExt.Visible = visable;
            textBox_videoExt.Visible = visable;
        }

        private void TextBox_checkNum(object sender, EventArgs e)
        {
            if (sender is TextBox _s)
            {
                if (!CheckInputNumberRange(_s, 0, 1))
                    _s.BackColor = COLOR_WARNING;
                else
                    _s.BackColor = COLOR_NORMAL;
            }
        }

        private bool CheckInputNumberRange(TextBox s, double min, double max)
        {
            string input = s.Text;
            // 判空
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (!double.TryParse(input, out double num))
            {
                return false;
            }

            if (num < min || num > max)
            {
                return false;
            }
            return true;
        }


        private void OnKeyDown_LoseFocus(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                // 原生属性，移除焦点
                this.ActiveControl = null;
            }
        }

        /// <summary>
        /// 工具方法：判断是否是外部拖入的文件夹
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsExternalFolder(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return false;
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths == null || paths.Length == 0) return false;
            return Directory.Exists(paths[0]);
        }

        /// <summary>
        /// 递归获取所有子控件
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private IEnumerable<Control> GetAllChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (var grandChild in GetAllChildren(child))
                    yield return grandChild;
            }
        }

        /// <summary>
        /// 临时禁用所有子控件的 AllowDrop
        /// </summary>
        private void DisableChildDrop()
        {
            _savedAllowDrop.Clear();
            foreach (var ctrl in GetAllChildren(this))
            {
                if (ctrl.AllowDrop)
                {
                    _savedAllowDrop[ctrl] = true;
                    ctrl.AllowDrop = false;
                }
            }
        }

        /// <summary>
        /// 恢复所有子控件的 AllowDrop
        /// </summary>
        private void RestoreChildDrop()
        {
            foreach (var kv in _savedAllowDrop)
            {
                kv.Key.AllowDrop = kv.Value;
            }
            _savedAllowDrop.Clear();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            _isExternalFolderDrag = false;
            //this.Invalidate();

            // ★ 恢复子控件拖拽能力
            RestoreChildDrop();

            if (!IsExternalFolder(e)) return;

            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            string folderPath = paths[0];

            // ===== 在这里处理拖入的文件夹 =====
            textBox_path.Text = folderPath;
            Button_name2_Click(sender, null);
            //MessageBox.Show($"收到文件夹：\n{folderPath}");
            // ================================
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (IsExternalFolder(e))
            {
                _isExternalFolderDrag = true;

                // ★ 关键：禁用子控件的拖拽，阻止它们响应
                DisableChildDrop();

                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // 不是外部文件夹 → 不处理，让子控件正常响应
                _isExternalFolderDrag = false;
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {
            _isExternalFolderDrag = false;

            // ★ 恢复子控件拖拽能力
#if DEBUG
            debug_drop_leave++;
            toolStripStatusLabel1.Text = "debug_drop_leave : " + debug_drop_leave;
#endif
            RestoreChildDrop();
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (_isExternalFolderDrag)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}