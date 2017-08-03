// <copyright file="PostVehicleInTransitTransferReceivingCreate.cs" company="">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author></author>
// <date>8/3/2017 3:48:39 PM</date>
// <summary>Implements the PostVehicleInTransitTransferReceivingCreate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace GSC.Rover.DMS.Platform.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using GSC.Rover.DMS.BusinessLogic.VehicleInTransitTransferReceiving;

    /// <summary>
    /// PostVehicleInTransitTransferReceivingCreate Plugin.
    /// </summary>    
    public class PostVehicleInTransitTransferReceivingCreate : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostVehicleInTransitTransferReceivingCreate"/> class.
        /// </summary>
        public PostVehicleInTransitTransferReceivingCreate()
            : base(typeof(PostVehicleInTransitTransferReceivingCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "gsc_iv_vehicleintransittransferreceiving", new Action<LocalPluginContext>(ExecutePostVehicleInTransitTransferReceivingCreate)));

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
        protected void ExecutePostVehicleInTransitTransferReceivingCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;
            Entity entity = (Entity)context.InputParameters["Target"];

            string message = context.MessageName;
            string error = "";

            try
            {              
                VehicleInTransitTransferReceivingHandler receivingHandler = new VehicleInTransitTransferReceivingHandler(service, trace);
                // check if in-transit transfer already exists.
                receivingHandler.DetectDuplicate(entity);
                // populate in-transit transfer details               

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            // TODO: Implement your custom Plug-in business logic.
        }
    }
}
