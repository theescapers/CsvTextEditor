﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCommandContainerBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace CsvTextEditor.Base
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Catel.Threading;
    using Orc.ProjectManagement;

    public abstract class ProjectCommandContainerBase : CommandContainerBase
    {
        #region Fields
        private readonly ICommandManager _commandManager;

        protected readonly IProjectManager _projectManager;
        #endregion

        #region Constructors
        protected ProjectCommandContainerBase(string commandName, ICommandManager commandManager, IProjectManager projectManager)
            : base(commandName, commandManager)
        {
            Argument.IsNotNull(() => projectManager);

            _commandManager = commandManager;
            _projectManager = projectManager;

            _projectManager.ProjectActivatedAsync += OnProjectActivated;
        }
        #endregion

        #region Methods
        private Task OnProjectActivated(object sender, ProjectUpdatedEventArgs e)
        {
            _commandManager.InvalidateCommands();

            return TaskHelper.Completed;
        }

        protected override bool CanExecute(object parameter)
        {
            if (_projectManager.ActiveProject == null)
            {
                return false;
            }

            return base.CanExecute(parameter);
        }
        #endregion
    }
}