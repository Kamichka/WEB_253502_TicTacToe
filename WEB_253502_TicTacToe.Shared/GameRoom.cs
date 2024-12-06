using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB_253502_TicTacToe.Shared
{
    public class GameRoom
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public List<Player> PlayerList { get; set; } = new();
        public Game Game { get; set; } = new();

        public GameRoom() { }
        public GameRoom(string roomId, string roomName)
        {
            RoomId = roomId;
            RoomName = roomName;
        }

        public bool TryAddPlayer(Player newPlayer)
        {
            if (PlayerList.Count < 2 && !PlayerList.Any(p => p.ConnectionId == newPlayer.ConnectionId))
            {
                PlayerList.Add(newPlayer);

                if (PlayerList.Count == 1)
                {
                    Game.PlayerXId = newPlayer.ConnectionId;
                }
                else if (PlayerList.Count == 2)
                {
                    Game.PlayerOId = newPlayer.ConnectionId;
                }

                return true;
            }

            return false;
        }
    }
}
