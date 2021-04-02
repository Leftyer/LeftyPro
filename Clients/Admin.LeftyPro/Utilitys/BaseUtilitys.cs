using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Admin.LeftyPro
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class BaseUtilitys : PageModel
    {

        public BaseUtilitys(bool isAllow = false)
        {
            IsAllow = isAllow;
        }
        #region Authentication
        public string UserToken { get; set; }
        bool IsAllow { get; set; }
        public RedirectResult LoginUrl => Redirect("/Areas/SystemModules/AccountPlugin/Login/LoginIndex");
        //public IActionResult OnGet() => AuthenticationExc();
        //public IActionResult OnPost() => AuthenticationExc();
        public Task<IActionResult> OnGetAsync() => Task.Run(() => AuthenticationExc());
        public Task<IActionResult> OnPostAsync() => Task.Run(() => AuthenticationExc());
        IActionResult AuthenticationExc()
        {
            if (IsAllow) return null;
            try
            {
                HttpContext.Request.Cookies.TryGetValue("UserToken", out string token);
                UserToken = token;
                return string.IsNullOrWhiteSpace(UserToken) ? LoginUrl : null;
            }
            catch
            {
                return LoginUrl;
            }
        }
        #endregion



    }
}
