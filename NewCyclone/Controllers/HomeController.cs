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
            if (ReturnUrl.IndexOf("/Admin") != -1)
            {
                //后台
                return RedirectToAction("login", "Manager", new { area = "Admin", ReturnUrl = ReturnUrl });
            }
            else if (ReturnUrl.IndexOf("/wx") != -1)
            {
                //微信
                return RedirectToAction("snsapi_base", "Auth", new { area = "WxWeb", returnUrl = ReturnUrl });
            }
            else {
                //其他
                return RedirectToAction("login", "Home", new { area = "Web", ReturnUrl = ReturnUrl });
            }
        }
    }
}