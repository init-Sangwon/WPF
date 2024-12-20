using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskServer
{
    internal class UserService
    {
        private readonly UserDB _userDB;

        public UserService(UserDB userDB)
        {
            _userDB = userDB;
        }

        public async Task<string> ProcessUserSignupAsync(string receivedData)
        {
            try
            {
                string[] parts = receivedData.Split('|');
                if (parts.Length != 3)
                {
                    return "잘못된 데이터 형식";
                }

                string id = parts[0];
                string password = parts[1];
                string name = parts[2];

                // 데이터베이스에 사용자 정보 저장
                await _userDB.InsertUserAsync(id, password, name);

                return "회원가입 성공";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"회원가입 처리 중 오류 발생: {ex.Message}");
                return "서버 내부 오류";
            }
        }
    }
}
