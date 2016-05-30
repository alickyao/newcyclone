using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewCyclone.Areas.Admin.Controllers
{
    /// <summary>
    /// 微信基础模块
    /// </summary>
    public class WeiXinController : MBaseController
    {
        // GET: Admin/WeiXin

        
        /// <summary>
        /// 菜单按钮类型说明
        /// </summary>
        /// <returns></returns>
        public ActionResult menuButtonDescribe()
        {
            return View();
        }

        /// <summary>
        /// 菜单管理
        /// </summary>
        /// <returns></returns>
        public ActionResult menu()
        {
            setPageId();
            return View();
        }
    }
}