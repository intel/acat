using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.Configuration;
using ACAT.Core.DataAccess;
using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ACATCore.Tests.Integration
{
    [TestClass]
    [DoNotParallelize]
    public class StartupAsyncIntegrationTests
    {
        private string _workspace;
        private IServiceProvider _originalProvider;

        [TestInitialize]
        public void Setup()
        {
            _workspace = Path.Combine(Environment.CurrentDirectory, "startup-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_workspace);
            _originalProvider = Context.ServiceProvider;
            Context.ServiceProvider = new EmptyServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(_workspace, true);
            Context.ServiceProvider = _originalProvider;
        }

        [TestMethod]
        public async Task ThemeRepository_AsyncMatchesSynchronousLoad()
        {
            string path = Path.Combine(_workspace, "Theme.json");
            File.WriteAllText(path, JsonSerializer.Serialize(ThemeJson.CreateDefaultHighContrast()));
            var repository = new ThemeRepository();
            using (Theme synchronous = repository.Load(path))
            using (Theme asynchronous = await repository.LoadAsync(path).ConfigureAwait(false))
            {
                Assert.IsNotNull(synchronous);
                Assert.IsNotNull(asynchronous);
                Assert.AreEqual(synchronous.Name, asynchronous.Name);
            }
            Assert.IsNull(await repository.LoadAsync(Path.Combine(_workspace, "missing.json")).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task ThemeManager_AsyncSelectionPreservesDefaultFallback()
        {
            string path = Path.Combine(_workspace, "Theme.json");
            File.WriteAllText(path, JsonSerializer.Serialize(ThemeJson.CreateDefaultHighContrast()));
            Theme originalDefault = ThemeManager.DefaultTheme;
            ThemeManager.DefaultTheme = Theme.Create(ThemeManager.DefaultThemeName);
            try
            {
                using (var manager = (ThemeManager)Activator.CreateInstance(typeof(ThemeManager),
                    BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new object[] { NullLogger<ThemeManager>.Instance, null }, CultureInfo.InvariantCulture))
                {
                    manager.ThemesLookupTable.Add(ThemeManager.DefaultThemeName, _workspace);
                    Assert.IsTrue(await manager.SetActiveThemeAsync("unknown").ConfigureAwait(false));
                    Assert.AreEqual(ThemeManager.DefaultThemeName, manager.ActiveThemeName);
                    Assert.IsTrue(manager.SetActiveTheme("unknown"));
                    Assert.AreEqual(ThemeManager.DefaultThemeName, manager.ActiveTheme.Name);
                }
            }
            finally
            {
                ThemeManager.DefaultTheme = originalDefault;
            }
        }

        [TestMethod]
        public async Task AbbreviationsManager_AsyncKeepsMissingFileBehavior()
        {
            using (var manager = (AbbreviationsManager)Activator.CreateInstance(typeof(AbbreviationsManager),
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[] { NullLogger<AbbreviationsManager>.Instance }, CultureInfo.InvariantCulture))
            {
                string path = Path.Combine(_workspace, "missing.json");
                Assert.IsTrue(manager.Init(path));
                Assert.IsTrue(await manager.InitAsync(path).ConfigureAwait(false));
                Assert.IsNotNull(manager.Abbreviations);
                File.WriteAllText(path, "{\"version\":\"1.0.0\",\"abbreviations\":[{\"word\":\"btw\",\"replaceWith\":\"by the way\",\"mode\":\"Write\"}]}");
                Assert.IsTrue(await manager.InitAsync(path).ConfigureAwait(false));
                Assert.IsNotNull(manager.Abbreviations.Lookup("btw"));
                File.WriteAllText(path, "{ invalid json");
                Assert.IsFalse(await manager.InitAsync(path).ConfigureAwait(false));
            }
        }

        [TestMethod]
        public Task WordPrediction_OptionalAsyncInitAndFallbackStayOnSta()
        {
            return OnStaAsync(async () =>
            {
                using (var manager = CreatePredictorManager(typeof(AsyncPredictor)))
                {
                    int owner = Thread.CurrentThread.ManagedThreadId;
                    AsyncPredictor.Succeed = true;
                    Assert.IsTrue(await manager.SetActiveWordPredictorAsync(new CultureInfo("en")).ConfigureAwait(false));
                    Assert.AreEqual(owner, AsyncPredictor.CreatedThread);
                    Assert.AreEqual(owner, AsyncPredictor.InitThread);
                    Assert.AreEqual(ApartmentState.STA, AsyncPredictor.CreatedApartment);
                }
            });
        }

        [TestMethod]
        public Task WordPrediction_SynchronousContractDoesNotRequireAsyncInit()
        {
            return OnStaAsync(async () =>
            {
                using (var manager = CreatePredictorManager(typeof(AsyncPredictor)))
                {
                    AsyncPredictor.Succeed = false;
                    Assert.IsTrue(manager.SetActiveWordPredictor(new CultureInfo("en")));
                    Assert.IsFalse(await manager.SetActiveWordPredictorAsync(new CultureInfo("en")).ConfigureAwait(false));
                    Assert.AreEqual(typeof(NullWordPredictor), manager.ActiveWordPredictor.GetType());
                }
            });
        }

        [TestMethod]
        public Task WordPrediction_LegacyPluginStillWorksWithAsyncSelection()
        {
            return OnStaAsync(async () =>
            {
                using (var manager = CreatePredictorManager(typeof(NullWordPredictor)))
                {
                    Assert.IsTrue(await manager.SetActiveWordPredictorAsync(new CultureInfo("en")).ConfigureAwait(false));
                    Assert.IsNotNull(manager.ActiveWordPredictor);
                }
            });
        }

        [TestMethod]
        public Task WordPrediction_NeutralFailureRetriesEnglishOnSta()
        {
            return OnStaAsync(async () =>
            {
                using (var manager = CreatePredictorManager(typeof(AsyncPredictor)))
                {
                    var collection = (WordPredictors)typeof(WordPredictionManager)
                        .GetField("_wordPredictors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(manager);
                    var cache = (Dictionary<Guid, Tuple<string, Type>>)typeof(WordPredictors)
                        .GetField("_wordPredictorsTypeCache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(collection);
                    cache[ClassDescriptorAttribute.GetDescriptor(typeof(AsyncPredictor)).Id] =
                        Tuple.Create("", typeof(AsyncPredictor));
                    cache.Add(ClassDescriptorAttribute.GetDescriptor(typeof(LegacyPredictor)).Id,
                        Tuple.Create("en", typeof(LegacyPredictor)));
                    int owner = Thread.CurrentThread.ManagedThreadId;
                    AsyncPredictor.Succeed = false;
                    Assert.IsTrue(await manager.SetActiveWordPredictorAsync(new CultureInfo("fr")).ConfigureAwait(false));
                    Assert.AreEqual(typeof(LegacyPredictor), manager.ActiveWordPredictor.GetType());
                    Assert.AreEqual(owner, LegacyPredictor.CreatedThread);
                }
            });
        }

        [TestMethod]
        public Task StartupDispatcher_PostsToOwnerAndPropagatesErrors()
        {
            return OnStaAsync(async () =>
            {
                int owner = Thread.CurrentThread.ManagedThreadId;
                Type dispatcherType = typeof(Context).Assembly.GetType("ACAT.Core.Utility.StartupDispatcher", true);
                object dispatcher = Activator.CreateInstance(dispatcherType, true);
                MethodInfo invoke = Array.Find(dispatcherType.GetMethods(),
                    method => method.Name == "InvokeAsync" &&
                        method.GetParameters()[0].ParameterType.GetGenericArguments()[0].IsGenericParameter);

                int actual = await Task.Run(() => InvokeDispatcher(invoke, dispatcher, () =>
                {
                    Assert.AreEqual(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
                    return Thread.CurrentThread.ManagedThreadId;
                })).ConfigureAwait(false);
                Assert.AreEqual(owner, actual);

                var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                    Task.Run(() => InvokeDispatcher<int>(invoke, dispatcher, () =>
                        throw new InvalidOperationException("dispatch failure")))).ConfigureAwait(false);
                Assert.AreEqual("dispatch failure", exception.Message);
            });
        }

        [TestMethod]
        public Task WordPrediction_LearningUsesCoreOptionalContractAndSyncFallback()
        {
            return OnStaAsync(async () =>
            {
                using (var manager = CreatePredictorManager(typeof(AsyncPredictor)))
                {
                    Assert.IsTrue(manager.SetActiveWordPredictor(new CultureInfo("en")));
                    Assert.IsFalse(await manager.LearnAsync("text", WordPredictorMessageTypes.LearnWords).ConfigureAwait(false));
                }
                using (var manager = CreatePredictorManager(typeof(NullWordPredictor)))
                {
                    Assert.IsTrue(manager.SetActiveWordPredictor(new CultureInfo("en")));
                    Assert.IsTrue(await manager.LearnAsync("text", WordPredictorMessageTypes.LearnWords).ConfigureAwait(false));
                }
            });
        }

        private static Task<T> InvokeDispatcher<T>(MethodInfo method, object dispatcher, Func<T> action)
        {
            return (Task<T>)method.MakeGenericMethod(typeof(T)).Invoke(dispatcher, new object[] { action });
        }

        private static WordPredictionManager CreatePredictorManager(Type predictorType)
        {
            var collection = (WordPredictors)Activator.CreateInstance(typeof(WordPredictors),
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[] { NullLogger<WordPredictors>.Instance, new PreferredWordPredictors() },
                CultureInfo.InvariantCulture);
            var cache = (Dictionary<Guid, Tuple<string, Type>>)typeof(WordPredictors)
                .GetField("_wordPredictorsTypeCache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(collection);
            cache.Add(ClassDescriptorAttribute.GetDescriptor(predictorType).Id, Tuple.Create("en", predictorType));
            var manager = (WordPredictionManager)Activator.CreateInstance(typeof(WordPredictionManager),
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[] { NullLogger<WordPredictionManager>.Instance, null }, CultureInfo.InvariantCulture);
            typeof(WordPredictionManager).GetField("_wordPredictors", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(manager, collection);
            return manager;
        }

        private static Task OnStaAsync(Func<Task> test)
        {
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var thread = new Thread(() =>
            {
                using (var context = new PumpContext())
                {
                    SynchronizationContext.SetSynchronizationContext(context);
                    context.Post(async _ =>
                    {
                        try
                        {
                            await test().ConfigureAwait(false);
                            completion.TrySetResult(true);
                        }
                        catch (Exception exception)
                        {
                            completion.TrySetException(exception);
                        }
                        finally
                        {
                            context.Complete();
                        }
                    }, null);
                    context.Run();
                }
            }) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return completion.Task;
        }

        private sealed class PumpContext : SynchronizationContext, IDisposable
        {
            private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();
            public override void Post(SendOrPostCallback callback, object state) => _queue.Add(() => callback(state));
            public void Run()
            {
                foreach (Action action in _queue.GetConsumingEnumerable())
                {
                    action();
                }
            }
            public void Complete() => _queue.CompleteAdding();
            public void Dispose() => _queue.Dispose();
        }

        private sealed class EmptyServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType) => null;
        }

        [ClassDescriptor("BDA96F2F-4A01-4DC3-AD97-7D31472E94CD", "Async test predictor", "Async startup test")]
        public class AsyncPredictor : NullWordPredictor, IAsyncInitializer<CultureInfo>, IAsyncWordPredictorLearning
        {
            public static bool Succeed;
            public static int CreatedThread;
            public static int InitThread;
            public static ApartmentState CreatedApartment;

            public AsyncPredictor()
            {
                CreatedThread = Thread.CurrentThread.ManagedThreadId;
                CreatedApartment = Thread.CurrentThread.GetApartmentState();
            }

            public async Task<bool> InitAsync(CultureInfo argument)
            {
                InitThread = Thread.CurrentThread.ManagedThreadId;
                await Task.Delay(20).ConfigureAwait(false);
                return Succeed;
            }

            public Task<bool> LearnAsync(string text, WordPredictorMessageTypes requestType)
            {
                return Task.FromResult(false);
            }
        }

        [ClassDescriptor("221D55C4-189F-44D7-B290-994818D4C805", "Legacy test predictor", "Sync startup test")]
        public class LegacyPredictor : NullWordPredictor
        {
            public static int CreatedThread;
            public LegacyPredictor()
            {
                CreatedThread = Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}
