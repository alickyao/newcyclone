using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace NewCyclone.Models.WeiXin
{

    /// <summary>
    /// 获取微信远程接口数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeiXinGetResponse<T> {
        /// <summary>
        /// 返回对象
        /// </summary>
        public T t;

        /// <summary>
        /// 错误详情
        /// </summary>
        public WxBaseResponse e;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="url">远程地址url</param>
        /// <param name="method">方法 get/post</param>
        /// <param name="datas">需要POST的数据</param>
        public WeiXinGetResponse(string url, string method, object datas = null) {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method.ToUpperInvariant();

            if (request.Method == "POST" && datas != null) {
                // 设置请求返回格式为json
                request.ContentType = "application/json";
                // 将请求参数写入请求流对象

                string d = JsonConvert.SerializeObject(datas);

                byte[] buffer = Encoding.UTF8.GetBytes(d);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
            }

            using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                {
                    string response = stream.ReadToEnd();

                    JObject j = JObject.Parse(response);
                    JToken error = j.GetValue("errcode");
                    string code = error == null ? "0" : error.ToString();
                    if (code == "0") {
                        t = JsonConvert.DeserializeAnonymousType<T>(response, t);
                    }
                    else{
                        e = JsonConvert.DeserializeAnonymousType<WxBaseResponse>(response, e);
                        throw new SysException(string.Format("微信接口返回异常:{0},错误代码:[{1}]",e.errmsg,e.errcode), string.Format("url:{0},method:{1};返回[code:{2},msg:{3}],数据包:", url, method, e.errcode, e.errmsg) + datas);
                    }

                    //保存到系统通知
                    if (typeof(WxAccess_token).Equals(t.GetType())) {
                        //如果返回对象是 WxAccess_token
                        SysNotice.createNotice(new VMMsgCreateSysNoticeRequest() {
                            alert = false,
                            message = "服务器已获取了微信通信凭证",
                            title = "微信接口"
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 获取返回对象
        /// </summary>
        /// <returns></returns>
        public T getRetrun() {
            return t;
        }
    }

    #region -- 基础模型

    /// <summary>
    /// 查询凭证返回对象
    /// </summary>
    public class WxAccess_token
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }

    /// <summary>
    /// 微信基本返回数据结构
    /// </summary>
    public class WxBaseResponse
    {
        /// <summary>
        /// 错误代码 http://mp.weixin.qq.com/wiki/10/6380dc743053a91c544ffd2b7c959166.html
        /// 为0是表示成功
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string errmsg { get; set; }
    }

    #endregion

    /// <summary>
    /// 微信接口-基础
    /// </summary>
    public class WeiXinBase {

        const string httpHost = "http://api.weixin.qq.com/cgi-bin";
        const string httpsHost = "https://api.weixin.qq.com/cgi-bin";

        /// <summary>
        /// Token(令牌)
        /// </summary>
        protected static string wxToken {
            get { return System.Configuration.ConfigurationManager.AppSettings["wxToken"].ToString(); }
        }

        /// <summary>
        /// AppID(应用ID)
        /// </summary>
        protected static string appId
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["appId"].ToString(); }
        }

        /// <summary>
        /// AppSecret(应用密钥)
        /// </summary>
        protected static string appSecret
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["appSecret"].ToString(); }
        }


        /// <summary>
        /// 获取远程请求的URL
        /// </summary>
        /// <param name="model">模块 例如 https://api.weixin.qq.com/cgi-bin/menu/create?access_token=ACCESS_TOKEN  menu 为 模块</param>
        /// <param name="action">操作 例如 https://api.weixin.qq.com/cgi-bin/menu/create?access_token=ACCESS_TOKEN  create 为 操作</param>
        /// <param name="httpType"></param>
        /// <param name="getToken">是否获取令牌</param>
        /// <returns>返回构建好的URL</returns>
        protected static string geturl(string model, string action, EnumHttpRequestType httpType = EnumHttpRequestType.http, bool getToken = true)
        {
            StringBuilder sb = new StringBuilder(httpType == EnumHttpRequestType.http ? httpHost : httpsHost);
            if (!string.IsNullOrEmpty(model))
            {
                sb.AppendFormat("/{0}", model);
            }
            if (!string.IsNullOrEmpty(action))
            {
                sb.AppendFormat("/{0}", action);
            }
            if (getToken)
            {
                sb.AppendFormat("?access_token=" + queryToken().access_token);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取凭证
        /// </summary>
        /// <returns></returns>
        public static WxAccess_token queryToken() {
            string cacheKey = appId + appSecret;
            if (HttpRuntime.Cache.Get(cacheKey) != null && HttpRuntime.Cache.Get(cacheKey).ToString().Length > 0)
            {
                return (WxAccess_token)HttpRuntime.Cache.Get(cacheKey);
            }
            string url = geturl("token", "", EnumHttpRequestType.https, false);
            url = string.Format(url + "?grant_type=client_credential&appid={0}&secret={1}", appId, appSecret);
            WxAccess_token t = new WeiXinGetResponse<WxAccess_token>(url, "get").getRetrun();
            //保存到缓存中
            HttpRuntime.Cache.Insert(cacheKey, t, null, DateTime.Now.AddSeconds(t.expires_in), TimeSpan.Zero);
            return t;
        }
    }


    #region -- 菜单模型

    /// <summary>
    /// 微信菜单点击事件类型
    /// </summary>
    public class WxMenuType
    {
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public string clickType { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 类型描述
        /// </summary>
        public string describe { get; set; }
    }

    /// <summary>
    /// 微信菜单详情
    /// </summary>
    public class WxWxMenuInfo {
        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// 一级菜单最多4个汉字，二级菜单最多7个汉字，多出来的部分将会以“...”代替。
        /// </summary>
        [Required]
        [StringLength(40,ErrorMessage ="菜单标题不能超过40个字节")]
        public string name { get; set; }

        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        [Required]
        public string type { get; set; }

        /// <summary>
        /// 菜单的响应动作类型 描述
        /// </summary>
        public WxMenuType typeInfo { get; set; }

        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// click等点击类型必须
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 网页链接，用户点击菜单可打开链接，不超过1024字节
        /// view类型必须
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// media_id类型和view_limited类型必须
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// 二级菜单
        /// </summary>
        public List<WxWxMenuInfo> sub_button { get; set; }
    }

    /// <summary>
    /// 标准菜单
    /// </summary>
    public class WxStandardMenuList
    {
        /// <summary>
        /// 标准菜单的按钮信息
        /// 一级菜单不超过3个 每个一级菜单下的菜单不超过5个
        /// </summary>
        [Required]
        public List<WxWxMenuInfo> button { get; set; }
    }

    /// <summary>
    /// 微信菜单信息
    /// </summary>
    public class WxMenuInfo {
        /// <summary>
        /// 微信标准菜单对象
        /// </summary>
        [Required]
        public WxStandardMenuList menu { get; set; }
    }

    #endregion

    /// <summary>
    /// 微信菜单
    /// </summary>
    public class WeiXinMenuService : WeiXinBase
    {
        /// <summary>
        /// 获取微信可用的菜单结合
        /// </summary>
        /// <returns></returns>
        public static List<WxMenuType> getMenuTypeList() {
            List<WxMenuType> result = SysHelp.getSysSetList<List<WxMenuType>>("WxMenuType.xml");
            return result;
        }

        /// <summary>
        /// 微信菜单
        /// </summary>
        protected WxMenuInfo info { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public WeiXinMenuService() {
            string url = geturl("menu", "get", EnumHttpRequestType.https);
            info = new WeiXinGetResponse<WxMenuInfo>(url, "get").getRetrun();
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <returns></returns>
        public WxMenuInfo getMenu() {
            return info;
        }

        /// <summary>
        /// 创建标准微信菜单
        /// </summary>
        public static WxStandardMenuList createStandardMenu(WxStandardMenuList request) {
            SysValidata.valiData(request);

            //数据验证
            int i = 0;
            foreach (var m1 in request.button) {
                i++;
                if (m1.sub_button != null) {
                    int j = 0;
                    foreach (var m2 in m1.sub_button) {
                        j++;
                        if (j >= 5) {
                            throw new SysException("二级菜单不能超过5个", request);
                        }
                    }
                }
                if (i >= 3) {
                    throw new SysException("一级菜单不能超过3个", request);
                }
            }

            string url = geturl("menu", "create", EnumHttpRequestType.https);
            var t = new WeiXinGetResponse<WxBaseResponse>(url, "post", request);
            return request;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        public static void delMenu() {
            string url = geturl("menu", "delete", EnumHttpRequestType.https);
            new WeiXinGetResponse<WxBaseResponse>(url, "get");
        }
    }


    #region -- 消息模型

    /// <summary>
    /// 基础消息
    /// </summary>
    public abstract class WxBaseMsg {
        /// <summary>
        /// 接收方帐号（OpenId或者开发者微信号）
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 发送者账号（OpenId或者开发者微信号）
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间 
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }
    }
    /// <summary>
    /// 文本消息
    /// </summary>
    public class WxTextMsg : WxBaseMsg
    {
        /// <summary>
        /// 回复的消息内容
        /// </summary>
        public string Content { get; set; }
    }

    #endregion

    /// <summary>
    /// 微信消息
    /// </summary>
    public class WeiXinMsgService : WeiXinBase {

    }
}