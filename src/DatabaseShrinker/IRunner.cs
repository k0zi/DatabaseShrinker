using DatabaseShrinker;

public interface IRunner
{
    void RunConnectionString(string connectionString, ShrinkSetting setting);
    void RunConnectionStrings(string jsonPath, ShrinkSetting setting);
}