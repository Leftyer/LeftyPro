using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Domains;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.IconPlugin.Icon
{
    public class IconIndexModel : BaseUtilitys
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public IconIndexModel(ILogger<IndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(IconsModel entity) => new JsonResult(await IconPluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await IconPluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(IconsModel entity) => new JsonResult(await IconPluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await IconPluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await IconPluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"IconData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");



    }



}
