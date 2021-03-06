using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
namespace XJU
{
    namespace SpellCheck
    {
        public class Spelling : IDisposable
        {

            #region 定义变量
            private System.Collections.Generic.Dictionary<String, String> myDic;
            private System.Collections.Generic.List<string> myDicList;
            private System.Collections.Generic.Dictionary<string,string> myRepDic;
            private System.Collections.Generic.List<int> DicTemp;
            private System.Collections.Generic.Dictionary<string, float> resulList;

            #region 为多线程部分用的变量, 以没有用
            //------------<
            /*
            private System.Collections.Generic.LinkedList<string> lnk1;
            private System.Collections.Generic.LinkedList<string> lnk2;
            private System.Collections.Generic.LinkedList<string> lnk3;
            private System.Collections.Generic.LinkedListNode<string> nod1;
            private System.Collections.Generic.LinkedListNode<string> nod2;
            private System.Collections.Generic.LinkedListNode<string> nod3;
            private System.Collections.Hashtable resultListHash;
            private Thread t1, t2, t3;
            */
            //---------------->
            #endregion

            private  char[] ouou = { '\u0648', '\u06c7', '\u06c6', '\u06c8' };
            private  char[] ououei = { '\u06d0', '\u0649', '\u0648', '\u06c7', '\u06c6', '\u06c8' };
            private  char[] ououeiae = { '\u0627', '\u06d5', '\u06d0', '\u0649', '\u0648', '\u06c7', '\u06c6', '\u06c8' };
            private  char[] ei = { '\u06d0', '\u0649' };
            private  char[] ae = { '\u0627', '\u06d5' };

            private System.IO.FileStream fs;
            private System.IO.StreamReader sr;
            private string errMessage = string.Empty;
            private int ignoredLength = 2; // 两个词的字母长度区别大于这个值,则不推荐它
            public int IgnoredLength
            {
                get
                {
                    return this.ignoredLength;
                }
                set
                {
                    this.ignoredLength = value;
                }
            }
            public string ErrMessage
            {
                get 
                {
                    return this.errMessage;
                }
            }


            #endregion
     

            //--------------------------------------------------------------------------------------------------------
            /// <summary>
            /// 载入词典词库 (载入的默认词库文件为同目目录下的DicKey.dic)
            /// </summary>
            /// <returns></returns>
            public bool LoadDic(string subPath)
            {

                //if (!subPath.Equals("XJU.SpellCheck.Spelling"))
                //    return false;
                try
                {

                    this.fs = new System.IO.FileStream("Dic.dic", System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    this.DicTemp = new List<int>();
                    this.myDic = new Dictionary<string, string>();
                    this.resulList = new Dictionary<string, float>();
                    this.myDicList = new List<string>();
                    string inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Trim();
                        if (!myDic.ContainsKey(inStr))
                        {
                            this.myDic.Add(inStr, String.Empty);
                            this.myDicList.Add(inStr);
                        }
                    }
                    
                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                }
                catch(Exception openErr)
                {
                    this.myDic = null;
                    this.errMessage = openErr.Message;
                    return false;
                }
                try
                {

                    this.fs = new System.IO.FileStream("user.dic", System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    string inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Trim();
                        if (!myDic.ContainsKey(inStr))
                        {
                            myDic.Add(inStr, String.Empty);
                            this.myDicList.Add(inStr);
                        }
                    }

                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                    return true;
                }
                catch (Exception openErr)
                {
                    this.errMessage = openErr.Message;
                    return true;

                }

            }
            public bool LoadRepDic(string subPath)
            {
                //if (!subPath.Equals("XJU.SpellCheck.Spelling"))
                //    return false;
                try
                {

                    this.fs = new System.IO.FileStream("Rep.dic", System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    this.myRepDic = new Dictionary<string, string>();
                    ArrayList al = new ArrayList(); 
                    string[] inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Split(new char[]{'\t'});
                        if (!myRepDic.ContainsKey(inStr[0]))
                        {

                            try
                            {
                                this.myRepDic.Add(inStr[0], inStr[1]);
                            }
                            catch (Exception err)
                            {
                                //past 
                            }
                        }
                    }

                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                    //return true;
                }
                catch (Exception openErr)
                {
                    this.errMessage = openErr.Message;
                    this.myRepDic = null;
                    return false;
                }

                try
                {

                    this.fs = new System.IO.FileStream("userRep.dic", System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    string[] inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Split(new char[] { '\t' });
                        if (!myRepDic.ContainsKey(inStr[0]))
                        {

                            try
                            {
                                this.myRepDic.Add(inStr[0], inStr[1]);
                            }
                            catch (Exception err)
                            {
                                //past 
                            }
                        }
                    }

                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                    return true;
                }
                catch (Exception openErr)
                {
                    this.errMessage = openErr.Message;
                    return true;

                }



            }

            /// <summary>
            /// 载入词典词库 (载入的默认词库文件为同目目录下的DicKey.dic)
            /// </summary>
            /// <param name="fileFullPath"></param>
            /// <returns></returns>
            public bool LoadDic(string dicFullPath,string subPath)
            {

                //if (!subPath.Equals("XJU.SpellCheck.Spelling"))
                //    return false;
                try
                {

                    this.fs = new System.IO.FileStream(dicFullPath, System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    this.myDic = new Dictionary<string, string>();
                    this.resulList = new Dictionary<string, float>();
                    this.DicTemp = new List<int>();
                    this.myDicList = new List<string>();
                    string inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Trim();
                        if (!myDic.ContainsKey(inStr))
                        {
                            this.myDic.Add(inStr, String.Empty);
                            this.myDicList.Add(inStr);
                        }
                    }

                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                }
                catch (Exception openErr)
                {
                    this.myDic = null;
                    this.errMessage = openErr.Message;
                    return false;
                }
                try
                {

                    this.fs = new System.IO.FileStream("user.dic", System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    string inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Trim();
                        if (!myDic.ContainsKey(inStr))
                        {
                            myDic.Add(inStr, String.Empty);
                            this.myDicList.Add(inStr);
                        }
                    }

                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                    return true;
                }
                catch (Exception openErr)
                {
                    this.errMessage = openErr.Message;
                    return true;

                }
            }

            public bool LoadRepDic(string repdicFullPath, string subPath)
            {
                //if (!subPath.Equals("XJU.SpellCheck.Spelling"))
                //    return false;
                try
                {

                    this.fs = new System.IO.FileStream(repdicFullPath, System.IO.FileMode.Open);
                    this.sr = new System.IO.StreamReader(fs, System.Text.Encoding.Default);
                    this.myRepDic = new Dictionary<string, string>();
                    ArrayList al = new ArrayList();
                    string[] inStr;
                    //开始读
                    while (sr.Peek() >= 0)
                    {
                        inStr = sr.ReadLine().Split(new char[] { '\t' });
                        if (!myRepDic.ContainsKey(inStr[0]))
                        {
                            //al.Clear();
                            //al.Add(inStr[1]);
                            try
                            {
                                this.myRepDic.Add(inStr[0], inStr[1]);
                            }
                            catch (Exception err)
                            {
                                //past
                            }
                        }

                    }

                    //清理工作
                    this.sr.Close();
                    this.fs.Close();
                    return true;
                }
                catch (Exception openErr)
                {
                    this.errMessage = openErr.Message;
                    this.myRepDic = null;
                    return false;
                }

            }


            /// <summary>
            /// 返回词库的总数
            /// </summary>
            /// <returns></returns>
            private int getDicCount()
            {
                if (this.myDic != null)
                {
                    return this.myDic.Count;
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            /// 返回替换词库的总数
            /// </summary>
            /// <returns></returns>
            public int getRepDicCount()
            {
                if (this.myRepDic != null)
                {
                    return this.myRepDic.Count;
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            /// 检查给定的词是否在词库里面, 如果有返回为true,否则返回为false. 如果词典没有加载成功的条件下调用本函数,会返回false.
            /// 本方法的依据是:"是否在词库里面"
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public bool isMisspelled(string word)
            {

                if (word.Length < 3)
                { return true; }
                try
                {
                    return !this.myDic.ContainsKey(word);
                }
                catch (NullReferenceException nul)
                {
                    return true;
                }
             }


            /// <summary>
            /// 判断某个拼写词有没有,直接替换的候选词
            /// </summary>
            /// <param name="mispellword"></param>
            /// <returns></returns>
            public bool hasReplacePear(string mispellword)
            {
                try
                {
                    return this.myRepDic.ContainsKey(mispellword);
                }
                catch (NullReferenceException nul)
                {
                    return false;
                }
            }

            public string getReplacePear(string mispellword)
            {
                return this.myRepDic[mispellword];
            }

            /// <summary>
            /// 按相似度排列    //mardan
            /// </summary>
            /// <param name="list"></param>
            /// <param name="str"></param>
            private void Sort(ref float[] list, ref string[] str)
            {
                float temp;
                string sTemp;
                int i, j;
                bool done = false;
                j = 1;
                while ((j < list.Length) && (!done))
                {
                    done = true;
                    for (i = 0; i < list.Length - j; i++)
                    {
                        if (list[i] < list[i + 1])
                        {
                            done = false;
                            temp = list[i];
                            list[i] = list[i + 1];
                            list[i + 1] = temp;
                            //
                            sTemp = str[i];
                            str[i] = str[i + 1];
                            str[i + 1] = sTemp;

                        }
                    }
                    j++;
                }

            }

            //--------------------------------------------------------------------------------------------------------
            /// <summary>
            /// 获取两个字符的 Levenshtein Distance 相似值
            /// </summary>
            /// <param name="s"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            private int getLD(string firstWord, string secondWord)
            {

                int n = firstWord.Length; //length of s
                int m = secondWord.Length; //length of t
                int[,] d = new int[n + 1, m + 1]; // matrix
                int cost; // cost
                // Step 1
                if (n == 0) return m;
                if (m == 0) return n;
                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++) ;
                for (int j = 0; j <= m; d[0, j] = j++) ;
                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        
                        //----------------< 这部分被为了提高性能,被mardan修改
                        //cost = (secondWord.Substring(j - 1, 1) == firstWord.Substring(i - 1, 1) ? 0 : 1);
                        cost = (secondWord[j - 1] == firstWord[i - 1] ? 0 : 1);
                        //--------------------------------------------------->

                        // Step 6
                        d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                                  d[i - 1, j - 1] + cost);


                    }
                }
                // Step 7
                return d[n, m];
            }
            
            /// <summary>
            /// Levenshtein Distance 相似值 变换为半分值,(小数精度为5)
            /// </summary>
            /// <param name="s"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            private float getLDPercent(string s, string t)
            {
                int l = s.Length > t.Length ? s.Length : t.Length;
                int d = getLevenshtein_distance(s, t);
                float db = 1 - ((float)d / l);
                return (float)Math.Round(db, 5);
                          
            }

            private float getEDPercent(string s, string t)
            {
                int l = s.Length > t.Length ? s.Length : t.Length;
                int d = EditDistance(s, t,true);
                float db = 1 - ((float)d / l);
                return (float)Math.Round(db, 5);

            }


            /// <summary>
            /// 获取两个字符的 Levenshtein Distance 相似值(Yul)
            /// </summary>
            /// <param name="s"></param>
            /// <param name="t"></param>
            /// <returns></returns>
           
           
            private int getLevenshtein_distance(string first, string second)
            {
                int[] d;
                int i;
                int j;
                int n; 
                int m;
                int k; 
                int cost; 
                //int distance; 

                n = first.Length;
                m = second.Length;

                if (n > 0 && m > 0)
                {
                    d = new int[(m + 1) * (n + 1)];
                    m++;
                    n++;

                    //Step 2
                    for (k = 0; k < n; k++)
                    {
                        d[k] = k;
                    }

                    for (k = 0; k < m; k++)
                    {
                        d[k * n] = k;
                    }

                    //Step 3 and 4
                    for (i = 1; i < n; i++)
                    {
                        for (j = 1; j < m; j++)
                        {
                            //Step 5
                            if (first[i - 1] == second[j - 1])
                            {
                                cost = 0;
                            }
                            else
                            {
                                cost = 1;
                            }
                            //Step 6
                            d[j * n + i] = Math.Min(Math.Min(d[(j - 1) * n + i] + 1, d[j * n + i - 1] + 1), d[(j - 1) * n + i - 1] + cost);
                        }
                    }

                    //distance = d[n * m - 1];
                    return d[n * m - 1];
                }
                else
                {
                    return -1; //a negative return value means that one or both strings are empty.
                }
            }


            /*  بۇرۇن سىناقق ئۈچۈن ئىشلەتكەن ئۇسۇل.    مەلۇم ھەرپنىڭ ئۈزۈك تاۋۇش ئىكەنلىكىگە ھۆكۈم قىلىدۇ
            private Boolean Uzuk(char c)                             //判断字母的辅音属性
            {
             
                int asci = (int)c;
                if (asci == 1662 || asci == 1578 || asci == 1670 || asci == 1582 || asci == 1587 || asci == 1588 || asci == 1601 || asci == 1602 || asci == 1603 || asci == 1726 || asci == 1576 || asci == 1583 || asci == 1585 || asci == 1586 || asci == 1688 || asci == 1580 || asci == 1594 || asci == 1711 || asci == 1709 || asci == 1604 || asci == 1605 || asci == 1606 || asci == 1739 || asci == 1610)

                    return true;
                else
                    return false;
            }
             */

            /*  بۇرۇن سىناقق ئۈچۈن ئىشلەتكەن ئۇسۇل.    مەلۇم سۆزنىڭ سوزۇق تاۋۇش قىسمىنى ئېلىپ بېرىدۇ
            private Boolean Sozuq(char c)                            //判断字母的元音属性
            {


                int asci = (int)c;
                if (asci == 1575 || asci == 1608 || asci == 1609 || asci == 1744 || asci == 1749 || asci == 1734 || asci == 1735 || asci == 1736)
                {
                    return true;

                }
                else
                    return false;

            }
             * /
            /// <summary>
            /// sozning sozuk tawuxlirini qikiewetix
            /// </summary>
            /// <param name="word"></param>
            /// <returns></returns>
            
            /* بۇرۇن سىناقق ئۈچۈن ئىشلەتكەن ئۇسۇل.    مەلۇم سۆزنىڭ ئۈزۈك تاۋۇش قىسمىنى ئېلىپ بېرىدۇ
            
            private string getUzuk(string word)
            {
                string uzukWord =  string.Empty;
                foreach (char c in word)
                {
                    if (this.Uzuk(c))
                    {
                        uzukWord = uzukWord + c.ToString();
                    }
                }
                return uzukWord;

            }
             
             */


            //-----------------------------------------------------------------------------------------------------------
            /// <summary>
            ///     Calculates the minimum number of change, inserts or deletes
            ///     required to change firstWord into secondWord
            /// </summary>
            /// <param name="source" type="string">
            ///     <para>
            ///         The first word to calculate
            ///     </para>
            /// </param>
            /// <param name="target" type="string">
            ///     <para>
            ///         The second word to calculate
            ///     </para>
            /// </param>
            /// <param name="positionPriority" type="bool">
            ///     <para>
            ///         set to true if the first and last char should have priority
            ///     </para>
            /// </param>
            /// <returns>
            ///     The number of edits to make firstWord equal secondWord
            /// </returns>
            private static int EditDistance(string source, string target, bool positionPriority)
            {

                // i.e. 2-D array
                Array matrix = Array.CreateInstance(typeof(int), source.Length + 1, target.Length + 1);

                // boundary conditions
                matrix.SetValue(0, 0, 0);

                for (int j = 1; j <= target.Length; j++)
                {
                    // boundary conditions
                    int val = (int)matrix.GetValue(0, j - 1);
                    matrix.SetValue(val + 1, 0, j);
                }

                // outer loop
                for (int i = 1; i <= source.Length; i++)
                {
                    // boundary conditions
                    int val = (int)matrix.GetValue(i - 1, 0);
                    matrix.SetValue(val + 1, i, 0);

                    // inner loop
                    for (int j = 1; j <= target.Length; j++)
                    {
                        int diag = (int)matrix.GetValue(i - 1, j - 1);

                        if (source.Substring(i - 1, 1) != target.Substring(j - 1, 1))
                        {
                            diag++;
                        }

                        int deletion = (int)matrix.GetValue(i - 1, j);
                        int insertion = (int)matrix.GetValue(i, j - 1);
                        int match = Math.Min(deletion + 1, insertion + 1);
                        matrix.SetValue(Math.Min(diag, match), i, j);
                    }//for j
                }//for i

                int dist = (int)matrix.GetValue(source.Length, target.Length);

                // extra edit on first and last chars
                if (positionPriority)
                {
                    if (source[0] != target[0]) dist++;
                    if (source[source.Length - 1] != target[target.Length - 1]) dist++;
                }
                return dist;
            }

            /// <summary>
            ///     Calculates the minimum number of change, inserts or deletes
            ///     required to change firstWord into secondWord
            /// </summary>
            /// <param name="source" type="string">
            ///     <para>
            ///         The first word to calculate
            ///     </para>
            /// </param>
            /// <param name="target" type="string">
            ///     <para>
            ///         The second word to calculate
            ///     </para>
            /// </param>
            /// <returns>
            ///     The number of edits to make firstWord equal secondWord
            /// </returns>
            /// <remarks>
            ///		This method automatically gives priority to matching the first and last char
            /// </remarks>
            public string[] getSuggestionsTemp(string word, int needCount, bool firstLetterSame, bool lastLetterSame)
            {
                //检查词库是否被加载, 如果没有加载,则返回空字符串数组
                try
                {

                    char first, second, secondKey;
                    string[] result;
                    float[] value;
                    float disf;
                    if (word.Length <= 2)
                    { return new string[] { }; }
                    resulList.Clear();
                    #region //从词库一个一个地读取词
                    first = word[0];
                    string key;
                    for (int i = 0; i < this.myDicList.Count; i++)
                    {
                        //两个词的长度区别大于2,则忽略

                        key = myDicList[i];
                        if (Math.Abs(word.Length - key.Length) > this.ignoredLength)
                        {
                            continue;
                        }
                        if (key.Length < 3)
                        {
                            continue;
                        }
                        //如果大开了智能功能,则执行以下算法 (可以设)

                        if (firstLetterSame == true)
                        {
                            #region  智能分析

                            if (Spelling.ContainChar(first, ououeiae))
                            {
                                //如果被检词的第一个字母时没有加 "ئ" 的元音的话暂时不提供智能分析
                            }
                            else if (first != 'ئ')
                            {

                                #region  如果第一个字母维复音
                                if (word[0] != key[0])
                                {
                                    continue;
                                }
                                else
                                {
                                    second = word[1];
                                    secondKey = key[1];
                                    if (Spelling.ContainChar(second, ei))
                                    {
                                        if (!Spelling.ContainChar(secondKey, ei))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (Spelling.ContainChar(second, ouou))
                                    {
                                        if (!Spelling.ContainChar(secondKey, ouou))
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //因为维语中有一些不符合维语传统规格的词(外来),所以取消了这部分. 如:(كرېدىتلىق , پسىخىكىسىدىمۇ)
                                        //if (!ei.IsMatch(secondKey))
                                        //{
                                        //    continue;
                                        //}
                                    }
                                }
                            }
                                #endregion




                            else
                            {

                                #region 如果被检此地第一个字母是元音的前缀 如:"ئ"
                                second = word[1];
                                secondKey = key[1];
                                if (Spelling.ContainChar(second, ei))
                                {
                                    if (key[0] != 'ئ' || !Spelling.ContainChar(secondKey, ei))
                                    {
                                        continue;
                                    }
                                }
                                else if (Spelling.ContainChar(second, ouou))
                                {
                                    if (key[0] != 'ئ' || !Spelling.ContainChar(secondKey, ouou))
                                    {
                                        continue;
                                    }
                                }
                                else if (Spelling.ContainChar(second, ae))
                                {
                                    if (key[0] != 'ئ' || !Spelling.ContainChar(secondKey, ae))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (!Spelling.ContainChar(secondKey, ei))
                                    {
                                        continue;
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }


                        //if (lastLetterSame == true)
                        //{
                        //    if (word.Length > 12)
                        //    {
                        //        if (getLevenshtein_distance(word.Substring(word.Length - 3, 3), word.Substring(key.Length - 3, 3)) > 1)
                        //        { continue; }
                        //    }
                        //}
                        //两个词的最后字母不同,则忽略(可以设)
                        if (lastLetterSame == true && word[word.Length - 1] != key[key.Length - 1])
                        {
                            continue;
                        }
                        if ((disf = this.getLDPercent(word, key)) > 0.6F)
                        {
                            resulList.Add(key, disf);
                        }

                    }//while((link = link.Next) != null );
                    #endregion
                    //排列并过滤工作

                    //如果没有推荐词返回空
                    if (resulList.Count == 0)
                    {
                        return new string[] { };

                    }

                    int count = resulList.Keys.Count;
                    value = new float[count];
                    result = new string[count];
                    resulList.Keys.CopyTo(result, 0);
                    for (int i = 0; i < count; i++)
                    {
                        value[i] = resulList[result[i]];
                    }
                    //排列
                    this.Sort(ref value, ref result);
                    //如果需要的推荐总数参数为-0的话,返回所有
                    if (needCount == -1)
                    {
                        return result;
                    }
                    else
                    {
                        #region  按需要的数目切
                        if (result.Length > needCount)
                        {
                            string[] strs = new string[needCount];
                            for (int i = 0; i < needCount; i++)
                            {
                                strs[i] = result[i];
                            }
                            return strs;
                        }
                        return result;
                        #endregion
                    }

                }
                catch (NullReferenceException nee)
                {
                    this.errMessage = nee.Message;
                    return new string[] { };
                }
            }

            public void getDicTemp(string word)
            {
                //检查词库是否被加载, 如果没有加载,则返回空字符串数组
                try
                {

                    char first, second, secondKey;
                    resulList.Clear();
                    string key;
                    if (word.Length < 3)
                    { return; }
                    #region //从词库一个一个地读取词
                    first = word[0];
                    this.DicTemp.Clear();
                    for (int i = 0; i < this.myDicList.Count; i++)
                    {
                        //两个词的长度区别大于2,则忽略

                        key = myDicList[i];
                        //如果大开了智能功能,则执行以下算法 (可以设)

                        if (Spelling.ContainChar(first, ououeiae))
                        {
                            //如果被检词的第一个字母时没有加 "ئ" 的元音的话暂时不提供智能分析
                        }
                        else if (first != 'ئ')
                        {

                            #region  如果第一个字母维复音
                            if (word[0] != key[0])
                            {
                                continue;
                            }
                            else
                            {
                                if (key.Length < 3)
                                { continue; }
                                second = word[1];
                                secondKey = key[1];
                                if (Spelling.ContainChar(second, ei))
                                {
                                    if (!Spelling.ContainChar(secondKey, ei))
                                    {
                                        continue;
                                    }
                                }
                                else if (Spelling.ContainChar(second, ouou))
                                {
                                    if (!Spelling.ContainChar(secondKey, ouou))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                }
                            }
                        }
                            #endregion




                        else
                        {

                            #region 如果被检此地第一个字母是元音的前缀 如:"ئ"
                            if (key.Length < 3)
                            { continue; }
                            second = word[1];
                            secondKey = key[1];
                            if (Spelling.ContainChar(second, ei))
                            {
                                if (key[0] != 'ئ' || !Spelling.ContainChar(secondKey, ei))
                                {
                                    continue;
                                }
                            }
                            else if (Spelling.ContainChar(second, ouou))
                            {
                                if (key[0] != 'ئ' || !Spelling.ContainChar(secondKey, ouou))
                                {
                                    continue;
                                }
                            }
                            else if (Spelling.ContainChar(second, ae))
                            {
                                if (key[0] != 'ئ' || !Spelling.ContainChar(secondKey, ae))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (!Spelling.ContainChar(secondKey, ei))
                                {
                                    continue;
                                }
                            }
                            #endregion
                        }
                        #endregion

                        this.DicTemp.Add(i);

                    }

                }
                catch (NullReferenceException nee)
                {
                    this.errMessage = nee.Message;
                }
            }

            public string[] getSuggestionsFromDicTemp(string word, int needCount)
            {
                //检查词库是否被加载, 如果没有加载,则返回空字符串数组
                try
                {
                    string[] result;
                    float[] value;
                    float disf;
                    if (word.Length <= 2)
                    {
                        return new string[] { }; //如果候选词的长度小于3个字符,则忽略
                    }
                    resulList.Clear();
                    string key;
                    //string key;
                    for (int i = 0; i < this.DicTemp.Count; i++)
                    {

                        key = this.myDicList[this.DicTemp[i]];
                        //两个词的长度区别大于2,则忽略
                        if (Math.Abs(word.Length - key.Length) > this.ignoredLength)
                        {
                            continue;
                        }
                        if ((disf = this.getLDPercent(word, key)) > 0.6F)
                        {
                            resulList.Add(key, disf);
                        }

                    }//while((link = link.Next) != null );

                    //如果没有推荐词返回空
                    if (resulList.Count == 0)
                    {
                        return new string[] { };

                    }

                    int count = resulList.Keys.Count;
                    value = new float[count];
                    result = new string[count];
                    resulList.Keys.CopyTo(result, 0);
                    for (int i = 0; i < count; i++)
                    {
                        value[i] = resulList[result[i]];
                    }
                    //排列
                    this.Sort(ref value, ref result);
                    //如果需要的推荐总数参数为-0的话,返回所有
                    if (needCount == -1)
                    {
                        return result;
                    }
                    else
                    {
                        #region  按需要的数目切
                        if (result.Length > needCount)
                        {
                            string[] strs = new string[needCount];
                            for (int i = 0; i < needCount; i++)
                            {
                                strs[i] = result[i];
                            }
                            return strs;
                        }
                        return result;
                        #endregion
                    }

                }
                catch (NullReferenceException nee)
                {
                    this.errMessage = nee.Message;
                    return new string[] { };

                }
            }

            public bool addWord(string word,string path)    //mardan
            {
                if (!this.myDic.ContainsKey(word))
                {
                    this.myDicList.Add(word);
                    this.myDic.Add(word, String.Empty);
                    this.fs = new System.IO.FileStream(Path.Combine(path, "user.dic"), System.IO.FileMode.Append);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
                    try
                    {
                        sw.WriteLine(word);
                    }
                    catch (Exception ee)
                    {
                        return false;
                    }

                    sw.Close();
                    this.fs.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool addWordRange(string[] word, string path)    //mardan
            {

                try
                {

                    this.fs = new System.IO.FileStream(Path.Combine(path, "user.dic"), System.IO.FileMode.Append);
                     System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
                     for (int i = 0; i < word.Length; i++)
                     {
                         if (this.myDic.ContainsKey(word[i]))
                             continue;
                         this.myDic.Add(word[i],string.Empty);
                         this.myDicList.Add(word[i]);
                         sw.WriteLine(word[i]);
                     }
                     sw.Close();
                     fs.Close();
                     return true;


                }
                catch (IOException ei)
                {
                    return false;
                }

            }

            public bool addToUserRepdic(string misWord,string rtWord,string path)
            {
                if (!this.myRepDic.ContainsKey(misWord))
                {
                    this.myRepDic.Add(misWord, rtWord);
                    this.fs = new System.IO.FileStream(Path.Combine(path, "userRep.dic"), System.IO.FileMode.Append);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
                    try
                    {
                        sw.WriteLine(misWord+"\t"+rtWord);
                    }
                    catch (Exception ee)
                    {
                        return false;
                    }

                    sw.Close();
                    this.fs.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void AddToUserRepdicWithoutSave(string misWord, string rtWord)
            {
                if (this.myRepDic.ContainsKey(misWord))
                    return;
                this.myRepDic.Add(misWord, rtWord);
            }

            public void AddToUserDicWithoutSave(string word)
            {
                if (this.myDic.ContainsKey(word))
                    return;
                this.myDic.Add(word, string.Empty);
                this.myDicList.Add(word);

            }
            /// <summary>
            /// 检查某个字符是否在某个字符数组里面
            /// </summary>
            /// <param name="c"></param>
            /// <param name="cc"></param>
            /// <returns></returns>
             private static bool ContainChar(char c, char[] cc)
            {

                for (int i = 0; i < cc.Length; i++)
                {
                    if (c == cc[i])
                        return true;
                }
                return false;
            }

           #region IDisposable 成员

            void IDisposable.Dispose()
            {
                if (this.sr != null)
                {
                    this.sr.Close();
                }
                if (this.fs != null)
                {
                    this.fs.Close();
                }
                if (this.myDic != null)
                {
                    this.myDic = null;
                }
                if (this.myDicList != null)
                {
                    this.myDicList = null;
                }
            }

            #endregion
        }


        
    }

}
