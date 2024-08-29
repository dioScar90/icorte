using ICorteApi.Application.Dtos;
using ICorteApi.Application.Interfaces;
using ICorteApi.Domain.Base;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Errors;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Infraestructure.Interfaces;

namespace ICorteApi.Application.Services;

public sealed class ChatService(IMessageRepository repository)
    : BaseService<Message>(repository), IChatService
{
    new private readonly IMessageRepository _repository = repository;
    public async Task<ISingleResponse<Message>> CreateAsync(IDtoRequest<Message> dtoRequest, int appointmentId, int senderId)
    {
        if (dtoRequest is not MessageDtoRequest dto)
            throw new ArgumentException("Tipo de DTO inválido", nameof(dtoRequest));

        var entity = new Message(dto, appointmentId, senderId);
        return await CreateAsync(entity);
    }

    public async Task<ISingleResponse<Message>> GetByIdAsync(int id, int appointmentId)
    {
        var resp = await GetByIdAsync(id);
        
        if (!resp.IsSuccess)
            return resp;

        if (resp.Value!.AppointmentId != appointmentId)
            return Response.Failure<Message>(Error.TEntityNotFound);

        return resp;
    }
    
    public async Task<ICollectionResponse<Message>> GetAllAsync(int? page, int? pageSize, int appointmentId)
    {
        return await GetAllAsync(new(page, pageSize, x => x.AppointmentId == appointmentId));
    }
    
    public async Task<IResponse> DeleteAsync(int id, int appointmentId)
    {
        var resp = await GetByIdAsync(id, appointmentId);

        if (!resp.IsSuccess)
            return resp;

        var entity = resp.Value!;
        return await DeleteAsync(entity);
    }
    
    public async Task<ISingleResponse<Message>> SendMessageAsync(MessageDtoRequest dtoRequest, int appointmentId, int senderId)
    {
        var entity = new Message(dtoRequest, appointmentId, senderId);
        return await CreateAsync(entity);
    }
    
    public async Task<IResponse> MarkMessageAsReadAsync(MessageIsReadDtoRequest[] dtoRequest, int senderId)
    {
        var ids = dtoRequest.Where(dto => dto.IsRead).Select(dto => dto.Id).ToArray();
        return await _repository.MarkMessageAsReadAsync(ids, senderId);
    }
    
    public async Task<IResponse> DeleteMessageAsync(int id, int appointmentId, int senderId)
    {
        var resp = await GetByIdAsync(x => x.Id == id && x.AppointmentId == appointmentId && x.SenderId == senderId);

        if (!resp.IsSuccess)
            return resp;

        return await DeleteAsync(resp.Value!);
    }
    
    public async Task<MessageDtoResponse[]> GetLastMessagesAsync(int appointmentId, int senderId, int? lastMessageId)
    {
        return await _repository.GetLastMessagesAsync(appointmentId, senderId, lastMessageId);
    }

    // public async Task<List<Message>> GetConversationAsync(int barberId, int clientId);
    // public async Task<bool> CanBarberSendMessageAsync(int barberId, int clientId);
    // public async Task<List<Message>> GetUnreadMessagesAsync(int barberId, int clientId);
    // public async Task<List<Conversation>> GetClientChatHistoryAsync(int clientId);
    // public async Task<List<Conversation>> GetBarberChatHistoryAsync(int barberId);
    // public async Task<bool> IsActiveConversationAsync(int barberId, int clientId);
    // public async Task<Conversation> StartConversationAsync(int clientId, int barberId);
}


/*
1. Enviar Mensagem
Nome: SendMessageAsync
Descrição: Envia uma mensagem de um usuário para outro, validando as regras de envio.
Parâmetros: int senderId, int recipientId, string messageText
Regras de Negócio:
Verificar se o remetente é o Barber e, se for, garantir que o Client já enviou uma mensagem ou existe um Appointment ativo entre eles.
Se o remetente for o Client, a mensagem é enviada sem restrições.
Retorno: Task<IResponse> (ou outro tipo adequado)

2. Obter Conversa Entre Cliente e Barbeiro
Nome: GetConversationAsync
Descrição: Retorna todas as mensagens trocadas entre um Barber e um Client.
Parâmetros: int barberId, int clientId
Retorno: Task<List<Message>>

3. Verificar Elegibilidade para Enviar Mensagem
Nome: CanBarberSendMessageAsync
Descrição: Verifica se um Barber pode enviar uma mensagem a um Client.
Parâmetros: int barberId, int clientId
Regras de Negócio:
O Client deve ter iniciado a conversa ou existir um Appointment ativo entre eles.
Retorno: Task<bool>

4. Obter Mensagens Não Lidas
Nome: GetUnreadMessagesAsync
Descrição: Retorna todas as mensagens não lidas de uma conversa entre um Barber e um Client.
Parâmetros: int barberId, int clientId
Retorno: Task<List<Message>>

5. Marcar Mensagem como Lida
Nome: MarkMessageAsReadAsync
Descrição: Marca uma mensagem como lida.
Parâmetros: int messageId
Retorno: Task<IResponse>

6. Excluir Mensagem
Nome: DeleteMessageAsync
Descrição: Exclui uma mensagem de uma conversa.
Parâmetros: int messageId, int userId
Regras de Negócio:
Somente o remetente pode excluir a mensagem.
Retorno: Task<IResponse>

7. Obter Histórico de Conversas do Cliente
Nome: GetClientChatHistoryAsync
Descrição: Retorna o histórico de conversas do Client com todos os Barbers com os quais ele interagiu.
Parâmetros: int clientId
Retorno: Task<List<Conversation>> (ou outro tipo que represente uma lista de conversas)

8. Obter Histórico de Conversas do Barbeiro
Nome: GetBarberChatHistoryAsync
Descrição: Retorna o histórico de conversas do Barber com todos os Clients com os quais ele interagiu.
Parâmetros: int barberId
Retorno: Task<List<Conversation>> (ou outro tipo que represente uma lista de conversas)

9. Verificar se Existe Conversa Ativa
Nome: IsActiveConversationAsync
Descrição: Verifica se existe uma conversa ativa (não finalizada) entre um Barber e um Client.
Parâmetros: int barberId, int clientId
Retorno: Task<bool>

10. Iniciar Conversa
Nome: StartConversationAsync
Descrição: Inicia uma nova conversa entre um Barber e um Client, geralmente após o Client enviar a primeira mensagem ou abrir um Appointment.
Parâmetros: int clientId, int barberId
Retorno: Task<Conversation> (ou outro tipo que represente a conversa iniciada)
*/
