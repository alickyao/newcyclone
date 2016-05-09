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
    /// 系统消息 日志 错误
    /// </summary>
    public class ApiSysMsgController : ApiController
    {
        /// <summary>
        /// 获取全部消息类型
        /// </summary>
        /// <returns></returns>
        public List<VMComboBox> getMesTypeList() {
            return SysHelp.getEnumList(typeof(SysMessageType));
        }

        /// <summary>
        /// 获取全部用户日志类型
        /// </summary>
        /// <returns></returns>
        public List<VMComboBox> getUserLogTypeList()
        {
            return SysHelp.getEnumList(typeof(SysUserLogType));
        }

        /// <summary>
        /// 获取系统异常类型
        /// </summary>
        /// <returns></returns>
        public List<VMComboBox> getExceptionErrorType() {
            return SysHelp.getEnumList(typeof(SysExceptionType));
        }

        /// <summary>
        /// 检索系统所有消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<SysMsg>> searchMsgList(VMMsgSearchMsgRequest condtion) {
            BaseResponse<BaseResponseList<SysMsg>> result = new BaseResponse<BaseResponseList<SysMsg>>();
            try
            {
                result.result = SysMsg.searchLog(condtion);
            }
            catch (Exception e) {
                SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 检索系统用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<SysUserLog>> searchUserLog(ViewModelMsgSearchUserLogReqeust condtion) {
            BaseResponse<BaseResponseList<SysUserLog>> result = new BaseResponse<BaseResponseList<SysUserLog>>();
            try
            {
                result.result = SysUserLog.searchLog(condtion);
            }
            catch (Exception e) {
                SysException.getResult(result, e, condtion);
            }
            return result;
        }

        /// <summary>
        /// 检索系统异常日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<SysExcptionLog>> searchExceptionLog(VMMsgSearchExceptionLogRequest condtion) {
            BaseResponse<BaseResponseList<SysExcptionLog>> result = new BaseResponse<BaseResponseList<SysExcptionLog>>();
            try
            {
                result.result = SysExcptionLog.searchLog(condtion);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, condtion);
            }
            return result;
        }
    }
}
