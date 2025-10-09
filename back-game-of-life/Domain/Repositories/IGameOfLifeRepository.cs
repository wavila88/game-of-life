using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    /*
     * Here we define the interfaz is called also output port
     * 
     * on Infra.Messaging we are going to implement thsi interface is called output adapter
     */
    public interface IGameOfLifeRepository
    {
        /// <summary>
        /// Saves a new GameOfLife board. Throws if the Id already exists.
        /// </summary>
        Task<GameOfLife> SaveGameOfLife(Domain.Models.GameOfLife gameOfLife);

        /// <summary>
        /// Gets the state of a GameOfLife board by Id.
        /// </summary>
        Task<GameOfLife?> GetState(Guid id);

        /// <summary>
        /// Updates an existing GameOfLife board. Throws if the Id does not exist.
        /// </summary>
        Task<GameOfLife?> Update(GameOfLife gameOfLife);
    }
}
