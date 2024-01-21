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
        private AbsolutePath _root;
        public TestFiles(ImmutableList<(RelativePath, DateOnly)> files)
        {
            _root = Path.Combine(Path.GetTempPath(), $"PhotoCopy_TestFiles_{Guid.NewGuid()}");

            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }

            Directory.CreateDirectory(_root);
            foreach (var file in files)
            {
                AbsolutePath absolutePath = Path.Combine(_root, file.Item1);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
                File.WriteAllBytes(absolutePath, GenerateTestFileContent(file.Item2));
            }
        }
        public AbsolutePath Root => _root;
        public AbsolutePath AbsolutePath(RelativePath relativePath) 
        {
            return Path.Combine(_root, relativePath);
        }
        public void Dispose() 
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }
        }
        public static byte[] GenerateTestFileContent(DateOnly date)
        {
            byte[] dateAsBytes = Encoding.ASCII.GetBytes($"{date.Year:D4}:{date.Month:D2}:{date.Day:D2}");
            byte[] guid = Guid.NewGuid().ToByteArray();
            return dateAsBytes.Concat(guid).ToArray();
        }
    }
}
