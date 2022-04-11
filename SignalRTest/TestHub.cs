using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace SignalRTest
{
    [HubName("testing")]
    public class TestHub : Hub
    {
        static int idBuilder = 0;
        private static bool gotUpdated = false;
        public static Dictionary<string, int> idMap = new Dictionary<string, int>();
        public static Dictionary<int, string> currentUsers = new Dictionary<int, string>();
        public static Dictionary<string, string> usersToIDConn = new Dictionary<string, string>();

        public int userConnected(string username) //When users connect
        {
            Console.WriteLine($"User {username}, with client ID: {idBuilder} connected, IP address: ");
            usersToIDConn.Add(username, Context.ConnectionId);
            currentUsers.Add(idBuilder, username);
            idMap.Add(Context.ConnectionId, idBuilder);
            string ipAddress;
            try
            {
                Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out object tempObj);
                ipAddress = (string)tempObj;
                Console.WriteLine(ipAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return idBuilder++;
                throw;
            }
            Player newP = new Player(username, Context.ConnectionId, ipAddress);
            Program.connectedPlayers.Add(idBuilder, newP);
            return idBuilder++;
        }
        public string grantAdminAcc(string s)
        {
            if (s == "suka")
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
        public void messageServer(int userID, string message)
        {
            if (currentUsers.TryGetValue(userID, out string name))
            {
                Console.WriteLine("User {0} sent: {1}", name, message);
            }
            else
            {
                Console.WriteLine("User {0} sent: {1}", userID, message);
            }
            gotUpdated = true;
        }
        public bool updateServer(string update)
        {
            gotUpdated = true;

            return true;
        }
        public bool isUpdated()
        {
            Console.WriteLine("Server was asked if updated, result: {0}", gotUpdated);
            if (gotUpdated)
            {
                gotUpdated = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ShowAllUsers()
        {
            for (int i = 0; i < currentUsers.Count; i++)
            {
                Console.WriteLine(currentUsers.ElementAt(i).Key + " = " + currentUsers.ElementAt(i).Value + " | " + usersToIDConn.ElementAt(i).Key + " = " + usersToIDConn.ElementAt(i).Value);
            }
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            if (idMap.TryGetValue(Context.ConnectionId, out int temp))
            {
                currentUsers.TryGetValue(temp, out string tempS);
                Console.WriteLine("User {0} disconnected", tempS);
                usersToIDConn.Remove(tempS);
                currentUsers.Remove(temp);
                idMap.Remove(Context.ConnectionId);
                Program.connectedPlayers.Remove(temp);
                Program.activeRooms.Remove(temp);
            }
            else
            {
                Console.WriteLine($"Client: {Context.ConnectionId} disconnected");
            }
            return base.OnDisconnected(stopCalled);
        }
        public bool whisper(int fromID, string toName, string message)
        {
            if (idMap.TryGetValue(Context.ConnectionId, out int _fromID))
            {
                if (_fromID == fromID)
                {
                    if (currentUsers.ContainsValue(toName))
                    {
                        currentUsers.TryGetValue(fromID, out string user);
                        Console.WriteLine("User {0} sent message to user {1}", user, toName);
                        usersToIDConn.TryGetValue(toName, out string connection);
                        string formatedMessage = string.Format("User {0} sent: {1}", user, message);
                        Clients.Client(connection).receiveMessage(formatedMessage);
                        return true;
                    }
                    Console.WriteLine("Sender tried to send a message to non existant receiver");
                    return false;
                }
            }
            Console.WriteLine($"Unknown user with {Context.ConnectionId} connection ID tried to send message");
            return false;
        }
        public void createRoom(string name, int playerID)
        {
            Program.connectedPlayers.TryGetValue(playerID, out Player player);
            Room newRoom = new Room(name, player);
            Console.WriteLine($"Room created with name: {name} {newRoom.ID}, host:{player.Username}");
            Program.activeRooms.Add(playerID, newRoom);
        }
        public string getRoomList()
        {
            string roomList = "";
            if(Program.activeRooms.Count == 0)
            {
                return "empty";
            }
            foreach(Room room in Program.activeRooms.Values)
            {
                string roomInfo = room.ID.ToString() + ';' + room.Name + ';' + room.Host.Username + ';' + room.Host.IpAddress;
                Program.PrintMessage($"{room.ID} {room.Name} {room.Host.Username}");
                if(roomList == "")
                {
                    roomList = roomInfo;
                }
                else
                {
                    roomList += ":" + roomInfo;
                }
            }
            return roomList;
        }
    }
}
