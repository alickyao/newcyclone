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
    /// 文件服务
    /// </summary>
    public class ApiSysFileController : ApiController
    {

        /// <summary>
        /// 调整文件排序
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponse<List<SysFileSort>> editFilesSort(VMEditListRequest<VMEditFileSortRequest> condtion) {
            BaseResponse<List<SysFileSort>> result = new BaseResponse<List<SysFileSort>>();
            try
            {
                result.result = SysFileSort.editSort(condtion);
                result.msg = "调整成功";
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
    }
}
