using CachesModules.Managers;
using Leftyer.Domains.Utilitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LivesModules.Domains;
using LivesModules.Utilitys;

namespace LivesModules.Managers
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class ContactsPluginManager : DBContexts<ContactsModel>
    {
        ContactsPluginManager() : base(ConfigCore.Instance.DBCon) { }
        public static ContactsPluginManager Instance => new();

        #region Memo
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelContacts", $@"ContactsData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "ContactsData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(ContactsModel model) => Task.Run(() => Operation(model));
        public Task<ResultCore> BatchRemoveAsync(List<string> ids) => Task.Run(() => BatchRemove(ids));
        public ResultCore BatchRemove(List<string> ids)
        {
            try
            {
                if (ids == null || ids.Count <= 0)
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "未勾选任何行"
                    };
                }
                var upList = new List<ContactsModel>();
                ids.ForEach(m => upList.Add(new ContactsModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
                if (Db.Updateable(upList).UpdateColumns(it => new { it.UTime, it.IsDeleted }).ExecuteCommand() <= 0)
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "批量删除操作失败"
                    };
                }
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.success,
                    Msg = "批量删除操作成功"
                };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("我的收藏-联系人-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(ContactsModel model)
        {
            try
            {
                model.UTime = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(model.Id) && model.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.Deleted)
                {
                    var delModel = GetById(model.Id);
                    delModel.UTime = DateTime.Now;
                    delModel.IsDeleted = model.IsDeleted;
                    var delResult = CurrentDB.Update(delModel);
                    return new ResultCore
                    {
                        Code = delResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = delResult ? "删除成功" : "删除失败"
                    };

                }

                if (string.IsNullOrWhiteSpace(model.ContactName))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "联系人不能为空"
                    };
                }
                
                if (string.IsNullOrWhiteSpace(model.ContactContent))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "联系信息不能为空"
                    };
                }
                if (model.ContactType<0)
                {
                   return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "请选择联系方式"
                    };
                }
                if (string.IsNullOrWhiteSpace(model.Id) && IsExist(model, out string existMsg))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = existMsg
                    };
                }
                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    model.CTime = DateTime.Now;
                    model.Id = Guid.NewGuid().ToString();
                    model.Remark = "我的收藏-联系人";
                    model.IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete;
                    var addResult = CurrentDB.Insert(model);
                    return new ResultCore
                    {
                        Code = addResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = addResult ? "添加成功" : "添加失败"
                    };
                }
                var updateModel = GetById(model.Id);
                if (!string.IsNullOrWhiteSpace(model.ContactName))
                {
                    updateModel.ContactName = model.ContactName;
                }
                if (!string.IsNullOrWhiteSpace(model.ContactContent))
                {
                    updateModel.ContactContent = model.ContactContent;
                }
                if (model.ContactType>=0)
                {
                    updateModel.ContactType = model.ContactType;
                }
                updateModel.UTime = model.UTime;
                var eidtResult = CurrentDB.Update(updateModel);
                return new ResultCore
                {
                    Code = eidtResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = eidtResult ? "修改成功" : "修改失败"
                };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("我的收藏-联系人-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(ContactsModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<ContactsModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public ContactsModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new ContactsModel() : (CurrentDB.GetById(id) ?? new ContactsModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("我的收藏-联系人-获取", ex.Message, ex.ToString());
                return new ContactsModel();
            }
        }
        public bool IsExist(ContactsModel model, out string msg)
        {
            var isContactName=!string.IsNullOrWhiteSpace(model.ContactName) && !string.IsNullOrWhiteSpace(CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.ContactName == model.ContactName)?.FirstOrDefault()?.ContactName);
            var isContactContent=!string.IsNullOrWhiteSpace(model.ContactContent) && !string.IsNullOrWhiteSpace(CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.ContactContent == model.ContactContent)?.FirstOrDefault()?.ContactContent);
             if (isContactName&&isContactContent)
            {
                msg = "数据重复，不允许添加";
                return true;
            }
            msg = string.Empty;
            return false;
        }
        public dynamic GetPageData(ContactsModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[L_M_Contacts] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.ContactName))
                {
                    sql += $" AND [ContactName] LIKE '{queryItem.ContactName}%'";
                }
                if (queryItem.StartCTime.Year > 2000 && queryItem.EndCTime.Year > 2000)
                {
                    sql += $" AND [CTime]>='{queryItem.StartCTime}' AND [CTime]<='{queryItem.EndCTime}'";
                }
                if (queryItem.StartCTime.Year > 2000 && queryItem.EndCTime.Year <= 2000)
                {
                    sql += $" AND [CTime]>='{queryItem.StartCTime}'";
                }
                if (queryItem.StartCTime.Year <= 200 && queryItem.EndCTime.Year >= 2000)
                {
                    sql += $" AND [CTime]<='{queryItem.EndCTime}'";
                }
                int i = 1;
                var pageList = (new PageCore<ContactsModel>(ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<ContactsModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayContactName = m.ContactName,
                        displayContactContent = m.ContactContent,
                        displayContactType =  $"<span class='layui-badge layui-bg-blue'>{m.ContactTypeStr}</span>",
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("我的收藏-联系人-列表", ex.Message, ex.ToString());
                return new { code = (int)Utilitys.EnumsCore.ResultType.Fail, msg = "Found", count = 0 };
            }


        }
        string GetDisplayOperationIcons(string id)
        {
            var html = "<div class=\"layui-btn-group\">";
            var updateHtml = $"<button class=\"layui-btn layui-btn-normal layui-btn-xs\" title=\"修改数据\" onclick=\"Instace.Managers.Updated('{id}')\">修改</button>";
            var deleteHtml = $"<button class=\"layui-btn layui-btn-danger layui-btn-xs\" title=\"删除数据\" onclick=\"Instace.Managers.Deleted('{id}')\">删除</button>";
            return html + updateHtml + deleteHtml + "</div>";
        }

        public List<string> GetContactType
        {
            get
            {
                var result = new List<string>
                {
                    string.Format("{0}_{1}", "", "无")
                };
                var list = Leftyer.Domains.Utilitys.EnumsCore.ConvertList<Utilitys.EnumsCore.ContactType>();
                list.ForEach(item => result.Add(string.Format("{0}_{1}", list.IndexOf(item), item)));
                return result;
            }
        }
        #endregion

    }
}
