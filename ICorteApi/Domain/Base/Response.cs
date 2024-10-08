using System.Diagnostics.CodeAnalysis;
using ICorteApi.Domain.Errors;
using ICorteApi.Domain.Interfaces;

namespace ICorteApi.Domain.Base;

public abstract record Response : IResponse
{
    protected Response(bool isSuccess, params Error[] error)
    {
        if (!IsValidResponseConstruction(isSuccess, error))
            throw new ArgumentException("Invalid error", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    private static bool IsValidResponseConstruction(bool isSuccess, Error[] error)
    {
        if (isSuccess && error.Length > 0)
            return false;

        if (!isSuccess && error.Length == 0)
            return false;

        return true;
    }

    public bool IsSuccess { get; }

    public Error[] Error { get; }

    public static Response Success() => new SuccessResponse();

    public static SingleResponse<TValue> Success<TValue>(TValue value)
        where TValue : class, IBaseTableEntity => new(value, true);

    public static CollectionResponse<TValue> Success<TValue>(ICollection<TValue> values)
        where TValue : class, IBaseTableEntity => new(values, true);
    
    public static CollectionResponseWithPagination<TValue> Success<TValue>(
        ICollection<TValue> values, ResponsePagination? pagination)
        where TValue : class, IBaseTableEntity => new(values, true, pagination);

    public static Response Failure(params Error[] error) => new FailureResponse(error);

    public static SingleResponse<TValue> Failure<TValue>(params Error[] error)
        where TValue : class, IBaseTableEntity => new(default, false, error);

    public static CollectionResponse<TValue> FailureCollection<TValue>(params Error[] error)
        where TValue : class, IBaseTableEntity => new(default, false, error);

    internal static IResponse Failure(Error removeError, Error[] errors)
    {
        throw new NotImplementedException();
    }
}

public record SingleResponse<TValue>(TValue Value, bool IsSuccess, params Error[] Error)
    : Response(IsSuccess, Error), ISingleResponse<TValue> where TValue : class, IBaseTableEntity
{
    [NotNull]
    public TValue Value { get; init; } = Value;
}

public record CollectionResponse<TValue>(ICollection<TValue> Values, bool IsSuccess, params Error[] Error)
    : Response(IsSuccess, Error), ICollectionResponse<TValue> where TValue : class, IBaseTableEntity
{
    [NotNull]
    public ICollection<TValue> Values { get; init; } = Values;
}

public record CollectionResponseWithPagination<TValue>(
    ICollection<TValue> Values,
    bool IsSuccess,
    IResponsePagination? Pagination)
    : Response(IsSuccess), ICollectionResponseWithPagination<TValue> where TValue : class, IBaseTableEntity
{
    [NotNull]
    public ICollection<TValue> Values { get; init; } = Values;
    public IResponsePagination? Pagination { get; init; } = Pagination;
}

public record SuccessResponse() : Response(true);

public record FailureResponse(params Error[] Error) : Response(false, Error);

public record ResponsePagination(
    int TotalItems,
    int TotalPages,
    int CurrentPage,
    int PageSize
) : IResponsePagination;
