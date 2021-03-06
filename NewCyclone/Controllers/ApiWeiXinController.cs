﻿using System;
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
    /// 微信-设置与资源，菜单，自动回复管理（继承自基础文档服务）、素材管理
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
        [Authorize(Roles = "admin")]
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


        /// <summary>
        /// 检查key出现的次数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="wid">需排除的id</param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpGet]
        public int getKeyCount(string key, string wid = null) {
            return WeiXinCallBackMsg.getKeyCount(key, wid);
        }

        /// <summary>
        /// 查询 自动消息回复信息列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<BaseResponseList<WeiXinCallBackMsg>> queryCallBackMsg(WxQueryCallBackMsgRequest condtion) {
            BaseResponse<BaseResponseList<WeiXinCallBackMsg>> result = new BaseResponse<BaseResponseList<WeiXinCallBackMsg>>();
            try
            {
                result.result = WeiXinCallBackMsg.query(condtion);
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 查询 自动回复文本消息列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<BaseResponseList<WeiXinCallBackTextMsg>> queryCallBackTextMsg(WxQueryCallBackMsgRequest condtion) {
            BaseResponse<BaseResponseList<WeiXinCallBackTextMsg>> result = new BaseResponse<BaseResponseList<WeiXinCallBackTextMsg>>();
            try
            {
                result.result = WeiXinCallBackTextMsg.queryTextMsg(condtion);
            }
            catch (SysException e)
            {
                result = e.getresult(result,true);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 创建/编辑 自动回复文本消息列表 可批量执行
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<List<WeiXinCallBackTextMsg>> editCallBackTextMsg(VMEditListRequest<WxEditCallBackTextMsgReqest> condtion) {
            BaseResponse<List<WeiXinCallBackTextMsg>> result = new BaseResponse<List<WeiXinCallBackTextMsg>>();
            try
            {
                result.result = new List<WeiXinCallBackTextMsg>();
                foreach (var row in condtion.rows) {
                    result.result.Add(WeiXinCallBackTextMsg.editTextMsg(row));
                }
                result.msg = "保存成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 创建/编辑 自动回复图文消息列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<WeiXinCallBackNewsMsg> editCallBackNewsMsg(WxEditCallBackMsgRequst condtion) {
            BaseResponse<WeiXinCallBackNewsMsg> result = new BaseResponse<WeiXinCallBackNewsMsg>();
            try
            {
                result.result = WeiXinCallBackNewsMsg.editNewsMsg(condtion);
                result.msg = "保存成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

    }
}
