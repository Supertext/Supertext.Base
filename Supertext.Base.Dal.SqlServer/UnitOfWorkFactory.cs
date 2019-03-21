using Supertext.Base.Common;
using Supertext.Base.Factory;

namespace Supertext.Base.Dal.SqlServer
{
    internal class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IFactory<string, IUnitOfWork> _unitOfWorkFactory;

        public UnitOfWorkFactory(IFactory<string, IUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public IUnitOfWork Create(string connectionString)
        {
            Validate.NotEmpty(connectionString, nameof(connectionString));
            Validate.NotBlank(connectionString, nameof(connectionString));

            return _unitOfWorkFactory.Create(connectionString);
        }
    }
}