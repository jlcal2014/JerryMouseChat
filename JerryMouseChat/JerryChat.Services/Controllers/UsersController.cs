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
        [HttpGet]
        [ActionName("all")]
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

        [HttpGet]
        [ActionName("online")]
        public IEnumerable<UserModel> GetOnlineUsers()
        {
            var userEntities = this.unitOfWork.UsersRepository.All().Where(us => us.Sessions.Count > 0);
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
        [HttpGet]
        [ActionName("single")]
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

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser(UserRegister user)
        {
            User userEntity = this.unitOfWork.UsersRepository.Find(us => us.Username == user.Username
                                                                    && us.Password == user.Password).FirstOrDefault();
            if (userEntity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            string sessionKey = this.unitOfWork.SessionsRepository.LoginUser(userEntity);
            this.unitOfWork.Save();

            UserRegister registeredUser = new UserRegister()
            {
                Id = userEntity.Id,
                AvatarUrl = userEntity.AvatarUrl,
                Username = userEntity.Username,
                SessionKey = sessionKey
            };

            var response = Request.CreateResponse<UserRegister>(HttpStatusCode.OK, registeredUser);
            return response;
        }

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserRegister user)
        {
            User userEntity = this.unitOfWork.UsersRepository.Find(us => us.Username == user.Username).FirstOrDefault();
            if (userEntity != null)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            userEntity = new User()
            {
                Username = user.Username,
                Password = user.Password,
                AvatarUrl = user.AvatarUrl
            };

            this.unitOfWork.UsersRepository.Add(userEntity);
            this.unitOfWork.Save();

            string sessionKey = this.unitOfWork.SessionsRepository.LoginUser(userEntity);
            this.unitOfWork.Save();

            UserRegister registeredUser = new UserRegister()
            {
                Id = userEntity.Id,
                AvatarUrl = userEntity.AvatarUrl,
                Username = userEntity.Username,
                SessionKey = sessionKey
            };

            var response = Request.CreateResponse<UserRegister>(HttpStatusCode.OK, registeredUser);
            return response;
        }

    }
}