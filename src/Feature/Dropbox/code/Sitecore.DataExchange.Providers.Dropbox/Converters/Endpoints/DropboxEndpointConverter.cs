using System;
using Sitecore.DataExchange.Converters.Endpoints;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Providers.Dropbox.Models.ItemModels.Endpoints;
using Sitecore.DataExchange.Providers.Dropbox.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace Sitecore.DataExchange.Providers.Dropbox.Converters.Endpoints
{
    public class DropboxEndpointConverter : BaseEndpointConverter<ItemModel>
    {
        private static readonly Guid TemplateId = Guid.Parse("276EF69B-1361-4ED9-AC10-8E200C252C51");
        public DropboxEndpointConverter(IItemModelRepository repository) : base(repository)
        {
            //
            //identify the template an item must be based
            //on in order for the converter to be able to
            //convert the item
            SupportedTemplateIds.Add(TemplateId);
        }
        protected override void AddPlugins(ItemModel source, Endpoint endpoint)
        {
            //
            //create the plugin
            var settings = new DropboxSettings();
            //
            //populate the plugin using values from the item
            settings.DropboxUrl =
                GetStringValue(source, DropboxEndpointItemModel.DropboxUrl);
            settings.FilesDirectory =
                GetStringValue(source, DropboxEndpointItemModel.FilesDirectory);
            //
            //add the plugin to the endpoint
            endpoint.Plugins.Add(settings);
        }
    }
}