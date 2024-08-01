using System;
using System.Collections.Generic;

#nullable disable

namespace MinimalApiSample.Models
{
    public partial class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IsCompleted { get; set; }
    }
}
