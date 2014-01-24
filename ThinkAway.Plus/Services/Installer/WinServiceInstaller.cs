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

using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using ThinkAway.Plus.Services.Service;

namespace ThinkAway.Plus.Services.Installer
{
    internal static class WinServiceInstaller
    {
        public static void Install(ServiceInfo serviceInfo)
        {
            Install(true,serviceInfo);
        }

        public static void UnInstall(ServiceInfo serviceInfo)
        {
            Install(false, serviceInfo);
        }

        private static System.Configuration.Install.Installer CreateInstaller(ServiceInfo serviceInfo)
        {
            System.Configuration.Install.Installer installer = new System.Configuration.Install.Installer();

            ServiceInstaller serviceInstaller = new ServiceInstaller();
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();

            serviceInstaller.Description = serviceInfo.Description;
            serviceInstaller.StartType = serviceInfo.ServiceStartMode;
            serviceInstaller.DisplayName = serviceInfo.DisplayName;
            serviceInstaller.ServiceName = serviceInfo.ServiceName;

            if (serviceInfo.DependsOn != null && serviceInfo.DependsOn.Length > 0)
                serviceInstaller.ServicesDependedOn = serviceInfo.DependsOn;

            serviceProcessInstaller.Account = serviceInfo.ServiceAccount;
            serviceProcessInstaller.Username = serviceInfo.UserName;
            serviceProcessInstaller.Password = serviceInfo.Password;

            installer.Installers.Add(serviceProcessInstaller);
            installer.Installers.Add(serviceInstaller);

            return installer;
        }

        private static void Install(bool install, ServiceInfo serviceInfo)
        {
            using (TransactedInstaller transactedInstaller = new TransactedInstaller())
            {
                using (System.Configuration.Install.Installer installer = CreateInstaller(serviceInfo))
                {
                    transactedInstaller.Installers.Add(installer);

                    string path = string.Format("/assemblypath={0}", Assembly.GetEntryAssembly().Location);

                    transactedInstaller.Context = new InstallContext("", new[] { path });

                    if (install)
                        transactedInstaller.Install(new Hashtable());
                    else
                        transactedInstaller.Uninstall(null);
                }
            }

        }
    }
}
