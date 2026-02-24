using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiProject1.Application.UntinesHelper
{
    /// <summary>
    /// 昵称匹配度计算服务
    /// </summary>
    public class NicknameMatchService
    {
        private readonly Random _random;

        public NicknameMatchService()
        {
            // 初始化随机数生成器（基于时间种子保证每次结果略有不同）
            _random = new Random(Guid.NewGuid().GetHashCode());
        }
       // private readonly Random _random = new Random();

        /// <summary>
        /// 计算两个昵称的匹配度（80-100），整合版单方法实现
        /// </summary>
        /// <param name="nickname1">第一个昵称</param>
        /// <param name="nickname2">第二个昵称</param>
        /// <returns>匹配度分数（80-100）</returns>
        /// <exception cref="ArgumentException">昵称为空时抛出</exception>
        public int CalculateMatchScore(string nickname1, string nickname2)
        {
            // ========== 核心常量定义（可直接修改此处调整规则） ==========
            const int MinScore = 80;          // 最低匹配分
            const int MaxScore = 100;         // 最高匹配分
            const double LengthWeight = 0.1;  // 长度相似度权重（10%）
            const double ContinuousSubstringWeight = 0.5; // 连续子串权重（50%，核心）
            const double CharCoverageWeight = 0.2; // 字符覆盖率权重（20%）
            const double PinyinWeight = 0.2;  // 拼音/谐音权重（20%）

            // ========== 1. 参数校验 ==========
            if (string.IsNullOrWhiteSpace(nickname1))
                throw new ArgumentException("第一个昵称不能为空或仅包含空白字符", nameof(nickname1));
            if (string.IsNullOrWhiteSpace(nickname2))
                throw new ArgumentException("第二个昵称不能为空或仅包含空白字符", nameof(nickname2));

            // ========== 2. 昵称清洗：去空格、转小写、移除特殊符号 ==========
            string CleanNickname(string name)
            {
                string cleaned = Regex.Replace(name.Trim().ToLowerInvariant(), @"[^\u4e00-\u9fa5a-z0-9]", "");
                return string.IsNullOrEmpty(cleaned) ? name.Trim().ToLowerInvariant() : cleaned;
            }
            string cleanName1 = CleanNickname(nickname1);
            string cleanName2 = CleanNickname(nickname2);

            // ========== 3. 特殊场景：完全相同直接返回100分 ==========
            if (cleanName1 == cleanName2)
                return 100;

            // ========== 4. 内部辅助方法：计算各维度相似度 ==========
            // 4.1 长度相似度（0-1）
            double CalculateLengthSimilarity(string n1, string n2)
            {
                int maxLen = Math.Max(n1.Length, n2.Length);
                int minLen = Math.Min(n1.Length, n2.Length);
                return maxLen == 0 ? 0 : (double)minLen / maxLen;
            }

            // 4.2 最长连续子串相似度（0-1）
            double CalculateContinuousSubstringSimilarity(string n1, string n2)
            {
                int maxLength = 0;
                int[,] dp = new int[n1.Length + 1, n2.Length + 1];
                for (int i = 1; i <= n1.Length; i++)
                {
                    for (int j = 1; j <= n2.Length; j++)
                    {
                        if (n1[i - 1] == n2[j - 1])
                        {
                            dp[i, j] = dp[i - 1, j - 1] + 1;
                            maxLength = Math.Max(maxLength, dp[i, j]);
                        }
                    }
                }
                int minLen = Math.Min(n1.Length, n2.Length);
                return minLen == 0 ? 0 : (double)maxLength / minLen;
            }

            // 4.3 字符覆盖率相似度（0-1）
            double CalculateCharCoverageSimilarity(string n1, string n2)
            {
                var set1 = new HashSet<char>(n1);
                var set2 = new HashSet<char>(n2);
                int intersection = set1.Intersect(set2).Count();
                int union = set1.Union(set2).Count();
                return union == 0 ? 0 : (double)intersection / union;
            }

            // 4.4 拼音/谐音相似度（0-1）
            double CalculatePinyinSimilarity(string n1, string n2)
            {
                // 基础拼音映射（可按需扩展）
                var pinyinMap = new Dictionary<char, string>
          {
              {'小',"xiao"},{'晓',"xiao"},{'笑',"xiao"},
              {'丽',"li"},{'莉',"li"},{'立',"li"},
              {'明',"ming"},{'铭',"ming"},{'鸣',"ming"},
              {'华',"hua"},{'花',"hua"},{'化',"hua"}
          };
                // 转换为拼音字符串
                string ConvertToPinyin(string name)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char c in name)
                    {
                        sb.Append(pinyinMap.TryGetValue(c, out var p) ? p : c.ToString());
                    }
                    return sb.ToString();
                }
                string p1 = ConvertToPinyin(n1);
                string p2 = ConvertToPinyin(n2);
                var set1 = new HashSet<char>(p1);
                var set2 = new HashSet<char>(p2);
                int intersection = set1.Intersect(set2).Count();
                int union = set1.Union(set2).Count();
                return union == 0 ? 0 : (double)intersection / union;
            }

            // ========== 5. 计算各维度分数 ==========
            double lenScore = CalculateLengthSimilarity(cleanName1, cleanName2);
            double continuousScore = CalculateContinuousSubstringSimilarity(cleanName1, cleanName2);
            double charScore = CalculateCharCoverageSimilarity(cleanName1, cleanName2);
            double pinyinScore = CalculatePinyinSimilarity(cleanName1, cleanName2);

            // ========== 6. 加权计算基础总分（0-1） ==========
            double baseScore =
                lenScore * LengthWeight +
                continuousScore * ContinuousSubstringWeight +
                charScore * CharCoverageWeight +
                pinyinScore * PinyinWeight;

            // ========== 7. 随机微调（±1分）+ 映射到80-100区间 ==========
            int randomAdjust = _random.Next(-1, 2); // -1、0、1
            double mappedScore = MinScore + baseScore * (MaxScore - MinScore);
            int finalScore = (int)Math.Round(mappedScore) + randomAdjust;

            // ========== 8. 兜底：确保分数在80-100之间 ==========
            finalScore = Math.Clamp(finalScore, MinScore, MaxScore);

            return finalScore;
        }
    }
}
