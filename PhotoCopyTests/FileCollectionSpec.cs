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
        [TestMethod]
        public void Files_to_copy_can_be_computed_from_set_of_source_and_target_hashes_with_Except()
        {
            var source = new List<Sha256> { "a", "b", "c", "c" }.ToImmutableHashSet();
            var target = new List<Sha256> { "b", "e"}.ToImmutableHashSet();

            var filesToCopy = source.Except(target);

            CollectionAssert.AreEqual(new List<Sha256> { "a", "c" }.ToImmutableHashSet(), filesToCopy);
        }
        [TestMethod]
        public void FilesToCopy_compares_the_Sha256_in_source_and_target_and_filters_out_distinct_files_which_are_only_in_source()
        {
            var sourceFileCollection = new Dictionary<AbsolutePath, (DateOnly, Sha256)>
                {
                    { @"C:/1.txt", (new DateOnly(), new Sha256("a")) },
                    { @"C:/2.txt", (new DateOnly(), new Sha256("b")) },
                    { @"C:/sub/3.jpeg", (new DateOnly(2010,7,30), new Sha256("b")) }
                }.ToImmutableDictionary();
            var targetFileCollection = new Dictionary<AbsolutePath, (DateOnly, Sha256)>
                {
                    { @"C:/1.txt", (new DateOnly(), new Sha256("a")) },
                    { @"C:/sub/3.jpeg", (new DateOnly(2010,7,30), new Sha256("c")) }
                }.ToImmutableDictionary();

            var filesToCopyCollection = FilesToCopy(sourceFileCollection, targetFileCollection);

            Assert.AreEqual(1, filesToCopyCollection.Count);
            Assert.AreEqual(new Sha256("b"), filesToCopyCollection.First().Value.Item2);
        }
        [TestMethod]
        public void A_source_target_set_can_be_created_from_files_to_copy()
        {
            var filesToCopy = new Dictionary<AbsolutePath, (DateOnly, Sha256)>
                {
                    { @"C:/source/1.txt", (new DateOnly(2011,4,21), new Sha256("a")) },
                    { @"C:/source/sub/3.jpeg", (new DateOnly(2010,7,30), new Sha256("c")) }
                }.ToImmutableDictionary();
            AbsolutePath targetRoot = "C:/target";

            var sourceTargetSet = SourceTargetSet(filesToCopy, targetRoot);

            CollectionAssert.AreEqual(
                new HashSet<(AbsolutePath, AbsolutePath)> 
                { 
                    (@"C:/source/1.txt", @"C:/target/2011/4/21/1.txt"),
                    (@"C:/source/sub/3.jpeg", @"C:/target/2010/7/30/3.jpeg")
                }.ToImmutableHashSet(), 
                sourceTargetSet);
        }
    }
}
