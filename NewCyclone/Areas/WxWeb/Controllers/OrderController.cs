using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 订单
    /// </summary>
    public class OrderController : Controller
    {
        // GET: WxWeb/Order

        /// <summary>
        /// 用户的订单 - 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult index()
        {
            return View();
        }
    }
}