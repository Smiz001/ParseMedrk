﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [Browsable(false)]
    public string Description { get; set; } = string.Empty;
    public long Id { get; set; } = 0;
    public string UrlImage { get; set; } = string.Empty;
    [Browsable(false)]
    public string InfoFromTable { get; set; } = string.Empty;
    [Browsable(false)]
    public List<Characteristic> Characteristics { get; set; } = new List<Characteristic>();
  }
}
