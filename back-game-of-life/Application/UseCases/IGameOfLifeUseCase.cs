using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IGameOfLifeUseCase
    {
        /// <summary>
        /// Saves a new GameOfLife board. Throws if the Id already exists.
        /// </summary>
        public Task<GameOfLife> SaveGameOfLife(GameOfLife gameOfLife);

        /// <summary>
        /// Gets the state of a GameOfLife board by Id.
        /// </summary>
        public Task<GameOfLife?> GetState(Guid id);

        /// <summary>
        /// Gets the state of a GameOfLife board after a number of generations.
        /// </summary>
        public Task<GameOfLife> NextGenerations(Guid id, int generations, List<Coords> LiveCells);
    }
}
