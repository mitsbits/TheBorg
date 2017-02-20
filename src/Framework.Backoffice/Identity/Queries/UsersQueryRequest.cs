
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Borg.Framework.Backoffice.Identity.Models;
using Borg.Infra.CQRS;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;

namespace Borg.Framework.Backoffice.Identity.Queries
{
    public class UsersQueryRequest : EntityQueryRequest<BorgUser>
    {
        public UsersQueryRequest(Expression<Func<BorgUser, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<BorgUser>> orderBy, params Expression<Func<BorgUser, dynamic>>[] paths) : base(predicate, page, size, orderBy, paths)
        {
        }

    }
    namespace Borg.Framework.Backoffice.Identity.Queries
    {
        public class UsersQueryRequestHandler : ScopeFactoryEntityQueryHandler<UsersQueryRequest, BorgUser>, IHandlesQueryRequest<UsersQueryRequest>
        {
   

            public UsersQueryRequestHandler(IDbContextScopeFactory dbContextScopeFactory, IQueryRepository<BorgUser> repository) : base(dbContextScopeFactory, repository)
            {
            }


            public new async Task<IQueryResult> Execute(UsersQueryRequest message)
            {
                return await base.Execute(message);
            }
        }
    }
}
