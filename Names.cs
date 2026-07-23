using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SubRenamer
{
    internal static class Extentions
    {
        public const int VIDEO = 1;
        public const int SUB = 2;
        public static HashSet<string> video_ext = new HashSet<string>();
        public static HashSet<string> sub_ext = new HashSet<string>();


        public static void SetExts(string exts, int type)
        {
            string[] strs = exts.Split(',');
            HashSet<string> set;
            switch (type)
            {
                case VIDEO:
                    set = video_ext;
                    break;
                case SUB:
                    set = sub_ext;
                    break;
                default:
                    return;
            }
            set.Clear();
            foreach (string str in strs)
            {
                set.Add("."+str.ToLower());
            }

        }
    }

    internal class VSFile
    {
        /// <summary>
        /// 文件
        /// </summary>
        public FileInfo File { get; }


        /// <summary>
        /// 打散的文件名
        /// </summary>
        public List<string> Splited_filename { get; }

        /// <summary>
        /// 集号
        /// </summary>
        public string Num { get; set; }

        public VSFile(FileInfo file)
        {
            File = file;
            Splited_filename = Renamer.SplitFileNameForGrouping(file);
        }

        //public static List<FileInfo> VSFileListTOFileInfoList<T>(IEnumerable<T> files) where T : VSFile
        //{
        //    var result = new List<FileInfo>();
        //    foreach (var item in files)
        //    {
        //        result.Add(item.File);
        //    }

        //    return result;
        //}
    }

    internal class Sub : VSFile
    {
        public Sub(FileInfo file) : base(file)
        {
        }
    }

    internal class Video : VSFile
    {
        public Video(FileInfo file) : base(file)
        {
        }
    }

    /// <summary>
    /// 配好对的一组视频字幕文件，用于生成界面
    /// </summary>
    internal class PairedVSFileGroup
    {
        /// <summary>
        /// 视频文件
        /// </summary>
        public Video Video { get; set; }

        /// <summary>
        /// 字幕文件列表
        /// </summary>
        public List<Sub> Subs { get; }

        public PairedVSFileGroup(Video v)
        {
            Video = v;
            Subs = new List<Sub>();
        }

        /// <summary>
        /// 添加字幕文件进列表
        /// </summary>
        /// <param name="sub"></param>
        public void AddSub(Sub sub)
        {
            Subs.Add(sub);
        }

    }


    internal class Names
    {
        public bool IsRegex { get; }
        public bool Resolved { get; set; }

        public string path;



        public string Video_Left { get; }
        public string Video_Right { get; }
        public string Sub_Left { get; }
        public string Sub_Right { get; }

        public List<Video> videos = new List<Video>();

        public List<Sub> subs = new List<Sub>();


        public Names(DirectoryInfo dInfo)
        {
            IsRegex = false;
            path = dInfo.Name;
            SetNames(dInfo);
        }

        public Names(DirectoryInfo dInfo, string v_left, string v_right, string s_left, string s_right)
        {
            IsRegex = true;
            path = dInfo.Name;
            Video_Left = v_left;
            Video_Right = v_right;
            Sub_Left = s_left;
            Sub_Right = s_right;
            SetNames2(dInfo);
        }

        private void SetNames2(DirectoryInfo dInfo)
        {
            if (dInfo.Exists)
            {
                string v_patt = "^" + Video_Left + "\\S{1,6}" + Video_Right + "$";
                string s_patt = "^" + Sub_Left + "\\S{1,6}" + Sub_Right + "$";
                //MessageBox.Show("视频：\n"+v_patt + "\n字幕：\n" + s_patt);
                //Regex regex_v = new Regex(v_patt);
                //Regex regex_s = new Regex(s_patt);
                try
                {
                    foreach (FileInfo item in dInfo.GetFiles())
                    {
                        string name = item.Name;
                        if (Regex.IsMatch(item.Name, v_patt))
                        {
                            videos.Add(new Video(item));
                            // Regex.Replace(name, "(" + v_left + ")|(" + v_right + ")", "");
                        }
                        else if (Regex.IsMatch(item.Name, s_patt))
                        {
                            subs.Add(new Sub(item));
                        }
                    }

                }
                catch (Exception e)
                {
                    _ = MessageBox.Show("匹配错误，请检查表达式\n" + e.Message);
                }
            }
        }

        internal string GetSubReplasePattern()
        {
            return "(" + Sub_Left + ")|(" + Sub_Right + ")";
        }

        internal string GetVideoReplasePattern()
        {
            return "(" + Video_Left + ")|(" + Video_Right + ")";
        }

        private void SetNames(DirectoryInfo dInfo)
        {
            if (dInfo.Exists)
            {
                foreach (FileInfo item in dInfo.GetFiles())
                {
                    if (IsVideo(item))
                    {
                        videos.Add(new Video(item));
                    }
                    else if (IsSub(item))
                    {
                        subs.Add(new Sub(item));
                    }
                }
            }
        }

        private bool IsSub(FileInfo item)
        {
            return MatchExtebsion(item, Extentions.sub_ext);
        }

        private bool IsVideo(FileInfo item)
        {
            return MatchExtebsion(item, Extentions.video_ext);
        }


        private bool MatchExtebsion(FileInfo item, HashSet<string> ext_set)
        {
            if (ext_set.Contains(item.Extension.ToLower())) return true;
            else return false;
        }

        internal int GetVideoCount()
        {
            int count = videos.Count;
            return count;
        }

        public static string[] GetStrArray(List<Video> list)
        {
            string[] res = new string[list.Count];

            int i = 0;
            foreach (var item in list)
            {
                res[i++] = item.File.Name;
            }

            return res;
        }

    }


}
