using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class Renamer
    {
        //private static String regex  = "(10[Bb][Ii][Tt])|([xXhH]26[45])|(\\d+([\\*Xx])\\d+)|([0-9]{2,5}([pP]))|(\\[[0-9a-fA-F]{8}\\])|(YYDM-11FANS)|([a-zA-Z]{2,5}([Rr][Ii][Pp]))|([0-9a-zA_Z\\s]{5,200})";
        private static String regex = "(10[Bb][Ii][Tt])|([xXhH]26[45])|(\\d+([\\*Xx])\\d+)|([0-9]{2,5}([pP]))|(\\[[0-9a-fA-F]{8}\\])|(YYDM-11FANS)|([a-zA-Z]{2,5}([Rr][Ii][Pp]))|([0-9a-zA-Z_]{6,200})";
        private static String regex2 = "(10[Bb][Ii][Tt])|([xXhH]26[45])|(\\d+([\\*Xx])\\d+)|(\\[[0-9a-fA-F]{8}\\])|(YYDM-11FANS)|([a-zA-Z]{2,5}([Rr][Ii][Pp]))";
        private static String regex_headAndTail = "第|話|话|集";
        public static void Rename(Names names, BackgroundWorker bkWorker)
        {
            Rename(names, bkWorker, 0, names.isRegex);
        }
        public static void Rename(Names names, BackgroundWorker bkWorker, int count, bool isRegex)
        {
            //      string aaa = "dwads 10bit 1080p 最终流放 Last_ecxile 1024x786 x264 ep011 14a341cf.ssa";
            //    aaa = System.Text.RegularExpressions.Regex.Replace(aaa, regex, "");
            //  bool b = isFitNum(aaa, "01");
            int c = count;

            if (isRegex)
            {
                Dictionary<FileInfo, string> videoDic = getDic(names.videos, names.getVideoReplasePattern());
                Dictionary<FileInfo, string> subDic = getDic(names.subs, names.getSubReplasePattern());

                foreach (var video in videoDic.Keys)
                {
                    if (bkWorker != null)
                    {
                        bkWorker.ReportProgress(++c, video.Name);
                    }
                    LinkedList<FileInfo> subs = getSubList(subDic, videoDic[video]);
                    renameSubs(video, subs);
                }
            }
            else
            {
                foreach (var video in names.videos)
                {
                    if (bkWorker != null)
                    {
                        bkWorker.ReportProgress(++c, video.Name);
                    }
                    string num = getVideoNumber(video);
                    if (num == null)
                    {
                        continue;
                    }
                    LinkedList<FileInfo> subs = getSubList(names, num);
                    renameSubs(video, subs);

                }
                foreach (var name in names.names)
                {
                    Rename(name, bkWorker, c, name.isRegex);
                }
            }
        }

        private static void renameSubs(FileInfo video, LinkedList<FileInfo> subs)
        {
            string vname = getFullNameWithOutExtension(video);
            foreach (var sub in subs)
            {
                string ext = getFullExtension(sub);
                try
                {
                    sub.MoveTo(vname + ext);
                }
                catch
                {
                    sub.MoveTo(vname + "." + sub.Name);
                }
            }
        }


        private static Dictionary<FileInfo, string> getDic(LinkedList<FileInfo> videos, string p)
        {
            Dictionary<FileInfo, string> dic = new Dictionary<FileInfo, string>();
            foreach (var video in videos)
            {
                string name = video.Name;
                string str = System.Text.RegularExpressions.Regex.Replace(name, p, "");
                dic.Add(video, str);
            }
            return dic;
        }

        private static string getFullNameWithOutExtension(FileInfo video)
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

        private static string getFullExtension(FileInfo sub)
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

        private static LinkedList<FileInfo> getSubList(Names names, string num)
        {
            LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
            foreach (var sub in names.subs)
            {
                if (isFit(sub, num))
                {
                    subs.AddLast(sub);
                }
            }
            return subs;
        }

        private static LinkedList<FileInfo> getSubList(Dictionary<FileInfo, string> subDic, string key)
        {
            LinkedList<FileInfo> subs = new LinkedList<FileInfo>();
            foreach (var sub in subDic.Keys)
            {
                if (subDic[sub].Equals(key))
                {
                    subs.AddLast(sub);
                   // subDic.Remove(sub);
                }
            }
            return subs;
        }

        private static bool isFit(FileInfo sub, string num)
        {
            string subNum = getVideoNumber(sub);
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
                if (isFitNum(name, num))
                {
                    return true;
                }
            }




            return false;
        }

        private static bool isFitNum(string name, string num)
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

        private static string getVideoNumber(System.IO.FileInfo video)
        {
            String name = (String)video.Name.Clone();
            name = name.Replace(video.Extension, "");
            LinkedList<string> strs = split(name);
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

                float f;
                if (!float.TryParse(str2, out f))
                {
                    continue;
                }

                if (f < 0 || f > 1900)
                {
                    continue;
                }

                return str2;


                //if (str2.Length <= 1 || str2.Length >= 4)
                //{
                //    continue;
                //}
                //else if (isNumberic(str2))
                //{
                //    return str2;
                //}
            }
            return null;
        }




        private static bool isNumberic(string str)
        {
            char[] cs = str.ToArray();
            foreach (var c in cs)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
            //try
            //{
            //    Convert.ToInt32(str);
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}

        }

        private static LinkedList<string> split(string name)
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
                        int end = findMatchingPos(ca, i, ' ');
                        result.AddLast(name2.Substring(i + 1, end - i - 1));
                    }
                    catch
                    {
                        result.AddLast(name2.Substring(i));
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

        private static int findMatchingPos(char[] ca, int begin, char left)
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
