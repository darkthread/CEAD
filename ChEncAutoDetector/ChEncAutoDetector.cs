using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace darkthread.tools
{
    public class ChEncAutoDetector
    {
        /// <summary>
        /// 分析報告
        /// </summary>
        class Report
        {
            //統計解讀為ASCII、符號、常用字、次常用字及亂碼(非有效字元)字數
            public int Ascii, Symbol, Common, Rare, Unknow;
            /// <summary>
            /// 亂碼指標(數值愈大，不是該編碼的機率愈高)
            /// </summary>
            public float BadSmell
            {
                get
                {
                    int total = Ascii + Symbol + Common + Rare + Unknow;
                    if (total == 0) return 0;
                    return (float)(Rare + Unknow * 3) / total;
                }
            }
        }

        //將00-01-02-03格式轉為byte[]
        public static byte[] ParseHexStr(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return new byte[] { };
            var l = (hex.Length + 1) / 3;
            byte[] b = new byte[l];
            int i = 0;
            foreach (string h in hex.Split('-'))
                b[i++] = Convert.ToByte(h, 16);
            return b;
        }

        /// <summary>
        /// 分析二進位資料為繁體或是簡體
        /// </summary>
        /// <param name="data">二進位內容</param>
        /// <returns>1表示較可能為繁體, -1表示較可能為簡體, 0表無法識別</returns>
        public static int Analyze(byte[] data)
        {
            var resBig5 = AnalyzeBig5(data);
            var resGB = AnalyzeGB2312(data);
            if (resBig5.BadSmell < resGB.BadSmell)
                return 1;
            else if (resBig5.BadSmell > resGB.BadSmell)
                return -1;
            else
                return 0;
        }
        //試著解析成BIG5編碼，取得分析報告
        private static Report AnalyzeBig5(byte[] data)
        {
            Report res = new Report();
            bool isDblBytes = false;
            byte dblByteHi = 0;
            foreach (byte b in data)
            {
                if (isDblBytes)
                {
                    if (b >= 0x40 && b <= 0x7e || b >= 0xa1 && b <= 0xfe)
                    {
                        int c = dblByteHi * 0x100 + b;
                        if (c >= 0xa140 && c <= 0xa3bf)
                            res.Symbol++; //符號
                        else if (c >= 0xa440 && c <= 0xc67e)
                            res.Common++; //常用字
                        else if (c >= 0xc940 && c <= 0xf9d5)
                            res.Rare++; //次常用字
                        else
                            res.Unknow++; //無效字元
                    }
                    else
                        res.Unknow++;
                    isDblBytes = false;
                }
                else if (b >= 0x80 && b <= 0xfe)
                {
                    isDblBytes = true;
                    dblByteHi = b;
                }
                else if (b < 0x80)
                    res.Ascii++;
            }
            return res;
        }
        //試著解析成GB2312，取得分析報告
        private static Report AnalyzeGB2312(byte[] data)
        {
            Report res = new Report();
            bool isDblBytes = false;
            byte dblByteHi = 0;
            foreach (byte b in data)
            {
                if (isDblBytes)
                {
                    if (b >= 0xa1 && b <= 0xfe)
                    {
                        if (dblByteHi >= 0xa1 && dblByteHi <= 0xa9)
                            res.Symbol++; //符號
                        else if (dblByteHi >= 0xb0 && dblByteHi <= 0xd7)
                            res.Common++; //一級漢字(常用字)
                        else if (dblByteHi >= 0xd8 && dblByteHi <= 0xf7)
                            res.Rare++; //二級漢字(次常用字)
                        else
                            res.Unknow++; //無效字元
                    }
                    else
                        res.Unknow++; //無效字元
                    isDblBytes = false;
                }
                else if (b >= 0xa1 && b <= 0xf7)
                {
                    isDblBytes = true;
                    dblByteHi = b;
                }
                else if (b < 0x80)
                    res.Ascii++;
            }
            return res;
        }
    }
}
