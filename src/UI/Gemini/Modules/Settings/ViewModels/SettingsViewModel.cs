using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;

namespace Gemini.Modules.Settings.ViewModels
{
    [Export(typeof (SettingsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsViewModel : WindowBase
    {
        private IEnumerable<ISettingsEditor> _settingsEditors;
        private SettingsPageViewModel _selectedPage;
        private bool _closeHandled = false;

        public SettingsViewModel()
        {
            CancelCommand = new RelayCommand(o=> DiscardChanges(o, true));
            OkCommand = new RelayCommand(o => SaveChanges(o, true));

            DisplayName = "Options";
        }

        public List<SettingsPageViewModel> Pages { get; private set; }

        public SettingsPageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                NotifyOfPropertyChange(() => SelectedPage);
                NotifyOfPropertyChange(() => SelectedPageEditors);
            }
        }

        public List<ISettingsEditor> SelectedPageEditors
        {
            get
            {
                if(_selectedPage == null)
                    return null;
                return _selectedPage.Editors.Any() ? _selectedPage.Editors 
                    : GetFirstLeafPageRecursive(_selectedPage.Children).Editors;
            }
        }

        public ICommand CancelCommand { get; private set; }
        public ICommand OkCommand { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            // Keep '_' on top
            var pages = new List<SettingsPageViewModel>();
            _settingsEditors = IoC.GetAll<ISettingsEditor>().OrderBy(p => p.SettingsPagePath).ThenBy(p => p.SettingsPageName);
            
            foreach (ISettingsEditor settingsEditor in _settingsEditors)
            {
                var parentCollection = GetParentCollection(settingsEditor, pages);

                var page =
                    parentCollection.FirstOrDefault(m => m.Name.TrimStart('_') == settingsEditor.SettingsPageName.TrimStart('_'));

                if (page == null)
                {
                    page = new SettingsPageViewModel
                    {
                        Name = settingsEditor.SettingsPageName.TrimStart('_'),
                    };
                    parentCollection.Add(page);
                }

                page.Editors.Add(settingsEditor);
            }

            Pages = pages;
            SelectedPage = GetFirstLeafPageRecursive(pages);
        }

        private static SettingsPageViewModel GetFirstLeafPageRecursive(List<SettingsPageViewModel> pages)
        {
            if (pages == null || !pages.Any())
                return null;

            var firstPage = pages.First();
            if (!firstPage.Children.Any())
                return firstPage;

            return GetFirstLeafPageRecursive(firstPage.Children);
        }

        private List<SettingsPageViewModel> GetParentCollection(ISettingsEditor settingsEditor,
            List<SettingsPageViewModel> pages)
        {
            if (string.IsNullOrEmpty(settingsEditor.SettingsPagePath))
            {
                return pages;
            }

            string[] path = settingsEditor.SettingsPagePath.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pathElement in path)
            {
                var page = pages.FirstOrDefault(s => s.Name.TrimStart('_') == pathElement.TrimStart('_'));

                if (page == null)
                {
                    page = new SettingsPageViewModel {Name = pathElement.TrimStart('_')};
                    pages.Add(page);
                }

                pages = page.Children;
            }

            return pages;
        }
        
        protected override void OnDeactivate(bool close)
        {
            DiscardChanges(this, false);
            base.OnDeactivate(close);
        }

        private void DiscardChanges(object obj, bool close)
        {
            if (_closeHandled)
                return;
            _closeHandled = true;

            foreach (ISettingsEditor settingsEditor in _settingsEditors)
            {
                settingsEditor.DiscardChanges();
            }

            if(close)
                TryClose(false);
        }
        
        private void SaveChanges(object obj, bool close)
        {
            if (_closeHandled)
                return;
            _closeHandled = true;

            foreach (ISettingsEditor settingsEditor in _settingsEditors)
            {
                settingsEditor.ApplyChanges();
            }

            if (close)
                TryClose(true);
        }
    }
}