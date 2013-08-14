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
        public IEnumerable<RoomModel> GetAll()
        {
            var roomsList = this.unitOfWork.RoomsRepository.All();
            var roomsModel = from rooms in roomsList
                             select new RoomModel()
                             {
                                 Name = rooms.Name,
                             };
            return roomsModel.ToList();
        }

        //// GET api/rooms/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/rooms
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
        public HttpResponseMessage Put([FromUri]int roomId, [FromBody]string username)
        {
            if (ModelState.IsValid)
            {
                Room room = unitOfWork.RoomsRepository.Find(x => x.Id == roomId).FirstOrDefault();
                User user = unitOfWork.UsersRepository.Find(x => x.Username == username).FirstOrDefault();
                room.Users.Add(user);
                unitOfWork.RoomsRepository.Update(room.Id, room);
                unitOfWork.Save();

                HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.Accepted, room.Users);
                successfulResponse.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = user.Id }));
                return successfulResponse;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/rooms/5
        public void Delete(int id)
        {
        }
    }
}
