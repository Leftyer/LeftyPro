using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using LivesModules.Utilitys;
using Leftyer.Domains.Utilitys;

namespace LivesModules.Domains
{
    [TestClass]
    public class DBBuild
    {
        [TestMethod]
        public void BuildEntitys()
        {
            using var db = new DBCore(ConfigCore.Instance.DBCon).Db;
            db.DbFirst.IsCreateAttribute().CreateClassFile(GetBuildPath("LiveDomains\\DBDomain\\"), "LivesModules.Domains");
        }

        string GetBuildPath(string path)
        {
            var filePath = string.Empty;
            var paths = System.IO.Directory.GetCurrentDirectory().Split('\\').ToList();
            paths.RemoveRange(paths.Count - 3, 3);
            paths.ForEach(m => { filePath += m + "\\"; });
            return filePath + path;
        }
    }
}
