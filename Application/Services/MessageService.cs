using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;

namespace Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _uow;

        public MessageService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Message>> CreateAsync(int messageTypeId, string text)
        {
            var type = await _uow.MessageTypes.GetByIdAsync(messageTypeId);
            if (type is null)
                return Result<Message>.Failure("Message type not found.");

            var result = Message.Create(type, text);
            if (!result.IsSuccess)
                return Result<Message>.Failure(result.ErrorMessage!);

            await _uow.Messages.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Message>.Success(result.Value!);
        }

        public async Task<Result<Message>> GetByIdAsync(int id)
        {
            var message = await _uow.Messages.GetByIdAsync(id);
            return message is null
                ? Result<Message>.Failure("Message not found.")
                : Result<Message>.Success(message);
        }

        public async Task<Result<IEnumerable<Message>>> GetAllAsync()
        {
            var messages = await _uow.Messages.GetAllAsync();
            return Result<IEnumerable<Message>>.Success(messages);
        }

        public async Task<Result<IEnumerable<Message>>> GetByTypeAsync(int messageTypeId)
        {
            var type = await _uow.MessageTypes.GetByIdAsync(messageTypeId);
            if (type is null)
                return Result<IEnumerable<Message>>.Failure("Message type not found.");

            var messages = await _uow.Messages.GetByTypeAsync(messageTypeId);
            return Result<IEnumerable<Message>>.Success(messages);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var message = await _uow.Messages.GetByIdAsync(id);
            if (message is null)
                return Result<bool>.Failure("Message not found.");

            await _uow.Messages.DeleteAsync(message);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
