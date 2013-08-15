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
    public class MessagesController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
          

        // GET api/messages
        [HttpGet]
        [ActionName("get")]
        public IEnumerable<MessageModel> GetAllMessages(int id)
        {
            var room = this.unitOfWork.RoomsRepository.Find(x => x.Id == id).FirstOrDefault();
            if (room != null)
            {
                var roomMessages = this.unitOfWork.MessagesRepository.All().Where(x => x.Room.Id == id).ToList();
                var messages = from message in roomMessages
                               select new MessageModel()
                               {
                                   Date = message.Date,
                                   Content = message.Content,
                                   Author = message.Author.Username
                               };

                return messages.ToList();
            }
            else
            {
                throw new HttpRequestException(HttpStatusCode.BadRequest.ToString());
            }
        }

        // POST api/messages
        [HttpPost]
        [ActionName("send")]
        public HttpResponseMessage Post(int id, [FromBody]MessageModel message)
        {
            if (ModelState.IsValid)
            {
                Message messageToPush = new Message()
                {
                    Author = unitOfWork.UsersRepository.Find(x => x.Username == message.Author).FirstOrDefault(),
                    Content = message.Content,
                    Date = message.Date,
                    Room = unitOfWork.RoomsRepository.Find(x => x.Id == id).FirstOrDefault()
                };

                unitOfWork.MessagesRepository.Add(messageToPush);
                unitOfWork.Save();

                MessageModel resultModel = new MessageModel()
                {
                    Date = messageToPush.Date,
                    Author = messageToPush.Author.Username,
                    Content = messageToPush.Content
                };

                HttpResponseMessage successfulResponse = Request.CreateResponse<MessageModel>(HttpStatusCode.Created, resultModel);
                successfulResponse.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = messageToPush.Id }));
                return successfulResponse;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }
}
