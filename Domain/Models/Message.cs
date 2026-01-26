using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Message : BaseEntity<int>
    {
        public MessageType Type { get; private set; }
        public string Text { get; private set; }

        private Message() { } // ORM

        private Message(MessageType type, string text)
        {
            Type = type;
            Text = text;
        }

        public static Result<Message> Create(MessageType type, string text)
        {
            if (type == null)
                return Result<Message>.Failure("Message type is required");

            if (string.IsNullOrWhiteSpace(text))
                return Result<Message>.Failure("Message text is required");

            return Result<Message>.Success(new Message(type, text));
        }
    }

}
