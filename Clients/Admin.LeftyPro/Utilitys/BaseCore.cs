using Leftyer.Domains.Utilitys;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemsModules.Domains;

namespace Admin.LeftyPro
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class BaseCore
    {
        BaseCore() { }
        public static BaseCore Instance => new();

        #region 配置
        public string SystemName => "LeftyPro";
        #endregion

        #region 用户信息
        public string UserName(string token) => GetUsersInfo(token)?.UName;
        public S_Users GetUsersInfo(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new S_Users();
            }
            using var Db = new DBCore(SystemsModules.Utilitys.ConfigCore.Instance.DBCon).Db;
            return Db.Queryable<S_Users>().First(m => m.Id == token) ?? new S_Users();
        }
        #endregion

        #region 菜单

        public Task<List<PagesModel>> PageDataAsync(string token) => Task.Run(() => PageData(token));
        public List<PagesModel> PageData(string token)
        {
            using var Db = new DBCore(SystemsModules.Utilitys.ConfigCore.Instance.DBCon).Db;
            if (UserName(token) == "Leftyer")
            {
                return Db.Queryable<PagesModel>().Where(m => m.IsDeleted == (int)SystemsModules.Utilitys.EnumsCore.IsDeleted.NotDelete).ToList() ?? new List<PagesModel>();
            }
            return Db.Queryable<PagesModel, S_RolesPages, S_UsersRoles>((p, r, u) => new JoinQueryInfos(
                  JoinType.Left, p.Id == r.PagesId,
                  JoinType.Left, r.RolesId == u.RolesId
              )).Where((p, r, u) => p.IsDeleted == (int)SystemsModules.Utilitys.EnumsCore.IsDeleted.NotDelete
              && r.IsDeleted == (int)SystemsModules.Utilitys.EnumsCore.IsDeleted.NotDelete
              && u.IsDeleted == (int)SystemsModules.Utilitys.EnumsCore.IsDeleted.NotDelete
              && u.UsersId == token
              ).Select<PagesModel>().ToList() ?? new List<PagesModel>();
        }
        #endregion

    }
}
