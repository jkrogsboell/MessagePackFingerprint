using MessagePack;

namespace LibraryWithoutGenerator;

[MessagePackObject]
public struct StructWithoutGenerator
{
    [Key(0)]
    public int Id { get; set; }

    [Key(1)]
    public string? Name { get; set; }
}