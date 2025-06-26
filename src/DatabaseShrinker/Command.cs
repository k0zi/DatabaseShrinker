namespace DatabaseShrinker;

public record Command(string CommandArgument, Action<string, ShrinkSetting> CommandAction);