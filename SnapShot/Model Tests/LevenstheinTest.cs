using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnapShot;
using SnapShot.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model_Tests
{
    [TestClass]
    public class LevenstheinTest
    {
      
        [TestMethod]
        public void OneLetterDifferenceCheckTest()
        {
            Assert.AreEqual("b", Levenshtein.FindDifferences("aa", "aab"));
        }

        [TestMethod]
        public void NoDifferenceCheckTest()
        {
            Assert.AreEqual("", Levenshtein.FindDifferences("", ""));
        }
        [TestMethod]
        public void TwoLettersDifferenceCheckTest()
        {
            Assert.AreEqual("bb", Levenshtein.FindDifferences("aa", "abab"));
        }
        [TestMethod]
        public void StartDifferenceCheckTest()
        {
            Assert.AreEqual("bb", Levenshtein.FindDifferences("aab", "bbaab"));
        }

        [TestMethod]
        public void IgnoreDeletedLetterTest()
        {
            Assert.AreEqual("cabcd", Levenshtein.FindDifferences("DOBAR DAN", "DOcAR DabcdA"));
        }

        [TestMethod]
        public void IgnoreDeletedLettersTest()
        {
            Assert.AreEqual("hosic", Levenshtein.FindDifferences("Nejra", "jrahosic"));
        }
        [TestMethod]
        public void BigDifferenceCheck()
        {
            Assert.AreEqual("aabbccddeeff", Levenshtein.FindDifferences("aabbccddeeff", "aaaabbbbccccddddeeeeffff"));
        }
        [TestMethod]
        public void BetweenCheck()
        {
            Assert.AreEqual("xxyyy", Levenshtein.FindDifferences("probaj", "pxrxoybyayj"));
        }
    }
}