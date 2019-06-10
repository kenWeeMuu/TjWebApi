using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ErpDb.Entitys.Enums;

namespace ErpDb.Entitys.Auth
{
    public class Role
    {
        /// <summary>
        ///
        /// </summary>
        public Role() {
            Users = new HashSet<User>();
           Permissions = new HashSet<Permission>();
        }


        public int RoleId { get; set; }

        #region props

        

      
        [Required]
  
      //  [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Required]
     //   [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
      //  [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }

        /// <summary>
        ///
        /// </summary>
        public CommonEnum.Status Status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedByUserName { get; set; }

        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// 最近修改者ID
        /// </summary>
        public int ModifiedByUserId { get; set; } = default(int);

        /// <summary>
        /// 最近修改者姓名
        /// </summary>
        public string ModifiedByUserName { get; set; }

        /// <summary>
        /// 是否是超级管理员(超级管理员拥有系统的所有权限)
        /// </summary>
        public bool IsSuperAdministrator { get; set; }

        /// <summary>
        /// 是否是系统内置角色(系统内置角色不允许删除,修改操作)
        /// </summary>
        public bool IsBuiltin { get; set; }
        #endregion
        /// <summary>
        /// 角色拥有的用户集合
        /// </summary>
        public ICollection<User> Users { get; set; }

        /// <summary>
        /// 角色拥有的权限集合
        /// </summary>
        public ICollection<Permission> Permissions { get; set; }
    }
}