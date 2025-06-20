public record DatabasePair(string Database, string File);

public record DbSize(string DbName, string FileName, string TypeDesc, decimal CurrentSizeMb, decimal FreeSizeMb);

public class DbccProcess(Guid clientConnectionId, Spectre.Console.ProgressTask progressTask, double progress = 0.0)
{
    public Guid ClientConnectionId => clientConnectionId;
    public Spectre.Console.ProgressTask ProgressTask => progressTask;

    public double Progress { get; set; } = progress;
}