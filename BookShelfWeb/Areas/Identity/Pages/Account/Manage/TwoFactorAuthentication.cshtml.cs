using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class TwoFactorAuthenticationModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public TwoFactorAuthenticationModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public bool Is2faEnabled { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
    }
}
