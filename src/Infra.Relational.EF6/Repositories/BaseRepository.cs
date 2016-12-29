using System.Threading.Tasks;

namespace Borg.Infra.Relational.EF6
{
    public abstract class BaseRepository
    {
        protected delegate Task PreProcessAsyncHandler();

        protected delegate void PreProcessHandler();

        protected delegate Task PostProcessUpdateAsyncHandler();

        protected delegate void PostProcessUpdateHandler();

        protected delegate Task PostProcessQueryAsyncHandler();

        protected delegate void PostProcessQueryHandler();

        protected PreProcessAsyncHandler PreProcessAsync { get; set; }
        protected PreProcessHandler PreProcess { get; set; }
        protected PostProcessUpdateAsyncHandler PostProcessUpdateAsync { get; set; }
        protected PostProcessUpdateHandler PostProcessUpdate { get; set; }
        protected PostProcessQueryAsyncHandler PostProcessQueryAsync { get; set; }
        protected PostProcessQueryHandler PostProcessQuery { get; set; }

        protected void InternallPreProcess()
        {
            if (PreProcessAsync != null)
            {
                AsyncHelpers.RunSync(() => PreProcessAsync.Invoke());
            }
            else
            {
                PreProcess?.Invoke();
            }
        }

        protected async Task InternallPreProcessAsync()
        {
            if (PostProcessUpdateAsync != null)
            {
                await PreProcessAsync.Invoke().AnyContext();
            }
            else
            {
                PreProcess?.Invoke();
            }
        }

        protected void InternallPostProcessUpdate()
        {
            if (PostProcessUpdateAsync != null)
            {
                AsyncHelpers.RunSync(() => PostProcessUpdateAsync.Invoke());
            }
            else
            {
                PostProcessUpdate?.Invoke();
            }
        }

        protected async Task InternallPostProcessUpdateAsync()
        {
            if (PostProcessUpdateAsync != null)
            {
                await PostProcessUpdateAsync.Invoke().AnyContext();
            }
            else
            {
                PostProcessUpdate?.Invoke();
            }
        }

        protected void InternallPostProcessQuery()
        {
            if (PostProcessQueryAsync != null)
            {
                AsyncHelpers.RunSync(() => PostProcessQueryAsync.Invoke());
            }
            else
            {
                PostProcessQuery?.Invoke();
            }
        }

        protected async Task InternallPostProcessQueryAsync()
        {
            if (PostProcessQueryAsync != null)
            {
                await PostProcessQueryAsync.Invoke().AnyContext();
            }
            else
            {
                PostProcessQuery?.Invoke();
            }
        }
    }
}