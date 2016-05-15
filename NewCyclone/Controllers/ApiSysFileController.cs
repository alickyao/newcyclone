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
        /// 创建可排序的文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="sort">排序号码</param>
        /// <returns>返回被创建的文件记录信息</returns>
        [HttpGet]
        [Authorize]
        public BaseResponse<SysFileSort> savefilesort(string filepath, int sort = 0)
        {
            BaseResponse<SysFileSort> res = new BaseResponse<SysFileSort>();
            try
            {
                res.result = SysFileSort.create(new VMCreateFileSortRequest(filepath)
                {
                   sort = sort
                });
                res.msg = "保存成功";
            }
            catch (SysException e) {
                res = e.getresult(res);
            }
            catch (Exception e) {
                res = SysException.getResult(res, e, filepath);
            }
            return res;
        }
    }
}
