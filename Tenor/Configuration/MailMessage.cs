namespace Tenor.Configuration
{
    /// <summary>
    /// Encapsulates default settings for MailMessage logic.
    /// </summary>
    public static class MailMessage
    {

        /// <summary>
        /// Defines the max length of a template key.
        /// </summary>
        /// <seealso cref="Tenor.Mail.MailMessage"/>
        public const int MaxLengthTemplateKey = 120;

        /// <summary>
        /// Defines the max length of a template value.
        /// </summary>
        /// <seealso cref="Tenor.Mail.MailMessage"/>
        public const int MaxLengthTemplateValue = 4000;
    }
}