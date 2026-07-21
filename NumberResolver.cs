using System;
using System.Collections.Generic;
using System.Linq;

namespace SubRenamer
{
    internal class NumberResolver
    {

        private class VSFileGroup<T> where T : VSFile
        {
            /// <summary>
            /// 文件列表
            /// </summary>
            public List<T> FileList { get; }

            /// <summary>
            /// 这组文件中疑似集号的位置
            /// </summary>
            public List<int> LikelyEpNumPos { get; }


            public VSFileGroup(T t)
            {
                FileList = new List<T>();
                AddVSFile(t);
                LikelyEpNumPos = GetLikelyEpNumPos(t.Splited_filename);
            }

            /// <summary>
            /// 添加一个文件
            /// </summary>
            /// <param name="file"></param>
            public void AddVSFile(T file)
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
            List<VSFileGroup<T>> group = GroupVSFiles(files, min_match_rate);
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
        private static List<VSFileGroup<T>> GroupVSFiles<T>(List<T> files, double min_match_rate) where T : VSFile
        {
            List<VSFileGroup<T>> result = new List<VSFileGroup<T>>();

            if (files.Count <= 0)
            {
                return result;
            }
            //将第一个文件创建为第一组的第一个元素
            var firstGroup = new VSFileGroup<T>(files[0]);
            result.Add(firstGroup);


            if (files.Count <= 1)
            {
                return result;
            }

            //从第二个文件开始遍历
            for (int i = 1; i < files.Count; i++)
            {
                var curr_file = files[i];
                var curr_splited_name = curr_file.Splited_filename;
                double curr_splited_name_length = 0;
                foreach (var item in curr_splited_name)
                {
                    curr_splited_name_length += item.Length;
                }

                double match_group_rate = 0; //匹配度 = 对应位置相对元素数 / 总元素数
                int match_group_num = -1; //匹配组

                for (int g_num = 0; g_num < result.Count; g_num++)
                {
                    var _group = result[g_num];
                    //取第一个元素
                    var group_head_file = _group.FileList[0];
                    var group_head_splited_name = group_head_file.Splited_filename;

                    int __a = group_head_splited_name.Count - curr_splited_name.Count;
                    if (__a < 2 && __a > -2)        //拆分出的文件名长度大致相等
                    {
                        double total_match_rate = 0; //对应位置相对元素数
                        for (int col = 0; col < curr_splited_name.Count; col++)
                        {
                            if (col < group_head_splited_name.Count)
                            {
                                double _rate;
                                if (curr_splited_name[col] == group_head_splited_name[col]) _rate = 1;
                                else _rate = CalculateWeightedSimilarity(curr_splited_name[col], group_head_splited_name[col]);
                                total_match_rate += _rate * curr_splited_name[col].Length / curr_splited_name_length;
                            }
                        }

                        if (total_match_rate >= min_match_rate)
                        {
                            var _pos1 = GetLikelyEpNumPos(curr_splited_name);
                            var _pos2 = _group.LikelyEpNumPos;
                            if (_pos1.SequenceEqual(_pos2) && total_match_rate > match_group_rate)
                            {
                                match_group_rate = total_match_rate;
                                match_group_num = g_num;
                            }
                        }
                    }
                }

                if (match_group_num >= 0)
                {
                    result[match_group_num].AddVSFile(files[i]);
                }
                else
                {
                    var _item = new VSFileGroup<T>(files[i]);
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
            if (files.Count <= 1) // 可以调整单文件/一组方法的数量边界
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



        /// <summary>
        /// 计算两个字符串的加权相似度。
        /// 规则：优先计算公共前缀（权重1.0）。如果没有公共前缀，则权重依次递补。
        /// </summary>
        /// <param name="s1">第一个字符串</param>
        /// <param name="s2">第二个字符串</param>
        /// <returns>加权相似度分数 (0.0 ~ 1.0)</returns>
        private static double CalculateWeightedSimilarity(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0.0;

            // 1. 计算公共前缀
            int prefixLen = 0;
            for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
            {
                if (s1[i] == s2[i])
                    prefixLen++;
                else
                    break;
            }



            // 2. 计算第一长和第二长的公共子串 (排除公共前缀)
            int lcs1Len;

            int lcs2Len;
            // 如果有公共前缀，我们只在前缀之后的部分寻找子串
            if (prefixLen > 0)
            {
                string suffix1 = s1.Substring(prefixLen);
                string suffix2 = s2.Substring(prefixLen);
                (lcs1Len, lcs2Len) = FindTopTwoLCS_Optimized(suffix1, suffix2);
            }
            else // 如果没有公共前缀，就在整个字符串中寻找
            {
                (lcs1Len, lcs2Len) = FindTopTwoLCS_Optimized(s1, s2);
            }

            // 3. 根据规则应用权重
            double totalWeight = 0;
            double maxPossibleWeight = Math.Max(s1.Length, s2.Length);

            if (prefixLen > 0)
            {
                // 规则: 有前缀时，权重为 1.0, 0.7, 0.4
                totalWeight += 1.0 * prefixLen;
                totalWeight += 0.7 * lcs1Len;
                totalWeight += 0.4 * lcs2Len;
            }
            else
            {
                // 规则: 无前缀时，第一长子串权重变为1.0，依次递补
                totalWeight += 1.0 * lcs1Len;
                totalWeight += 0.7 * lcs2Len;
            }

            // 4. 计算并返回归一化的相似度分数
            return totalWeight / maxPossibleWeight;
        }

        /// <summary>
        /// 找出两个字符串中第一长和第二长的公共子串的长度（最小长度>=3），
        /// 且保证两者在原字符串s1和s2中均无交集（完全独立）。
        /// </summary>
        private static (int, int) FindTopTwoLCS_Optimized(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return (0, 0);

            int m = s1.Length;
            int n = s2.Length;
            const int MIN_LEN = 3; // 【补回】最小长度阈值

            if (m < MIN_LEN || n < MIN_LEN)
                return (0, 0);

            // 【修改】收集所有“极大公共子串”，现在需要同时记录在 s1 和 s2 中的起始位置
            var maximalSubstrings = new List<(int start1, int start2, int len)>();

            // 【修改】记录当前的 Top 2 完整信息（包含双轨位置和长度），用于剪枝和重叠判断
            (int start1, int start2, int len) top1 = (-1, -1, 0);
            (int start1, int start2, int len) top2 = (-1, -1, 0);

            // 局部函数：沿着一条对角线斜向对比
            void ScanDiagonal(int startI, int startJ, int diagLength)
            {
                // 如果对角线总长度连最小阈值都达不到，或者连当前的“第二名”都超不过，直接跳过！
                if (diagLength < MIN_LEN) return;
                if (top2.len > 0 && diagLength <= top2.len) return;

                int i = startI, j = startJ;
                int currentLen = 0;
                // 同时记录在 s1 和 s2 中的起始位置
                int currentStartI = -1, currentStartJ = -1;

                while (i < m && j < n)
                {
                    if (s1[i] == s2[j])
                    {
                        if (currentLen == 0)
                        {
                            currentStartI = i;
                            currentStartJ = j;
                        }
                        currentLen++;
                    }
                    else if (currentLen > 0)
                    {
                        // 只有达到最小长度的子串才进行收割和记录
                        if (currentLen >= MIN_LEN)
                        {
                            TryUpdateTopTwo(currentStartI, currentStartJ, currentLen);
                            maximalSubstrings.Add((currentStartI, currentStartJ, currentLen));
                        }
                        currentLen = 0;

                        // 动态剪枝：如果当前断开了，剩下的长度已经不足以超越 top2 或 MIN_LEN，提前结束本条对角线
                        int remainingLength = Math.Min(m - i, n - j);
                        if (remainingLength < MIN_LEN || (top2.len > 0 && remainingLength <= top2.len)) break;
                    }
                    i++; j++;
                }

                // 收割对角线末尾的子串
                if (currentLen >= MIN_LEN) // 【补回】最小长度检查
                {
                    TryUpdateTopTwo(currentStartI, currentStartJ, currentLen);
                    maximalSubstrings.Add((currentStartI, currentStartJ, currentLen));
                }
            }

            // 【核心重构】状态机式更新 Top 2，严格保证双轨独立
            void TryUpdateTopTwo(int start1, int start2, int len)
            {
                var candidate = (start1, start2, len);

                if (len > top1.len)
                {
                    // ================= 场景 A：新来的比老大长，直接当新老大 =================
                    var old1 = top1;
                    var old2 = top2;

                    // 1. 新来的直接当老大
                    top1 = candidate;

                    // 2. 计算旧老大：如果和新老大完全独立（s1和s2都不重叠），就更新为老二
                    if (old1.len > 0 && !IsOverlappingGlobal(top1, old1))
                    {
                        top2 = old1;
                    }
                    else
                    {
                        // 旧老大和新老大冲突了，让旧老二顶上
                        top2 = old2;
                    }

                    // 3. 计算老二：如果老二和新老大冲突，就删掉老二
                    if (top2.len > 0 && IsOverlappingGlobal(top1, top2))
                    {
                        top2 = (-1, -1, 0);
                    }
                }
                else if (len > top2.len)
                {
                    // ================= 场景 B：新来的比老大小，比老二长 =================
                    // 必须和老大完全独立，才能当老二
                    if (!IsOverlappingGlobal(top1, candidate))
                    {
                        top2 = candidate;
                    }
                }
            }

            // 【优化遍历顺序】：优先扫描最长的对角线
            // 主对角线最长，先扫它
            ScanDiagonal(0, 0, Math.Min(m, n));

            // 按距离主对角线的距离（offset），由近及远扫描
            // 越近的对角线越长，越容易触发 top2 的剪枝条件
            int maxOffset = Math.Max(m, n) - 1;
            for (int offset = 1; offset <= maxOffset; offset++)
            {
                // 如果当前偏移量下的最长对角线，已经小于 MIN_LEN 或 小于等于 top2，后续更短的对角线也不用扫了，直接全局终止！
                int currentMaxDiagLength = Math.Min(m, n) - offset;
                if (currentMaxDiagLength < MIN_LEN) break; // 【补回】
                if (top2.len > 0 && currentMaxDiagLength <= top2.len) break;

                // 扫描下方的对角线 (起点在 s1 的左边界)
                if (offset < m)
                    ScanDiagonal(offset, 0, Math.Min(m - offset, n));

                // 扫描上方的对角线 (起点在 s2 的上边界)
                if (offset < n)
                    ScanDiagonal(0, offset, Math.Min(m, n - offset));
            }

            // 后置处理：找出真正互斥的 Top 2（兜底保障 100% 正确）
            if (maximalSubstrings.Count == 0) return (0, 0);

            maximalSubstrings.Sort((a, b) => b.len.CompareTo(a.len));

            int finalLen1 = maximalSubstrings[0].len;
            var finalTop1 = maximalSubstrings[0];
            int finalLen2 = 0;

            for (int k = 1; k < maximalSubstrings.Count; k++)
            {
                var sub = maximalSubstrings[k];
                // 【修改】必须和老大在 s1 和 s2 中都不重叠
                if (!IsOverlappingGlobal(finalTop1, sub))
                {
                    finalLen2 = sub.len;
                    break;
                }
            }

            return (finalLen1, finalLen2);
        }

        /// <summary>
        /// 【新增】判断两个子串是否在任意一个字符串（s1 或 s2）中重叠。
        /// 只要有一边重叠，就视为不独立。
        /// </summary>
        private static bool IsOverlappingGlobal((int s1, int s2, int len) a, (int s1, int s2, int len) b)
        {
            // 判断在 s1 中是否重叠
            bool overlapInS1 = !(a.s1 + a.len <= b.s1 || b.s1 + b.len <= a.s1);
            // 判断在 s2 中是否重叠
            bool overlapInS2 = !(a.s2 + a.len <= b.s2 || b.s2 + b.len <= a.s2);

            return overlapInS1 || overlapInS2;
        }
    }
}
