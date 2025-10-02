using Instagram.Models;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly InstagramContext _ctx;
    private readonly UserManager<User> _userManager;

    public UsersController(InstagramContext ctx, UserManager<User> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
    }

    
    [HttpGet]
    public IActionResult Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return View(new List<User>());
        }

        var users = _ctx.Users
            .Where(u => u.UserName.Contains(q) || u.Name.Contains(q))
            .ToList();

        return View(users);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> Profile(int id)
    {
        var user = await _ctx.Users
            .Include(u => u.Posts)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        ViewBag.Posts = user.Posts?.OrderByDescending(p => p.CreatedAt).ToList();
        return View(user);
    }
    
    [HttpGet]
    public async Task<IActionResult> MyProfile()
    {
        var me = await _userManager.GetUserAsync(User);
        if (me == null) return RedirectToAction("Login", "Account");

        var loadedUser = await _ctx.Users
            .Include(u => u.Posts)
            .FirstOrDefaultAsync(u => u.Id == me.Id);

        ViewBag.Posts = loadedUser?.Posts?.OrderByDescending(p => p.CreatedAt).ToList();
        return View(loadedUser);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var me = await _userManager.GetUserAsync(User);
        if (me == null) return RedirectToAction("Login", "Account");

        var vm = new EditProfileViewModel
        {
            Name = me.Name,
            UserInfo = me.UserInfo,
            Gender = me.Gender
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        var me = await _userManager.GetUserAsync(User);
        if (me == null) return RedirectToAction("Login", "Account");

        if (ModelState.IsValid)
        {
            me.Name = model.Name;
            me.UserInfo = model.UserInfo;
            me.Gender = model.Gender;

            if (model.ProfilePicture != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                me.ProfilePicturePath = "/images/" + fileName;
            }

            _ctx.Update(me);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("MyProfile");
        }

        return View(model);
    }
}
