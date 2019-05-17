namespace Chan.Net.Captchas
{
    public class PassAlreadyInUseException : PassException
    {
        public override string Message
        {
            get { return "This Pass is already in use by another IP"; }
        }
    }
}