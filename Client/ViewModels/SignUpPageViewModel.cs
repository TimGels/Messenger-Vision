﻿using Messenger_Client.Models;
using Messenger_Client.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Messenger_Client.ViewModels
{
    class SignUpPageViewModel : ObservableRecipient
    {
        public ICommand GoToLoginButtonCommand { get; set; }
        public ICommand RegisterButtonCommand { get; set; }
        public string RepeatPassword { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }


        private string signUpErrorMessage = "";

        public string SignUpErrorMessage
        {
            get
            {
                return signUpErrorMessage;
            }
            set
            {
                signUpErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public SignUpPageViewModel()
        {
            RegisterButtonCommand = new RelayCommand(RegisterButtonClicked);
            GoToLoginButtonCommand = new RelayCommand(GoToLoginButtonClicked);
        }

        private void GoToLoginButtonClicked()
        {
            Helper.NavigateTo(typeof(LoginPage));
        }

        private async void RegisterButtonClicked()
        {
            if (Name == null || Name.Equals(""))
            {
                SignUpErrorMessage = "You have to fill in a name";
                return;
            }

            if (Mail == null || Mail.Equals(""))
            {
                SignUpErrorMessage = "You have to fill in an E-mail";
                return;
            }

            if (Password == null || Password.Equals(""))
            {
                SignUpErrorMessage = "You have to fill in a password";
                return;
            }

            if(RepeatPassword == null)
            {
                SignUpErrorMessage = "You have to repeat your password";
                return;
            }

            if (!Password.Equals(RepeatPassword))
            {
                SignUpErrorMessage = "Your password has to be the same";
                return;
            }

            if (await Client.Instance.Connection.OpenAsync() == false)
            {
                SignUpErrorMessage = "Could not connect to the server!";
                return;
            }

            CommunicationHandler.SignUpResponse += OnSignUpResponseReceived;
            CommunicationHandler.SendRegisterMessage(Mail, Name, Password);
        }

        private async void OnSignUpResponseReceived(object sender, CommunicationHandler.ResponseStateEventArgs e)
        {
            CommunicationHandler.SignUpResponse -= OnSignUpResponseReceived;

            switch (e.State)
            {
                case -1:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        SignUpErrorMessage = "E-Mail al in gebruik";
                    });
                    break;
                default:
                    Helper.NavigateTo(typeof(LoginPage));
                    break;
            }
        }
    }
}
