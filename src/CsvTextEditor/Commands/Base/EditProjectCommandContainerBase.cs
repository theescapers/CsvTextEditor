﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditProjectCommandContainerBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace CsvTextEditor
{
    using System;
    using System.Windows.Threading;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Orc.CsvTextEditor.Services;
    using Orc.ProjectManagement;

    public abstract class EditProjectCommandContainerBase : ProjectCommandContainerBase
    {
        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private readonly DispatcherTimer _invalidateTimer;

        private ICsvTextEditorService _csvTextEditorService;
        #endregion

        #region Constructors
        protected EditProjectCommandContainerBase(string commandName, ICommandManager commandManager, IProjectManager projectManager, IServiceLocator serviceLocator)
            : base(commandName, commandManager, projectManager)
        {
            Argument.IsNotNull(() => serviceLocator);

            _serviceLocator = serviceLocator;

            _invalidateTimer = new DispatcherTimer();
            _invalidateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _invalidateTimer.Tick += OnInvalidateTimerTick;
        }        
        #endregion

        protected ICsvTextEditorService CsvTextEditorService
        {
            get
            {
                var activeProject = _projectManager.ActiveProject;

                if (_csvTextEditorService == null && _serviceLocator.IsTypeRegistered<ICsvTextEditorService>(activeProject))
                {
                    _csvTextEditorService = _serviceLocator.ResolveType<ICsvTextEditorService>(activeProject);
                }

                return _csvTextEditorService;
            }
        }

        #region Methods
        protected override bool CanExecute(object parameter)
        {
            if (!base.CanExecute(parameter))
            {
                return false;
            }

            var isEditorServiceNull = ReferenceEquals(CsvTextEditorService, null);
            if (isEditorServiceNull && !_invalidateTimer.IsEnabled)
            {
                _invalidateTimer.Start();
            }

            return !isEditorServiceNull;
        }

        private void OnInvalidateTimerTick(object sender, EventArgs e)
        {
            _invalidateTimer.Stop();
            
            _commandManager.InvalidateCommands();
        }
        #endregion
    }
}