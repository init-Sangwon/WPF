using System;
using System.Collections.Generic;

namespace TaskServer
{
    public class ClientManager
    {
        private readonly List<string> _messages = new List<string>();

        public string BroadcastMessage(string message)
        {
            _messages.Add(message);
            Console.WriteLine($"메시지: {message}");
            return "메시지 브로드캐스트 성공";
        }
    }
}
