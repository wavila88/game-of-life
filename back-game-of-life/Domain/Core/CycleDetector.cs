using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class CycleDetector
    {
        private readonly HashSet<string> _seenPatterns = new();

        public bool HasCycle(IEnumerable<Coords> liveCells)
        {
            var hash = ComputePatternHash(liveCells);
            if (_seenPatterns.Contains(hash))
                return true;

            _seenPatterns.Add(hash);
            return false;
        }

        /// <summary>
        /// Take the list of live cells and compute a hash to identify the pattern.
        /// </summary>
        public static string ComputePatternHash(IEnumerable<Coords> liveCells)
        {
            var ordered = liveCells
                .OrderBy(c => c.X)
                .ThenBy(c => c.Y)
                .Select(c => $"{c.X},{c.Y}")
                .ToArray();

            var pattern = string.Join(";", ordered);

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(pattern);
            var hashBytes = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hashBytes);
        }


    }
}
