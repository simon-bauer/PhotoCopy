using System.Collections.Immutable;
using System.Text.Json;
using PhotoCopy;

namespace PhotoCopySpec
{
    [TestClass]
    public class FileSystemLayerSpec
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
            var fileSystem = Abc.RenameMe(root);

            using (FileStream fs = new FileStream(@"C:\temp\text.txt", FileMode.Create, FileAccess.Write))
            {
                JsonSerializer.Serialize(fs, fileSystem.Files, new JsonSerializerOptions { WriteIndented = true });
            }
            foreach (var file in fileSystem.Files) { Console.WriteLine($"{file.Key}:{file.Value}"); }
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