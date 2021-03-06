﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;

namespace NewCyclone.Models
{

    /// <summary>
    /// 实现该接口的类需提供方法将类的信息格式化为日志文本
    /// </summary>
    public interface ItoSysLogMesable {
        /// <summary>
        /// 返回日志文本
        /// </summary>
        /// <returns></returns>
        string toLogString();
    }

    /// <summary>
    /// 系统消息类型
    /// </summary>
    public enum SysMessageType
    {
        /// <summary>
        /// 来自远程服务器的通知
        /// </summary>
        通知,
        /// <summary>
        /// 用户日志
        /// </summary>
        日志,
        /// <summary>
        /// 异常信息
        /// </summary>
        异常
    }

    /// <summary>
    /// 用户日志类型
    /// </summary>
    public enum SysUserLogType {
        注册,
        登陆,
        编辑,
        删除,
    }


    /// <summary>
    /// 系统消息
    /// </summary>
    public class SysMsg
    {
        /// <summary>
        /// 信息
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }

        /// <summary>
        /// 简化的时间
        /// </summary>
        public string showTime { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public SysMessageType msgType { get; set; }

        /// <summary>
        /// 使用ID构造消息
        /// </summary>
        /// <param name="id"></param>
        public SysMsg(long id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysMsgSet.Single(p => p.Id == id);
                this.Id = d.Id;
                this.createdOn = d.createdOn;
                this.message = d.message;
                this.msgType = (SysMessageType)d.msgType;

                //时间格式化
                DateTime thistime = DateTime.Now;
                var s = thistime - this.createdOn;
                if (s.TotalMinutes < 60)
                {
                    this.showTime = s.TotalMinutes.ToString("0") + "分钟前";
                }
                else if (s.TotalHours < 24)
                {
                    this.showTime = s.TotalHours.ToString("0") + "小时前";
                }
                else if (s.TotalDays < 4)
                {
                    this.showTime = s.TotalDays.ToString("0") + "天前";
                }
                else {
                    this.showTime = this.createdOn.ToShortDateString();
                }
            }
        }

        /// <summary>
        /// 检索系统消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<SysMsg> searchLog(VMMsgSearchMsgRequest condtion) {
            BaseResponseList<SysMsg> result = new BaseResponseList<SysMsg>();
            using (var db = new SysModelContainer()) {
                var r = (from c in db.Db_SysMsgSet.AsEnumerable()
                         where (condtion.msgType.Count == 0 ? true : condtion.msgType.Contains(c.msgType))
                         orderby c.Id descending
                         select c.Id);
                result.total = r.Count();
                if (result.total > 0) {
                    if (condtion.page == 0) {
                        result.rows = r.Select(p => new SysMsg(p)).ToList();
                    }
                    else
                    {
                        result.rows = r.Skip(condtion.getSkip()).Take(condtion.pageSize).Select(p => new SysMsg(p)).ToList();
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 用户日志
    /// </summary>
    public class SysUserLog : SysMsg
    {

        /// <summary>
        /// 对应用户的登录名
        /// </summary>
        protected string loginName { get; set; }

        /// <summary>
        /// 对应用户的显示信息
        /// </summary>
        public SysUser userInfo { get; set; }


        /// <summary>
        /// 用户日志类型
        /// </summary>
        public SysUserLogType logType { get; set; }

        /// <summary>
        /// 关联的其他信息
        /// </summary>
        protected string fkId { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public string device { get; set; }

        /// <summary>
        /// 使用ID构造消息
        /// </summary>
        /// <param name="id"></param>
        public SysUserLog(long id) : base(id)
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysMsgSet.OfType<Db_SysUserLog>().Single(p => p.Id == id);
                this.device = d.device;
                this.ip = d.ip;
                this.fkId = d.fkId;
                this.logType = (SysUserLogType)d.logType;
                this.loginName = d.Db_SysUser_loginName;
            }
            this.userInfo = new SysUser(this.loginName);
        }


        /// <summary>
        /// 保存系统用户日志
        /// </summary>
        /// <param name="condtion">需要保存的参数</param>
        /// <param name="t">类型</param>
        /// <param name="fkId">关联的ID</param>
        public static void saveLog(ItoSysLogMesable condtion, SysUserLogType t, string fkId = null)
        {
            saveLog(condtion.toLogString(), t, fkId);
        }


        /// <summary>
        /// 保存系统用户日志【用户登录请不要使用该方法】
        /// </summary>
        /// <param name="message">需要保存的日志文本</param>
        /// <param name="t">类型</param>
        /// <param name="fkId">关联的ID</param>
        public static void saveLog(string message, SysUserLogType t, string fkId = null)
        {
            if (t != SysUserLogType.登陆)
            {
                string loginname = "admin";
                IIdentity user = HttpContext.Current.User.Identity;
                if (user.IsAuthenticated)
                {
                    loginname = user.Name;
                }
                using (var db = new SysModelContainer())
                {
                    Db_SysUserLog log = new Db_SysUserLog()
                    {
                        createdOn = DateTime.Now,
                        Db_SysUser_loginName = loginname,
                        fkId = fkId,
                        logType = t.GetHashCode(),
                        msgType = SysMessageType.日志.GetHashCode(),
                        message = message,
                        ip = HttpContext.Current.Request.UserHostAddress,
                        device = HttpContext.Current.Request.UserAgent
                    };
                    db.Db_SysMsgSet.Add(log);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 保存用户登录日志
        /// </summary>
        /// <param name="loginName"></param>
        public static void saveLoginLog(string loginName) {
            using (var db = new SysModelContainer())
            {
                Db_SysUserLog log = new Db_SysUserLog()
                {
                    createdOn = DateTime.Now,
                    Db_SysUser_loginName = loginName,
                    logType = SysUserLogType.登陆.GetHashCode(),
                    msgType = SysMessageType.日志.GetHashCode(),
                    message = "用户登录",
                    ip = HttpContext.Current.Request.UserHostAddress,
                    device = HttpContext.Current.Request.UserAgent
                };
                db.Db_SysMsgSet.Add(log);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 检索日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<SysUserLog> searchLog(ViewModelMsgSearchUserLogReqeust condtion) {
            BaseResponseList<SysUserLog> result = new BaseResponseList<SysUserLog>();
            using (var db = new SysModelContainer()) {
                var r = (from c in db.Db_SysMsgSet.OfType<Db_SysUserLog>().AsEnumerable()
                         where ((string.IsNullOrEmpty(condtion.loginName) && string.IsNullOrEmpty(condtion.fkid))? true :(
                            (string.IsNullOrEmpty(condtion.loginName)?false:c.Db_SysUser_loginName == condtion.loginName) || (string.IsNullOrEmpty(condtion.fkid) ? false : c.fkId == condtion.fkid)
                         ))
                         && (condtion.logType == null ? true : c.logType == condtion.logType.GetHashCode())
                         orderby c.createdOn descending
                         select c.Id);
                result.total = r.Count();
                if (result.total > 0) {
                    if (condtion.page == 0)
                    {
                        result.rows = r.Select(p => new SysUserLog(p)).ToList();
                    }
                    else {
                        result.rows = r.Skip(condtion.getSkip()).Take(condtion.pageSize).Select(p => new SysUserLog(p)).ToList();
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 系统异常日志
    /// </summary>
    public class SysExcptionLog : SysMsg {

        /// <summary>
        /// 错误类型
        /// </summary>
        public SysExceptionType errorType { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string condtion { get; set; }

        /// <summary>
        /// 引发异常的应用程序或对象的名称
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string stackTrace { get; set; }

        /// <summary>
        /// 引发当前异常的方法
        /// </summary>
        public string targetSite { get; set; }



        /// <summary>
        /// 使用ID构造消息
        /// </summary>
        /// <param name="id"></param>
        public SysExcptionLog(long id) : base(id)
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysMsgSet.OfType<Db_SysExceptionLog>().Single(p => p.Id == id);
                this.errorType = (SysExceptionType)d.errorCode.GetHashCode();
                this.condtion = d.condtion;
                this.source = d.source;
                this.stackTrace = d.stackTrace;
                this.targetSite = d.targetSite;
            }
        }
        /// <summary>
        /// 检索异常日志
        /// </summary>
        /// <returns></returns>
        public static BaseResponseList<SysExcptionLog> searchLog(VMMsgSearchExceptionLogRequest condtion) {
            BaseResponseList<SysExcptionLog> result = new BaseResponseList<SysExcptionLog>();
            using (var db = new SysModelContainer()) {
                var r = (from c in db.Db_SysMsgSet.OfType<Db_SysExceptionLog>().AsEnumerable()
                         where (condtion.type == null? true :c.errorCode == condtion.type.GetHashCode())
                         && (condtion.beginDate == null? true :c.createdOn>=condtion.beginDate)
                         && (condtion.endDate==null? true :c.createdOn<=condtion.endDate)
                         orderby c.Id descending
                         select c.Id
                         );
                result.total = r.Count();
                if (result.total > 0)
                {
                    if (condtion.page == 0)
                    {
                        result.rows = r.Select(p => new SysExcptionLog(p)).ToList();
                    }
                    else {
                        result.rows = r.Skip(condtion.getSkip()).Take(condtion.pageSize).Select(p => new SysExcptionLog(p)).ToList();
                    }
                }
            }
            return result;
        }
    }


    /// <summary>
    /// 系统通知
    /// </summary>
    public class SysNotice : SysMsg {
        /// <summary>
        /// 是否提示
        /// </summary>
        public bool alert { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool isRead { get; set; }

        /// <summary>
        /// 标记读取的时间
        /// </summary>
        public Nullable<DateTime> readTime { get; set; }

        /// <summary>
        /// 读取的用户ID
        /// </summary>
        public SysUser readUser { get; set; }

        /// <summary>
        /// 点击跳转的URL地址
        /// </summary>
        public string linkUrl { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="id"></param>
        public SysNotice(long id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysMsgSet.OfType<Db_SysNotice>().Single(p => p.Id == id);
                this.alert = d.alert;
                this.title = d.title;
                this.isRead = d.isRead;
                this.readTime = d.readTime;
                this.linkUrl = d.linkUrl;
                if (!string.IsNullOrEmpty(d.readUser)) {
                    this.readUser = new SysUser(d.readUser);
                }
            }
        }

        /// <summary>
        /// 设置为已读
        /// </summary>
        /// <returns></returns>
        public SysNotice setToRead() {
            if (this.isRead) {
                throw new SysException("该信息已是已读状态");
            }
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysMsgSet.OfType<Db_SysNotice>().Single(p => p.Id == this.Id);
                d.isRead = true;
                d.readTime = DateTime.Now;
                d.readUser = HttpContext.Current.User.Identity.Name;
                db.SaveChanges();
                return new SysNotice(this.Id);
            }
        }

        /// <summary>
        /// 批量设置为已读
        /// </summary>
        /// <param name="ids">消息的ID</param>
        /// <returns>返回已成功设置为已读状态的消息的数量</returns>
        public static int batchSetToRead(List<long> ids) {
            if (ids.Count == 0) {
                throw new SysException("消息的ID为必填项", ids);
            }
            int i = 0;
            foreach (long id in ids) {
                SysNotice n = new SysNotice(id);
                if (!n.isRead) {
                    n.setToRead();
                    i++;
                }
            }
            return i;
        }

        /// <summary>
        /// 创建一个系统通知
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static SysNotice createNotice(VMMsgCreateSysNoticeRequest condtion) {
            SysValidata.valiData(condtion);
            using (var db = new SysModelContainer()) {
                Db_SysNotice c = new Db_SysNotice() {
                    alert = condtion.alert,
                    createdOn = DateTime.Now,
                    isRead = false,
                    linkUrl = condtion.linkUrl,
                    message = condtion.message,
                    msgType = SysMessageType.通知.GetHashCode(),
                    title = condtion.title
                };

                Db_SysMsg newrow = db.Db_SysMsgSet.Add(c);
                db.SaveChanges();
                SysNotice n = new SysNotice(newrow.Id);
                return n;
            }
        }

        /// <summary>
        /// 检索系统通知
        /// </summary>
        /// <param name="alert">是否只查询需要提示的消息</param>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<SysNotice> queryNotice(bool alert, BaseRequest condtion) {
            BaseResponseList<SysNotice> result = new BaseResponseList<SysNotice>();

            using (var db = new SysModelContainer()) {
                var rows = (from c in db.Db_SysMsgSet.OfType<Db_SysNotice>().AsEnumerable()
                            where (alert ? c.alert : true)
                            orderby c.isRead ascending, c.Id descending
                            select c.Id
                            );
                result.total = rows.Count();
                if (result.total > 0) {
                    if (condtion.page != 0) {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new SysNotice(p)).ToList();
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 创建系统通知请求
    /// </summary>
    public class VMMsgCreateSysNoticeRequest {
        /// <summary>
        /// 是否提示-设置为true，后台系统会播放声音及弹出提示框
        /// </summary>
        public bool alert { get; set; }

        /// <summary>
        /// 消息标题-显示框上的标题
        /// </summary>
        [Required]
        public string title { get; set; }

        /// <summary>
        /// 消息正文-显示在提示框中的消息内容
        /// </summary>
        [Required]
        public string message { get; set; }

        /// <summary>
        /// 点击消息跳转的链接地址
        /// </summary>
        public string linkUrl { get; set; }
    }


    /// <summary>
    /// 检索系统消息请求
    /// </summary>
    public class VMMsgSearchMsgRequest : BaseRequest {
        private List<int> _msgType = new List<int>();

        /// <summary>
        /// 消息类型 来自枚举 SysMessageType
        /// </summary>
        public List<int> msgType {
            get { return _msgType; }
            set { _msgType = value; }
        }
    }

    /// <summary>
    /// 用户日志检索请求
    /// </summary>
    public class ViewModelMsgSearchUserLogReqeust : BaseRequest {

        /// <summary>
        /// 用户ID，登录名
        /// </summary>
        public string loginName { get; set; }

        /// <summary>
        /// 关联的ID
        /// </summary>
        public string fkid { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public Nullable<SysUserLogType> logType { get; set; }
    }

    /// <summary>
    /// 错误日志检索请求
    /// </summary>
    public class VMMsgSearchExceptionLogRequest : BaseRequest {
        /// <summary>
        /// 类型
        /// </summary>
        public Nullable<SysExceptionType> type { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public Nullable<DateTime> beginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public Nullable<DateTime> endDate { get; set; }
    }
}