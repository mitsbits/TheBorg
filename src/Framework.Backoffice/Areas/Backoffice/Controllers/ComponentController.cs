using Borg.Framework.MVC;
using Borg.Framework.System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Framework.System.Domain;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    public class ComponentController<TComponent> : BackofficeController where TComponent : Component
    {
        protected IDbContextScopeFactory ScopeFactory { get; }
        protected ICRUDRespoditory<TComponent> Repository { get; }

        public ComponentController(ISystemService<BorgSettings> systemService, IDbContextScopeFactory scopeFactory) : base(systemService)
        {
            ScopeFactory = scopeFactory;
            Repository = ScopeFactory.CreateRepo<ICRUDRespoditory<TComponent>>();
        }

        protected virtual async Task<IPagedResult<TComponent>> ReadPage()
        {
            using (ScopeFactory.CreateReadOnly())
            {
                return await Repository.FindAsync(x => true, Pager.Current, Pager.RowCount, new []{new OrderByInfo<TComponent>() {Property = x=> x.Id, Ascending = false} });
            }
        }

        protected virtual async Task<TComponent> ReadOne(int id)
        {
            using (ScopeFactory.CreateReadOnly())
            {
                return await Repository.GetAsync(x => x.Id.Equals(id));
            }
        }

        protected virtual async Task<TComponent> ReadOne(string cqrsId)
        {
            using (ScopeFactory.CreateReadOnly())
            {
                return await Repository.GetAsync(x => x.CQRSKey.Equals(cqrsId));
            }
        }

        protected virtual async Task<TComponent> Update(TComponent component)
        {
            using (var db = ScopeFactory.Create())
            {
                var result = await Repository.UpdateAsync(component);
                await db.SaveChangesAsync();
                return result;
            }
        }
    }
}