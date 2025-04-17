using System.Threading.Tasks;
using Xunit;

namespace MessagePackFingerprintGenerator.Tests;

public class FingerprintGeneratorSimpleTests : FingerprintGeneratorTestBase
{
    [Fact]
    public async Task GeneratedFingerprint_For_SimpleMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;
              
              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public class SimpleMPClass
              {
                  [Key(0)]
                  public string AProperty { get; init; }
              
                  [Key(1)]
                  public int BProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:0:string
              PROP:1:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }
    
    [Fact]
    public async Task GeneratedFingerprint_For_SimpleMPClass_NonInterchangeable_IsCorrect()
    {
        var sourceBefore
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests
              {
                  [MessagePackObject]
                  public class SimpleMPClass
                  {
                      [Key(0)]
                      public required string AProperty { get; init; }
              
                      [Key(1)]
                      public required int BProperty { get; init; }
                  }
              }
              """;

        var sourceAfter
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests
              {
                  [MessagePackObject]
                  public class SimpleMPClass
                  {
                      [Key(1)]
                      public required string AProperty { get; init; }
                      
                      [Key(0)]
                      public required int YProperty { get; init; }
                  }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:0:string
              PROP:1:int
              """);
              """";

        var expectedGeneratedSourceAfter
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:0:int
              PROP:1:string
              """);
              """";

        await TestFingerprintGeneratorAsync(sourceBefore, expectedGeneratedSource);
        await TestFingerprintGeneratorAsync(sourceAfter, expectedGeneratedSourceAfter);
    }
    
    [Fact]
    public async Task GeneratedFingerprint_For_SimpleMPClass_Interchangeable_IsCorrect()
    {
        var sourceBefore
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests
              {
                  [MessagePackObject]
                  public class SimpleMPClass
                  {
                      [Key(0)]
                      public required string AProperty { get; init; }
              
                      [Key(1)]
                      public required int BProperty { get; init; }
                  }
              }
              """;

        var sourceAfter
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests
              {
                  [MessagePackObject]
                  public class SimpleMPClass
                  {
                      [Key(0)]
                      public required string XProperty { get; init; }
                      
                      [Key(1)]
                      public required int YProperty { get; init; }
                  }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:0:string
              PROP:1:int
              """);
              """";

        await TestFingerprintGeneratorAsync(sourceBefore, expectedGeneratedSource);
        await TestFingerprintGeneratorAsync(sourceAfter, expectedGeneratedSource);
    }

    [Fact]
    public async Task GeneratedFingerprint_For_SimpleMPClassKeyed_IsCorrect()
    {
        var source
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public class SimpleMPClass
              {
                  [Key("A")]
                  public string AProperty { get; init; }
              
                  [Key("B")]
                  public int BProperty { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:A:string
              PROP:B:int
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }

    [Fact]
    public async Task GeneratedFingerprint_For_SimpleMPClassKeyAsProps_IsCorrect()
    {
        var source
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject(keyAsPropertyName: true)]
              public class SimpleMPClass
              {
                  public string AProperty { get; init; }
                  public int BProperty { get; init; }
              }
              """;

        var sourceRenamed
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject(keyAsPropertyName: true)]
              public class SimpleMPClass
              {
                  public string APropertyRN { get; init; }
                  public int BPropertyRN { get; init; }
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:AProperty:string
              PROP:BProperty:int
              """);
              """";

        var expectedGeneratedSourceRenamed
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_SimpleMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.SimpleMPClass
              PROP:APropertyRN:string
              PROP:BPropertyRN:int
              """);
              """";
        
        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
        await TestFingerprintGeneratorAsync(sourceRenamed, expectedGeneratedSourceRenamed);
    }
    
    [Fact]
    public async Task GeneratedFingerprint_For_EmptyMPClass_IsCorrect()
    {
        var source
            = """
              using MessagePack;

              namespace MessagePackFingerprintGenerator.Tests;

              [MessagePackObject]
              public class EmptyMPClass
              {
              }
              """;

        var expectedGeneratedSource
            = """"
                  public static readonly string MessagePackFingerprintGenerator_Tests_EmptyMPClass = ShaHelper(
              $"""
              TYPE:MessagePackFingerprintGenerator.Tests.EmptyMPClass
              """);
              """";

        await TestFingerprintGeneratorAsync(source, expectedGeneratedSource);
    }
}