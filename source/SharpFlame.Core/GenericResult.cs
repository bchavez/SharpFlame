using System;
using System.Collections.Generic;
using NLog;
using SharpFlame.Core.Interfaces;

namespace SharpFlame.Core
{
    public class GenericResult<T> : Result
    {
        public T Value { get; set; }

        public GenericResult(string text, bool log = true) : base(text, log)
        {
        }
            
        public Result ToResult()
        {
            var r = new Result(Text, false);
            r.Take<T>(this);

            return r;
        }
    }
}