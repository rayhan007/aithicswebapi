using aithics.data.Models;

namespace aithics.service.Interfaces
{
    public interface ISleepTrackerService
    {
        Task<IEnumerable<SleepTracker>> GetAllSleepRecordsAsync();
    }
}
