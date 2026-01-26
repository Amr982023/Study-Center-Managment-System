using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repos;
using Domain.Models;
using Infrastructure.Repository.Generic;

namespace Infrastructure.Repository
{
    public class MessageTypeRepository : GenericRepository<MessageType>, IMessageTypeRepository
    {
        public MessageTypeRepository(CenterDbContext context) : base(context)
        {
        }
    }
}
