﻿using Messenger_Client.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;


namespace Messenger_Client.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, INotifyPropertyChanged
    {
        public ICommand SendMessageCommand { get; set; }
        public ICommand CheckEnterCommand { get; set; }

        public List<TestMessage> MessageList { get; set; }

        //public Client Client { get; set; }
        public List<Group> GroupList { get; set; }
        //public string Base64ImageData { get; set; }

        private string base64ImageData;

        //TypedTextValue wordt gebruikt om een StackOverflowException uit te sluiten
        private string TypedTextValue;
        public string TypedText
        {
            get
            {
                return TypedTextValue;
            }
            set
            {
                TypedTextValue = value;
                OnPropertyChanged();
            }
        }

        public string Base64ImageData
        {
            get
            {
                return base64ImageData;
            }
            set
            {
                base64ImageData = value;
                OnPropertyChanged();
            }

        }

        private string img;

        public string Img
        {
            get
            {
                return img;
            }
            set
            {
                img = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage bmi;

        public BitmapImage Bmi
        {
            get { return bmi; }
            set
            {
                bmi = value;
                OnPropertyChanged();
            }
        }

        private Group selectedGroupChat;

        public Group SelectedGroupChat
        {
            get
            {
                return selectedGroupChat;
            }
            set
            {
                selectedGroupChat = value;
                Debug.WriteLine("Selected group chat: " + value.Name);
                OnPropertyChanged();
            }
        }

        public void SendMessage()
        {
            Client.Instance.Connection.SendMessage(new Message()
            {
                MessageType = MessageType.ChatMessage,
                ClientId = Client.Instance.Id,
                GroupID = SelectedGroupChat.Id,
                PayloadData = this.TypedText
            });
            this.TypedText = "";
        } 
        
        public void CheckEnterPressed(object args)
        {
            KeyRoutedEventArgs keyargs = (KeyRoutedEventArgs) args;
            Debug.WriteLine(keyargs.Key);
            if (keyargs.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
            }
        }       

        public MainPageViewModel()
        {
            SendMessageCommand = new RelayCommand(() => SendMessage());
            CheckEnterCommand = new RelayCommand<object>(CheckEnterPressed);

            this.MessageList = new List<TestMessage>();
            this.GroupList = new List<Group>();
            this.Base64ImageData = "nodata";
            this.TypedText = "";

            Client client = Client.Instance;

            Random rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                Group group = new Group(string.Format("GroupName {0}", i), i);
                client.AddGroup(group);
                GroupList.Add(group);
            }


            for (int i = 0; i < 1000; i++)
            {
                int clientId = rnd.Next(0, 2);
                int groupId = rnd.Next(0, 20);
                Message message = new Message()
                {
                    ClientName = "ClientName1",
                    ClientId = clientId,
                    DateTime = DateTime.Now,
                    GroupID = groupId,
                    MessageType = MessageType.ChatMessage,
                    ImageString = "R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==",
                    PayloadData = $"this is a message with client {clientId} and group {groupId}",
                };

                GroupList[groupId].AddMessage(message);
            }
        }
    }
}
