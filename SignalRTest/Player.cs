using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTest
{
    internal class Player
    {
        public string Username { get; private set; }
        public string ConnectionID { get; private set; }
        public string IpAddress { get; private set; } 
        public Player(string username, string connectionID,string ipaddress)
        {
            Username = username;
            ConnectionID = connectionID;
            IpAddress = ipaddress;
        }
    }
}
