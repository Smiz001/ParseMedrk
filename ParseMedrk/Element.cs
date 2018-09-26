using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMedrk
{
  public class Element
  {
    public string NameElement { get; set; } = string.Empty;
    public string NameCategory { get; set; } = string.Empty;
    public string NameSubCategory { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Id { get; set; } = 0;
  }
}
