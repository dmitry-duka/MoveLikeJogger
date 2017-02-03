namespace MoveLikeJogger.DataMining.Commands
{
    public interface ICommand<out TResult, in TPayload>
    {
        TResult Execute(TPayload payload);
    }

    public interface ICommand<out TResult, in TPayload, in TArg>
    {
        TResult Execute(TPayload payload, TArg arg);
    }
}
