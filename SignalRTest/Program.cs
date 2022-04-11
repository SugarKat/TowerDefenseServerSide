using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Host.HttpListener;

namespace SignalRTest
{
    internal class Program
    {
        static bool quit = false;

        public static Dictionary<int, Player> connectedPlayers;
        public static Dictionary<int, Room> activeRooms;

        static void Main(string[] args)
        {
            connectedPlayers = new Dictionary<int, Player>();
            activeRooms = new Dictionary<int, Room>();
            string url = "http://192.168.0.146:30502";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                while (!quit)
                {
                    string comm = Console.ReadLine();
                    switch (comm)
                    {
                        case "users":
                            if (TestHub.currentUsers.Count == 0)
                            {
                                Console.WriteLine("No current users.");
                                break;
                            }
                            for (int i = 0; i < TestHub.currentUsers.Count; i++)
                            {
                                Console.WriteLine(TestHub.currentUsers.ElementAt(i).Key + " : " + TestHub.currentUsers.ElementAt(i).Value + " | " + TestHub.usersToIDConn.ElementAt(i).Key + " = " + TestHub.usersToIDConn.ElementAt(i).Value);
                            }
                            break;
                        case "list":
                            foreach (Room room in activeRooms.Values)
                            {
                                Console.WriteLine($"{room.ID} {room.Name} {room.Host.Username}");
                            }
                            break;
                        case "quit":
                            quit = true;
                            break;
                    }
                }
            }
            Console.WriteLine("Server shutdown");
            System.Console.Read();
        }
        public static void PrintMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
