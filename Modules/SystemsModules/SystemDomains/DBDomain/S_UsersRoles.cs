using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SystemsModules.Domains
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("S_UsersRoles")]
    public partial class S_UsersRoles
    {
           public S_UsersRoles(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string Id {get;set;}

           /// <summary>
           /// Desc:用户ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string UsersId {get;set;}

           /// <summary>
           /// Desc:角色ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RolesId {get;set;}

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
