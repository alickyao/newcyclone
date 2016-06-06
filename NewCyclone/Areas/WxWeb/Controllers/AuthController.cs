using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewCyclone.Models;
using NewCyclone.Models.WeiXin;
using System.Web.Security;

namespace NewCyclone.Areas.WxWeb.Controllers
{
    /// <summary>
    /// 授权
    /// </summary>
    public class AuthController : WxHomeController
    {
        /// <summary>
        /// 跳转到网页授权页面（静默/用户授权）
        /// </summary>
        /// <param name="returnUrl">授权成功后返回的本地url地址  格式:/controller/action?args=xxx</param>
        /// <param name="state">一些自定义的参数</param>
        /// <param name="scope">应用授权作用域，snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）</param>
        public void snsapi_base(string returnUrl, string state = "",string scope = "snsapi_base")
        {
            string callback = "snsapi_base_callback";
            if (scope == "snsapi_userinfo") {
                callback = "snsapi_userinfo_callback";
            }
            string rUrl = "http://" + Request.Url.Authority + Url.Action(callback) + "?returnUrl=" + returnUrl;
            string url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect", this.appId, Server.UrlEncode(rUrl), scope, state);
            Response.Redirect(url);
        }

        /// <summary>
        /// 静默授权回调处理
        /// </summary>
        /// <param name="returnUrl">回调URL</param>
        /// <param name="code">code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。</param>
        /// <param name="state">由静默回调中传入的自定义的一些参数</param>
        public void snsapi_base_callback(string returnUrl, string code, string state)
        {
            WxAuthWebAccess_token t = WeiXinBase.getWebAccess_token(code);
            WeiXinUser userinfo = WeiXinUser.getLoginNameByOpenId(t.openid);
            if (userinfo != null)
            {
                if (!userinfo.isDisabled && !userinfo.isDeleted)
                {
                    //保存登录信息
                    FormsAuthentication.SetAuthCookie(userinfo.loginName, true);
                    //更新最后一次登录时间
                    try
                    {
                        userinfo.updateLastLoginTime();
                        SysUserLog.saveLoginLog(userinfo.loginName);
                    }
                    catch { }
                    Response.Redirect(returnUrl);
                }
                else {
                    Response.Redirect("showAuthLoginError");
                }
            }
            else {
                //检查用户是否已在系统注册-如果未注册则跳转到用户授权界面
                Response.Redirect(string.Format("snsapi_base?returnUrl={0}&state={1}&scope=snsapi_userinfo", returnUrl, state));
            }
        }

        /// <summary>
        /// 弹出授权页面回调处理
        /// </summary>
        /// <param name="returnUrl">回调URL</param>
        /// <param name="code">code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。</param>
        /// <param name="state">由静默回调中传入的自定义的一些参数</param>
        public void snsapi_userinfo_callback(string returnUrl, string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                //用户点击了确认授权按钮

                //根据code换access_token
                WxAuthWebAccess_token t = WeiXinBase.getWebAccess_token(code);
                //根据access_token拉取用户信息
                WxAuthWebUserInfo weixinuserinfo = WeiXinBase.getWebUserInfo(t.access_token, t.openid);
                //保存用户信息
                WeiXinUser userinfo = WeiXinUser.create(weixinuserinfo);
                //设置用户为登录状态
                if (!userinfo.isDisabled && !userinfo.isDeleted)
                {
                    //保存登录信息
                    FormsAuthentication.SetAuthCookie(userinfo.loginName, true);
                    //跳转到returnUrl
                    Response.Redirect(returnUrl);
                }
                else {
                    Response.Redirect("showAuthLoginError");
                }
            }
            else {
                //用户没有点击确认授权按钮
                Response.Redirect("authCancel");
            }
        }

        /// <summary>
        /// 清除当前登录者的用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult singOut() {
            if (User.Identity.IsAuthenticated) {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// 用户取点击了取消授权按钮后显示
        /// </summary>
        /// <returns></returns>
        public ActionResult authCancel() {
            getTicket();
            return View();
        }

        /// <summary>
        /// 当用户状态禁用或者已删除时呈现
        /// </summary>
        /// <returns></returns>
        public ActionResult showAuthLoginError()
        {
            getTicket();
            return View();
        }
    }
}