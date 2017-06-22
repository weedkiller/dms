// <copyright file="PreValidateContactDelete.cs" company="">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author></author>
// <date>12/7/2016 1:24:05 PM</date>
// <summary>Implements the PreValidateContactDelete Plugin.</summary>
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
    using Microsoft.Xrm.Sdk.Query;
    using GSC.Rover.DMS.BusinessLogic.Contact;

    /// <summary>
    /// PreValidateContactDelete Plugin.
    /// </summary>    
    public class PreValidateContactDelete: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreValidateContactDelete"/> class.
        /// </summary>
        public PreValidateContactDelete()
            : base(typeof(PreValidateContactDelete))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(10, "Delete", "contact", new Action<LocalPluginContext>(ExecutePreValidateContactDelete)));

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
        protected void ExecutePreValidateContactDelete(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;
            var contact = (EntityReference)context.InputParameters["Target"];
            string message = context.MessageName;

            if (context.Depth > 1) { return; }

            try
            {

                ContactHandler contactHandler = new ContactHandler(service, trace);

                EntityCollection contactCollection = CommonHandler.RetrieveRecordsByOneValue("contact", "contactid", contact.Id, service,
                    null, OrderType.Ascending, new[] { "parentcontactid", "gsc_recordtype", "gsc_ispotential" });

                Entity contactEntity = contactCollection.Entities[0];
                Boolean isProspect = contactEntity.GetAttributeValue<Boolean>("gsc_ispotential");

                if (contactEntity.Contains("gsc_recordtype") && contactEntity.GetAttributeValue<OptionSetValue>("gsc_recordtype").Value == 100000003)
                {
                    contactHandler.ClearPrimaryContactDetails(contactEntity);
                }

                if (contactHandler.IsUsedInTransaction(contactEntity) == true)
                    {
                        throw new InvalidPluginExecutionException("Unable to delete individual record(s) used in transaction");
                    }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("Unable to delete individual record(s) used in transaction"))
                    throw new InvalidPluginExecutionException(ex.Message);
                else
                throw new InvalidPluginExecutionException(String.Concat("(Exception)\n", ex.Message, Environment.NewLine, ex.StackTrace));
            }
        }
    }
}
