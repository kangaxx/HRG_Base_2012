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
            List<double> int1 = char2int_chs(str1);
            List<double> int2 = char2int_chs(str2);

            double soundAdj = 0.3; //发音调整值
            if (int1.Count() == int2.Count())
            {
                for (int i = 0; i < int1.Count(); ++i)
                {
                    if (int1[i] == int2[i])
                        soundAdj -= 0.15;
                }
            }
            else
            {
                if (int1.Count() > 1 && int1[1] == int2[0])
                    soundAdj -= 0.2;
                else if (int2.Count() > 1 && int2[1] == int1[0])
                    soundAdj -= 0.2;

            }
            //向量夹角公式 m^v = mv /|m| * |v|
            //|m| =  sqrt（m1*m1+m2*m2+…+mn*mn）
            // mv = m1v1 + m2v2 + m3v3...mn vn
            double result = getCrossProduct(int1, int2) /(getCrossSqrt(int1) * getCrossSqrt(int2));
            return result - soundAdj >= 0 ? result - soundAdj : 0;

        }

        public static double getMD_UT(double a, double b, double c, double d)
        {
            List<double> d1 = new List<double>();
            List<double> d2 = new List<double>();
            d1.Add(a);
            d1.Add(b);
            d2.Add(c);
            d2.Add(d);
            return getCrossProduct(d1, d2) / (getCrossSqrt(d1) * getCrossSqrt(d2));

        }

        //比较两个词之间的匹配度,算出最大匹配度，limit是单字阈值（这是一个均值，就是用匹配值除以待匹配key的长度，如果小于均值则直接视作 0）
        public static double getMD_String(string input, string key, double limit = 0.4)
        {
            double result,temp;
            double result_max = 0;
            //a. 在input中扫描key
            for (int i = 0; i < input.Count(); ++i)
            {
                result = 0;
                //从第input的第i这个字开始，依次对位比较j次
                for (int j = 0; j < Math.Min(key.Count(), input.Count() - i); ++j)
                {
                    temp = 0;
                    temp = getMD_Word(input.Substring(i + j, 1), key.Substring(j, 1));
                    if (temp < limit)
                        temp = 0;
                    result += temp;

                }
                if (result > result_max)
                    result_max = result; //result_max永远记录整组比较结果中最大的值
            }

            //b. 扫描key的尾部，有可能会input的头部契合
            for (int m = 1;m < key.Count(); ++m)
            {
                result = 0;
                //从第key的第m(m >= 1)这个字开始，依次对位比较n次
                for (int n = 0; n < Math.Min(key.Count() - m, input.Count()); ++n)
                {
                    temp = 0;
                    temp = getMD_Word(input.Substring(n, 1), key.Substring(m+n, 1));
                    temp *= temp;
                    result += temp;
                }
                if (result > result_max)
                    result_max = result; //result_max永远记录整组比较结果中最大的值
            }
            return result_max / Convert.ToDouble(Math.Min(key.Count(), input.Count()));
        }

        //求向量的积， 公式 mv = m1v1 + m2v2 + m3v3...mn vn，注意下面的单词是向量积不是向量的积，忽略这个问题吧。
        public static double getCrossProduct(List<double> i1, List<double> i2)
        {
            double result = 0;
            int num = Math.Min(i1.Count, i2.Count);
            if (num <=  0)
                return result;
            for (int i = 0; i < num; ++i)
            {
                result += i1[i] * i2[i]; 

            }
            return result;
        }

        //求向量的膜
        public static double getCrossSqrt(List<double> i1)
        {
            double result = 0;
            if (i1.Count <= 0)
                return result;
            for (int i = 0; i < i1.Count; ++i)
            {
                result += i1[i] * i1[i];

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
                int asc =  Convert.ToInt32(chs[i]); //将字母转成asc数字
                if (asc >= 97 && asc <= 97 + 25)
                    asc -= 96;
                else if (asc >= 65 && asc <= 65 + 25)
                    asc -= 64;
                else
                    asc = 0;
                result.Add(asc);
            }
            return result;
        }

        public static List<double> char2int_chs(string str)
        {
            List<double> result = new List<double>();
            string tmp = HRG_BaseLibrary_2012.CharacterHelper.Get(str);
            char[] chs = tmp.ToLower().ToCharArray();

            //声母表b p m f d t n l g k h j q x r z c s y w 
            //       zh ch sh
            //塞音 清 不送气 b d g w
            //塞擦音 清 不送气 j z zh 
            //擦音 清 f h x s sh 
            //送气 p t k 

            //送气 y q c ch  

            //浊 r 
            //鼻音 浊 m n 
            //边音 浊 l
            //小写声母
            char[] initialTable = { 'b', 'd', 'g', ' ', 
                                    'w', 'j', 'z', ' ',
                                    'f', 'h', 'x', 's',' ',
                                    'p', 't', 'k',' ',
                                    'y', 'q', 'c',
                                    'r',' ',
                                    'm', 'n', ' ',
                                    'l' };


            //首先判断第二个字母是否是 h, 如是则说明 声母是ch 或者sh等双声母，

            int initialLen = 0; //声母的结尾位置
            if (chs[1] == 'h')
                initialLen = 1;
            else
                initialLen = 0;

            //将声母装成数字
            for (int i = 0; i < initialTable.Count(); ++i)
            {
                if (chs[0] == initialTable[i])
                {
                    result.Add(Convert.ToDouble(i * 3 + initialLen)); //声母位置， z和zh这样的间距是1 ，否则是3
                    break;
                }
            }

            //韵母表
            //单元音 a、o、e、ê、i、u、ü (有两个实际不可能出现）
            //前响复韵母 ɑi、ɑo、ei、ou
            //后响复韵母 ia、ie、ua、uo、üe
            //中响复韵母 iao、iou、uai、uei
            //前鼻音尾韵母 ɑn、en、in、un、iɑn、uɑn、uen、 üɑn（这个其实不会出现）
            //后鼻音韵母 ang、eng、ing、ong、iang、uang、ueng、iong
            string[] finalTable = { "a","o","e","i","u"," ",
                                    "ai","ao","ei","ou", " ",
                                    "ia","ie","ua","uo", " ",
                                    "an","en","in","un","ui","ian","uen"," ",
                                    "ang","eng","ing","ong","iang","uang","ueng","iong"," "
                                  };
            //从声母结束的位置开始，对韵母进行判断
            for (int i = 0; i < finalTable.Count(); ++i)
            {
                if (tmp.Substring(initialLen+1).Trim() == finalTable[i])
                {
                    result.Add(Convert.ToDouble(i * 3)); //声母位置， z和zh这样的间距是1 ，否则是3
                    break;
                }

            }

            return result;
        }

    }
}
