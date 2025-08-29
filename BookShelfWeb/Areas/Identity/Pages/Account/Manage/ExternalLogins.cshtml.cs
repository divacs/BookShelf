using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ExternalLoginsModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public ExternalLoginsModel(UserManager<IdentityUser> userManager,
                               SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IList<UserLoginInfo> CurrentLogins { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        CurrentLogins = await _userManager.GetLoginsAsync(user);
    }
}
