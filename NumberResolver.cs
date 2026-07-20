using System;
using System.Collections.Generic;
using System.Linq;

namespace SubRenamer
{
    internal class NumberResolver
    {

        private class VSFileGroupItem<T> where T : VSFile
        {
            /// <summary>
            /// 文件列表
            /// </summary>
            public List<VSFile> FileList { get; }

            /// <summary>
            /// 这组文件中疑似集号的位置
            /// </summary>
            public List<int> LikelyEpNumPos { get; }


            public VSFileGroupItem(T t)
            {
                FileList = new List<VSFile>();
                AddVSFile(t);
                LikelyEpNumPos = GetLikelyEpNumPos(t.Splited_filename);
            }

            /// <summary>
            /// 添加一个文件
            /// </summary>
            /// <param name="file"></param>
            public void AddVSFile(VSFile file)
            {
                FileList.Add(file);
            }
        }

        public static bool Resolve(Names names)
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
                    string s = video.File.Name;
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
                    video.Num = s2;
                }

                return true;
            }
            catch (System.Exception)
            {

                return false;
            }

        }

        private static bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }




        /// <summary>
        /// 获取疑似集号的位置
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static List<int> GetLikelyEpNumPos(List<string> a)
        {
            var result = new List<int>();
            for (int i = 0; i < a.Count; i++)
            {
                if (Renamer.IsLikelyEpisodeNumber(a[i])) result.Add(i);
            }

            return result;
        }


        /// <summary>
        /// 处理分好组的文件名二维数组，将集号填回
        /// </summary>
        /// <param name="subList"></param>
        /// <param name="tempGroupSubArray"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal static void ResolveGroupFileList<T>(List<T> files, double min_match_rate) where T : VSFile
        {
            List<VSFileGroupItem<T>> group = GroupVSFiles(files, min_match_rate);
            foreach (var item in group)
            {
                ResolveFileList(item.FileList);
            }
        }

        /// <summary>
        /// 将文件列表按文件名格式分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files"></param>
        /// <returns></returns>
        private static List<VSFileGroupItem<T>> GroupVSFiles<T>(List<T> files, double min_match_rate) where T : VSFile
        {
            List<VSFileGroupItem<T>> result = new List<VSFileGroupItem<T>>();

            //将第一个文件创建为第一组的第一个元素
            var item = new VSFileGroupItem<T>(files[0]);
            result.Add(item);

            //从第二给文件开始遍历
            for (int i = 1; i < files.Count; i++)
            {
                var _a = files[i].Splited_filename;

                double match_rate = 0; //匹配度 = 对应位置相对元素数 / 总元素数
                int match_group = -1; //匹配组

                for (int g_num = 0; g_num < result.Count; g_num++)
                {
                    var _group = result[g_num];
                    //取第一个元素
                    var _f = _group.FileList[0].Splited_filename;

                    int __a = _f.Count - _a.Count;
                    if (__a < 3 && __a > -3)        //拆分出的文件名长度大致相等
                    {
                        double match_count = 0; //对应位置相对元素数
                        for (int col = 0; col < _a.Count; col++)
                        {
                            if (col < _f.Count)
                                if (_a[col] == _f[col]) match_count++;
                        }

                        double _mr = match_count / _f.Count;
                        if (_mr >= min_match_rate)
                        {
                            var _pos1 = GetLikelyEpNumPos(_a);
                            var _pos2 = _group.LikelyEpNumPos;
                            if (_pos1.SequenceEqual(_pos2) && _mr > match_rate)
                            {
                                match_rate = _mr;
                                match_group = g_num;
                            }
                        }
                    }
                }

                if (match_group >= 0)
                {
                    result[match_group].AddVSFile(files[i]);
                }
                else
                {
                    var _item = new VSFileGroupItem<T>(files[i]);
                    result.Add(_item);
                }
            }

            return result;
        }

        /// <summary>
        /// 使用打散后的一组文件名，计算离散度最高的一列作为列名，保存回去
        /// </summary>
        /// <param name="files"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool ResolveFileList<T>(List<T> files) where T : VSFile
        {
            // 边界检查：如果列表为空或没有行，返回 -1
            if (files == null || files.Count == 0) return false;

            // 如果只有一个文件，调用单文件提取集号方法
            if(files.Count <= 1) // 可以调整单文件/一组方法的数量边界
            {
                foreach (var item in files)
                {
                    item.Num = Renamer.GetEpisodeNumber(item.File);
                }
                return true;
            }

            // 获取列数（以第一行的长度为准）
            // 注意：这里假设所有行的列数是相同的。如果不相同，需要取最大列数或最小列数，视需求而定。
            int colCount = files[0].Splited_filename.Count;
            if (colCount == 0) return false;

            int maxUniqueCount = -1;
            int maxUniqueColumn = -1;

            // 遍历每一列
            for (int col = 0; col < colCount; col++)
            {
                HashSet<string> uniqueValues = new HashSet<string>();
                // 遍历当前列的每一行
                foreach (var _v in files)
                {
                    // 防止某一行长度不足导致越界
                    if (col < _v.Splited_filename.Count)
                        // 将值加入 HashSet，自动去重
                        uniqueValues.Add(_v.Splited_filename[col]);
                }

                // 比较当前列的唯一值数量
                int currentUniqueCount = uniqueValues.Count;

                // 如果当前列的离散度更高，更新结果
                // 使用 > 而不是 >=，确保在离散度相同时返回最前面的列（最小索引）
                if (currentUniqueCount > maxUniqueCount)
                {
                    maxUniqueCount = currentUniqueCount;
                    maxUniqueColumn = col;
                }
            }

            //将离散度最高那列的值，赋值到对象中
            foreach (var _v in files)
            {
                string str = Renamer.ResolveEpisodeNumber(_v.Splited_filename[maxUniqueColumn]);
                _v.Num = str;
            }

            return true;
        }
    }
}
