using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 页面呈现
    /// </summary>
    public class PageController : Controller
    {
        // GET: WxWeb/Page


        
        public ActionResult show()
        {
            return View();
        }
    }
}