using System.Collections.Immutable;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace PhotoCopy
{
    public readonly record struct LightWeightFile(DateOnly Date, string Sha256);
    public readonly record struct FileSystemSubTree(string Root, ImmutableDictionary<string, LightWeightFile> Files);
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
    public static class FileSystemLayer
    {
        public static DateOnly ExtractDate(byte[] bytes)
        {
            byte[] digitsBetween0And9AndColons = bytes.Where(b => b >= 48 && b <= 58).ToArray();
            string s = Encoding.ASCII.GetString(digitsBetween0And9AndColons);
            var m = Regex.Match(s, @"(\d\d\d\d):(\d\d):(\d\d)");
            try
            {
                return new DateOnly(int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(m.Groups[3].Value));
            }
            catch
            {
                return new DateOnly(1970, 1, 1);
            }
        }
        public static DateOnly ExtractDate(string path)
        {
            using (FileStream fs = new(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new(fs))
                {
                    return ExtractDate(br.ReadBytes(400));
                }
            }
        }
        public static FileSystemSubTree CreateFileSystemSubTree(string root)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                return new FileSystemSubTree
                {
                    Root = root,
                    Files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).
                    Select(f => new KeyValuePair<string, LightWeightFile>(f,
                        new LightWeightFile
                        {
                            Date = ExtractDate(f),
                            Sha256 = mySHA256.FileHash(f)
                        })).
                    ToImmutableDictionary()
                };
            }
        }
    }

}
