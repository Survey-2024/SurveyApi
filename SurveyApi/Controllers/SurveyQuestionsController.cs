using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyApi.Enums;
using SurveyApi.Models;

namespace SurveyApi.Controllers
{
    /// <summary>
    /// Survey Questions
    /// </summary>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyQuestionsController(DbSurveyProjectContext context) : ControllerBase
    {

        /// <summary>
        /// Get all Survey Questions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyQuestion>>> GetSurveyQuestions()
        {
            return await context.SurveyQuestions.ToListAsync();
        }

        /// <summary>
        /// Gets Survey Questions based on Type
        /// </summary>
        /// <returns>List of SurveyQuestion</returns>
        [HttpGet("getsurveyquestions/{surveyType}")]
        public async Task<ActionResult<IEnumerable<SurveyQuestion>>> GetSurveyQuestions(string surveyType)
        {
            var filteredSurveys = await context.SurveyQuestions.Where(q => q.SurveyType.Origin == surveyType).ToListAsync();

            return filteredSurveys;
        }

        /// <summary>
        /// Get Survey Question By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SurveyQuestion</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyQuestion>> GetSurveyQuestion(int id)
        {
            var surveyQuestion = await context.SurveyQuestions.FindAsync(id);

            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return surveyQuestion;
        }
    }
}
