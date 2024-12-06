using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WEB_253502_TicTacToe.Shared
{
    public class Player
    {
        public Player(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
        }

        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
}
