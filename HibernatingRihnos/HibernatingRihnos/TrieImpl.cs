using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HibernatingRihnos
{
    public class TrieImpl : ITrie
    {
        private const int KILO = 1000;
        private const int BYTE_ARR_SIZE = 32 * KILO;
        private const int BLOCK_SIZE = sizeof(long);
        private static byte[] data = new byte[BYTE_ARR_SIZE];

        public void Delete(string key)
        {
            long indexOfData;
            int metaIndexOfData;
            int rootInd = trySearchTrieGetIndexOfKeyData(key,out indexOfData, out metaIndexOfData);
            if (rootInd != -1)
            {
                long deletionNum = 0;
                byte[] arr = BitConverter.GetBytes(deletionNum);
                Buffer.BlockCopy(arr, 0, data, (rootInd - (int)indexOfData) * BLOCK_SIZE, BLOCK_SIZE);
                tryRemoveKeyIfLast(key, rootInd ,(int)indexOfData, metaIndexOfData);
            }
        }

        private void tryRemoveKeyIfLast(string key, int rootInd , int indexOfData, int metaIndexOfData)
        {
            if (BitConverter.ToInt64(data, (rootInd - indexOfData + 1) * BLOCK_SIZE) == 0) //if the number of children of node is 0
            {
                byte[] arr = BitConverter.GetBytes((long)0);
                Buffer.BlockCopy(arr, 0, data, metaIndexOfData * BLOCK_SIZE , BLOCK_SIZE); //remove the offset to next child that has been removed 
                Buffer.BlockCopy(arr, 0, data, (metaIndexOfData - 1) * BLOCK_SIZE, BLOCK_SIZE); // remove the key character
            }
        }

        public void Load(string filename)
        {
            data = File.ReadAllBytes(filename);
        }

        public void Save(string filename)
        {
            File.WriteAllBytes(filename, data);
        }

        public bool TryRead(string key, out long value)
        {
            value = -1;
            long indexOfData;
            int metaIndexOfData;
            int rootInd = trySearchTrieGetIndexOfKeyData(key,  out indexOfData, out metaIndexOfData);
            if (rootInd != -1)
            {
                rootInd = rootInd - (int)indexOfData;
                value = BitConverter.ToInt64(data, rootInd * BLOCK_SIZE);
            }
            return (rootInd != -1);
        }

        private bool traverseTrie(ref int rootInd, ref long indexOfData, ref int metaIndexOfData, byte ch)
        {
            int lengthOfChildNodes = (int)BitConverter.ToInt64(data, (rootInd + 1) * BLOCK_SIZE);
            bool isNodeValue = true;
            bool isFound = false;
            metaIndexOfData = 0;
            char valueInData;

            indexOfData = 0;
            for (int i = 2; i <= lengthOfChildNodes + 1; i++)
            {

                if (isNodeValue)
                {
                    valueInData = BitConverter.ToChar(data, (i + rootInd) * BLOCK_SIZE);
                    if (valueInData == ch)
                    {
                        isFound = true;
                        metaIndexOfData = rootInd + i + 1;
                        break;
                    }
                }
                isNodeValue = !isNodeValue;
            }

            if (isFound)
                indexOfData = BitConverter.ToInt64(data, metaIndexOfData * BLOCK_SIZE);
            else
                rootInd = -1;

            return isFound;
        }

        private int trySearchTrieGetIndexOfKeyData(string key, out long indexOfData,out int metaIndexOfData)
        {
            int rootInd = getRootInd();
            byte[] keyAsBytes = Encoding.UTF8.GetBytes(key);
            Boolean isFound = false;
            metaIndexOfData = 0;
            indexOfData = 0;
            for (int i = 0; i < key.Length; i++)
            {
                byte ch = keyAsBytes[i];
                isFound = traverseTrie(ref rootInd, ref indexOfData, ref metaIndexOfData, ch);
                if (i != key.Length - 1)
                    rootInd -= (int)indexOfData;
                if (!isFound)
                    break;
            }

            return rootInd;
        }

        public bool TryWrite(string key, long value)
        {
            throw new NotImplementedException();
        }

        private int getRootInd()
        {
            return (int)BitConverter.ToInt64(data, 0);
        }
    }
}
