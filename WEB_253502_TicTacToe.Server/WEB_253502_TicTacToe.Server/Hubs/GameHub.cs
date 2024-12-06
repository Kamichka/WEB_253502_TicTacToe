using Microsoft.AspNetCore.SignalR;
using WEB_253502_TicTacToe.Server.Hubs.Interfaces;
using WEB_253502_TicTacToe.Shared;

namespace WEB_253502_TicTacToe.Server.Hubs
{
    public class GameHub: Hub<IGameClient>, IGameServer
    {
        private static readonly List<GameRoom> _rooms = new();
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.Rooms(_rooms.OrderBy(r => r.RoomName));
        }

        public async Task<GameRoom> CreateRoomAsync(string roomName, string playerName)
        {
            var roomId = Guid.NewGuid().ToString();
            var room = new GameRoom(roomId, roomName);
            _rooms.Add(room);

            var newPlayer = new Player(Context.ConnectionId, playerName);
            room.TryAddPlayer(newPlayer);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.All.Rooms(_rooms.OrderBy(r => r.RoomName));
            return room;
        }

        public async Task<GameRoom?> JoinRoom(string roomId, string playerName)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room is not null)
            {
                var newPlayer = new Player(Context.ConnectionId, playerName);
                if (room.TryAddPlayer(newPlayer))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                    await Clients.Group(roomId).OnPlayerJoined(newPlayer);
                    return room;
                }
            }
            return null;
        }

        public async Task StartGame(string roomId)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room is not null)
            {
                room.Game.StartGame();
                await Clients.Group(roomId).UpdateGame(room);
            }
        }

        public async Task MakeMove(string roomId, int row, int col, string playerId)
        {
            var room = _rooms.FirstOrDefault(r => r.RoomId == roomId);

            if (room != null && room.Game.MakeMove(row, col, playerId))
            {
                room.Game.Winner = room.Game.CheckWinner();
                room.Game.IsDraw = room.Game.CheckDraw() && string.IsNullOrEmpty(room.Game.Winner);
                if (!string.IsNullOrEmpty(room.Game.Winner) || room.Game.IsDraw)
                {
                    room.Game.IsGameOver = true;
                }

                await Clients.Group(roomId).UpdateGame(room);
            }
        }
    }
}
