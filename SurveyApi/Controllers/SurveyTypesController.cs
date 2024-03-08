using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Models;

namespace SurveyApi.Controllers
{
    /// <summary>
    /// Survey types can be Domestic or Foreign
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyTypesController(DbSurveyProjectContext context) : ControllerBase
    {
        /// <summary>
        /// Get all Survey Types
        /// </summary>
        /// <returns>List of SurveyType</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyType>>> GetSurveyTypes()
        {
            return await context.SurveyTypes.ToListAsync();
        }

        /// <summary>
        /// Get Survey Type by Type ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SurveyType</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyType>> GetSurveyType(int id)
        {
            var surveyType = await context.SurveyTypes.FindAsync(id);

            if (surveyType == null)
            {
                return NotFound();
            }

            return surveyType;
        }
    }
}
