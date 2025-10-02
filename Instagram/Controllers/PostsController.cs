using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Instagram.Models;

[Authorize]
public class PostsController : Controller
{
    private readonly InstagramContext _ctx;
    private readonly UserManager<User> _um;

    public PostsController(InstagramContext ctx, UserManager<User> um)
    {
        _ctx = ctx; _um = um;
    }

    [HttpGet]
    public IActionResult Create() => View();
    
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IFormFile image, string? description)
    {
        if (image == null || image.Length == 0)
        {
            ModelState.AddModelError("image", "Загрузите изображение");
            return View();
        }
        
        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        var root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, fileName);
        using (var fs = new FileStream(path, FileMode.Create)) { await image.CopyToAsync(fs); }

        var user = await _um.GetUserAsync(User);

        var post = new Post
        {
            ImagePath = "/images/" + fileName,
            Description = description,
            UserId = user.Id,
            LikesCount = 0,
            CommentsCount = 0
        };

        _ctx.Posts.Add(post);
        user.PostsCount += 1;
        await _ctx.SaveChangesAsync();

        return RedirectToAction("Details", new { id = post.Id });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var post = await _ctx.Posts
            .Include(p => p.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (post == null) return NotFound();
        post.Comments = post.Comments?.OrderBy(c => c.CreatedAt).ToList();
        return View(post);
    }
}
