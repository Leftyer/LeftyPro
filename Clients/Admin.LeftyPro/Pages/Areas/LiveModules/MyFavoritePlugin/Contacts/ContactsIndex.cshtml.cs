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

namespace Admin.LeftyPro.Pages.Areas.LiveModules.MyFavoritePlugin.Contacts
{
    public class ContactsIndexModel : BaseUtilitys
    {
        private readonly ILogger<ContactsModel> _logger;
        private readonly IHostEnvironment _rootInfo;

        public ContactsIndexModel(ILogger<ContactsModel> logger, IHostEnvironment rootInfo)
        {
            _logger = logger;
            _rootInfo = rootInfo;
        }
        public async Task<IActionResult> OnGetPageData(ContactsModel entity) => new JsonResult(await ContactsPluginManager.Instance.GetPageDataAsync(entity));
        public async Task<IActionResult> OnPostGetById(string id) => new JsonResult(await ContactsPluginManager.Instance.GetByIdAsync(id));
        public async Task<IActionResult> OnPostAddAndEditExc(ContactsModel entity) => new JsonResult(await ContactsPluginManager.Instance.OperationAsync(entity));
        public async Task<IActionResult> OnPostBatchRemove(string ids) => new JsonResult(await ContactsPluginManager.Instance.BatchRemoveAsync(ids.Split(",").ToList()));
        public async Task<IActionResult> OnGetDownloadData() => File(await ContactsPluginManager.Instance.DownloadDataAsync(_rootInfo.ContentRootPath), "application/octet-stream", $"ContactsData{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        public List<string> GetContactType() => ContactsPluginManager.Instance.GetContactType;

    }



}
