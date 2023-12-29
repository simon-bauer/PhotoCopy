
using System;

namespace PhotoCopy
{
    public readonly record struct Sha256(string Value)
    {
        public static implicit operator string(Sha256 s)
        {
            return s.Value;
        }
        public static implicit operator Sha256(string s)
        {
            return new Sha256(s);
        }
        public static implicit operator Sha256(byte[] b)
        {
            return new Sha256(Convert.ToHexString(b));
        }
    }
    //using AbsolutePath = String;
    //using FileDictionary = ImmutableDictionary<AbsolutePath, (DateOnly, Sha256)>;
}
