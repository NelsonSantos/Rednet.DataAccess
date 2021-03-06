﻿#region License

// Copyright (C) Tomáš Pažourek, 2014
// All rights reserved.
// 
// Distributed under MIT license as a part of project Endless.
// https://github.com/tompazourek/Endless

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Endless.Functional
{
    /// <summary>
    /// Functional extensions
    /// </summary>
    public static partial class Function
    {
        /// <summary>
        /// Flips binary function arguments
        /// </summary>
        public static Func<T2, T1, TResult> Flip<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            if (func == null) throw new ArgumentNullException("func");
            return (x, y) => func(y, x);
        }

        /// <summary>
        /// Flips binary action arguments
        /// </summary>
        public static Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            return (x, y) => action(y, x);
        }

        /// <summary>
        /// Applies function to a result, but can be still read from the end (as fluent interfaces do)
        /// </summary>
        public static TOut Pipe<TIn, TOut>(this TIn _this, Func<TIn, TOut> func)
        {
            if (func == null) throw new ArgumentNullException("func");
            return func(_this);
        }
    }
}

namespace Endless
{
    using Endless.Functional;

    /// <summary>
    /// Fold reductions (<see cref="Enumerable.Aggregate{TSource}"/> variations)
    /// </summary>
    public static class AggregateRightExtensions
    {
        /// <summary>
        /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
        /// The elements are combined from the right (starting at the end of the collection).
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
        /// <param name="sequence">Sequence to accumulate over</param>
        /// <param name="end">Seed value</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The final accumulator value.</returns>
        public static TAccumulate AggregateRight<TSource, TAccumulate>(this IEnumerable<TSource> sequence, TAccumulate end, Func<TSource, TAccumulate, TAccumulate> func)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (func == null) throw new ArgumentNullException("func");

            TAccumulate result = sequence.Reverse().Aggregate(end, func.Flip());
            return result;
        }

        /// <summary>
        /// Applies an accumulator function over a sequence. The initial accumulator value is the last element of the collection.
        /// The elements are combined from the right (starting at the end of the collection).
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="sequence">Sequence to accumulate over</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <returns>The final accumulator value.</returns>
        public static TSource AggregateRight<TSource>(this IEnumerable<TSource> sequence, Func<TSource, TSource, TSource> func)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (func == null) throw new ArgumentNullException("func");

            TSource result = sequence.Reverse().Aggregate(func.Flip());
            return result;
        }
    }
}