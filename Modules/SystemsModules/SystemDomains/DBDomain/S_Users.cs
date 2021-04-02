using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SystemsModules.Domains
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("S_Users")]
    public partial class S_Users
    {
           public S_Users(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string Id {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime CTime {get;set;}

           /// <summary>
           /// Desc:创建人
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Creator {get;set;}

           /// <summary>
           /// Desc:修改时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime UTime {get;set;}

           /// <summary>
           /// Desc:修改人
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Modifier {get;set;}

           /// <summary>
           /// Desc:删除标识
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int IsDeleted {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Remark {get;set;}

           /// <summary>
           /// Desc:登录名
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string UName {get;set;}

           /// <summary>
           /// Desc:登录密码
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PwdVal {get;set;}

           /// <summary>
           /// Desc:昵称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Nickname {get;set;}

           /// <summary>
           /// Desc:真实姓名
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RealName {get;set;}

           /// <summary>
           /// Desc:电话号码
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PhoneNo {get;set;}

           /// <summary>
           /// Desc:身份证号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string IDNO {get;set;}

           /// <summary>
           /// Desc:性别
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int GenderVal {get;set;}

           /// <summary>
           /// Desc:婚姻状况
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int MaritalStatus {get;set;}

           /// <summary>
           /// Desc:QQ号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string QQNo {get;set;}

           /// <summary>
           /// Desc:微信号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string WechatNo {get;set;}

           /// <summary>
           /// Desc:家庭地址
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RealAddressVal {get;set;}

           /// <summary>
           /// Desc:现住址
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string NowAddressVal {get;set;}

           /// <summary>
           /// Desc:职业
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ProfessionVal {get;set;}

           /// <summary>
           /// Desc:用户类型
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int UType {get;set;}

           /// <summary>
           /// Desc:用户状态
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int UStatus {get;set;}

           /// <summary>
           /// Desc:头像
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Avatar {get;set;}

    }
}
