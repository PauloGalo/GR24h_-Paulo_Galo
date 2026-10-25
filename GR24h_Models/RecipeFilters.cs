using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class RecipeFilters
    {
        public string? SearchTerm { get; set; }
        public string? Name { get; set; }

        public List<string> Ingredients { get; set; } = new List<string>();

        public List<string> Categories { get; set; } = new List<string>();

        public List<string> Difficulties { get; set; } = new List<string>();

        public string? Author { get; set; }
        public string? SortBy { get; set; }

    }
}
