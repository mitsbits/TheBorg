using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Borg.Framework.MVC;
using Borg.Framework.System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Framework.System.Domain;
using System.Threading.Tasks;
using Borg.Framework.Backoffice.Pages.Data;
using Borg.Infra.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    public class ComponentController<TComponent> : BackofficeController where TComponent : Component
    {
        private static ConcurrentDictionary<Type, IEnumerable<OrderByInfo<TComponent>>> _orderbys =
            new ConcurrentDictionary<Type, IEnumerable<OrderByInfo<TComponent>>>();
        protected IDbContextScopeFactory ScopeFactory { get; }
        protected ICRUDRespoditory<TComponent> Repository { get; }

        public ComponentController(IBackofficeService<BorgSettings> systemService, IDbContextScopeFactory scopeFactory) : base(systemService)
        {
            ScopeFactory = scopeFactory;
            Repository = ScopeFactory.CreateRepo<ICRUDRespoditory<TComponent>>();
        }


        public virtual async Task<IActionResult> Index(int? id)
        {
            return id.HasValue ? View("~/Areas/Backoffice/Views/Component/Item.cshtml", await ReadOne(id.Value)) : View("~/Areas/Backoffice/Views/Component/Index.cshtml", await ReadPage());
        }



        protected virtual async Task<IPagedResult<Tidings>> ReadPage()
        {
            using (ScopeFactory.CreateReadOnly())
            {
                var collection = await Repository.FindAsync(x => true, Pager.Current, Pager.RowCount, GetOrderBys());
                var data = collection.Select(x => new Dto<TComponent>(x)).Select(x => x.Props).ToArray();
                return new PagedResult<Tidings>(data, collection.Page, collection.PageSize, collection.TotalRecords);
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

        private IEnumerable<OrderByInfo<TComponent>> GetOrderBys()
        {
            var type = typeof(TComponent);
            if (!_orderbys.ContainsKey(type))
            {
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                var hits = props.Where(p => p.CustomAttributes.Any(c => c.AttributeType == typeof(OrderByAttribute)));
                if (hits.Any())
                {
                    var internalDict = new List<Tuple<string, int, PropertyInfo, OrderByAttribute>>();
                    foreach (var source in hits)
                    {
                        var attr = source.GetCustomAttribute<OrderByAttribute>();
                        internalDict.Add(Tuple.Create(source.Name, attr.Precedence, source, attr));
                    }
                    var ords = new List<OrderByInfo<TComponent>>();
                    foreach (var tuple in internalDict.OrderByDescending(x => x.Item2))
                    {
                        ParameterExpression parameterExpression = Expression.Parameter(typeof(TComponent), "c");
                        var memberExpression = Expression.Property(parameterExpression, tuple.Item3);
                        Expression conversion = Expression.Convert(memberExpression, typeof(object));
                        Expression<Func<TComponent, dynamic>> composedLambdaExpression =
                            Expression.Lambda<Func<TComponent, dynamic>>(conversion, tuple.Item1,
                                new ParameterExpression[] { parameterExpression });
                        ords.Add(
                            new OrderByInfo<TComponent>()
                            {
                                Ascending = tuple.Item4.Ascending,
                                Property = composedLambdaExpression
                            });

                    }
                    _orderbys[typeof(TComponent)] = ords;
                }
                else
                {
                    _orderbys[typeof(TComponent)] = new[]
                        {new OrderByInfo<TComponent>() {Ascending = false, Property = (c) => c.Id},};
                }


            }
            return _orderbys[typeof(TComponent)];
        }
    }

    public class Dto<TComponent> where TComponent : Component
    {
        public TComponent Source { get; }

        public Tidings Props { get; }
        public Dto(TComponent source)
        {
            Source = source;
            Props = new Tidings();
            var props = typeof(TComponent).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in props)
            {
                try
                {

                    var val = propertyInfo.GetValue(Source).ToString();
                    Props.Add(propertyInfo.Name, val);

                }
                catch (Exception e)
                {
                    Props.Add(propertyInfo.Name, string.Empty);
                }
            }
        }
    }
}