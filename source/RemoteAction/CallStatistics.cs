﻿using System.Diagnostics;
using Common;
using RailgunNet.System.Types;

namespace RemoteAction
{
    public struct CallTrace
    {
        public FieldChange? Change { get; set; }
        public MethodCall? Call { get; set; }
        public Tick Tick { get; set; }
    }

    public class CallStatistics : DropoutStack<CallTrace>
    {
        public CallStatistics(int capacity) : base(capacity)
        {
        }

        [Conditional("DEBUG")]
        public void Push(MethodCall call, Tick tick)
        {
            Push(new CallTrace
            {
                Call = call,
                Tick = tick
            });
        }

        [Conditional("DEBUG")]
        public void Push(FieldChange change, Tick tick)
        {
            Push(new CallTrace
            {
                Change = change,
                Tick = tick
            });
        }
    }
}