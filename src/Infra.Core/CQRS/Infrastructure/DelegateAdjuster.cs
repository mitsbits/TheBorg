using System;
using System.Linq.Expressions;

namespace Borg.Infra.CQRS
{
    internal class DelegateAdjuster
    {
        public static Action<BaseT> CastArgument<BaseT, DerivedT>(Expression<Action<DerivedT>> source) where DerivedT : BaseT
        {
            if (typeof(DerivedT) == typeof(BaseT))
            {
                return (Action<BaseT>)((Delegate)source.Compile());
            }
            ParameterExpression sourceParameter = Expression.Parameter(typeof(BaseT), "source");
            var result = Expression.Lambda<Action<BaseT>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(sourceParameter, typeof(DerivedT))),
                sourceParameter);
            return result.Compile();
        }

        public static Func<BaseT, IResponse> CastArgument<BaseT, DerivedT>(Expression<Func<DerivedT, IResponse>> source) where DerivedT : BaseT
        {
            if (typeof(DerivedT) == typeof(BaseT))
            {
                return (Func<BaseT, IResponse>)((Delegate)source.Compile());
            }
            ParameterExpression sourceParameter = Expression.Parameter(typeof(BaseT), "source");
            var result = Expression.Lambda<Func<BaseT, IResponse>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(sourceParameter, typeof(DerivedT))),
                sourceParameter);
            return result.Compile();
        }
    }
}