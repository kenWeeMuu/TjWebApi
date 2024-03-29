﻿using System;
using ErpDb.Entitys.Enums;

namespace WebApi.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class MenuJsonModel
    {
        /// <summary>
        ///
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 页面别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 菜单图标(可选)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 父级GUID
        /// </summary>
        public string ParentGuid { get; set; }

        /// <summary>
        /// 上级菜单名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 菜单层级深度
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否可用(0:禁用,1:可用)
        /// </summary>
        public CommonEnum.Status Status { get; set; }

        /// <summary>
        /// 是否为默认路由
        /// </summary>
        public CommonEnum.YesOrNo IsDefaultRouter { get; set; }

        /// <summary>
        /// 是否已删
        /// </summary>
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedOn { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatedByUserGuid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedByUserName { get; set; }

        /// <summary>
        /// 最近修改时间
        /// </summary>
        public string ModifiedOn { get; set; }

        /// <summary>
        /// 最近修改者ID
        /// </summary>
        public Guid? ModifiedByUserGuid { get; set; }

        /// <summary>
        /// 最近修改者姓名
        /// </summary>
        public string ModifiedByUserName { get; set; }

        /// <summary>
        /// 前端组件(.vue)
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 在菜单中隐藏
        /// </summary>
        public CommonEnum.YesOrNo HideInMenu { get; set; }

        /// <summary>
        /// 不缓存页面
        /// </summary>
        public CommonEnum.YesOrNo NotCache { get; set; }

        /// <summary>
        /// 页面关闭前的回调函数
        /// </summary>
        public string BeforeCloseFun { get; set; }
    }
}