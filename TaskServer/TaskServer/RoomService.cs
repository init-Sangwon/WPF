using System.Collections.Concurrent;

namespace TaskServer
{
    public class RoomService
    {
        public ConcurrentDictionary<int, string> Rooms { get; } = new();

        public int CreateRoom(string roomName)
        {
            int roomId = Rooms.Count + 1;
            Rooms[roomId] = roomName;
            return roomId;
        }
    }
}
