using System.Timers;
using System.Collections;
using System.Diagnostics;

namespace ThinkAway.Net.Monitor
{
	/// <summary>
	/// The NetworkMonitor class monitors network speed for each network adapter on the computer,
	/// using classes for Performance counter in .NET library.
	/// </summary>
	public class NetworkMonitor
	{
		private readonly Timer _timer;						// The timer event executes every second to refresh the values in adapters.
		private readonly ArrayList _adapters;					// The list of adapters on the computer.
		private readonly ArrayList _monitoredAdapters;		// The list of currently monitored adapters.

		public NetworkMonitor()
		{
			this._adapters	=	new ArrayList();
			this._monitoredAdapters	=	new ArrayList();
			EnumerateNetworkAdapters();
			
			_timer	=	new Timer(1000);
			_timer.Elapsed	+=	timer_Elapsed;
		}

		/// <summary>
		/// Enumerates network adapters installed on the computer.
		/// </summary>
		private void EnumerateNetworkAdapters()
		{
			PerformanceCounterCategory category	=	new PerformanceCounterCategory("Network Interface");

			foreach (string name in category.GetInstanceNames())
			{
				// This one exists on every computer.
				if (name == "MS TCP Loopback interface")
					continue;
				// Create an instance of NetworkAdapter class, and create performance counters for it.
				NetworkAdapter adapter	=	new NetworkAdapter(name);
				adapter.DlCounter	=	new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
				adapter.UlCounter	=	new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
				this._adapters.Add(adapter);			// Add it to ArrayList adapter
			}
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (NetworkAdapter adapter in this._monitoredAdapters)
				adapter.Refresh();
		}

		/// <summary>
		/// Get instances of NetworkAdapter for installed adapters on this computer.
		/// </summary>
		public NetworkAdapter[] Adapters
		{
			get
			{
				return (NetworkAdapter[])this._adapters.ToArray(typeof(NetworkAdapter));
			}
		}

		// Enable the timer and add all adapters to the monitoredAdapters list, unless the adapters list is empty.
		public void StartMonitoring()
		{
			if (this._adapters.Count > 0)
			{
				foreach(NetworkAdapter adapter in this._adapters)
					if (!this._monitoredAdapters.Contains(adapter))
					{
						this._monitoredAdapters.Add(adapter);
						adapter.Init();
					}
				
				_timer.Enabled	=	true;
			}
		}

		// Enable the timer, and add the specified adapter to the monitoredAdapters list
		public void StartMonitoring(NetworkAdapter adapter)
		{
			if (!this._monitoredAdapters.Contains(adapter))
			{
				this._monitoredAdapters.Add(adapter);
				adapter.Init();
			}
			_timer.Enabled	=	true;
		}

		// Disable the timer, and clear the monitoredAdapters list.
		public void StopMonitoring()
		{
			this._monitoredAdapters.Clear();
			_timer.Enabled	=	false;
		}

		// Remove the specified adapter from the monitoredAdapters list, and disable the timer if the monitoredAdapters list is empty.
		public void StopMonitoring(NetworkAdapter adapter)
		{
			if (this._monitoredAdapters.Contains(adapter))
				this._monitoredAdapters.Remove(adapter);	
			if(this._monitoredAdapters.Count == 0)
				_timer.Enabled	=	false;
		}
	}
}
