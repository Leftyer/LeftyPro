using SqlSugar;
using System.Collections.Generic;

namespace Leftyer.Domains.Utilitys
{
    public class PageCore<T> where T : class, new()
    {
        public PageCore(string dbCon)
        {
            var dbClient = new DBCore(dbCon);
            Db = dbClient?.Db;
        }
        SqlSugarClient Db { get; set; }
        public List<T> GetPage(PageItem item, ref int totals) => Db?.SqlQueryable<T>(item.PageSql).OrderBy(string.IsNullOrWhiteSpace(item.PageSort) ? " [UTime] DESC" : item.PageSort).ToPageList(item.PageIndex <= 0 ? 1 : item.PageIndex, item.PageSize <= 0 ? 10 : item.PageSize, ref totals);


    }
    public class PageItem
    {
        public string PageSql { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string PageSort { get; set; }
    }
}
