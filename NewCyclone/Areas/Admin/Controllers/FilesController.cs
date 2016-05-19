using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewCyclone.Models;

namespace NewCyclone.Areas.Admin.Controllers
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public class FilesController : MBaseController
    {
        // GET: Admin/Files


        /// <summary>
        /// 列出上传目录下的文件夹
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult listUpload(string pageId,string path)
        {
            ViewBag.path = path;
            setPageId(pageId);

            return View();
        }
    }
}