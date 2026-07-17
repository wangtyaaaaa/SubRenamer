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


        #region 新增：提取视频2-3位数字为二维数组
        /// <summary>
        /// 从视频列表中提取2-3位连续数字，拆分为单个数字的二维数组
        /// </summary>
        /// <param name="videos">视频列表（对应 names.videos）</param>
        /// <returns>二维数组（每行=单个视频的2-3位数字拆分；无符合条件数字则返回空）</returns>
        public static List<List<string>> ExtractVideoNumber2DArray(List<Video> videos)
        {
            var number2DArray = new List<List<string>>();
            if (videos == null || videos.Count == 0)
                return number2DArray;

            // 遍历每个视频，严格按顺序添加行（无匹配则加空列表）
            foreach (var video in videos)
            {
                // 初始化当前视频的数字列表（默认空）
                var episodeStrList = new List<string>();

                if (video?.file != null)
                {
                    // 复用GetVideoNumber的匹配算法，获取所有匹配的数字字符串
                    var numberStrings = Renamer.GetVideoNumberArray(video.file);
                    if (numberStrings.Length > 0)
                    {
                        episodeStrList.AddRange(numberStrings);
                    }
                }

                // 无论是否有数字，都添加当前行（空列表也加），保证行数和视频数一致
                number2DArray.Add(episodeStrList);
            }

            return number2DArray;
        }
        #endregion

        #region 新增：筛选最左侧「每行数字唯一」的列
        /// <summary>
        /// 遍历二维数组的列（从左到右），返回第一个「所有行数字不重复」的列索引
        /// </summary>
        /// <param name="number2DArray">视频数字二维数组</param>
        /// <returns>符合条件的列索引（-1=无）</returns>
        public static int FindLeftUniqueColumn(List<List<string>> number2DArray)
        {
            if (number2DArray == null || number2DArray.Count == 0) return -1;

            int maxCol = number2DArray.Max(row => row.Count);
            for (int colIdx = 0; colIdx < maxCol; colIdx++)
            {
                var columnNumberValues = new HashSet<int>(); // 存数字值，用于判断唯一性
                bool isColumnUnique = true;

                foreach (var row in number2DArray)
                {
                    // 空行/短行→该列不满足唯一
                    if (row.Count == 0 || colIdx >= row.Count)
                    {
                        isColumnUnique = false;
                        break;
                    }

                    string episodeStr = row[colIdx];
                    // 尝试转数字（仅用于判断唯一性，不修改原始字符串）
                    if (!int.TryParse(episodeStr, out int episodeNum))
                    {
                        isColumnUnique = false;
                        break;
                    }

                    // 数字值重复→该列不满足唯一
                    if (columnNumberValues.Contains(episodeNum))
                    {
                        isColumnUnique = false;
                        break;
                    }
                    columnNumberValues.Add(episodeNum);
                }

                if (isColumnUnique) return colIdx;
            }
            return -1;
        }
        #endregion

        #region 新增：集号标准化（转纯数字，适配长度不一致）
        /// <summary>
        /// 标准化集号：提取所有数字并转为整数（如 "EP05"→5、"003"→3、"5"→5）
        /// </summary>
        /// <param name="episodeStr">原始集号字符串（视频/字幕）</param>
        /// <returns>标准化后的数字（null=无有效数字）</returns>
        public static int? NormalizeEpisodeNumber(string episodeStr)
        {
            if (string.IsNullOrEmpty(episodeStr)) return null;

            // 提取所有数字字符
            var pureDigit = new string(episodeStr.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(pureDigit)) return null;

            // 转为整数（自动忽略前导零，适配长度）
            return int.Parse(pureDigit);
        }
        #endregion

        #region 新增：视频集号与字幕集号匹配
        /// <summary>
        /// 基于筛选出的列，匹配视频集号和字幕集号（标准化后匹配）
        /// </summary>
        /// <param name="videos">视频列表</param>
        /// <param name="subFiles">字幕文件列表</param>
        /// <param name="targetColIdx">视频集号所在列索引</param>
        /// <returns>匹配结果（视频文件 → 匹配的字幕文件列表）</returns>
        public static Dictionary<FileInfo, List<FileInfo>> MatchVideoAndSubtitle(
            List<Video> videos,
            List<FileInfo> subFiles,
            int targetColIdx)
        {
            var matchResult = new Dictionary<FileInfo, List<FileInfo>>();
            if (targetColIdx < 0 || videos == null || subFiles == null) return matchResult;

            // 步骤1：提取视频集号（标准化后）
            var videoEpisodeDict = new Dictionary<FileInfo, int?>();
            var regex23Digit = new Regex(@"\d{2,3}");
            foreach (var video in videos)
            {
                if (video?.file == null) continue;

                // 提取视频文件名的2-3位数字
                var match = regex23Digit.Match(video.file.Name);
                if (!match.Success)
                {
                    videoEpisodeDict[video.file] = null;
                    continue;
                }

                // 拆分数字并从目标列提取值
                var digitList = match.Value.Select(c => int.Parse(c.ToString())).ToList();
                if (targetColIdx >= digitList.Count)
                {
                    videoEpisodeDict[video.file] = null;
                    continue;
                }

                // 标准化视频集号（单个数字→整数，如列值为0→0、5→5）
                int videoEpisode = digitList[targetColIdx];
                videoEpisodeDict[video.file] = videoEpisode;
            }

            // 步骤2：提取字幕集号（标准化后）并匹配
            foreach (var videoFile in videoEpisodeDict.Keys)
            {
                var videoEp = videoEpisodeDict[videoFile];
                if (videoEp == null) continue;

                // 筛选出标准化后与视频集号匹配的字幕
                var matchedSubs = subFiles.Where(sub =>
                {
                    var subEp = NormalizeEpisodeNumber(sub.Name);
                    return subEp.HasValue && subEp.Value == videoEp.Value;
                }).ToList();

                matchResult[videoFile] = matchedSubs;
            }

            return matchResult;
        }
        #endregion
    }
}
