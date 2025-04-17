using System.Threading.Tasks;
using Xunit;

namespace MessagePackFingerprintGenerator.Tests;

public class FingerprintGeneratorSimpleRecordTests : FingerprintGeneratorTestBase
{
    [Fact]
    public async Task GeneratedFingerprint_For_SimpleRecordMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;
              
              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public record SimpleMPClass([property: Key(0)] string AProperty, [property: Key(1)] int BProperty)
              {
                  [Key(2)]
                  public int CProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:0:string
              PROP:1:int
              PROP:2:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }
}