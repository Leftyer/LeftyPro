using CachesModules.Managers;
using Leftyer.Domains.Utilitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class IconPluginManager : DBContexts<IconsModel>
    {
        IconPluginManager() : base(ConfigCore.Instance.DBCon) { }
        public static IconPluginManager Instance => new();

        #region Icon
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelIcon", $@"IconData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "IconData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(IconsModel model) => Task.Run(() => Operation(model));
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
                var upList = new List<IconsModel>();
                ids.ForEach(m => upList.Add(new IconsModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("图标管理-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(IconsModel model)
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

                if (string.IsNullOrWhiteSpace(model.IconName))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "名称不能为空"
                    };
                }
                if (string.IsNullOrWhiteSpace(model.IconClass))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "类不能为空"
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
                    model.Remark = "图标信息";
                    model.IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete;
                    var addResult = CurrentDB.Insert(model);
                    return new ResultCore
                    {
                        Code = addResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = addResult ? "添加成功" : "添加失败"
                    };
                }
                var updateModel = GetById(model.Id);
                if (!string.IsNullOrWhiteSpace(model.IconName))
                {
                    updateModel.IconName = model.IconName;
                }
                if (!string.IsNullOrWhiteSpace(model.IconClass))
                {
                    updateModel.IconClass = model.IconClass;
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
                LogManager.Instance.Log("图标管理-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(IconsModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<IconsModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public IconsModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new IconsModel() : (CurrentDB.GetById(id) ?? new IconsModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("图标管理-获取", ex.Message, ex.ToString());
                return new IconsModel();
            }
        }
        public bool IsExist(IconsModel model, out string msg)
        {
            if (!string.IsNullOrWhiteSpace(model.IconName) && !string.IsNullOrWhiteSpace(CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.IconName == model.IconName)?.IconName))
            {
                msg = "图标名称已存在";
                return true;
            }
            if (!string.IsNullOrWhiteSpace(model.IconClass) && !string.IsNullOrWhiteSpace(CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.IconClass == model.IconClass)?.IconClass))
            {
                msg = "图标类名已存在";
                return true;
            }
            msg = string.Empty;
            return false;
        }
        public dynamic GetPageData(IconsModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[S_Icons] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.IconName))
                {
                    sql += $" AND [IconName] LIKE '{queryItem.IconName}%'";
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
                var pageList = (new PageCore<IconsModel>(ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<IconsModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayIconImg = $"<i class=\"layui-icon {m.IconClass}\" style=\"color:#FF5722;\"></i>",
                        displayIconName = m.IconName,
                        displayIconClass = m.IconClass,
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("图标管理-列表", ex.Message, ex.ToString());
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
