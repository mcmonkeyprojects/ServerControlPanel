using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ServerControlPanel.Models
{
    public static class FloodPrevention
    {
        /// <summary>
        /// How many invalid password attempts in a short period before the attempts are considered spam.
        /// </summary>
        public static int MaxAttempts = 4;

        /// <summary>
        /// How long to reject flooders for.
        /// </summary>
        public static TimeSpan RejectFor = new(hours: 0, minutes: 10, seconds: 0);

        /// <summary>
        /// How often to clear the flood prevention map. This is to reduce risk of RAM waste when a large number of source IPs make invalid auth attempts.
        /// </summary>
        public static TimeSpan ClearRate = new(hours: 2, minutes: 0, seconds: 0);

        public static Object ClearingLock = new();

        public static ConcurrentDictionary<string, FloodTracker> FloodTrackMap = new();

        public static DateTimeOffset NextClear = DateTimeOffset.Now;

        public static bool ShouldDeny(string flooder)
        {
            return FloodTrackMap.TryGetValue(flooder.ToLowerInvariant(), out FloodTracker tracker)
                && tracker.Attempts >= MaxAttempts && tracker.RejectedUntil > DateTimeOffset.Now;
        }

        public static void NoteFlooding(string flooder)
        {
            FloodTracker tracker = FloodTrackMap.GetOrAdd(flooder.ToLowerInvariant(), new FloodTracker());
            lock (tracker)
            {
                if (tracker.Attempts > 0 && tracker.RejectedUntil < DateTimeOffset.Now)
                {
                    tracker.Attempts = 0;
                }
                tracker.Attempts++;
                tracker.RejectedUntil = DateTimeOffset.Now.Add(RejectFor);
            }
            if (NextClear < DateTimeOffset.Now)
            {
                lock (ClearingLock)
                {
                    if (NextClear < DateTimeOffset.Now)
                    {
                        NextClear = DateTimeOffset.Now.Add(ClearRate);
                        FloodTrackMap = new ConcurrentDictionary<string, FloodTracker>();
                    }
                }
            }
        }

        public class FloodTracker
        {
            public int Attempts;

            public DateTimeOffset RejectedUntil;
        }
    }
}
