using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SubRenamer
{
    internal class Renamer
    {
        //private static String regex  = "(10[Bb][Ii][Tt])|([xXhH]26[45])|(\\d+([\\*Xx])\\d+)|([0-9]{2,5}([pP]))|(\\[[0-9a-fA-F]{8}\\])|(YYDM-11FANS)|([a-zA-Z]{2,5}([Rr][Ii][Pp]))|([0-9a-zA_Z\\s]{5,200})";
        private static readonly string regex = "(10[Bb][Ii][Tt])|([xXhH]26[45])|(\\d+([\\*Xx])\\d+)|([0-9]{2,5}([pP]))|(\\[[0-9a-fA-F]{8}\\])|(YYDM-11FANS)|([a-zA-Z]{2,5}([Rr][Ii][Pp]))|([0-9a-zA-Z_]{6,200})";
        // private static String regex2 = "(10[Bb][Ii][Tt])|([xXhH]26[45])|(\\d+([\\*Xx])\\d+)|(\\[[0-9a-fA-F]{8}\\])|(YYDM-11FANS)|([a-zA-Z]{2,5}([Rr][Ii][Pp]))";
        private static readonly string regex_headAndTail = "第|話|话|集";
        private static readonly Dictionary<string, string> redo = new Dictionary<string, string>();
        public static void Rename(Names names, BackgroundWorker bkWorker)
        {
            if (names.IsRegex)
            {
                Rename_Regex(names, bkWorker);
            }
            else if (names.Reslovered)
            {
                Rename_Reslobered(names, bkWorker);
            }
            else
            {
                Rename(names, bkWorker, 0);
            }
        }

        private static void Rename_Reslobered(Names names, BackgroundWorker bkWorker)
        {
            int c = 0;
            foreach (Video video in names.videos)
            {
                bkWorker?.ReportProgress(++c, video.file.Name);
                string num = video.num; ;
                if (num == null || num == "")
                {
                    continue;
                }
                LinkedList<FileInfo> subs = GetSubList(names, num);
                RenameSubs(video.file, subs);

            }
        }

        public static void Rename(Names names, BackgroundWorker bkWorker, int count)
        {
            int c = count;
            foreach (Video video in names.videos)
            {
                bkWorker?.ReportProgress(++c, video.file.Name);
                string num = GetVideoNumber(video.file);
                if (num == null)
                {
                    continue;
                }
                LinkedList<FileInfo> subs = GetSubList(names, num);
                RenameSubs(video.file, subs);

            }
            foreach (Names name in names.names)
            {
                Rename(name, bkWorker, c);
            }

        }

        private static void Rename_Regex(Names names, BackgroundWorker bkWorker)
        {
            Dictionary<FileInfo, string> videoDic = GetDic(names.GetVideoFileList(), names.GetVideoReplasePattern());
            Dictionary<FileInfo, string> subDic = GetDic(names.subs, names.GetSubReplasePattern());
            int c = 0;
            foreach (FileInfo video in videoDic.Keys)
            {
                bkWorker?.ReportProgress(++c, video.Name);
                LinkedList<FileInfo> subs = GetSubList(subDic, videoDic[video]);
                RenameSubs(video, subs);
            }
        }

        internal static void RenameSubs(FileInfo video, LinkedList<FileInfo> subs)
        {
            string vname = GetFullNameWithOutExtension(video);
            foreach (FileInfo sub in subs)
            {
                string ext = GetFullExtension(sub);
                try
                {
                    string new_name = vname + ext;
                    SetRedoDic(sub.FullName, new_name);
                    sub.MoveTo(new_name);
                }
                catch
                {
                    string new_name = vname + "." + sub.Name;
                    SetRedoDic(sub.FullName, new_name);
                    sub.MoveTo(new_name);
                }
            }
        }

        private static void SetRedoDic(string oldname, string newname)
        {
            if (redo.ContainsKey(oldname))
            {
                _ = redo.Remove(oldname);
            }

            redo.Add(oldname, newname);
        }

        public static void ClearRedoDic()
        {
            redo.Clear();
        }

        public static bool Redo()
        {
            Dictionary<string, string>.Enumerator e = redo.GetEnumerator();
            while (e.MoveNext())
            {
                string old = e.Current.Key;
                FileInfo newfile = new FileInfo(e.Current.Value);
                if (newfile.Exists)
                {
                    try
                    {
                        newfile.MoveTo(old);
                    }
                    catch
                    {
                        ClearRedoDic();
                        return false;
                    }
                }
            }
            ClearRedoDic();
            return true;
        }

        public static bool IsRedoAvailable()
        {
            return redo.Count != 0;
        }

        internal static Dictionary<FileInfo, string> GetDic(LinkedList<FileInfo> videos, string p)
        {
            Dictionary<FileInfo, string> dic = new Dictionary<FileInfo, string>();
            foreach (FileInfo video in videos)
            {
                string name = video.Name;
                string str = System.Text.RegularExpressions.Regex.Replace(name, p, "");
                dic.Add(video, str);
            }
            return dic;
        }

        private static string GetFullNameWithOutExtension(FileInfo video)
        {
            char[] cs = video.FullName.ToArray();
            for (int i = cs.Length - 1; i >= 0; i--)
            {
                if (cs[i] == '.')
                {
                    return new string(cs).Substring(0, i);
                }
            }
            return cs.ToString();
        }

        private static string GetFullExtension(FileInfo sub)
        {
            string name = sub.Name.Trim();
            char[] cs = name.ToArray();
            List<int> index = new List<int>();
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i] == '.')
                {
                    index.Add(i);
                }
            }

            for (int i = 0; i < index.Count; i++)
            {
                if (i == index.Count - 1)
                {
                    return sub.Extension;
                }
                if (index[i + 1] - index[i] <= 10)
                {
                    return name.Substring(index[i]);
                }
                string ext = name.Substring(index[i]);
                string ext2 = System.Text.RegularExpressions.Regex.Replace(ext, regex, "");
                if (ext == ext2)
                {
                    return ext;
                }
            }
            return sub.Extension;
        }

        internal static LinkedList<FileInfo> GetSubList(Names names, string num)
        {
            LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
            foreach (FileInfo sub in names.subs)
            {
                if (IsFit(sub, num))
                {
                    _ = subs.AddLast(sub);
                }
            }
            return subs;
        }

        internal static LinkedList<FileInfo> GetSubList(Dictionary<FileInfo, string> subDic, string key)
        {
            LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
            foreach (FileInfo sub in subDic.Keys)
            {
                if (subDic[sub].Equals(key))
                {
                    _ = subs.AddLast(sub);
                    // subDic.Remove(sub);
                }
            }
            return subs;
        }

        private static bool IsFit(FileInfo sub, string num)
        {
            string subNum = GetVideoNumber(sub);
            if (subNum != null)
            {
                if (subNum == num)
                {
                    return true;
                }
                else if (float.Parse(subNum) == float.Parse(num))
                {
                    return true;
                }
            }
            else
            {
                string name = sub.Name.Replace(sub.Extension, "");
                name = System.Text.RegularExpressions.Regex.Replace(name, regex, "");
                if (IsFitNum(name, num))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFitNum(string name, string num)
        {
            char[] na = name.ToCharArray();
            char[] nm = num.ToCharArray();
            for (int i = 0; i < na.Length - nm.Length + 1; i++)
            {
                bool ifcontinue = false;
                if (na[i] == nm[0])
                {
                    int j = 1;
                    for (; j < nm.Length; j++)
                    {
                        if (na[i + j] != nm[j])
                        {
                            ifcontinue = true;
                            break;
                        }
                    }
                    if (ifcontinue)
                    {
                        continue;
                    }

                    if (i + j < na.Length)
                    {
                        if (na[i + j] >= '0' && na[i + j] <= '9')
                        {
                            continue;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        internal static string GetVideoNumber(FileInfo video)
        {
            string name = (string)video.Name.Clone();
            name = name.Replace(video.Extension, "");
            LinkedList<string> strs = Split(name);
            foreach (string str in strs)
            {
                string str2 = str;
                while (str2.ToLower().Contains("ep"))
                {
                    char[] p = { 'p', 'P' };
                    int index = str2.IndexOfAny(p);
                    str2 = str2.Substring(index + 1);
                }

                str2 = System.Text.RegularExpressions.Regex.Replace(str2, regex_headAndTail, "");

                if (!float.TryParse(str2, out float f))
                {
                    continue;
                }

                if (f < 0 || f > 1900)
                {
                    continue;
                }

                return str2;

            }
            return null;
        }

        private static LinkedList<string> Split(string name)
        {
            LinkedList<string> result = new LinkedList<string>();
            string name2 = Replace(name);
            char[] ca = name2.ToCharArray();
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == ' ')
                {
                    try
                    {
                        int end = FindMatchingPos(ca, i, ' ');
                        _ = result.AddLast(name2.Substring(i + 1, end - i - 1));
                    }
                    catch
                    {
                        _ = result.AddLast(name2.Substring(i));
                    }
                }
            }
            return result;
        }

        private static string Replace(string name)
        {
            string s = name.Replace('[', ' ');
            s = s.Replace(']', ' ');
            s = s.Replace('(', ' ');
            s = s.Replace(')', ' ');
            s = s.Replace('{', ' ');
            s = s.Replace('}', ' ');
            //s = s.Replace('.', ' ');
            s = System.Text.RegularExpressions.Regex.Replace(s, "[\\s]+", " ");
            return s;
        }

        private static int FindMatchingPos(char[] ca, int begin, char left)
        {
            char right;
            switch (left)
            {
                case '[':
                    right = ']';
                    break;
                case '(':
                    right = ')';
                    break;
                case '{':
                    right = '}';
                    break;
                case ' ':
                    right = left;
                    break;
                default:
                    throw new Exception("cannot get matching char on RIGHT");
            }
            int count = 0;
            for (int i = begin + 1; i < ca.Length; i++)
            {

                if (ca[i] == right)
                {
                    if (count == 0)
                    {
                        return i;
                    }
                    else
                    {
                        count--;
                    }
                }
                else if (ca[i] == left)
                {
                    count++;
                }
            }
            throw new Exception("cannot find matching pos");
        }
    }
}
