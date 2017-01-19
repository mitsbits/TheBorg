using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Borg.Infra.Relational
{
    public class OrderByInfo<T> where T : class
    {
        public Expression<Func<T, dynamic>> Property;
        public bool Ascending;

        public string TruePropertyName => TruePropertyNameInternal(Property);

        private static string TruePropertyNameInternal(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpr = expression.Body as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = expression.Body as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                var prp = $"{expression.Parameters[0].Name}.";
                var nme = $"{memberExpr.Expression}.{memberExpr.Member.Name}";
                if (nme.StartsWith(prp)) nme = nme.Substring(prp.Length);
                return nme;
            }

            throw new ArgumentException("No property reference expression was found.", nameof(expression));
        }

        public override string ToString()
        {
            return $"{typeof(T).Name}.{TruePropertyName}|" + (Ascending ? "ASC" : "DESC");
        }
    }
}