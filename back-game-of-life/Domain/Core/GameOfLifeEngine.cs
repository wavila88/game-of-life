using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public static class GameOfLifeEngine
    {
        /// <summary>
        /// Calculates the next generation by applying` Conway's rules.
        /// Uses sparse logic for scalability.
        /// </summary>
        /// <param name="currentLiveCells">The current HashSet of live cells.</param>
        /// <returns>The new HashSet of live cells for the next generation.</returns>
        public static HashSet<Coords> CalculateNextGeneration(HashSet<Coords> currentLiveCells)
        {
            // Stores all cells that need to be examined: the live ones and their neighbors.
            var cellsToExamine = new HashSet<Coords>();

            // 1. Identify the set of candidates
            foreach (var cell in currentLiveCells)
            {
                // Add the live cell itself (since it could die)
                cellsToExamine.Add(cell);

                // Add all its neighbors (since they could be born)
                foreach (var neighbor in GetNeighbors(cell))
                {
                    cellsToExamine.Add(neighbor);
                }
            }

            var nextLiveCells = new HashSet<Coords>();

            // 2. Apply rules to each candidate
            foreach (var cell in cellsToExamine)
            {
                // Count the number of live neighbors.
                int liveNeighbors = CountLiveNeighbors(cell, currentLiveCells);

                // Check if the examined cell was alive in the current generation.
                bool isCurrentlyAlive = currentLiveCells.Contains(cell);

                // Apply Conway's 4 rules:

                if (isCurrentlyAlive)
                {
                    // Survival and Death rules
                    if (liveNeighbors == 2 || liveNeighbors == 3)
                    {
                        // Rule 3: Overpopulation (dies) or Rule 4: Isolation (dies) - FALSE!
                        // Rule 2: 2 or 3 neighbors -> Survives
                        nextLiveCells.Add(cell);
                    }
                    // If it has < 2 or > 3, it dies. Not added to nextLiveCells.
                }
                else // The examined cell is currently dead (it's in the candidate set because it's a neighbor of a live cell)
                {
                    // Birth rule
                    if (liveNeighbors == 3)
                    {
                        // Rule 1: Birth.
                        nextLiveCells.Add(cell);
                    }
                }
                var patternHash = CycleDetector.ComputePatternHash(nextLiveCells);
            }

            return nextLiveCells;
        }

        /*
         * Time Complexity: O(X * k)
         * Space Complexity: O(k)
         * Where X = number of generations, k = average number of live cells per generation.
         * If you have 100 generations (X = 100) and 50 live cells (k = 50) per generation,
         * the complexity is about O(100 * 50) = O(5000) operations.
         */
        /// <summary>
        /// Calculates the state of the board after X generations, with cycle detection.
        /// </summary>
        public static GameOfLife CalculateNGerations(HashSet<Coords> initialCells, int X)
        {
            var detector = new CycleDetector();
            var currentCells = initialCells;
            bool cycleDetected = false;
            int cycleGeneration = 0;
            for (int i = 0; i < X; i++)
            {
                if(detector.HasCycle(currentCells))
                {
                    cycleDetected = true;
                    cycleGeneration = i;
                    break;
                }
                currentCells = CalculateNextGeneration(currentCells);
            }
            return new GameOfLife()
            { 
                LiveCells = currentCells.ToList(),
                isCycleDetected = cycleDetected,
                Generation = cycleGeneration +1
            };
        }

        /*
         * Time Complexity: O(1) (since it always yields 8 neighbors)
         * Space Complexity: O(1)
         */
        /// <summary>
        /// Returns the 8 neighbors of a cell.
        /// </summary>
        private static IEnumerable<Coords> GetNeighbors(Coords cell)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++) // Outer loop
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++) // Inner loop
                {
                    if (xOffset == 0 && yOffset == 0) continue;

                    yield return new Coords(cell.X + xOffset, cell.Y + yOffset);
                }
            }
        }

        /*
         * Time Complexity: O(1) (since there are always 8 neighbors, and HashSet.Contains is O(1) average)
         * Space Complexity: O(1)
         */
        /// <summary>
        /// Counts the number of live neighbors for a given cell.
        /// </summary>
        private static int CountLiveNeighbors(Coords cell, HashSet<Coords> liveCells)
        {
            int count = 0;
            foreach (var neighbor in GetNeighbors(cell))
            {
                // Using 'Contains' on a HashSet is O(1) on average,
                // making neighbor counting very fast.
                if (liveCells.Contains(neighbor))
                {
                    count++;
                }
            }
            return count;
        }
    }
}
