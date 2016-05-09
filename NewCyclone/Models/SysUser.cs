using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.SqlClient;
using System.Web;
using System.IO;
using NewCyclone.DataBase;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Text;

namespace NewCyclone.Models
{
    /// <summary>
    /// 系统角色类型
    /// </summary>
    public enum SysRolesType {
        /// <summary>
        /// 后台专用角色
        /// </summary>
        后台,
        /// <summary>
        /// 其他角色，会员，访客等
        /// </summary>
        其他
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class SysRoles {
        /// <summary>
        /// 系统全部角色
        /// </summary>
        public static List<SysRoles> sysRoles { get; private set; }

        static SysRoles() {
            string path = HttpContext.Current.Server.MapPath("/App_Set/SysRole.xml");
            StreamReader file = new StreamReader(path);
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(List<SysRoles>));
                sysRoles = ((List<SysRoles>)reader.Deserialize(file));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                file.Close();
            }
        }

        /// <summary>
        /// 对应类型的角色
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<SysRoles> getRolesList(SysRolesType t) {
            return sysRoles.Where(p => p.cat == t.GetHashCode()).ToList();
        }


        /// <summary>
        /// 角色
        /// </summary>
        public string role { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public int cat { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string discribe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}[{1}]", this.name, this.role);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (typeof(SysRoles).Equals(obj.GetType())) {
                return this.role.Equals(((SysRoles)obj).role);
            }
            return base.Equals(obj);
        }
    }

    /// <summary>
    /// 基类用户
    /// </summary>
    public class SysUser
    {
        /// <summary>
        /// 初始密码
        /// </summary>
        protected static string defaultPwd = System.Configuration.ConfigurationManager.AppSettings["defaultPwd"].ToString();

        /// <summary>
        /// 登录名（ID）
        /// </summary>
        public string loginName { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string showName { get; set; }

        /// <summary>
        /// 角色，用户的角色
        /// </summary>
        public string role { get; set; }

        /// <summary>
        /// 角色详情
        /// </summary>
        public SysRoles roleInfo { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime createdOn { get; set; }


        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        public Nullable<DateTime> lastLoginTime { get; set; }

        /// <summary>
        /// 是否已经禁用
        /// </summary>
        public bool isDisabled { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loginname"></param>
        public SysUser(string loginname) {
            getInfo(loginname);
        }

        /// <summary>
        /// 设置一个新密码
        /// </summary>
        public void reSetNewPwd(ViewModelChangePwdRequest condtion) {
            SysValidata.valiData(condtion);
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysUserSet.Single(p => p.loginName.Equals(this.loginName));
                if (d.passWord.ToLower().Equals(condtion.oldPwd.ToLower()))
                {
                    changePwd(condtion.newPwd);
                    SysUserLog.saveLog("用户设置了新的密码", SysUserLogType.编辑);
                }
                else {
                    throw new SysException("旧密码不正确", condtion);
                }
            }
        }

        /// <summary>
        /// 重置为系统设定的初始密码
        /// </summary>
        public void reSetPwd() {
            changePwd(defaultPwd);
            SysUserLog.saveLog("重置用户的密码", SysUserLogType.编辑, this.loginName);
        }

        /// <summary>
        /// 在数据库中修改密码
        /// </summary>
        protected void changePwd(string pwd) {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysUserSet.Single(p => p.loginName.Equals(this.loginName));
                d.passWord = pwd;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns>返回被删除的用户对象</returns>
        public void delete() {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysUserSet.Single(p => p.loginName == loginName);
                d.isDeleted = true;
                db.SaveChanges();
                SysUserLog.saveLog("删除用户:" + this.ToString(), SysUserLogType.删除, this.loginName);
            }
        }

        /// <summary>
        /// 禁用或者恢复禁用
        /// </summary>
        /// <param name="condtion"></param>
        public void setDisable(ViewModelSetUserDisable condtion) {
            using (var db = new SysModelContainer()) {
                var user = db.Db_SysUserSet.Single(p => p.loginName.Equals(this.loginName));
                user.isDisabled = condtion.isDisabled;
                db.SaveChanges();
                StringBuilder sb = new StringBuilder();
                sb.Append(condtion.isDisabled ? "设置禁用:" : "恢复禁用:");
                sb.AppendFormat("{0}[备注:{1}]", this.ToString(), condtion.describe);
                SysUserLog.saveLog(sb.ToString(), SysUserLogType.编辑, this.loginName);
            }
        }

        /// <summary>
        /// 设置用户的信息
        /// </summary>
        /// <param name="d"></param>
        protected void setUserInfo(Db_SysUser d) {
            this.loginName = d.loginName;
            this.lastLoginTime = d.lastLoginTime;
            this.isDisabled = d.isDisabled;
            this.role = d.role;
            this.createdOn = d.createdOn;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="loginname"></param>
        protected virtual void getInfo(string loginname) {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysUserSet.Single(p => p.loginName == loginname);
                setUserInfo(d);
                //获取角色
                this.roleInfo = SysRoles.sysRoles.Single(p => p.role.Equals(d.role));

                SysRoles userrole = SysRoles.sysRoles.Single(p => p.role.Equals(this.role));
                string showName = string.Empty;
                if (userrole.cat == 0)
                {
                    //后台
                    
                    showName = db.Db_SysUserSet.OfType<Db_ManagerUser>().Single(p => p.loginName == this.loginName).fullName;
                }
                else {
                    //其他,会员

                }
                this.showName = string.Format("{0}[{1},{2}]", showName, this.roleInfo.name, this.loginName);
            }
        }

        /// <summary>
        /// 比较是否为同一个用户
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (typeof(SysUser).IsAssignableFrom(obj.GetType()))
            {
                SysUser s = (SysUser)obj;
                return s.loginName.Equals(this.loginName);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.loginName.GetHashCode();
        }

    }

    /// <summary>
    /// 系统管后台用户
    /// </summary>
    public class SysManagerUser : SysUser
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string fullName;
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilePhone;
        /// <summary>
        /// 职位
        /// </summary>
        public string jobTitle;


        /// <summary>
        /// 使用用户的ID获取详情
        /// </summary>
        /// <param name="loginname">登录名</param>
        public SysManagerUser(string loginname) : base(loginname)
        {
            getInfo(loginname);
        }

        #region -- 用户菜单
        private List<SysMenu> userMenu = new List<SysMenu>();

        /// <summary>
        /// 获取用户的菜单(根据用户的角色)
        /// </summary>
        /// <returns></returns>
        public List<SysMenu> getUserMenu()
        {
            List<SysMenu> m = SysMenu.sysMenu;
            foreach (var s in m)
            {
                if (s.roles.Count == 0 ? true : s.roles.Contains(this.role))
                {
                    this.userMenu.Add(new SysMenu()
                    {
                        icon = s.icon,
                        roles = s.roles,
                        text = s.text,
                        url = s.url,
                        children = s.children == null ? new List<SysMenu>() : getUserCheldMenu(s.children)
                    });
                }
            }
            return this.userMenu;
        }


        /// <summary>
        /// 根据权限递归子菜单
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private List<SysMenu> getUserCheldMenu(List<SysMenu> children)
        {
            List<SysMenu> child = new List<SysMenu>();
            foreach (var s in children) {
                if (s.roles.Count == 0 ? true : s.roles.Contains(this.role))
                {
                    child.Add(new SysMenu()
                    {
                        icon = s.icon,
                        roles = s.roles,
                        text = s.text,
                        url = s.url,
                        children = s.children == null ? new List<SysMenu>() : getUserCheldMenu(s.children)
                    });
                }
            }
            return child;
        }
        #endregion


        #region -- 验证与新增

        /// <summary>
        /// 获取登录名在数据库中出现的次数
        /// </summary>
        /// <param name="lgname"></param>
        /// <returns></returns>
        public static int getLoginNameCount(string lgname) {
            using (var db = new SysModelContainer()) {
                return (from c in db.Db_SysUserSet where c.loginName.Equals(lgname) select c.loginName).Count();
            }
        }

        /// <summary>
        /// 创建新的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static SysManagerUser create(ViewModelUserRegisterRequest condtion) {
            SysValidata.valiData(condtion);
            int c = getLoginNameCount(condtion.loginname);
            if (c > 0) {
                throw new SysException("登录名已存在", condtion);
            }
            using (var db = new SysModelContainer()) {
                Db_ManagerUser dbuser = new Db_ManagerUser() {
                    createdOn = DateTime.Now,
                    fullName = condtion.fullName,
                    isDeleted = false,
                    isDisabled = false,
                    jobTitle = condtion.jobTitle,
                    loginName = condtion.loginname,
                    mobilePhone = condtion.mobilePhone,
                    passWord = defaultPwd,
                    role = condtion.role
                };
                db.Db_SysUserSet.Add(dbuser);
                db.SaveChanges();
            }
            SysManagerUser newuser = new SysManagerUser(condtion.loginname);
            SysUserLog.saveLog(condtion, SysUserLogType.编辑, newuser.loginName);
            return newuser;
        }

        #endregion


        #region -- 获取

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="loginname"></param>
        protected override void getInfo(string loginname)
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysUserSet.OfType<Db_ManagerUser>().Single(p => p.loginName == loginname);
                setUserInfo(d);
                base.getInfo(loginname);
            }
        }

        /// <summary>
        /// 从数据库中赋值到对象
        /// </summary>
        /// <param name="d"></param>
        private void setUserInfo(Db_ManagerUser d) {
            this.fullName = d.fullName;
            this.jobTitle = d.jobTitle;
            this.mobilePhone = d.mobilePhone;
        }

        #endregion


        #region -- 登陆

        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static SysManagerUser checkLogin(ViewModelLoginReqeust condtion) {
            SysValidata.valiData(condtion);
            using (var db = new SysModelContainer()) {

                //如果还没有用户则需要初始化一个第一个管理员
                int c = (from x in db.Db_SysUserSet.OfType<Db_ManagerUser>() select x.loginName).Count();
                if (c == 0) {
                    create(new ViewModelUserRegisterRequest() {
                        fullName = "管理员",
                        loginname = "admin",
                        role = "admin"
                    });
                }

                var d = db.Db_SysUserSet.OfType<Db_ManagerUser>().SingleOrDefault(p => p.loginName.Equals(condtion.loginName) && p.passWord.ToLower().Equals(condtion.pwd.ToLower()) && !p.isDeleted && !p.isDisabled);
                if (d == null) {
                    throw new SysException("用户名或密码不正确", condtion);
                }
                d.lastLoginTime = DateTime.Now;
                db.SaveChanges();
                SysManagerUser user = new SysManagerUser(condtion.loginName);
                SysUserLog.saveLoginLog(user.loginName);
                return user;
            }
        }

        #endregion

        /// <summary>
        /// 修改后台用户的信息
        /// </summary>
        /// <param name="condtion"></param>
        public SysManagerUser saveInfo(ViewModelUserEditReqeust condtion) {
            SysValidata.valiData(condtion);
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysUserSet.OfType<Db_ManagerUser>().Single(p => p.loginName.Equals(this.loginName));
                d.fullName = condtion.fullName;
                d.jobTitle = condtion.jobTitle;
                d.mobilePhone = condtion.mobilePhone;
                d.role = condtion.role;
                db.SaveChanges();
            }
            SysUserLog.saveLog(condtion, SysUserLogType.编辑, this.loginName);
            getInfo(this.loginName);
            return this;
        }

        /// <summary>
        /// 检索用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static BaseResponseList<SysManagerUser> searchUserList(ViewModelSearchUserBaseRequest condtion)
        {
            BaseResponseList<SysManagerUser> res = new BaseResponseList<SysManagerUser>();
            using (var db = new SysModelContainer()) {

                var r = (from x in db.Db_SysUserSet.OfType<Db_ManagerUser>().AsEnumerable()
                         where !x.isDeleted
                         && (condtion.roles.Count == 0 ? true : condtion.roles.Contains(x.role))
                         && (condtion.loginName.Count == 0 ? true : condtion.loginName.Contains(x.loginName))
                         && (string.IsNullOrEmpty(condtion.q) ? true : (x.loginName.Contains(condtion.q) || x.fullName.Contains(condtion.q) || (string.IsNullOrEmpty(x.mobilePhone) ? false : x.mobilePhone.Contains(condtion.q))))
                         select new { x.loginName, x.createdOn });
                res.total = r.Count();
                if (res.total > 0)
                {
                    r = r.OrderByDescending(p => p.createdOn);
                    if (condtion.page == 0)
                    {
                        res.rows = r.Select(p => new SysManagerUser(p.loginName)).ToList();
                    }
                    else {
                        res.rows = r.Skip(condtion.getSkip()).Take(condtion.pageSize).Select(p => new SysManagerUser(p.loginName)).ToList();
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 格式化用户信息  姓名[角色,登录名]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}[{1},{2}]", this.fullName, this.roleInfo.name, this.loginName);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 编辑后台用户请求
    /// </summary>
    public class ViewModelUserEditReqeust : ItoSysLogMesable
    {


        /// <summary>
        /// 角色
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "角色信息必填")]
        public string role { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "请填写姓名")]
        [StringLength(20, MinimumLength = 2)]
        public string fullName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string jobTitle { get; set; }

        public virtual string toLogString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("编辑用户信息:").AppendFormat("{0},[{1}],[{2}]", this.fullName, this.role, this.mobilePhone);
            return s.ToString();
        }
    }

    /// <summary>
    /// 新增后台用户请求参数
    /// </summary>
    public class ViewModelUserRegisterRequest : ViewModelUserEditReqeust {

        /// <summary>
        /// 登录名
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "登录名必填")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "登录名至少需要5个字符")]
        public string loginname { get; set; }

        public override string toLogString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("添加新用户:").Append(this.fullName).AppendFormat("[{0},{1}]", this.role, this.loginname);
            return s.ToString();
        }
    }

    /// <summary>
    /// 用户登录请求
    /// </summary>
    public class ViewModelLoginReqeust
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required(ErrorMessage = "登录名必填")]
        public string loginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "请输入密码")]
        public string pwd { get; set; }

    }

    /// <summary>
    /// 检索用户请求
    /// </summary>
    public class ViewModelSearchUserBaseRequest : BaseRequest {

        private List<string> _loginName = new List<string>();

        /// <summary>
        /// 登录名
        /// </summary>
        public List<string> loginName {
            get { return _loginName; }
            set { _loginName = value; }
        }

        private List<string> _roles = new List<string>();

        /// <summary>
        /// 角色
        /// </summary>
        public List<string> roles {
            get { return _roles; }
            set { _roles = value; }
        }

        /// <summary>
        /// 关键字：姓名，电话，登录名
        /// </summary>
        public string q { get; set; }
    }

    /// <summary>
    /// 修改密码请求
    /// </summary>
    public class ViewModelChangePwdRequest {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "旧密码需要md5 32位加密")]
        public string oldPwd { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "新密码需要md5 32位加密")]
        public string newPwd { get; set; }
    }

    /// <summary>
    /// 设置用户禁用请求
    /// </summary>
    public class ViewModelSetUserDisable {
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool isDisabled { get; set; }
        /// <summary>
        /// 本次操作的描述
        /// </summary>
        public string describe { get; set; }
    }
}