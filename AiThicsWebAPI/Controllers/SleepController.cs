using System.Collections.Generic;
using System.Threading.Tasks;
using aithics.service.Interfaces;
using aithics.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace aithics.api.Controllers
{
    [Route("sleep")]  // ✅ To follow REST API naming convention
    [ApiController]
    [Authorize]  // ✅ Requires JWT authentication
    public class SleepController : ControllerBase
    {
        private readonly ISleepTrackerService _sleepTrackerService;

        public SleepController(ISleepTrackerService sleepTrackerService)
        {
            _sleepTrackerService = sleepTrackerService;
        }

        // ✅ API to Get All Sleep Records
        //[HttpGet("all")]
        //[AllowAnonymous]  // ✅ This endpoint can be accessed without authentication       
        // [Authorize]     // Can be called individually also without calling at header(line no 11)

        [HttpGet("all")]

        public async Task<ActionResult<IEnumerable<SleepTracker>>> GetAllSleepRecords()
        {
            var records = await _sleepTrackerService.GetAllSleepRecordsAsync();
            return Ok(records);
        }


    }
}
