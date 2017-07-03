using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Install.Items;
using Sitecore.Shell.Applications.Install;
using Sitecore.Shell.Applications.Install.Controls;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Support.Shell.Applications.Install.Dialogs
{
    public class AddItemSourceForm : WizardForm
    {
        private static readonly string NoItemSelectionMessage = "Select an item first";

        /// <summary></summary>
        protected Border Internals;

        /// <summary></summary>
        protected ItemsFilterEditor Filters;

        /// <summary></summary>
        protected ItemSourceRootEditor Root;

        /// <summary></summary>
        protected NameEditor Name;

        /// <summary></summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                ApplicationContext.AttachDocument(new ItemSource());
                this.Bind();
            }
        }

        /// <summary></summary>
        protected override bool ActivePageChanging(string page, ref string newpage)
        {
            bool result = base.ActivePageChanging(page, ref newpage);
            if (string.Compare(page, "LoadItemSource", true, CultureInfo.InvariantCulture) == 0 && string.Compare(newpage, "ApplyFilters", true, CultureInfo.InvariantCulture) == 0 && this.Root.Selection == null)
            {
                Context.ClientPage.ClientResponse.Alert(Translate.Text(AddItemSourceForm.NoItemSelectionMessage));
                result = false;
            }
            return result;
        }

        /// <summary></summary>
        protected override void ActivePageChanged(string page, string oldPage)
        {
            base.ActivePageChanged(page, oldPage);
            this.NextButton.Header = ((page == "SetName") ? "Add" : "Next");
            if (page == "LastPage")
            {
                this.BackButton.Disabled = true;
            }
        }
        
        /// <summary></summary>
        protected override void EndWizard()
        {
            
            Item selection = this.Root.Selection;
            if (selection != null && this.CancelButton.Header=="Close")
            {
                ItemSource itemSource = (ItemSource)ApplicationContext.DocumentHolder.Document;
                itemSource.Database = selection.Database.Name;
                itemSource.Root = selection.ID.ToString();
                string dialogValue = ApplicationContext.StoreObject(itemSource);
                Context.ClientPage.ClientResponse.SetDialogValue(dialogValue);
            }
            base.EndWizard();
        }

        [HandleMessage("document:bind")]
        private void OnBind(Message message)
        {
            this.Bind();
        }

        private void Bind()
        {
            object document = ApplicationContext.DocumentHolder.Document;
            this.Root.BindTo(document);
            this.Filters.BindTo(document);
            this.Name.BindTo(document);
        }
    }
}