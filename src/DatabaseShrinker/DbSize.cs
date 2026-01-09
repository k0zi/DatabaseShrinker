namespace DatabaseShrinker;

public record DbSize(string DbName, string FileName, string TypeDesc, decimal CurrentSizeMb, decimal FreeSizeMb);