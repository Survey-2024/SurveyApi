namespace SurveyApi.Enums
{
    /// <summary>
    /// Survey Status Enums
    /// </summary>
    public class SurveyStatusEnum
    {
        /// <summary>
        /// Survey Statuses
        /// </summary>
        public enum SurveyStatus
        {
            /// <summary>
            /// Draft
            /// </summary>
            SurveyStatusDraft = 1,
            /// <summary>
            /// Review
            /// </summary>
            SurveyStatusReview,
            /// <summary>
            /// Published
            /// </summary>
            SurveyStatusPublished
        }
    }
}
