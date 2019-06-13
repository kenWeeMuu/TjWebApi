using System;
using ErpDb.Entitys.Enums;

namespace WebApi.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class IconJsonModel
    {
        /// <summary>
        ///
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 图标名称
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 图标的大小，单位是 px
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 图标颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 自定义图标
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        ///
        /// </summary>
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
        ///
        /// </summary>
        public string CreatedOn { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CreatedByUserName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ModifiedOn { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int ModifiedByUserId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ModifiedByUserName { get; set; }
    }
}