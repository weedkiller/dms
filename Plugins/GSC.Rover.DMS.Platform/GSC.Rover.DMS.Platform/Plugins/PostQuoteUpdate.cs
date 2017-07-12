// <copyright file="PostQuoteUpdate.cs" company="">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/11/2016 1:13:11 PM</date>
// <summary>Implements the PostQuoteUpdate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace GSC.Rover.DMS.Platform.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using GSC.Rover.DMS.BusinessLogic.Quote;

    /// <summary>
    /// PostQuoteUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostQuoteUpdate : Plugin
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
        /// Initializes a new instance of the <see cref="PostQuoteUpdate"/> class.
        /// </summary>
        public PostQuoteUpdate()
            : base(typeof(PostQuoteUpdate))
        {
            base.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "quote", new Action<LocalPluginContext>(ExecutePostQuoteUpdate)));

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
        protected void ExecutePostQuoteUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService trace = localContext.TracingService;

            Entity preImageEntity = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
            Entity postImageEntity = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;

            if (!(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)) { return; }

            if (postImageEntity.LogicalName != "quote") { return; }

            string message = context.MessageName;

            if (context.Mode == 0) //synchronous plugin
            {
                try
                {
                    QuoteHandler quotehandler = new QuoteHandler(service, trace);

                    var status = postImageEntity.Contains("statecode")
                        ? postImageEntity.GetAttributeValue<OptionSetValue>("statecode").Value.ToString()
                        : "";

                    if (status == "0")
                    {
                        #region Call functions that will run when Vehicle Model (gsc_productid) was changed.

                        var preImageProductId = preImageEntity.Contains("gsc_productid")
                                ? preImageEntity.GetAttributeValue<EntityReference>("gsc_productid").Id
                                : Guid.Empty;

                        var postImageProductId = postImageEntity.Contains("gsc_productid")
                                    ? postImageEntity.GetAttributeValue<EntityReference>("gsc_productid").Id
                                    : Guid.Empty;

                        if (preImageProductId != postImageProductId)
                        {
                            quotehandler.CheckifVehicleHasTax(postImageEntity);
                            quotehandler.ConcatenateVehicleDescription(postImageEntity, message);
                            quotehandler.DeleteExistingVehicleFreebies(postImageEntity, message);
                            quotehandler.DeleteExistingCabChassis(postImageEntity);
                          /*  if (quotehandler.CheckIfHasUnitPrice(postImageEntity) == true)
                            {
                                throw new InvalidPluginExecutionException("Cannot update quote. The selected vehicle model does not have a unit price. Please contact the administrator.");
                            }*/
                        }

                        #endregion

                        #region Call functions that will run when Preferred Color 1 was changed.

                        var preImageColorId = preImageEntity.Contains("gsc_vehiclecolorid1")
                                ? preImageEntity.GetAttributeValue<EntityReference>("gsc_vehiclecolorid1").Id
                                : Guid.Empty;

                        var postImageColroId = postImageEntity.Contains("gsc_vehiclecolorid1")
                                    ? postImageEntity.GetAttributeValue<EntityReference>("gsc_vehiclecolorid1").Id
                                    : Guid.Empty;

                        if (preImageColorId != postImageColroId)
                        {
                           quotehandler.PopulateColorPrice(postImageEntity, message);
                        }

                        #endregion

                        #region Call functions that will run when Financing Scheme was changed.

                        var preImageFinancingSchemeId = preImageEntity.Contains("gsc_financingschemeid")
                                ? preImageEntity.GetAttributeValue<EntityReference>("gsc_financingschemeid").Id
                                : Guid.Empty;

                        var postImageFinancingSchemeId = postImageEntity.Contains("gsc_financingschemeid")
                                    ? postImageEntity.GetAttributeValue<EntityReference>("gsc_financingschemeid").Id
                                    : Guid.Empty;

                        if (preImageFinancingSchemeId != postImageFinancingSchemeId)
                        {
                            quotehandler.CheckMonthlyAmortizationRecord(postImageEntity);
                        }

                        #endregion

                        #region Call functions that will run when Total Insurance Charges was changed.

                        var preImageTotalInsurance = preImageEntity.Contains("gsc_totalinsurancecharges")
                               ? preImageEntity.GetAttributeValue<Money>("gsc_totalinsurancecharges").Value
                               : new Decimal(0);

                        var postImageTotalInsurance = postImageEntity.Contains("gsc_totalinsurancecharges")
                                    ? postImageEntity.GetAttributeValue<Money>("gsc_totalinsurancecharges").Value
                                    : new Decimal(0);

                        if (preImageTotalInsurance != postImageTotalInsurance)
                        {
                            quotehandler.UpdateInsurance(postImageEntity, message);
                        }

                        #endregion

                        #region Call functions that will run when Amount in AF, UP and AP were updated.

                        var preImageDpAmount = preImageEntity.Contains("gsc_applytodpamount")
                               ? preImageEntity.GetAttributeValue<Money>("gsc_applytodpamount").Value
                               : new Decimal(0);

                        var postImageDpAmount = postImageEntity.Contains("gsc_applytodpamount")
                                    ? postImageEntity.GetAttributeValue<Money>("gsc_applytodpamount").Value
                                    : new Decimal(0);

                        var preImageAfAmount = preImageEntity.Contains("gsc_applytoafamount")
                               ? preImageEntity.GetAttributeValue<Money>("gsc_applytoafamount").Value
                               : new Decimal(0);
                        var preImageAmountFinanced = preImageEntity.Contains("gsc_amountfinanced")
                                ? preImageEntity.GetAttributeValue<Money>("gsc_amountfinanced").Value
                                : new Decimal(0);

                        var postImageAfAmount = postImageEntity.Contains("gsc_applytoafamount")
                                    ? postImageEntity.GetAttributeValue<Money>("gsc_applytoafamount").Value
                                    : new Decimal(0);

                        var preImageUpAmount = preImageEntity.Contains("gsc_applytoupamount")
                               ? preImageEntity.GetAttributeValue<Money>("gsc_applytoupamount").Value
                               : new Decimal(0);

                        var postImageUpAmount = postImageEntity.Contains("gsc_applytoupamount")
                                    ? postImageEntity.GetAttributeValue<Money>("gsc_applytoupamount").Value
                                    : new Decimal(0);
                        var postImageAmountFinanced = postImageEntity.Contains("gsc_amountfinanced")
                                ? postImageEntity.GetAttributeValue<Money>("gsc_amountfinanced").Value
                                : new Decimal(0);

                        if (preImageAfAmount != postImageAfAmount || preImageUpAmount != postImageUpAmount || preImageDpAmount != postImageDpAmount || preImageAmountFinanced != postImageAmountFinanced)
                        {
                            quotehandler.SetLessDiscountValues(postImageEntity);
                        }

                        #endregion

                        #region Call functions that will run when Net Downpayment was updated.

                        var preImageNetDownpayment = preImageEntity.Contains("gsc_netdownpayment")
                               ? preImageEntity.GetAttributeValue<Money>("gsc_netdownpayment").Value
                               : new Decimal(0);

                        var postImageNetDownpayment = postImageEntity.Contains("gsc_netdownpayment")
                                    ? postImageEntity.GetAttributeValue<Money>("gsc_netdownpayment").Value
                                    : new Decimal(0);


                        if (preImageNetDownpayment != postImageNetDownpayment)
                        {
                            quotehandler.SetLessDiscountValues(postImageEntity);
                        }

                        #endregion

                        #region Call functions that will run when Product, Bank and Free Chattel Feewere updated.

                        var preImageFreeChattel = preImageEntity.Contains("gsc_freechattelfee")
                               ? preImageEntity.GetAttributeValue<Boolean>("gsc_freechattelfee")
                               : false;

                        var postImageFreeChattel = postImageEntity.Contains("gsc_freechattelfee")
                               ? postImageEntity.GetAttributeValue<Boolean>("gsc_freechattelfee")
                               : false;

                        var preImageBankId = preImageEntity.Contains("gsc_bankid")
                               ? preImageEntity.GetAttributeValue<EntityReference>("gsc_bankid").Id
                               : Guid.Empty;

                        var postImageBankId = postImageEntity.Contains("gsc_bankid")
                                    ? postImageEntity.GetAttributeValue<EntityReference>("gsc_bankid").Id
                                    : Guid.Empty;

                        if (preImageProductId != postImageProductId || preImageBankId != postImageBankId || preImageFreeChattel != postImageFreeChattel)
                        {
                            quotehandler.SetChattelFeeAmount(postImageEntity, message);
                        }

                        #endregion

                        //Created By : Jerome Anthony Gerero, Created On : 9/15/2016
                        #region Call function on Chattle Fee (gsc_chattelfeeeditable) change

                        Decimal preImageChattelFeeEditable = preImageEntity.Contains("gsc_chattelfeeeditable")
                            ? preImageEntity.GetAttributeValue<Money>("gsc_chattelfeeeditable").Value
                            : Decimal.Zero;

                        Decimal postImageChattelFeeEditable = postImageEntity.Contains("gsc_chattelfeeeditable")
                            ? postImageEntity.GetAttributeValue<Money>("gsc_chattelfeeeditable").Value
                            : Decimal.Zero;

                        if (preImageChattelFeeEditable != postImageChattelFeeEditable)
                        {
                            quotehandler.ReplicateEditableChattelFee(postImageEntity);
                        }

                        #endregion

                        #region Call functions on Close Quote change

                        Boolean preImageCloseQuote = preImageEntity.GetAttributeValue<Boolean>("gsc_closequote");

                        Boolean postImageCloseQuote = postImageEntity.GetAttributeValue<Boolean>("gsc_closequote");

                        if (preImageCloseQuote != postImageCloseQuote)
                        {
                            quotehandler.CloseRelatedOpportunity(postImageEntity);
                        }

                        #endregion

                        #region Call functions of Create Order change

                        Boolean preImageCreateOrder = preImageEntity.GetAttributeValue<Boolean>("gsc_createorder");

                        Boolean postImageCreateOrder = postImageEntity.GetAttributeValue<Boolean>("gsc_createorder");

                        if (preImageCreateOrder != postImageCreateOrder)
                        {
                            quotehandler.CreateOrder(postImageEntity);
                            quotehandler.ConvertPotentialtoCustomer(postImageEntity);
                        }

                        #endregion

                        #region Call functions that will run when Customer Id was updated.

                        var preCustomerId = preImageEntity.Contains("customerid")
                               ? preImageEntity.GetAttributeValue<EntityReference>("customerid").Id
                               : Guid.Empty;

                        var postCustomerId = postImageEntity.Contains("customerid")
                               ? postImageEntity.GetAttributeValue<EntityReference>("customerid").Id
                               : Guid.Empty;

                        if (preCustomerId != postCustomerId)
                        {
                            quotehandler.PopulateCustomerInformation(postImageEntity, message);
                        }

                        #endregion

                        //Added by: Raphael Herrera, Added On: 4/27/2016
                        #region Call functions that will run if Payment Mode was changed
                        var prePaymentMode = preImageEntity.Contains("gsc_paymentmode")
                               ? preImageEntity.GetAttributeValue<OptionSetValue>("gsc_paymentmode").Value
                               : 0;
                        var postPaymentMode = postImageEntity.Contains("gsc_paymentmode")
                               ? postImageEntity.GetAttributeValue<OptionSetValue>("gsc_paymentmode").Value
                               : 0;

                       /* if (prePaymentMode != postPaymentMode)
                        {
                            quotehandler.SetCabChassisFinancing(postImageEntity);

                            //Not Financed
                            if (postPaymentMode != 100000001)
                                quotehandler.ClearFinancingFields(postImageEntity);
                        }*/
                        #endregion

                        //Recompute Unit Price when Markup % Changed - Leslie Baliguat 05/08/17
                        var preMarkup = preImageEntity.Contains("gsc_markup")
                               ? preImageEntity.GetAttributeValue<Decimal>("gsc_markup")
                               : 0;
                        var postMarkup = postImageEntity.Contains("gsc_markup")
                               ? postImageEntity.GetAttributeValue<Decimal>("gsc_markup")
                               : 0;

                        if (preMarkup != postMarkup)
                        {
                            quotehandler.UpdateGovernmentTax(postImageEntity, message);
                        }

                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot update opportunity status to lost since there's at least one quote record which is not closed."))
                    {
                        throw new InvalidPluginExecutionException("Cannot update opportunity status to lost since there's at least one quote record which is not closed.");
                    }
                    else
                    {
                        //throw new InvalidPluginExecutionException(String.Concat("(Exception)\n", ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, error));
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                    
                }
            }
        }
    }
}
