namespace Act.Caixa.BuildingBlocks.Shared.Models;

public interface ICommandResult
{
    public Guid GetId();
}

public class FailedCommandResult : ICommandResult
{
    public Guid FailureId { get; } = Guid.CreateVersion7();
    public string Message { get; set; }

    public Guid GetId() => FailureId;

    protected FailedCommandResult(string message)
    {
        Message = message;
    }

    public static FailedCommandResult New(string message)
    {
        return new FailedCommandResult(message);
    }
}

public class SuccessCommandResult : ICommandResult
{
    public Guid SuccessId { get; } = Guid.CreateVersion7();

    protected SuccessCommandResult()
    {
    }

    public static SuccessCommandResult New()
    {
        return new SuccessCommandResult();
    }

    public Guid GetId() => SuccessId;
}