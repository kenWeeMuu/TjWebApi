using System;
using System.Web.Http;
using WebApi.Extensions;

namespace WebApi.Controllers.Base
{
    [Route("api/v1/[controller]/[action]")]
   // [ApiAuthorize]
    public class MessageController : ApiController
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/message/count")]
        public IHttpActionResult Count() {
            return Ok(1);
        }

        /// <summary>
        /// 初始化消息标题列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Init() {
            var response = ResponseModelFactory.CreateInstance;
            var unread = new object[] {
                new {title="消息1",create_time=DateTime.Now,msg_id=1}
            };
            response.SetData(new { unread });
            return Ok(response);
        }

        /// <summary>
        /// 获取指定ID的消息内容
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Content(int msgid) {
            var response = ResponseModelFactory.CreateInstance;

            response.SetData($"消息[{msgid}]内容");
            return Ok(response);
        }

        /// <summary>
        /// 将消息标为已读
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/message/has_read/{msgid}")]
        [HttpGet]
        public IHttpActionResult HasRead(int msgid) {
            var response = ResponseModelFactory.CreateInstance;
            return Ok(response);
        }

        /// <summary>
        /// 删除已读消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/message/remove_readed/{msgid}")]
        public IHttpActionResult RemoveRead(int msgid) {
            var response = ResponseModelFactory.CreateInstance;
            return Ok(response);
        }

        /// <summary>
        /// 恢复已删消息
        /// </summary>
        /// <returns></returns>
        [HttpGet ]
        [Route("api/v1/message/restore/{msgid}")]
        public IHttpActionResult Restore(int msgid) {
            var response = ResponseModelFactory.CreateInstance;
            return Ok(response);
        }
    }
}