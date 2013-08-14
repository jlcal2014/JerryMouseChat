using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using JerryChat.Models;
using JerryChat.Data;
using JerryMouse.Repositories;
using JerryChat.Services.Models;

namespace JerryChat.Services.Controllers
{
    public class UsersController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET api/Users
        public IEnumerable<UserModel> GetUsers()
        {
            var userEntities = this.unitOfWork.UsersRepository.All();
            var userModels =
                                from usEntity in userEntities
                                select new UserModel()
                                {
                                    Id = usEntity.Id,
                                    Username = usEntity.Username
                                };
            return userModels.ToList();
        }

        // GET api/Users/5
        public UserDetails GetUser(int id)
        {
            var entity = this.unitOfWork.UsersRepository.Get(id);
            var model = new UserDetails()
            {
                Username = entity.Username,
                AvatarUrl = entity.AvatarUrl,
                Messages = (from msg in entity.Messages
                            select new MessageModel()
                            {
                                Date = msg.Date,
                                Content = msg.Content
                            }).ToList(),
                Rooms = (from r in entity.Rooms
                         select new RoomModel()
                         {
                             Name = r.Name
                         }).ToList()
            };

            return model;
        }

        // POST api/Users
        public HttpResponseMessage PostStudent([FromBody]UserRegister user)
        {
            var entityToAdd = new User()
            {
                Username = user.Username,
                Password = user.Password,
                AvatarUrl = user.AvatarUrl
            };

            var createdEntity = this.unitOfWork.UsersRepository.Add(entityToAdd);
            this.unitOfWork.Save();

            var createdModel = new UserRegister()
            {
                Id = createdEntity.Id,
                Username = createdEntity.Username,
                Password = createdEntity.Password
            };
            
            var response = Request.CreateResponse<UserRegister>(HttpStatusCode.Created, createdModel);
            var resourceLink = Url.Link("DefaultApi", new { id = createdModel.Id });

            response.Headers.Location = new Uri(resourceLink);
            return response;
        }
    }
}