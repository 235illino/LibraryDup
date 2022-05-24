using LibraryDup;
namespace TestLibraryDup
{
    [TestClass]
    public class LibraryDupTests
    {
        [TestMethod]
        public void TestCollectCandidates_Default()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CollectCandidates(@"C:\Users\ichvyr\Documents\test");
            IReadOnlyCollection<IDuplicate> listResult = new List<IDuplicate>() {new Duplicate(new List<string>() {
                                                                 @"C:\Users\ichvyr\Documents\test\TesT.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\test.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\testin\qqq\tesT.txt"}),
                                                                 new Duplicate(new List<string>() {
                                                                 @"C:\Users\ichvyr\Documents\test\test\test3.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\testin\qqq\test3.txt"}) };
            Assert.AreEqual(listActual.Count, 2);
            for (int i = 0; i < listActual.Count; i++)
            {
                Assert.IsTrue(listActual.ElementAt(i).Filepaths.SequenceEqual(listResult.ElementAt(i).Filepaths));
            }
        }

        [TestMethod]
        public void TestCollectCandidates_WithParam_Size()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CollectCandidates(@"C:\Users\ichvyr\Documents\test", ComparisonMode.Size);
            IReadOnlyCollection<IDuplicate> listResult = new List<IDuplicate>() {new Duplicate(new List<string>() {
                                                                 @"C:\Users\ichvyr\Documents\test\TesT.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\test.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\test2.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\testin\qqq\tesT.txt"}),
                                                                 new Duplicate(new List<string>() {
                                                                 @"C:\Users\ichvyr\Documents\test\test\test3.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\testin\qqq\test3.txt"}) };

            Assert.AreEqual(listActual.Count, 2);
            for (int i = 0; i < listActual.Count; i++)
            {
                Assert.IsTrue(listActual.ElementAt(i).Filepaths.SequenceEqual(listResult.ElementAt(i).Filepaths));
            }
        }

        [TestMethod]
        public void TestCollectCandidates_WithParam_SizeAndName()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CollectCandidates(@"C:\Users\ichvyr\Documents\test", ComparisonMode.SizeAndName);
            IReadOnlyCollection<IDuplicate> listResult = new List<IDuplicate>() {new Duplicate(new List<string>() {
                                                                 @"C:\Users\ichvyr\Documents\test\TesT.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\test.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\testin\qqq\tesT.txt"}),
                                                                 new Duplicate(new List<string>() {
                                                                 @"C:\Users\ichvyr\Documents\test\test\test3.txt",
                                                                 @"C:\Users\ichvyr\Documents\test\test\testin\qqq\test3.txt"}) };
            Assert.AreEqual(listActual.Count, 2);
            for (int i = 0; i < listActual.Count; i++)
            {
                Assert.IsTrue(listActual.ElementAt(i).Filepaths.SequenceEqual(listResult.ElementAt(i).Filepaths));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Path does not exist!")]
        public void TestCollectCandidates_WithWrongParam()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CollectCandidates("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Empty Argument!")]
        public void TestCollectCandidates_WithNullParam()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CollectCandidates(null);
        }

        [TestMethod]
        public void CheckCandidates_in_5candidates_out_3()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> candidates = duplicate.CollectCandidates(@"C:\Users\ichvyr\Documents\test");
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CheckCandidates(candidates);
            IReadOnlyCollection<IDuplicate> listResult = new List<IDuplicate>() {new Duplicate(new List<string>() {
                                                                                     @"C:\Users\ichvyr\Documents\test\test\test.txt",
                                                                                     @"C:\Users\ichvyr\Documents\test\test\testin\qqq\tesT.txt"}) };
            Assert.AreEqual(listActual.Count, 1);
            for (int i = 0; i < listActual.Count; i++)
            {
                Assert.IsTrue(listActual.ElementAt(i).Filepaths.SequenceEqual(listResult.ElementAt(i).Filepaths));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Empty candidates!")]
        public void CheckCandidates_NullParam()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection<IDuplicate> listActual = duplicate.CheckCandidates(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Empty candidates!")]
        public void CheckCandidates_Param_Empty_Collection()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            IReadOnlyCollection <IDuplicate> listActual = duplicate.CheckCandidates(new List<IDuplicate>());
        }

    }
}