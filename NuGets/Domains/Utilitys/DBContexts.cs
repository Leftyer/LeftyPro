using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Leftyer.Domains.Utilitys
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public abstract class DBContexts<T> : DBCore where T : class, new()
    {
        public DBContexts(string dbCon) : base(dbCon){}

        #region normal operation
        public SimpleClient<T> CurrentDB => Db.GetSimpleClient<T>();
        public bool Insert(T model) => Db.Insertable(model).ExecuteCommand() > 0;
        public T InsertEntity(T model) => Db.Insertable(model).ExecuteReturnEntity();
        public bool Delete(Expression<Func<T, bool>> whereExpression) => Db.Deleteable(whereExpression).ExecuteCommandHasChange();
        public bool Update(T model) => Db.Updateable(model).ExecuteCommandHasChange();
        public bool Update(Expression<Func<T, bool>> whereExpression) => Db.Updateable(whereExpression).ExecuteCommandHasChange();
        public bool Update(List<T> list) => Db.Updateable(list).ExecuteCommandHasChange();
        #endregion



    }
}
