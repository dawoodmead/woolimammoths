using System;
using Sitecore.DataExchange.Converters.PipelineSteps;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Providers.Dropbox.Models.ItemModels.PipelineSteps;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace Sitecore.DataExchange.Providers.Dropbox.Converters.PipelineSteps
{
    public class ReadFromDropboxAndUnzipAndIterateFilesStepConverter : BasePipelineStepConverter<ItemModel>
    {
        private static readonly Guid TemplateId = Guid.Parse("6F64D12A-7AE2-4DD7-B747-6BC800ED1E64");
        public ReadFromDropboxAndUnzipAndIterateFilesStepConverter(IItemModelRepository repository) : base(repository)
        {
            SupportedTemplateIds.Add(TemplateId);
        }
        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            AddEndpointSettings(source, pipelineStep);
        }
        private void AddEndpointSettings(ItemModel source, PipelineStep pipelineStep)
        {
            var settings = new EndpointSettings();
            var endpointFrom = ConvertReferenceToModel<Endpoint>(source, ReadFromDropboxAndUnzipAndIterateFilesStepItemModel.EndpointFrom);
            if (endpointFrom != null)
            {
                settings.EndpointFrom = endpointFrom;
            }
            pipelineStep.Plugins.Add(settings);
        }
    }
}
