using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject(keyAsPropertyName: true)]
public class IgnoreMemberClass
{
    public string Visible { get; set; } = string.Empty;

    [IgnoreMember]
    public string Hidden { get; set; } = "ignore me";
}