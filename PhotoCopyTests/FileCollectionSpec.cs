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
        [TestMethod]
        public void An_absolute_path_created_by_implicit_constructor_normalizes_all_slashed_to_forward_slashes()
        {
            AbsolutePath p = "C:\\abc\\def.txt";
            Assert.AreEqual("C:/abc/def.txt", (string)p);
        }
        [TestMethod]
        public void An_absolute_path_created_by_implicit_constructor_throws_if_given_path_is_not_absolute()
        {
            Assert.ThrowsException<ArgumentException>(() => (AbsolutePath)"abc\\def.txt");
        }
        [TestMethod]
        public void An_relative_path_created_by_implicit_constructor_normalizes_all_slashed_to_forward_slashes()
        {
            RelativePath p = "abc\\def.txt";
            Assert.AreEqual("abc/def.txt", (string)p);
        }
        [TestMethod]
        public void A_relative_path_created_by_implicit_constructor_throws_if_given_path_is_not_relative()
        {
            Assert.ThrowsException<ArgumentException>(() => (RelativePath)"C:\\abc\\def.txt");
        }
    }
}
