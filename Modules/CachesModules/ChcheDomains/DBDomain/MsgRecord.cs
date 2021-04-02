using System;
using SqlSugar;
namespace CachesModules.Domains
    {
        [SugarTable("MsgRecord")]
        public class MsgRecord
        {    
		    #region 映射字段
                         
            /// <summary>
            ///主键
            /// </summary> 
             [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }   
                         
            /// <summary>
            ///创建时间
            /// </summary> 
            
            public DateTime CTime { get; set; }   
                         
            /// <summary>
            ///修改时间
            /// </summary> 
            
            public DateTime UTime { get; set; }   
                         
            /// <summary>
            ///删除标识
            /// </summary> 
            
            public int IsDeleted { get; set; }   
                         
            /// <summary>
            ///备注
            /// </summary> 
            
            public string Remark { get; set; }   
                         
            /// <summary>
            ///标题
            /// </summary> 
            
            public string MsgName { get; set; }   
                         
            /// <summary>
            ///详情
            /// </summary> 
            
            public string MsgDetail { get; set; }   
                         
            /// <summary>
            ///
            /// </summary> 
            
            public int MsgIsRead { get; set; }   
               
			#endregion
			
        }    
     }

    