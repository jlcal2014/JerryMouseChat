using JerryChat.Models;
using JerryChat.Services.Models;
using JerryMouse.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JerryChat.Services.Controllers
{
    public class RoomsController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET api/rooms
        [HttpGet]
        [ActionName("get")]
        public IEnumerable<RoomModel> GetAll()
        {
            var roomsList = this.unitOfWork.RoomsRepository.All();
            var roomsModel = from rooms in roomsList
                             select new RoomModel()
                             {
                                 Name = rooms.Name,
                                 UsersCount = rooms.Users.Count,
                                 IsLocked = rooms.Password != null
                             };
            return roomsModel.ToList();
        }

        // POST api/rooms
        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage PostCreate(RoomModel newRoom)
        {
            if (ModelState.IsValid)
            {
                Room roomToAdd = new Room()
                {
                    Name = newRoom.Name
                };

                unitOfWork.RoomsRepository.Add(roomToAdd);
                unitOfWork.Save();

                HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.Created, roomToAdd);
                successfulResponse.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = roomToAdd.Id }));
                return successfulResponse;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // PUT api/rooms/5
        [HttpPut]
        [ActionName("join")]
        public HttpResponseMessage PutJoin(int id, [FromBody]UserModel username)
        {
            if (ModelState.IsValid)
            {
                Room room = unitOfWork.RoomsRepository.Find(x => x.Id == id).FirstOrDefault();
                User user = unitOfWork.UsersRepository.Find(x => x.Username == username.Username).FirstOrDefault();
                room.Users.Add(user);
                unitOfWork.RoomsRepository.Update(room.Id, room);
                unitOfWork.Save();

                HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.Accepted, room.Id);
                successfulResponse.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = room.Id }));
                return successfulResponse;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // PUT api/rooms/5
        [HttpPut]
        [ActionName("leave")]
        public HttpResponseMessage PutLeave(int id, [FromBody]UserModel username)
        {
            if (ModelState.IsValid)
            {
                Room room = unitOfWork.RoomsRepository.Find(x => x.Id == id).FirstOrDefault();
                User user = unitOfWork.UsersRepository.Find(x => x.Username == username.Username).FirstOrDefault();
                var isInRoom = room.Users.FirstOrDefault(x => x.Id == user.Id);

                room.Users.Remove(user);
                user.Rooms.Remove(room);
                unitOfWork.RoomsRepository.Update(room.Id, room);
                unitOfWork.UsersRepository.Update(user.Id, user);
                unitOfWork.Save();

                HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.Accepted, room.Id);
                successfulResponse.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = room.Id }));
                return successfulResponse;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/rooms/5
        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage Delete(int id)
        {
            var roomToDel = unitOfWork.RoomsRepository.Find(x => x.Id == id).FirstOrDefault();
            if (roomToDel == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                unitOfWork.RoomsRepository.Delete(roomToDel);
                unitOfWork.Save();
                return Request.CreateResponse(HttpStatusCode.OK, roomToDel);
            }
        }
    }
}
