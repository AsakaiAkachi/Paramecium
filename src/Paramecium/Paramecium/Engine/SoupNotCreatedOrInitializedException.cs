namespace Paramecium.Engine
{
    public class SoupNotCreatedOrInitializedException : Exception
    {
        public SoupNotCreatedOrInitializedException() : base("Soup has not been created or initialized.")
        {
        }

        public SoupNotCreatedOrInitializedException(string message) : base(message)
        {
        }

        public SoupNotCreatedOrInitializedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
