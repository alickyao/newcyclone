using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;
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
        /// 使用ID构造带描述的文件
        /// </summary>
        /// <param name="id"></param>
        public SysFileInfo(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileInfo>().Single(p => p.Id == id);
                this.title = d.title;
                this.describe = d.describe;
            }
        }

        /// <summary>
        /// 通过创建请求构造对象
        /// </summary>
        /// <param name="condtion"></param>
        public SysFileInfo(VMCreateFileInfoRequest condtion) : base(condtion) {
            this.title = condtion.title;
            this.describe = condtion.describe;
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
                    title = this.title
                };
                Db_SysFileSet newrow = db.Db_SysFileSet.Add(d);
                db.SaveChanges();
                return new SysFileInfo(newrow.Id);
            }
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
    }
}