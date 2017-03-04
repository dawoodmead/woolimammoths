using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Services.Core.Model;

namespace Examples.FileSystem.Models.ItemModels.Endpoints
{
    public class TextFileEndpointItemModel : ItemModel
    {
        public const string ColumnSeparator = "ColumnSeparator";
        public const string ColumnHeadersInFirstLine = "ColumnHeadersInFirstLine";
        public const string Path = "Path";
    }
}
