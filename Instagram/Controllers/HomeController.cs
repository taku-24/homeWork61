using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly InstagramContext _ctx;
    private readonly UserManager<User> _um;
    public HomeController(InstagramContext ctx, UserManager<User> um) { _ctx = ctx; _um = um; }

    public async Task<IActionResult> Index()
    {
        var me = await _um.GetUserAsync(User);
        var followingIds = await _ctx.Follows
            .Where(f => f.FollowerId == me.Id)
            .Select(f => f.FollowingId)
            .ToListAsync();

        var feed = await _ctx.Posts
            .Include(p => p.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Where(p => followingIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(feed);
    }


    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}