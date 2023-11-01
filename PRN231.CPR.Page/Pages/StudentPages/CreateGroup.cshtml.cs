using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN231.CPR.Page.Pages.StudentPages
{
    public class CreateGroupModel : PageModel
    {

        public IActionResult OnPost()
        {
            return RedirectToPage("/StudentPages/ManageGroup");
        }
    }
}
