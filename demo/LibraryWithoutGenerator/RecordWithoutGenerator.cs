using MessagePack;

namespace LibraryWithoutGenerator;

[MessagePackObject]
public record RecordWithoutGenerator(
    [property: Key(0)] int Age,
    [property: Key(1)] string Name
);