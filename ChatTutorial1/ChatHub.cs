using System;
using System.Collections.Generic;
using System.Linq;
using ChatDAL.CommonModels;
using Microsoft.AspNet.SignalR;

namespace ChatTutorial1
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);

            if (!string.IsNullOrEmpty(message))
            {
                var logsToSave = new List<ChatLog>()
                {
                    new ChatLog(name, message, DateTime.Now.ToString())
                };

                SaveChatLogs(logsToSave);
            }
        }

        public void TypingIndicator(string name)
        {
            Clients.Others.someoneIsTyping(name);
        }

        public void GetChatLogs()
        {
            var chatlogs =  new HistoryService().GetChatLogs().OrderByDescending(c=>c.TimeStamp);

            if (chatlogs.Any())
            {
                //Clients.Caller.broadcastMessage("", "Chat history:");

                foreach (var chat in chatlogs)
                {
                    Clients.Caller.broadcastMessage(chat.DisplayName, chat.Chat);
                }
            }
        }

        public void SaveChatLogs(List<ChatLog> logs)
        {
            new HistoryService().SaveChatLogs(logs);
        }
    }
}