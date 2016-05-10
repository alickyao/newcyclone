using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Web;
using System.Xml.Linq;

namespace NewCyclone.Models
{
    /// <summary>
    /// 系统菜单项
    /// </summary>
    public class SysMenu 
    {
        /// <summary>
        /// 系统菜单
        /// </summary>
        public static List<SysMenu> sysMenu;

        static SysMenu() {
            //构造系统菜单

            #region -- 初始化xml文档
            //sysMenu = new List<SysMenu>() {
            //    new SysMenu() {
            //        roles = new List<string>(){ "admin","user"},
            //        text = "业务",
            //        icon = "icon-edit",
            //        url = "",
            //        children = new List<SysMenu>() {
            //            new SysMenu() {
            //                text = "会员",
            //                children = new List<SysMenu> {
            //                    new SysMenu() {
            //                        text = "会员管理",
            //                        url = ""
            //                    }
            //                }
            //            }
            //        }
            //    },
            //    new SysMenu() {
            //        roles= new List<string>() { "admin","user"},
            //        text = "资源",
            //        children = new List<SysMenu>() {
            //            new SysMenu() {
            //                text = "分类管理"
            //            }
            //        }
            //    },
            //    new SysMenu() {
            //        roles = new List<string>() { "admin"},
            //        text = "管理",
            //        children = new List<SysMenu>() {
            //            new SysMenu() {
            //                text = "系统管理员维护",
            //            },
            //            new SysMenu() {
            //                text="系统日志查看"
            //            }
            //        }
            //    }
            //};
            #endregion

            //序列化为xml对象

            string path = HttpContext.Current.Server.MapPath("/App_Set/SysMenu.xml");
            //var writer = new XmlSerializer(typeof(List<SysMenu>));
            //var wfile = new StreamWriter(path);
            //writer.Serialize(wfile, sysMenu);
            //wfile.Close();

            //从文件反序列化
            XmlSerializer reader = new XmlSerializer(typeof(List<SysMenu>));
            StreamReader file = new StreamReader(path);
            try
            {
                sysMenu = (List<SysMenu>)reader.Deserialize(file);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                file.Close();
            }
        }

        /// <summary>
        /// 菜单标题
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 可访问的角色
        /// </summary>
        public List<string> roles { get; set; }

        /// <summary>
        /// 对应的easyui图片class名
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 对应的链接
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<SysMenu> children { get; set; }
    }
}