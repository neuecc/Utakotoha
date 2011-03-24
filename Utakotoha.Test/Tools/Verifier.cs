using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utakotoha
{
    public interface IVerifier
    {
        void Verify(int count);
    }

    public static class Verifier
    {
        private class AnonymousVerifier : IVerifier
        {
            Action<int> verifier;

            public AnonymousVerifier(Action<int> verifier)
            {
                this.verifier = verifier;
            }

            public void Verify(int count)
            {
                verifier(count);
            }
        }

        /// <summary>should not be invoked.</summary>
        public static readonly IVerifier AtZero = new AnonymousVerifier(c => c.Is(0, "Verifier AtZero"));

        /// <summary>should be invoked at once.</summary>
        public static readonly IVerifier AtOnce = new AnonymousVerifier(c => c.Is(1, "Verifier AtOnce"));

        /// <summary>at least once invoked.</summary>
        public static readonly IVerifier AtLeastOnce = new AnonymousVerifier(c => c.Is(i => i >= 1, "Verifier AtLeastOnce"));

        /// <summary>at n count invoked.</summary>
        public static IVerifier AtCount(int count) { return new AnonymousVerifier(c => c.Is(count, "Verifier AtCount")); }

        /// <summary>at most n count invoked.</summary>
        public static IVerifier AtMost(int count) { return new AnonymousVerifier(c => c.Is(i => i <= count, "Verifier AtMost")); }

        /// <summary>at least n count invoked.</summary>
        public static IVerifier AtLeast(int count) { return new AnonymousVerifier(c => c.Is(i => i >= count, "Verifier AtLeast")); }

        /// <summary>verify invoked count when disposed.</summary>
        public static IDisposable Verify<T>(this IObservable<T> source, Action<int> verifier, Action<T> onNext = null)
        {
            return Verify(source, new AnonymousVerifier(verifier), onNext);
        }

        /// <summary>verify invoked count when disposed.</summary>
        public static IDisposable Verify<T>(this IObservable<T> source, IVerifier verifier, Action<T> onNext = null)
        {
            var count = 0;
            return source.Do(_ => count += 1)
                .Finally(() => verifier.Verify(count))
                .Subscribe(onNext ?? (_ => { }));
        }
    }
}