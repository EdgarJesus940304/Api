using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApi.Business.Handlers;
using WebApi.Business.Models;
using WebApi.Business.Utils;

namespace WebApi.Controllers
{
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("api/users/list")]
        public HttpResponseMessage GetUsers([FromBody] FilterDataTableModel filterData)
        {
            using (var userHandler = new UserHandler())
            {
                var response = userHandler.ListUsers(filterData);

                HttpStatusCode statusCode = response.ResponseType == Business.Utils.ResponseType.OK
                         ? HttpStatusCode.OK
                         : HttpStatusCode.InternalServerError;

                return Request.CreateResponse(statusCode, new
                {
                    response.Data
                });
            }
        }

        [HttpGet]
        [Route("api/users/{id}")]
        public HttpResponseMessage GetUser(int id)
        {
            using (var userHandler = new UserHandler())
            {
                var response = userHandler.GetUserById(id);

                HttpStatusCode statusCode = response.ResponseType == Business.Utils.ResponseType.OK
                    ? HttpStatusCode.OK
                    : HttpStatusCode.InternalServerError;

                return Request.CreateResponse(statusCode, new MessageResponse
                {
                    ResponseType = response.ResponseType,
                    Message = response.Message,
                    Data = response.Data
                });
            }
        }

        [HttpPost]
        [Route("api/users")]
        public HttpResponseMessage SaveUser([FromBody] UserModel user)
        {
            using (var userHandler = new UserHandler())
            {
                var response = userHandler.CreateUser(user);

                HttpStatusCode statusCode = response.ResponseType == Business.Utils.ResponseType.OK
                    ? HttpStatusCode.OK
                    : HttpStatusCode.InternalServerError;

                return Request.CreateResponse(statusCode, new MessageResponse
                {
                    ResponseType = response.ResponseType,
                    Message = response.Message
                });
            }
        }

        [HttpPut]
        [Route("api/users/{id}")]
        public HttpResponseMessage UpdateUser(int id, [FromBody] UserModel user)
        {
            using (var userHandler = new UserHandler())
            {
                var response = userHandler.ModifyUser(id, user);

                HttpStatusCode statusCode = response.ResponseType == Business.Utils.ResponseType.OK
                   ? HttpStatusCode.OK
                   : HttpStatusCode.InternalServerError;

                return Request.CreateResponse(statusCode, new MessageResponse
                {
                    ResponseType = response.ResponseType,
                    Message = response.Message
                });
            }
        }

        [HttpDelete]
        [Route("api/users/{id}")]
        public HttpResponseMessage DeleteUser(int id)
        {
            using (var userHandler = new UserHandler())
            {
                var response = userHandler.RemoveUser(id);

                HttpStatusCode statusCode = response.ResponseType == Business.Utils.ResponseType.OK
                   ? HttpStatusCode.OK
                   : HttpStatusCode.InternalServerError;

                return Request.CreateResponse(statusCode, new MessageResponse
                {
                    ResponseType = response.ResponseType,
                    Message = response.Message
                });
            }
        }


        [HttpPost]
        [Route("api/users/login")]
        public HttpResponseMessage Login([FromBody] UserModel user)
        {
            using (var userHandler = new UserHandler())
            {
                var response = userHandler.Login(user);

                HttpStatusCode statusCode = response.ResponseType == Business.Utils.ResponseType.OK
                    ? HttpStatusCode.OK
                    : response.ResponseType == Business.Utils.ResponseType.Warning 
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError;

                return Request.CreateResponse(statusCode, new MessageResponse
                {
                    ResponseType = response.ResponseType,
                    Message = response.Message,
                    Data = response.Data
                });
            }
        }
    }
}