using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SystemsModules.Domains
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("S_Pages")]
    public partial class S_Pages
    {
           public S_Pages(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string Id {get;set;}

           /// <summary>
           /// Desc:页面名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PageTitle {get;set;}

           /// <summary>
           /// Desc:页面图标
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PageIcon {get;set;}

           /// <summary>
           /// Desc:页面排序
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int PageSort {get;set;}

           /// <summary>
           /// Desc:页面路径
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PagePathUrl {get;set;}

           /// <summary>
           /// Desc:页面级别
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ParentLevel {get;set;}

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
