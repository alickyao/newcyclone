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
    
    public partial class Db_SysDoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Db_SysDoc()
        {
            this.Db_DocCat = new HashSet<Db_SysDocCat>();
            this.Db_DocFile = new HashSet<Db_SysDocFile>();
        }
    
        public string Id { get; set; }
        public System.DateTime createdOn { get; set; }
        public string createdBy { get; set; }
        public System.DateTime modifiedOn { get; set; }
        public string modifiedBy { get; set; }
        public bool isDeleted { get; set; }
        public string caption { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Db_SysDocCat> Db_DocCat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Db_SysDocFile> Db_DocFile { get; set; }
    }
}
