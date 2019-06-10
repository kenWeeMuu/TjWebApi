using ErpDb.Entitys.Enums;

namespace WebApi.RequestPayload.Rbac.Role
{
    /// <summary>
    ///
    /// </summary>
    public class RoleRequestPayload : RequestPayload
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