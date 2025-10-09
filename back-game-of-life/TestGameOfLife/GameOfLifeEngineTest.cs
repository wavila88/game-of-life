using Domain.Core;
using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public sealed class GameOfLifeEngineTest
    {

        [TestMethod]
        public void EmptyGrid_ShouldRemainEmpty()
        {
            // ARRANGE: An empty grid (no live cells)
            var initialCells = new HashSet<Coords>();
            // ACT: Calculate the next generation
            var nextGeneration = GameOfLifeEngine.CalculateNextGeneration(initialCells);
            // ASSERT: The next generation should also be empty
            Assert.AreEqual(0, nextGeneration.Count, "The grid should remain empty.");
        }

        [TestMethod]
        public void SingleCell_ShouldDie()
        {
            // Initial state:
            // [X]
            // After one generation (should die):
            // [ ]
            // ARRANGE: A single live cell at (0,0)
            var initialCells = new HashSet<Coords> { new Coords(0, 0) };
            // ACT: Calculate the next generation
            var nextGeneration = GameOfLifeEngine.CalculateNextGeneration(initialCells);
            // ASSERT: The single cell should die due to underpopulation
            Assert.AreEqual(0, nextGeneration.Count, "The single cell should die.");
        }

        [TestMethod]
        public void TwoAdjacentCells_ShouldDie()
        {
            // Initial state:
            // [X] [X]
            // After one generation (both should die):
            // [ ] [ ]
            // ARRANGE: Two adjacent live cells at (0,0) and (0,1)
            var initialCells = new HashSet<Coords>
            {
                new Coords(0, 0),
                new Coords(0, 1)
            };
            // ACT: Calculate the next generation
            var nextGeneration = GameOfLifeEngine.CalculateNextGeneration(initialCells);
            // ASSERT: Both cells should die due to underpopulation
            Assert.AreEqual(0, nextGeneration.Count, "Both cells should die.");
        }

        [TestMethod]
        public void Blinker_ShouldOscillateCorrectly()
        {
            // Initial state: The Blinker in its horizontal state
            // [ ] [ ] [ ]
            // [ ] [X] [ ]  <- (1, 1)
            // [ ] [X] [ ]  <- (1, 2)
            // [ ] [X] [ ]  <- (1, 0)
            // [ ] [ ] [ ]
            var initialCells = new HashSet<Coords>
                {
                    new Coords(1, 0),
                    new Coords(1, 1),
                    new Coords(1, 2)
                };

        
            var nextGeneration = GameOfLifeEngine.CalculateNextGeneration(initialCells);

            // ASSERT: The Blinker should flip to its vertical state (Period 2 Oscillation)
            // The test validates the application of Conway's 4 rules:
            //
            //       (0, 1) [X] (1, 1) [X] (2, 1) [X]
            //
            // Expected next state (vertical Blinker):
            //   [ ] [ ] [ ]
            //   [X] [X] [X]
            //   [ ] [ ] [ ]
            //   (coords: (0,1), (1,1), (2,1))
            var expectedCells = new HashSet<Coords>
            {
                new Coords(0, 1), // <-- BORN (Rule: Dead with exactly 3 neighbors)
                new Coords(1, 1), // <-- SURVIVES (Rule: Alive with 2 or 3 neighbors)
                new Coords(2, 1)  // <-- BORN (Rule: Dead with exactly 3 neighbors)
            };

            // 1. Validate the set size
            // If the size does not match, some cell survived or died incorrectly.
            Assert.AreEqual(expectedCells.Count, nextGeneration.Count);

            // 2. Validate that the content is exactly as expected (SetEquals handles order)
            // - (1, 0) and (1, 2) should have died by Isolation (only 1 neighbor).
            // - (0, 1) and (2, 1) should have been born (3 live neighbors).
            // - (1, 1) should have survived (2 live neighbors).
            Assert.IsTrue(expectedCells.SetEquals(nextGeneration), "The pattern did not oscillate correctly.");
        }

        [TestMethod]
        public void Block_ShouldRemainStable()
        {
            // Initial and expected state (Block):
            // [ ] [ ] [ ] [ ]
            // [ ] [X] [X] [ ]
            // [ ] [X] [X] [ ]
            // [ ] [ ] [ ] [ ]
            var initialCells = new HashSet<Coords>
            {
                new Coords(1, 1), new Coords(2, 1),
                new Coords(1, 2), new Coords(2, 2)
            };

            // ACT
            var nextGeneration = GameOfLifeEngine.CalculateNextGeneration(initialCells);

            // ASSERT: Stability validation (Period 1 pattern)
            // All 4 cells have exactly 3 neighbors.
            // Rule: Alive with 2 or 3 neighbors -> SURVIVES.
            Assert.IsTrue(initialCells.SetEquals(nextGeneration), "El Bloque no permaneció estable.");
        }

        [TestMethod]
        public void Glider_ShouldMoveDiagonally()
        {
            // Initial state: Glider (top-left corner)
            // [ ] [X] [ ]
            // [ ] [ ] [X]
            // [X] [X] [X]
            var initialCells = new HashSet<Coords>
            {
                new Coords(1, 0),
                new Coords(2, 1),
                new Coords(0, 2),
                new Coords(1, 2),
                new Coords(2, 2)
            };

            // ACT: Advance 4 generations (a glider returns to its shape shifted diagonally)
           var nextGen = GameOfLifeEngine.CalculateNGerations(initialCells, 4);

            // Expected state after 4 generations (shifted diagonally)
            // [ ] [ ] [ ]
            //     [ ] [X] [ ]  <- (2,1)
            //     [ ] [ ] [X]  <- (3,2)
            //     [X] [X] [X]  <- (1,3), (2, 3), (3, 3)
            var expectedCells = new HashSet<Coords>
            {
                new Coords(2, 1),
                new Coords(3, 2),
                new Coords(1, 3),
                new Coords(2, 3),
                new Coords(3, 3)
            };

            // Assert: The glider has moved diagonally
            Assert.AreEqual(expectedCells.Count, nextGen.Count);
            Assert.IsTrue(expectedCells.SetEquals(nextGen), "The glider did not move as expected.");
        }

        [TestMethod]
        public void Toad_ShouldOscillateCorrectly()
        {
            // ARRANGE: Initial state of the Toad (6 live cells)
            //
            // G0 (Initial State):
            // [ ] [ ] [ ] [ ] [ ] [ ]
            // [ ] [ ] [X] [X] [X] [ ]  <- Cells (2,1), (3,1), (4,1)
            // [ ] [X] [X] [X] [ ] [ ]  <- Cells (1,2), (2,2), (3,2)
            // [ ] [ ] [ ] [ ] [ ] [ ]
            //
            var initialCells = new HashSet<Coords>
            {
                // Vivas en G0: (2,1), (3,1), (4,1), (1,2), (2,2), (3,2)
                new Coords(2, 1), new Coords(3, 1), new Coords(4, 1),
                new Coords(1, 2), new Coords(2, 2), new Coords(3, 2)
            };

            // ACT: Advance one generation using the core engine
            var nextGeneration = GameOfLifeEngine.CalculateNextGeneration(initialCells);

            // ACT: Expected next state of the Toad (6 live cells)
            //
            // G1 (Next State):
            // [ ] [ ] [ ] [X] [ ] [ ]
            // [ ] [X] [ ] [ ] [X] [ ]
            // [ ] [X] [ ] [ ] [X] [ ]
            // [ ] [ ] [X] [ ] [ ] [ ]
            //
            var expectedCells = new HashSet<Coords>
            {
                // Coordenadas que deben ser producidas por G0 -> G1
                new Coords(3, 0),
                new Coords(1, 1), new Coords(4, 1),
                new Coords(1, 2), new Coords(4, 2),
                new Coords(2, 3)
            };

            // Verify: The total live cell count must be 6 (4 born + 2 survivors).
            Assert.AreEqual(6, nextGeneration.Count, "El número de células vivas debe ser 6. 4 mueren (superpoblación) y 4 nacen.");

            // Final Check: Verify that the actual set matches the expected set of coordinates.
            Assert.IsTrue(expectedCells.SetEquals(nextGeneration), "El patrón del Toad no se transformó correctamente a G1.");
        }
    }
}
