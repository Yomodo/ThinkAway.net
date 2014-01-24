#region License
//=============================================================================
// Vici WinService - .NET Windows Service library 
//
// Copyright (c) 2009 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Threading;
using ThinkAway.Plus.Services.Event;

namespace ThinkAway.Plus.Services.Tasks
{
    public abstract class ServiceTask
    {
        public event EventHandler<ServiceTaskExceptionEventArgs> ExceptionOccurred;

        private Thread _thread;
        private bool _stopRequested;

        private bool _synchronous;
        private object _syncObject;

        public virtual bool Synchronous
        {
            get { return _synchronous; }
            set { _synchronous = value; }
        }

        public bool IsRunning { get; private set; }

        public virtual bool StopRequested
        {
            get { return _stopRequested; }
            set { _stopRequested = value; }
        }

        protected abstract void RunTask();

        protected void OnException(Exception e)
        {
        }

        protected ServiceTask(bool synchronous)
        {
            _synchronous = synchronous;
        }

        public void Start(object syncObject)
        {
            _syncObject = syncObject;

            _thread = new Thread(Run);

            _thread.IsBackground = true;

            StopRequested = false;

            _thread.Start();
        }

        public void Stop()
        {
            StopRequested = true;
        }

        public void WaitUntilFinished()
        {
            _thread.Join(5000);
        }

        protected virtual bool WaitForNextRun()
        {
            while (!StopRequested)
                Thread.Sleep(100);

            return false;
        }

        private void Run()
        {
            IsRunning = true;

            while (!StopRequested)
            {
                if (!WaitForNextRun())
                    continue;

                try
                {
                    if (Synchronous)
                    {
                        lock (_syncObject)
                        {
                            RunTask();
                        }
                    }
                    else
                    {
                        RunTask();
                    }
                }
                catch (Exception e)
                {
                    OnException(e);

                    if (ExceptionOccurred != null)
                        ExceptionOccurred(this,new ServiceTaskExceptionEventArgs(this,e));
                }
            }

            IsRunning = false;
        }
    }
}
