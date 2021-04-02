using CachesModules.Utilitys;
using SqlSugar;
using System;

namespace CachesModules.Domains
{
    [SugarTable("CacheInfo")]
    public class CacheInfoModel : CacheInfo
    {
        [SugarColumn(IsIgnore = true)]
        public string CTimeStr => CTime.ToString("yyyy-MM-dd HH:mm:ss");
        [SugarColumn(IsIgnore = true)]
        public string UTimeStr => UTime.ToString("yyyy-MM-dd HH:mm:ss");
        [SugarColumn(IsIgnore = true)]
        public string IsDeletedStr => ((EnumsCore.IsDeleted)IsDeleted).ToString();
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
    }
}
