using System.Collections.Immutable;
using System.Text.Json;

namespace PhotoCopyTests
{
    public record FileSystemSubTree(string Root, ImmutableDictionary<string, File> Files);
    public record File(DateOnly Date, string Sha256);

    [TestClass]
    public class UnitTest1
    {
        public static FileSystemSubTree CreateTestFiles()
        {
            return new FileSystemSubTree("C:/temp",new Dictionary<string, File>
            {
                {"test\\1", new File(new DateOnly(), "abcdef0123456789")}
            }.ToImmutableDictionary());
        }
        [TestMethod]
        public void TestMethod1()
        {
            var files = CreateTestFiles();
            Console.WriteLine(JsonSerializer.Serialize(files, new JsonSerializerOptions { WriteIndented=true}));
        }
        [TestMethod]
        public void TestMethod2() 
        { 
            var files = Directory.EnumerateFiles(@"C:\temp\1", "*", SearchOption.AllDirectories);
            foreach(var file in files) { Console.WriteLine(file); }
        }
    }
}