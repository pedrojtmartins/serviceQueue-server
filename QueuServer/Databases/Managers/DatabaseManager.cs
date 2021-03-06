﻿using QueuServer.Databases.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QueuServer.Managers
{
    class DatabaseManager
    {
        private queue_dbEntities1 dbContext;
        private static DatabaseManager databaseManager;

        public static DatabaseManager getInstance()
        {
            if (databaseManager == null)
                databaseManager = new DatabaseManager();

            return databaseManager;
        }

        private DatabaseManager()
        {
            dbContext = new queue_dbEntities1();
        }

        public ticket AddNewTicket(int iType)
        {
            var newTicket = new ticket()
            {
                number = GetNextNumberForType(iType),
                type = iType,
                date_start = DateTime.Now
            };

            dbContext.tickets.Add(newTicket);
            var res = dbContext.SaveChanges();

            if (res == 1)
                return newTicket;

            return null;
        }

        public int SetTicketAsComplete(int iId, int clientId)
        {
            if (iId == -1) return 0;

            var ticket = GetPendingById(iId);
            if (ticket == null)
                throw new Exception();

            ticket.date_end = DateTime.Now;
            ticket.clientId = clientId;

            return dbContext.SaveChanges();
        }

        public List<ticket> GetPendingList(int count)
        {
            return (from p in dbContext.tickets where p.clientId == null select p).Take(count).ToList();
        }

        public ticket GetNextTicket()
        {
            return (from p in dbContext.tickets where p.clientId == null orderby p.type descending, p.id ascending select p).Take(1).SingleOrDefault();
        }

        public int SetTicketForClient(int ticketId, int clientId)
        {
            ticket t = (from p in dbContext.tickets where p.id == ticketId select p).SingleOrDefault();
            if (t == null)
                return -1;

            t.clientId = clientId;
            return dbContext.SaveChanges();
        }

        private ticket GetPendingById(int id)
        {
            return (from p in dbContext.tickets where p.id == id select p).First();
        }

        private int GetNextNumberForType(int type)
        {
            var t = (from p in dbContext.tickets where p.type == type && p.date_start >= DateTime.Today orderby p.id descending select p).FirstOrDefault();
            if (t == null)
                return 1;

            return t.number + 1;
        }
    }
}
