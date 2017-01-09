using System.Threading.Tasks;

namespace Borg.Infra.EF6
{
    public abstract class BaseRepository
    {
        protected delegate Task PostProcessUpdateAsyncHandler();

        protected delegate void PostProcessUpdateHandler();

        protected PostProcessUpdateAsyncHandler PostProcessUpdateAsync { get; set; }
        protected PostProcessUpdateHandler PostProcessUpdate { get; set; }

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
    }
}