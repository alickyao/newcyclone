using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 微信网站主界面
    /// </summary>
    public class HomeController : Controller
    {

        // GET: WxWeb/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}