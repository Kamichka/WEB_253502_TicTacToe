using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Hubs.Interfaces
{
    public interface IGameServer
    {
        Task<GameRoom> CreateRoomAsync(string roomName, string playerName);
        Task<GameRoom?> JoinRoom(string roomId, string playerName);
        Task StartGame(string roomId);
        Task MakeMove(string roomId, int row, int col, string playerId);
    }
}
