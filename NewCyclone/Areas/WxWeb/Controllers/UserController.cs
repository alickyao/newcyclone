using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewCyclone.Models;
using NewCyclone.Models.WeiXin;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    
    /// <summary>
    /// 用户基础
    /// </summary>
    [Authorize]
    public class UserBaseController : WxHomeController {
        /// <summary>
        /// 当前登录者的信息
        /// </summary>
        protected WeiXinUser userInfo { get; set; }

        /// <summary>
        /// 初始化当前登录的用户信息
        /// </summary>
        public UserBaseController():base() {
            
        }
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : UserBaseController
    {
        // GET: WxWeb/User

        /// <summary>
        /// 用户中心 -- 入口首页
        /// </summary>
        /// <returns></returns>
        public ActionResult index() {
            this.userInfo = new WeiXinUser(User.Identity.Name);
            ViewBag.userInfo = this.userInfo;
            getTicket();
            return View();
        }
    }
}