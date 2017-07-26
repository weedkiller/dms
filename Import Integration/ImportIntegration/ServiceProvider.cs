﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportIntegration
{
    public class ServiceProvider
    {
        private readonly IOrganizationService _service;
        private readonly Logger _logger;

        public int RecordsUploaded { get; set; }
        public int RecordsFailedUpload { get; set; }

        public ServiceProvider(IOrganizationService service, Logger logger)
        {
            _service = service;
            _logger = logger;
        }

        public async void MassUploadReceiving(IEnumerable<ReceivingTransaction> receivingTransactions)
        {
            int counter = 1;
            foreach (ReceivingTransaction item in receivingTransactions)
            {
                
                Entity rt = new Entity("gsc_cmn_receivingtransaction");

                List<ConditionExpression> vpoConditionExp = new List<ConditionExpression>();
                vpoConditionExp.Add(new ConditionExpression("gsc_purchaseorderpn", ConditionOperator.Equal, item.VehiclePurchaseOrderNumber));
                // check if vpo status is equal to Ordered
                vpoConditionExp.Add(new ConditionExpression("gsc_vpostatus", ConditionOperator.Equal, 100000002));

                Entity vpo = GetEntityRecord("gsc_cmn_purchaseorder", vpoConditionExp,
                         new string[] { "gsc_branchid", "gsc_dealerid", "gsc_recordownerid" });

                Guid vpoId = vpo.Id;

                Guid siteId = GetEntityReferenceId("gsc_iv_site",
                  GetEntityConditionExpression("gsc_sitepn", item.InTransitSite));

                Guid vehicleColorId = GetEntityReferenceId("gsc_iv_color",
                  GetEntityConditionExpression("gsc_colorcode", item.ReceivingDetails.ColorCode));

                List<ConditionExpression> productConditionExp = new List<ConditionExpression>();
                productConditionExp.Add(new ConditionExpression("gsc_optioncode", ConditionOperator.Equal, item.ReceivingDetails.OptionCode));
                productConditionExp.Add(new ConditionExpression("gsc_modelcode", ConditionOperator.Equal, item.ReceivingDetails.ModelCode));
                productConditionExp.Add(new ConditionExpression("gsc_modelyear", ConditionOperator.Equal, item.ReceivingDetails.ModelYear));

                Guid productId = GetEntityReferenceId("product", productConditionExp);

                List<ConditionExpression> productColorConditionExp = new List<ConditionExpression>();
                productColorConditionExp.Add(new ConditionExpression("gsc_colorcode", ConditionOperator.Equal, item.ReceivingDetails.ColorCode));
                productColorConditionExp.Add(new ConditionExpression("gsc_productid", ConditionOperator.Equal, productId));

                Guid vehicleProductColorId = GetEntityReferenceId("gsc_cmn_vehiclecolor", productColorConditionExp);

                if (vpoId == Guid.Empty)
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Vehicle Purchase Purchase Order Number \"{1}\" does not exist in the DMS.", counter, item.VehiclePurchaseOrderNumber);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (siteId == Guid.Empty)
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} In-Transit Site \"{1}\" does not exist in the DMS.", counter, item.VehiclePurchaseOrderNumber);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.InvoiceNumber))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Invoice Number \"{1}\" cannot be empty or null.", counter, item.VehiclePurchaseOrderNumber);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.MMPCStatus))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} MMPC Status \"{1}\" cannot be empty or null.", counter, item.VehiclePurchaseOrderNumber);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.InTransitReceiptDate))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} In-Transit Receipt Date cannot be empty or null and has to be in valid format.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfValidStringDateTime(item.InvoiceDate))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Invoice Date cannot be empty or null and has to be in valid format.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.ModelCode))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Model Code cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.OptionCode))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Option Code cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.ModelYear))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Model Year cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (productId == Guid.Empty)
                {
                    _logger.Log(LogLevel.Error, @"Unable to save row {0} 
                                                  Product Item with Model Code : {1}, Option Code : {2}, Model Year {3}
                                                  does not exist in DMS.",
                                                  counter, item.ReceivingDetails.ModelCode, item.ReceivingDetails.OptionCode, item.ReceivingDetails.ModelYear);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.ColorCode))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Color Code cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (vehicleColorId == Guid.Empty)
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Color Code \"{1}\" does not exist in the DMS..", counter, item.ReceivingDetails.ColorCode);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (vehicleProductColorId == Guid.Empty)
                {
                    _logger.Log(LogLevel.Error, @"Unable to save row {0}
                                                  Vehicle Id {1} with Color {2}
                                                  does not exist in the DMS..", counter, productId, item.ReceivingDetails.ColorCode);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.EngineNumber))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Engine Number cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.CSNumber))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} CS Number cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.ProductionNumber))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} Production Number cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                if (!CheckIfNullOrValidString(item.ReceivingDetails.VIN))
                {
                    _logger.Log(LogLevel.Error, "Unable to save row {0} VIN cannot be empty or null.", counter);
                    counter++;
                    this.RecordsFailedUpload++;
                    continue;
                }

                rt.Attributes.Add("gsc_recordtype", new OptionSetValue(100000000));
                rt.Attributes.Add("gsc_purchaseorderid", new EntityReference("gsc_cmn_purchaseorder", vpoId));              
                rt.Attributes.Add("gsc_intransitsiteid", new EntityReference("gsc_iv_site", siteId));
                DateTime pullOutDate;
                DateTime.TryParse(item.PullOutDate, out pullOutDate);
                DateTime intransitReceiptDate;
                DateTime.TryParse(item.InTransitReceiptDate, out intransitReceiptDate);
                rt.Attributes.Add("gsc_intransitreceiptdate", intransitReceiptDate);
                rt.Attributes.Add("gsc_pulloutdate", pullOutDate);
                rt.Attributes.Add("gsc_invoiceno", item.InvoiceNumber);
                DateTime invoiceDate;
                DateTime.TryParse(item.InvoiceDate, out invoiceDate);
                rt.Attributes.Add("gsc_invoicedate", invoiceDate);
                rt.Attributes.Add("gsc_mmpcstatus", item.MMPCStatus);
                rt.Attributes.Add("gsc_branchid", vpo.GetAttributeValue<EntityReference>("gsc_branchid"));
                rt.Attributes.Add("gsc_dealerid", vpo.GetAttributeValue<EntityReference>("gsc_dealerid"));
                rt.Attributes.Add("gsc_recordownerid", vpo.GetAttributeValue<EntityReference>("gsc_recordownerid"));

                Guid rtId = _service.Create(rt);

                _logger.Log(LogLevel.Info, @"Row {0} with Vehicle Purchase Number {1}
                                             was successfully created in Receiving Transaction
                                             with record id {2}", counter, item.VehiclePurchaseOrderNumber, rtId);


                Entity rtDetails = new Entity("gsc_cmn_receivingtransactiondetail");

              //  await Task.Delay(2500);

                rtDetails = GetEntityRecord("gsc_cmn_receivingtransactiondetail",
                    GetEntityConditionExpression("gsc_receivingtransactionid", rtId),
                    new string[] { "gsc_receivingtransactionid" });

                rtDetails.Attributes.Add("gsc_modelcode", item.ReceivingDetails.ModelCode);
                rtDetails.Attributes.Add("gsc_optioncode", item.ReceivingDetails.OptionCode);
                rtDetails.Attributes.Add("gsc_modelyear", item.ReceivingDetails.ModelYear);
                rtDetails.Attributes.Add("gsc_colorcode", item.ReceivingDetails.ColorCode);
                rtDetails.Attributes.Add("gsc_engineno", item.ReceivingDetails.EngineNumber);
                rtDetails.Attributes.Add("gsc_csno", item.ReceivingDetails.CSNumber);
                rtDetails.Attributes.Add("gsc_productionno", item.ReceivingDetails.ProductionNumber);
                rtDetails.Attributes.Add("gsc_vin", item.ReceivingDetails.VIN);                

                _service.Update(rtDetails);
               
                _logger.Log(LogLevel.Info, @"Row {0} with Vehicle Purchase Number {1}
                                             successfully created a record in Receiving Transaction Detail
                                             with Id {2}", counter, item.VehiclePurchaseOrderNumber, rtDetails.Id);
                counter++;
                this.RecordsUploaded++;
            }
        }

        private ConditionExpression GetEntityConditionExpression(string attribute, object value)
        {
            return new ConditionExpression(attribute, ConditionOperator.Equal, value);
        }

        private bool CheckIfValidStringDateTime(string date)
        {
            DateTime dateValue;
            return DateTime.TryParse(date, out dateValue);
        }


        private bool CheckIfNullOrValidString(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value)) return false;
            return true;
        }

        private Guid GetEntityReferenceId(string entityName, ConditionExpression condition)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumn(condition.AttributeName);
            query.ColumnSet.AddColumn(entityName + "id");
            query.Criteria.AddCondition(condition);


            EntityCollection collection = _service.RetrieveMultiple(query);

            if (collection.Entities.Count > 0)
            {
                return collection[0].Id;
            }

            return Guid.Empty;
        }

        private Guid GetEntityReferenceId(string entityName, List<ConditionExpression> conditions)
        {
            QueryExpression query = new QueryExpression(entityName);

            foreach (ConditionExpression item in conditions)
            {
                query.ColumnSet.AddColumn(item.AttributeName);
            }

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.AddRange(conditions);

            query.Criteria.Filters.Add(filter);
            query.ColumnSet.AddColumn(entityName + "id");

            EntityCollection collection = _service.RetrieveMultiple(query);

            if (collection.Entities.Count > 0)
            {
                return collection[0].Id;
            }

            return Guid.Empty;
        }

        private Entity GetEntityRecord(string entityName, ConditionExpression condition, string[] columns)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(columns);
            query.Criteria.AddCondition(condition);


            EntityCollection collection = _service.RetrieveMultiple(query);

            if (collection.Entities.Count > 0)
            {
                return collection.Entities[0];
            }

            return new Entity(entityName);
        }

        private Entity GetEntityRecord(string entityName, List<ConditionExpression> conditions, string[] columns)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AddColumns(columns);
            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.AddRange(conditions);

            query.Criteria.Filters.Add(filter);

            EntityCollection collection = _service.RetrieveMultiple(query);

            if (collection.Entities.Count > 0)
            {
                return collection.Entities[0];
            }

            return new Entity();
        }


    }
}
