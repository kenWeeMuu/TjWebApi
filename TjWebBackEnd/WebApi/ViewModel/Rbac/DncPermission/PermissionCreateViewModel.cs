﻿using System;
using ErpDb.Entitys.Enums;

namespace WebApi.ViewModel
{
    /// <summary>
    /// 权限实体类
    /// </summary>
    public class PermissionCreateViewModel
    {
        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 菜单GUID
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限操作码
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// 图标(可选)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
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
    }
}