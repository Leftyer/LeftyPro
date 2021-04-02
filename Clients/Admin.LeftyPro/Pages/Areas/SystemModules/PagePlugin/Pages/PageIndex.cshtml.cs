using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Domains;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.PagePlugin.Pages
{
    public class PageIndexModel : BaseUtilitys
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public PageIndexModel(ILogger<IndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(PagesModel entity) => new JsonResult(await PagePluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await PagePluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(PagesModel entity) => new JsonResult(await PagePluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await PagePluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await PagePluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"PageData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        public async Task<IActionResult> OnGetDefaultSort(string ParentId) => new JsonResult(await PagePluginManager.Instance.DefaultSortAsync(ParentId));
        #region  Method/Property
        public List<string> GetIconID() => PagePluginManager.Instance.GetIconID;
        public List<string> GetParentID() => PagePluginManager.Instance.GetParentID;
        #endregion

    }



}
