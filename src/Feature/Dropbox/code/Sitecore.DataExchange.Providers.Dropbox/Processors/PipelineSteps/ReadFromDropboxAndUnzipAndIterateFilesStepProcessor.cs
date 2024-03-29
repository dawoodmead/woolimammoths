﻿using System;
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
    [RequiredEndpointPlugins(typeof (DropboxSettings))]
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
                    "No Dropbox Url or Files Directory is specified on the endpoint. " +
                    "(pipeline step: {0}, endpoint: {1})",
                    pipelineStep.Name, endpoint.Name);
                return;
            }


            DownloadAndUnzipHelper.Run(settings.DropboxUrl);

            logger.Info("All files updated successfully");
        }
    }
}