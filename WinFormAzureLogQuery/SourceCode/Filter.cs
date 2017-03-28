using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormAzureLogQuery.SourceCode
{
    public class FilterSet
    {
        public const int levelCount = 5;
        public int LevelTotalCount { get { return levelCount; } }
        public List<String> Levels { get; set; }
        public string Keyword { get; set; }
        

    }
}
