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
using System.Collections.Generic;
using System.ServiceProcess;
using ThinkAway.Plus.Services.Event;
using ThinkAway.Plus.Services.Service;
using ThinkAway.Plus.Services.Installer;
using ThinkAway.Plus.Services.Tasks;

namespace ThinkAway.Plus.Services.Service
{
    public class Service
    {
        public event EventHandler<ServiceStateEventArgs> StateChanged;

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;
        public event EventHandler<ServiceTaskEventArgs> TaskStarting;

        public event EventHandler<ServiceTaskEventArgs> TaskStarted;
        public event EventHandler<ServiceTaskEventArgs> TaskStopping;
        public event EventHandler<ServiceTaskEventArgs> TaskStopped;

        public event EventHandler<ServiceTaskExceptionEventArgs> ServiceTaskExceptionOccurred;

        private void InvokeStateChanged(ServiceTask serviceTask, ServiceState state)
        {
            if (StateChanged != null) 
                StateChanged(this, new ServiceStateEventArgs(serviceTask, state));

            OnStateChanged(serviceTask, state);
        }

        private void InvokeStarting()
        {
            if (Starting != null) 
                Starting(this, EventArgs.Empty);

            OnStarting();

            InvokeStateChanged(null,ServiceState.Starting);
        }

        private void InvokeStarted()
        {
            if (Started != null) 
                Started(this, EventArgs.Empty);

            OnStarted();

            InvokeStateChanged(null, ServiceState.Started);
        }

        private void InvokeStopping()
        {
            if (Stopping != null) 
                Stopping(this, EventArgs.Empty);

            OnStopping();

            InvokeStateChanged(null, ServiceState.Stopping);
        }

        private void InvokeStopped()
        {
            if (Stopped != null) 
                Stopped(this, EventArgs.Empty);

            OnStopped();

            InvokeStateChanged(null, ServiceState.Stopped);
        }

        private void InvokeTaskStarting(ServiceTask serviceTask)
        {
            if (TaskStarting != null)
                TaskStarting(this, new ServiceTaskEventArgs(serviceTask));

            OnTaskStarting(serviceTask);

            InvokeStateChanged(serviceTask, ServiceState.Starting);
        }

        private void InvokeTaskStarted(ServiceTask serviceTask)
        {
            if (TaskStarted != null)
                TaskStarted(this, new ServiceTaskEventArgs(serviceTask));

            OnTaskStarted(serviceTask);

            InvokeStateChanged(serviceTask, ServiceState.Started);
        }

        private void InvokeTaskStopping(ServiceTask serviceTask)
        {
            if (TaskStopping != null)
                TaskStopping(this, new ServiceTaskEventArgs(serviceTask));

            OnTaskStopping(serviceTask);

            InvokeStateChanged(serviceTask, ServiceState.Stopping);
        }

        private void InvokeTaskStopped(ServiceTask serviceTask)
        {
            if (TaskStopped != null)
                TaskStopped(this, new ServiceTaskEventArgs(serviceTask));

            OnTaskStopped(serviceTask);

            InvokeStateChanged(serviceTask, ServiceState.Stopped);
        }

        public Service(string serviceName)
        {
            ServiceTasks = new List<ServiceTask>();
            ServiceInfo = new ServiceInfo(serviceName);
        }

        public Service(ServiceInfo serviceInfo)
        {
            ServiceTasks = new List<ServiceTask>();
            ServiceInfo = serviceInfo;
        }

        public List<ServiceTask> ServiceTasks { get; private set; }
        public ServiceInfo ServiceInfo { get; private set; }

        protected virtual void OnStateChanged(ServiceTask serviceTask, ServiceState serviceState)
        {
        }

        protected virtual void OnTaskException(ServiceTask serviceTask, Exception e)
        {
        }

        protected virtual void OnStarting()
        {
        }

        protected virtual void OnStarted()
        {
        }

        protected virtual void OnStopping()
        {
        }

        protected virtual void OnStopped()
        {
        }

        protected virtual void OnTaskStarting(ServiceTask serviceTask)
        {
        }

        protected virtual void OnTaskStarted(ServiceTask serviceTask)
        {
        }

        protected virtual void OnTaskStopping(ServiceTask serviceTask)
        {
        }

        protected virtual void OnTaskStopped(ServiceTask serviceTask)
        {
        }

        public void Install()
        {
            WinServiceInstaller.Install(ServiceInfo);
        }

        public void UnInstall()
        {
            WinServiceInstaller.UnInstall(ServiceInfo);
        }

        public void RunConsole()
        {
            StartServiceTasks();

            Console.Read();

            StopServiceTasks();
        }

        public void Run()
        {
            ServiceBase.Run(new InternalService(this));
        }

        internal void StartServiceTasks()
        {
            object syncObject = new object();

            InvokeStarting();

            foreach (ServiceTask serviceTask in ServiceTasks)
            {
                InvokeTaskStarting(serviceTask);

                serviceTask.ExceptionOccurred += ServiceTask_ExceptionOccurred;

                serviceTask.Start(syncObject);

                InvokeTaskStarted(serviceTask);
            }

            InvokeStarted();
        }

        private void ServiceTask_ExceptionOccurred(object sender, ServiceTaskExceptionEventArgs e)
        {
            OnTaskException(e.ServiceTask, e.Exception);

            if (ServiceTaskExceptionOccurred != null)
                ServiceTaskExceptionOccurred(sender, e);
        }

        internal void StopServiceTasks()
        {
            InvokeStopping();

            foreach (ServiceTask serviceTask in ServiceTasks)
            {
                InvokeTaskStopping(serviceTask);

                serviceTask.Stop();
            }

            foreach (ServiceTask serviceTask in ServiceTasks)
            {
                serviceTask.WaitUntilFinished();

                serviceTask.ExceptionOccurred -= ServiceTask_ExceptionOccurred;

                InvokeTaskStopped(serviceTask);
            }

            InvokeStopped();
        }
    }
}