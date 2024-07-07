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
                if (request.Username.Length < 4 || request.Username.Length > 16)
                    return "Имя пользователя должно быть больше 4 и меньше 16 символов";
                foreach (char letter in request.Username) {
                    if ((!(letter >= 'a' && letter <= 'z') && !(letter >= 'A' && letter <= 'Z'))) {
                        return "В логине присутствует кириллица";
                    }
                }

                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (user != null)
                    return "Введённый пользователь уже существует";

                if (string.IsNullOrEmpty(request.Password))
                    return "Пароль не должен быть пустым";
                if (string.IsNullOrWhiteSpace(request.Password))
                    return "Пароль не должен состоять только из пробелов либо быть пустым";
                if (request.Password.Length < 6 || request.Password.Length > 24)
                    return "Пароль должен быть не менее 6 и не более 24 символов";


#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                return null;
#pragma warning restore CS8603
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке регистрации: {ex.Message}");
                throw;
            }

        }
    }
}
