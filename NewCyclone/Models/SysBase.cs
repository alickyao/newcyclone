using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Xml.Serialization;

namespace NewCyclone.Models
{
    /// <summary>
    /// 基本返回类型
    /// </summary>
    public enum BaseResponseCode {
        /// <summary>
        /// 操作成功
        /// </summary>
        正常,
        /// <summary>
        /// 发生异常
        /// </summary>
        异常
    }

    /// <summary>
    /// 标准返回对象
    /// </summary>
    public class BaseResponse {
        /// <summary>
        /// 代码
        /// </summary>
        public BaseResponseCode code { get; set; }

        /// <summary>
        /// 返回提示
        /// </summary>
        public string msg { get; set; }
    }

    /// <summary>
    /// 带数据的标准返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T> {

        /// <summary>
        /// 代码
        /// </summary>
        public BaseResponseCode code { get; set; }

        /// <summary>
        /// 返回提示
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 具体的返回内容
        /// </summary>
        public T result { get; set; }
    }


    /// <summary>
    /// 标准返回列表对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponseList<T> {
        /// <summary>
        /// 存在的记录的总数
        /// </summary>
        public int total { get; set; }


        private List<T> _rows = new List<T>();
        /// <summary>
        /// 行
        /// </summary>
        public List<T> rows {
            get { return _rows; }
            set { _rows = value; }
        }
    }

    /// <summary>
    /// 标准列表请求参数
    /// </summary>
    public class BaseRequest {
        private int _page = 1;
        /// <summary>
        /// 调取的页码 默认1，如果不需要系统进行翻页则需要传入0
        /// </summary>
        public int page {
            get { return _page; }
            set { _page = value; }
        }

        private int _pageSize = 20;
        /// <summary>
        /// 每页的数量 默认 20
        /// </summary>
        public int pageSize {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
        /// <summary>
        /// 根据页码获取需要跳过的行的数量
        /// </summary>
        /// <returns></returns>
        public int getSkip() {
            return (this.page - 1) * this.pageSize;
        }
    }

    /// <summary>
    /// 系统参数验证
    /// </summary>
    public class SysValidata {
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="condtion">参数</param>
        /// <param name="throwException">是否直接抛出异常，默认为true</param>
        /// <returns></returns>
        public static BaseResponse<List<ValidationResult>> valiData(object condtion,bool throwException = true) {
            BaseResponse<List<ValidationResult>> result = new BaseResponse<List<ValidationResult>>();

            var context = new ValidationContext(condtion, null, null);

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(condtion, context, results, true);

            result.result = results;

            if (results != null && results.Count > 0) {
                if (throwException)
                {
                    throw new SysException(results[0].ErrorMessage, condtion);
                }
                result.code = BaseResponseCode.异常;
                result.msg = results[0].ErrorMessage;
            }
            return result;
        }
    }

    /// <summary>
    /// 扩展的系统角色验证【API】
    /// </summary>
    public class ApiAuthorize : System.Web.Http.AuthorizeAttribute {
        /// <summary>
        /// 角色类型，如果指定了特定的角色，该值无效
        /// </summary>
        public SysRolesType RoleType { get; set; }

        /// <summary>
        /// 自定义权限验证
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(this.Roles))
                {
                    List<SysRoles> roles = SysRoles.getRolesList(RoleType);
                    foreach (SysRoles role in roles)
                    {
                        if (HttpContext.Current.User.IsInRole(role.role))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else {
                    return base.IsAuthorized(actionContext);
                }
            }
            else {
                return false;
            }
        }
    }

    /// <summary>
    /// 扩展的系统角色验证【UI】
    /// </summary>
    public class SysAuthorize : System.Web.Mvc.AuthorizeAttribute
    {
        /// <summary>
        /// 角色类型，如果指定了特定的角色，该值无效
        /// </summary>
        public SysRolesType RoleType { get; set; }

        /// <summary>
        /// 自定义授权验证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(this.Roles))
                {
                    List<SysRoles> roles = SysRoles.getRolesList(RoleType);
                    foreach (SysRoles role in roles)
                    {
                        if (httpContext.User.IsInRole(role.role))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else {
                    return base.AuthorizeCore(httpContext);
                }
            }
            else {
                return false;
            }

        }
    }

    /// <summary>
    /// 常用的一些静态方法
    /// </summary>
    public class SysHelp {

        private static char[] code = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static Random random = new Random();

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static string getrandmonstirng(int c = 4) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < c; i++) {
                sb.Append(code[random.Next(26)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 随机生成一个不重复的字符串ID
        /// </summary>
        /// <param name="arg">字符串ID中的时间格式，默认为yyMMddHHmmss</param>
        /// <returns></returns>
        public static string getNewId(string arg = null) {
            if (string.IsNullOrEmpty(arg))
            {
                arg = "yyMMddHHmmss";
            }
            
            StringBuilder sb = new StringBuilder();
            sb.Append(getrandmonstirng()).Append("_");
            sb.Append(DateTime.Now.ToString(arg));
            
            return sb.ToString();
        }

        /// <summary>
        /// 获取枚举集合
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<VMComboBox> getEnumList(Type t) {
            List<VMComboBox> result = new List<VMComboBox>();
            foreach (var s in Enum.GetValues(t))
            {
                int value = Convert.ToInt32(s);
                string text = s.ToString();
                result.Add(new VMComboBox()
                {
                    id = value.ToString(),
                    text = text
                });
            }
            return result;
        }

        /// <summary>
        /// 将指定的配置文件转换为对象
        /// </summary>
        /// <param name="filename">配置文件名，在App_Set文件夹中</param>
        /// <returns></returns>
        public static T getSysSetList<T>(string filename) {
            string path = HttpContext.Current.Server.MapPath("/App_Set/" + filename);
            StreamReader file = new StreamReader(path);
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(T));
                T t = (T)reader.Deserialize(file);
                return t;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                file.Close();
            }
        }
    }

    /// <summary>
    /// HTTP协议类型
    /// </summary>
    public enum EnumHttpRequestType {
        /// <summary>
        /// http协议
        /// </summary>
        http,
        /// <summary>
        /// https协议
        /// </summary>
        https
    }
}