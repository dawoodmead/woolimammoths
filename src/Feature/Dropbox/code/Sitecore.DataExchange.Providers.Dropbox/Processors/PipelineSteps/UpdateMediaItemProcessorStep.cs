using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using Sitecore.DataExchange.Providers.Sc.Extensions;
using Sitecore.DataExchange.Repositories;
using Sitecore.Resources.Media;
using Sitecore.Services.Core.Model;

namespace Sitecore.DataExchange.Providers.Dropbox.Processors.PipelineSteps
{
    [RequiredPipelineStepPlugins(new Type[] { typeof(EndpointSettings) })]
    public class UpdateMediaItemProcessorStep : BasePipelineStepProcessor
    {
        public override void Process(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            if (!CanProcess(pipelineStep, pipelineContext))
                return;
            var itemModelRepository = GetItemModelRepository(pipelineStep);
            if (itemModelRepository == null)
                return;
            var objectAsItemModels = GetTargetObjectAsItemModels(pipelineStep, pipelineContext);
            if (objectAsItemModels == null)
                return;
            var id = Guid.Empty;
            foreach (var itemModel in objectAsItemModels)
            {
                FixItemModel(itemModel);
                var language = itemModel.ContainsKey("ItemLanguage") ? itemModel["ItemLanguage"].ToString() : string.Empty;
                if (id == Guid.Empty)
                    id = itemModel.GetItemId();
                bool flag;
                if (id == Guid.Empty)
                {
                    id = itemModelRepository.Create(itemModel);
                    flag = id != Guid.Empty;
                }
                else
                    flag = itemModelRepository.Update(id, itemModel, language);
               
                if (!flag)
                {
                    pipelineContext.Logger.Error("Item was not saved. (id: {0}, language: {1})", (object)id, (object)language);
                    break;
                }
                pipelineContext.Logger.Debug("Item was saved. (id: {0}, language: {1})", (object)id, (object)language);
            }
        }

        protected virtual void FixItemModel(ItemModel itemModel)
        {
            if (itemModel == null)
                return;
            foreach (var index in itemModel.Keys.ToArray())
            {
                var obj = itemModel[index];
                if (obj != null)
                    itemModel[index] = obj.ToString();
            }
        }

        private IItemModelRepository GetItemModelRepository(PipelineStep pipelineStep)
        {
            var endpointSettings = pipelineStep.GetEndpointSettings();
            var endpointTo = endpointSettings?.EndpointTo;
            var repositorySettings = endpointTo?.GetItemModelRepositorySettings();
            return repositorySettings?.ItemModelRepository;
        }

        protected virtual IEnumerable<ItemModel> GetTargetObjectAsItemModels(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            if (!pipelineContext.HasSynchronizationSettings())
                return null;
            var synchronizationSettings = pipelineContext.GetSynchronizationSettings();
            if (synchronizationSettings?.Target == null)
                return null;
            var itemModelList = new List<ItemModel>();
            var target1 = synchronizationSettings.Target as ItemModel;
            if (target1 != null)
            {
                itemModelList.Add(target1);
            }
            else
            {
                var target2 = synchronizationSettings.Target as IDictionary<string, ItemModel>;
                if (target2 != null)
                {
                    foreach (var key in target2.Keys)
                        itemModelList.Add(target2[key]);
                }
            }
            if (itemModelList.Count == 0)
                pipelineContext.Logger.Error("The target object is not compatible with the pipeline step processor.");
            return itemModelList;
        }
    }
}
