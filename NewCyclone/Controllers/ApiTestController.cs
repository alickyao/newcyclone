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
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<List<VMComboBox>> test()
        {
            BaseResponse<List<VMComboBox>> res = new BaseResponse<List<VMComboBox>>();
            res.result = SysHelp.getSysSetList<List<VMComboBox>>("FunWebCms.xml");
            return res;
        }
    }
}
