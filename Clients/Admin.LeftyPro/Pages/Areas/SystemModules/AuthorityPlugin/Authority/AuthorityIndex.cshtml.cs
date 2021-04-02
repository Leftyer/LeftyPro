using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Domains;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.AuthorityPlugin.Authority
{
    public class AuthorityIndexModel : BaseUtilitys
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public AuthorityIndexModel(ILogger<IndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(RolesModel entity) => new JsonResult(await RolePluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await RolePluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(RolesModel entity) => new JsonResult(await RolePluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await RolePluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await RolePluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"RoleData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        public async Task<IActionResult> OnPostGetAuthorityCurr(string id) => new JsonResult(await RolePluginManager.Instance.GetAuthorityAsync(id));
        public async Task<IActionResult> OnPostUpdateAuthority(string id, string ids) => new JsonResult(await RolePluginManager.Instance.UpdateAuthorityAsync(id, ids.Split(",").ToList()));

        #region  Method/Property
        public List<string> GetParentID() => RolePluginManager.Instance.GetParentID;
        #endregion

    }



}
