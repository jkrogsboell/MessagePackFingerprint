using MessagePack;

namespace LibraryWithGenerator;

[Union(0, typeof(Dog))]
[Union(1, typeof(Cat))]
[MessagePackObject]
public abstract class Animal
{
    [Key(0)] public string Name { get; set; } = "";
}

[MessagePackObject]
public class Dog : Animal
{
    [Key(1)] public bool Barks { get; set; }
}

[MessagePackObject]
public class Cat : Animal
{
    [Key(1)] public bool Meows { get; set; }
}