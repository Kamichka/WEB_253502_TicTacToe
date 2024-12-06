using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Client.Services.RoomService
{
    public interface IRoomService
    {
        Task InitializeAsync();
        GameRoom? CurrentRoom { get; }
        public string? _playerId { get; }
        Task StartGameAsync(GameRoom CurrentRoom);
        Task MakeMoveAsync(int row, int col);
        public void SetInvokeAsync(Func<Func<Task>, Task> invokeAsync);
        bool IsMyTurn();
        bool CanMove();
        public event Func<Task>? OnGameUpdated;
    }
}
