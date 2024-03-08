namespace SurveyApi.Models
{
    public class SurveyAnswerSubmit
    {
        public required int SurveyTypeId { get; set; }
        public required List<SurveyAnswerProxy> SurveyAnswers { get; set; }
    }
}
