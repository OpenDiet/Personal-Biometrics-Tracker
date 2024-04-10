namespace PersonalBiometricsTracker.Exceptions
{
    public class KeyAlreadyInUseException : Exception
    {
        public KeyAlreadyInUseException(string message) : base(message) { }
    }
}