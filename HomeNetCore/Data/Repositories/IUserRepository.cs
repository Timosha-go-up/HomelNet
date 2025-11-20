using HomeNetCore.Models;

namespace HomeNetCore.Data.Repositories
{
    public interface IUserRepository
    {
        // Создание (Create)
        Task<UserEntity> InsertUserAsync(UserEntity user);

        // Чтение (Read)
        Task<List<UserEntity>> GetAllAsync();
        Task<UserEntity?> GetByIdAsync(int id);
        Task<UserEntity?> GetByEmailAsync(string email);

      
        Task UpdateAsync(UserEntity user);

        // Удаление (Delete)
        Task DeleteByIdAsync(int id);
    }
}
