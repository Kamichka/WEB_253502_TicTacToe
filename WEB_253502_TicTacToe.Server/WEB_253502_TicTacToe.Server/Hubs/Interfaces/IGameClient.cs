using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Hubs.Interfaces
{
    public interface IGameClient
    {
        Task Rooms(IEnumerable<GameRoom> rooms); // Событие отправки списка комнат
        Task OnPlayerJoined(Player player); // Событие добавления игрока в комнату
        Task UpdateGame(GameRoom gameRoom); // Событие обновления игры
    }
}
