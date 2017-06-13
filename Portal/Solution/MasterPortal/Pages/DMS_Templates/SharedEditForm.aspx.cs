﻿using Adxstudio.Xrm.Web.UI.EntityForm;
using Site.Areas.DMS_Api;
using Site.Areas.DMSApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Site.Pages.DMS_Templates
{
    public partial class SharedEditForm : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectToLoginIfAnonymous();
        }
        protected void OnItemSaving(object sender, EntityFormSavingEventArgs e)
        {
            XrmConnection _conn = new XrmConnection();
            var service = new XrmServiceContext(_conn);

            Guid entityId = EntityForm1.EntitySourceDefinition.ID;
            string logicalName = EntityForm1.EntitySourceDefinition.LogicalName;

            e.Cancel = service.HasDuplicate(logicalName, e.Values, entityId);

            if (e.Cancel)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "DupeDetectedNotification();", true);
            }
        }
        protected void OnItemSaved(object sender, EntityFormSavedEventArgs e)
        {

            SalesHub hub = new SalesHub();
            string url = Request.Url.OriginalString;
            string userId = Portal.User.Id.ToString();
            string fullName = Portal.User.Attributes["fullname"].ToString();
            hub.UserHasSaved(url, userId, fullName);
        }
    }
}