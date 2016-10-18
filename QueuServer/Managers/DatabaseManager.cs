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

        public ticket AddNewTicket(int iNumber, int iType)
        {
            return dbContext.tickets.Add(new ticket()
            {
                number = iNumber,
                type = iType,
                date_start = DateTime.UtcNow
            });
        }

        public int SetTicketAsComplete(int iId, int clientId)
        {
            var ticket = GetPendingById(iId);
            if (ticket == null)
                throw new Exception();

            ticket.date_end = DateTime.UtcNow;
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


    }
}
