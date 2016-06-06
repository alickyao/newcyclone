using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewCyclone.Models;
using NewCyclone.Models.WeiXin;
using System.Text;
using System.Security.Cryptography;
using NewCyclone.Areas.WxWeb.Models;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 微信网站基础
    /// </summary>
    public class WxHomeController : Controller {
        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        protected string appId {
            get { return System.Configuration.ConfigurationManager.AppSettings["wxAppId"].ToString(); }
        }

        /// <summary>
        /// 需要使用的JS接口列表
        /// </summary>
        protected List<string> jsApiList { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        protected string signature { get; set; }

        /// <summary>
        /// 构造微信JSSDK签名
        /// </summary>
        public WxHomeController() {
            ViewBag.appId = this.appId;

            //默认的JS接口权限列表
            jsApiList = new List<string>() {
                "checkJsApi",
                "onMenuShareTimeline",
                "onMenuShareAppMessage",
                "onMenuShareQQ",
                "onMenuShareQZone",
                "onMenuShareWeibo"
            };
            ViewBag.jsApiList = jsApiList;
        }

        
        private string getSHA(string str) {
            byte[] strRes = Encoding.UTF8.GetBytes(str);
            HashAlgorithm iSha = new SHA1CryptoServiceProvider();
            strRes = iSha.ComputeHash(strRes);
            var enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        protected void getTicket() {
            WxJsapi_ticket ticket = WeiXinBase.queryJsApiTicket();
            if (ticket.errcode == 0)
            {
                //签名算法 
                //签名生成规则如下：参与签名的字段包括noncestr（随机字符串）, 有效的jsapi_ticket, timestamp（时间戳）, url（当前网页的URL，不包含#及其后面部分） 。对所有待签名参数按照字段名的ASCII 码从小到大排序（字典序）后，使用URL键值对的格式（即key1=value1&key2=value2…）拼接成字符串string1。这里需要注意的是所有参数名均为小写字符。对string1作sha1加密，字段名和字段值都采用原始值，不进行URL 转义
                StringBuilder sb = new StringBuilder();
                string url = Request.Url.AbsoluteUri;
                string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
                long timestamp = SysHelp.convertDateTimeInt(DateTime.Now);
                url = url.IndexOf("#") > 0 ? url.Substring(0, url.IndexOf("#")) : url;
                sb.Append("jsapi_ticket=").Append(ticket.ticket).Append("&")
         .Append("noncestr=").Append(nonceStr).Append("&")
         .Append("timestamp=").Append(timestamp).Append("&")
         .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
                this.signature = getSHA(sb.ToString());
                ViewBag.signature = signature;
                ViewBag.timestamp = timestamp;
                ViewBag.nonceStr = nonceStr;
            }
            else {
                RedirectToAction("wxJsSdkError");
            }
        }

        /// <summary>
        /// 设置分享信息 - 
        /// </summary>
        /// <param name="info"></param>
        protected void setShare(ShareInfo info) {
            ViewBag.shareInfo = info;
        }
    }

    /// <summary>
    /// 微信网站主界面
    /// </summary>
    public class HomeController : WxHomeController
    {

        // GET: WxWeb/Home

        /// <summary>
        /// 微信网站主界面
        /// </summary>
        /// <returns></returns>
        public ActionResult index()
        {
            //jsApiList //该属性默认设置了一些JS权限  可根据需要进行修改
            getTicket();//需要使用微信JS接口的页面 比如设置右上角的分享等功能 必须调用本方法

            //设置分享 -- 可选
            //setShare(new ShareInfo()
            //{
            //    title = "分享的标题",
            //    desc = "请描述该页面在分享到微信后的描述文本",
            //    imgUrl = "/upload/asadf.jpg"//分享的图片链接地址
            //});

            return View();
        }
    }
}