namespace DatabaseShrinker;

public interface ICommand
{
    string CommandArgument { get; }
    void Command(string[] args);
}