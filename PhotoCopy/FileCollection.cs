﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PhotoCopy
{
    public readonly record struct Sha256(string Value)
    {
        public Sha256(byte[] b) : this(Convert.ToHexString(b))
        { }
    }
    public readonly record struct AbsolutePath(string Value)
    {
        public static implicit operator AbsolutePath(string s) => new AbsolutePath(s);
        public static implicit operator string(AbsolutePath p) => p.Value;
    }
    public readonly record struct RelativePath(string Value)
    {
        public static implicit operator RelativePath(string s) => new RelativePath(s);
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
    }
}
