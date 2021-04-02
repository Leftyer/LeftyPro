using CachesModules.Domains;
using CachesModules.Managers;
using Leftyer.Domains.Utilitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SystemsModules.Managers
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class LogRecordPluginManager : DBContexts<LogRecordModel>
    {
        LogRecordPluginManager() : base(CachesModules.Utilitys.ConfigCore.Instance.DBCon) { }
        public static LogRecordPluginManager Instance => new();

        #region LogRecord
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelLogRecord", $@"LogRecordData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "LogRecordData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(LogRecordModel model) => Task.Run(() => Operation(model));
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
                var upList = new List<LogRecordModel>();
                ids.ForEach(m => upList.Add(new LogRecordModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("日志管理-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(LogRecordModel model)
        {
            try
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
            catch (Exception ex)
            {
                LogManager.Instance.Log("日志管理-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(LogRecordModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<LogRecordModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public LogRecordModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new LogRecordModel() : (CurrentDB.GetById(id) ?? new LogRecordModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("日志管理-获取", ex.Message, ex.ToString());
                return new LogRecordModel();
            }
        }
        public dynamic GetPageData(LogRecordModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[LogRecord] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.LogName))
                {
                    sql += $" AND [LogName] LIKE '{queryItem.LogName}%'";
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
                var pageList = (new PageCore<LogRecordModel>(CachesModules.Utilitys.ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<LogRecordModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayLogName = m.LogName,
                        displayLogMsg = m.LogMsg,
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("日志管理-列表", ex.Message, ex.ToString());
                return new { code = (int)Utilitys.EnumsCore.ResultType.Fail, msg = "Found", count = 0 };
            }

        }
        string GetDisplayOperationIcons(string id)
        {
            var html = "<div class=\"layui-btn-group\">";
            var deleteHtml = $"<button class=\"layui-btn layui-btn-danger layui-btn-xs\" title=\"删除数据\" onclick=\"Instace.Managers.Deleted('{id}')\">删除</button>";
            return html + deleteHtml + "</div>";
        }
        #endregion
    }
}
