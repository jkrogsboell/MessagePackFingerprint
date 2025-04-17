# MessagePack Fingerprint Generator

**MessagePack Fingerprint Generator** is a set of Roslyn incremental source generators that provides deterministic fingerprints and usage registries for `MessagePackObject`-annotated types across a solution.

This generator is intended for developers who use [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp) and need to:

- Generate content-stable fingerprints for serializable types
- Reference those fingerprints at runtime

---

## 📦 Package Contents

This generator project contains **two incremental source generators**:

### 1. `MessagePackFingerprintGenerator`

Generates a static class containing SHA256-based fingerprints for all `MessagePackObject` types declared in the current project.

**Example output (generated file):**

```csharp
namespace MyProject.GeneratedFingerprints;

public static class MessagePackFingerprints
{
    public static readonly string MyProject_MyType = ShaHelper("""
        ...content-dependent fingerprint seed...
    """);
}
```

### 2. `MessagePackRegistryGenerator`

Generates a switch-based registry of all `MessagePackObject` types *referenced* in the current assembly—regardless of where they are declared.

This allows consumers to dynamically inspect, map, or emit fingerprints only for the types they actually use.

### 🧠 Performance & Multi-project Setup

To fully benefit from **incremental build performance**, you should configure the generator in each project that:

- Declares `[MessagePackObject]` types _and/or_
- Consumes them (e.g. via method parameters, `new` expressions, or collections)

This ensures:

- ✅ **Fast builds**: Only changed files/types are re-evaluated per project
- ✅ **Accurate discovery**: Each project emits fingerprints and registry entries for the types it _actually uses_
- ✅ **Modular output**: Generators scale across large solutions without scanning the entire dependency graph from a single root

💡 You must tell the generator what other projects it is installed in. That can be done in your `.csproj` files or globally in your `Directory.Build.props` file:

```xml
<Project>
   <ItemGroup>
      <CompilerVisibleProperty Include="MessagePackFingerprintReferencedWithGenerator" />
   </ItemGroup>

   <PropertyGroup>
      <MessagePackFingerprintReferencedWithGenerator>MessagePackApp,LibraryWithGenerator</MessagePackFingerprintReferencedWithGenerator>
   </PropertyGroup>
</Project>
```
---

## Demo

This repository includes a demo app (`MessagePackApp`) that showcases how the fingerprint generator works in practice. The app references types from two libraries – one that uses the generator and one that doesn't – and prints out the fingerprints of all relevant `MessagePackObject`-annotated types that are actually used.

### Sample Output
```text
🎯 MessagePack Demo App

📦 Types used:
LibraryWithGenerator.DemoStruct
LibraryWithGenerator.DemoRecordStruct
LibraryWithGenerator.DemoRecord
LibraryWithGenerator.KeyAsPropClass
LibraryWithGenerator.IgnoreMemberClass
LibraryWithGenerator.DerivedClass
LibraryWithGenerator.Dog
LibraryWithGenerator.Cat
LibraryWithGenerator.ComposedClass
LibraryWithGenerator.NullableCollections
LibraryWithoutGenerator.RecordWithoutGenerator
LibraryWithoutGenerator.StructWithoutGenerator
LibraryWithoutGenerator.TreeNodeWithoutGenerator

🔐 Fingerprints:
DemoStruct: 2E1AE44C
DemoRecordStruct: 08C5B370
DemoRecord: E443845A
KeyAsPropClass: B0574CB3
IgnoreMemberClass: D90C6E60
DerivedClass: 4EB96C23
Dog: 75F5F586
Cat: 8A08F5C0
ComposedClass: 8656637D
NullableCollections: 27869092
RecordWithoutGenerator: B3CB0C34
StructWithoutGenerator: B7D2293E
TreeNodeWithoutGenerator: E607E32F

✅ Done!
```
---
## 🚀 Usage

1. **Reference the generator project or NuGet package** in each MessagePack-using project.

2. **Annotate your serializable types** with `[MessagePackObject]`.

3. **(Optional)** Pass a build property to indicate referenced projects that also use this generator:

   In your `.csproj`:

   ```xml
   <PropertyGroup>
     <MessagePackFingerprintReferencedWithGenerator>ProjectA,ProjectB</MessagePackFingerprintReferencedWithGenerator>
   </PropertyGroup>
   ```

4. **Access fingerprints at runtime:**

   ```csharp
   var fingerprint = GeneratedFingerprints.MessagePackFingerprints.MyProject_MyType;
   ```

5. **(Optional)** Use the registry:

   ```csharp
   var fingerprint = GeneratedRegistry.MessagePackFingerprintRegistry.GetFingerprint(myObject);
   ```

   ```csharp
   var fingerprint = GeneratedRegistry.MessagePackFingerprintRegistry.ShortPrint(myObject);
   ```
---

## ⚙️ Compatibility

- Requires Microsoft.CodeAnalysis.CSharp 4.4.0 or higher.
- Minimum required .NET SDK: 7.0.200.
- Minimum required Visual Studio: 17.5.
- Compatible with C# source generators using incremental generator APIs.
- Tested against Roslyn 4.4.0 to ensure consistent behavior between generation and runtime.

---

## 📝 License

This project is licensed under the [MIT License](LICENSE).
