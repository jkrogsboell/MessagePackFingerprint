using LibraryWithGenerator;
using LibraryWithoutGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MessagePackApp.GeneratedRegistry;

Console.WriteLine("🎯 MessagePack Demo App\n");

var examples = new object[]
{
    new DemoStruct { X = 10, Y = 20 },
    new DemoRecordStruct(Id: 10, Value: 20 ),
    new DemoRecord(Id: 1, Name: "Alice"),
    new KeyAsPropClass { A = 1, B = 2 },
    new IgnoreMemberClass { Visible = "Hello", Hidden = "World" },
    new DerivedClass { BaseProp = "Base", ExtraProp = 42 },
    new Dog { Name = "Fido", Barks = true },
    new Cat { Name = "Misse", Meows = true },
    new ComposedClass
    {
        StructPart = new DemoStruct { X = 1, Y = 2 },
        ClassPart = new KeyAsPropClass { A = 10, B = 20 }
    },
    new NullableCollections
    {
        Items = ["One", "Two"],
        Scores = new Dictionary<string, int> { ["A"] = 1 }
    },
    new RecordWithoutGenerator(Age: 12, Name: "Noob"),
    new StructWithoutGenerator { Name = "Hi", Id = 1},
    new TreeNodeWithoutGenerator { Label = "X", Children = [new TreeNodeWithoutGenerator { Label = "Y" }] }
};

Console.WriteLine("📦 Types used:");
foreach (var obj in examples)
{
    Console.WriteLine($"  {obj.GetType().FullName}");
}

Console.WriteLine("\n🔐 Fingerprints:");
foreach (var obj in examples)
{
    var fingerprint = MessagePackFingerprintRegistry.ShortPrint(obj, "missing");
            
    Console.WriteLine($"  {obj.GetType().Name}: {fingerprint}");
}

Console.WriteLine("\n✅ Done!");
