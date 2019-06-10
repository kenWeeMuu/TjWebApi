/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using System;
using ErpDb.Entitys.Enums;

namespace WebApi.RequestPayload.Rbac.Permission
{
    /// <summary>
    ///
    /// </summary>
    public class PermissionRequestPayload : RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CommonEnum.Status Status { get; set; }

        /// <summary>
        /// 关联菜单GUID
        /// </summary>
        public int MenuId { get; set; }
    }
}