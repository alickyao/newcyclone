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
        /// 添加/编辑网站页面图文内容
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public BaseResponse<WebDocPage> editWebPages(VMEditWebDocPageRequest condtion) {
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
    }
}
