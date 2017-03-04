// Decompiled with JetBrains decompiler
// Type: Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.UpdateSitecoreItemStepProcessor
// Assembly: Sitecore.DataExchange.Providers.Sc, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 007A1EE1-2A0C-4DCC-8CF4-9F7ABCCADF6B
// Assembly location: C:\inetpub\wwwroot\Hackathon\Website\bin\Sitecore.DataExchange.Providers.Sc.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Data;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using Sitecore.DataExchange.Providers.Sc.Extensions;
using Sitecore.DataExchange.Providers.Sc.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Resources.Media;
using Sitecore.Services.Core.Model;

namespace Examples.FileSystem.Processors.PipelineSteps
{
    [RequiredPipelineStepPlugins(new Type[] { typeof(EndpointSettings) })]
    public class UpdateSitecoreItemStep : BasePipelineStepProcessor
    {
        public override void Process(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            if (!this.CanProcess(pipelineStep, pipelineContext)) 
                return;
            IItemModelRepository itemModelRepository = this.GetItemModelRepository(pipelineStep, pipelineContext);
            if (itemModelRepository == null)
                return;
            IEnumerable<ItemModel> objectAsItemModels = this.GetTargetObjectAsItemModels(pipelineStep, pipelineContext);
            if (objectAsItemModels == null)
                return;
            Guid id = Guid.Empty;
            foreach (ItemModel itemModel in objectAsItemModels)
            {
                this.FixItemModel(itemModel);
                string language = itemModel.ContainsKey("ItemLanguage") ? itemModel["ItemLanguage"].ToString() : string.Empty;
                if (id == Guid.Empty)
                    id = itemModel.GetItemId();
                bool flag;
                if (id == Guid.Empty)
                {
                    id = itemModelRepository.Create(itemModel);
                    flag = id != Guid.Empty;
                }
                else
                    flag = itemModelRepository.Update(id, itemModel, language, 0);
                var filePath = itemModel["File Path"];
                var a = pipelineContext.GetPlugin<IterableDataSettings>();
                var item = Sitecore.Data.Database.GetDatabase("master").GetItem(new ID(id));

                MediaCreator creator = new MediaCreator();
                MediaCreatorOptions options = new MediaCreatorOptions();

                FileInfo fi = new System.IO.FileInfo(filePath.ToString());
                FileStream fs = fi.OpenRead();

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    item.Fields["File Path"].Value = string.Empty;
                    creator.AttachStreamToMediaItem(fs, item.Paths.FullPath, "iis-85.png", options);
                    item.Editing.EndEdit();

                }
                fs.Close();






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
            foreach (string index in itemModel.Keys.ToArray<string>())
            {
                object obj = itemModel[index];
                if (obj != null)
                    itemModel[index] = (object)obj.ToString();
            }
        }

        private IItemModelRepository GetItemModelRepository(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            EndpointSettings endpointSettings = pipelineStep.GetEndpointSettings();
            if (endpointSettings == null)
                return (IItemModelRepository)null;
            Endpoint endpointTo = endpointSettings.EndpointTo;
            if (endpointTo == null)
                return (IItemModelRepository)null;
            ItemModelRepositorySettings repositorySettings = endpointTo.GetItemModelRepositorySettings();
            if (repositorySettings == null)
                return (IItemModelRepository)null;
            return repositorySettings.ItemModelRepository;
        }

        protected virtual IEnumerable<ItemModel> GetTargetObjectAsItemModels(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            if (!pipelineContext.HasSynchronizationSettings())
                return (IEnumerable<ItemModel>)null;
            SynchronizationSettings synchronizationSettings = pipelineContext.GetSynchronizationSettings();
            if (synchronizationSettings == null)
                return (IEnumerable<ItemModel>)null;
            if (synchronizationSettings.Target == null)
                return (IEnumerable<ItemModel>)null;
            List<ItemModel> itemModelList = new List<ItemModel>();
            ItemModel target1 = synchronizationSettings.Target as ItemModel;
            if (target1 != null)
            {
                itemModelList.Add(target1);
            }
            else
            {
                IDictionary<string, ItemModel> target2 = synchronizationSettings.Target as IDictionary<string, ItemModel>;
                if (target2 != null)
                {
                    foreach (string key in (IEnumerable<string>)target2.Keys)
                        itemModelList.Add(target2[key]);
                }
            }
            if (itemModelList.Count == 0)
                pipelineContext.Logger.Error("The target object is not compatible with the pipeline step processor.");
            return (IEnumerable<ItemModel>)itemModelList;
        }
    }
}
