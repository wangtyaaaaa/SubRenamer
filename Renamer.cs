using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
            else if (names.Resolved)
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
                bkWorker?.ReportProgress(++c, video.File.Name);
                if (video.Num != null && video.Num != "")
                {
                    // string num = video.num;
                    // if (num == null || num == "")
                    // {
                    //     continue;
                    // }
                    List<FileInfo> subs = GetSubList(names, video.Num);
                    RenameSubs(video.File, subs, null);
                }
            }
        }

        public static void Rename(Names names, BackgroundWorker bkWorker, int count)
        {
            int c = count;
            foreach (Video video in names.videos)
            {
                bkWorker.ReportProgress(++c, video.File.Name);
                string num = GetEpisodeNumber(video.File);
                if (num == null)
                {
                    continue;
                }
                List<FileInfo> subs = GetSubList(names, num);
                RenameSubs(video.File, subs, null);

            }
            foreach (Names name in names.names)
            {
                Rename(name, bkWorker, c);
            }

        }

        private static void Rename_Regex(Names names, BackgroundWorker bkWorker)
        {
            Dictionary<FileInfo, string> videoDic = GetDic(VSFile.FileListTOFileInfoList(names.videos), names.GetVideoReplasePattern());
            Dictionary<FileInfo, string> subDic = GetDic(VSFile.FileListTOFileInfoList(names.subs), names.GetSubReplasePattern());
            int c = 0;
            foreach (FileInfo video in videoDic.Keys)
            {
                bkWorker?.ReportProgress(++c, video.Name);
                List<FileInfo> subs = GetSubList(subDic, videoDic[video]);
                RenameSubs(video, subs, null);
            }
        }

        internal static void RenameSubs(FileInfo video, List<FileInfo> subs, string delimiter)
        {
            string vname = GetFullNameWithOutExtension(video);
            foreach (FileInfo sub in subs)
            {
                string ext = GetFullExtension(sub, delimiter);
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

        public static bool IsRedoAvailabel()
        {
            return redo.Count != 0;
        }

        internal static Dictionary<FileInfo, string> GetDic(List<FileInfo> videos, string p)
        {
            Dictionary<FileInfo, string> dic = new Dictionary<FileInfo, string>();
            foreach (FileInfo video in videos)
            {
                string name = video.Name;
                string str = Regex.Replace(name, p, "");
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
            string str = cs.ToString();
            return str ?? "";
        }


        private static string GetFullExtension(FileInfo sub, string delimiter)
        {
            if (delimiter == null || delimiter.Length == 0)
                return GetFullExtension(sub);
            string name = sub.Name.Trim();
            int index = name.LastIndexOf(delimiter[0]);
            if (index == -1)
                return GetFullExtension(sub);
            return name.Substring(index);
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
                string ext2 = Regex.Replace(ext, regex, "");
                if (ext == ext2)
                {
                    return ext;
                }
            }
            return sub.Extension;
        }


        /// <summary>
        /// 使用存储的集号来获取字幕文件
        /// </summary>
        /// <param name="names"></param>
        /// <param name="num">集号</param>
        /// <returns>字幕文件列表</returns>
        internal static List<FileInfo> GetSubListByNum(Names names, string num)
        {
            List<FileInfo> subs = new List<FileInfo>();
            foreach (Sub sub in names.subs)
            {
                if (sub.Num == num) subs.Add(sub.File);
            }
            return subs;
        }

        internal static List<FileInfo> GetSubList(Names names, string num)
        {
            List<FileInfo> subs = new List<FileInfo>();
            foreach (Sub sub in names.subs)
            {
                if (IsFit(sub.File, num))
                {
                    subs.Add(sub.File);
                }
            }
            return subs;
        }

        internal static List<FileInfo> GetSubList(Dictionary<FileInfo, string> subDic, string key)
        {
            List<FileInfo> subs = new List<FileInfo>();
            foreach (FileInfo sub in subDic.Keys)
            {
                if (subDic[sub].Equals(key))
                {
                    subs.Add(sub);
                    // subDic.Remove(sub);
                }
            }
            return subs;
        }

        private static bool IsFit(FileInfo sub, string num)
        {
            string subNum = GetEpisodeNumber(sub);
            if (subNum != null)
            {
                if (subNum == num)
                {
                    return true;
                }
                //else if (double.Parse(subNum) == double.Parse(num))
                //{
                //    return true;
                //}
                else if (double.TryParse(subNum, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double d1) &&
         double.TryParse(num, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double d2))
                {
                    if (d1 == d2) return true;
                }
            }
            else
            {
                string name = sub.Name.Replace(sub.Extension, "");
                name = Regex.Replace(name, regex, "");
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


        ///// <summary>
        ///// （重构后）提取文件名中所有匹配的数字字符串（和原GetVideoNumber算法一致）
        ///// </summary>
        ///// <param name="file">文件信息</param>
        ///// <returns>匹配的数字字符串数组（无匹配返回空数组）</returns>
        //public static string[] GetEpisodeNumberArray(FileInfo video)
        //{
        //    if (video == null || string.IsNullOrEmpty(video.Name))
        //        return Array.Empty<string>();

        //    var result = new List<string>(); ;
        //    // 复用原GetVideoNumber的匹配逻辑，提取所有匹配项
        //    List<string> strs = SplitFileNameWithoutExtension(video);
        //    foreach (string str in strs)
        //    {
        //        string str2 = ResolveEpisodeNumber(str);

        //        if (!float.TryParse(str2, out float f))
        //        {
        //            continue;
        //        }

        //        if (f < 0 || f > 1900)
        //        {
        //            continue;
        //        }


        //        result.Add(str2);
        //    }
        //    return result.ToArray();
        //}

        /// <summary>
        /// 判断str是不是疑似集号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static bool IsLikelyEpisodeNumber(string str)
        {
            string str2 = ResolveEpisodeNumber(str);

            if (!double.TryParse(str2, out double f))
            {
                return false;
            }

            if (f < 0 || f > 1900)
            {
                return false;
            }

            return true;
        }

        internal static List<string> SplitFileNameWithoutExtension(FileInfo file)
        {
            var name = file.Name.Replace(file.Extension, "");
            List<string> strs = Split(name);
            return strs;
        }

        /// <summary>
        /// 将文件名打散用于分组计算集号位置，比Split方法打的更细碎
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        internal static List<string> SplitFileNameForGrouping(FileInfo file)
        {
            var filename = file.Name.Replace(file.Extension, "");
            List<string> result = new List<string>();
            StringBuilder current = new StringBuilder();

            for (int i = 0; i < filename.Length; i++)
            {
                char c = filename[i];

                // 【新增拦截器】如果是小数点（前是数字，后也是数字），直接“吃掉”它和下一个字符
                if (c == '.' && current.Length > 0 && i + 1 < filename.Length)
                {
                    bool prevIsDigit = char.IsDigit(current[current.Length - 1]);
                    bool nextIsDigit = char.IsDigit(filename[i + 1]);

                    if (prevIsDigit && nextIsDigit)
                    {
                        current.Append(c);           // 加入 '.'
                        current.Append(filename[i + 1]); // 加入下一个数字
                        i++;                         // 索引跳过下一个字符
                        continue;                    // 直接进入下一次循环
                    }
                }

                // --- 以下是你原有的逻辑，一行都不用改 ---

                // 原有的分隔符判断
                if (c == ' ' || c == '.' || c == '_' || c == '-' ||
                    c == '[' || c == ']' || c == '(' || c == ')' ||
                    c == '{' || c == '}')
                {
                    if (current.Length > 0)
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                {
                    // 检测数字与非数字的边界
                    if (current.Length > 0)
                    {
                        bool lastIsDigit = char.IsDigit(current[current.Length - 1]);
                        bool currentIsDigit = char.IsDigit(c);

                        // 如果上一个字符是数字而当前不是，或者上一个不是数字而当前是，则分割
                        if (lastIsDigit != currentIsDigit)
                        {
                            result.Add(current.ToString());
                            current.Clear();
                        }
                    }
                    current.Append(c);
                }
            }

            // 添加最后一个片段
            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }

            return result;
        }



        /// <summary>
        /// 处理集号，去掉ep，第，集之类的字符，尽量保留纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string ResolveEpisodeNumber(string str)
        {
            string str2 = str;
            while (str2.ToLower().Contains("Episode"))
            {
                char[] p = { 'p', 'P' };
                int index = str2.IndexOfAny(p);
                str2 = str2.Substring(index + 1);
            }
            while (str2.ToLower().Contains("ep"))
            {
                char[] p = { 'p', 'P' };
                int index = str2.IndexOfAny(p);
                str2 = str2.Substring(index + 1);
            }

            str2 = Regex.Replace(str2, regex_headAndTail, "");
            return str2;
        }

        /// <summary>
        /// 获取文件名中第一个疑似集号的部分
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        internal static string GetEpisodeNumber(FileInfo video)
        {
            //var numbers = GetEpisodeNumberArray(video);
            //return numbers.Length > 0 ? numbers[0] : null;

            string name = (string)video.Name.Clone();
            name = name.Replace(video.Extension, "");
            List<string> strs = Split(name);
            foreach (string str in strs)
            {
                string str2 = str;
                while (str2.ToLower().Contains("ep"))
                {
                    char[] p = { 'p', 'P' };
                    int index = str2.IndexOfAny(p);
                    str2 = str2.Substring(index + 1);
                }

                str2 = Regex.Replace(str2, regex_headAndTail, "");

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


        public static List<string> Split(string name)
        {
            List<string> result = new List<string>();
            string name2 = Replace(name);
            char[] ca = name2.ToCharArray();
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == ' ')
                {
                    try
                    {
                        int end = FindMatchingPos(ca, i, ' ');
                        result.Add(name2.Substring(i + 1, end - i - 1));
                    }
                    catch
                    {
                        result.Add(name2.Substring(i));
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
            s = Regex.Replace(s, "[\\s]+", " ");
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
