/*
    Copyright CodeMerx 2024
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

using System.Diagnostics;
using CodemerxDecompile.Notifications;
using CodemerxDecompile.Services;

namespace CodemerxDecompile.ViewModels.Design;

public class DesignNotificationsViewModel : NotificationsViewModel
{
    public DesignNotificationsViewModel()
        : base(new DesignNotificationService())
    {
        Notifications.Add(new Notification
        {
            Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            Level = NotificationLevel.Information,
            Actions = new[]
            {
                new NotificationAction()
                {
                    Title = "Click here",
                    Action = () => Process.Start(new ProcessStartInfo("https://codemerx.com") { UseShellExecute = true })
                },
                new NotificationAction()
                {
                    Title = "or here",
                    Action = () => Process.Start(new ProcessStartInfo("https://decompiler.codemerx.com") { UseShellExecute = true })
                }
            }
        });
        Notifications.Add(new Notification
        {
            Message = "Success",
            Level = NotificationLevel.Success
        });
        Notifications.Add(new Notification
        {
            Message = "Warning",
            Level = NotificationLevel.Warning
        });
        Notifications.Add(new Notification
        {
            Message = "Error",
            Level = NotificationLevel.Error
        });
    }
}

file class DesignNotificationService : INotificationService
{
    public void RegisterHandler(INotificationHandler handler)
    {
    }

    public void ShowNotification(Notification notification)
    {
    }

    public void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification)
    {
    }
}
