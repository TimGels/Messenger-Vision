﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.Threading;
using Shared;

namespace Messenger_Client.Models
{
    class CommunicationHandler
    {

        public static void HandleMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.RegisterClientResponse:
                    HandleRegisterClientResponse(message);
                    break;
                case MessageType.SignInClientResponse:
                    HandleSignInClientResponse(message);
                    break;
                case MessageType.RegisterGroupResponse:
                    HandleRegisterGroupResponse(message);
                    break;
                case MessageType.RequestGroupsResponse:
                    HandleRequestGroupsResponse(message);
                    break;
                case MessageType.JoinGroupResponse:
                    HandleJoinGroupResponse(message);
                    break;
                case MessageType.ChatMessage:
                    HandleChatMessage(message);
                    break;
            }
        }

        public static event EventHandler<LoggedInResponseEventArgs> LoggedInResponse;

        public class LoggedInResponseEventArgs : EventArgs
        {
            public int State { get; set; }
            public LoggedInResponseEventArgs(int state)
            {
                this.State = state;
            }
        }

        public static void SendLoginMessage(string email, string password)
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.SignInClient,
                LoginInfo = new LoginStruct()
                {
                    Email = email,
                    Password = password
                }
            });
        }

        internal static void SendRegisterMessage(string mail, string name, string password)
        {
            Client.Instance.Connection.SendData(new Message()
            {
                MessageType = MessageType.RegisterClient,
                RegisterInfo = new RegisterStruct()
                {
                    Username = name,
                    Login = new LoginStruct()
                    {
                        Email = mail,
                        Password = password
                    }
                }
            });
        }

        public static void SendRegisterGroupMessage(string name) {
            Client.Instance.Connection.SendData(new Message()
            {
                ClientId = Client.Instance.Id,
                MessageType = MessageType.RegisterGroup,
                PayloadData = name
            });
        }
        

        private static void HandleRegisterClientResponse(Message message)
        {
            switch (message.ClientId)
            {
                case -1:
                    Debug.WriteLine("e-mail al in gebruik");
                    //TODO: geef melding dat e-mail al in gebruik is.
                    break;
                default:
                    //TODO mooie pop up ofso
                    Debug.WriteLine("Account aangemaakt!");
                    Debug.WriteLine("ClientID: " + message.ClientId);

                    break;
            }
        }

        private static void HandleSignInClientResponse(Message message)
        {
            if (message.ClientId >= 0)
            {
                Client.Instance.Id = message.ClientId;
                message.GroupList.ForEach(group => Client.Instance.AddGroup(new Group(group)));
            }
            LoggedInResponse?.Invoke(null, new LoggedInResponseEventArgs(message.ClientId));
        }

        private static void HandleRegisterGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    Debug.WriteLine("failed to create group");
                    //TODO: geef melding dat het aanmaken van een group mislukt is
                    break;
                default:
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    Console.WriteLine("Group aangemaakt!");
                    break;
            }
        }
        
        private static void HandleRequestGroupsResponse(Message message)
        {
            List<Group> list = new List<Group>();
            // Convert Messenger_Client.Group to Shared.Group.
            message.GroupList.ForEach(group => list.Add(new Group(group)));

            // TODO: assign to some viewmodel list (with writelock?).
        }

        private static void HandleJoinGroupResponse(Message message)
        {
            switch (message.GroupID)
            {
                case -1:
                    Console.WriteLine("something went wrong!");
                    //TODO: create pop up
                    break;
                default:
                    Console.WriteLine("joined a group");
                    Client.Instance.AddGroup(new Group(message.GroupID, message.PayloadData));
                    break;
            }
        }

        private static void HandleChatMessage(Message message)
        {
            Group group = Client.Instance.GetGroup(message.GroupID);
            if (group != null)
            {
                group.AddMessage(message);
            }
            else
            {
                Console.WriteLine("group doesn't exist");
                //TODO: doe iets wanneer de group niet bestaat
            }
        }
    }
}
