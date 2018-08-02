using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    class Names
    {
        String path;
        public LinkedList<FileInfo> videos = new LinkedList<FileInfo>();
        public LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
        //LinkedList<DirectoryInfo> directories = new LinkedList<DirectoryInfo>();
        public LinkedList<Names> names = new LinkedList<Names>();
        public Names(DirectoryInfo dInfo)
        {
            this.path = dInfo.Name;
            setNames(dInfo);
        }

        private void setNames(DirectoryInfo dInfo)
        {
            if (dInfo.Exists)
            {
                foreach (var item in dInfo.GetFiles())
                {
                    if (isVideo(item))
                    {
                        this.videos.AddLast(item);
                    }
                    else if (isSub(item))
                    {
                        this.subs.AddLast(item);
                    }
                }

                foreach (var dir in dInfo.GetDirectories())
                {
                    Names name = new Names(dir);
                    this.names.AddLast(name);
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
            addNodes(videos, names.videos);
            root.Nodes.Add(videos);

            TreeNode subs = new TreeNode("subs");
            addNodes(subs, names.subs);
            root.Nodes.Add(subs);

            TreeNode directories = new TreeNode("directories");
            addNodes(directories, names.names);
            root.Nodes.Add(directories);
            return root;
        }

        private void addNodes(TreeNode directories, LinkedList<Names> names)
        {
            foreach (var name in names)
            {
                TreeNode node = getNodeByNames(name);
                directories.Nodes.Add(node);
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
    }
}
