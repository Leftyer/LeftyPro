using SqlSugar;
using System;
using LivesModules.Utilitys;

namespace LivesModules.Domains
{
    [SugarTable("L_M_Memo")]
    public class MemosModel : L_M_Memo
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
        
        [SugarColumn(IsIgnore=true)]
        public string MemoLevelTypeStr=>((Utilitys.EnumsCore.MemoLevelType)MemoLevel).ToString();


    }
}
