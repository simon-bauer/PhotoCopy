using static PhotoCopy.FileSystemTools;
using static PhotoCopy.FileCollection;
using static PhotoCopySpec.TestFiles;
using PhotoCopy;
using System.Collections.Immutable;

namespace PhotoCopySpec
{
    [TestClass]
    public class FileSystemToolsSpec
    {
        [TestMethod, TestCategory("Integration")]
        public void A_FileCollection_can_be_created_given_a_root_path()
        {
            using TestFiles testFiles = new TestFiles(new List<(RelativePath, DateOnly)>
            {
                ("top_level_file.jpg", new DateOnly(2021, 1, 12)),
                ("subdir/file.jpg", new DateOnly(2020, 3, 14)),
            }.ToImmutableList());

            ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> files = CreateFileCollection(testFiles.Root);

            Assert.AreEqual(2, files.Count);
            Assert.AreEqual(new DateOnly(2021, 1, 12), files[testFiles.AbsolutePath("top_level_file.jpg")].Item1);
            Assert.AreEqual(new DateOnly(2020, 3, 14), files[testFiles.AbsolutePath("subdir/file.jpg")].Item1);
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
        [TestMethod, TestCategory("Integration")]
        public void RobustCopy_copies_file_from_source_path_to_target_path()
        {
            using TestFiles testFiles = new TestFiles(new List<(RelativePath, DateOnly)>
            {
                ("file.jpg", new ()),
            }.ToImmutableList());

            CopyFeedback feedback = RobustCopy(testFiles.AbsolutePath("file.jpg"), testFiles.AbsolutePath("file_copy.jpg"));

            Assert.IsInstanceOfType(feedback, typeof(Success));
            ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> files = CreateFileCollection(testFiles.Root);
            Assert.AreEqual(2, files.Count);
            Assert.AreEqual(
                files[testFiles.AbsolutePath("file.jpg")].Item2,
                files[testFiles.AbsolutePath("file_copy.jpg")].Item2
            );
        }
        [TestMethod, TestCategory("Integration")]
        public void RobustCopy_copies_to_different_path_if_target_path_does_already_exist()
        {
            using TestFiles testFiles = new TestFiles(new List<(RelativePath, DateOnly)>
            {
                ("file.jpg", new ()),
                ("other_file.jpg", new ()),
            }.ToImmutableList());

            CopyFeedback feedback = RobustCopy(testFiles.AbsolutePath("file.jpg"), testFiles.AbsolutePath("other_file.jpg"));

            Assert.IsInstanceOfType(feedback, typeof(Success));
            ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> files = CreateFileCollection(testFiles.Root);
            Assert.AreEqual(3, files.Count);
        }
    }
}