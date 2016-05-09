using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
using Newtonsoft.Json;

namespace NewCyclone.Models
{

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum SysExceptionType {
        /// <summary>
        /// 表示这是一个自定义的异常
        /// </summary>
        自定义,
        /// <summary>
        /// 表示这是一个来自系统的异常
        /// </summary>
        系统
    }


    /// <summary>
    /// 系统自定义异常
    /// </summary>
    public class SysException : ApplicationException {

        /// <summary>
        /// 请求参数
        /// </summary>
        private string condtion { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message">异常内容</param>
        /// <param name="condtion">请求参数</param>
        public SysException(string message, object condtion = null) : base(message) {
            
            if (condtion != null) {
                this.condtion = JsonConvert.SerializeObject(condtion);
            }
        }

        /// <summary>
        /// 保存自定义的异常
        /// </summary>
        private void save() {
            using (var db = new SysModelContainer()) {
                Db_SysExceptionLog d = new Db_SysExceptionLog() {
                    condtion = this.condtion,
                    createdOn = DateTime.Now,
                    message = this.Message,
                    msgType = SysMessageType.异常.GetHashCode(),
                    source = this.Source,
                    stackTrace = this.StackTrace,
                    targetSite = this.TargetSite==null? null :this.TargetSite.ToString(),
                    errorCode = SysExceptionType.自定义.GetHashCode()
                };
                db.Db_SysMsgSet.Add(d);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 返回标准带参数的异常返回对象，并可选是否保存该异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">返回对象</param>
        /// <param name="saveLog">是否保存该异常</param>
        /// <returns></returns>
        public BaseResponse<T> getresult<T>(BaseResponse<T> response, bool saveLog = false)
        {
            if (saveLog) {
                save();
            }
            response.code = BaseResponseCode.异常;
            response.msg = this.Message;
            return response;
        }

        /// <summary>
        /// 返回标准异常返回对象，并可选是否保存该异常
        /// </summary>
        /// <param name="response">返回对象</param>
        /// <param name="saveLog">是否保存该异常</param>
        /// <returns></returns>
        public BaseResponse getresult(BaseResponse response, bool saveLog = false)
        {
            if (saveLog)
            {
                save();
            }
            response.code = BaseResponseCode.异常;
            response.msg = this.Message;
            return response;
        }


        /// <summary>
        /// 保存系统异常
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="request">当前信息的请求参数</param>

        private static void save(Exception e, object request = null)
        {
            string condtion = string.Empty;
            if (request != null) {
                condtion = JsonConvert.SerializeObject(request); 
            }
            using (var db = new SysModelContainer())
            {
                Db_SysExceptionLog d = new Db_SysExceptionLog()
                {
                    condtion = condtion,
                    createdOn = DateTime.Now,
                    message = e.Message,
                    msgType = SysMessageType.异常.GetHashCode(),
                    source = e.Source,
                    stackTrace = e.StackTrace,
                    targetSite = e.TargetSite == null ? null : e.TargetSite.ToString(),
                    errorCode = SysExceptionType.系统.GetHashCode()
                };
                db.Db_SysMsgSet.Add(d);
                db.SaveChanges();
            }
        }



        /// <summary>
        /// 返回标准带参数的异常返回对象，并可选是否保存该异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">返回对象</param>
        /// <param name="e">异常信息</param>
        /// <param name="condtion">请求参数</param>
        /// <param name="saveLog">是否保存异常，默认为true</param>
        /// <returns></returns>
        public static BaseResponse<T> getResult<T>(BaseResponse<T> response,Exception e,object condtion = null,bool saveLog = true) {

            if (saveLog)
            {
                save(e, condtion);
            }
            response.code = BaseResponseCode.异常;
            response.msg = e.Message;
            return response;
        }

        /// <summary>
        /// 返回标准异常返回对象，并可选是否保存异常
        /// </summary>
        /// <param name="response">返回对象</param>
        /// <param name="e">异常信息</param>
        /// <param name="condtion">请求参数</param>
        /// <param name="saveLog">是否保存异常，默认为true</param>
        /// <returns></returns>
        public static BaseResponse getResult(BaseResponse response, Exception e, object condtion = null, bool saveLog = true) {
            if (saveLog)
            {
                save(e, condtion);
            }
            response.code = BaseResponseCode.异常;
            response.msg = e.Message;
            return response;
        }
    }
}