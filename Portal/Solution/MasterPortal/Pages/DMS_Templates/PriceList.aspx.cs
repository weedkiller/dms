﻿using Adxstudio.Xrm.Web.UI.EntityForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Site.Pages.DMS_Templates
{
    public partial class PriceList : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectToLoginIfAnonymous();
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