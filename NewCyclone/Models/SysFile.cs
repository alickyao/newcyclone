using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewCyclone.DataBase;

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
    }

    /// <summary>
    /// 可排序的文件
    /// </summary>
    public class FileSort :SysFile {

        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        /// 使用ID构造可排序的文件
        /// </summary>
        /// <param name="id"></param>
        public FileSort(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileSort>().Single(p => p.Id == id);
                this.sort = d.sort;
            }
        }
    }

    /// <summary>
    /// 带描述的文件
    /// </summary>
    public class FileInfo : FileSort
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
        public FileInfo(string id) : base(id) {
            using (var db = new SysModelContainer()) {
                var d = db.Db_SysFileSet.OfType<Db_FileInfo>().Single(p => p.Id == id);
                this.title = d.title;
                this.describe = d.describe;
            }
        }
    }

    /// <summary>
    /// 创建文件记录请求
    /// </summary>
    public class VMCreateFileRequest {

        /// <summary>
        /// 文件名
        /// </summary>
        protected string fileName { get; set; }
        /// <summary>
        /// 文件存放的路径
        /// </summary>
        protected string filePath { get; set; }

        /// <summary>
        /// 文件存放的位置
        /// </summary>
        protected string url { get; set; }

        /// <summary>
        /// 传入文件的url构造请求参数
        /// </summary>
        /// <param name="url">文件存放的路径</param>
        public VMCreateFileRequest(string url) {

        }
        /// <summary>
        /// 传入文件名称与路径构造函数
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="filePath">文件存放的路径</param>
        public VMCreateFileRequest(string fileName, string filePath) {

        }
    }
}