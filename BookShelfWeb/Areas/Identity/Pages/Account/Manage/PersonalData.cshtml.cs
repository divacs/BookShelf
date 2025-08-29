using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class PersonalDataModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public PersonalDataModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IdentityUser UserData { get; set; }

    public async Task OnGetAsync()
    {
        UserData = await _userManager.GetUserAsync(User);
    }
}
