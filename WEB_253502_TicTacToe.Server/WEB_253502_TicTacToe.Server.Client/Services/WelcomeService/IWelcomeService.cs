using Microsoft.AspNetCore.SignalR.Client;
using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Client.Services.WelcomeService
{
    public interface IWelcomeService
    {
        public HubConnection _hubConnection { get; }
        List<GameRoom> Rooms { get; }
        Task InitializeAsync();
        GameRoom? CurrentRoom { get; }
        Task CreateRoomAsync(string roomName, string playerName);
        Task JoinRoomAsync(string roomId, string playerName);

        public void SetInvokeAsync(Func<Func<Task>, Task> invokeAsync);

        public event Func<Task>? OnRoomsUpdated;
        public event Func<Task>? OnGameUpdated;
       
    }
}
