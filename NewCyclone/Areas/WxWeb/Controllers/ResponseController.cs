using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NewCyclone.Models;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 微信 响应与基础
    /// </summary>
    public class ResponseController : Controller
    {
        /// <summary>
        /// 微信响应
        /// </summary>
        /// <returns></returns>
        public void response()
        {
            string token = System.Configuration.ConfigurationManager.AppSettings["wxToken"].ToString();

            if (Request.HttpMethod.ToUpper() == "GET")
            {
                #region -- 接口验证
                string echostr = Request["echostr"];
                string signature = Request["signature"];
                string timestamp = Request["timestamp"];
                string nonce = Request["nonce"];
                var arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
                var arrString = string.Join("", arr);
                var sha1 = System.Security.Cryptography.SHA1.Create();
                var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
                StringBuilder enText = new StringBuilder();
                foreach (var b in sha1Arr)
                {
                    enText.AppendFormat("{0:x2}", b);
                }
                if (enText.ToString() == signature)
                {
                    Response.Output.Write(echostr);
                }
                else {
                    Response.Output.Write("false");
                }
                #endregion
            }
            else {
                #region -- 推送消息
                using (Stream stream = Request.InputStream) {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    string postString = Encoding.UTF8.GetString(postBytes);
                    SysNotice.createNotice(new VMMsgCreateSysNoticeRequest {
                        alert = false,
                        message = postString,
                        title = "收到微信消息"
                    });
                }
                #endregion
            }
        }
    }
}