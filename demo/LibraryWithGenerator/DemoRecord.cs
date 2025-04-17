using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject]
public record DemoRecord(
    [property: Key(0)] int Id,
    [property: Key(1)] string Name
);