// <copyright file="PostQuoteAccessoryUpdate.cs" company="">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author></author>
// <date>9/27/2016 2:30:55 PM</date>
// <summary>Implements the PostQuoteAccessoryUpdate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace GSC.Rover.DMS.Platform.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using GSC.Rover.DMS.BusinessLogic.QuoteAccessory;

    /// <summary>
    /// PostQuoteAccessoryUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostQuoteAccessoryUpdate: Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private readonly string preImageAlias = "preImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// 
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "postImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostQuoteAccessoryUpdate"/> class.
        /// </summary>
        public PostQuoteAccessoryUpdate()
            : base(typeof(PostQuoteAccessoryUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "gsc_sls_quoteaccessory", new Action<LocalPluginContext>(ExecutePostQuoteAccessoryUpdate)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected void ExecutePostQuoteAccessoryUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;

            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
            Entity postImageEntity = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;

            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;
            Entity accessoryEntity = (Entity)context.InputParameters["Target"];

            if (!(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)) { return; }

            if (accessoryEntity.LogicalName != "gsc_sls_quoteaccessory") { return; }

            if (context.Mode == 0) //Synchronous Plug-in
            {
                string message = context.MessageName;

                try
                {
                    QuoteAccessoryHandler quoteAccessoryHandler = new QuoteAccessoryHandler(service, trace);

                    #region pre-Image

                    Guid preImageItem = preImageEntity.GetAttributeValue<EntityReference>("gsc_productid") != null
                        ? preImageEntity.GetAttributeValue<EntityReference>("gsc_productid").Id
                        : Guid.Empty;

                    var preImageFree = preImageEntity.Contains("gsc_free")
                        ? preImageEntity.GetAttributeValue<Boolean>("gsc_free")
                        : false;

                    var preImageActualCost = preImageEntity.Contains("gsc_actualcost")
                        ? preImageEntity.GetAttributeValue<Money>("gsc_actualcost").Value
                        : 0;

                    #endregion

                    #region post-Image

                    Guid postImageItem = postImageEntity.GetAttributeValue<EntityReference>("gsc_productid") != null
                        ? postImageEntity.GetAttributeValue<EntityReference>("gsc_productid").Id
                        : Guid.Empty;

                    var postImageFree = postImageEntity.Contains("gsc_free")
                        ? postImageEntity.GetAttributeValue<Boolean>("gsc_free")
                        : false;

                    var postImageActualCost = postImageEntity.Contains("gsc_actualcost")
                        ? postImageEntity.GetAttributeValue<Money>("gsc_actualcost").Value
                        : 0;

                    #endregion

                    if (preImageItem != postImageItem)
                    {
                        quoteAccessoryHandler.PopulateDetails(postImageEntity, message);
                        quoteAccessoryHandler.SetTotalAccessories(postImageEntity, message);
                    }

                    if (preImageFree != postImageFree)
                    {
                        quoteAccessoryHandler.UpdateActualCost(postImageEntity);
                    }

                    if (preImageActualCost != postImageActualCost)
                    {
                        quoteAccessoryHandler.SetTotalAccessories(postImageEntity, message);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(String.Concat(ex.Message));
                }
            }
        }
    }
}
