using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ErpDb.Entitys.Enums;

namespace ErpDb.Entitys.Auth
{
    public class Permission
    {
        /// <summary>
        ///
        /// </summary>
        public Permission() {
            Roles = new HashSet<Role>();
        }


        public int PermissionId { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        [Required]
      //  [Column(TypeName = "nvarchar(20)")]
        public string Code { get; set; }



        public Guid MenuGuid { get; set; }



        /// <summary>
        /// 权限名称
        /// </summary>
        [Required]
      //  [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        [Required]
    //    [Column(TypeName = "nvarchar(80)")]
        public string ActionCode { get; set; }

        /// <summary>
        /// 图标(可选)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
     //   [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CommonEnum.Status Status { get; set; }

        /// <summary>
        /// 是否已删
        /// </summary>
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 权限类型(0:菜单,1:按钮/操作/功能等)
        /// </summary>
        public CommonEnum.PermissionType Type { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

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
        public int ModifiedByUserId { get; set; }

        /// <summary>
        /// 最近修改者姓名
        /// </summary>
        public string ModifiedByUserName { get; set; }


        // Foreign key
        /// <summary>
        /// 菜单GUID
        /// </summary>
        public int MenuId { get; set; }
        // Navigation properties
        /// <summary>
        /// 关联的菜单
        /// </summary>
        public virtual Menu Menu { get; set; }

        // Navigation property
        /// <summary>
        /// 权限所属的角色集合
        /// </summary>
        public ICollection<Role> Roles { get; set; }
    }
}