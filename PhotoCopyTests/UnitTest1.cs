using System.Collections.Immutable;
using System.Text.Json;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace PhotoCopyTests
{
    public readonly record struct FileSystemSubTree(string Root, ImmutableDictionary<string, LightWeightFile> Files);
    public readonly record struct LightWeightFile(DateOnly Date, string Sha256);

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

    public static class Abc
    {
        public static DateOnly ExtractDate(byte[] bytes)
        {
            byte[] digitsBetween0And9AndColons = bytes.Where(b => b >= 48 && b <= 58).ToArray();
            string s = Encoding.ASCII.GetString(digitsBetween0And9AndColons);
            var m = Regex.Match(s, @"(\d\d\d\d):(\d\d):(\d\d)");
            try
            {
                return new DateOnly(Int32.Parse(m.Groups[1].Value),
                    Int32.Parse(m.Groups[2].Value),
                    Int32.Parse(m.Groups[3].Value));
            }
            catch
            {
                return new DateOnly(1970, 1, 1);
            }
        }

        public static DateOnly ExtractDate(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    return ExtractDate(br.ReadBytes(400));
                }
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
                Files = new Dictionary<string, LightWeightFile>{{@"test\1", new LightWeightFile(new DateOnly(), "abcdef0123456789")}}.
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
                    Select(f => new KeyValuePair<string, LightWeightFile>(f, 
                        new LightWeightFile 
                        { 
                            Date = Abc.ExtractDate(f), 
                            Sha256 = mySHA256.FileHash(f) 
                        })).
                    ToImmutableDictionary();
                using (FileStream fs = new FileStream(@"C:\temp\text.txt", FileMode.Create, FileAccess.Write))
                {
                    JsonSerializer.Serialize(fs, items, new JsonSerializerOptions { WriteIndented = true });
                }
                foreach(var item in items) { Console.WriteLine($"{item.Key}:{item.Value}"); }
            }
        }

        string byteArrayAsHexStringFromBeginningOfJpegFile = "FFD8FFE16FFE45786966000049492A00080000000D000E01020020000000AA0000000F01020006000000CA000000100102001D000000D00000001201030001000000010000001A01050001000000EE0000001B01050001000000F60000002801030001000000020000003201020014000000FE0000003B010200010000000000000013020300010000000200000098820200010000000000000069870400010000009201000025880400010000003440000034410000202020202020202020202020202020202020202020202020202020202020200043616E6F6E0043616E6F6E20506F77657253686F742047372058204D61726B2049490051B400000001000000B400000001000000323032303A30373A30332031313A31393A333300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000028009A82050001000000780300009D8205000100000080030000228803000100000002000000278803000100000020030000308803000100000002000000328804000100000020";

        [TestMethod]
        public void TestMethod3()
        {
            byte[] beginningOfJpegFile = Convert.FromHexString(byteArrayAsHexStringFromBeginningOfJpegFile);

            DateOnly date = Abc.ExtractDate(beginningOfJpegFile);
            Assert.AreEqual(new DateOnly(2020, 7, 3), date);
        }
        [TestMethod]
        public void TestMethod4()
        {
            var bytes = File.ReadAllBytes(@"C:\temp\1\148___07\IMG_8188.JPG");
            var str = Convert.ToHexString(bytes);
            //Console.WriteLine(str);

            var d = Abc.ExtractDate(@"C:\temp\1\148___07\IMG_8188.JPG");
            Console.WriteLine(d);
        }
    }
}