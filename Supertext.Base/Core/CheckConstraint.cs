using System;

namespace Supertext.Base.Core
{
    public class CheckConstraint
    {
        private readonly bool _assertion;

        public CheckConstraint(bool assertion)
        {
            _assertion = assertion;
        }

        public void OnConstraintFailure(Action onFailure)
        {
            if (!_assertion)
            {
                onFailure();
            }
        }
    }
}