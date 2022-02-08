namespace Play.Common
{
    //Represents the ServiceSettings in appsettings.json
    public class ServiceSettings
    {
        public string ServiceName { get; init; }

        //For the Identity authority for issuing tokens
        public string Authority { get; init;}
    }
}