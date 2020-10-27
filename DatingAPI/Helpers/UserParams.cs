using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Helpers
{
  public class UserParams
  {
    private int pageSize = 10;
    private const int MaxPageSize = 20;
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
      get { return pageSize; }
      set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
    }
  }
}
