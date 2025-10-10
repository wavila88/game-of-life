using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGameOfLife
{
    [TestClass]
    public sealed class CycleDetectorGameOfLiveTest
    {
        [TestMethod]
        public void DetectsCycleInBlinkerPattern()
        {
            // ARRANGE: Initialize the CycleDetector and the Blinker pattern
            var cycleDetector = new Domain.Core.CycleDetector();
            var blinkerPattern1 = new List<Domain.Models.Coords>
            {
                new Domain.Models.Coords(0, 1),
                new Domain.Models.Coords(1, 1),
                new Domain.Models.Coords(2, 1)
            };
            var blinkerPattern2 = new List<Domain.Models.Coords>
            {
                new Domain.Models.Coords(1, 0),
                new Domain.Models.Coords(1, 1),
                new Domain.Models.Coords(1, 2)
            };
            // ACT & ASSERT: The first time we see the pattern, it should not detect a cycle
            bool hasCycleFirstCheck = cycleDetector.HasCycle(blinkerPattern1);
            Assert.IsFalse(hasCycleFirstCheck, "The first occurrence of the Blinker pattern should not be detected as a cycle.");
            // The second time we see the same pattern, it should detect a cycle
            bool hasCycleSecondCheck = cycleDetector.HasCycle(blinkerPattern1);
            Assert.IsTrue(hasCycleSecondCheck, "The second occurrence of the same Blinker pattern should be detected as a cycle.");
            // Check with the alternate state of the Blinker pattern
            bool hasCycleThirdCheck = cycleDetector.HasCycle(blinkerPattern2);
            Assert.IsFalse(hasCycleThirdCheck, "The first occurrence of the alternate Blinker pattern should not be detected as a cycle.");
            // The second time we see the alternate pattern, it should detect a cycle
            bool hasCycleFourthCheck = cycleDetector.HasCycle(blinkerPattern2);
            Assert.IsTrue(hasCycleFourthCheck, "The second occurrence of the alternate Blinker pattern should be detected as a cycle.");
        }


        [TestMethod]
        public void TestComputePatternHashConsistency()
        {
            // ARRANGE: Two identical patterns
            var patternA = new List<Domain.Models.Coords>
            {
                new Domain.Models.Coords(0, 0),
                new Domain.Models.Coords(1, 1),
                new Domain.Models.Coords(2, 2)
            };
            var patternB = new List<Domain.Models.Coords>
            {
                new Domain.Models.Coords(2, 2),
                new Domain.Models.Coords(0, 0),
                new Domain.Models.Coords(1, 1)
            };
            // ACT: Compute hashes for both patterns
            var hashA = Domain.Core.CycleDetector.ComputePatternHash(patternA);
            var hashB = Domain.Core.CycleDetector.ComputePatternHash(patternB);
            // ASSERT: The hashes should be identical since the patterns are the same
            Assert.AreEqual(hashA, hashB, "Hashes for identical patterns should match.");
        }
    }
}
