using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewCyclone.Models
{
    /// <summary>
    /// 组合
    /// </summary>
    public class VMComboBox {
        /// <summary>
        /// 键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string text { get; set; }
    }

    /// <summary>
    /// 通用批量编辑请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VMEditListRequest<T>
    {

        private List<T> _rows = new List<T>();
        /// <summary>
        /// 需要更改的行
        /// </summary>
        [Required]
        public List<T> rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
    }
}