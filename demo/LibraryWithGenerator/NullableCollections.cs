using System.Collections.Generic;
using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject(keyAsPropertyName: true)]
public class NullableCollections
{
    public List<string>? Items { get; set; }
    public Dictionary<string, int>? Scores { get; set; }
}