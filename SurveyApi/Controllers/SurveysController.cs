using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SurveyApi.Enums;
using SurveyApi.Models;

namespace SurveyApi.Controllers
{
    /// <summary>
    /// Surveys
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController(DbSurveyProjectContext context, IOptions<KeyVaultOptions> options) : ControllerBase
    {
        private readonly DbSurveyProjectContext _context = context;
        private readonly IOptions<KeyVaultOptions> _options = options;

        /// <summary>
        /// Get all Surveys including relations with Survey Types and their Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Survey>>> GetSurveys()
        {
            var surveys = await _context.Surveys
                .Include(s => s.SurveyType)
                .Include(s => s.SurveyStatus)
            .ToListAsync();

            return surveys;
        }

        /// <summary>
        /// Get Survey by ID with Answers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Survey>> GetSurvey(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.SurveyAnswers)
                .FirstOrDefaultAsync(i => i.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var answers in survey.SurveyAnswers)
                {
                    var questions = _context.SurveyQuestions.ToList();

                    if (questions != null)
                    {
                        answers.SurveyQuestion.QuestionText = questions
                            .Find(q => q.SurveyQuestionId == answers.SurveyQuestionId)!.QuestionText;
                    }
                }
            }

            return survey;
        }

        /// <summary>
        /// Creates a new Survey record, inserts into Answers, inserts into SurveyAnswers join table
        /// </summary>
        /// <param name="surveyAnswerSubmit"></param>
        /// <returns>Status201Created</returns>
        [HttpPost("createsurvey")]
        public async Task<ActionResult<Survey>> CreateSurvey(SurveyAnswerSubmit surveyAnswerSubmit)
        {
            if (surveyAnswerSubmit.SurveyTypeId < 1 || surveyAnswerSubmit.SurveyAnswers == null)
            {
                return BadRequest();
            }

            Survey survey = new()
            {
                SurveyTypeId = surveyAnswerSubmit.SurveyTypeId,
                SurveyStatusId = (int)SurveyStatusEnum.SurveyStatus.SurveyStatusDraft,
                CreatedDate = DateTimeOffset.Now
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            int surveyId = survey.SurveyId;

            foreach (var answer in surveyAnswerSubmit.SurveyAnswers)
            {
                _context.SurveyAnswers.Add(new SurveyAnswer
                {
                    SurveyQuestionId = answer.SurveyQuestionId,
                    AnswerText = answer.AnswerText,
                    SurveyId = surveyId
                });
            }

            await _context.SaveChangesAsync();

            foreach (var surveyAnswer in survey.SurveyAnswers)
            {
                AnswersInSurvey answersInSurvey = new()
                {
                    SurveyId = surveyId,
                    SurveyAnswerId = surveyAnswer.SurveyAnswerId,
                };

                _context.AnswersInSurveys.Add(answersInSurvey);
            }

            await _context.SaveChangesAsync();

            await SendMessageToServiceBus($"New Survey Created: Survey ID = {survey.SurveyId}");

            return CreatedAtAction("GetSurvey", new { id = survey.SurveyId }, survey);
        }

        /// <summary>
        /// Contact Service Bus resource and append to queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<Task> SendMessageToServiceBus(string message)
        {
            // Publish Message to Service Bus

            ServiceBusClient clientServiceBus;
            ServiceBusSender senderServiceBus;

            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            clientServiceBus = new ServiceBusClient(_options.Value.ServiceBusConnStr, clientOptions);

            // is "surveyqueue" the name of a resource? At least create a variable.
            senderServiceBus = clientServiceBus.CreateSender("surveyqueue");

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await senderServiceBus.SendMessageAsync(new ServiceBusMessage(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await senderServiceBus.DisposeAsync();
                await clientServiceBus.DisposeAsync();
            }

            return Task.CompletedTask;
        }
    }
}
