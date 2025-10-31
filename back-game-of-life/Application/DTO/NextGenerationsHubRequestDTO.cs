using Domain.Models;

namespace Api.DTO
{
    public class NextGenerationsHubRequestDTO
    {
        public string BoardId { get; set; }
        public int Generations { get; set; }
        public List<Coords> LiveCells { get; set; } = new List<Coords>();
    }
}
