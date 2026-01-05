using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;
using TaskManagement.Data;
using TaskManagement.ViewModels;

public class AccountController : Controller
{
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> _signInManager;

    public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register() 
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new Users
        {
            Name = model.Name,
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult Login() 
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
        if (result.Succeeded)
        {
            user.HasLoggedIn = true;
            await _userManager.UpdateAsync(user);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Index", "UserDashboard");
        }

        ModelState.AddModelError("", "Invalid credentials");
        return View(model);
    }

    [HttpGet]
    public IActionResult AdminLogin() 
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AdminLogin(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        if (!await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ModelState.AddModelError("", "Access denied. Admins only.");
            return View(model);
        }

        await _signInManager.SignOutAsync();
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

        if (result.Succeeded)
        {
            user.HasLoggedIn = true;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "Dashboard");
        }

        ModelState.AddModelError("", "Invalid credentials");
        return View(model);
    }

     [HttpPost]
     public async Task<IActionResult> Logout()
     {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
