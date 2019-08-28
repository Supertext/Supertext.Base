using System;

namespace Supertext.Base.Common
{
    internal class SequentialGuidFactory : ISequentialGuidFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public SequentialGuidFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Guid Create()
        {
            return SequentialGuid.NewSequentialGuid(_dateTimeProvider);
        }
    }
}