using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRG_BaseLibrary_2012
{
    //TF-IDF 关键字匹配算法
    //TF: Term frequency即关键词词频，是指一篇文章中关键词出现的频率，比如在一篇M个词的文章中有N个该关键词，TF= N/M（公式1.1-1）
    //为该关键词在这篇文章中的词频。

    //IDF: Inverse document frequency指逆向文本频率，是用于衡量关键词权重的指数，由公式 IDF = log(D/Dw)计算而得，其中D为文章总数，Dw为关键词出现过的文章数。
    public class HRG_TF_IDF
    {
        //输入已经字母化序列化的待匹配文字例如，比较两个字之间的匹配度， 车 会被以数组"Che"的形式转入，函数会把字母转成数字，刚才这个应该是 67，104，101
        public static double getMD_Word(string str1, string str2)
        {
            //将两个单词转换成向量数组
            List<int> int1 = char2int(str1);
            List<int> int2 = char2int(str2);

            //向量夹角公式 m^v = mv /|m| * |v|
            //|m| =  sqrt（m1*m1+m2*m2+…+mn*mn）
            // mv = m1v1 + m2v2 + m3v3...mn vn
            return getCrossProduct(int1, int2) /(getCrossSqrt(int1) * getCrossSqrt(int2));
        }

        //比较两个词之间的匹配度,看看是否有匹配为1的结果，如果没有，则看看最大匹配值是多少

        public static double getMD_String(string input, string key)
        {
            double result = 0;
            //a. 在input中扫描key
            for (int i = 0; i < input.Count(); ++i)
            {
                for (int j = 0; j < Math.Min(key.Count(), input.Count() - i); ++j)
                {
                    double temp = getMD_Word(input.Substring(i, 1), key.Substring(j, 1));
                }
                //to be contiuned
            }

            return 1.0f;
        }

        //求向量的积， 公式 mv = m1v1 + m2v2 + m3v3...mn vn，注意下面的单词是向量积不是向量的积，忽略这个问题吧。
        public static double getCrossProduct(List<int> i1, List<int> i2)
        {
            double result = 0;
            int num = Math.Min(i1.Count, i2.Count);
            if (num <=  0)
                return result;
            for (int i = 0; i < num; ++i)
            {
                result += Convert.ToDouble(i1[i] * i2[i]);

            }
            return result;
        }

        //求向量的膜
        public static double getCrossSqrt(List<int> i1)
        {
            double result = 0;
            if (i1.Count <= 0)
                return result;
            for (int i = 0; i < i1.Count; ++i)
            {
                result += Convert.ToDouble(i1[i] * i1[i]);

            }
            result = Math.Sqrt(result);
            return result;
        }


        public static List<int> char2int(string str)
        {
            List<int> result = new List<int>();
            string tmp = HRG_BaseLibrary_2012.CharacterHelper.Get(str);
            char[] chs = tmp.ToCharArray();
            for (int i = 0; i < tmp.Length; ++i)
            {
                result.Add(Convert.ToInt32(chs[i]));
            }
            return result;
        }

    }
}
