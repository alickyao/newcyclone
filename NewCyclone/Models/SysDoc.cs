using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

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
        /// 使用ID构造一个网站文档
        /// </summary>
        /// <param name="id"></param>
        public WebDoc(string id) : base(id)
        {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysDocSet.OfType<Db_DocWeb>().Single(p => p.Id == id);
                this.fun = funlist.Single(p => p.id == d.fun);
                this.describe = d.describe;
            }
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
            }
        }

        /// <summary>
        /// 创建/编辑图文文档
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static WebDocPage edit(VMEditWebDocPageRequest condtion) {
            SysValidata.valiData(condtion);
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
                        seoTitle = condtion.seoTitle
                    };
                    var newrow = db.Db_SysDocSet.Add(d);
                    db.SaveChanges();
                    return new WebDocPage(newrow.Id);
                }
                else {
                    //编辑
                    return new WebDocPage(condtion.Id);
                }
            }
        }
    }

    /// <summary>
    /// 创建/编辑网站图文文档请求
    /// </summary>
    public class VMEditWebDocPageRequest {

        /// <summary>
        /// 主键
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

        /// <summary>
        /// 所在分类
        /// </summary>
        public HashSet<string> catTreeIds { get; set; }
    }
}