using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConsultantPlatform.Web.Pages
{
    [Authorize(Roles = "Admin")]
    public class CustomerEditModel : PageModel
    {
        public void OnGet() { }
    }
}
