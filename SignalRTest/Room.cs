using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTest
{
    internal class Room
    {
        private static int IDbuilder = 0;
        public int ID { get; private set; }
        public string Name { get; set; }  
        public Player Host { get; set; }
        public Player Guest { get; set; }
        public Room (string name, Player host, Player guest = null)
        {
            ID = IDbuilder++;
            Name = name;
            Host = host;
            Guest = guest;
        }
    }
}
