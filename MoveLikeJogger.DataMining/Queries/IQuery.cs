namespace MoveLikeJogger.DataMining.Queries
{
    public interface IQuery<out TResult>
    {
        TResult Execute();
    }

    public interface IQuery<out TResult, in TArg>
    {
        TResult Execute(TArg arg);
    }
}
