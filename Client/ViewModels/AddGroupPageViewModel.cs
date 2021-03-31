﻿using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Messenger_Client.ViewModels
{
    class AddGroupPageViewModel
    {
        public ICommand BackToMainPageCommand { get; set; }
        public ICommand AddGroupCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }

        public string NewGroupName { get; set; }


        public AddGroupPageViewModel()
        {
            BackToMainPageCommand = new RelayCommand<object>(BackToMain);
            AddGroupCommand = new RelayCommand(AddNewGroup);
            CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);
        }

        private void CheckEnterPressed(object obj)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs)obj;
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                AddNewGroup();
                navigateToMain();
            }
        }

        private void navigateToMain()
        {
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }

        private void AddNewGroup()
        {
            if (!this.NewGroupName.Equals(""))
            {
                CommunicationHandler.SendRegisterGroupMessage(this.NewGroupName);
                navigateToMain();
            }
        }

        private void BackToMain(object obj)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }
    }
}
