using Microsoft.AspNetCore.SignalR.Client;
using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Client.Services.WelcomeService
{
    public class WelcomeService : IWelcomeService
    {
        public HubConnection _hubConnection { get; private set; }
        private Func<Func<Task>, Task>? _invokeAsync;
        public List<GameRoom> Rooms { get; private set; } = new();
        public GameRoom? CurrentRoom { get; private set; }
        public event Func<Task>? OnRoomsUpdated;
        public event Func<Task>? OnGameUpdated;

        public WelcomeService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            _hubConnection.On<List<GameRoom>>("Rooms", async roomList =>
            {
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
        }

        public async Task CreateRoomAsync(string roomName, string playerName)
        {
            CurrentRoom = await _hubConnection.InvokeAsync<GameRoom>("CreateRoomAsync", roomName, playerName);

            if (OnGameUpdated != null && _invokeAsync != null)
            {
                await _invokeAsync(() => OnGameUpdated.Invoke());
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
            CurrentRoom = await _hubConnection.InvokeAsync<GameRoom>("JoinRoom", roomId, playerName);
            if (OnGameUpdated != null && _invokeAsync != null)
            {
                await _invokeAsync(() => OnGameUpdated.Invoke());
            }
        }

        public void SetInvokeAsync(Func<Func<Task>, Task> invokeAsync)
        {
            _invokeAsync = invokeAsync;
        }
    }
}
