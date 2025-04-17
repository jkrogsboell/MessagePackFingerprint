using MessagePack;

namespace LibraryWithGenerator;

[MessagePackObject(keyAsPropertyName: true)]
public class KeyAsPropClass
{
    public int A { get; set; }
    public int B { get; set; }
}