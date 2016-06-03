using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace NewCyclone.Models.WeiXin
{
    /*
    * 微信消息接收与回复服务 
    */


    #region -- 消息模型


    /// <summary>
    /// 基础消息
    /// </summary>
    [Serializable]
    public class WxBaseMsg
    {
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
    public class WxReceiveMsg : WxBaseMsg
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get; set; }

        /// <summary>
        /// 消息返回
        /// </summary>
        /// <returns></returns>
        public virtual string returnMsg()
        {
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
            return base.returnMsg();
        }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class WxPicMsg : WxReceiveMsg
    {
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
            return base.returnMsg();
        }
    }
    /// <summary>
    /// 语音消息
    /// </summary>
    public class WxVoiceMsg : WxReceiveMsg
    {
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
            return base.returnMsg();
        }
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    public class WxVideoMsg : WxReceiveMsg
    {
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
            return base.returnMsg();
        }
    }

    /// <summary>
    /// 小视频消息
    /// </summary>
    public class WxShortvideoMsg : WxReceiveMsg
    {
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
            return base.returnMsg();
        }
    }

    /// <summary>
    /// 地理位置消息
    /// </summary>
    public class WxLocationMsg : WxReceiveMsg
    {
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
            return base.returnMsg();
        }
    }

    /// <summary>
    /// 链接消息
    /// </summary>
    public class WxLinkMsg : WxReceiveMsg
    {
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
            return base.returnMsg();
        }
    }

    /// <summary>
    /// 事件推送
    /// </summary>
    public class WxEventMsg : WxReceiveMsg
    {
        /// <summary>
        /// 事件类型 subscribe 关注，LOCATION  CLICK  VIEW
        /// </summary>
        public string Event { get; set; }
    }

    /// <summary>
    /// 关注事件
    /// </summary>
    public class WxEventSubscribeMsg : WxEventMsg
    {
        /// <summary>
        /// 关注事件为字符串空值
        /// </summary>
        public string EventKey { get; set; }

        public override string returnMsg()
        {
            var l = WeiXinCallBackMsg.query(new WxQueryCallBackMsgRequest()
            {
                key = "subscribe"
            });
            if (l.total > 0)
            {
                var info = l.rows[0];
                if (info.fun == "text")
                {
                    WeiXinCallBackTextMsg msg = new WeiXinCallBackTextMsg(info.Id);
                    return WeiXinMsgService.getTextMsg(msg, this.FromUserName);
                }
                if (info.fun == "news")
                {
                    WeiXinCallBackNewsMsg msg = new WeiXinCallBackNewsMsg(info.Id);
                    return WeiXinMsgService.getNewsMsg(msg, this.FromUserName);
                }
            }
            return base.returnMsg();
        }
    }
    /// <summary>
    /// 菜单点击事件
    /// </summary>
    public class WxEventClickMsg : WxEventMsg
    {
        /// <summary>
        /// 事件KEY值，与自定义菜单接口中KEY值对应
        /// </summary>
        public string EventKey { get; set; }

        public override string returnMsg()
        {
            if (EventKey.IndexOf("alert&") != -1)
            {
                SysNotice.createNotice(new VMMsgCreateSysNoticeRequest()
                {
                    alert = true,
                    message = "微信用户点击事件提醒，Key=" + this.EventKey,
                    title = "微信接口"
                });
            }
            var l = WeiXinCallBackMsg.query(new WxQueryCallBackMsgRequest()
            {
                key = this.EventKey
            });
            if (l.total > 0)
            {
                var info = l.rows[0];
                if (info.fun == "text")
                {
                    WeiXinCallBackTextMsg msg = new WeiXinCallBackTextMsg(info.Id);
                    return WeiXinMsgService.getTextMsg(msg, this.FromUserName);
                }
                if (info.fun == "news")
                {
                    WeiXinCallBackNewsMsg msg = new WeiXinCallBackNewsMsg(info.Id);
                    return WeiXinMsgService.getNewsMsg(msg, this.FromUserName);
                }
            }
            return base.returnMsg();
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

        /// <summary>
        /// 
        /// </summary>
        public string MenuId { get; set; }
    }

    /// <summary>
    /// 上报地理位置事件
    /// 用户同意上报地理位置后，每次进入公众号会话时，都会在进入时上报地理位置，或在进入会话后每5秒上报一次地理位置，公众号可以在公众平台网站中修改以上设置。上报地理位置时，微信会将上报地理位置事件推送到开发者填写的URL。
    /// </summary>
    public class WxEventLatitudeMsg : WxEventMsg
    {
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
    public class WxReturnPicMsgDetail
    {
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
    public class WeiXinMsgService : WeiXinBase
    {

        /// <summary>
        /// 返序列化微信推送的XML格式消息
        /// </summary>
        /// <param name="postString">微信推送的XML数据</param>
        public static WxReceiveMsg deserializePostString(string postString)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(postString);
            XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
            if (MsgType != null)
            {
                WxReceiveMsg t = new WxReceiveMsg();
                switch (MsgType.InnerText)
                {
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
                        if (EventType != null)
                        {
                            switch (EventType.InnerText)
                            {
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
        /// 获取一个被动回复的文本消息
        /// </summary>
        /// <param name="content">文本消息内容</param>
        /// <param name="toUserOpenId">接收者的OpenId</param>
        /// <returns>XML格式的字符串</returns>
        public static string getTextMsg(string content, string toUserOpenId)
        {
            StringBuilder sb = new StringBuilder("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", toUserOpenId);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", originalId);
            sb.AppendFormat("<CreateTime>{0}</CreateTime>", SysHelp.convertDateTimeInt(DateTime.Now));
            sb.Append("<MsgType><![CDATA[text]]></MsgType>");
            sb.AppendFormat("<Content><![CDATA[{0}]]></Content>", content);
            sb.Append("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取一个被动回复的文本消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="toUserOpenId"></param>
        /// <returns></returns>
        public static string getTextMsg(WeiXinCallBackTextMsg content, string toUserOpenId)
        {
            return getTextMsg(content.content, toUserOpenId);
        }

        /// <summary>
        /// 获取一个被动回复的图文消息（多个图文集合）
        /// </summary>
        /// <param name="news"></param>
        /// <param name="toUserOpenId"></param>
        /// <returns></returns>
        public static string getNewsMsg(List<WxReturnPicMsgDetail> news, string toUserOpenId)
        {
            if (news.Count == 0)
            {
                throw new SysException("图文集长度不能为0", news);
            }
            StringBuilder sb = new StringBuilder("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", toUserOpenId);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", originalId);
            sb.AppendFormat("<CreateTime>{0}</CreateTime>", SysHelp.convertDateTimeInt(DateTime.Now));
            sb.Append("<MsgType><![CDATA[news]]></MsgType>");
            sb.AppendFormat("<ArticleCount>{0}</ArticleCount>", news.Count);
            sb.Append("<Articles>");
            foreach (var n in news)
            {
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
        /// 获取一个被动回复的图文消息（从后台维护的图文列表中生成）
        /// </summary>
        /// <param name="news"></param>
        /// <param name="toUserOpenId"></param>
        /// <returns></returns>
        public static string getNewsMsg(WeiXinCallBackNewsMsg news, string toUserOpenId)
        {
            if (news.files.Count == 0)
            {
                return "";
            }
            else {
                List<WxReturnPicMsgDetail> nlist = new List<WxReturnPicMsgDetail>();
                int i = 0;
                foreach (var f in news.files)
                {
                    string picUrl = hostName;
                    if (i == 0)
                    {
                        //头图微信要求360X200像素
                        picUrl += SysFile.getThumbnail(360, 200, f.url);
                    }
                    else {
                        //其他  200X200像素
                        picUrl += SysFile.getThumbnail(200, 200, f.url);
                    }
                    i++;
                    nlist.Add(new WxReturnPicMsgDetail()
                    {
                        Description = f.describe,
                        PicUrl = hostName + f.url,
                        Title = f.title,
                        Url = f.link
                    });
                }
                return getNewsMsg(nlist, toUserOpenId);
            }
        }
    }

    #region -- 被动消息回复模型

    /// <summary>
    /// 被动消息编辑基础请求参数
    /// </summary>
    public class WxEditCallBackMsgRequst
    {
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
    public class WxEditCallBackTextMsgReqest : WxEditCallBackMsgRequst
    {

        /// <summary>
        /// 文本内容
        /// </summary>
        [Required]
        public string content { get; set; }
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
        public static BaseResponseList<WeiXinCallBackMsg> query(WxQueryCallBackMsgRequest condtion)
        {
            BaseResponseList<WeiXinCallBackMsg> result = new BaseResponseList<WeiXinCallBackMsg>();
            using (var db = new SysModelContainer())
            {
                var rows = (from c in db.Db_SysDocSet.OfType<Db_WXCallBackMsg>().AsEnumerable()
                            where (!c.isDeleted)
                            && (string.IsNullOrEmpty(condtion.fun) ? true : (c.fun == condtion.fun))
                            && (string.IsNullOrEmpty(condtion.key) ? true : (c.key == condtion.key))
                            && (string.IsNullOrEmpty(condtion.caption) ? true : (c.caption.Contains(condtion.caption)))
                            orderby c.createdOn descending
                            select (c.Id));

                result.total = rows.Count();
                if (result.total > 0)
                {
                    if (condtion.page != 0)
                    {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new WeiXinCallBackMsg(p)).ToList();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取key出现的次数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="id">需要排除的id 编辑状态时排除自己</param>
        /// <returns></returns>
        public static int getKeyCount(string key, string id = null)
        {
            using (var db = new SysModelContainer())
            {
                int count = (from c in db.Db_SysDocSet.OfType<Db_WXCallBackMsg>().AsEnumerable()
                             where (!c.isDeleted)
                             && (c.key == key)
                             && (string.IsNullOrEmpty(id) ? true : (c.Id != id))
                             select c.Id).Count();
                return count;
            }
        }
    }

    /// <summary>
    /// 被动回复的文本消息
    /// </summary>
    public class WeiXinCallBackTextMsg : WeiXinCallBackMsg
    {

        /// <summary>
        /// 文本内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WeiXinCallBackTextMsg(string id) : base(id)
        {
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
        public static WeiXinCallBackTextMsg editTextMsg(WxEditCallBackTextMsgReqest condtion)
        {
            SysValidata.valiData(condtion);
            if (getKeyCount(condtion.key, condtion.Id) > 0)
            {
                throw new SysException("key已被使用");
            }
            using (var db = new SysModelContainer())
            {

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
                return new WeiXinCallBackTextMsg(condtion.Id);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<WeiXinCallBackTextMsg> queryTextMsg(WxQueryCallBackMsgRequest condtion)
        {
            BaseResponseList<WeiXinCallBackTextMsg> result = new BaseResponseList<WeiXinCallBackTextMsg>();
            using (var db = new SysModelContainer())
            {
                var rows = (from c in db.Db_SysDocSet.OfType<Db_WXCallBackTextMsg>().AsEnumerable()
                            where (!c.isDeleted)
                            && (string.IsNullOrEmpty(condtion.key) ? true : c.key == condtion.key)
                            && (string.IsNullOrEmpty(condtion.caption) ? true : c.caption.Contains(condtion.caption))
                            orderby c.createdOn descending
                            select c.Id
                            );
                result.total = rows.Count();
                if (result.total > 0)
                {
                    if (condtion.page != 0)
                    {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.Select(p => new WeiXinCallBackTextMsg(p)).ToList();
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 被动回复的图文消息
    /// </summary>
    public class WeiXinCallBackNewsMsg : WeiXinCallBackMsg
    {

        private List<SysFileInfo> _files = new List<SysFileInfo>();

        /// <summary>
        /// 图文详情
        /// </summary>
        public List<SysFileInfo> files
        {
            get { return _files; }
            set { _files = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WeiXinCallBackNewsMsg(string id) : base(id)
        {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysDocSet.OfType<Db_WXCallBackNesMsg>().Single(p => p.Id == id);
                if (d.Db_DocFile.Count > 0)
                {
                    this.files = d.Db_DocFile.Select(p => new SysFileInfo(p.Db_SysFileId)).ToList();
                }
            }
        }

        /// <summary>
        /// 创建/编辑自动回复的图文消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static WeiXinCallBackNewsMsg editNewsMsg(WxEditCallBackMsgRequst condtion)
        {
            SysValidata.valiData(condtion);
            if (getKeyCount(condtion.key, condtion.Id) > 0)
            {
                throw new SysException("key已被使用");
            }
            using (var db = new SysModelContainer())
            {
                if (string.IsNullOrEmpty(condtion.Id))
                {
                    condtion.Id = SysHelp.getNewId();
                }
                var c = db.Db_SysDocSet.OfType<Db_WXCallBackNesMsg>().SingleOrDefault(p => p.Id == condtion.Id);
                if (c == null)
                {
                    //新增
                    Db_WXCallBackNesMsg d = new Db_WXCallBackNesMsg()
                    {
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
                    db.Db_SysDocSet.Add(d);
                }
                else {
                    //编辑
                    c.caption = condtion.caption;
                    c.key = condtion.key;
                    c.modifiedOn = DateTime.Now;
                    c.modifiedBy = HttpContext.Current.User.Identity.Name;
                }
                db.SaveChanges();
                return new WeiXinCallBackNewsMsg(condtion.Id);
            }
        }
    }
}