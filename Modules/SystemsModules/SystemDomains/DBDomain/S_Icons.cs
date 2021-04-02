using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SystemsModules.Domains
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("S_Icons")]
    public partial class S_Icons
    {
           public S_Icons(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string Id {get;set;}

           /// <summary>
           /// Desc:图标名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string IconName {get;set;}

           /// <summary>
           /// Desc:图标类
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string IconClass {get;set;}

           /// <summary>
           /// Desc:删除标识
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int IsDeleted {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime CTime {get;set;}

           /// <summary>
           /// Desc:修改时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime UTime {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Remark {get;set;}

    }
}
