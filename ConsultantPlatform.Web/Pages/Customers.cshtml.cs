using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConsultantPlatform.Web.Pages
{
    [Authorize(Roles = "Admin,User")]
    public class CustomersModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
