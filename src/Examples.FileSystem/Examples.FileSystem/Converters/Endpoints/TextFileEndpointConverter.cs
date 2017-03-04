using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examples.FileSystem.Models.ItemModels.Endpoints;
using Examples.FileSystem.Plugins;
using Sitecore.DataExchange.Converters.Endpoints;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace Examples.FileSystem.Converters.Endpoints
{
    public class TextFileEndpointConverter : BaseEndpointConverter<ItemModel>
    {
        private static readonly Guid TemplateId = Guid.Parse("74A31BDD-0619-4223-B8C4-3FF4D316A2FD");
        public TextFileEndpointConverter(IItemModelRepository repository) : base(repository)
        {
            //
            //identify the template an item must be based
            //on in order for the converter to be able to
            //convert the item
            this.SupportedTemplateIds.Add(TemplateId);
        }
        protected override void AddPlugins(ItemModel source, Endpoint endpoint)
        {
            //
            //create the plugin
            var settings = new TextFileSettings();
            //
            //populate the plugin using values from the item
            settings.ColumnHeadersInFirstLine =
                base.GetBoolValue(source, TextFileEndpointItemModel.ColumnHeadersInFirstLine);
            settings.ColumnSeparator =
                base.GetStringValue(source, TextFileEndpointItemModel.ColumnSeparator);
            settings.Path =
                base.GetStringValue(source, TextFileEndpointItemModel.Path);
            //
            //add the plugin to the endpoint
            endpoint.Plugins.Add(settings);
        }
    }
}
