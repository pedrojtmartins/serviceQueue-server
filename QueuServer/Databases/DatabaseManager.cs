using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int AddNewTicket(int iType)
        {
            dbContext.tickets.Add(new ticket()
            {
                number = GetNextNumberForType(iType),
                type = iType,
                date_start = DateTime.Now
            });
            return dbContext.SaveChanges();
        }

        public int SetTicketAsComplete(int iId, int clientId)
        {
            var ticket = GetPendingById(iId);
            if (ticket == null)
                throw new Exception();

            ticket.date_end = DateTime.Now;
            ticket.clientId = clientId;

            return dbContext.SaveChanges();
        }

        public List<ticket> GetPendingList(int count)
        {
            return (from p in dbContext.tickets where p.date_end == null select p).Take(count).ToList();
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
