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
using System.Collections;
using NewCyclone.DataBase;

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
        protected T t;

        /// <summary>
        /// 错误详情
        /// </summary>
        protected WxBaseResponse e;

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
                        if (j > 5) {
                            throw new SysException("二级菜单不能超过5个", request);
                        }
                    }
                }
                if (i > 3) {
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
    /// 接收到的消息或者事件
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

    /// <summary>
    /// 事件推送
    /// </summary>
    public class WxEventMsg:WxReceiveMsg {
        /// <summary>
        /// 事件类型 subscribe 关注，LOCATION  CLICK  VIEW
        /// </summary>
        public string Event { get; set; }
    }

    /// <summary>
    /// 关注事件
    /// </summary>
    public class WxEventSubscribeMsg : WxEventMsg {
        /// <summary>
        /// 关注事件为字符串空值
        /// </summary>
        public string EventKey { get; set; }

        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("欢迎关注", this.FromUserName);
        }
    }
    /// <summary>
    /// 菜单点击事件
    /// </summary>
    public class WxEventClickMsg : WxEventMsg {
        /// <summary>
        /// 事件KEY值，与自定义菜单接口中KEY值对应
        /// </summary>
        public string EventKey { get; set; }

        public override string returnMsg()
        {
            return WeiXinMsgService.getTextMsg("您点击的按钮的KEY是：" + this.EventKey, this.FromUserName);
        }
    }

    /// <summary>
    /// 菜单点击跳转URL事件
    /// </summary>
    public class WxEventViewMsg : WxEventMsg
    {
        /// <summary>
        /// 事件KEY值，设置的跳转URL
        /// </summary>
        public string EventKey { get; set; }
    }

    /// <summary>
    /// 上报地理位置事件
    /// 用户同意上报地理位置后，每次进入公众号会话时，都会在进入时上报地理位置，或在进入会话后每5秒上报一次地理位置，公众号可以在公众平台网站中修改以上设置。上报地理位置时，微信会将上报地理位置事件推送到开发者填写的URL。
    /// </summary>
    public class WxEventLatitudeMsg : WxEventMsg {
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 地理位置精度
        /// </summary>
        public double Precision { get; set; }
    }

    /// <summary>
    /// 被动回复的图文消息详情
    /// </summary>
    public class WxReturnPicMsgDetail {
        /// <summary>
        /// 图文消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图文消息描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 点击图文消息跳转链接
        /// </summary>
        public string Url { get; set; }
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
                    case "text"://文字
                        t = ConvertObj<WxTextMsg>(postString);
                        break;
                    case "image"://图片
                        t = ConvertObj<WxPicMsg>(postString);
                        break;
                    case "voice"://语音
                        t = ConvertObj<WxVoiceMsg>(postString);
                        break;
                    case "video"://视频
                        t = ConvertObj<WxVideoMsg>(postString);
                        break;
                    case "shortvideo"://小视频
                        t = ConvertObj<WxShortvideoMsg>(postString);
                        break;
                    case "location"://位置
                        t = ConvertObj<WxLocationMsg>(postString);
                        break;
                    case "link"://链接
                        t = ConvertObj<WxLinkMsg>(postString);
                        break;
                    case "event"://事件
                        XmlNode EventType = xmldoc.SelectSingleNode("/xml/Event");
                        if (EventType != null) {
                            switch (EventType.InnerText) {
                                case "subscribe":
                                    t = ConvertObj<WxEventSubscribeMsg>(postString);
                                    break;
                                case "LOCATION":
                                    t = ConvertObj<WxEventLatitudeMsg>(postString);
                                    break;
                                case "CLICK":
                                    t = ConvertObj<WxEventClickMsg>(postString);
                                    break;
                                case "VIEW":
                                    t = ConvertObj<WxEventViewMsg>(postString);
                                    break;
                            }
                        }
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

        /// <summary>
        /// 获取图文消息（多个）
        /// </summary>
        /// <param name="news"></param>
        /// <param name="toUserOpenId"></param>
        /// <returns></returns>
        public static string getNewsMsg(List<WxReturnPicMsgDetail> news, string toUserOpenId) {
            StringBuilder sb = new StringBuilder("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", toUserOpenId);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", originalId);
            sb.AppendFormat("<CreateTime>{0}</CreateTime>", SysHelp.convertDateTimeInt(DateTime.Now));
            sb.Append("<MsgType><![CDATA[news]]></MsgType>");
            sb.AppendFormat("<ArticleCount>{0}</ArticleCount>", news.Count);
            sb.Append("<Articles>");
            foreach (var n in news) {
                sb.Append("<item>");
                sb.AppendFormat("<Title><![CDATA[{0}]]></Title>", n.Title);
                sb.AppendFormat("<Description><![CDATA[{0}]]></Description>", n.Description);
                sb.AppendFormat("<PicUrl><![CDATA[{0}]]></PicUrl>", n.PicUrl);
                sb.AppendFormat("<Url><![CDATA[{0}]]></Url>", n.Url);
                sb.Append("</item>");
            }
            sb.Append("</Articles>");
            sb.Append("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取图文消息（单个）
        /// </summary>
        /// <param name="news"></param>
        /// <param name="toUserOpenId"></param>
        /// <returns></returns>
        public static string getNewsMsg(WxReturnPicMsgDetail news, string toUserOpenId) {
            StringBuilder sb = new StringBuilder("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", toUserOpenId);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", originalId);
            sb.AppendFormat("<CreateTime>{0}</CreateTime>", SysHelp.convertDateTimeInt(DateTime.Now));
            sb.Append("<MsgType><![CDATA[news]]></MsgType>");
            sb.AppendFormat("<ArticleCount>{0}</ArticleCount>", 1);
            sb.Append("<Articles>");
            sb.Append("<item>");
            sb.AppendFormat("<Title><![CDATA[{0}]]></Title>", news.Title);
            sb.AppendFormat("<Description><![CDATA[{0}]]></Description>", news.Description);
            sb.AppendFormat("<PicUrl><![CDATA[{0}]]></PicUrl>", news.PicUrl);
            sb.AppendFormat("<Url><![CDATA[{0}]]></Url>", news.Url);
            sb.Append("</item>");
            sb.Append("</Articles>");
            sb.Append("</xml>");
            return sb.ToString();
        }
    }



    #region -- 被动消息回复模型

    /// <summary>
    /// 被动消息编辑基础请求参数
    /// </summary>
    public class WxEditCallBackMsgRequst {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 对应点击事件的key值
        /// </summary>
        [Required]
        [StringLength(50)]
        public string key { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public string caption { get; set; }
    }

    /// <summary>
    /// 创建/编辑自动回复文本消息请求
    /// </summary>
    public class WxEditCallBackTextMsgReqest : WxEditCallBackMsgRequst {

        /// <summary>
        /// 文本内容
        /// </summary>
        [Required]
        public string content { get; set; }
    }

    /// <summary>
    /// 创建/编辑自动回复图文消息请求
    /// </summary>
    public class WxEditCallBackNewsMsgRequest : WxEditCallBackMsgRequst {
        /// <summary>
        /// 图文消息列表-最多不超过10个
        /// </summary>
        [Required]
        public List<WeiXinCallBackNewsMsgDetail> detail { get; set; }
    }

    /// <summary>
    /// 查询文本请求
    /// </summary>
    public class WxQueryCallBackMsgRequest : BaseRequest
    {
        /// <summary>
        /// 类型 test/news
        /// </summary>
        public string fun { get; set; }

        /// <summary>
        /// 对应的点击事件的key  全字匹配
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 对应的标题  模糊匹配
        /// </summary>
        public string caption { get; set; }
    }

    #endregion

    /// <summary>
    /// 被动消息回复 继承自 SysDoc
    /// </summary>
    public class WeiXinCallBackMsg : SysDoc
    {
        /// <summary>
        /// 类型  text/news
        /// </summary>
        public string fun { get; set; }
        /// <summary>
        /// 对应点击事件的key值
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WeiXinCallBackMsg(string id) : base(id)
        {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysDocSet.OfType<Db_WXCallBackMsg>().Single(p => p.Id == id);
                this.fun = d.fun;
                this.key = d.key;
            }
        }

        /// <summary>
        /// 查询被动消息回复数据
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<WeiXinCallBackMsg> query(WxQueryCallBackMsgRequest condtion) {
            BaseResponseList<WeiXinCallBackMsg> result = new BaseResponseList<WeiXinCallBackMsg>();
            using (var db = new SysModelContainer()) {
                var rows = (from c in db.Db_SysDocSet.OfType<Db_WXCallBackMsg>().AsEnumerable()
                            where (!c.isDeleted)
                            && (string.IsNullOrEmpty(condtion.fun) ? true : (c.fun == condtion.fun))
                            && (string.IsNullOrEmpty(condtion.key) ? true : (c.key == condtion.key))
                            && (string.IsNullOrEmpty(condtion.caption) ? true : (c.caption.Contains(condtion.caption)))
                            orderby c.createdOn descending
                            select (c.Id));

                result.total = rows.Count();
                if (result.total > 0) {
                    if (condtion.page != 0) {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new WeiXinCallBackMsg(p)).ToList();
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 被动回复的文本消息
    /// </summary>
    public class WeiXinCallBackTestMsg : WeiXinCallBackMsg {

        /// <summary>
        /// 文本内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WeiXinCallBackTestMsg(string id) : base(id) {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysDocSet.OfType<Db_WXCallBackTextMsg>().Single(p => p.Id == id);
                this.content = d.content;
            }
        }

        /// <summary>
        /// 创建/编辑
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static WeiXinCallBackTestMsg editTextMsg(WxEditCallBackTextMsgReqest condtion) {
            SysValidata.valiData(condtion);
            using (var db = new SysModelContainer()) {

                if (string.IsNullOrEmpty(condtion.Id))
                {
                    condtion.Id = SysHelp.getNewId();
                }
                var c = db.Db_SysDocSet.OfType<Db_WXCallBackTextMsg>().SingleOrDefault(p => p.Id == condtion.Id);
                if (c == null)
                {
                    Db_WXCallBackTextMsg d = new Db_WXCallBackTextMsg()
                    {
                        caption = condtion.caption,
                        content = condtion.content,
                        fun = "text",
                        key = condtion.key,
                        createdBy = HttpContext.Current.User.Identity.Name,
                        createdOn = DateTime.Now,
                        isDeleted = false,
                        modifiedBy = HttpContext.Current.User.Identity.Name,
                        modifiedOn = DateTime.Now,
                        Id = condtion.Id
                    };
                    db.Db_SysDocSet.Add(d);
                }
                else {
                    c.caption = condtion.caption;
                    c.content = condtion.content;
                    c.key = condtion.key;
                    c.modifiedOn = DateTime.Now;
                    c.modifiedBy = HttpContext.Current.User.Identity.Name;
                }
                db.SaveChanges();
                return new WeiXinCallBackTestMsg(condtion.Id);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<WeiXinCallBackTestMsg> queryTextMsg(WxQueryCallBackMsgRequest condtion) {
            BaseResponseList<WeiXinCallBackTestMsg> result = new BaseResponseList<WeiXinCallBackTestMsg>();
            using (var db = new SysModelContainer()) {
                var rows = (from c in db.Db_SysDocSet.OfType<Db_WXCallBackTextMsg>().AsEnumerable()
                            where (!c.isDeleted)
                            && (string.IsNullOrEmpty(condtion.key) ? true : c.key == condtion.key)
                            && (string.IsNullOrEmpty(condtion.caption) ? true : c.caption.Contains(condtion.caption))
                            orderby c.createdOn descending
                            select c.Id
                            );
                result.total = rows.Count();
                if (result.total > 0) {
                    if (condtion.page != 0) {
                        rows = rows.Take(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new WeiXinCallBackTestMsg(p)).ToList();
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 被动回复的图文消息
    /// </summary>
    public class WeiXinCallBackNewsMsg : WeiXinCallBackMsg {

        private List<WeiXinCallBackNewsMsgDetail> _detail = new List<WeiXinCallBackNewsMsgDetail>();

        /// <summary>
        /// 详情
        /// </summary>
        public List<WeiXinCallBackNewsMsgDetail> detail {
            get { return _detail; }
            set { detail = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WeiXinCallBackNewsMsg(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysDocSet.OfType<Db_WXCallBackNesMsg>().Single(p => p.Id == id);
                if (d.Db_WXCallBackNesMsgDetail.Count > 0) {
                    this.detail = d.Db_WXCallBackNesMsgDetail.Select(p => new WeiXinCallBackNewsMsgDetail() {
                        description = p.description,
                        picUrl = p.picUrl,
                        title = p.title,
                        url = p.url
                    }).ToList();
                }
            }
        }

        /// <summary>
        /// 创建/编辑自动回复的图文消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static WeiXinCallBackNewsMsg editNewsMsg(WxEditCallBackNewsMsgRequest condtion) {
            SysValidata.valiData(condtion);
            using (var db = new SysModelContainer()) {
                if (string.IsNullOrEmpty(condtion.Id)) {
                    condtion.Id = SysHelp.getNewId();
                }
                var c = db.Db_SysDocSet.OfType<Db_WXCallBackNesMsg>().SingleOrDefault(p => p.Id == condtion.Id);
                if (c == null)
                {
                    //新增
                    Db_WXCallBackNesMsg d = new Db_WXCallBackNesMsg() {
                        caption = condtion.caption,
                        fun = "news",
                        key = condtion.key,
                        createdBy = HttpContext.Current.User.Identity.Name,
                        createdOn = DateTime.Now,
                        isDeleted = false,
                        modifiedBy = HttpContext.Current.User.Identity.Name,
                        modifiedOn = DateTime.Now,
                        Id = condtion.Id
                    };
                    foreach (var l in condtion.detail) {
                        d.Db_WXCallBackNesMsgDetail.Add(new Db_WXCallBackNesMsgDetail() {
                            description = l.description,
                            picUrl = l.picUrl,
                            title = l.title,
                            url = l.url
                        });
                    }
                    db.Db_SysDocSet.Add(d);
                }
                else {
                    //编辑
                    db.Db_WXCallBackNesMsgDetailSet.RemoveRange(c.Db_WXCallBackNesMsgDetail);
                    db.SaveChanges();

                    c.caption = condtion.caption;
                    c.key = condtion.key;
                    c.modifiedOn = DateTime.Now;
                    c.modifiedBy = HttpContext.Current.User.Identity.Name;
                    foreach (var l in condtion.detail)
                    {
                        c.Db_WXCallBackNesMsgDetail.Add(new Db_WXCallBackNesMsgDetail()
                        {
                            description = l.description,
                            picUrl = l.picUrl,
                            title = l.title,
                            url = l.url
                        });
                    }
                }
                db.SaveChanges();
                return new WeiXinCallBackNewsMsg(condtion.Id);
            }
        }
    }

    /// <summary>
    /// 被动回复的图文消息详情
    /// </summary>
    public class WeiXinCallBackNewsMsgDetail {

        /// <summary>
        /// 图文消息标题
        /// </summary>
        [Required]
        public string title { get; set; }

        /// <summary>
        /// 图文消息描述
        /// </summary>
        [Required]
        public string description { get; set; }

        /// <summary>
        /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        [Required]
        public string picUrl { get; set; }

        /// <summary>
        /// 点击图文消息跳转链接
        /// </summary>
        [Required]
        public string url { get; set; }
    }

    #region -- 素材模型
    /// <summary>
    /// 查询永久素材总数返回对象
    /// </summary>
    public class WxQueryMaterialCountReplay
    {
        /// <summary>
        /// 语音总数量
        /// </summary>
        public int voice_count { get; set; }
        /// <summary>
        /// 视频总数量
        /// </summary>
        public int video_count { get; set; }
        /// <summary>
        /// 图片总数量
        /// </summary>
        public int image_count { get; set; }
        /// <summary>
        /// 图文总数量
        /// </summary>
        public int news_count { get; set; }
    }

    /// <summary>
    /// 素材类型
    /// </summary>
    public enum WxMaterialType {
        /// <summary>
        /// 图文
        /// </summary>
        news,
        /// <summary>
        /// 图片
        /// </summary>
        image,
    }
    /// <summary>
    /// 查询素材请求
    /// </summary>
    public class WxQueryMaterialListRequest {
        /// <summary>
        /// 素材的类型，图片（image）、视频（video）、语音 （voice）、图文（news）
        /// </summary>
        internal string type { get; set; }
        /// <summary>
        /// 从全部素材的该偏移位置开始返回，0表示从第一个素材 返回
        /// </summary>
        public int offset { get; set; }

        /// <summary>
        /// 返回素材的数量，取值在1到20之间
        /// </summary>
        public int count { get; set; }
    }

    public class WxQueryMaterialListBaseReplay {
        public int total_count { get; set; }
        public int item_count { get; set; }

    }

    #endregion

    /// <summary>
    /// 素材管理服务
    /// </summary>
    public class WeiXinMaterialService : WeiXinBase {
        /// <summary>
        /// 查询永久素材的总数
        /// </summary>
        /// <returns></returns>
        public static WxQueryMaterialCountReplay queryCount() {
            string url = geturl("material", "get_materialcount", EnumHttpRequestType.https);
            WxQueryMaterialCountReplay t = new WeiXinGetResponse<WxQueryMaterialCountReplay>(url, "get").getRetrun();
            return t;
        }

        /// <summary>
        /// 查询永久素材
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="type"></param>
        public static void queryMaterial(WxQueryMaterialListRequest condtion, WxMaterialType type) {

        }

        /// <summary>
        /// 上传永久图像素材
        /// </summary>
        public static void uploadFile1()
        {
            string fileurl = "/upload/webpage/160523/swml_160523153853.jpg";
            string path = HttpContext.Current.Server.MapPath(fileurl);

            string url = geturl("material", "add_material", EnumHttpRequestType.https);


            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            int pos = path.LastIndexOf("\\");
            string fileName = path.Substring(pos + 1);

            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] bArr = new byte[fs.Length];
            fs.Read(bArr, 0, bArr.Length);
            fs.Close();

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(bArr, 0, bArr.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();
        }

        /// <summary>
        /// 下载永久素材 - 未实现
        /// </summary>
        /// <param name="mid"></param>
        public static void downloadFile1(string mid) {
            string url = geturl("material", "get_material", EnumHttpRequestType.https);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            Hashtable t = new Hashtable();
            t["media_id"] = mid;
            string d = JsonConvert.SerializeObject(t);
            byte[] buffer = Encoding.UTF8.GetBytes(d);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);

            using (HttpWebResponse wr = request.GetResponse() as HttpWebResponse) {
                string strpath = wr.ResponseUri.ToString();
                WebClient mywebclient = new WebClient();
                string savepath = HttpContext.Current.Server.MapPath("/upload/weixin") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next().ToString().Substring(0, 4) + ".txt";
                try
                {
                    mywebclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    mywebclient.Headers.Add("ContentLength", buffer.Length.ToString());
                    byte[] a = mywebclient.UploadData(url, buffer);
                    string a89324 = Encoding.UTF8.GetString(a);
                    //mywebclient.DownloadFile(strpath, savepath);
                    //string file = savepath;
                }
                catch (Exception ex)
                {
                    savepath = ex.ToString();
                }



            }
        }

        /// <summary>
        /// 上传零时素材
        /// </summary>
        public static void uploadFile() {
            string fileurl = "/upload/webpage/160523/swml_160523153853.jpg";
            string path = HttpContext.Current.Server.MapPath(fileurl);

            string url = geturl("media", "upload", EnumHttpRequestType.https);
            url += "&type=image";

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            int pos = path.LastIndexOf("\\");
            string fileName = path.Substring(pos + 1);

            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] bArr = new byte[fs.Length];
            fs.Read(bArr, 0, bArr.Length);
            fs.Close();

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(bArr, 0, bArr.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();
        }

        /// <summary>
        /// 下载零时素材
        /// </summary>
        /// <param name="mid"></param>
        public static void downloadFile(string mid) {
            string file = string.Empty;
            string content = string.Empty;
            string strpath = string.Empty;
            string savepath = string.Empty;
            string stUrl = geturl("media", "get", EnumHttpRequestType.https);
            stUrl += "&media_id=" + mid;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(stUrl);

            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();

                strpath = myResponse.ResponseUri.ToString();
                WebClient mywebclient = new WebClient();
                savepath = HttpContext.Current.Server.MapPath("/upload/weixin") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next().ToString().Substring(0, 4) + ".jpg";
                try
                {
                    mywebclient.DownloadFile(strpath, savepath);
                    file = savepath;
                }
                catch (Exception ex)
                {
                    savepath = ex.ToString();
                }

            }
        }
    }


    
}