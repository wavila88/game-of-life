using Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Application.UseCases;
using Domain.Models;

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
                    return BadRequest(new ApiResponseDTO<object>(false, "Initial state cannot be empty"));
                }

                var gameOfLife = new GameOfLife
                {
                    LiveCells = initialState.Select(c => new Domain.Models.Coords(c.X, c.Y)).ToList(),
                };

                var gameOfLifeResult = await _gameOfLifeUseCase.SaveGameOfLife(gameOfLife);
                //Implement Graceful Degradation in the event of failure.
                return StatusCode(201, new ApiResponseDTO<GameOfLife>(true, "Board created", gameOfLifeResult));
            }
            catch (InvalidOperationException ex)
            {
                // This returns Validation errors like "Cel is out of allowed range"
                return StatusCode(409, new ApiResponseDTO<object>(false, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in CreateNewBoard");
                //Implement Graceful Degradation in the event of failure.
                return StatusCode(500, new ApiResponseDTO<object>(false, "An unexpected error occurred."));
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
                    return NotFound(new ApiResponseDTO<object>(false, $"No board found with Id {boardId}."));
                }
                return Ok(new ApiResponseDTO<GameOfLife>(true, null, board));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetBoard for boardId {BoardId}", boardId);
                //Implement Graceful Degradation in the event of failure.
                return StatusCode(500, new ApiResponseDTO<object>(false, "An unexpected error occurred."));
            }
        }

        [HttpPost("boards/next")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GetNextGenerations([FromBody] NextGenerationsRequestDTO request)
        {
            try
            {
                var board = await _gameOfLifeUseCase.NextGenerations(request.BoardId, request.Generations, request.LiveCells);

                if (board == null)
                {
                    return NotFound(new ApiResponseDTO<object>(false, $"No board found with Id {request.BoardId}."));
                }
                if (board.isCycleDetected)
                {
                    return Ok(new ApiResponseDTO<GameOfLife>(true, $"Cycle detected at generation {board.Generation}.", board));
                }
                return Ok(new ApiResponseDTO<GameOfLife>(true, null, board));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, new ApiResponseDTO<object>(false, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetNextGenerations for boardId {BoardId} and generations {Generations}", request.BoardId, request.Generations);
                return StatusCode(500, new ApiResponseDTO<object>(false, "An unexpected error occurred."));
            }
        }

    }
}
