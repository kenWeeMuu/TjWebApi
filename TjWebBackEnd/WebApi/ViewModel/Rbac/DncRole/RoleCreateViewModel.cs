﻿using ErpDb.Entitys.Enums;

namespace WebApi.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class RoleCreateViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CommonEnum.Status Status { get; set; }
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 是否是超级管理员(超级管理员拥有系统的所有权限)
        /// </summary>
        public bool IsSuperAdministrator { get; set; }

        /// <summary>
        /// 是否是系统内置角色(系统内置角色不允许删除,修改操作)
        /// </summary>
        public bool IsBuiltin { get; set; }
    }
}