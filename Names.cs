using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    class video
    {
        public FileInfo file;
        public string num;

        public video(FileInfo file)
        {
            this.file = file;
        }
    }
    class Names
    {
        public bool isRegex { get; }
        public bool reslovered { get; set; }

        String path;

        public string v_left { get; }
        public string v_right { get; }
        public string s_left { get; }
        public string s_right { get; }

        public LinkedList<video> videos = new LinkedList<video>();
        
        public LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
        //LinkedList<DirectoryInfo> directories = new LinkedList<DirectoryInfo>();
        public LinkedList<Names> names = new LinkedList<Names>();
        //private string v_left;
        //private string v_right;
        //private string s_left;
        //private string s_right;

        public Names(DirectoryInfo dInfo,bool recursion)
        {
            isRegex = false;
            this.path = dInfo.Name;
            setNames(dInfo,recursion);
        }

        public Names(DirectoryInfo dInfo, string v_left, string v_right, string s_left, string s_right)
        {
            isRegex = true;
            this.path = dInfo.Name;
            this.v_left = v_left;
            this.v_right = v_right;
            this.s_left = s_left;
            this.s_right = s_right;
            setNames2(dInfo);
        }

        private void setNames2(DirectoryInfo dInfo)
        {
            if (dInfo.Exists)
            {
                string v_patt = "^" + v_left + "\\S{1,6}" + v_right + "$";
                string s_patt = "^" + s_left + "\\S{1,6}" + s_right + "$";
                //MessageBox.Show("视频：\n"+v_patt + "\n字幕：\n" + s_patt);
                //Regex regex_v = new Regex(v_patt);
                //Regex regex_s = new Regex(s_patt);
                try
                {
                foreach (var item in dInfo.GetFiles())
                {
                    string name = item.Name;
                    if (Regex.IsMatch(item.Name, v_patt))
                    {
                        this.videos.AddLast(new video(item));
                       // Regex.Replace(name, "(" + v_left + ")|(" + v_right + ")", "");
                    }
                    else if (Regex.IsMatch(item.Name, s_patt))
                    {
                        this.subs.AddLast(item);
                    }
                }

                }
                catch(Exception e)
                {
                    MessageBox.Show("匹配错误，请检查表达式\n"+e.Message);
                }
            }
        }

        internal string getSubReplasePattern()
        {
            return "(" + s_left + ")|(" + s_right + ")";
        }

        internal string getVideoReplasePattern()
        {
            return "(" + v_left + ")|(" + v_right + ")";
        }

        private void setNames(DirectoryInfo dInfo,bool recursion)
        {
            if (dInfo.Exists)
            {
                foreach (var item in dInfo.GetFiles())
                {
                    if (isVideo(item))
                    {
                        this.videos.AddLast(new video(item));
                    }
                    else if (isSub(item))
                    {
                        this.subs.AddLast(item);
                    }
                }
                if (recursion)
                {
                    foreach (var dir in dInfo.GetDirectories())
                    {
                        Names name = new Names(dir,true);
                        this.names.AddLast(name);
                    }
                }
            }
        }

        private bool isSub(FileInfo item)
        {
            String a = item.Extension.ToLower();
            if (item.Extension.ToLower() == ".ass" || item.Extension.ToLower() == ".ssa" || item.Extension.ToLower() == "sub")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isVideo(FileInfo item)
        {
            if (item.Extension.ToLower() == ".mp4" || item.Extension.ToLower() == ".mkv")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        internal void setTreeView(System.Windows.Forms.TreeView treeView)
        {
            treeView.Nodes.Clear();
            TreeNode node = getNodeByNames(this);
            treeView.Nodes.Add(node);
            node.Expand();
        }

        private TreeNode getNodeByNames(Names names)
        {
            TreeNode root = new TreeNode(names.path);

            TreeNode videos = new TreeNode("videos");
            LinkedList<string> video_nodes = getVideoStringList();
            addNodes(videos, video_nodes);
            root.Nodes.Add(videos);

            TreeNode subs = new TreeNode("subs");
            LinkedList<string> sub_nodes = getSubStringList();
            addNodes(subs, sub_nodes);
            root.Nodes.Add(subs);


            TreeNode directories = new TreeNode("directories");
            addNodes(directories, names.names);
            root.Nodes.Add(directories);
            return root;
        }


        private LinkedList<string> getVideoStringList()
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

            foreach (var video in videos)
            {
                if (this.isRegex)
                {
                    string str = Regex.Replace(video.file.Name, getVideoReplasePattern(), "");
                    result.AddLast(video.file.Name + "  ---(" + str + ")");
                }
                else if (reslovered)
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

        private LinkedList<string> getSubStringList()
        {
            LinkedList<string> result = new LinkedList<string>();
            foreach (var sub in subs)
            {
                if (this.isRegex)
                {
                    string str = Regex.Replace(sub.Name, getSubReplasePattern(), "");
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


        internal int getVideoCount()
        {
            int count = this.videos.Count;
            foreach (var name in names)
            {
                count += name.getVideoCount();
            }
            return count;
        }

        public LinkedList<FileInfo> getVideoFileList()
        {
            LinkedList<FileInfo> res = new LinkedList<FileInfo>();
            foreach(var v in this.videos)
            {
                res.AddLast(v.file);
            }
            return res;
        }

        public static string[] getStrArray(LinkedList<video> list)
        {
            string[] res = new string[list.Count];
            LinkedListNode<video> node = list.First;

            for (int i = 0; i < list.Count; i++)
            {
                res[i] = node.Value.file.Name;
                node = node.Next;
            }


            return res;
        }

    }


}
