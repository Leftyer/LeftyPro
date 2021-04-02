using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Leftyer.Domains.Utilitys
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class ExcelCore
    {
        ExcelCore() { }
        public static ExcelCore Instance => new();

        public FileStream Export<T>(string rootPath, string sFileName, string sheetName, List<T> sourceData)
        {
            if (Directory.Exists(rootPath))
            {
                new DirectoryInfo(rootPath).Delete(true);
            }
            Directory.CreateDirectory(rootPath);
            var path = Path.Combine(rootPath, sFileName);
            var file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new(file))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells.LoadFromCollection(sourceData, true);
                package.Save();
            }
            return new FileStream(Path.Combine(rootPath, sFileName), FileMode.Open);
        }


    }

}
