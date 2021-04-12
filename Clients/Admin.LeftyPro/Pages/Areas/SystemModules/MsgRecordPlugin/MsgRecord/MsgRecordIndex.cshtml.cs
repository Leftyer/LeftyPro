using CachesModules.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemsModules.Managers;

namespace Admin.LeftyPro.Pages.Areas.SystemModules.MsgRecordPlugin.MsgRecord
{
    public class MsgRecordIndexModel : BaseUtilitys
    {
        private readonly ILogger<MsgRecordIndexModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public MsgRecordIndexModel(ILogger<MsgRecordIndexModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(MsgRecordModel entity) => new JsonResult(await MsgRecordPluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await MsgRecordPluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(MsgRecordModel entity) => new JsonResult(await MsgRecordPluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await MsgRecordPluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await MsgRecordPluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"MsgRecordData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        public async Task<IActionResult> OnPostReadMsg(string ids) => new JsonResult(await MsgRecordPluginManager.Instance.ReadMsgAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetMsgIcon() => new JsonResult(await MsgRecordPluginManager.Instance.MsgIsonAsync());

    }



}
