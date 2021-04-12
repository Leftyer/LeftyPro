using LivesModules.Domains;
using LivesModules.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Domains;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.LiveModules.MyFavoritePlugin.Memo
{
    public class MemoIndexModel : BaseUtilitys
    {
        private readonly ILogger<MemoIndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public MemoIndexModel(ILogger<MemoIndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(MemosModel entity) => new JsonResult(await MemoPluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await MemoPluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(MemosModel entity) => new JsonResult(await MemoPluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await MemoPluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await MemoPluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"MemoData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        public List<string> GetMemoLevelType() => MemoPluginManager.Instance.GetMemoLevelType;

    }



}
