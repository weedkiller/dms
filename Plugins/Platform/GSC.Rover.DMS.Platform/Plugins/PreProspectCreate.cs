// <copyright file="PreProspectCreate.cs" company="">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author></author>
// <date>4/18/2016 5:45:47 PM</date>
// <summary>Implements the PreProspectCreate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace GSC.Rover.DMS.Platform.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using GSC.Rover.DMS.BusinessLogic.Prospect;

    /// <summary>
    /// PreProspectCreate Plugin.
    /// </summary>    
    public class PreProspectCreate: Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreProspectCreate"/> class.
        /// </summary>
        public PreProspectCreate()
            : base(typeof(PreProspectCreate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "gsc_sls_prospect", new Action<LocalPluginContext>(ExecutePreProspectCreate)));

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
        protected void ExecutePreProspectCreate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;
            Entity prospectEntity = (Entity)context.InputParameters["Target"];

            try
            {
                ProspectHandler prospect = new ProspectHandler(service, trace);
                if (prospect.CheckForExistingRecords(prospectEntity) == true)
                {
                    throw new InvalidPluginExecutionException("This record has been identified as a fraud account. Please ask the customer to provide further information.");
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("This record has been identified as a fraud account. Please ask the customer to provide further information."))
                    throw new InvalidPluginExecutionException(ex.Message);
                else
                    throw new InvalidPluginExecutionException(String.Concat("(Exception)\n", ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, ex));
            }

        }
    }
}
