using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Borg.Infra.Core.Tests
{

    public class AsyncHelpersTests
    {
        private DummyWorker _dummyWorker;


        public  AsyncHelpersTests()
        {
            _dummyWorker = new DummyWorker();
        }

        [Fact]
        public void check_that_we_can_run_a_task_as_void()
        {
            _dummyWorker.Reset();
            Should.NotThrow(() =>
            {
                AsyncHelpers.RunSync(() => _dummyWorker.DoAsync());
            });
            _dummyWorker.Runs.ShouldBe(1);
        }

        [Fact]
        public void check_that_we_can_run_a_task_as_function_that_returns_string()
        {
            _dummyWorker.Reset();
            var result = string.Empty;
            Should.NotThrow(() =>
            {
                result = AsyncHelpers.RunSync(() => _dummyWorker.ReturnAsync());
            });
            _dummyWorker.Runs.ShouldBe(1);
            result.ShouldBe(DummyWorker.ReturnAsyncStringResult);
        }

        [Fact]
        public void check_that_we_can_run_a_task_as_function_that_returns_string_and_accepts_a_string()
        {
            _dummyWorker.Reset();
            var result = string.Empty;
            var input = "-ooo-ooooo-oo";
            Should.NotThrow(() =>
            {
                result = AsyncHelpers.RunSync(() => _dummyWorker.ReturnAsync(input));
            });
            _dummyWorker.Runs.ShouldBe(1);
            result.ShouldBe($"{DummyWorker.ReturnAsyncStringResult}{input}" );
        }

        private class DummyWorker
        {
            public const string ReturnAsyncStringResult = "xxx-xxxxx-xxxx-xx";
            private int _runs = 0;

            public int Runs => _runs;

            public void Reset()
            {
                _runs = 0;
            }

            public Task DoAsync()
            {
                _runs++; return Task.CompletedTask;
            }

            public Task<string> ReturnAsync()
            {
                _runs++;
                return Task.FromResult(ReturnAsyncStringResult);
            }

            public Task<string> ReturnAsync(string input)
            {
                _runs++;
                return Task.FromResult($"{ReturnAsyncStringResult}{input}");
            }
        }
    }
}