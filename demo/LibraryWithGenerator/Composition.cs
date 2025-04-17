// ComposedChild.cs
using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject]
public class Composition
{
    [Key(0)]
    public int Value { get; set; }

    [Key(1)]
    public string? Description { get; set; }
}

[MessagePackObject]
public class ComposedParent
{
    [Key(0)]
    public string? Name { get; set; }

    [Key(1)]
    public Composition? Child { get; set; }
}
