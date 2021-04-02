using CachesModules.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.LogRecordPlugin.LogRecord
{
    public class LogRecordIndexModel : BaseUtilitys
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public LogRecordIndexModel(ILogger<IndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(LogRecordModel entity) => new JsonResult(await LogRecordPluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await LogRecordPluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(LogRecordModel entity) => new JsonResult(await LogRecordPluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await LogRecordPluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await LogRecordPluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"LogRecordData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

    }



}
