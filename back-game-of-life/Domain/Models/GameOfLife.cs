using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GameOfLife
    {
        // Requirement: Unique ID for the board
        public Guid Id { get; set; }

        // Requirement: The current state of the board (sparse list of live cells)
        // List<T> or IEnumerable<T> is used to facilitate JSON serialization (Redis)
        public List<Coords> LiveCells { get; set; } = new List<Coords>();

        // Requirement: Track the current generation
        public int Generation { get; set; }

        // Requirement: Pattern fingerprint, essential for cycle detection
        public string PatternHash { get; set; } = string.Empty;

        //Require for cycle detection
        public bool isCycleDetected { get; set; } = false;
    }

    public record struct Coords(int X, int Y);
}
