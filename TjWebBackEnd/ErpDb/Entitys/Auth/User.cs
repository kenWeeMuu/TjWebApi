using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ErpDb.Entitys.Enums;

namespace ErpDb.Entitys.Auth
{
    public class User
    {
        /// <summary>
        /// 用户GUID
        /// </summary>
  
        public int UserId { get; set; }

        #region props

        

          
        [Required]
      //  [Column(TypeName = "nvarchar(50)", Order = 10)]
        public string LoginName { get; set; }

      //  [Column(TypeName = "nvarchar(50)")]
        public string DisplayName { get; set; }

     //   [Column(TypeName = "nvarchar(255)")]
        public string Password { get; set; }

     //   [Column(TypeName = "nvarchar(255)", Order = 100)]
        public string Avatar { get; set; }

        public UserType UserType { get; set; }
        public CommonEnum.IsLocked IsLocked { get; set; }

        //[EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }

        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者ID
        /// </summary>
        public   int CreatedByUserId { get; set; }

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
        public   int ModifiedByUserId { get; set; }

        /// <summary>
        /// 最近修改者姓名
        /// </summary>
        public string ModifiedByUserName { get; set; }

        /// <summary>
        /// 用户描述信息
        /// </summary>
    //    [Column(TypeName = "nvarchar(800)")]
        public string Description { get; set; }
        #endregion
        /// <summary>
        /// 用户的角色集合
        /// </summary>
        public ICollection<Role> Roles { get; set; }
    }
}