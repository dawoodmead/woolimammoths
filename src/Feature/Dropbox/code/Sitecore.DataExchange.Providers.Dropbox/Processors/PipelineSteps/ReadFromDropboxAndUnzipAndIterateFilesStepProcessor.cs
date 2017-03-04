using System;
using System.IO;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Processors.PipelineSteps;
using Sitecore.DataExchange.Providers.Dropbox.Endpoints.Extensions;
using Sitecore.DataExchange.Providers.Dropbox.Helpers;
using Sitecore.DataExchange.Providers.Dropbox.Plugins;
using Sitecore.Resources.Media;

namespace Sitecore.DataExchange.Providers.Dropbox.Processors.PipelineSteps
{
    [RequiredEndpointPlugins(typeof(DropboxSettings))]
    public class ReadFromDropboxAndUnzipAndIterateFilesStepProcessor : BaseReadDataStepProcessor
    {
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
            var settings = endpoint.GetDropboxSettings();
            if (settings == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(settings.DropboxUrl))
            {
                logger.Error(
                    "No Dropbox Url is specified on the endpoint. " +
                    "(pipeline step: {0}, endpoint: {1})",
                    pipelineStep.Name, endpoint.Name);
                return;
            }
            if (!Directory.Exists(settings.FilesDirectory))
            {
                logger.Error(
                    "The Files Directory specified on the endpoint does not exist. " +
                    "(pipeline step: {0}, endpoint: {1}, path: {2})",
                    pipelineStep.Name, endpoint.Name, settings.FilesDirectory);
                return;
            }

            var folder = DownloadAndUnzipHelper.Run(settings.DropboxUrl);

            foreach (var filePath in Directory.GetFiles(folder))
            {
                try
                {
                        var mediaCreator = new MediaCreator();
                        var mediaCreatorOptions = new MediaCreatorOptions();

                        var fi = new FileInfo(filePath);
                        var fs = fi.OpenRead();

                        mediaCreator.CreateFromFile(filePath, mediaCreatorOptions);
                        fs.Close();
                }
                catch (Exception ex)
                {
                    logger.Error("Error while processing files (pipeline step: {0}, endpoint: {1}, path: {2}, exception message: {3})",
                        pipelineStep.Name, endpoint.Name, filePath, ex.ToString());
                    throw ex;
                }
            }
        }
    }
}