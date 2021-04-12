using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace LivesModules.Domains
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("L_M_Memo")]
    public partial class L_M_Memo
    {
           public L_M_Memo(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string Id {get;set;}

           /// <summary>
           /// Desc:备忘录
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MemoContent {get;set;}

           /// <summary>
           /// Desc:优先级
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int MemoLevel {get;set;}

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
