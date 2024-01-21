using PhotoCopy;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCopySpec
{
    public class TestFiles : IDisposable
    {
        public TestFiles(ImmutableList<(RelativePath, DateOnly)> files)
        {
        }
        public void Dispose() 
        {
        }
        public static byte[] GenerateTestFileContent(DateOnly date)
        {
            byte[] dateAsBytes = Encoding.ASCII.GetBytes($"{date.Year:D4}:{date.Month:D2}:{date.Day:D2}");
            byte[] guid = Guid.NewGuid().ToByteArray();
            return dateAsBytes.Concat(guid).ToArray();
        }
        public static string TestFilesRoot()
        {
            return Path.Combine(Path.GetTempPath(), "PhotoCopy_TestFiles");
        }
        public static string AbsoluteTestPath(RelativePath relativePath)
        {
            return Path.Combine(TestFilesRoot(), relativePath);
        }
        /// <summary>
        /// Delete recursively TestFilesRoot first and then create given files.
        /// </summary>
        public static void CreateTestFiles(ImmutableList<(RelativePath, DateOnly)> files)
        {
            DeleteTestFiles();
            Directory.CreateDirectory(TestFilesRoot());
            foreach (var file in files)
            {
                AbsolutePath absolutePath = Path.Combine(TestFilesRoot(), file.Item1);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
                File.WriteAllBytes(absolutePath, GenerateTestFileContent(file.Item2));
            }
        }
        public static void DeleteTestFiles()
        {
            if (Directory.Exists(TestFilesRoot()))
            {
                Directory.Delete(TestFilesRoot(), true);
            }
        }
    }
}
