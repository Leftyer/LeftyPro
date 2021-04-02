using CachesModules.Domains;
using CachesModules.Utilitys;
using Leftyer.Domains.Utilitys;
using Newtonsoft.Json;
using System;

namespace CachesModules.Managers
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class CachesManager : DBContexts<CacheInfoModel>
    {
        CachesManager() : base(ConfigCore.Instance.DBCon) { }
        public static CachesManager Instance => new();

        public T GetCache<T>(string key)
        {
            try
            {
                var model = CurrentDB.GetSingle(m => m.IsDeleted == (int)Utilitys.EnumsCore.IsDeleted.NotDelete && m.CacheKey == key) ?? new CacheInfoModel();
                if (string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.CacheVals))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(model.CacheVals);
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
                return default;
            }
        }

        public bool SetCache<T>(string key, string val, string operatorName = "", string remark = "")
        {
            try
            {
                return CurrentDB.Insert(new CacheInfoModel
                {
                    Id = Guid.NewGuid().ToString(),
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    Creator = operatorName,
                    Modifier = operatorName,
                    IsDeleted = (int)Utilitys.EnumsCore.IsDeleted.NotDelete,
                    Remark = remark,
                    CacheKey = key,
                    CacheVals = val
                });
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
                return false;
            }
        }

        public bool DelCache<T>(string key)
        {
            try
            {
                return CurrentDB.Delete(m => m.CacheKey == key);
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
                return false;
            }
        }


    }
}
