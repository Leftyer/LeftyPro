using System;
using SqlSugar;
namespace CachesModules.Domains
    {
        [SugarTable("CacheInfo")]
        public class CacheInfo
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
            ///创建人
            /// </summary> 
            
            public string Creator { get; set; }   
                         
            /// <summary>
            ///修改时间
            /// </summary> 
            
            public DateTime UTime { get; set; }   
                         
            /// <summary>
            ///修改人
            /// </summary> 
            
            public string Modifier { get; set; }   
                         
            /// <summary>
            ///删除标识
            /// </summary> 
            
            public int IsDeleted { get; set; }   
                         
            /// <summary>
            ///备注
            /// </summary> 
            
            public string Remark { get; set; }   
                         
            /// <summary>
            ///缓存Key
            /// </summary> 
            
            public string CacheKey { get; set; }   
                         
            /// <summary>
            ///缓存Val
            /// </summary> 
            
            public string CacheVals { get; set; }   
               
			#endregion
			
        }    
     }

    