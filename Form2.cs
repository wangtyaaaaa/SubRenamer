using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SubRenamer
{

    public partial class Form2 : Form
    {
        Names names;
        private Form1 form1;
        int count = 0;

        //static int lb_hi = 18;
        static int pl_hi = 20;
        private Label dragTraget;
        private DateTime scrolltime;
        static readonly string name_video_lable = "lable_video";
        static readonly string name_sub_lable = "lable_sub";

        public Form2()
        {
            InitializeComponent();
            setDebugEnable(false);
        }

        private void setDebugEnable(bool v)
        {
            button1.Visible = v;
            button1.Enabled = v;
            button2.Visible = v;
            button2.Enabled = v;
            button3.Visible = v;
            button3.Enabled = v;
            label1.Visible = v;
            label1.Enabled = v;
        }

        internal void setNames(Names names, Form1 form1)
        {
            this.names = names;
            this.form1 = form1;
            loadNames(names);
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            removeAll(this.panel1);
            loadNames(names);
        }

        private void removeAll(Panel panel1)
        {
            panel1.Controls.Clear();
        }

        private void loadNames(Names names)
        {
            if (names == null) return;

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
            foreach(var var in names.subs)
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
            foreach(var sub in allsubs)
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

                foreach(FileInfo sub in subs)
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
            int c = 0;
            foreach (var video in videoDic.Keys)
            {
                Panel panel = getNewChildPanel();
                LinkedList<FileInfo> subs = Renamer.getSubList(subDic, videoDic[video]);
                Label label_v = newFileLable(video.Name, name_video_lable,video);
                addNewSubLable(panel, label_v);
                foreach(FileInfo sub in subs)
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

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (form1 != null)
            {
                form1.Show();
                //form1.Close();
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            

            Panel panel = getNewChildPanel();

            Label lable = newFileLable(String.Concat(count), name_video_lable);
            //lable.MouseEnter += new EventHandler(lable_MouseEnter);
            //lable.MouseLeave += new EventHandler(lable_MouseLeave);
            //lable.Location = new System.Drawing.Point(3, 3);

            Label lable2 = newFileLable(String.Concat(count, count, "aaaaaaaaaaaqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"), name_sub_lable);
            //lable2.Location = new Point(lable.Right+3, 3);

            Label lable3 = newFileLable(String.Concat(count, count, "bbbbbbbbbbbbbbb"), name_sub_lable);

            //lable3.Location = new Point(lable.Right+3, lable2.Bottom);

            addNewSubLable(panel, lable);
            addNewSubLable(panel, lable2);
            addNewSubLable(panel, lable3);


            //panel.Controls.Add(lable);
            //panel.Controls.Add(lable2);
            //panel.Controls.Add(lable3);
            addChildrenPanel(panel);


            count++;
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
                    (p as Panel).Location = new Point(3,butt);
                    butt += (p as Panel).Height + 3;
                }
            }
        }

        private void panelAutoSizeChanged(object sender, EventArgs e)
        {
            (sender as Panel).Width = this.panel1.Width - 30;
        }

        private void label_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = System.Drawing.SystemColors.ControlLightLight;
            ((Label)sender).Parent.BackColor = System.Drawing.SystemColors.ActiveCaption;
        }

        private void label_MouseEnter(object sender, EventArgs e)
        {
            if (typeof(Label).IsInstanceOfType(sender))
            {
                ((Label)sender).BackColor = System.Drawing.SystemColors.ControlDark;
                ((Label)sender).Parent.BackColor = System.Drawing.SystemColors.ControlLightLight;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var panel in this.panel1.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(panel))
                {
                    Label lable = newFileLable("bbbbbbbb", name_sub_lable);
                    addNewSubLable(panel as Panel, lable);
                }
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
                //this.label1.Text = "MouseMove (" + e.X + "," + e.Y + ")";
                dragTraget.DoDragDrop(dragTraget.Text, DragDropEffects.Move);
                setDragTraget(null);
            }
        }

        private void SubLable_MouseDown(object sender, MouseEventArgs e)
        {
            this.label1.Text = "MouseDown";
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
            else {
                dragTraget = label;
                this.toolStripStatusLabel1.Text = dragTraget.Text;
                dragTraget.BackColor = System.Drawing.SystemColors.MenuHighlight;
            }
            
        }

        private void SubLable_MouseUp(object sender, MouseEventArgs e)
        {
            this.label1.Text = "MouseUp";
            setDragTraget(null);
            //dragTraget = null;
            //this.label_file.Text = null;
        }


        private void SubLable_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

            Form f = (sender as Control).FindForm();
            Point screenLocation_Panel1 = PointToScreen(panel1.Location);
            Point screenOffset = SystemInformation.WorkingArea.Location;
            this.label1.Text = "m (" + (Control.MousePosition.X - screenLocation_Panel1.X) + "," + (Control.MousePosition.Y - screenLocation_Panel1.Y) + ") L ("
                + panel1.AutoScrollPosition.X + "," + panel1.AutoScrollPosition.Y + ") "+ panel1.Height;
            //(this.Height - this.ClientRectangle.Height)



            // Cancel the drag if the mouse moves off the form. The screenOffset
            // takes into account any desktop bands that may be at the top or left
            // side of the screen.
            if (((Control.MousePosition.X - screenOffset.X) < f.DesktopBounds.Left) ||
                ((Control.MousePosition.X - screenOffset.X) > f.DesktopBounds.Right) ||
                ((Control.MousePosition.Y - screenOffset.Y) < f.DesktopBounds.Top) ||
                ((Control.MousePosition.Y - screenOffset.Y) > f.DesktopBounds.Bottom))
            {
                label1.Text = "cancel";
                e.Action = DragAction.Cancel;
                setDragTraget(null);
                //dragTraget = null;
                //this.label_file.Text = null;
            }
            else if (Control.MousePosition.Y - screenLocation_Panel1.Y < 15)
            {
                //scrolltime = DateTime.Now;
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

      
        private void DragDrop_Panel(object sender, DragEventArgs e)
        {
            this.label1.Text = "DragDrop";
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
            this.label1.Text = "DragEnter";
            if (dragTraget != null )
            {
                if (typeof(Panel).IsInstanceOfType(sender))
                {
                    if(!(sender as Panel).Controls.Contains(dragTraget))
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


        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var panel in this.panel1.Controls)
            {
                Label sub = null;
                foreach (var lab in (panel as Panel).Controls)
                {
                    if (typeof(Label).IsInstanceOfType(lab))
                    {
                        if ((lab as Label).Name == name_sub_lable)
                        {
                            sub = (lab as Label);
                            break;
                        }
                    }
                }

                if (typeof(Panel).IsInstanceOfType(panel))
                {
                    removeSubLable(panel as Panel, sub);
                }
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

        private void panel1_Resize(object sender, EventArgs e)
        {
            rewidthChildrenPanel(sender);
        }

        private void rewidthChildrenPanel(object sender)
        {
            if (!typeof(Panel).IsInstanceOfType(sender))
            {
                return;
            }
            Panel panel = (sender as Panel);
            int a = 10;

            if (panel.VerticalScroll.Enabled)
            {
                a = a + 10;
            }

            foreach (var p in panel.Controls)
            {
                if (typeof(Panel).IsInstanceOfType(p))
                {
                    (p as Panel).Width = panel.Width - a;
                }
            }
        }

        private void button_doRename_Click(object sender, EventArgs e)
        {

            if (names == null)
            {
                MessageBox.Show("请先获取文件");
                return;
            }
            //if (!form1.ischecked)
            //{
            //    if (!form1.DoCheckMessage())
            //    {
            //        return;
            //    }
            //}
            doRename();

            if (form1 != null)
            {
                form1.Show();
                form1.button_name_Click(null, null);
                this.Close();
                //form1.Close();
            }
        }

        private void doRename()
        {
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
                        Renamer.renameSubs(video, subs);
                    }
                }
            }
        }
    }
}
