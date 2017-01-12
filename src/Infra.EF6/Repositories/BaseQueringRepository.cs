//using System.Threading.Tasks;

//namespace Borg.Infra.EF6
//{
//    public abstract class BaseQueringRepository
//    {
//        protected delegate Task PreProcessAsyncHandler();

//        protected delegate void PreProcessHandler();

//        protected delegate Task PostProcessQueryAsyncHandler();

//        protected delegate void PostProcessQueryHandler();
//        protected PreProcessAsyncHandler PreProcessAsync { get; set; }
//        protected PreProcessHandler PreProcess { get; set; }
//        protected PostProcessQueryAsyncHandler PostProcessQueryAsync { get; set; }
//        protected PostProcessQueryHandler PostProcessQuery { get; set; }

//        protected void InternallPreProcess()
//        {
//            if (PreProcessAsync != null)
//            {
//                AsyncHelpers.RunSync(() => PreProcessAsync.Invoke());
//            }
//            else
//            {
//                PreProcess?.Invoke();
//            }
//        }
//        protected async Task InternallPreProcessAsync()
//        {
//            if (PreProcessAsync != null)
//            {
//                await PreProcessAsync.Invoke().AnyContext();
//            }
//            else
//            {
//                PreProcessAsync?.Invoke();
//            }
//        }
//        protected void InternallPostProcessQuery()
//        {
//            if (PostProcessQueryAsync != null)
//            {
//                AsyncHelpers.RunSync(() => PostProcessQueryAsync.Invoke());
//            }
//            else
//            {
//                PostProcessQuery?.Invoke();
//            }
//        }

//        protected async Task InternallPostProcessQueryAsync()
//        {
//            if (PostProcessQueryAsync != null)
//            {
//                await PostProcessQueryAsync.Invoke().AnyContext();
//            }
//            else
//            {
//                PostProcessQuery?.Invoke();
//            }
//        }

//    }
//}