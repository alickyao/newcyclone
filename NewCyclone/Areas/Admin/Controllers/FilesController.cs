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

        /// <summary>
        /// 显示一个带描述的文件
        /// </summary>
        /// <param name="pageId">页面的ID</param>
        /// <param name="fileId">文件的ID</param>
        /// <param name="url">或者文件的URL</param>
        /// <returns></returns>
        public ActionResult getFileInfoListHtml(string pageId, string fileId, string url = null)
        {
            setPageId(pageId);
            ViewBag.url = url;
            VMEditFileInfoRequest f = new VMEditFileInfoRequest();
            if (!string.IsNullOrEmpty(fileId)) {
                SysFileInfo info = new SysFileInfo(fileId);
                f = new VMEditFileInfoRequest() {
                    describe = info.describe,
                    fileId = info.Id,
                    link = info.link,
                    sort = info.sort,
                    title = info.title,
                };
                ViewBag.url = info.url;
            }
            ViewBag.file = f;
            return View();
        }
    }
}