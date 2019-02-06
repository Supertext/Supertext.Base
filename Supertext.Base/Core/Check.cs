namespace Supertext.Base.Core
{
    public static class Check
    {
        public static CheckConstraint That(bool assertion)
        {
            return new CheckConstraint(assertion);
        }
    }
}