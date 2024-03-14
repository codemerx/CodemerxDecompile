using System.Collections.Generic;

namespace CodemerxDecompile.Notifications;

public record Notification
{
    public required string Message { get; init; }
    public required NotificationLevel Level { get; init; }
    public IEnumerable<NotificationAction>? Actions { get; init; }
}
