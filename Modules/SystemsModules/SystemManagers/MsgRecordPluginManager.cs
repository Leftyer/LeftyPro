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
    public class MsgRecordPluginManager : DBContexts<MsgRecordModel>
    {
        MsgRecordPluginManager() : base(CachesModules.Utilitys.ConfigCore.Instance.DBCon) { }
        public static MsgRecordPluginManager Instance => new();

        #region MsgRecord
        public Task<ResultCore> MsgIsonAsync() => Task.Run(() => MsgIson());
        public ResultCore MsgIson()
        {
            var model = CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.MsgIsRead == (int)CachesModules.Utilitys.EnumsCore.MsgIsRead.UnRead)?.FirstOrDefault() ?? new MsgRecordModel();
            return new ResultCore
            {
                Code = (int)Utilitys.EnumsCore.ResultType.success,
                Msg = "Success",
                Data = string.IsNullOrWhiteSpace(model.Id) ? 0 : 1
            };
        }
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public Task<ResultCore> ReadMsgAsync(List<string> ids) => Task.Run(() => ReadMsg(ids));
        public ResultCore ReadMsg(List<string> ids)
        {
            foreach (var item in ids)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var upModel = GetById(item);
                upModel.UTime = DateTime.Now;
                upModel.MsgIsRead = (int)CachesModules.Utilitys.EnumsCore.MsgIsRead.Read;
                Db.Updateable(upModel).UpdateColumns(it => new { it.UTime, it.MsgIsRead }).ExecuteCommand();

            }
            return new ResultCore
            {
                Code = (int)Utilitys.EnumsCore.ResultType.success,
                Msg = "操作成功"
            };
        }
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelMsgRecord", $@"MsgRecordData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "MsgRecordData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(MsgRecordModel model) => Task.Run(() => Operation(model));
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
                var upList = new List<MsgRecordModel>();
                ids.ForEach(m => upList.Add(new MsgRecordModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("消息管理-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(MsgRecordModel model)
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
                LogManager.Instance.Log("消息管理-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(MsgRecordModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<MsgRecordModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));

        public MsgRecordModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new MsgRecordModel() : (CurrentDB.GetById(id) ?? new MsgRecordModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("消息管理-获取", ex.Message, ex.ToString());
                return new MsgRecordModel();
            }
        }
        public dynamic GetPageData(MsgRecordModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[MsgRecord] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.MsgName))
                {
                    sql += $" AND [MsgName] LIKE '{queryItem.MsgName}%'";
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
                var pageList = (new PageCore<MsgRecordModel>(CachesModules.Utilitys.ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<MsgRecordModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayMsgName = m.MsgName,
                        displayMsgDetail = m.MsgDetail,
                        displayMsgIsRead = $"<span {((m.MsgIsRead == (int)CachesModules.Utilitys.EnumsCore.MsgIsRead.UnRead) ? "class='layui-badge layui-bg-orange'" : string.Empty)}>{m.MsgIsReadStr}</span>",
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id, m.MsgIsRead)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("消息管理-列表", ex.Message, ex.ToString());
                return new { code = (int)Utilitys.EnumsCore.ResultType.Fail, msg = "Found", count = 0 };
            }


        }
        string GetDisplayOperationIcons(string id, int isRead)
        {
            var html = "<div class=\"layui-btn-group\">";
            var showTitle = isRead == (int)CachesModules.Utilitys.EnumsCore.MsgIsRead.UnRead ? "未读" : "已读";
            var showbackgroud = isRead == (int)CachesModules.Utilitys.EnumsCore.MsgIsRead.UnRead ? "layui-btn-normal" : "layui-btn-disabled";
            var deleteHtml = $"<button class=\"layui-btn layui-btn-danger layui-btn-xs\" title=\"删除数据\" onclick=\"Instace.Managers.Deleted('{id}')\">删除</button>";
            var updateHtml = $"<button class=\"layui-btn {showbackgroud} layui-btn-xs\" title=\"标记已读\" onclick=\"Instace.Managers.ReadMsgFuc('{id}',1)\">{showTitle}</button>";
            return html + updateHtml + deleteHtml + "</div>";
        }
        #endregion
    }
}
