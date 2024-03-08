using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Models;

namespace SurveyApi.Controllers
{
    /// <summary>
    /// Survey Status starts as Draft
    /// </summary>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyStatusController(DbSurveyProjectContext context) : ControllerBase
    {
        /// <summary>
        /// Get all Survey Statuses
        /// </summary>
        /// <returns>List of SurveyStatus</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyStatus>>> GetSurveyStatuses()
        {
            return await context.SurveyStatuses.ToListAsync();
        }

        /// <summary>
        /// Get Survey Status by Status ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SurveyStatus</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyStatus>> GetSurveyStatus(int id)
        {
            var surveyStatus = await context.SurveyStatuses.FindAsync(id);

            if (surveyStatus == null)
            {
                return NotFound();
            }

            return surveyStatus;
        }
    }
}
