using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Client.Services.RoomService
{
    public class RoomService:IRoomService
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<RoomService> _logger;
        public string? _playerId { get; set; }
        public GameRoom? CurrentRoom { get; private set; }
        public event Func<Task>? OnGameUpdated;
        private Func<Func<Task>, Task>? _invokeAsync;

        public RoomService(HubConnection hubConnection, ILogger<RoomService> logger)
        {
            _hubConnection = hubConnection;
            _playerId = _hubConnection.ConnectionId;

            _hubConnection.On<GameRoom>("UpdateGame", async serverRoom =>
            {
                CurrentRoom = serverRoom;
                if (OnGameUpdated != null && _invokeAsync != null)
                {
                    await _invokeAsync(() => OnGameUpdated.Invoke());
                }
            });

            _hubConnection.On<Player>("OnPlayerJoined", async player =>
            {
                CurrentRoom?.PlayerList.Add(player);
                if (OnGameUpdated != null && _invokeAsync != null)
                {
                    await _invokeAsync(() => OnGameUpdated.Invoke());
                }
            });
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _hubConnection.StartAsync();
        }

        public void SetInvokeAsync(Func<Func<Task>, Task> invokeAsync)
        {
            _invokeAsync = invokeAsync;
        }


        public async Task StartGameAsync(GameRoom CurrentRoom)
        {
            if (CurrentRoom != null)
            {
                _logger.LogInformation("Game started");
                await _hubConnection.InvokeAsync("StartGame", CurrentRoom.RoomId);
            }
        }

        public async Task MakeMoveAsync(int row, int col)
        {
            _logger.LogInformation($"Move on row {row}, col {col}");
            if (IsMyTurn() && CanMove())
            {
                await _hubConnection.InvokeAsync("MakeMove", CurrentRoom.RoomId, row, col, _playerId);
            }
        }

        public bool IsMyTurn()
        {
            if (CurrentRoom == null || CurrentRoom.Game == null)
            {
                _logger.LogWarning("IsMyTurn called but CurrentRoom or Game is null.");
                return false;
            }

            _logger.LogInformation($"Checking turn: CurrentPlayer = {CurrentRoom.Game.CurrentPlayerId}, PlayerID = {_playerId}");
            return _playerId == CurrentRoom.Game.CurrentPlayerId;
        }

        public bool CanMove()
        {
            return CurrentRoom != null && CurrentRoom.Game.IsGameStarted && !CurrentRoom.Game.IsGameOver;
        }
    }
}
