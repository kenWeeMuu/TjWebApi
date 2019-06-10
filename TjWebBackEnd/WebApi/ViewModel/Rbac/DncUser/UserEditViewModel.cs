using System;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;

namespace WebApi.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class UserEditViewModel
    {
        /// <summary>
        ///
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public CommonEnum.IsLocked IsLocked { get; set; }

        /// <summary>
        ///
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 用户描述信息
        /// </summary>
        public string Description { get; set; }
    }
}