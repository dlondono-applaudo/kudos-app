using KudosApp.Domain.DTOs.Users;
using KudosApp.Domain.Entities;
using KudosApp.Domain.Interfaces;
using KudosApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Application.Services;

public class UsersService : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public UsersService(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<UserProfileResponse?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return null;

        var totalPoints = await _context.Kudos.Where(k => k.ReceiverId == userId).SumAsync(k => (int?)k.Points) ?? 0;
        var kudosSent = await _context.Kudos.CountAsync(k => k.SenderId == userId);
        var kudosReceived = await _context.Kudos.CountAsync(k => k.ReceiverId == userId);

        var badges = await _context.UserBadges
            .Include(ub => ub.Badge)
            .Where(ub => ub.UserId == userId)
            .Select(ub => new BadgeResponse(ub.Badge.Id, ub.Badge.Name, ub.Badge.Description, ub.Badge.Icon, ub.AwardedAt))
            .ToListAsync();

        return new UserProfileResponse(
            user.Id, user.Email!, user.FullName, user.Department,
            user.AvatarUrl, totalPoints, kudosSent, kudosReceived, badges, user.CreatedAt);
    }

    public async Task<IReadOnlyList<UserListItem>> GetAllAsync()
    {
        return await _context.Users
            .Select(u => new UserListItem(u.Id, u.Email!, u.FullName, u.Department))
            .ToListAsync();
    }
}
