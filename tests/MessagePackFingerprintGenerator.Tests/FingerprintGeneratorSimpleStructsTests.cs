namespace MessagePackFingerprintGenerator.Tests;

using System.Threading.Tasks;
using Xunit;

public class FingerprintGeneratorSimpleStructsTests : FingerprintGeneratorTestBase
{
    [Fact]
    public async Task GeneratedFingerprint_For_SimpleStructMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public readonly struct SimpleMPStruct
              {
                  [Key(0)]
                  public int CProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPStruct = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPStruct
              PROP:0:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }

    [Fact]
    public async Task GeneratedFingerprint_For_SimpleRecordStructMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public readonly record struct SimpleMPStruct([property: Key(0)] string AProperty, [property: Key(1)] int BProperty)
              {
                  [Key(2)]
                  public int CProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPStruct = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPStruct
              PROP:0:string
              PROP:1:int
              PROP:2:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }
}