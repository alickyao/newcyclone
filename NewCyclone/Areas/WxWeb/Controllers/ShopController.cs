using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 商城
    /// </summary>
    public class ShopController : Controller
    {
        // GET: WxWeb/Shop
        public ActionResult Index()
        {
            return View();
        }
    }
}