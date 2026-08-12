using System;
using System.Collections.Generic;
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
        private static readonly string regex_episode = @"(?i)episode";
        private static readonly string regex_ep = @"(?i)ep";
        private static readonly Dictionary<string, string> Redo_Log = new Dictionary<string, string>();


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
            if (Redo_Log.ContainsKey(oldname))
            {
                _ = Redo_Log.Remove(oldname);
            }

            Redo_Log.Add(oldname, newname);
        }

        public static void ClearRedoDic()
        {
            Redo_Log.Clear();
        }

        public static bool Revoke()
        {
            Dictionary<string, string>.Enumerator e = Redo_Log.GetEnumerator();
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
            return Redo_Log.Count > 0;
        }

        /// <summary>
        /// 获取不含扩展名的完整文件名
        /// </summary>
        /// <param name="video">文件信息</param>
        /// <returns>不含扩展名的完整路径</returns>
        private static string GetFullNameWithOutExtension(FileInfo video)
        {
            for (int i = video.FullName.Length - 1; i >= 0; i--)
            {
                if (video.FullName[i] == '.')
                {
                    return video.FullName.Substring(0, i);
                }
            }
            return video.FullName;
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
        internal static List<T> GetSubListByNum<T>(List<T> list, string num) where T : VSFile
        {
            List<T> result = new List<T>();
            foreach (T file in list)
            {
                if (file.Num == num) result.Add(file);
                else if (file.Num.Contains(".") == num.Contains("."))
                {
                    if (
                        double.TryParse(
                            file.Num,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double d1)
                        &&
                        double.TryParse(
                            num,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double d2)
                        )
                    {
                        if (d1 == d2) result.Add(file);
                    }
                }
            }
            return result;
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


        private static bool IsFit(FileInfo sub, string num)
        {
            string subNum = GetEpisodeNumber(sub);
            if (subNum != null)
            {
                if (subNum == num)
                {
                    return true;
                }
                else if (
                        double.TryParse(
                            subNum,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double d1
                            )
                        &&
                        double.TryParse(
                            num,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double d2
                            )
                        )
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


        /// <summary>
        /// 将文件名打散用于分组计算集号位置，比Split方法打的更细碎
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        internal static List<string> SplitFileNameForGrouping(FileInfo file)
        {
            var extension = GetFullExtension(file);
            var filename = file.Name.Replace(extension, "");

            if(getSeplitorCount(filename) > 2) {
                return Split(filename);
            }

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

        private static int getSeplitorCount(string filename)
        {
            int count = 0;
            foreach (var item in filename)
            {
                switch (item)
                {
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case '_':
                    case '-':
                        count++;
                        break;
                    default: break;
                }
            }
            return count;
        }



        /// <summary>
        /// 处理集号，去掉ep，第，集之类的字符，尽量保留纯数字
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>解析后的集号</returns>
        internal static string ResolveEpisodeNumber(string str)
        {
            // 去除 Episode/ep 前缀（大小写不敏感）
            string result = Regex.Replace(str, regex_episode, "");
            result = Regex.Replace(result, regex_ep, "");
            // 去除集号前缀（如"第"、"話"、"话"、"集"）
            result = Regex.Replace(result, regex_headAndTail, "");
            return result;
        }

        /// <summary>
        /// 获取文件名中第一个疑似集号的部分
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        internal static string GetEpisodeNumber(FileInfo video)
        {
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

                if (!double.TryParse(str2, out double f))
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
            s = s.Replace('-', ' ');
            s = s.Replace('_', ' ');
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

        /// <summary>
        /// 给所有视频匹配字幕，返回列表，列表最后是匹配不到视频的字幕
        /// </summary>
        /// <param name="allVideos"></param>
        /// <param name="subs"></param>
        /// <returns></returns>
        internal static List<PairedVSFileGroup> GetPairedVSFileGroups(List<Video> allVideos, List<Sub> subs)
        {
            var result = new List<PairedVSFileGroup>();
            var allSubs = new List<Sub>(subs);
            foreach (var video in allVideos)
            {
                var group = new PairedVSFileGroup(video);
                result.Add(group);
                string episodeNum = video.Num;
                if (!string.IsNullOrEmpty(episodeNum))
                {
                    var matchedSubs = GetSubListByNum(allSubs, episodeNum);
                    foreach (var sub in matchedSubs)
                    {
                        group.AddSub(sub);
                        allSubs.Remove(sub);
                    }
                }
            }
            var endGroup = new PairedVSFileGroup(null);
            result.Add(endGroup);
            foreach (var sub in allSubs)
            {
                endGroup.AddSub(sub);
            }
            return result;
        }
    }
}
