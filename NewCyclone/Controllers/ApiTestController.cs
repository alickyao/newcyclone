using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NewCyclone.Models;
using NewCyclone.Models.WeiXin;
using System.Text;

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
        public BaseResponse test()
        {
            BaseResponse res = new BaseResponse();
            try
            {
                //string str = @"<xml><ToUserName><![CDATA[toUser]]></ToUserName><FromUserName><![CDATA[fromUser]]></FromUserName> <CreateTime>1348831860</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[this is a test]]></Content><MsgId>1234567890123456</MsgId></xml>";
                //WxReceiveMsg msg = WeiXinMsgService.deserializePostString(str);
                //res.msg = msg.returnMsg();
                WeiXinMaterialService.uploadFile1();
                //WeiXinMaterialService.downloadFile("tK4Hq4h_5otxhcq-5Ica-VhaGC2NZIKHgFZwXXacBfrJF-c2ve805PH_-ticarGg");
            }
            catch (SysException e) {
                res = e.getresult(res, true);
            }
            return res;
        }
    }
}
