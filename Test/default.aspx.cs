using darkthread.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ChEncAutoDetect_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            if (Request["mode"] == "analyze")
            {
                string s = "難以判斷";
                try
                {
                    int res = ChEncAutoDetector.Analyze(ChEncAutoDetector.ParseHexStr(Request["data"]));
                    if (res == -1)
                        s = "我猜是簡體";
                    else if (res == 1)
                        s = "我猜是繁體";
                }
                catch
                {

                }
                Response.Write(s);
                Response.End();
            }
            else if (Request["mode"] == "convert")
            {
                string hex = string.Empty, str = string.Empty;
                try
                {
                    Encoding enc = Encoding.GetEncoding(Request["encoding"]);
                    byte[] data = enc.GetBytes(Request["text"]);
                    hex = BitConverter.ToString(data);
                    str = enc.GetString(data);
                }
                catch
                {
                }
                Response.Write(hex + "♞" + str);
                Response.End();
            }
        }
    }
}
