using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Models;

namespace SurveyApi.Controllers
{
    /// <summary>
    /// Survey Answers
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyAnswersController(DbSurveyProjectContext context) : ControllerBase
    {

        /// <summary>
        /// Get all Survey Answers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyAnswer>>> GetSurveyAnswers()
        {
            return await context.SurveyAnswers.ToListAsync();
        }

        /// <summary>
        /// Get Survey Answer by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyAnswer>> GetSurveyAnswer(int id)
        {
            var surveyAnswer = await context.SurveyAnswers.FindAsync(id);

            if (surveyAnswer == null)
            {
                return NotFound();
            }

            return surveyAnswer;
        }
    }
}
