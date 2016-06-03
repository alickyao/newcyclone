using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace NewCyclone.Models.WeiXin
{

    /*
    * 微信基础与菜单
    */


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
                    if (typeof(WxJsapi_ticket).Equals(t.GetType())) {
                        //如果返回对象是 WxJsapi_ticket
                        SysNotice.createNotice(new VMMsgCreateSysNoticeRequest()
                        {
                            alert = false,
                            message = "服务器已获取了微信网页JSSDK凭证",
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
    /// 查询凭证返回对象 access_token
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

    /// <summary>
    /// 查询网页凭证返回对象 jsapi_ticket
    /// </summary>
    public class WxJsapi_ticket : WxBaseResponse {
        /// <summary>
        /// 凭证
        /// </summary>
        public string ticket { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public int expires_in { get; set; }
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
            get { return System.Configuration.ConfigurationManager.AppSettings["wxAppId"].ToString(); }
        }

        /// <summary>
        /// AppSecret(应用密钥)
        /// </summary>
        protected static string appSecret
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["wxAppSecret"].ToString(); }
        }

        /// <summary>
        /// 公众号原始ID
        /// </summary>
        protected static string originalId {
            get { return System.Configuration.ConfigurationManager.AppSettings["wxOriginalId"].ToString(); }
        }

        /// <summary>
        /// 主机域名/IP访问地址
        /// </summary>
        protected static string hostName {
            get { return System.Configuration.ConfigurationManager.AppSettings["hostName"].ToString(); }
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
        /// 获取凭证 access_token
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

        /// <summary>
        /// 获取网页凭证 jsapi_ticket
        /// </summary>
        /// <returns></returns>
        public static WxJsapi_ticket queryJsApiTicket() {
            string cacheKey = "js_" + appId + appSecret;
            if (HttpRuntime.Cache.Get(cacheKey) != null && HttpRuntime.Cache.Get(cacheKey).ToString().Length > 0)
            {
                return (WxJsapi_ticket)HttpRuntime.Cache.Get(cacheKey);
            }
            string url = geturl("ticket", "getticket", EnumHttpRequestType.https);
            url += "&type=jsapi";
            WxJsapi_ticket result = new WeiXinGetResponse<WxJsapi_ticket>(url, "get").getRetrun();

            //保存到缓存中
            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(result.expires_in), TimeSpan.Zero);
            return result;
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