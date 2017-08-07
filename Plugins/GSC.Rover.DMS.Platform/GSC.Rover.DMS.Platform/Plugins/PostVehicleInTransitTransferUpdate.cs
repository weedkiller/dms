// <copyright file="PostVehicleInTransitTransferUpdate.cs" company="">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author></author>
// <date>8/23/2016 5:32:09 PM</date>
// <summary>Implements the PostVehicleInTransitTransferUpdate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace GSC.Rover.DMS.Platform.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using GSC.Rover.DMS.BusinessLogic.VehicleInTransitTransfer;
    using GSC.Rover.DMS.BusinessLogic.VehicleInTransitTransferReceiving;

    /// <summary>
    /// PostVehicleInTransitTransferUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostVehicleInTransitTransferUpdate: Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// All Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// All Attributes
        /// 
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostVehicleInTransitTransferUpdate"/> class.
        /// </summary>
        public PostVehicleInTransitTransferUpdate()
            : base(typeof(PostVehicleInTransitTransferUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "gsc_iv_vehicleintransittransfer", new Action<LocalPluginContext>(ExecutePostVehicleInTransitTransferUpdate)));

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
        protected void ExecutePostVehicleInTransitTransferUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;

            if (context.Depth > 1)
                return;

            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
            Entity postImageEntity = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;

            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;
            Entity vehicleInTransitTransfer = (Entity)context.InputParameters["Target"];

            if (!(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)) { return; }

            if (vehicleInTransitTransfer.LogicalName != "gsc_iv_vehicleintransittransfer") { return; }

            if (context.Mode == 0) //Synchronous Plug-in
            {
                string message = context.MessageName;

                try
                {
                    string preInventoryId = preImageEntity.Contains("gsc_inventoryidtoallocate") ? preImageEntity.GetAttributeValue<string>("gsc_inventoryidtoallocate")
                        : String.Empty;
                    var preTransferStatus = preImageEntity.Contains("gsc_intransittransferstatus") ? preImageEntity.GetAttributeValue<OptionSetValue>("gsc_intransittransferstatus").Value
                        : 0;
                    String preImageAllocatedItemsToDelete = preImageEntity.Contains("gsc_allocateditemstodelete")
                        ? preImageEntity.GetAttributeValue<String>("gsc_allocateditemstodelete")
                        : String.Empty;

                    string postInventoryId = postImageEntity.Contains("gsc_inventoryidtoallocate") ? postImageEntity.GetAttributeValue<string>("gsc_inventoryidtoallocate")
                        : String.Empty;
                    var postTransferStatus = postImageEntity.Contains("gsc_intransittransferstatus") ? postImageEntity.GetAttributeValue<OptionSetValue>("gsc_intransittransferstatus").Value
                        : 0;
                    String postImageAllocatedItemsToDelete = postImageEntity.Contains("gsc_allocateditemstodelete")
                        ? postImageEntity.GetAttributeValue<String>("gsc_allocateditemstodelete")
                        : String.Empty;

                    VehicleInTransitTransferReceivingHandler receivingHandler = new VehicleInTransitTransferReceivingHandler(service, trace);
                    VehicleInTransitTransferHandler vehicleInTransitHandler = new VehicleInTransitTransferHandler(service, trace, receivingHandler);

                    //BL for update of inventoryidtoallocate
                    if (preInventoryId != postInventoryId && postInventoryId != string.Empty)
                    {
                        vehicleInTransitHandler.AllocateVehicle(postImageEntity);
                    }

                    //BL for status change to Cancelled
                    if (preTransferStatus != postTransferStatus && postTransferStatus == 100000003)
                    {
                        vehicleInTransitHandler.ValidateTransaction(preImageEntity, message);
                    }
                    //BL for status change to Shipped
                    if (preTransferStatus != postTransferStatus && postTransferStatus == 100000001)
                    {
                        vehicleInTransitHandler.ShipVehicle(preImageEntity);
                    }
                    if (preImageAllocatedItemsToDelete != postImageAllocatedItemsToDelete && postImageAllocatedItemsToDelete != String.Empty)
                    {
                        vehicleInTransitHandler.DeleteInTransitTransferVehicle(postImageEntity);
                    }
                }

                catch (Exception ex)
                {
                    if (ex.Message.Contains("The inventory for entered vehicle is not available."))
                        throw new InvalidPluginExecutionException("The inventory for entered vehicle is not available.");
                    else if (ex.Message.Contains("Unable to cancel Shipped Vehicle In-Transit Transfer record."))
                        throw new InvalidPluginExecutionException("Unable to cancel Shipped Vehicle In-Transit Transfer record.");
                    else if (ex.Message.Contains("No Allocated Vehicle to Ship."))
                        throw new InvalidPluginExecutionException("No Allocated Vehicle to Ship.");
                    else
                        throw new InvalidPluginExecutionException(String.Concat("(Exception)\n", ex.Message, Environment.NewLine, ex.StackTrace));
                }
            }
        }
    }
}
