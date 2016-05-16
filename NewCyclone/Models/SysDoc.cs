﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewCyclone.Models
{
    /// <summary>
    /// 系统基础文档
    /// </summary>
    public class SysDoc
    {

        /// <summary>
        /// 文档ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public SysUser createdBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public SysUser modifiedBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modifiedOn { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string caption { get; set; }

        /// <summary>
        /// 是否已被标记为删除
        /// </summary>s
        public bool isDeleted { get; set; }


        private List<SysCatTree> _cat = new List<SysCatTree>();

        /// <summary>
        /// 所属在类别
        /// </summary>
        public List<SysCatTree> cat {
            get { return _cat; }
            set { _cat = value; }
        }


        /// <summary>
        /// 使用ID构造文档基础项
        /// </summary>
        /// <param name="id"></param>
        public SysDoc(string id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysDocSet.Single(p => p.Id == id);
                this.Id = d.Id;
                this.createdBy = new SysUser(d.createdBy);
                this.createdOn = d.createdOn;
                this.modifiedBy = new SysUser(d.modifiedBy);
                this.modifiedOn = d.modifiedOn;

                this.caption = d.caption;
                this.isDeleted = d.isDeleted;

                this.cat = d.Db_DocCat.Select(p => new SysCatTree(p.Db_CatTreeId, false)).ToList();
            }
        }

        /// <summary>
        /// 删除（标记删除）
        /// </summary>
        public virtual void delete() {
            using (var db = new SysModelContainer())
            {
                var d = db.Db_SysDocSet.Single(p => p.Id == this.Id);
                d.isDeleted = true;
                db.SaveChanges();
                SysUserLog.saveLog("删除文档:" + this.caption, SysUserLogType.删除, this.Id);
            }
        }

        /// <summary>
        /// 记录到文件关联表
        /// </summary>
        /// <param name="file">文件</param>
        public void savefiles(SysFile file) {
            using (var db = new SysModelContainer()) {
                Db_SysDocFile f = new Db_SysDocFile()
                {
                    Db_SysFileId = file.Id,
                    Db_SysDocId = this.Id
                };
                db.Db_SysDocFileSet.Add(f);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="files">需要删除的文件ID集合</param>
        /// <returns>被删除的记录总数</returns>
        public static int delfiles(VMEditListRequest<string> files) {
            SysValidata.valiData(files);
            int i = 0;
            using (var db = new SysModelContainer()) {
                var delrow = (from x in db.Db_SysDocFileSet where files.rows.Contains(x.Db_SysFileId) select x);
                db.Db_SysDocFileSet.RemoveRange(delrow);
                foreach (string file in files.rows) {
                    SysFile f = new SysFile(file);
                    f.delete();
                    i++;
                }
                db.SaveChanges();
            }
            return i;
        }
    }

    /// <summary>
    /// 网站文档
    /// </summary>
    public class WebDoc : SysDoc
    {
        /// <summary>
        /// 所属的功能模块描述
        /// </summary>
        public VMComboBox fun { get; set; }
        /// <summary>
        /// 文档描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// 网站功能组件列表
        /// </summary>
        protected List<VMComboBox> funlist = SysHelp.getSysSetList<List<VMComboBox>>("FunWebCms.xml");

        /// <summary>
        /// 展示的创建时间
        /// </summary>
        public DateTime showTime { get; set; }

        /// <summary>
        /// 使用ID构造一个网站文档
        /// </summary>
        /// <param name="id"></param>
        public WebDoc(string id) : base(id)
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysDocSet.OfType<Db_DocWeb>().Single(p => p.Id == id);
                this.fun = funlist.Single(p => p.id == d.fun);
                this.describe = d.describe;
                this.showTime = d.showTime;
            }
        }

        /// <summary>
        /// 检索网页文档
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<WebDoc> searchWebDoc(VMSearchWebDocReqeust condtion) {
            BaseResponseList<WebDoc> result = new BaseResponseList<WebDoc>();
            using (var db = new SysModelContainer()) {
                var list = (from c in db.Db_SysDocSet.OfType<Db_DocWeb>().AsEnumerable()
                            where (string.IsNullOrEmpty(condtion.q) ? true : (c.caption.Contains(condtion.q) || (c.describe == null ? false : c.describe.Contains(condtion.q))))
                            && (condtion.catTreeIds.Count == 0 ? true : (condtion.catTreeIds.Intersect(c.Db_DocCat.Select(p => p.Db_CatTreeId)).Count() > 0 ? true : false))
                            && (string.IsNullOrEmpty(condtion.fun) ? true : c.fun == condtion.fun)
                            && (!c.isDeleted)
                            orderby c.showTime descending
                            select c.Id
                            );
                result.total = list.Count();
                if (result.total > 0) {
                    if (condtion.page > 0)
                    {
                        list = list.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = list.Select(p => new WebDoc(p)).ToList();
                }
            }
            return result;
        }


        /// <summary>
        /// 新增可排序的图片集
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public List<SysFileSort> createFilesSort(List<VMCreateFileSortRequest> condtion)
        {
            List<SysFileSort> result = new List<SysFileSort>();
            foreach (var f in condtion) {
                SysFileSort s = new SysFileSort(f);
                SysFileSort newrow = s.create();
                result.Add(newrow);
                savefiles(newrow);
            }
            return result;
        }

        /// <summary>
        /// 新增带描述的图片集
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public List<SysFileInfo> createFilesInfo(List<VMCreateFileInfoRequest> condtion) {
            List<SysFileInfo> result = new List<SysFileInfo>();
            foreach (var f in condtion)
            {
                SysFileInfo s = new SysFileInfo(f);
                SysFileInfo newrow = s.create();
                result.Add(newrow);
                savefiles(newrow);
            }
            return result;
        }
    }

    /// <summary>
    /// 网页页面（图文信息）
    /// </summary>
    public class WebDocPage : WebDoc {

        /// <summary>
        /// seo标题
        /// </summary>
        public string seoTitle { get; set; }

        /// <summary>
        /// seo关键字
        /// </summary>
        public string seoKeyWords { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        private List<SysFileSort> _files = new List<SysFileSort>();

        /// <summary>
        /// 图片集
        /// </summary>
        public List<SysFileSort> files {
            get { return _files; }
            set { _files = value; }
        }

        /// <summary>
        /// 使用ID构造网页页面文档（图文信息）
        /// </summary>
        /// <param name="id"></param>
        public WebDocPage(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysDocSet.OfType<Db_WebPage>().Single(p => p.Id == id);
                this.seoTitle = d.seoTitle;
                this.seoKeyWords = d.seoKeyWords;
                this.content = d.content;

                //图片集
                if (d.Db_DocFile.Count > 0) {
                    foreach (var f in d.Db_DocFile) {
                        this.files.Add(new SysFileSort(f.Db_SysFileId));
                    }
                    this.files.Sort();
                }
            }
        }

        /// <summary>
        /// 创建/编辑图文文档
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static WebDocPage edit(VMEditWebDocPageRequest condtion) {
            SysValidata.valiData(condtion);

            if (condtion.catTreeIds.Count == 0) {
                throw new SysException("所在分类必选", condtion);
            }

            using (var db = new SysModelContainer()) {

                if (string.IsNullOrEmpty(condtion.Id)) {
                    condtion.Id = SysHelp.getNewId();
                }
                var c = db.Db_SysDocSet.OfType<Db_WebPage>().SingleOrDefault(p => p.Id == condtion.Id);
                if (c == null)
                {
                    //新增
                    Db_WebPage d = new Db_WebPage()
                    {
                        caption = condtion.caption,
                        content = condtion.content,
                        createdBy = HttpContext.Current.User.Identity.Name,
                        createdOn = DateTime.Now,
                        describe = condtion.describe,
                        fun = condtion.fun,
                        Id = condtion.Id,
                        isDeleted = false,
                        modifiedBy = HttpContext.Current.User.Identity.Name,
                        modifiedOn = DateTime.Now,
                        seoKeyWords = condtion.seoKeyWords,
                        seoTitle = condtion.seoTitle,
                        showTime = condtion.showTime
                    };

                    //分类
                    foreach (string cat in condtion.catTreeIds) {
                        d.Db_DocCat.Add(new Db_SysDocCat()
                        {
                            Db_CatTreeId = cat
                        });
                    }
                    var newrow = db.Db_SysDocSet.Add(d);
                    db.SaveChanges();

                    SysUserLog.saveLog(condtion, SysUserLogType.编辑, newrow.Id);
                    return new WebDocPage(newrow.Id);
                }
                else {
                    //编辑

                    var d = db.Db_SysDocSet.OfType<Db_WebPage>().Single(p => p.Id == condtion.Id);

                    //删除原来的的分类
                    db.Db_SysDocCatSet.RemoveRange(d.Db_DocCat);
                    db.SaveChanges();


                    d.caption = condtion.caption;
                    d.content = condtion.content;
                    d.modifiedOn = DateTime.Now;
                    d.modifiedBy = HttpContext.Current.User.Identity.Name;
                    d.fun = condtion.fun;
                    d.seoKeyWords = condtion.seoKeyWords;
                    d.seoTitle = condtion.seoTitle;
                    d.showTime = condtion.showTime;
                    d.describe = condtion.describe;

                    //分类
                    foreach (string cat in condtion.catTreeIds)
                    {
                        d.Db_DocCat.Add(new Db_SysDocCat()
                        {
                            Db_CatTreeId = cat
                        });
                    }

                    db.SaveChanges();

                    return new WebDocPage(condtion.Id);
                }
            }
        }

        
    }

    /// <summary>
    /// 创建/编辑网站图文文档请求
    /// </summary>
    public class VMEditWebDocPageRequest:ItoSysLogMesable {

        /// <summary>
        /// 主键  新建时 调用 SysHelp.getNewId() 或不填
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public string caption { get; set; }
        /// <summary>
        /// 模块的ID
        /// </summary>
        [Required]
        public string fun { get; set; }

        private DateTime _showTime = DateTime.Now;

        /// <summary>
        /// 展示的创建时间 默认为系统当前时间
        /// </summary>
        public DateTime showTime {
            get { return _showTime; }
            set { _showTime = value; }
        }

        /// <summary>
        /// 文档描述
        /// </summary>
        public string describe { get; set; }

        /// <summary>
        /// seo标题
        /// </summary>
        public string seoTitle { get; set; }

        /// <summary>
        /// seo关键字
        /// </summary>
        public string seoKeyWords { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        private List<string> _catTreeIds = new List<string>();

        /// <summary>
        /// 所在分类
        /// </summary>
        [Required]
        public List<string> catTreeIds {
            get { return _catTreeIds; }
            set { _catTreeIds = value; }
        }

        /// <summary>
        /// 生成日志文本
        /// </summary>
        /// <returns></returns>
        public string toLogString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("新增/编辑图文消息:").Append(this.caption);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 检索网页文档请求参数
    /// </summary>
    public class VMSearchWebDocReqeust : BaseRequest {
        /// <summary>
        /// 关键字 模糊匹配文档的标题,描述
        /// </summary>
        public string q { get; set; }

        private List<string> _catTreeIds = new List<string>();

        /// <summary>
        /// 分类
        /// </summary>
        public List<string> catTreeIds {
            get { return _catTreeIds; }
            set { _catTreeIds = value; }
        }

        /// <summary>
        /// 功能ID
        /// </summary>
        public string fun { get; set; }
    }

    /// <summary>
    /// 为文档追加可排序的图片请求
    /// </summary>
    public class VMAppendWebDocFilesSortRequest {
        /// <summary>
        /// 文档的ID
        /// </summary>
        [Required]
        public string docId { get; set; }

        private List<VMCreateFileSortRequest> _rows = new List<VMCreateFileSortRequest>();

        /// <summary>
        /// 可排序的文件列表
        /// </summary>
        [Required]
        public List<VMCreateFileSortRequest> rows {
            get { return _rows; }
            set { _rows = value; }
        }
    }

    /// <summary>
    /// 为文档追加带描述的图片请求
    /// </summary>
    public class VMAppendWebDocFilesInfoRequest
    {
        /// <summary>
        /// 文档的ID
        /// </summary>
        [Required]
        public string docId { get; set; }

        private List<VMCreateFileInfoRequest> _rows = new List<VMCreateFileInfoRequest>();

        /// <summary>
        /// 可排序的文件列表
        /// </summary>
        [Required]
        public List<VMCreateFileInfoRequest> rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
    }
}