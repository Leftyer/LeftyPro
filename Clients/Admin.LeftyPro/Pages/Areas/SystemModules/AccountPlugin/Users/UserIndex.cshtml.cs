using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Domains;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.AccountPlugin.Users
{
    public class UserIndexModel : BaseUtilitys
    {
        private readonly ILogger<UserIndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public UserIndexModel(ILogger<UserIndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(UsersModel entity) => new JsonResult(await AccountPluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await AccountPluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(UsersModel entity) => new JsonResult(await AccountPluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await AccountPluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await AccountPluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"UserData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        public async Task<IActionResult> OnPostGetAuthorityCurr(string id) => new JsonResult(await AccountPluginManager.Instance.GetAuthorityAsync(id));
        public async Task<IActionResult> OnPostUpdateAuthority(string id, string ids) => new JsonResult(await AccountPluginManager.Instance.UpdateAuthorityAsync(id, ids.Split(",").ToList()));
        #region  Method/Property
        #endregion

    }



}
