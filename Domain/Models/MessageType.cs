using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class MessageType : BaseEntity<int>
    {
        public string Name { get; private set; }

        private MessageType() { } // ORM

        private MessageType(string name)
        {
            Name = name;
        }

        public static Result<MessageType> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<MessageType>.Failure("Message type name is required");

            return Result<MessageType>.Success(new MessageType(name));
        }
    }

}
