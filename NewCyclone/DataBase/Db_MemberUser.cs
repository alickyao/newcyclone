//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewCyclone.DataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Db_MemberUser : Db_SysUser
    {
        public string nickName { get; set; }
        public string openId { get; set; }
        public byte sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headImgUrl { get; set; }
    }
}
