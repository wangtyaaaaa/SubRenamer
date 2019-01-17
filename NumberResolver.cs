using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubRenamer
{
    class NumberResolver
    {

        public static bool Reslove(Names names)
        {
            //int idx1,idx2 = 0;
            string[] video_num = Names.GetStrArray(names.videos);
            string[] strs = Names.GetStrArray(names.videos);
            int len = strs[0].Length;
            int i = 0;
            for (;i<len;i++)            //检查第一个不一样的字符
            {
                char c = strs[0][i];
                bool fl = false;
                foreach(string s in strs)
                {
                    if (c != s[i])
                    {
                        fl = true;
                        break;
                    }
                }
                if (fl) break;
            }
            if (i >=len)
            {
                return false;
            }
            
            foreach(var video in names.videos)
            {
                string s = video.file.Name;
                int j = i;
                for (; j < s.Length; j++)
                {
                    if (!isNumber(s[j]))
                    {
                        break;
                    }
                }
                if (j == i) continue;
                string s2 = s.Substring(i, j - i);
                video.num = s2;
            }
               
                return true;
        }

        private static bool isNumber(char c)
        {
            if (c < '0' || c > '9') return false;
            else return true;
        }

        //private static string[] getStrArray(LinkedList<FileInfo> list)
        //{
        //    string[] res = new string[list.Count];
        //    LinkedListNode<FileInfo> node = list.First;
            
        //    for(int i = 0; i < list.Count; i++)
        //    {
        //        res[i] = node.Value.Name;
        //        node = node.Next;
        //    }


        //    return res;
        //}
    }
}
