// <copyright file="PreValidatePurchaseOrderDelete.cs" company="">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author></author>
// <date>6/29/2016 11:01:29 AM</date>
// <summary>Implements the PreValidatePurchaseOrderDelete Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace GSC.Rover.DMS.Platform.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using GSC.Rover.DMS.BusinessLogic.Common;
    using GSC.Rover.DMS.BusinessLogic.VehiclePurchaseOrder;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// PreValidatePurchaseOrderDelete Plugin.
    /// </summary>    
    public class PreValidatePurchaseOrderDelete: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreValidatePurchaseOrderDelete"/> class.
        /// </summary>
        public PreValidatePurchaseOrderDelete()
            : base(typeof(PreValidatePurchaseOrderDelete))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(10, "Delete", "gsc_cmn_purchaseorder", new Action<LocalPluginContext>(ExecutePreValidatePurchaseOrderDelete)));

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
        protected void ExecutePreValidatePurchaseOrderDelete(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;
            var vehiclePurchaseOrderEntity = (EntityReference)context.InputParameters["Target"];
            string message = context.MessageName;
            string error = "";

            if (context.Depth > 1) { return; }

            try
            {
                VehiclePurchaseOrderHandler vpoHandler = new VehiclePurchaseOrderHandler(service, trace);

                EntityCollection vpoCollection = CommonHandler.RetrieveRecordsByOneValue(vehiclePurchaseOrderEntity.LogicalName, "gsc_cmn_purchaseorderid", vehiclePurchaseOrderEntity.Id, service,
                    null, OrderType.Ascending, new[] { "gsc_cmn_purchaseorderid", "gsc_vpostatus" });

                if (vpoCollection.Entities.Count > 0)
                {
                    if (vpoHandler.ValidateDelete(vpoCollection.Entities[0]) != true)
                        throw new InvalidPluginExecutionException("Unable to delete VPO record(s)");
                }
            }
                
            catch (Exception ex)
            {
                    throw new InvalidPluginExecutionException(ex.Message);
                   // throw new InvalidPluginExecutionException(String.Concat("(Exception)\n", ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, error));
            }

        }
    }
}
