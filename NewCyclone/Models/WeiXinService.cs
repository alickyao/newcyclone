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
using System.Xml;
using System.Xml.Linq;

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
                    else {
                        e = JsonConvert.DeserializeAnonymousType<WxBaseResponse>(response, e);
                        throw new SysException(string.Format("微信接口返回异常:{0},错误代码:[{1}]", e.errmsg, e.errcode), string.Format("url:{0},method:{1};返回[code:{2},msg:{3}],数据包:", url, method, e.errcode, e.errmsg) + datas);
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
    /// 微信接口-基础服务
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
        /// 公众号原始ID
        /// </summary>
        protected static string originalId {
            get { return System.Configuration.ConfigurationManager.AppSettings["originalId"].ToString(); }
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
        [StringLength(40, ErrorMessage = "菜单标题不能超过40个字节")]
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
    /// 微信菜单服务
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
    [Serializable]
    public class WxBaseMsg {
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
    /// 接收到的消息
    /// </summary>
    [Serializable]
    public  class WxReceiveMsg : WxBaseMsg
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get; set; }

        /// <summary>
        /// 消息返回
        /// </summary>
        /// <returns></returns>
        public virtual string returnMsg() {
            return "";
        }
    }

    /// <summary>
    /// 文本消息
    /// </summary>
    [Serializable]
    public class WxTextMsg : WxReceiveMsg
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 接收消息的服务器地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("您好", this.FromUserName);
        }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class WxPicMsg : WxReceiveMsg {
        /// <summary>
        /// 图片链接
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 图片消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 接收消息的服务器地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("已经收到图片", this.FromUserName);
        }
    }
    /// <summary>
    /// 语音消息
    /// </summary>
    public class WxVoiceMsg : WxReceiveMsg {
        /// <summary>
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Recognition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("已经收到语音消息", this.FromUserName);
        }
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    public class WxVideoMsg : WxReceiveMsg {
        /// <summary>
        /// 视频消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string ThumbMediaId { get; set; }

        /// <summary>
        /// 接收消息的服务器地址
        /// </summary>
        public string URL { get; set; }

        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("已经收到视频消息", this.FromUserName);
        }
    }

    /// <summary>
    /// 小视频消息
    /// </summary>
    public class WxShortvideoMsg : WxReceiveMsg {
        /// <summary>
        /// 视频消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string ThumbMediaId { get; set; }

        /// <summary>
        /// 接收消息的服务器地址
        /// </summary>
        public string URL { get; set; }

        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("已经收到小视频消息", this.FromUserName);
        }
    }

    /// <summary>
    /// 地理位置消息
    /// </summary>
    public class WxLocationMsg : WxReceiveMsg {
        /// <summary>
        /// 地理位置维度
        /// </summary>
        public double Location_X { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Location_Y { get; set; }
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 接收消息的服务器地址
        /// </summary>
        public string URL { get; set; }

        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("已经收到地理位置", this.FromUserName);
        }
    }

    /// <summary>
    /// 链接消息
    /// </summary>
    public class WxLinkMsg : WxReceiveMsg {
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 接收消息的服务器地址
        /// </summary>
        public string URL { get; set; }

        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("已经收到您发送的链接", this.FromUserName);
        }
    }

    #endregion

    /// <summary>
    /// 微信消息服务
    /// </summary>
    public class WeiXinMsgService : WeiXinBase {

        /// <summary>
        /// 返序列化微信推送的XML格式消息
        /// </summary>
        /// <param name="postString">微信推送的XML数据</param>
        public static WxReceiveMsg deserializePostString(string postString) {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(postString);
            XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
            if (MsgType != null)
            {
                WxReceiveMsg t = new WxReceiveMsg();
                switch (MsgType.InnerText) {
                    case "text":
                        t = ConvertObj<WxTextMsg>(postString);
                        break;
                    case "image":
                        t = ConvertObj<WxPicMsg>(postString);
                        break;
                    case "voice":
                        t = ConvertObj<WxVoiceMsg>(postString);
                        break;
                    case "video":
                        t = ConvertObj<WxVideoMsg>(postString);
                        break;
                    case "shortvideo":
                        t = ConvertObj<WxShortvideoMsg>(postString);
                        break;
                    case "location":
                        t = ConvertObj<WxLocationMsg>(postString);
                        break;
                    case "link":
                        t = ConvertObj<WxLinkMsg>(postString);
                        break;
                }
                return t;
            }
            else {
                throw new SysException("错误的XML文件格式", postString);
            }
        }

        /// <summary>
        /// 将微信发送的XML字符串序列化为对象
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="xmlstr">微信发送的XML字符串</param>
        /// <returns></returns>
        public static T ConvertObj<T>(string xmlstr)
        {
            XElement xdoc = XElement.Parse(xmlstr);
            var type = typeof(T);
            var t = Activator.CreateInstance<T>();
            foreach (XElement element in xdoc.Elements())
            {
                var pr = type.GetProperty(element.Name.ToString());
                pr.SetValue(t, Convert.ChangeType(element.Value, pr.PropertyType), null);
            }
            return t;
        }

        /// <summary>
        /// 获取一个文本消息
        /// </summary>
        /// <param name="content">文本消息内容</param>
        /// <param name="toUserOpenId">接收者的OpenId</param>
        /// <returns>XML格式的字符串</returns>
        public static string getTextMsg(string content, string toUserOpenId) {
            StringBuilder sb = new StringBuilder("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>",toUserOpenId);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", originalId);
            sb.AppendFormat("<CreateTime>{0}</CreateTime>", SysHelp.convertDateTimeInt(DateTime.Now));
            sb.Append("<MsgType><![CDATA[text]]></MsgType>");
            sb.AppendFormat("<Content><![CDATA[{0}]]></Content>", content);
            sb.Append("</xml>");
            return sb.ToString();
        }
    }
}