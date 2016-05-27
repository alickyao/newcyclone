using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NewCyclone.Models;
using NewCyclone.Models.WeiXin;

namespace NewCyclone.Controllers
{
    /// <summary>
    /// 微信-设置与资源，菜单、素材管理
    /// </summary>
    public class ApiWeiXinSetController : ApiController
    {
        /// <summary>
        /// 获取微信菜单按钮类型项目
        /// </summary>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpGet]
        public BaseResponse<List<WxMenuType>> getMenuTypeList() {
            BaseResponse<List<WxMenuType>> result = new BaseResponse<List<WxMenuType>>();
            try
            {
                result.result = WeiXinMenuService.getMenuTypeList();
            }
            catch (Exception e) {
                result = SysException.getResult(result, e);
            }
            return result;
        }

        /// <summary>
        /// 创建微信基本菜单
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles ="admin")]
        [HttpPost]
        public BaseResponse<WxStandardMenuList> createStandardMenu(WxStandardMenuList condtion) {
            BaseResponse<WxStandardMenuList> result = new BaseResponse<WxStandardMenuList>();
            try
            {
                result.result = WeiXinMenuService.createStandardMenu(condtion);
                result.msg = "保存成功";
            }
            catch (SysException e) {
                result = e.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e);
            }
            return result;
        }

        /// <summary>
        /// 获取微信菜单配置详情
        /// </summary>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpGet]
        public BaseResponse<WxMenuInfo> getMenu() {
            BaseResponse<WxMenuInfo> result = new BaseResponse<WxMenuInfo>();
            try
            {
                var m = new WeiXinMenuService();
                result.result = m.getMenu();
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e);
            }
            return result;
        }

        /// <summary>
        /// 删除微信菜单
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public BaseResponse delMenu() {
            BaseResponse result = new BaseResponse();
            try
            {
                WeiXinMenuService.delMenu();
                result.msg = "删除成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e);
            }
            return result;
        }
    }
}
