using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class ObservableVerifyExtensions
    {
        /// <summary>verify called count when disposed. first argument is called count.</summary>
        public static IObservable<T> Verify<T>(this IObservable<T> source, Expression<Func<int, bool>> verify)
        {
            var count = 0;
            return source
                .Do(_ => count += 1)
                .Finally(() =>
                {
                    var msg = verify.Parameters.First().Name + " = " + count + " => " + verify.Body;
                    Assert.IsTrue(verify.Compile().Invoke(count), "Verifier " + msg);
                });
        }

        /// <summary>verify called count when disposed. first argument is called count.</summary>
        public static IDisposable VerifyAll<T>(this IObservable<T> source, Expression<Func<int, bool>> verify, Action<T> onNext = null)
        {
            return source.Verify(verify).Subscribe(onNext ?? (_ => { }));
        }

        /// <summary>verify not called when disposed.</summary>
        public static IDisposable VerifyZero<T>(this IObservable<T> source)
        {
            return source.VerifyAll(i => i == 0);
        }

        /// <summary>verify called once when disposed.</summary>
        public static IDisposable VerifyOnce<T>(this IObservable<T> source, Action<T> onNext = null)
        {
            return source.VerifyAll(i => i == 1);
        }
    }
}