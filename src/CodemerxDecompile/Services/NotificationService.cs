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

using System;
using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public class NotificationService : INotificationService
{
    private INotificationHandler? handler;
    
    public void RegisterHandler(INotificationHandler handler)
    {
        this.handler = handler;
    }

    public void ShowNotification(Notification notification)
    {
        if (handler == null)
            throw new InvalidOperationException($"{nameof(NotificationService)} should be first initialized using {nameof(RegisterHandler)}.");
        
        handler.ShowNotification(notification);
    }

    public void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification)
    {
        if (handler == null)
            throw new InvalidOperationException($"{nameof(NotificationService)} should be first initialized using {nameof(RegisterHandler)}.");
        
        handler.ReplaceNotification(notificationToBeReplaced, replacementNotification);
    }
}
