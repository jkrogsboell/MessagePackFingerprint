using System.Threading.Tasks;
using Xunit;

namespace MessagePackFingerprintGenerator.Tests;

public class FingerprintGeneratorTreeTests : FingerprintGeneratorTestBase
{
    [Fact]
    public async Task GeneratedFingerprint_For_TreeMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;
              
              namespace Tests;

              [MessagePackObject]
              public class TreeMPClass
              {
                  [Key(0)]
                  public OtherMPClass OtherMPClass { get; init; }
              
                  [Key(1)]
                  public int BProperty { get; init; }
              }
              
              [MessagePackObject]
              public class OtherMPClass 
              {
                  [Key(0)]
                  public string AProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string Tests_OtherMPClass = ShaHelper(
              $"""
              TYPE:Tests.OtherMPClass
              PROP:0:string
              """);
                  public static readonly string Tests_TreeMPClass = ShaHelper(
              $"""
              TYPE:Tests.TreeMPClass
              PROP:0:Tests.OtherMPClass
              TYPE:{TestProject.GeneratedFingerprints.MessagePackFingerprints.Tests_OtherMPClass}
              PROP:1:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }
    
    [Fact]
    public async Task GeneratedFingerprint_For_TreeMPClass_Changed_IsCorrect()
    {
        var source
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public class TreeMPClass
              {
                  [Key(0)]
                  public OtherMPClass OtherMPClass { get; init; }
              
                  [Key(1)]
                  public int BProperty { get; init; }
              }

              [MessagePackObject]
              public class OtherMPClass 
              {
                  [Key(0)]
                  public string AProperty { get; init; }
              }
              """;

        var sourceAfter
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public class TreeMPClass
              {
                  [Key(0)]
                  public OtherMPClass OtherMPClass { get; init; }
              
                  [Key(1)]
                  public int BProperty { get; init; }
              }

              [MessagePackObject]
              public class OtherMPClass 
              {
                  [Key(0)]
                  public int AProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_OtherMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.OtherMPClass
              PROP:0:string
              """);
                  public static readonly string MessagePackFingerprintGenerator_Tests_TreeMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.TreeMPClass
              PROP:0:MessagePackFingerprintGenerator.Tests.OtherMPClass
              TYPE:{TestProject.GeneratedFingerprints.MessagePackFingerprints.MessagePackFingerprintGenerator_Tests_OtherMPClass}
              PROP:1:int
              """);
              """";

        var expectedGeneratedSourceAfter
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_OtherMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.OtherMPClass
              PROP:0:int
              """);
                  public static readonly string MessagePackFingerprintGenerator_Tests_TreeMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.TreeMPClass
              PROP:0:MessagePackFingerprintGenerator.Tests.OtherMPClass
              TYPE:{TestProject.GeneratedFingerprints.MessagePackFingerprints.MessagePackFingerprintGenerator_Tests_OtherMPClass}
              PROP:1:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
        await TestFingerprintGeneratorAsync(sourceAfter, expectedGeneratedSourceAfter);
    }
}