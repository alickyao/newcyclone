using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using NewCyclone.Models;
using System.Web.Security;
using System.Threading;

namespace NewCyclone.Controllers
{

    /// <summary>
    /// 系统管理员相关
    /// </summary>
    public class ApiSysManagerUserController : ApiController
    {

        /// <summary>
        /// 获取系统角色信息
        /// </summary>
        /// <param name="t">角色的类型</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public List<SysRoles> getRolsList(SysRolesType t) {
            return SysRoles.getRolesList(t);
        } 

        /// <summary>
        /// 验证并登陆
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<SysManagerUser> login(ViewModelLoginReqeust condtion) {
            BaseResponse<SysManagerUser> result = new BaseResponse<SysManagerUser>();
            try
            {
                Thread.Sleep(500);
                result.result = SysManagerUser.checkLogin(condtion);
                FormsAuthentication.SetAuthCookie(result.result.loginName, true);
                result.msg = "登录成功";
            }
            catch (SysException ex)
            {
                result = ex.getresult(result, true);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse logout() {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                return new BaseResponse()
                {
                    msg = "退出登录成功"
                };
            }
            else {
                return new BaseResponse()
                {
                    msg = "您还没有登录"
                };
            }
        }

        /// <summary>
        /// 重置用户的密码
        /// </summary>
        /// <param name="loginName">用户的登录名</param>
        /// <returns></returns>
        [Authorize(Roles ="admin")]
        [HttpGet]
        public BaseResponse reSetPwd(string loginName) {
            BaseResponse result = new BaseResponse();
            try
            {
                SysManagerUser user = new SysManagerUser(loginName);
                user.reSetPwd();
                result.msg = "重置成功";
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, loginName);
            }
            return result;
        }

        /// <summary>
        /// 修改新的登录密码
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse reSetNewPwd(ViewModelChangePwdRequest condtion) {
            BaseResponse result = new BaseResponse();
            try
            {
                SysManagerUser user = new SysManagerUser(User.Identity.Name);
                user.reSetNewPwd(condtion);
                result.msg = "密码修改成功";
            }
            catch (SysException ex) {
                result = ex.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }


        /// <summary>
        /// 检查登录名出现的次数
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpGet]
        public int checkLoginName(string loginName)
        {
            return SysManagerUser.getLoginNameCount(loginName);
        }

        /// <summary>
        /// 创建后台人员
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles ="admin")]
        public BaseResponse<SysManagerUser> createUser(ViewModelUserRegisterRequest condtion) {
            BaseResponse<SysManagerUser> result = new BaseResponse<SysManagerUser>();
            try
            {
                result.result = SysManagerUser.create(condtion);
                result.msg = "保存成功";
            }
            catch (SysException r)
            {
                return r.getresult(result,true);
            }
            catch (Exception e) {
                return SysException.getResult(result, e, condtion);
            }
            return result;
        }


        /// <summary>
        /// 获取后台用户信息
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public SysManagerUser getUserInfo(string loginName) {
            SysManagerUser user = new SysManagerUser(loginName);
            return user;
        }

        /// <summary>
        /// 编辑后台用户信息
        /// </summary>
        /// <param name="loginName">被编辑的用户ID(登录名)</param>
        /// <param name="condtion">请求</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<SysManagerUser> editUser(string loginName, ViewModelUserEditReqeust condtion) {
            BaseResponse<SysManagerUser> result = new BaseResponse<SysManagerUser>();
            try
            {
                if (!User.IsInRole("admin"))
                {
                    loginName = User.Identity.Name;
                }

                SysManagerUser smu = new SysManagerUser(loginName);
                result.result = smu.saveInfo(condtion);
                result.msg = "保存成功";
            }
            catch (SysException ex)
            {
                result =  ex.getresult(result, true);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            
            return result;
        }

        /// <summary>
        /// 检索用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles ="admin")]
        public BaseResponse<BaseResponseList<SysManagerUser>> searchUserList(ViewModelSearchUserBaseRequest condtion) {
            BaseResponse<BaseResponseList<SysManagerUser>> res = new BaseResponse<BaseResponseList<SysManagerUser>>();
            try
            {
                res.result = SysManagerUser.searchUserList(condtion);
            }
            catch (Exception e) {
                res = SysException.getResult(res, e, condtion);
            }
            return res;
        }

        /// <summary>
        /// 设置禁用
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public BaseResponse setDisable(string loginName, ViewModelSetUserDisable condtion) {
            BaseResponse result = new BaseResponse();
            try
            {
                SysManagerUser user = new SysManagerUser(loginName);
                user.setDisable(condtion);
                result.msg = "设置成功";
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 删除后台用户
        /// </summary>
        /// <param name="loginName">用户登录名</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public BaseResponse delete(string loginName)
        {
            BaseResponse res = new BaseResponse();
            try
            {
                SysUser user = new SysManagerUser(loginName);
                user.delete();
                res.msg = "删除成功";
            }
            catch (Exception e) {
                res = SysException.getResult(res, e, loginName);
            }
            return res;
        }
    }
}
