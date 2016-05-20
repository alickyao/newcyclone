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
        [ApiAuthorize(RoleType = SysRolesType.后台)]
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
        /// 检查文档的别名是否可用
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="wid">需排除的ID</param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public int checkAliasIsExist(string alias, string wid)
        {
            return WebDoc.checkAliasIsExist(alias, wid);
        }

        /// <summary>
        /// 添加/编辑网站页面图文内容
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
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
        public BaseResponse<WebDocPage> getpic(string id)
        {
            BaseResponse<WebDocPage> result = new BaseResponse<WebDocPage>();
            try
            {
                result.result = new WebDocPage(id);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, id);
            }
            return result;
        }

        /// <summary>
        /// 根据别名获取图文内容详情
        /// </summary>
        /// <param name="alias">文档的别名</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<WebDocPage> getpicbyAlias(string alias) {
            BaseResponse<WebDocPage> result = new BaseResponse<WebDocPage>();
            try
            {
                result.result = WebDocPage.getDocByAlias(alias);
            }
            catch (SysException e) {
                result = e.getresult(result);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, alias);
            }
            return result;
        }

        /// <summary>
        /// 新增/编辑网站轮播
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<WebDocRote> editrote(VMEditWebPicRoteRequest condtion) {
            BaseResponse<WebDocRote> result = new BaseResponse<WebDocRote>();
            try
            {
                result.result = WebDocRote.edit(condtion);
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
        /// 获取轮播信息详情
        /// </summary>
        /// <param name="id">轮播的ID</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<WebDocRote> getrote(string id) {
            BaseResponse<WebDocRote> result = new BaseResponse<WebDocRote>();
            try
            {
                result.result = new WebDocRote(id);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, id);
            }
            return result;
        }

        /// <summary>
        /// 根据别名获取轮播信息详情
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<WebDocRote> getrotebyAlias(string alias)
        {
            BaseResponse<WebDocRote> result = new BaseResponse<WebDocRote>();
            try
            {
                result.result = WebDocRote.getDocByAlias(alias);
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, alias);
            }
            return result;
        }

        /// <summary>
        /// 为网站内容追加可排序的图集
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<List<SysFileSort>> appendsortpic(VMAppendWebDocFilesSortRequest condtion) {
            BaseResponse<List<SysFileSort>> result = new BaseResponse<List<SysFileSort>>();
            try
            {
                WebDocPage page = new WebDocPage(condtion.docId);
                result.result = page.createFilesSort(condtion.rows);
                result.msg = "图片追加成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 为网站内容追加带描述的图集
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<List<SysFileInfo>> appendinfopic(VMAppendWebDocFilesInfoRequest condtion) {
            BaseResponse<List<SysFileInfo>> result = new BaseResponse<List<SysFileInfo>>();
            try
            {
                WebDoc doc = new WebDoc(condtion.docId);
                result.result = doc.createFilesInfo(condtion.rows);
                result.msg = "图片追加成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 删除页面的图片
        /// </summary>
        /// <param name="condtion">请求参数中的请传入需要删除的文件的ID集合</param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<int> delpic(VMEditListRequest<string> condtion)
        {
            BaseResponse<int> result = new BaseResponse<int>();

            try
            {
                result.result = SysDoc.delfiles(condtion);
                result.msg = "删除成功";
            }
            catch (SysException e)
            {
                result = e.getresult(result);
            }
            catch (Exception e)
            {
                result = SysException.getResult(result, e, condtion);
            }

            return result;
        }
    }
}
