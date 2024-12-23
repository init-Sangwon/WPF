using System;
using System.Threading.Tasks;

namespace TaskServer
{
    internal class LoginService
    {
        private readonly UserDB _userDB;

        public LoginService(UserDB userDB)
        {
            _userDB = userDB ?? throw new ArgumentNullException(nameof(userDB));
        }

        public async Task<string> ProcessUserLoginAsync(string receivedData)
        {
            try
            {
                string[] parts = receivedData.Split('|');
                if (parts.Length != 2)
                {
                    return "잘못된 데이터 형식";
                }

                string id = parts[0];
                string hashedPassword = parts[1];

                // 데이터베이스에서 사용자 정보 확인
                bool isValidUser = await _userDB.ValidateUserAsync(id, hashedPassword);

                if (isValidUser)
                {
                    return "로그인 성공";
                }
                else
                {
                    return "ID 또는 비밀번호가 잘못되었습니다.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"로그인 처리 중 오류 발생: {ex.Message}");
                return "서버 내부 오류";
            }
        }
    }
}
