using static PhotoCopy.FileSystemTools;
using static PhotoCopy.FileCollection;

namespace PhotoCopySpec
{
    [TestClass]
    public class FileSystemToolsSpec
    {
        [TestMethod]
        public void A_FileCollection_can_be_created_given_a_root_path()
        {
            string root = @"C:\temp";
            var files = CreateFileCollection(root);

            File.WriteAllText(@"C:\temp\text.txt", Serialize(files));
            foreach (var file in files) { Console.WriteLine($"{file.Key}:{file.Value}"); }
        }

        string byteArrayAsHexStringFromBeginningOfJpegFile = "FFD8FFE16FFE45786966000049492A00080000000D000E01020020000000AA0000000F01020006000000CA000000100102001D000000D00000001201030001000000010000001A01050001000000EE0000001B01050001000000F60000002801030001000000020000003201020014000000FE0000003B010200010000000000000013020300010000000200000098820200010000000000000069870400010000009201000025880400010000003440000034410000202020202020202020202020202020202020202020202020202020202020200043616E6F6E0043616E6F6E20506F77657253686F742047372058204D61726B2049490051B400000001000000B400000001000000323032303A30373A30332031313A31393A333300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000028009A82050001000000780300009D8205000100000080030000228803000100000002000000278803000100000020030000308803000100000002000000328804000100000020";

        [TestMethod]
        public void A_Date_can_be_extracted_from_the_beginning_of_a_jpeg_file()
        {
            byte[] beginningOfJpegFile = Convert.FromHexString(byteArrayAsHexStringFromBeginningOfJpegFile);

            DateOnly date = ExtractDate(beginningOfJpegFile);
            Assert.AreEqual(new DateOnly(2020, 7, 3), date);
        }
        [TestMethod]
        public void Extract_beginning_of_jpeg_file_as_hexstring()
        {
            var bytes = File.ReadAllBytes(@"C:\temp\1\148___07\IMG_8188.JPG");
            var str = Convert.ToHexString(bytes);
            //Console.WriteLine(str);

            var d = ExtractDate(@"C:\temp\1\148___07\IMG_8188.JPG");
            Console.WriteLine(d);
        }
    }
}