using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using TracerLib.Tracer;

namespace TracerTests
{
    [TestFixture]
    public class TracerLibTest
    {
        public ITracer Tracer = new Tracer();

        private readonly List<Thread> _threads = new List<Thread>();

        readonly int ThreadsCount = 5;
        readonly int MethodsCount = 5;

        readonly int MillisecondsTimeout = 100;

        private void Method()
        {
            Tracer.StartTrace();
            Thread.Sleep(MillisecondsTimeout);
            Tracer.StopTrace();
        }
        
        // TIME
        [Test]
        public void ExecutionTimeMoreThreadTimeout()
        {
            Method();
            TraceResult traceResult = Tracer.GetTraceResult();
            
            double methodTime = traceResult.GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodInfo[0].GetTime(); 
            double threadTime = traceResult.GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].ThreadTime;
        
            Assert.IsTrue(methodTime >= MillisecondsTimeout);
            Assert.IsTrue(threadTime >= MillisecondsTimeout);
        }
        
        [Test]
        public void ThreadTimeIsCorrect()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Method();
            Method();
            Method();
            
            var time = stopwatch.ElapsedMilliseconds;
            
            TraceResult traceResult = Tracer.GetTraceResult();
             
            double threadTime = traceResult.GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].ThreadTime;

            bool flag = threadTime + 5 >= time;
            flag |= threadTime - 5 <= time;
            
            Assert.IsTrue(flag);
        }
        
        [Test]
        public void MethodTimeIsCorrect()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Method();
            var time = stopwatch.ElapsedMilliseconds;
            Method();
            Method();
            
            
            TraceResult traceResult = Tracer.GetTraceResult();
             
            var methodTime = traceResult.GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodInfo[0].Time;

            Console.WriteLine(time);
            Console.WriteLine(methodTime);

            bool flag = methodTime + 5 >= time;
            flag |= methodTime - 5 <= time;

            Assert.IsTrue(flag);
        }
        
        // THREADS
        [Test]
        public void ThreadCount()
        {
            for (int i = 0; i < ThreadsCount; i++)
            {
                _threads.Add(new Thread(Method));
            }

            foreach (Thread thread in _threads)
            {
                thread.Start();
                thread.Join();
            }

            TraceResult traceResult = Tracer.GetTraceResult();
            Assert.AreEqual(ThreadsCount, traceResult.GetThreadTraces().Count-1);
        }

        // METHODS
        [Test]
        public void MethodCount()
        {
            for (int i = 0; i < MethodsCount; i++)
            {
                Method();
            }
            TraceResult traceResult = Tracer.GetTraceResult();
            Assert.AreEqual(MethodsCount, Tracer.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodInfo.Count-1);
        }

    }
}
