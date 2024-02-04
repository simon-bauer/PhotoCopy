using System;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PhotoCopy
{
    public static class FileCollectionExtensions
    {
        public static ImmutableHashSet<Sha256> ToSha256Set(this ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> fileCollection)
        {
            return fileCollection.Select(f => f.Value.Item2).ToImmutableHashSet();
        }
    }

    public readonly record struct Sha256(string Value)
    {
        public Sha256(byte[] b) : this(Convert.ToHexString(b))
        { }
        public static implicit operator Sha256(string s) => new Sha256(s);
    }
    public static class PathHelper
    {
        public static string NormalizeSlashes(string s)
        {
            return s.Replace("\\", "/");
        }
        public static string CheckForAbsolutePath(string path)
        {
            if (Path.IsPathFullyQualified(path))
            {
                return path;
            }
            throw new ArgumentException($"Given path {path} is not an absolute path");
        }
        public static string CheckForRelativePath(string path)
        {
            if (!Path.IsPathFullyQualified(path))
            {
                return path;
            }
            throw new ArgumentException($"Given path {path} is not a relative path");
        }
    }
    public readonly record struct AbsolutePath(string Value)
    {
        public static implicit operator AbsolutePath(string s) => 
            new AbsolutePath(PathHelper.NormalizeSlashes(PathHelper.CheckForAbsolutePath(s)));
        public static implicit operator string(AbsolutePath p) => p.Value;
    }
    public readonly record struct RelativePath(string Value)
    {
        public static implicit operator RelativePath(string s) => 
            new RelativePath(PathHelper.NormalizeSlashes(PathHelper.CheckForRelativePath(s)));
        public static implicit operator string(RelativePath p) => p.Value;
    }
    public class FileCollection
    {
        public static string Serialize(ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> fileCollection)
        {
            StringBuilder stringBuilder = new ();
            foreach (var f in fileCollection)
            {
                stringBuilder
                    .Append(f.Key).Append(",")
                    .Append(f.Value.Item1).Append(",")
                    .Append(f.Value.Item2.Value).Append("\n");
            }
            return stringBuilder.ToString();
        }
        public static ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> Deserialize(string jsonString)
        {
            return jsonString.Split("\n")
                .Select(x => x.Split(","))
                .Where(x => x.Count() == 3)
                .Select(x => 
                    new KeyValuePair<AbsolutePath, (DateOnly, Sha256)>
                    (
                        x[0],
                        (DateOnly.Parse(x[1]), new Sha256(x[2]))
                    ))
                .ToImmutableDictionary();
        }
        public static ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> FilesToCopy(ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> source, ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> target)
        {
            var filesToCopy = source.ToSha256Set().Except(target.ToSha256Set());
            return source.
                Where(f => filesToCopy.Contains(f.Value.Item2)).
                DistinctBy(f => f.Value.Item2).
                ToImmutableDictionary();
        }
        public static ImmutableHashSet<(AbsolutePath, AbsolutePath)> SourceTargetSet(ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)> filesToCopy, AbsolutePath targetRoot)
        {
            return filesToCopy.Select(f => 
                (
                    f.Key,
                    (AbsolutePath)Path.Combine(targetRoot, CombineDate(f.Value.Item1, "/"), Path.GetFileName(f.Key))
                )).ToImmutableHashSet();
        }

        public static string CombineDate(DateOnly date, string delimiter)
        {
            return $"{date.Year}{delimiter}{date.Month}{delimiter}{date.Day}";
        }
    }
}
