using CachesModules.Managers;
using Leftyer.Domains.Utilitys;
using Newtonsoft.Json;
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
    public class AccountPluginManager : DBContexts<UsersModel>
    {
        public static AccountPluginManager Instance => new();
        AccountPluginManager() : base(ConfigCore.Instance.DBCon) { AuthorityAll = GetAuthorityAll(); }

        #region Login
        public Task<ResultCore> LoginAsync(string userName, string userPwd) => Task.Run(() => Login(userName, userPwd));
        public ResultCore Login(string userName, string userPwd)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPwd))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "账号或密码错误"
                    };
                }
                var model = CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.UName == userName && m.PwdVal == SecurityCore.Instance.MD5(userPwd) && m.UStatus == (int)Utilitys.EnumsCore.UserStatus.Usable) ?? new UsersModel();
                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "账号或密码错误"
                    };
                }
                RedisCacheManagers.Instance.Del(model.Id);
                RedisCacheManagers.Instance.Set(model.Id, JsonConvert.SerializeObject(model));
                //HttpContext.Response.Cookies.Append("getCookie", "setCookieValue");
                var logInfoMsg = $"{userName}登录成功系统";
                LogManager.Instance.Log("Login", logInfoMsg, logInfoMsg);
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.success,
                    Msg = "登录成功",
                    Data = model.Id
                };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("Login", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        #endregion

        #region Users
        public Task<ResultCore> OperationAsync(UsersModel model) => Task.Run(() => Operation(model));
        public ResultCore Operation(UsersModel model)
        {
            try
            {
                model.UTime = DateTime.Now;
                model.PwdVal = string.IsNullOrWhiteSpace(model.PwdVal) ? string.Empty : SecurityCore.Instance.MD5(model.PwdVal);
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

                if (string.IsNullOrWhiteSpace(model.UName))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "用户名不能为空"
                    };
                }
                if (string.IsNullOrWhiteSpace(model.PwdVal))
                {
                    return new ResultCore
                    {
                        Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = "密码不能为空"
                    };
                }
                model.Avatar = string.Empty;
                model.Creator = "Leftyer";
                model.GenderVal = -1;
                model.IDNO = string.Empty;
                model.MaritalStatus = -1;
                model.Nickname = model.UName;
                model.NowAddressVal = string.Empty;
                model.PhoneNo = string.Empty;
                model.ProfessionVal = string.Empty;
                model.QQNo = string.Empty;
                model.RealAddressVal = string.Empty;
                model.RealName = model.UName;
                model.UStatus = (int)Utilitys.EnumsCore.UserStatus.Usable;
                model.UType = (int)Utilitys.EnumsCore.UserType.Admin;
                model.WechatNo = string.Empty;
                model.Modifier = "Leftyer";

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
                    model.Remark = "用户信息";
                    model.IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete;
                    var addResult = CurrentDB.Insert(model);
                    return new ResultCore
                    {
                        Code = addResult ? (int)Utilitys.EnumsCore.ResultType.success : (int)Utilitys.EnumsCore.ResultType.Fail,
                        Msg = addResult ? "添加成功" : "添加失败"
                    };
                }
                var updateModel = GetById(model.Id);
                if (!string.IsNullOrWhiteSpace(model.UName))
                {
                    updateModel.UName = model.UName;
                }
                if (!string.IsNullOrWhiteSpace(model.PwdVal))
                {
                    updateModel.PwdVal = model.PwdVal;
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
                LogManager.Instance.Log("用户管理-增删改", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
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
                var upList = new List<UsersModel>();
                ids.ForEach(m => upList.Add(new UsersModel { Id = m, IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.Deleted, UTime = DateTime.Now }));
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
                LogManager.Instance.Log("用户管理-批量删除", ex.Message, ex.ToString());
                return new ResultCore
                {
                    Code = (int)Utilitys.EnumsCore.ResultType.Fail,
                    Msg = ex.Message
                };
            }
        }
        List<AuthorityEntity> AuthorityAll { get; set; }
        public Task<ResultCore> UpdateAuthorityAsync(string userId, List<string> ids) => Task.Run(() => UpdateAuthority(userId, ids));
        public ResultCore UpdateAuthority(string userId, List<string> ids)
        {
            Db.Deleteable<S_UsersRoles>().Where(m => m.UsersId == userId).ExecuteCommand();
            foreach (var item in ids)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                Db.Insertable(new S_UsersRoles
                {
                    Id = Guid.NewGuid().ToString(),
                    RolesId = item,
                    UsersId = userId,
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
            var list = Db.Queryable<S_UsersRoles, RolesModel>((m, n) => new JoinQueryInfos(JoinType.Left, m.RolesId == n.Id)).Where(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.UsersId == id).Select((m, n) => new { Id = n.Id, Title = n.RoleTitle })?.ToList();
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
            var list = Db.Queryable<RolesModel>().Where(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete)?.ToList() ?? new List<RolesModel>();
            list.ForEach(m => result.Add(new AuthorityEntity
            {
                Value = m.Id,
                Title = m.RoleTitle,
                Disabled = false,
                Checked = false
            }));
            return result;
        }
        public Task<FileStream> DownloadDataAsync(string rootPath) => Task.Run(() => DownloadData(rootPath));
        public FileStream DownloadData(string rootPath) => ExcelCore.Instance.Export(rootPath + $"/ExcelExport/ExcelUser", $@"UserData{DateTime.Now:yyyyMMddHHmmss}.xlsx", "UserData", CurrentDB.GetList(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete));
        public Task<dynamic> GetPageDataAsync(UsersModel queryItem) => Task.Run(() => GetPageData(queryItem));
        public Task<UsersModel> GetByIdAsync(string id) => Task.Run(() => GetById(id));
        public UsersModel GetById(string id)
        {
            try
            {
                return string.IsNullOrWhiteSpace(id) ? new UsersModel() : (CurrentDB.GetById(id) ?? new UsersModel());
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("用户管理-获取", ex.Message, ex.ToString());
                return new UsersModel();
            }
        }
        public bool IsExist(UsersModel model, out string msg)
        {
            if (!string.IsNullOrWhiteSpace(model.UName) && !string.IsNullOrWhiteSpace(CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.UName == model.UName)?.UName))
            {
                msg = "用户已存在";
                return true;
            }
            msg = string.Empty;
            return false;
        }
        public dynamic GetPageData(UsersModel queryItem)
        {
            try
            {
                var total = 0;
                var sql = $"SELECT * FROM  [dbo].[S_Users] WHERE [IsDeleted]={(int)Utilitys.EnumsCore.IsDeleted.NotDelete} ";
                if (!string.IsNullOrWhiteSpace(queryItem.UName))
                {
                    sql += $" AND [UName] LIKE '{queryItem.UName}%'";
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
                var pageList = (new PageCore<UsersModel>(ConfigCore.Instance.DBCon).GetPage(new PageItem { PageSql = sql, PageIndex = queryItem.PageIndex, PageSize = queryItem.PageSize }, ref total) ?? new List<UsersModel>()).Select(m =>
                {
                    return new
                    {
                        hideId = m.Id,
                        displayID = ((m.PageIndex <= 0 ? 1 : m.PageIndex) * m.PageSize) - (m.PageSize - i++),
                        displayUName = m.UName,
                        displayCTime = m.CTimeStr,
                        displayUTime = m.UTimeStr,
                        displayOperation = GetDisplayOperationIcons(m.Id)
                    };
                }).ToList();
                return new { code = pageList.Count <= 0 ? (int)Utilitys.EnumsCore.ResultType.Fail : (int)Utilitys.EnumsCore.ResultType.success, msg = pageList.Count <= 0 ? "Found" : "successfully", count = total, data = pageList };
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log("用户管理-列表", ex.Message, ex.ToString());
                return new { code = (int)Utilitys.EnumsCore.ResultType.Fail, msg = "Found", count = 0 };
            }
        }
        string GetDisplayOperationIcons(string id)
        {
            var html = "<div class=\"layui-btn-group\">";
            var updateHtml = $"<button class=\"layui-btn layui-btn-normal layui-btn-xs\" title=\"修改数据\" onclick=\"Instace.Managers.Updated('{id}')\">修改</button>";
            var deleteHtml = $"<button class=\"layui-btn layui-btn-danger layui-btn-xs\" title=\"删除数据\" onclick=\"Instace.Managers.Deleted('{id}')\">删除</button>";
            var pllocationHtml = $"<button class=\"layui-btn layui-btn-xs\" title=\"设置角色\" onclick=\"Instace.Managers.PllocationPage('{id}')\">设置角色</button>";
            return html + updateHtml + deleteHtml + pllocationHtml + "</div>";
        }
        #endregion

    }
}


