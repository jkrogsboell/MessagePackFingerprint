using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject]
public struct DemoStruct
{
    [Key(0)] public int X;
    [Key(1)] public int Y;
}