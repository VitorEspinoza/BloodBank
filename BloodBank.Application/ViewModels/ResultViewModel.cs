namespace BloodBank.Application.ViewModels;

public class ResultViewModel
{
    public ResultViewModel(bool isSuccess = true, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; private set; }
    public List<string>? Errors { get; private set; }

    public static ResultViewModel Success()
        => new();

    public static ResultViewModel Error(List<string>? errors)
        => new(false, errors);
}

public class ResultViewModel<T> : ResultViewModel
{
    public ResultViewModel(T? data, bool isSuccess = true, List<string>? errors = null)
        : base(isSuccess, errors)
    {
        Data = data;
    }

    public T? Data { get; private set; }

    public static ResultViewModel<T> Success(T data)
        => new(data);

    public static ResultViewModel<T> Error(List<string>? errors = null)
        => new(default, false, errors);
}