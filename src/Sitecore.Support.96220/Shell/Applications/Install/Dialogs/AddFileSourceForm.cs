using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Install.Files;
using Sitecore.Shell.Applications.Install.Controls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using System;
using System.Globalization;
using Sitecore.Shell.Applications.Install;

namespace Sitecore.Support.Shell.Applications.Install.Dialogs
{
    /// <summary></summary>
    public class AddFileSourceForm : WizardForm
    {
        private static readonly string NoFolderSelectionMessage = "Select a folder first";

        /// <summary></summary>
        protected FilesFilterEditor Filters;

        /// <summary></summary>
        protected FileSourceRootEditor Root;

        /// <summary></summary>
        protected NameEditor Name;

        /// <summary></summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                ApplicationContext.AttachDocument(new FileSource());
                this.Bind();
            }
        }

        /// <summary></summary>
        protected override bool ActivePageChanging(string page, ref string newpage)
        {
            bool result = base.ActivePageChanging(page, ref newpage);
            if (string.Compare(page, "LoadFileSource", true, CultureInfo.InvariantCulture) == 0 && string.Compare(newpage, "ApplyFilters", true, CultureInfo.InvariantCulture) == 0 && this.Root.Selection == null)
            {
                Context.ClientPage.ClientResponse.Alert(Translate.Text(AddFileSourceForm.NoFolderSelectionMessage));
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
            if (selection != null && this.CancelButton.Header == "Close")
            {
                FileSource fileSource = ApplicationContext.DocumentHolder.Document as FileSource;
                FileToEntryConverter converter = new FileToEntryConverter();
                fileSource.Converter = converter;
                fileSource.Root = selection["path"];
                string dialogValue = ApplicationContext.StoreObject(fileSource);
                Context.ClientPage.ClientResponse.SetDialogValue(dialogValue);
            }
            base.EndWizard();
        }

        /// <summary>
        /// Called when this instance has bind.
        /// </summary>
        /// <param name="message">The message.</param>
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
