using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using NewCyclone.Models;

namespace NewCyclone.Controllers
{
    /// <summary>
    /// 后台基础
    /// </summary>
    public class MBaseController : Controller {
        /// <summary>
        /// 页面的ID
        /// </summary>
        public string pageId;
        /// <summary>
        /// 为页面ID赋值
        /// </summary>
        public MBaseController() {
            pageId = SysHelp.getNewId("HHmmss");
            ViewBag.pageId = pageId;
        }

        /// <summary>
        /// 重新设置页面的PageId
        /// </summary>
        /// <param name="pId">重设的pageId</param>
        public void setPageId(string pId = null) {
            if (!string.IsNullOrEmpty(pId)) {
                this.pageId = pId;
                ViewBag.pageId = pId;
            }
        }
    }

    /// <summary>
    /// 后台系统界面控制登陆退出与主界面
    /// </summary>
    public class ManagerController : MBaseController
    {
        /// <summary>
        /// 后台登录
        /// </summary>
        /// <returns></returns>
        public ActionResult login() {
            return View();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public ActionResult logout() {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("login");
        }

        /// <summary>
        /// 后台主页
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult index()
        {
            SysManagerUser mu = new SysManagerUser(User.Identity.Name);
            ViewBag.userMenu = mu.getUserMenu();
            ViewBag.userinfo = mu;
            return View();
        }

        /// <summary>
        /// 工作台
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult workTab() {
            return View();
        }
    }

    /// <summary>
    /// 后台用户相关
    /// </summary>
    public class ManagerUserController : MBaseController {
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult changepwd(string pageId = null)
        {
            setPageId(pageId);
            return View();
        }

        /// <summary>
        /// 当前登陆人的信息
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userInfo(string pageId = null) {
            setPageId(pageId);
            SysManagerUser user = new SysManagerUser(User.Identity.Name);
            ViewBag.user = user;
            return View();
        }

        /// <summary>
        /// 后台用户列表
        /// </summary>
        /// <param name="pageId">界面ID</param>
        /// <param name="viewset">界面设置</param>
        /// <param name="query">查询参数</param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userList(ViewModelSearchUserBaseRequest query, string pageId = null, string viewset = "default")
        {
            setPageId(pageId);
            ViewBag.query = query;
            return View("userList_" + viewset);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public ActionResult editUser(string loginName, string pageId = null) {
            setPageId(pageId);
            ViewBag.loginName = loginName;
            return View();
        }
    }

    /// <summary>
    /// 后台消息
    /// </summary>
    public class ManagerMsgController : MBaseController {

        /// <summary>
        /// 用户日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <param name="viewset"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userLog(ViewModelMsgSearchUserLogReqeust condtion, string pageId, string viewset = "default") {
            setPageId(pageId);
            ViewBag.condtion = condtion;
            ViewModelSearchUserBaseRequest searchUserRequest = new ViewModelSearchUserBaseRequest();
            ViewBag.searchUserRequest = searchUserRequest;
            return View("userLog_" + viewset);
        }

        /// <summary>
        /// 用户日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult userLogInfo(int id) {
            ViewBag.info = new SysUserLog(id);
            return View();
        }

        /// <summary>
        /// 异常日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult exceptionLog(VMMsgSearchExceptionLogRequest condtion, string pageId)
        {
            setPageId(pageId);
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 异常日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult exceptionLogInfo(long id) {
            ViewBag.info = new SysExcptionLog(id);
            return View();
        }

    }

    /// <summary>
    /// 分类与树
    /// </summary>
    public class ManagerTreeController : MBaseController {

        /// <summary>
        /// 分类树网格
        /// </summary>
        /// <param name="fun">功能模块标示</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult catTreeList(string fun, string pageId = null)
        {
            setPageId(pageId);
            ViewBag.fun = fun;
            return View();
        }

        /// <summary>
        /// 左分类树又网格界面
        /// </summary>
        /// <param name="fun">功能模块标示</param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult catTreeGrid(string fun, string pageId = null)
        {
            setPageId(pageId);
            ViewBag.fun = fun;
            return View();
        }
    }

    /// <summary>
    /// 网页文档（图文消息，轮播等）
    /// </summary>
    public class ManagerWebDocController : MBaseController {

        /// <summary>
        /// 检索文档
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult docList(VMSearchWebDocReqeust condtion, string pageId)
        {
            setPageId(pageId);
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 创建/编辑图文
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ActionResult editpic(VMEditWebDocPageRequest condtion, string pageId) {
            setPageId(pageId);
            ViewBag.condtion = condtion;
            if (!string.IsNullOrEmpty(condtion.Id)) {
                ViewBag.info = new WebDocPage(condtion.Id);
            }
            return View();
        }

        /// <summary>
        /// 创建/编辑轮播
        /// </summary>
        /// <returns></returns>
        public ActionResult editrote() {
            return View();
        }

        /// <summary>
        /// 编辑产品
        /// </summary>
        /// <returns></returns>
        public ActionResult editproduct() {
            return View();
        }
    }
}