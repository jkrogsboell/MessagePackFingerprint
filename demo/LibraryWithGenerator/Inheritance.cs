using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject]
public class BaseClass
{
    [Key(0)] public string BaseProp { get; set; } = "";
}

[MessagePackObject]
public class DerivedClass : BaseClass
{
    [Key(1)] public int ExtraProp { get; set; }
}
