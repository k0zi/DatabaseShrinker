namespace DatabaseShrinker;

public class DbccProcess(int sessionId, Spectre.Console.ProgressTask progressTask, double progress = 0.0)
{
    public int SessionId => sessionId;
    public Spectre.Console.ProgressTask ProgressTask => progressTask;

    public double Progress { get; set; } = progress;
}