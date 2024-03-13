using System;

namespace CodemerxDecompile.Notifications;

public record NotificationAction
{
    public required string Title { get; init; }
    public required Action Action { get; init; }
}
