using System;
using ErpDb.Entitys.Auth;

namespace WebApi.QueryModels.DncPermission
{
    /// <summary>
    ///
    /// </summary>
    public class PermissionWithAssignProperty
    {
        public PermissionWithAssignProperty() { }

        public PermissionWithAssignProperty(Permission p, Role r)
        {
            RoleCode = r == null ? "" : r.Code;
            IsAssigned = r == null ? 0 : 1;
            ActionCode = p.ActionCode;
            Code = p.Code;
            MenuId = p.MenuId;
            Name = p.Name;
        }
        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限关联的菜单GUID
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 权限是否已分配到当前角色
        /// </summary>
        public int IsAssigned { get; set; }
    }
}