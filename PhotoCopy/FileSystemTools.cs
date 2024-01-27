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
    public record class CopyFeedback(AbsolutePath Source, AbsolutePath Target);
    public record class Success(AbsolutePath Source, AbsolutePath Target) : CopyFeedback(Source, Target);
    public record class TargetExist(AbsolutePath Source, AbsolutePath Target, AbsolutePath AlternativeTarget) : CopyFeedback(Source, Target);
    public record class Failed(AbsolutePath Source, AbsolutePath Target, Exception Exception) : CopyFeedback(Source, Target);
    public static class FileSystemTools
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
        public static ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> CreateFileCollection(string root)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).
                    Select(f => new KeyValuePair<AbsolutePath, (DateOnly, Sha256)>
                    (
                        f,
                        (ExtractDate(f), new Sha256(mySHA256.FileHash(f)))
                    )).
                    ToImmutableDictionary();
            }
        }
        public static CopyFeedback RobustCopy(AbsolutePath source, AbsolutePath target)
        {
            try
            {
                if (File.Exists(target))
                {
                    AbsolutePath alternativeTarget =
                        Path.Combine(
                            Path.GetDirectoryName(target),
                            Path.GetFileNameWithoutExtension(target) +
                                Guid.NewGuid().ToString() +
                                Path.GetExtension(target));
                    File.Copy(source, alternativeTarget);
                    return new TargetExist(source, target, alternativeTarget);
                }
                else
                {
                    File.Copy(source, target);
                    return new Success(source, target);
                }
            }
            catch (Exception exception) 
            {
                return new Failed(source, target, exception);
            }
        }
    }
}
