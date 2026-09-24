namespace Rabisco.Core
{
    /// <summary>
    /// Marks a class as a game service.
    /// All services registered in ServiceLocator must implement this interface.
    /// Used as a generic constraint: where T : IGameService.
    /// </summary>
    public interface IGameService
    {
    }
}
