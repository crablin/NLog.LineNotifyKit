using NLog.LineNotifyKit.Models;

namespace NLog.LineNotifyKit
{
    public interface ILineNotifyLogger
    {
        /// <summary>
        /// To the line notify request.
        /// </summary>
        /// <returns></returns>
        LineNotifyRequest ToLineNotifyRequest(LogEventInfo info);
    }
}
