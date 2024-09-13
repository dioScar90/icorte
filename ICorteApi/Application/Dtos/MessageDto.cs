namespace ICorteApi.Application.Dtos;

public record MessageDtoCreate(
    string Content,
    DateTime SentAt
) : IDtoRequest<Message>;

public record MessageDtoIsReadUpdate(
    int Id,
    bool IsRead
) : IDtoRequest<Message>;

public record MessageDtoResponse(
    int Id,
    int AppointmentId,
    int SenderId,
    string Content,
    DateTime SentAt,
    bool IsRead,
    string FirstName,
    string LastName
) : IDtoResponse<Message>;
