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
    /// 测试用
    /// </summary>
    public class ApiTestController : ApiController
    {
        /// <summary>
        /// 测试
        /// </summary>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<VMDiskFileQueryResponse> test(string path)
        {
            BaseResponse<VMDiskFileQueryResponse> res = new BaseResponse<VMDiskFileQueryResponse>();
            res.result = SysFile.listFiles(path);
            return res;
        }
    }
}
