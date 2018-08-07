using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HibernatingRihnos;
using System.IO;

namespace UnitTestProject
{
    [TestClass]
    public class TrieTests
    {
        private const string PATH_TO_DATA_FILE = "../../../HibernatingRihnos/resources/DataFile.bin";

        [TestMethod]
        public void validateRead()
        {
            TrieImpl trie = new TrieImpl();
            long val;
            trie.Load(PATH_TO_DATA_FILE);
            trie.TryRead("ab", out val);

            Assert.AreEqual(3, val);
        }

        [TestMethod]
        public void validateReadWriteFromFile()
        {
            long[] arr = new long[] { 13, 10, 0, 3, 0, 13, 4, (long)'a', 4, (long)'b', 2, 7, 0, 20, 4, (long)'a', 8, (long)'b', 2 };
            byte[] bytes = new byte[arr.Length * sizeof(long)];
            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
            File.WriteAllBytes(PATH_TO_DATA_FILE, bytes);
            bytes = File.ReadAllBytes(PATH_TO_DATA_FILE);
            Assert.AreEqual(arr.Length * 8, bytes.Length);
        }

        [TestMethod]
        public void validateDeletion()
        {
            TrieImpl trie = new TrieImpl();
            long val;
            trie.Load(PATH_TO_DATA_FILE);
            trie.Delete("ab");
            trie.Save(PATH_TO_DATA_FILE);
            byte[] bytes = File.ReadAllBytes(PATH_TO_DATA_FILE);
            Assert.IsFalse(trie.TryRead("ab", out val));
        }
    }
}
