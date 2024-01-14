using static PhotoCopy.FileSystemTools;
using static PhotoCopy.FileCollection;
using System.Text;
using PhotoCopy;

namespace PhotoCopySpec
{
    [TestClass]
    public class FileSystemToolsSpec
    {
        [TestMethod]
        public void A_FileCollection_can_be_created_given_a_root_path()
        {
            string root = @"C:\temp";
            System.Collections.Immutable.ImmutableDictionary<PhotoCopy.AbsolutePath, (DateOnly, PhotoCopy.Sha256)> files = CreateFileCollection(root);

            File.WriteAllText(@"C:\temp\text.txt", Serialize(files));
            foreach (KeyValuePair<PhotoCopy.AbsolutePath, (DateOnly, PhotoCopy.Sha256)> file in files) { Console.WriteLine($"{file.Key}:{file.Value}"); }
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
            byte[] bytes = File.ReadAllBytes(@"C:\temp\1\148___07\IMG_8188.JPG");
            string str = Convert.ToHexString(bytes);
            //Console.WriteLine(str);

            DateOnly d = ExtractDate(@"C:\temp\1\148___07\IMG_8188.JPG");
            Console.WriteLine(d);
        }
        [TestMethod]
        public void A_test_file_contains_the_input_date_similar_as_media_files()
        {
            DateOnly inputDate = new DateOnly(2020, 2, 2);
            byte[] fileContent = GenerateTestFileContent(inputDate);
            DateOnly extractedDate = ExtractDate(fileContent);
            Assert.AreEqual(inputDate, extractedDate);
        }

        [TestMethod]
        public void Two_test_files_generated_with_the_same_date_have_a_different_Sha256()
        {
            DateOnly inputDate = new DateOnly(2020, 2, 2);
            byte[] firstFileContent = GenerateTestFileContent(inputDate);
            byte[] secondFileContent = GenerateTestFileContent(inputDate);
            Assert.AreNotEqual(new Sha256(firstFileContent), new Sha256(secondFileContent));
        }
    }
}