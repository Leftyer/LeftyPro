using SqlSugar;
using System;
using SystemsModules.Utilitys;

namespace SystemsModules.Domains
{
    [SugarTable("S_Users")]
    public class UsersModel : S_Users
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
        public string UserTypeStr => ((Utilitys.EnumsCore.UserType)UType).ToString();
        [SugarColumn(IsIgnore = true)]
        public string UserStatus => ((Utilitys.EnumsCore.UserStatus)UStatus).ToString();
    }
}
