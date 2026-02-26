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
    public class MessageTypeService : IMessageTypeService
    {
        private readonly IUnitOfWork _uow;

        public MessageTypeService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<MessageType>> CreateAsync(string name)
        {
            var existing = await _uow.MessageTypes.AnyAsync(m => m.Name == name);
            if (existing)
                return Result<MessageType>.Failure($"Message type '{name}' already exists.");

            var result = MessageType.Create(name);
            if (!result.IsSuccess)
                return Result<MessageType>.Failure(result.ErrorMessage!);

            await _uow.MessageTypes.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<MessageType>.Success(result.Value!);
        }

        public async Task<Result<MessageType>> GetByIdAsync(int id)
        {
            var type = await _uow.MessageTypes.GetByIdAsync(id);
            return type is null
                ? Result<MessageType>.Failure("Message type not found.")
                : Result<MessageType>.Success(type);
        }

        public async Task<Result<IEnumerable<MessageType>>> GetAllAsync()
        {
            var types = await _uow.MessageTypes.GetAllAsync();
            return Result<IEnumerable<MessageType>>.Success(types);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var type = await _uow.MessageTypes.GetByIdAsync(id);
            if (type is null)
                return Result<bool>.Failure("Message type not found.");

            await _uow.MessageTypes.DeleteAsync(type);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
