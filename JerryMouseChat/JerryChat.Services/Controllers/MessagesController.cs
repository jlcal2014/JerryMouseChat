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
    using System.IO;
    using System.Threading;

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
                
                this.SendNotification(id);
                return successfulResponse;
            }

            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
        }

        [HttpPost]
        [ActionName("upload")]
        public HttpResponseMessage PostMultipartStream([FromUri]string fileExtension)
        {
            // Verify that this is an HTML Form file upload request
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var contentType = this.Request.Content.GetType();
            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            var task = this.Request.Content.ReadAsMultipartAsync(provider).
            ContinueWith<HttpResponseMessage>(o =>
            {
                var fileInfo = provider.FileData.Select(i =>
                {
                    var info = new FileInfo(i.LocalFileName);
                    return info.FullName;
                });

                var fileFullPath = fileInfo.FirstOrDefault();
                string fileWithExtension = fileFullPath + fileExtension;

                File.Move(fileFullPath, fileWithExtension);
                string drpbxLink = DropboxSaver.DropboxInit(fileWithExtension);

                File.Delete(fileWithExtension);
                var messg = new MessageModel();
                messg.Content = drpbxLink;
                messg.Date = DateTime.Now;
                messg.Author = "batman";
                Post(1, messg);
                var msg1 = this.Request.CreateResponse(HttpStatusCode.NotAcceptable);
                return msg1;
            }
            );
            Thread.Sleep(2000);

            var msg = this.Request.CreateResponse(HttpStatusCode.OK);
            return msg;
        }

        private void SendNotification(int id)
        {
            PubnubAPI pubnub = new PubnubAPI(
               "pub-c-579e5400-3dc9-4ec4-829c-9cd214313647",               // PUBLISH_KEY
                "sub-c-3507fb0e-057f-11e3-991c-02ee2ddab7fe",               // SUBSCRIBE_KEY
                "sec-c-ZjFkY2UxNzUtNTRhYS00OTMxLTkxMzgtZDdhODMwZDE0Zjk5",   // SECRET_KEY
                true                                                        // SSL_ON?
            );

            string channel = "room-" + id;

            // Publish a sample message to Pubnub
            List<object> publishResult = pubnub.Publish(channel, id.ToString());
        }
    }
}
