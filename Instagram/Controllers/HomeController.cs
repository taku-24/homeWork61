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
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(InstagramContext context, UserManager<User> userManager)
    {
        _context = context; 
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var me = await _userManager.GetUserAsync(User);
        var followingIds = await _context.Follows
            .Where(f => f.FollowerId == me.Id)
            .Select(f => f.FollowingId)
            .ToListAsync();

        var feed = await _context.Posts
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