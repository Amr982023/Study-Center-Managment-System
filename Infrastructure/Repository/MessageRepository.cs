using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repos;
using Domain.Models;
using Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class MessageRepository
     : GenericRepository<Message>, IMessage
    {
        public MessageRepository(CenterDbContext context)
            : base(context)
        {
        }

        
        public async Task<IEnumerable<Message>> GetByTypeAsync(int messageTypeId)
        {
            return await _dbSet
                .Where(m => EF.Property<int>(m, "MessageTypeId") == messageTypeId)
                .OrderBy(m => m.Id)
                .ToListAsync();
        }
    }

}
