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
    public class MemoPluginManager : DBContexts<MemosModel>
    {
        MemoPluginManager() : base(ConfigCore.Instance.DBCon) { }
        public static MemoPluginManager Instance => new();

        #region Memo
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelMemo", $@"MemoData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "MemoData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(MemosModel model) => Task.Run(() => Operation(model));
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
                var upList = new List<MemosModel>();
                ids.ForEach(m => upList.Add(new MemosModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("我的收藏-备忘录-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(MemosModel model)
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

                if (string.IsNullOrWhiteSpace(model.MemoContent))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "内容不能为空"
                    };
                }
                if (model.MemoLevel<0)
                {
                    model.MemoLevel=(int)Utilitys.EnumsCore.MemoLevelType.一般;
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
                    model.Remark = "我的收藏-备忘录";
                    model.IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete;
                    var addResult = CurrentDB.Insert(model);
                    return new ResultCore
                    {
                        Code = addResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = addResult ? "添加成功" : "添加失败"
                    };
                }
                var updateModel = GetById(model.Id);
                if (!string.IsNullOrWhiteSpace(model.MemoContent))
                {
                    updateModel.MemoContent = model.MemoContent;
                }
                if (model.MemoLevel>=0)
                {
                    updateModel.MemoLevel = model.MemoLevel;
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
                LogManager.Instance.Log("我的收藏-备忘录-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(MemosModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<MemosModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public MemosModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new MemosModel() : (CurrentDB.GetById(id) ?? new MemosModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("我的收藏-备忘录-获取", ex.Message, ex.ToString());
                return new MemosModel();
            }
        }
        public bool IsExist(MemosModel model, out string msg)
        {
            msg = string.Empty;
            return false;
        }
        public dynamic GetPageData(MemosModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[L_M_Memo] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.MemoContent))
                {
                    sql += $" AND [MemoContent] LIKE '%{queryItem.MemoContent}%'";
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
                var pageList = (new PageCore<MemosModel>(ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<MemosModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayMemoContent = m.MemoContent.Length<=20?m.MemoContent:m.MemoContent.Substring(0,20)+"...",
                        displayMemoLevelType =  $"<span class='layui-badge {((m.MemoLevel == (int)Utilitys.EnumsCore.MemoLevelType.一般)?"layui-bg-orange":string.Empty)}'>{m.MemoLevelTypeStr}</span>",
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("我的收藏-备忘录-列表", ex.Message, ex.ToString());
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

        public List<string> GetMemoLevelType
        {
            get
            {
                var result = new List<string>
                {
                    string.Format("{0}_{1}", "", "无")
                };
                var list = Leftyer.Domains.Utilitys.EnumsCore.ConvertList<Utilitys.EnumsCore.MemoLevelType>();
                list.ForEach(item => result.Add(string.Format("{0}_{1}", list.IndexOf(item), item)));
                return result;
            }
        }
        #endregion

    }
}
