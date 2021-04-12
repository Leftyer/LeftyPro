using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.AccountPlugin.Login
{
    public class LoginIndexModel : BaseUtilitys
    {
        private readonly ILogger<LoginIndexModel> _logger;
        public LoginIndexModel(ILogger<LoginIndexModel> logger) : base(true)
        {
            _logger = logger;
        }
        public async Task<IActionResult> OnPostLogin(string UserName, string UserPwd)
        {
            var result = await AccountPluginManager.Instance.LoginAsync(UserName, UserPwd);
            if (result.Code == (int)SystemsModules.Utilitys.EnumsCore.ResultType.success)
            {
                HttpContext.Response.Cookies.Append("UserToken", result.Data.ToString());
            }
            return new JsonResult(result);
        }
        public void OnGetLogOut()
        {
            try { HttpContext.Response.Cookies.Delete("UserToken"); } catch { }
        }

    }
}
