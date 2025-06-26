namespace DatabaseShrinker;

public record ShrinkSetting(bool Log = false, 
    bool ShrinkAll = false, 
    bool ShrinkOnlyLarge = false, 
    bool AutoConfirm = false,
    bool DropLog = false);