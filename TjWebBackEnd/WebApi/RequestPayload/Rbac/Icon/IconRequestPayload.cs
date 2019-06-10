using ErpDb.Entitys.Enums;

namespace WebApi.RequestPayload.Rbac.Icon
{
    /// <summary>
    /// 图标请求参数实体
    /// </summary>
    public class IconRequestPayload : RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public CommonEnum.IsDeleted IsDeleted { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CommonEnum.Status Status { get; set; }
    }
}