using System;
using System.Linq;
using System.Linq.Expressions;
using Gridify.Syntax;

namespace UcarMobileApi.Infrastructure.Configurations.Gridify.Operators
{
    /// <summary>
    /// usage: .ApplyFiltering("name #In John;David;Felipe")
    /// NOTE: this operator assumes the target property is a string.
    /// </summary>
    public class InOperator : IGridifyOperator
    {
        public string GetOperator() => "#In";

        public Expression<OperatorParameter> OperatorHandler()
        {
            // expression-bodied lambda (no { } ), no ?. and no .ToString()
            return (prop, value) =>
                ((string)value)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Contains((string)prop);
        }
    }
}
