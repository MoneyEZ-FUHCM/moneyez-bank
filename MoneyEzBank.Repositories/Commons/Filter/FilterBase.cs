using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Repositories.Commons.Filter
{
    public class FilterBase
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "field")]
        public string? Field { get; set; }

        [FromQuery(Name = "sort_by")]
        public string? SortBy { get; set; }

        [FromQuery(Name = "dir")]
        public string? Dir { get; set; }

        [FromQuery(Name = "is_deleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
