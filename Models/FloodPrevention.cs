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
        public static ConcurrentDictionary<string, FloodTracker> FloodTrackMap = new ConcurrentDictionary<string, FloodTracker>();

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
        }

        public static int MaxAttempts = 4;

        public static TimeSpan RejectFor = new TimeSpan(hours: 0, minutes: 10, seconds: 0);

        public class FloodTracker
        {
            public int Attempts;

            public DateTimeOffset RejectedUntil;
        }
    }
}
