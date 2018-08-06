﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace CsvTextEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.Squirrel;
    using Services;

    public class SettingsViewModel : ViewModelBase
    {
        #region Fields
        private readonly IConfigurationService _configurationService;
        private readonly IManageUserDataService _manageUserDataService;
        private readonly IUpdateService _updateService;
        private readonly IOpenFileService _openFileService;
        #endregion

        #region Constructors
        public SettingsViewModel(IConfigurationService configurationService, IManageUserDataService manageUserDataService, IUpdateService updateService, IOpenFileService openFileService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => manageUserDataService);
            Argument.IsNotNull(() => updateService);
            Argument.IsNotNull(() => openFileService);

            _configurationService = configurationService;
            _manageUserDataService = manageUserDataService;
            _updateService = updateService;
            _openFileService = openFileService;

            PickEditor = new TaskCommand(PickEditorExecuteAsync);
            OpenApplicationDataDirectory = new Command(OnOpenApplicationDataDirectoryExecute);
            BackupUserData = new TaskCommand(OnBackupUserDataExecuteAsync);

            Title = "Settings";
        }
        #endregion

        #region Properties
        public bool IsUpdateSystemAvailable { get; private set; }
        public bool CheckForUpdates { get; set; }
        public bool AutoSaveEditor { get; set; }
        public string CustomEditor { get; private set; }
        public List<UpdateChannel> AvailableUpdateChannels { get; private set; }
        public UpdateChannel UpdateChannel { get; set; }
        #endregion

        #region Commands
        public Command OpenApplicationDataDirectory { get; private set; }

        private void OnOpenApplicationDataDirectoryExecute()
        {
            _manageUserDataService.OpenApplicationDataDirectory();
        }

        public TaskCommand PickEditor { get; private set; }

        private async Task PickEditorExecuteAsync()
        {
            _openFileService.Filter = "Program Files (*.exe)|*exe";
            _openFileService.IsMultiSelect = false;

            if (await _openFileService.DetermineFileAsync())
            {
                CustomEditor = _openFileService.FileName;
            }       
        }

        public TaskCommand BackupUserData { get; private set; }

        private async Task OnBackupUserDataExecuteAsync()
        {
            await _manageUserDataService.BackupUserDataAsync();
        }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            IsUpdateSystemAvailable = _updateService.IsUpdateSystemAvailable;
            CheckForUpdates = _updateService.CheckForUpdates;
            AvailableUpdateChannels = new List<UpdateChannel>(_updateService.AvailableChannels);
            UpdateChannel = _updateService.CurrentChannel;

            var customEditor = _configurationService.GetRoamingValue<string>(Configuration.CustomEditor, Environment.SystemDirectory + "\\notepad.exe");
            CustomEditor = customEditor;
            AutoSaveEditor = _configurationService.GetRoamingValue<bool>(Configuration.AutoSaveEditor);
        }

        protected override async Task<bool> SaveAsync()
        {
            _updateService.CheckForUpdates = CheckForUpdates;
            _updateService.CurrentChannel = UpdateChannel;

            _configurationService.SetRoamingValue(Configuration.CustomEditor, CustomEditor);
            _configurationService.SetRoamingValue(Configuration.AutoSaveEditor, AutoSaveEditor);

            return await base.SaveAsync();
        }
        #endregion
    }
}
