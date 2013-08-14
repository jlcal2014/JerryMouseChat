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
        private UsersRepository usersRepository;
        private MessagesRepository messagesRepository;
        private RoomsRepository roomsRepository;
        private SessionsRepository sessionsRepository;

        public UsersRepository UsersRepository
        {
            get
            {
                if (this.usersRepository == null)
                {
                    this.usersRepository = new UsersRepository(this.context);
                }

                return this.usersRepository;
            }
        }

        public MessagesRepository MessagesRepository
        {
            get
            {
                if (this.messagesRepository == null)
                {
                    this.messagesRepository = new MessagesRepository(this.context);
                }

                return this.messagesRepository;
            }
        }

        public RoomsRepository RoomsRepository
        {
            get
            {
                if (this.roomsRepository == null)
                {
                    this.roomsRepository = new RoomsRepository(this.context);
                }

                return this.roomsRepository;
            }
        }

        public SessionsRepository SessionsRepository
        {
            get
            {
                if (this.sessionsRepository == null)
                {
                    this.sessionsRepository = new SessionsRepository(this.context);
                }

                return this.sessionsRepository;
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
