using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMe.Models.DTOs.Responses
{
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int PageSize { get; set; }
    public string? NextCursor { get; set; }
}


}