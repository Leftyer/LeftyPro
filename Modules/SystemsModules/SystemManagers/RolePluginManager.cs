using CachesModules.Managers;
using Leftyer.Domains.Utilitys;
using SqlSugar;
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
    public class RolePluginManager : DBContexts<RolesModel>
    {
        RolePluginManager(): base(ConfigCore.Instance.DBCon) { AuthorityAll = GetAuthorityAll(); }
        public static RolePluginManager Instance => new();

        #region Role
        List<AuthorityEntity> AuthorityAll { get; set; }
        public Task<ResultCore> UpdateAuthorityAsync(string roleId, List<string> ids) => Task.Run(() => UpdateAuthority(roleId, ids));
        public ResultCore UpdateAuthority(string roleId, List<string> ids)
        {
            Db.Deleteable<S_RolesPages>().Where(m => m.RolesId == roleId).ExecuteCommand();
            foreach (var item in ids)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                Db.Insertable<S_RolesPages>(new S_RolesPages
                {
                    Id = Guid.NewGuid().ToString(),
                    RolesId = roleId,
                    PagesId = item,
                    IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete,
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    Remark = string.Empty
                }).ExecuteCommand();

            }
            return new ResultCore
            {
                Code = (int)Utilitys.EnumsCore.ResultType.success,
                Msg = "Success"
            };

        }
        public Task<ResultCore> GetAuthorityAsync(string id) => Task.Run(() => GetAuthority(id));
        public ResultCore GetAuthority(string id)
        {

            var currList = new List<AuthorityEntity>();
            var list = Db.Queryable<S_RolesPages, PagesModel>((m, n) => new JoinQueryInfos(JoinType.Left, m.PagesId == n.Id)).Where(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.RolesId == id).Select((m, n) => new { Id = n.Id, Title = n.PageTitle })?.ToList();
            list.ForEach(m => currList.Add(new AuthorityEntity
            {
                Value = m.Id,
                Title = m.Title,
                Disabled = false,
                Checked = false
            }));
            return new ResultCore
            {
                Code = (int)Utilitys.EnumsCore.ResultType.success,
                Msg = "Success",
                Data = new
                {
                    AuthorityData = AuthorityAll,
                    CurrData = currList?.Select(m => m.Value).ToList()
                }
            };
        }
        List<AuthorityEntity> GetAuthorityAll()
        {
            var result = new List<AuthorityEntity>();
            var list = Db.Queryable<PagesModel>().Where(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete)?.ToList() ?? new List<PagesModel>();
            list.ForEach(m => result.Add(new AuthorityEntity
            {
                Value = m.Id,
                Title = m.PageTitle,
                Disabled = false,
                Checked = false
            }));
            return result;
        }
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelRole", $@"RoleData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "RoleData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<ResultCore> OperationAsync(RolesModel model) => Task.Run(() => Operation(model));
        public Task<ResultCore> BatchRemoveAsync(List<string> ids) => Task.Run(() => BatchRemove(ids));
        public List<string> GetParentID
        {
            get
            {
                var result = new List<string>
                {
                    string.Format("{0}_{1}", "", "无")
                };
                GetParentLevelPages.ForEach(item => result.Add(string.Format("{0}_{1}", item.Id, item.RoleTitle)));
                return result;
            }
        }
        List<RolesModel> GetParentLevelPages => CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete) ?? new List<RolesModel>();
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
                var upList = new List<RolesModel>();
                ids.ForEach(m => upList.Add(new RolesModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("权限管理-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public ResultCore Operation(RolesModel model)
        {
            try
            {
                model.UTime = DateTime.Now;
                if (string.IsNullOrWhiteSpace(model.ParentLevel))
                {
                    model.ParentLevel = string.Empty;
                }
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

                if (string.IsNullOrWhiteSpace(model.RoleTitle))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "名称不能为空"
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
                    model.Remark = "角色信息";
                    model.IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete;
                    var addResult = CurrentDB.Insert(model);
                    return new ResultCore
                    {
                        Code = addResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = addResult ? "添加成功" : "添加失败"
                    };
                }
                var updateModel = GetById(model.Id);
                if (!string.IsNullOrWhiteSpace(model.RoleTitle))
                {
                    updateModel.RoleTitle = model.RoleTitle;
                }
                if (!string.IsNullOrWhiteSpace(model.ParentLevel))
                {
                    updateModel.ParentLevel = model.ParentLevel;
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
                LogManager.Instance.Log("权限管理-操作", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        public Task<dynamic> GetPageDataAsync(RolesModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<RolesModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public RolesModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new RolesModel() : (CurrentDB.GetById(id) ?? new RolesModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("权限管理-获取", ex.Message, ex.ToString());
                return new RolesModel();
            }
        }
        public bool IsExist(RolesModel model, out string msg)
        {
            if (!string.IsNullOrWhiteSpace(model.RoleTitle) && !string.IsNullOrWhiteSpace(CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.RoleTitle == model.RoleTitle)?.RoleTitle))
            {
                msg = "角色名称已存在";
                return true;
            }
            msg = string.Empty;
            return false;
        }
        public dynamic GetPageData(RolesModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[S_Roles] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.RoleTitle))
                {
                    sql += $" AND [RoleTitle] LIKE '{queryItem.RoleTitle}%'";
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
                var pageList = (new PageCore<RolesModel>(ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<RolesModel>()).Select(m =>

                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayRoleTitle = m.RoleTitle,
                        displayParentLevelName = GetById(m.ParentLevel).RoleTitle ?? string.Empty,
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("权限管理-列表", ex.Message, ex.ToString());
                return new { code = (int)Utilitys.EnumsCore.ResultType.Fail, msg = "Found", count = 0 };
            }


        }
        string GetDisplayOperationIcons(string id)
        {
            var html = "<div class=\"layui-btn-group\">";
            var updateHtml = $"<button class=\"layui-btn layui-btn-normal layui-btn-xs\" title=\"修改数据\" onclick=\"Instace.Managers.Updated('{id}')\">修改</button>";
            var deleteHtml = $"<button class=\"layui-btn layui-btn-danger layui-btn-xs\" title=\"删除数据\" onclick=\"Instace.Managers.Deleted('{id}')\">删除</button>";
            var pllocationHtml = $"<button class=\"layui-btn layui-btn-xs\" title=\"设置权限\" onclick=\"Instace.Managers.PllocationPage('{id}')\">设置权限</button>";
            return html + updateHtml + deleteHtml + pllocationHtml + "</div>";
        }
        #endregion

    }
}
