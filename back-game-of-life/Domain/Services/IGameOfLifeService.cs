using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IGameOfLifeService
    {
        /// <summary>
        /// Saves a new GameOfLife board. Throws if the Id already exists.
        /// </summary>
        Task<GameOfLife> SaveGameOfLife(GameOfLife gameOfLife);

        Task<GameOfLife> GetNGerationsStateAsync(Guid boardId, int x, List<Coords> LiveCells);
    }
}
