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
        /// Calculates the next generation by applying Conway's rules.
        /// Uses sparse logic (O(k)) for scalability.
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
            }

            return nextLiveCells;
        }


        public static HashSet<Coords> CalculateNGerations(HashSet<Coords> initialCells, int X)
        {
            var currentCells = initialCells;
            for (int i = 0; i < X; i++)
            {
                // Llama repetidamente al método núcleo.
                currentCells = CalculateNextGeneration(currentCells);
            }
            return currentCells;
        }


        // get the 8 Neighbors
        private static IEnumerable<Coords> GetNeighbors(Coords cell)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++) // Bucle exterior
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++) // Bucle interior
                {
                    if (xOffset == 0 && yOffset == 0) continue;

                    // EL PUNTO DE CONTROL CLAVE: Asegúrate que X y Y están correctos
                    yield return new Coords(cell.X + xOffset, cell.Y + yOffset);
                }
            }
        }

        // Método auxiliar para contar vecinos vivos (clave para la eficiencia)
        private static int CountLiveNeighbors(Coords cell, HashSet<Coords> liveCells)
    {
        int count = 0;
        foreach (var neighbor in GetNeighbors(cell))
        {
            // Usar 'Contains' en un HashSet es una operación O(1) promedio,
            // lo que hace que el conteo de vecinos sea muy rápido.
            if (liveCells.Contains(neighbor))
            {
                count++;
            }
        }
        return count;
    }
    }
}
