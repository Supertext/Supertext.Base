namespace Supertext.Base.Authentication
{
    public class AlternativeAuthorityDetails
    {
        public string Authority { get; set; }

        public string ClientSecret { get; set; }

        public AlternativeAuthorityDetails(string authority, string clientSecret)
        {
            Authority = authority;
            ClientSecret = clientSecret;
        }
    }
}