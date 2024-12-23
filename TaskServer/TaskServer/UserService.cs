using System;
using System.Threading.Tasks;

namespace TaskServer
{
    public class UserService
    {
        private readonly UserDB _userDB;
        private readonly RoomService _roomService;
        private readonly ClientManager _clientManager;

        public UserService(UserDB userDB, RoomService roomService, ClientManager clientManager)
        {
            _userDB = userDB;
            _roomService = roomService;
            _clientManager = clientManager;
        }

        public async Task<string> ProcessRequestAsync(string message)
        {
            if (message.StartsWith("SIGNUP|"))
            {
                string signupData = message.Substring(7);
                return await ProcessUserSignupAsync(signupData);
            }
            if (message.StartsWith("LOGIN|"))
            {
                string loginData = message.Substring(6);
                return await ProcessUserLoginAsync(loginData);
            }
            if (message.StartsWith("MESSAGE|"))
            {
                string chatData = message.Substring(8);
                return _clientManager.BroadcastMessage(chatData);
            }

            return "잘못된 요청 형식";
        }

        public async Task<string> ProcessUserSignupAsync(string data)
        {
            string[] parts = data.Split('|');
            if (parts.Length != 3) return "잘못된 데이터 형식";

            string id = parts[0];
            string password = parts[1];
            string name = parts[2];

            await _userDB.InsertUserAsync(id, password, name);
            return "회원가입 성공";
        }

        public async Task<string> ProcessUserLoginAsync(string data)
        {
            string[] parts = data.Split('|');
            if (parts.Length != 2) return "잘못된 데이터 형식";

            string id = parts[0];
            string hashedPassword = parts[1];

            return await _userDB.ValidateUserAsync(id, hashedPassword) ? "로그인 성공" : "로그인 실패";
        }
    }
}
