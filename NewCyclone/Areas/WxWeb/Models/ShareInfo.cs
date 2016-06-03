using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewCyclone.Areas.WxWeb.Models
{
    /// <summary>
    /// 基础分享
    /// </summary>
    public class ShareInfo
    {
        private string _title = System.Configuration.ConfigurationManager.AppSettings["WebSiteName"].ToString();
        /// <summary>
        /// 分享标题 - 默认为 appsetting.config中的网站名称设置
        /// </summary>
        public string title {
            get { return _title; }
            set { _title = value; }
        }

        private string _link = HttpContext.Current.Request.RawUrl;
        /// <summary>
        /// 分享链接 默认为当前页面的链接地址 必须为全域名+地址的模式
        /// </summary>
        public string link {
            get { return _link; }
            set { _link = value; }
        }

        /// <summary>
        /// 获取分享的链接
        /// </summary>
        /// <returns>返回为加上域名的链接</returns>
        public string getLink() {
            return "http://" + HttpContext.Current.Request.Url.Authority + this.link;
        }

        private string _imgUrl = System.Configuration.ConfigurationManager.AppSettings["WebSiteLogo"].ToString();
        /// <summary>
        /// 分享图标 这里只用传 文件的路径即可 不用管域名 例如/upload/123.jpg
        /// 默认为appsetting.config中的默认图标设置
        /// </summary>
        public string imgUrl {
            get { return _imgUrl; }
            set { _imgUrl = value; }
        }

        /// <summary>
        /// 获取图像的地址
        /// </summary>
        /// <returns>返回加上域名的完整地址</returns>
        public string getImgUrl() {
            return "http://" + HttpContext.Current.Request.Url.Authority + this.imgUrl;
        }

        private string _desc = System.Configuration.ConfigurationManager.AppSettings["WebSiteDesc"].ToString();
        /// <summary>
        /// 分享描述
        /// </summary>
        public string desc {
            get { return _desc; }
            set { _desc = value; }
        }
    }
}