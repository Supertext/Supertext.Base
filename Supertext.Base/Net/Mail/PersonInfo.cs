namespace Supertext.Base.Net.Mail
{
    public class PersonInfo
    {
        public PersonInfo(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; }
        public string Email { get; }
    }
}
