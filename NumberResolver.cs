using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubRenamer
{
    internal class NumberResolver
    {

        public static bool Reslove(Names names)
        {
            //int idx1,idx2 = 0;
            // _ = Names.GetStrArray(names.videos);
            try
            {
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
            catch (System.Exception e)
            {

                return false;
            }

        }

        private static bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// 把文件名打散转换为一个二维数组，用于下一步处理
        /// /// </summary>
        /// <typeparam name="T">文件对象</typeparam>
        /// <param name="files">一组视频或字母文件，要求格式相同</param>
        /// <returns></returns>
        public static List<List<string>> ExtractFileName2DArray<T>(List<T> files) where T : File
        {
            var fileName2DArray = new List<List<string>>();

            foreach (var item in files)
            {
                string name = item.file.Name.Replace(item.file.Extension,"");
                var strs = Renamer.Split(name);
                var list = new List<string>(strs.Count);
                foreach (var str in strs)
                {
                    list.Add(Renamer.ResloveTitleNumber(str));
                }

                fileName2DArray.Add(list);
            }


            return fileName2DArray;
        }


        /// <summary>
        /// 返回数组离散度最高的列序号
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>-1:错误; 0~arral.size:序号</returns>
        public static int FindHighestMODColumn(List<List<string>> array)
        {
            // 1. 边界检查：如果列表为空或没有行，返回 -1
            if (array == null || array.Count == 0)
                return -1;

            // 2. 获取列数（以第一行的长度为准）
            // 注意：这里假设所有行的列数是相同的。如果不相同，需要取最大列数或最小列数，视需求而定。
            int colCount = array[0].Count;
            if (colCount == 0)
                return -1;

            int maxUniqueCount = -1;
            int res = -1;

            // 3. 遍历每一列
            for (int col = 0; col < colCount; col++)
            {
                HashSet<string> uniqueValues = new HashSet<string>();

                // 4. 遍历当前列的每一行
                foreach (var row in array)
                {
                    // 防止某一行长度不足导致越界
                    if (col < row.Count)
                    {
                        // 将值加入 HashSet，自动去重
                        // 如果希望区分 null 和 ""，可以直接添加；如果希望视为相同，可以预处理
                        uniqueValues.Add(row[col]);
                    }
                }

                // 5. 比较当前列的唯一值数量
                int currentUniqueCount = uniqueValues.Count;

                // 如果当前列的离散度更高，更新结果
                // 使用 > 而不是 >=，确保在离散度相同时返回最前面的列（最小索引）
                if (currentUniqueCount > maxUniqueCount)
                {
                    maxUniqueCount = currentUniqueCount;
                    res = col;
                }
            }

            return res;
        }

    }
}
