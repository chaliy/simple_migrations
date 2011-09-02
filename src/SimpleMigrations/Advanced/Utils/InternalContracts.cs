using System;
using JetBrains.Annotations;

namespace SimpleMigrations.Advanced.Utils
{
    // Build-in BCL Contracts does not work without code-rewrite
    // this is not something I want for my lib, so this is 
    // stupid but working replacement.
    internal static class InternalContracts
    {
        public static void Require(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            object arg, [InvokerParameterName]string name)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
