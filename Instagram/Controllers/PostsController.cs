using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Instagram.Models;

[Authorize]
public class PostsController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;

    public PostsController(InstagramContext context, UserManager<User> userManager)
    {
        _context = context; 
        _userManager = userManager;
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

        var user = await _userManager.GetUserAsync(User);

        var post = new Post
        {
            ImagePath = "/images/" + fileName,
            Description = description,
            UserId = user.Id,
            LikesCount = 0,
            CommentsCount = 0
        };

        _context.Posts.Add(post);
        user.PostsCount += 1;
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = post.Id });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (post == null) return NotFound();
        post.Comments = post.Comments?.OrderBy(c => c.CreatedAt).ToList();
        return View(post);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var me = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == me.Id);
        if (post == null)
            return Json(new { success = false, message = "Пост не найден или не принадлежит вам" });

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDescription(int id, string newDescription)
    {
        var me = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == me.Id);
        if (post == null)
            return Json(new { success = false, message = "Пост не найден или не принадлежит вам" });

        post.Description = newDescription;
        await _context.SaveChangesAsync();

        return Json(new { success = true, newText = post.Description });
    }
}
