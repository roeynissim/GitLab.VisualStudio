﻿using CodeCloud.TeamFoundation.ViewModels;
using CodeCloud.TeamFoundation.Views;
using CodeCloud.VisualStudio.Shared;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.Git.Controls.Extensibility;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace CodeCloud.TeamFoundation.Connect
{
    [TeamExplorerSection(ConnectSectionId, TeamExplorerPageIds.Connect, 10)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CodeCloudConnectSection : TeamExplorerSectionBase
    {
        public const string ConnectSectionId = "519B47D3-F2A9-4E19-8491-8C9FA25ABE90";

        private readonly IMessenger _messenger;
        private readonly IRegistry _registry;
        private readonly IShellService _shell;
        private readonly IStorage _storage;
        private readonly ITeamExplorerServices _teamexplorer;
        private readonly IViewFactory _viewFactory;
        private readonly IVisualStudioService _vs;
        private readonly IWebService _web;

        [ImportingConstructor]
        public CodeCloudConnectSection(IMessenger messenger, IRegistry registry, IShellService shell, IStorage storage, ITeamExplorerServices teamexplorer, IViewFactory viewFactory, IVisualStudioService vs, IWebService web)
        {
            _messenger = messenger;
            _registry = registry;
            _shell = shell;
            _storage = storage;
            _teamexplorer = teamexplorer;
            _viewFactory = viewFactory;
            _vs = vs;
            _web = web;

            messenger.Register("OnLogined", OnLogined);
            messenger.Register("OnSignOuted", OnSignOuted);
            messenger.Register<string, Repository>("OnClone", OnClone);
            messenger.Register<string>("OnOpenSolution", OnOpenSolution);
        }

        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var temp = new TeamExplorerSectionViewModelBase
            {
                Title = "Code Cloud"
            };

            return temp;
        }
        public override void Refresh()
        {
            base.Refresh();

            IsExpanded = true;
            IsVisible = _storage.IsLogined;
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            _vs.ServiceProvider = ServiceProvider;
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new ConnectSectionView();
        }
        protected override void InitializeView(SectionInitializeEventArgs e)
        {
            var view = this.SectionContent as FrameworkElement;

            if (view != null)
            {
                view.DataContext = new ConnectSectionViewModel(_messenger, _registry, _shell, _storage, _teamexplorer, _viewFactory, _vs, _web);
            }
        }

        public void OnLogined()
        {
            IsVisible = true;
        }

        public void OnSignOuted()
        {
            IsVisible = false;
        }

        public void OnClone(string url, Repository repository)
        {
            var gitExt = ServiceProvider.GetService<IGitRepositoriesExt>();

            gitExt.Clone(url, repository.Path, CloneOptions.RecurseSubmodule);
        }

        public void OnOpenSolution(string path)
        {
            var x = ServiceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            if (x != null)
            {
                x.OpenSolutionViaDlg(path, 1);
            }
        }

        public override void Dispose()
        {
            _messenger.UnRegister(this);

            var disposable = ViewModel as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}