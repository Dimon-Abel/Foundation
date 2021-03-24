using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.Data
{
    /// <summary>
    /// 数据查询对象
    /// 用于按关键字搜索、单字段排序、分页
    /// </summary>
    public class DataSearch
    {
        public DataSearch()
        {
            _sortType = "descending";
            SortField = "Id";
        }

        public DataSearch(int pageIndex, int pageSize) : this()
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        private string _sortType = "descending";

        /// <summary>
        /// 获取或设置 页索引
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 获取或设置 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 获取或设置 查询关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 获取或设置 排序字段
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// 获取或设置 排序方式
        /// </summary>
        public string SortType
        {
            get { return _sortType; }
            set
            {
                if (value != null)
                {
                    if (value.ToLower() == "asc")
                        value = "ascending";
                    else if (value.ToLower() == "desc")
                        value = "descending";
                    else if (value != "descending" && value != "ascending")
                        value = null;
                }
                _sortType = value ?? "descending"; // 为null时默认为倒序
            }
        }
        /// <summary>
        /// 总记录数（输出字段）
        /// </summary>
        public int TotalCount { get; set; }
    }
}
