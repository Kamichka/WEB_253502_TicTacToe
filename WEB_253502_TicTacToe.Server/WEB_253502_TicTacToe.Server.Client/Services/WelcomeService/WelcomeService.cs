using Microsoft.AspNetCore.SignalR.Client;
using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Client.Services.WelcomeService
{
    public class WelcomeService : IWelcomeService
    {
        public HubConnection _hubConnection { get; private set; }
        private readonly ILogger<WelcomeService> _logger;
        private Func<Func<Task>, Task>? _invokeAsync;
        public List<GameRoom> Rooms { get; private set; } = new();
        public GameRoom? CurrentRoom { get; private set; }
        public event Func<Task>? OnRoomsUpdated;
        public event Func<Task>? OnGameUpdated;

        public WelcomeService(HubConnection hubConnection, ILogger<WelcomeService> logger)
        {
            _hubConnection = hubConnection;
            _hubConnection.On<List<GameRoom>>("Rooms", async roomList =>
            {
                _logger.LogInformation("Received room list update: {RoomCount} rooms.", roomList.Count);
                Rooms = roomList;
                if (OnRoomsUpdated != null && _invokeAsync != null)
                {
                    await _invokeAsync(() => OnRoomsUpdated.Invoke());
                }
            });

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

        public async Task CreateRoomAsync(string roomName, string playerName)
        {
            try
            {
                _logger.LogInformation("Attempting to create room '{RoomName}' with player '{PlayerName}'.", roomName, playerName);

                CurrentRoom = await _hubConnection.InvokeAsync<GameRoom>("CreateRoomAsync", roomName, playerName);

                if (OnGameUpdated != null && _invokeAsync != null)
                {
                    await _invokeAsync(() => OnGameUpdated.Invoke());
                }
                _logger.LogInformation("Room created successfully. CurrentRoom: {@CurrentRoom}", CurrentRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating room '{RoomName}' with player '{PlayerName}'.", roomName, playerName);
                throw;
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task JoinRoomAsync(string roomId, string playerName)
        {
            try
            {
                _logger.LogInformation("Attempting to join room '{RoomId}' with player '{PlayerName}'.", roomId, playerName);

                CurrentRoom = await _hubConnection.InvokeAsync<GameRoom>("JoinRoom", roomId, playerName);
                if (OnGameUpdated != null && _invokeAsync != null)
                {
                    await _invokeAsync(() => OnGameUpdated.Invoke());
                }
                _logger.LogInformation("Joined room successfully. CurrentRoom: {@CurrentRoom}", CurrentRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while joining room '{RoomId}' with player '{PlayerName}'.", roomId, playerName);
                throw;
            }
        }

        public void SetInvokeAsync(Func<Func<Task>, Task> invokeAsync)
        {
            _invokeAsync = invokeAsync;
        }
    }
}
