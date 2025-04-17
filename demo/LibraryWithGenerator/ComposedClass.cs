using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject]
public class ComposedClass
{
    [Key(0)] public DemoStruct StructPart { get; set; }
    [Key(1)] public KeyAsPropClass ClassPart { get; set; } = new();
}