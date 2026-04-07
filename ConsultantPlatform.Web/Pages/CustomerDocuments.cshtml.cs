using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConsultantPlatform.Web.Pages;

[Authorize(Roles = "Admin")]
public class CustomerDocumentsModel : PageModel
{
    public void OnGet() { }
}
