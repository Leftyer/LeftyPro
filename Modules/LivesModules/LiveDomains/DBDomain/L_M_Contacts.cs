using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace LivesModules.Domains
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("L_M_Contacts")]
    public partial class L_M_Contacts
    {
           public L_M_Contacts(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Id {get;set;}

           /// <summary>
           /// Desc:联系人姓名
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ContactName {get;set;}

           /// <summary>
           /// Desc:联系人内容
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ContactContent {get;set;}

           /// <summary>
           /// Desc:优先级
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int ContactType {get;set;}

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
