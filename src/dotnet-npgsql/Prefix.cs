namespace npgsql
{
    /// <summary>
    /// The type of prefix continuation character.
    /// </summary>
    public enum Prefix
    {
        /// <summary>
        /// The previous statement was complete.
        /// </summary>
        Complete = '=',

        /// <summary>
        /// The previous statement was incomplete.
        /// </summary>
        Incomplete = '-',

        /// <summary>
        /// The previous statement was incomplete due to a missing quote.
        /// </summary>
        Quote = '\'',

        /// <summary>
        /// The previous statement was incomplete due to a missing double quote.
        /// </summary>
        Identifier = '"',

        /// <summary>
        /// The previous statement was incomplete due to a missing parenthesis.
        /// </summary>
        Parenthsis = '('
    }
}