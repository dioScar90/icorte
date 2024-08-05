using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Entities;
using ICorteApi.Infraestructure.Interfaces;

namespace ICorteApi.Application.Services;

public sealed class MessageService(IMessageRepository messageRepository)
    : BasePrimaryKeyService<Message, int>(messageRepository), IMessageService
{
}
