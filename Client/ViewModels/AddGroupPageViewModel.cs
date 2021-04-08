﻿using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace Messenger_Client.ViewModels
{
    class AddGroupPageViewModel : ObservableRecipient
    {
        public ICommand BackToMainPageCommand { get; set; }
        public ICommand AddGroupCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand AboutDialogCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand ShowGroupsToJoinCommand { get; set; }
        public ICommand ExportMessageCommand { get; set; }

        public string NewGroupName { get; set; }

        private string createGroupErrorMessage = "";
        public string CreateGroupErrorMessage
        {
            get
            {
                return createGroupErrorMessage;
            }
            set
            {
                createGroupErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public AddGroupPageViewModel()
        {
            // Menubar buttons
            LogoutCommand = new RelayCommand(Logout);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ShowGroupsToJoinCommand = new RelayCommand(ShowGroupsToJoin);
            AboutDialogCommand = new RelayCommand(DisplayAboutDialog);
            ExportMessageCommand = new RelayCommand(ExportMessage);

            // Page buttons
            AddGroupCommand = new RelayCommand(AddNewGroup);
            CheckEnterCommand = new RelayCommand<KeyRoutedEventArgs>(CheckEnterPressed);
            BackToMainPageCommand = new RelayCommand(NavigateToMain);
        }

        private void CheckEnterPressed(KeyRoutedEventArgs keyargs)
        {
            if (keyargs.Key == VirtualKey.Enter)
            {
                AddNewGroup();
            }
        }

        private void ShowGroupsToJoin()
        {
            Helper.NavigateTo(typeof(JoinGroupPage));
        }

        private async void ExportMessage()
        {
            await Client.Instance.ExportMessageToFileAsync();
        }

        private void NavigateToMain()
        {
            Helper.NavigateTo(typeof(MainPage));
        }
        private void ShowSettings()
        {
            Helper.NavigateTo(typeof(SettingsPage));
        }

        private void AddNewGroup()
        {
            if (this.NewGroupName != null && !this.NewGroupName.Equals(""))
            {
                CommunicationHandler.RegisterGroupResponse += OnRegisterGroupResponseReceived;
                CommunicationHandler.SendRegisterGroupMessage(this.NewGroupName);
            }
            else
            {
                CreateGroupErrorMessage = "A group needs a name";
            }
        }

        private async void OnRegisterGroupResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.RegisterGroupResponse -= OnRegisterGroupResponseReceived;

            switch (e.State)
            {
                case -1:
                    Helper.RunOnUI(() => CreateGroupErrorMessage = "Group not created");
                    break;
                default:
                    Helper.NavigateTo(typeof(MainPage));
                    break;
            }
        }

        private void Logout()
        {
            Client.Instance.Connection.Close();

            Helper.NavigateTo(typeof(LoginPage));

            Debug.WriteLine("Logout");
        }

        private async void DisplayAboutDialog()
        {
            await Helper.AboutDialog().ShowAsync();
        }
    }
}
