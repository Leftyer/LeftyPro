using CachesModules.Managers;
using Leftyer.Domains.Utilitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemsModules.Domains;
using SystemsModules.Utilitys;

namespace SystemsModules.Managers
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class PagePluginManager : DBContexts<PagesModel>
    {
        PagePluginManager() : base(ConfigCore.Instance.DBCon) { }
        public static PagePluginManager Instance => new();

        #region Page
        public List<string> GetIconID
        {
            get
            {
                var result = new List<string>
                {
                    string.Format("{0}_{1}", "", "无")
                };
                GetIconLists.ForEach(item => result.Add(string.Format("{0}_{1}", item.Id, item.IconName)));
                return result;
            }
        }
        public List<string> GetParentID
        {
            get
            {
                var result = new List<string>
                {
                    string.Format("{0}_{1}", "", "无")
                };
                GetParentLevelPages.ForEach(item => result.Add(string.Format("{0}_{1}", item.Id, item.PageTitle)));
                return result;
            }
        }
        List<PagesModel> GetParentLevelPages => CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && string.IsNullOrWhiteSpace(m.ParentLevel)) ?? new List<PagesModel>();
        List<IconsModel> GetIconLists => Db.Queryable<IconsModel>().Where(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete)?.ToList() ?? new List<IconsModel>();
        public Task<ResultCore> DefaultSortAsync(string parentId) => Task.Run(() => DefaultSort(parentId));
        public ResultCore DefaultSort(string parentId)
        {
            try
            {
                var sortNum = CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.ParentLevel == (string.IsNullOrWhiteSpace(parentId) ? string.Empty : parentId)).OrderByDescending(o => o.PageSort)?.FirstOrDefault().PageSort + 1;
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.success,
                    Msg = "获取成功",
                    Data = sortNum
                };

            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("页面管理-默认排序", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.success,
                    Msg = ex.Message,
                    Data = 0
                };
            }


        }
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelPage", $@"PageData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "PageData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(PagesModel model) => Task.Run(() => Operation(model));
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
                var upList = new List<PagesModel>();
                ids.ForEach(m => upList.Add(new PagesModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("页面管理-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(PagesModel model)
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

                if (string.IsNullOrWhiteSpace(model.PageTitle))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "名称不能为空"
                    };
                }
                if (string.IsNullOrWhiteSpace(model.PageIcon))
                {
                    model.PageIcon = string.Empty;
                }
                if (string.IsNullOrWhiteSpace(model.PagePathUrl))
                {
                    model.PagePathUrl = string.Empty;
                }
                if (string.IsNullOrWhiteSpace(model.ParentLevel))
                {
                    model.ParentLevel = string.Empty;
                }
                if (model.PageSort < 0)
                {
                    model.PageSort = 0;
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
                    model.Remark = "页面信息";
                    model.IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete;
                    var addResult = CurrentDB.Insert(model);
                    return new ResultCore
                    {
                        Code = addResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = addResult ? "添加成功" : "添加失败"
                    };
                }
                var updateModel = GetById(model.Id);
                if (!string.IsNullOrWhiteSpace(model.PageTitle))
                {
                    updateModel.PageTitle = model.PageTitle;
                }
                if (!string.IsNullOrWhiteSpace(model.PageIcon))
                {
                    updateModel.PageIcon = model.PageIcon;
                }
                if (!string.IsNullOrWhiteSpace(model.PagePathUrl))
                {
                    updateModel.PagePathUrl = model.PagePathUrl;
                }
                if (!string.IsNullOrWhiteSpace(model.ParentLevel))
                {
                    updateModel.ParentLevel = model.ParentLevel;
                }
                if (updateModel.PageSort != model.PageSort)
                {
                    updateModel.PageSort = model.PageSort;
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
                LogManager.Instance.Log("页面管理-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(PagesModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<PagesModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public PagesModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new PagesModel() : (CurrentDB.GetById(id) ?? new PagesModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("页面管理-获取", ex.Message, ex.ToString());
                return new PagesModel();
            }
        }
        public bool IsExist(PagesModel model, out string msg)
        {
            if (!string.IsNullOrWhiteSpace(model.PageTitle) && !string.IsNullOrWhiteSpace(CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.PageTitle == model.PageTitle)?.PageTitle))
            {
                msg = "页面名称已存在";
                return true;
            }
            msg = string.Empty;
            return false;
        }
        public dynamic GetPageData(PagesModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[S_Pages] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.PageTitle))
                {
                    sql += $" AND [PageTitle] LIKE '{queryItem.PageTitle}%'";
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
                var pageList = (new PageCore<PagesModel>(ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<PagesModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayPageName = m.PageTitle,
                        displayPageIcon = $"<i class=\"layui-icon {m.PageIconVal}\" style=\"color:#FF5722;\"></i>",
                        displayParentLevelName = GetById(m.ParentLevel).PageTitle ?? string.Empty,
                        displayPageSort = m.PageSort,
                        displayPageUrl = m.PagePathUrl,
                        displayPageLevel = string.IsNullOrWhiteSpace(m.ParentLevel) ? "是" : "否",
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("页面管理-列表", ex.Message, ex.ToString());
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
        #endregion

    }
}
