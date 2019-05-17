namespace Chan.Net.Captchas
{
    public class IncorrectCredentialsException : PassException
    {
        public override string Message
        {
            get { return "Incorrect Token or PIN"; }
        }
    }
}