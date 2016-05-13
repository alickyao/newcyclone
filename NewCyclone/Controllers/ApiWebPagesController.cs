using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NewCyclone.Models;

namespace NewCyclone.Controllers
{
    /// <summary>
    /// 网站内容管理
    /// </summary>
    public class ApiWebPagesController : ApiController
    {

        /// <summary>
        /// 获取网站后台所有功能列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<VMComboBox> getfunlist() {
            List<VMComboBox> result = SysHelp.getSysSetList<List<VMComboBox>>("FunWebCms.xml");
            return result;
        }

        /// <summary>
        /// 检索网页文档
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<BaseResponseList<WebDoc>> searchWebDocList(VMSearchWebDocReqeust condtion) {
            BaseResponse<BaseResponseList<WebDoc>> result = new BaseResponse<BaseResponseList<WebDoc>>();
            try
            {
                result.result = WebDoc.searchWebDoc(condtion);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 将文档标记为删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<SysDoc> delete(string id) {
            BaseResponse<SysDoc> result = new BaseResponse<SysDoc>();
            try
            {
                SysDoc d = new SysDoc(id);
                d.delete();
                result.msg = "删除成功";
                result.result = d;
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, id);
            }
            return result;
        }

        /// <summary>
        /// 添加/编辑网站页面图文内容
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<WebDocPage> editpic(VMEditWebDocPageRequest condtion) {
            BaseResponse<WebDocPage> result = new BaseResponse<WebDocPage>();
            try
            {
                result.result = WebDocPage.edit(condtion);
                result.msg = "保存成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result, true);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 获取页面图文内容详情
        /// </summary>
        /// <param name="id">图文内容的ID</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<WebDocPage> getpic(string id) {
            BaseResponse<WebDocPage> result = new BaseResponse<WebDocPage>();
            try
            {
                result.result = new WebDocPage(id);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, id);
            }
            return result;
        }
    }
}
