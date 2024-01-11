using PhotoCopy;
using System.Collections.Immutable;
using static PhotoCopy.FileCollection;

namespace PhotoCopySpec
{
    [TestClass]
    public class FileCollectionSpec
    {
        [TestMethod]
        public void A_FileCollection_can_be_serialized_and_deserialized_to_and_from_string()
        {
            var fileCollection = new Dictionary<AbsolutePath, (DateOnly, Sha256)>
                {
                    { @"C:/1.txt", (new DateOnly(), new Sha256("abcdef0123456789")) },
                    { @"C:/sub/3.jpeg", (new DateOnly(2010,7,30), new Sha256("a879b8789398c87f")) }
                }.ToImmutableDictionary();

            string serialization = Serialize(fileCollection);
            var deserializedFileCollection = Deserialize(serialization);

            Console.WriteLine(serialization);
            CollectionAssert.AreEquivalent(fileCollection, deserializedFileCollection);
        }
    }
}
