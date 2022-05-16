namespace Supertext.Base.Specs.Factory.AttributeComponents
{
    internal class SomethingWithParameters : ISomethingWithParameters
    {
        public SomethingWithParameters(string parameter)
        {
            Parameter = parameter;
        }

        public string Parameter { get; }
    }
}