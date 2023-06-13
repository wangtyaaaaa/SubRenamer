namespace SubRenamer
{
    internal class NumberResolver
    {

        public static bool Reslove(Names names)
        {
            //int idx1,idx2 = 0;
            _ = Names.GetStrArray(names.videos);
            string[] strs = Names.GetStrArray(names.videos);
            int len = strs[0].Length;
            int i = 0;
            for (; i < len; i++)            //检查第一个不一样的字符
            {
                char c = strs[0][i];
                bool fl = false;
                foreach (string s in strs)
                {
                    if (c != s[i])
                    {
                        fl = true;
                        break;
                    }
                }
                if (fl)
                {
                    break;
                }
            }
            if (i >= len)
            {
                return false;
            }

            foreach (Video video in names.videos)
            {
                string s = video.file.Name;
                int j = i;
                for (; j < s.Length; j++)
                {
                    if (!IsNumber(s[j]))
                    {
                        break;
                    }
                }
                if (j == i)
                {
                    continue;
                }

                string s2 = s.Substring(i, j - i);
                video.num = s2;
            }

            return true;
        }

        private static bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
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
