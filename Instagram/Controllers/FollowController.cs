using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class FollowController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;
    public FollowController(InstagramContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FollowUser(int userId)
    {
        var me = await _userManager.GetUserAsync(User);
        if (me.Id == userId)
            return Json(new { success = false, message = "Нельзя подписаться на себя" });

        var exists = await _context.Follows.AnyAsync(f => f.FollowerId == me.Id && f.FollowingId == userId);
        if (exists)
            return Json(new { success = false, message = "Уже подписаны" });

        _context.Follows.Add(new Follow { FollowerId = me.Id, FollowingId = userId });
        var target = await _context.Users.FindAsync(userId);

        if (target != null)
        {
            me.FollowingCount++;
            target.FollowersCount++;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, following = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnfollowUser(int userId)
    {
        var me = await _userManager.GetUserAsync(User);
        var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == me.Id && f.FollowingId == userId);

        if (follow == null)
            return Json(new { success = false, message = "Не подписаны" });

        _context.Follows.Remove(follow);

        var target = await _context.Users.FindAsync(userId);
        if (target != null)
        {
            me.FollowingCount = Math.Max(0, me.FollowingCount - 1);
            target.FollowersCount = Math.Max(0, target.FollowersCount - 1);
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, following = false });
    }
}
