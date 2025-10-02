using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class FollowController : Controller
{
    private readonly InstagramContext _ctx;
    private readonly UserManager<User> _um;
    public FollowController(InstagramContext ctx, UserManager<User> um) { _ctx = ctx; _um = um; }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FollowUUser(int userId)
    {
        var me = await _um.GetUserAsync(User);
        if (me.Id == userId) return BadRequest("Нельзя подписаться на себя");

        var exists = await _ctx.Follows.AnyAsync(f => f.FollowerId == me.Id && f.FollowingId == userId);
        if (exists) return RedirectToAction("Profile", "Users", new { id = userId });

        _ctx.Follows.Add(new Follow { FollowerId = me.Id, FollowingId = userId });

        var target = await _ctx.Users.FindAsync(userId);
        if (target != null)
        {
            me.FollowingCount += 1;
            target.FollowersCount += 1;
        }
        await _ctx.SaveChangesAsync();

        return RedirectToAction("Profile", "Users", new { id = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnfollowUser(int userId)
    {
        var me = await _um.GetUserAsync(User);
        var follow = await _ctx.Follows.FirstOrDefaultAsync(f => f.FollowerId == me.Id && f.FollowingId == userId);
        if (follow != null)
        {
            _ctx.Follows.Remove(follow);
            var target = await _ctx.Users.FindAsync(userId);
            if (target != null)
            {
                me.FollowingCount = Math.Max(0, me.FollowingCount - 1);
                target.FollowersCount = Math.Max(0, target.FollowersCount - 1);
            }
            await _ctx.SaveChangesAsync();
        }
        return RedirectToAction("Profile", "Users", new { id = userId });
    }
}
