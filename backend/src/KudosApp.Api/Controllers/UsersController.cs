using System.Security.Claims;
using KudosApp.Core.DTOs.Users;
using KudosApp.Core.Entities;
using KudosApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public UsersController(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await GetProfileById(userId);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileResponse>> GetProfile(string id)
    {
        return await GetProfileById(id);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserListItem>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserListItem(u.Id, u.Email!, u.FullName, u.Department))
            .ToListAsync();

        return Ok(users);
    }

    private async Task<ActionResult<UserProfileResponse>> GetProfileById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var totalPoints = await _context.Kudos
            .Where(k => k.ReceiverId == userId)
            .SumAsync(k => (int?)k.Points) ?? 0;

        var kudosSent = await _context.Kudos.CountAsync(k => k.SenderId == userId);
        var kudosReceived = await _context.Kudos.CountAsync(k => k.ReceiverId == userId);

        var badges = await _context.UserBadges
            .Include(ub => ub.Badge)
            .Where(ub => ub.UserId == userId)
            .Select(ub => new BadgeResponse(
                ub.Badge.Id, ub.Badge.Name, ub.Badge.Description,
                ub.Badge.Icon, ub.AwardedAt))
            .ToListAsync();

        return Ok(new UserProfileResponse(
            user.Id, user.Email!, user.FullName, user.Department,
            user.AvatarUrl, totalPoints, kudosSent, kudosReceived,
            badges, user.CreatedAt));
    }
}
