using MessagePack;

namespace LibraryWithoutGenerator;

[MessagePackObject]
public class TreeNodeWithoutGenerator
{
    [Key(0)]
    public string? Label { get; set; }

    [Key(1)]
    public List<TreeNodeWithoutGenerator>? Children { get; set; }
}
