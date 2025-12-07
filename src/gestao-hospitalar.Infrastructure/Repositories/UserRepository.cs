using gestao_hospitalar.Domain.Users.Aggregates;
using gestao_hospitalar.Domain.Users.Repositories;
using gestao_hospitalar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace gestao_hospitalar.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<List<User>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }
}