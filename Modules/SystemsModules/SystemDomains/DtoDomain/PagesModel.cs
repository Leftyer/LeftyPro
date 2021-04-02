using Leftyer.Domains.Utilitys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemsModules.Utilitys;

namespace SystemsModules.Domains
{
    [SugarTable("S_Pages")]
    public class PagesModel : S_Pages
    {
        [SugarColumn(IsIgnore = true)]
        public string CTimeStr => CTime.ToString("yyyy-MM-dd HH:mm:ss");
        [SugarColumn(IsIgnore = true)]
        public string UTimeStr => UTime.ToString("yyyy-MM-dd HH:mm:ss");
        [SugarColumn(IsIgnore = true)]
        public string IsDeletedStr => ((Utilitys.EnumsCore.IsDeleted)IsDeleted).ToString();
        [SugarColumn(IsIgnore = true)]
        public int Totals { get; set; }
        [SugarColumn(IsIgnore = true)]
        public DateTime StartCTime { get; set; }
        [SugarColumn(IsIgnore = true)]
        public DateTime EndCTime { get; set; }
        [SugarColumn(IsIgnore = true)]
        public int PageIndex { get; set; }
        [SugarColumn(IsIgnore = true)]
        public int PageSize { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string PageIconVal
        {
            get
            {
                try
                {
                    using var Db = new DBCore(ConfigCore.Instance.DBCon).Db;
                    var iconModel = (Db.Queryable<IconsModel>().Where(m => m.Id == PageIcon && m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete).ToList() ?? new List<IconsModel>()).FirstOrDefault();
                    return iconModel.IconClass ?? string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

    }
}
