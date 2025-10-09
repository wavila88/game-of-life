using Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Api.Controllers;
using Application.UseCases;

namespace Api.Controllers
{
        
    [ApiController]
    [Route("[controller]")]
    public class GameOfLifeController : ControllerBase
    {
        private readonly ILogger<GameOfLifeController> _logger;
        private readonly IGameOfLifeUseCase _gameOfLifeUseCase;

        public GameOfLifeController(ILogger<GameOfLifeController> logger, IGameOfLifeUseCase gameOfLifeUseCase)
        {
            _logger = logger;
            _gameOfLifeUseCase = gameOfLifeUseCase;

        }

        /// <summary>
        /// Creates a new Game of Life board with the given initial state.
        /// </summary>
        /// <param name="initialState">La list of coords to create inital state.</param>
        [HttpPost("boards")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNewBoard([FromBody] IEnumerable<CoordsDTO> initialState)
        {
            try
            {
                if (initialState == null || !initialState.Any())
                {
                    return BadRequest("Initial state cannot be empty");
                }

                var gameOfLife = new Domain.Models.GameOfLife
                {
                    LiveCells = initialState.Select(c => new Domain.Models.Coords(c.X, c.Y)).ToList(),
                };

                var gameOfLifeResult = await _gameOfLifeUseCase.SaveGameOfLife(gameOfLife);

                // returns a new object with the ID and initial state
                return StatusCode(201, gameOfLifeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in CreateNewBoard");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("boards/{boardId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBoard(Guid boardId)
        {
            try
            {
                var board = await _gameOfLifeUseCase.GetState(boardId);
                if (board == null)
                {
                    return NotFound($"No board found with Id {boardId}.");
                }
                return Ok(board);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetBoard for boardId {BoardId}", boardId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("boards/{boardId}/next/{generations}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GetNextGenerations(Guid boardId, int generations)
        {
            try
            {
                var board = await _gameOfLifeUseCase.NextGenerations(boardId, generations);
                if (board == null)
                    return NotFound($"No board found with Id {boardId}.");
                return Ok(board);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetNextGenerations for boardId {BoardId} and generations {Generations}", boardId, generations);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    }
}
