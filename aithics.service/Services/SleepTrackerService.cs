using aithics.data;
using aithics.data.Models;
using aithics.service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace aithics.service.Services
{
    public class SleepTrackerService : ISleepTrackerService
    {
        private readonly AithicsDbContext _context;

        public SleepTrackerService(AithicsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SleepTracker>> GetAllSleepRecordsAsync()
        {
            return await _context.SleepTrackers.ToListAsync();
        }
    }
}
