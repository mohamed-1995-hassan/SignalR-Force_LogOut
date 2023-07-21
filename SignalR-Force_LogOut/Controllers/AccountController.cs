using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR_Force_LogOut.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SignalR_Force_LogOut.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHubContext<ChatHub> _hub;
        public Dictionary<string, string> AllConnections { get; set; } = new Dictionary<string, string>();

        public AccountController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hub = hubContext;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user,string connectionId)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password,false,false);
                if (result.Succeeded)
                {
                    return RedirectToAction("LogoutOtherUsers", "Account", new { connectionId });
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogoutOtherUsers(string connectionId)
        {
            if (ChatHub.MyUsers.ContainsKey(User.Identity.Name))
            {
                await _hub.Clients.Client(ChatHub.MyUsers[User.Identity.Name]).SendAsync("ForceLogOut");
                ChatHub.MyUsers.Remove(User.Identity.Name);
            }
            ChatHub.MyUsers.Add(User.Identity.Name, connectionId);

            return RedirectToAction("Index","Home");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogoutOthers()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
    }
}
