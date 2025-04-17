namespace MessagePackFingerprintGenerator.Tests;

using System.Threading.Tasks;
using Xunit;

public class FingerprintGeneratorUnionTypeTests : FingerprintGeneratorTestBase
{
    [Fact]
    public async Task GeneratedFingerprint_For_UnionMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;
              
              namespace Tests;

              [MessagePackObject]
              [Union(0, typeof(A))]
              [Union(1, typeof(B))]
              public abstract class UnionMPClass
              {
              }
              
              [MessagePackObject]
              public class A : UnionMPClass 
              {
                  [Key(0)]
                  public string AProperty { get; init; }
              }

              [MessagePackObject]
              public class B : UnionMPClass
              {
                  [Key(0)]
                  public string AProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string Tests_A = ShaHelper(
              $"""
              TYPE:Tests.A
              PROP:0:string
              """);
                  public static readonly string Tests_B = ShaHelper(
              $"""
              TYPE:Tests.B
              PROP:0:string
              """);
                  public static readonly string Tests_UnionMPClass = ShaHelper(
              $"""
              TYPE:Tests.UnionMPClass
              UNION:0:Tests.A
              TYPE:{TestProject.GeneratedFingerprints.MessagePackFingerprints.Tests_A}
              UNION:1:Tests.B
              TYPE:{TestProject.GeneratedFingerprints.MessagePackFingerprints.Tests_B}
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }
}