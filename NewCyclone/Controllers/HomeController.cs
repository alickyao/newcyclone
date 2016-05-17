using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Controllers
{
    /// <summary>
    /// 网站首页
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult index()
        {
            return View();
        }

        /// <summary>
        /// 判断登录跳转
        /// </summary>
        /// <returns></returns>
        public ActionResult login(string ReturnUrl) {
            if (ReturnUrl.IndexOf("Admin") > 0)
            {
                return RedirectToAction("login", "Manager", new { area = "Admin", ReturnUrl = ReturnUrl });
            }
            else {
                return RedirectToAction("login", "Home", new { area = "Web", ReturnUrl = ReturnUrl });
            }
        }
    }
}