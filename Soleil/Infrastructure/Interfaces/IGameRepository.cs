using Soleil.Models.DTOs.Game;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Interfaces;

public interface IGameRepository
{
    Task<GameSuggestionDto?> GetSuggestedGameAsync(int childId);
    Task<GameSession> StartSessionAsync(int childId, int gameId);
    Task<SessionStatusDto> CompleteSessionAsync(int sessionId, int durationInSeconds);
    Task<SessionStatusDto> GetSessionStatusAsync(int childId);
    Task<bool> CheckReTestRequiredAsync(int childId);
}