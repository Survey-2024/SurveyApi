namespace SurveyApi
{
    /// <summary>
    /// Property names must match App Configuration (Azure Resource) key values
    /// </summary>
    public class KeyVaultOptions
    {
        public string DbAdminPassword { get; set; } = string.Empty;
        public string ServiceBusConnStr { get; set; } = string.Empty;
    }
}
