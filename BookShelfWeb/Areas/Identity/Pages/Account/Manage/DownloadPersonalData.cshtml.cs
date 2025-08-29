using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class DownloadPersonalDataModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public DownloadPersonalDataModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        // Scaffold generiše logiku za pripremu i preuzimanje korisnickih podataka
        return File(System.Text.Encoding.UTF8.GetBytes("data"), "text/plain", "PersonalData.txt");
    }
}
