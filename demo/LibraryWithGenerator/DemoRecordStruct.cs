using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject]
public readonly record struct DemoRecordStruct([property: Key(0)] int Id, [property: Key(1)] int Value);