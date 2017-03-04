using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using System;
using System.Collections.Generic;
using System.IO;
using Examples.FileSystem.Plugins;

namespace Examples.FileSystem.Processors.PipelineSteps
{
    [RequiredEndpointPlugins(typeof(TextFileSettings))]
    public class ReadFilesStepProcessor : BaseReadDataStepProcessor
    {
        public ReadFilesStepProcessor()
        {
        }
        protected override void ReadData(
            Endpoint endpoint,
            PipelineStep pipelineStep,
            PipelineContext pipelineContext)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (pipelineStep == null)
            {
                throw new ArgumentNullException(nameof(pipelineStep));
            }
            if (pipelineContext == null)
            {
                throw new ArgumentNullException(nameof(pipelineContext));
            }
            var logger = pipelineContext.PipelineBatchContext.Logger;
            //
            //get the file path from the plugin on the endpoint
            var settings = endpoint.GetTextFileSettings();
            if (settings == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(settings.Path))
            {
                logger.Error(
                    "No path is specified on the endpoint. " +
                    "(pipeline step: {0}, endpoint: {1})",
                    pipelineStep.Name, endpoint.Name);
                return;
            }

            if (!Directory.Exists(settings.Path))
            {
                logger.Error(
                    "The path specified on the endpoint does not exist. " +
                    "(pipeline step: {0}, endpoint: {1}, path: {2})",
                    pipelineStep.Name, endpoint.Name, settings.Path);
                return;
            }

            var files = Directory.GetFiles(settings.Path,"*.*",SearchOption.AllDirectories);
            var dataSettings = new IterableDataSettings(files);
            pipelineContext.Plugins.Add(dataSettings);
            
        }
    }
}
