using Shared.Packets.Enums;
using Shared.Packets.Server;
using System.Windows;
using System.Windows.Controls;

namespace ClientTest.TemplateSelectors
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            svChat c = item as svChat;
            switch (c.Type)
            {
                case ChatTypes.Error:
                    return element.FindResource("ChatError") as DataTemplate;
                case ChatTypes.Whisper:
                    return element.FindResource("ChatWhisper") as DataTemplate;
                case ChatTypes.Normal:
                    return element.FindResource("ChatNormal") as DataTemplate;
                case ChatTypes.Party:
                    return element.FindResource("ChatParty") as DataTemplate;
                case ChatTypes.Guild:
                    return element.FindResource("ChatGuild") as DataTemplate;
                case ChatTypes.Server:
                    return element.FindResource("ChatServer") as DataTemplate;
                case ChatTypes.All:
                    return element.FindResource("ChatAll") as DataTemplate;
            }

            return element.FindResource("ChatWhisper") as DataTemplate;
        }
    }
}
