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
    /// 基础文档服务
    /// </summary>
    public class ApiSysDocController : ApiController
    {

        /// <summary>
        /// 将文档标记为删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<SysDoc> delete(string id)
        {
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
        /// 为文档追加可排序的文件
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<List<SysFileSort>> appendsortpic(VMAppendWebDocFilesSortRequest condtion)
        {
            BaseResponse<List<SysFileSort>> result = new BaseResponse<List<SysFileSort>>();
            try
            {
                SysDoc page = new SysDoc(condtion.docId);
                result.result = page.createFilesSort(condtion.rows);
                result.msg = "图片追加成功";
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

        /// <summary>
        /// 为文档内容追加带描述的文件
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<List<SysFileInfo>> appendinfopic(VMAppendWebDocFilesInfoRequest condtion)
        {
            BaseResponse<List<SysFileInfo>> result = new BaseResponse<List<SysFileInfo>>();
            try
            {
                SysDoc doc = new SysDoc(condtion.docId);
                result.result = doc.createFilesInfo(condtion.rows);
                result.msg = "图片追加成功";
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

        /// <summary>
        /// 删除文档的文件
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
