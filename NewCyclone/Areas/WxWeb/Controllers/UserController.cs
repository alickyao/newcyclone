using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : Controller
    {
        // GET: WxWeb/User

        /// <summary>
        /// 用户中心 -- 入口首页
        /// </summary>
        /// <returns></returns>
        public ActionResult index() {
            return View();
        }
    }
}