using PhotoCopy;
using static PhotoCopySpec.FileSystemTestHelper;
using static PhotoCopy.FileSystemTools;
using System.Collections.Immutable;

namespace PhotoCopySpec
{
    [TestClass]
    public class FileSystemTestHelperSpec
    {
        [TestMethod]
        public void A_test_file_contains_the_input_date_similar_as_media_files()
        {
            DateOnly inputDate = new DateOnly(2020, 2, 22);
            byte[] fileContent = GenerateTestFileContent(inputDate);
            DateOnly extractedDate = ExtractDate(fileContent);
            Assert.AreEqual(inputDate, extractedDate);
        }

        [TestMethod]
        public void Two_test_files_generated_with_the_same_date_have_a_different_Sha256()
        {
            DateOnly inputDate = new DateOnly(2020, 2, 21);
            byte[] firstFileContent = GenerateTestFileContent(inputDate);
            byte[] secondFileContent = GenerateTestFileContent(inputDate);
            Assert.AreNotEqual(new Sha256(firstFileContent), new Sha256(secondFileContent));
        }
        [TestMethod]
        public void Temporary_test_files_can_be_created_and_deleted()
        {
            CreateTestFiles(new List<(RelativePath, DateOnly)>
            {
                    ("subdir/file1.jpg", new DateOnly(2020, 3, 14)),
                    ("subdir2/file3.jpg", new DateOnly(2020, 3, 14)),
                    ("otherSubDir/file2.mp4", new DateOnly(2020, 3, 15))
            }.ToImmutableList());
            DeleteTestFiles();
        }
    }
}
