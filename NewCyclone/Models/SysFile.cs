﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace NewCyclone.Models
{
    /// <summary>
    /// 文件 - 基础文件
    /// </summary>
    public class SysFile
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 文件存放路径
        /// </summary>
        public string filePath { get; set; }

        /// <summary>
        /// 访问url  路径+文件名
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public SysUser createdBy { get; set; }

        /// <summary>
        /// 使用ID构造文件信息
        /// </summary>
        /// <param name="id"></param>
        public SysFile(string id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.Single(p => p.Id == id);
                this.Id = d.Id;
                this.createdOn = d.createdOn;
                this.createdBy = new SysUser(d.createdBy);
                this.fileName = d.fileName;
                this.filePath = d.filePath;
                this.url = this.filePath + this.fileName;
            }
        }
        /// <summary>
        /// 通过URL构造一个文件信息
        /// </summary>
        public SysFile(VMCreateFileRequest condtion) {
            //根据condtion.url构造filePath与fileName
            SysValidata.valiData(condtion);
            int s =condtion.url.LastIndexOf('/');
            if (s == -1) {
                throw new SysException("输入的文件位置不正确，争取的格式为:/filepath/filename.xxx");
            }
            this.filePath = condtion.url.Substring(0, s + 1);
            this.fileName = condtion.url.Substring(s + 1);
            this.url = condtion.url;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public void delete() {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.Single(p => p.Id == this.Id);
                db.Db_SysFileSet.Remove(d);
                db.SaveChanges();
            }

            try
            {
                deleteFile(this.url);
                //同时删除缩略图
                string dir = this.url.Substring(0, this.url.LastIndexOf('.'));
                DirectoryInfo d = new DirectoryInfo(HttpContext.Current.Server.MapPath(dir));
                if (d.Exists)
                {
                    dir = dir.Replace("/upload/", "");
                    deleteDirs(dir);
                }
            }
            catch { }
        }

        /// <summary>
        /// 列出上传目录下的所有文档
        /// </summary>
        /// <param name="path"></param>
        public static VMDiskFileQueryResponse listFiles(string path = "") {
            string rootUrl = "/upload/" + (string.IsNullOrEmpty(path) ? "" : path + "/");
            string rootPath = HttpContext.Current.Server.MapPath(rootUrl);
            string [] dirlist = Directory.GetDirectories(rootPath);
            string[] fileslist = Directory.GetFiles(rootPath);
            VMDiskFileQueryResponse result = new VMDiskFileQueryResponse();

            string[] imgFilesType = { ".jpg", ".png", ".jpeg", ".bmp", ".gif" };
            result.path = (string.IsNullOrEmpty(path) ? "" : path + "/");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.IndexOf('/') > 0)
                {
                    result.prvUrl = path.Substring(0, path.LastIndexOf('/'));
                }
                else {
                    result.prvUrl = "";
                }
            }
            else {
                result.prvUrl = "root";
            }

            foreach (string d in dirlist) {
                DirectoryInfo dir = new DirectoryInfo(d);
                result.rows.Add(new VMDiskFileInfo() {
                    fileSize = 0,
                    hasFile = (dir.GetFileSystemInfos().Length > 0),
                    isDir = true,
                    lastWriteTime = dir.LastWriteTime.ToUniversalTime().AddHours(8),
                    name = dir.Name,
                    url = rootUrl+ dir.Name,
                    fileType = string.Empty,
                    isImg = false
                });
            }

            foreach (string f in fileslist) {
                FileInfo file = new FileInfo(f);
                result.rows.Add(new VMDiskFileInfo()
                {
                    fileSize = file.Length,
                    hasFile = false,
                    isDir = false,
                    lastWriteTime = file.LastWriteTime.ToUniversalTime().ToUniversalTime().AddHours(8),
                    name = file.Name,
                    url = rootUrl + file.Name,
                    fileType = file.Extension,
                    isImg = imgFilesType.Contains(file.Extension)
                });
            }
            result.total = result.rows.Count();
            result.rows.Sort();
            return result;
        }

        /// <summary>
        /// 从磁盘中删除文件
        /// </summary>
        /// <param name="path">文件的路径/path/file.ext</param>
        public static void deleteFile(string path) {
            if (string.IsNullOrEmpty(path)) {
                throw new SysException("路径不能为空", path);
            }
            string p = HttpContext.Current.Server.MapPath(path);
            FileInfo file = new FileInfo(p);
            if (file.Exists)
            {
                file.Delete();
            }
            else {
                throw new SysException("文件不存在", path);
            }
        }

        /// <summary>
        /// 从磁盘中删除文件(批量)
        /// </summary>
        /// <param name="path">路径集合</param>
        public static void deleteFile(List<string> path) {
            if (path == null) {
                throw new SysException("请传入需要删除的文件的路径参数集合");
            }
            if (path.Count == 0) {
                throw new SysException("需要删除的文件的路径参数集合为空");
            }
            foreach (string p in path) {
                deleteFile(p);
            }
        }

        /// <summary>
        /// 从磁盘中删除文件夹以及其子文件夹和文件
        /// </summary>
        /// <param name="path"></param>
        public static void deleteDirs(string path) {
            VMDiskFileQueryResponse list = listFiles(path);
            if (list.total > 0) {
                foreach (var l in list.rows) {
                    if (l.isDir)
                    {
                        if (l.hasFile)
                        {
                            deleteDirs(path + "/" + l.name);
                        }
                        else {
                            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(l.url));
                            if (dir.Exists) {
                                dir.Delete();
                            }
                        }
                    }
                    else {
                        FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(l.url));
                        if (file.Exists) {
                            file.Delete();
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                DirectoryInfo rootDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("/upload/" + path));
                if (rootDir.Exists) {
                    rootDir.Delete();
                }
            }
        }

        /// <summary>
        /// 根据文件存放的路径获取缩略图地址
        /// 该方法不会抛出异常，如果遇到异常会返回字符串空值
        /// </summary>
        /// <param name="width">宽（单位：像素）</param>
        /// <param name="height">高（单位：像素）</param>
        /// <param name="url">源文件的路径</param>
        /// <returns>缩略图的地址</returns>
        public static string getThumbnail(int width, int height, string url) {
            string thumbnail = string.Empty;
            BaseResponse result = new BaseResponse();
            try
            {
                SysFile file = new SysFile(new VMCreateFileRequest()
                {
                    url = url
                });
                thumbnail = file.getThumbnail(width, height);
            }
            catch (SysException e) {
                result = e.getresult(result, true);
            }
            catch (Exception e) {
                result = SysException.getResult(result, e, string.Format("width:{0},height{1},url:{2}", width, height, url));
            }
            return thumbnail;
        }
        /// <summary>
        /// 获取文件的缩略图
        /// </summary>
        /// <param name="width">宽（单位：像素）</param>
        /// <param name="height">高（单位：像素）</param>
        /// <returns>缩略图的地址</returns>
        public string getThumbnail(int width, int height)
        {
            if (width <= 0 || height <= 0) {
                throw new SysException("指定的宽度和高度不能小于或等于0", string.Format("width:{0},height:{1}", width, height));
            }
            string sourceFilePath = HttpContext.Current.Server.MapPath(this.url);
            FileInfo sourceFile = new FileInfo(sourceFilePath);
            if (!sourceFile.Exists) {
                throw new SysException("源文件不存在", this);
            }
            string[] imgType = ".gif,.jpg,.jpeg,.png,.bmp".Split(',').ToArray();
            if (!imgType.Contains(sourceFile.Extension)) {
                throw new SysException("只有图片文件才能生成缩略图");
            }
            if (this.url.IndexOf("thumbnail") != -1) {
                throw new SysException("传入的文件已经是缩略图地址了，不能再生成缩略图");
            }
            string thumbnailDir = string.Format("{0}/thumbnail/{1}_{2}", this.url.Substring(0, this.url.LastIndexOf('.')), width, height);
            string thumbnailFile = thumbnailDir + "/" + this.fileName;
            string thumbnailDirPath = HttpContext.Current.Server.MapPath(thumbnailDir);
            string thumbnailFilePath = HttpContext.Current.Server.MapPath(thumbnailFile);
            //判断目录是否存在
            DirectoryInfo dir = new DirectoryInfo(thumbnailDirPath);
            if (!dir.Exists) {
                Directory.CreateDirectory(thumbnailDirPath);
            }
            FileInfo file = new FileInfo(thumbnailFilePath);
            if (!file.Exists)
            {
                //创建缩略图
                makeThumbnail(sourceFilePath, thumbnailFilePath, width, height);
            }
            return thumbnailFile;
        }

        /// <summary>
        /// 获取缩略图并保存
        /// </summary>
        /// <param name="sourcePath">原地址</param>
        /// <param name="newPath">目标地址</param>
        /// <param name="width">宽 像素</param>
        /// <param name="height">高 像素</param>
        private static void makeThumbnail(string sourcePath, string newPath, int width, int height)
        {
            System.Drawing.Image ig = System.Drawing.Image.FromFile(sourcePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = ig.Width;
            int oh = ig.Height;
            if ((double)ig.Width / (double)ig.Height > (double)towidth / (double)toheight)
            {
                oh = ig.Height;
                ow = ig.Height * towidth / toheight;
                y = 0;
                x = (ig.Width - ow) / 2;

            }
            else
            {
                ow = ig.Width;
                oh = ig.Width * height / towidth;
                x = 0;
                y = (ig.Height - oh) / 2;
            }
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawImage(ig, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ig.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }

        }

    }

    /// <summary>
    /// 可排序的文件
    /// </summary>
    public class SysFileSort :SysFile,IComparable {

        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        /// 使用ID构造可排序的文件
        /// </summary>
        /// <param name="id"></param>
        public SysFileSort(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileSort>().Single(p => p.Id == id);
                this.sort = d.sort;
            }
        }

        /// <summary>
        /// 通过创建请求构造对象
        /// </summary>
        /// <param name="condtion"></param>
        public SysFileSort(VMCreateFileSortRequest condtion):base(condtion) {
            this.sort = condtion.sort;
        }

        /// <summary>
        /// 创建一个可排序的文件记录
        /// </summary>
        /// <returns></returns>
        public SysFileSort create() {
            using (var db = new SysModelContainer()) {
                Db_FileSort d = new Db_FileSort()
                {
                    createdOn = DateTime.Now,
                    createdBy = HttpContext.Current.User.Identity.Name,
                    fileName = this.fileName,
                    filePath = this.filePath,
                    Id = Guid.NewGuid().ToString(),
                    sort = this.sort
                };
                Db_SysFileSet newrow = db.Db_SysFileSet.Add(d);
                db.SaveChanges();
                return new SysFileSort(newrow.Id);
            }
        }

        /// <summary>
        /// 编辑排序
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static List<SysFileSort> editSort(VMEditListRequest<VMEditFileSortRequest> condtion) {
            SysValidata.valiData(condtion);

            List<SysFileSort> result = new List<SysFileSort>();
            foreach (var row in condtion.rows)
            {
                var file = new SysFileSort(row.fileId);
                file.sort = row.sort;
                file.saveSort();
                result.Add(file);
            }
            return result;
        }

        /// <summary>
        /// 保存排序
        /// </summary>
        private void saveSort()
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileSort>().Single(p => p.Id == this.Id);
                d.sort = this.sort;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 比较 用于排序
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (typeof(SysFileSort).IsAssignableFrom(obj.GetType())) {
                SysFileSort t = (SysFileSort)obj;
                if (this.sort > t.sort)
                {
                    return 1;
                }
                else if (this.sort < t.sort)
                {
                    return -1;
                }
                else {
                    return 0;
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// 带描述的文件
    /// </summary>
    public class SysFileInfo : SysFileSort
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// 使用ID构造带描述的文件
        /// </summary>
        /// <param name="id"></param>
        public SysFileInfo(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileInfo>().Single(p => p.Id == id);
                this.title = d.title;
                this.describe = d.describe;
                this.link = d.link;
            }
        }

        /// <summary>
        /// 通过创建请求构造对象
        /// </summary>
        /// <param name="condtion"></param>
        public SysFileInfo(VMCreateFileInfoRequest condtion) : base(condtion) {
            this.title = condtion.title;
            this.describe = condtion.describe;
            this.link = condtion.link;
        }

        /// <summary>
        /// 创建一个带描述的文件
        /// </summary>
        /// <returns></returns>
        public new SysFileInfo create() {
            using (var db = new SysModelContainer()) {
                Db_FileInfo d = new Db_FileInfo()
                {
                    createdBy = HttpContext.Current.User.Identity.Name,
                    createdOn = DateTime.Now,
                    describe = this.describe,
                    fileName = this.fileName,
                    filePath = this.filePath,
                    Id = Guid.NewGuid().ToString(),
                    sort = this.sort,
                    title = this.title,
                    link = this.link
                };
                Db_SysFileSet newrow = db.Db_SysFileSet.Add(d);
                db.SaveChanges();
                return new SysFileInfo(newrow.Id);
            }
        }

        /// <summary>
        /// 修改带描述的文件信息（包括排序）
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static List<SysFileInfo> editInfo(VMEditListRequest<VMEditFileInfoRequest> condtion) {
            SysValidata.valiData(condtion);
            List<SysFileInfo> result = new List<SysFileInfo>();
            foreach (var file in condtion.rows) {
                SysFileInfo info = new SysFileInfo(file.fileId);
                info.sort = file.sort;
                info.link = file.link;
                info.title = file.title;
                info.describe = file.describe;
                info.saveInfo();
                result.Add(info);
            }
            return result;
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        private void saveInfo()
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileInfo>().Single(p => p.Id == this.Id);
                d.sort = this.sort;
                d.link = this.link;
                d.title = this.title;
                d.describe = this.describe;
                db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// 文件查询返回对象
    /// </summary>
    public class VMDiskFileQueryResponse {

        /// <summary>
        /// 总计
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 当前请求的path
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 可返回的Path 当值为root时表示当前已为根目录
        /// </summary>
        public string prvUrl { get; set; }

        private List<VMDiskFileInfo> _rows = new List<VMDiskFileInfo>();
        /// <summary>
        /// 行
        /// </summary>
        public List<VMDiskFileInfo> rows {
            get { return _rows; }
            set { _rows = value; }
        }
    }

    /// <summary>
    /// 系统文件信息
    /// </summary>
    public class VMDiskFileInfo:IComparable {
        /// <summary>
        /// 是否目录
        /// </summary>
        public bool isDir { get; set; }
        /// <summary>
        /// 如果是目录该目录下是否有文件
        /// </summary>
        public bool hasFile { get; set; }
        /// <summary>
        /// 文件夹或文件的名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string fileType { get; set; }

        /// <summary>
        /// 是否为图像文件
        /// </summary>
        public bool isImg { get; set; }

        /// <summary>
        /// 访问路径
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 如果是文件则表示文件的大小
        /// </summary>
        public long fileSize { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime lastWriteTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (typeof(VMDiskFileInfo).Equals(obj.GetType())) {
                VMDiskFileInfo o = (VMDiskFileInfo)obj;
                return -this.lastWriteTime.CompareTo(o.lastWriteTime);
            }
            return 0;
        }
    }


    /// <summary>
    /// 创建文件记录请求
    /// </summary>
    public class VMCreateFileRequest {

        /// <summary>
        /// 文件存放的位置
        /// </summary>
        [Required]
        public string url { get; set; }
    }

    /// <summary>
    /// 创建可排序的文件请求参数
    /// </summary>
    public class VMCreateFileSortRequest : VMCreateFileRequest {
        /// <summary>
        /// 排序号
        /// </summary>
        public int sort { get; set; }
    }

    /// <summary>
    /// 创建带标题与描述的文件请求
    /// </summary>
    public class VMCreateFileInfoRequest : VMCreateFileSortRequest { 
        
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string link { get; set; }
    }

    /// <summary>
    /// 编辑可排序文件排序请求
    /// </summary>
    public class VMEditFileSortRequest {
        /// <summary>
        /// 文件ID
        /// </summary>
        [Required]
        [StringLength(36,MinimumLength =36)]
        public string fileId { get; set; }
        /// <summary>
        /// 文件排序码
        /// </summary>
        public int sort { get; set; }
    }

    /// <summary>
    /// 编辑可描述的文件请求
    /// </summary>
    public class VMEditFileInfoRequest : VMEditFileSortRequest {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string link { get; set; }
    }
}