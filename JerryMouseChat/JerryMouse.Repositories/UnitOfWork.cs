using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JerryChat.Data;
using JerryChat.Models;

namespace JerryMouse.Repositories
{
    public class UnitOfWork
    {
        private JerryChatContext context = new JerryChatContext();
        private EfRepository<User> usersRepository;
        private EfRepository<Message> messagesRepository;
        private EfRepository<Room> roomsRepository;

        public EfRepository<User> UsersRepository
        {
            get
            {
                if (this.usersRepository == null)
                {
                    this.usersRepository = new EfRepository<User>(this.context);
                }

                return this.usersRepository;
            }
        }

        public EfRepository<Message> MessagesRepository
        {
            get
            {
                if (this.messagesRepository == null)
                {
                    this.messagesRepository = new EfRepository<Message>(this.context);
                }

                return this.messagesRepository;
            }
        }

        public EfRepository<Room> RoomsRepository
        {
            get
            {
                if (this.roomsRepository == null)
                {
                    this.roomsRepository = new EfRepository<Room>(this.context);
                }

                return this.roomsRepository;
            }
        }

        public void Save()
        {
            this.context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
