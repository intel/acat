using System;
using System.Threading.Tasks;
using ACAT.Core.Utility;
using Xunit;

namespace ACATCore.Tests
{
    public class TaskExtensionsTests
    {
        [Fact]
        public async Task ForgetObservesTaskException()
        {
            var expected = new InvalidOperationException("test");
            var observed = new TaskCompletionSource<Exception>();

            Task.FromException(expected).Forget(ex =>
            {
                observed.TrySetResult(ex);
            });

            Task completed = await Task.WhenAny(observed.Task, Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken));

            Assert.Same(observed.Task, completed);
            Assert.Same(expected, await observed.Task);
        }
    }
}
