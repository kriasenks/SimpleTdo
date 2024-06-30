using SimpleTdo.Contracts;
using SimpleTdo.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace SimpleTdo.Check
{
    public class RegisterCheck
    {
        public static async Task<string> CheckRegister(RegisterRequest request, UsersDbContext context) 
        {
            try
            {
                if (request.Username.Length < 4)
                    return "Имя пользователя должно быть больше 4 символов";

                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (user != null)
                    return "Введённый пользователь уже существует";

                if (string.IsNullOrEmpty(request.Password))
                    return "Пароль не должен быть пустым";
                if (string.IsNullOrWhiteSpace(request.Password))
                    return "Пароль не должен состоять только из пробелов либо быть пустым";
                if (request.Password.Length < 6)
                    return "Пароль должен быть не менее 6 символов";

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке регистрации: {ex.Message}");
                throw;
            }

        }
    }
}
