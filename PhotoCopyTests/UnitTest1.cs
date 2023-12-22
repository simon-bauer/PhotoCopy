using System.Collections.Immutable;
using System.Text.Json;
using System.Security.Cryptography;
using System.IO;


namespace PhotoCopyTests
{
    public readonly record struct FileSystemSubTree(string Root, ImmutableDictionary<string, MiniFile> Files);
    public readonly record struct MiniFile(DateOnly Date, string Sha256);

    public static class Extensions
    {
        public static string FileHash(this SHA256 sha256, string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                var hashValue = sha256.ComputeHash(fs);
                return Convert.ToHexString(hashValue);
            }
        }
    }

    [TestClass]
    public class UnitTest1
    {
        public static FileSystemSubTree CreateTestFiles()
        {
            return new FileSystemSubTree
            {
                Root = @"C:\temp",
                Files = new Dictionary<string, MiniFile>{{@"test\1", new MiniFile(new DateOnly(), "abcdef0123456789")}}.
                    ToImmutableDictionary()
            };
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
            string root = @"C:\temp";
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var items = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).
                    Select(f => mySHA256.FileHash(f));
                foreach(var item in items) { Console.WriteLine(item); }
            }
        }
    }
}