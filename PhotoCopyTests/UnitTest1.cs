using System.Collections.Immutable;
using System.Text.Json;

namespace PhotoCopyTests
{
    using Files = ImmutableDictionary<string, MediaFile>;
    public record MediaFile(DateOnly RecordingDate, string Sha256);

    [TestClass]
    public class UnitTest1
    {
        public static Files CreateTestFiles() 
        {
            return new Dictionary<string, MediaFile>
            {
                {"test\\1", new MediaFile(new DateOnly(), "abcdef0123456789")}
            }.ToImmutableDictionary();
        }
        [TestMethod]
        public void TestMethod1()
        {
            var files = CreateTestFiles();
            Console.WriteLine(JsonSerializer.Serialize(files, new JsonSerializerOptions { WriteIndented=true}));
        }
    }
}