using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using ThinkAway.Core;

namespace ThinkAway.Controls.Dwm
{
    public sealed class Thumbnail : SafeHandle
    {
        internal Thumbnail() : base(IntPtr.Zero, true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        protected override bool ReleaseHandle()
        {
            return (Win32API.DwmUnregisterThumbnail(handle) == 0);
        }

        public void Update(Rectangle destination, byte opacity, bool visible, bool onlyClientArea)
        {
            DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
            ptnProperties.dwFlags = DwmThumbnailFlags.SourceClientAreaOnly | DwmThumbnailFlags.Visible | DwmThumbnailFlags.Opacity | DwmThumbnailFlags.RectDestination;
            ptnProperties.rcDestination = new RECT(destination);
            ptnProperties.opacity = opacity;
            ptnProperties.fVisible = visible;
            ptnProperties.fSourceClientAreaOnly = onlyClientArea;
            if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
            {
                throw new DwmCompositionException("DWMThumbnailUpdateFailure");
            }
        }

        public void Update(Rectangle destination, Rectangle source, byte opacity, bool visible, bool onlyClientArea)
        {
            DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
            ptnProperties.dwFlags = DwmThumbnailFlags.SourceClientAreaOnly | DwmThumbnailFlags.Visible | DwmThumbnailFlags.Opacity | DwmThumbnailFlags.RectSource | DwmThumbnailFlags.RectDestination;
            ptnProperties.rcDestination = new RECT(destination);
            ptnProperties.rcSource = new RECT(source);
            ptnProperties.opacity = opacity;
            ptnProperties.fVisible = visible;
            ptnProperties.fSourceClientAreaOnly = onlyClientArea;
            if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
            {
                throw new DwmCompositionException("DWMThumbnailUpdateFailure");
            }
        }

        public Rectangle DestinationRectangle
        {
            set
            {
                DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
                ptnProperties.dwFlags = DwmThumbnailFlags.RectDestination;
                ptnProperties.rcDestination = new RECT(value);
                if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
                {
                    throw new DwmCompositionException("DWMThumbnailUpdateFailure");
                }
            }
        }

        public override bool IsInvalid
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            get
            {
                if (!IsClosed)
                {
                    return (handle == IntPtr.Zero);
                }
                return true;
            }
        }

        public byte Opacity
        {
            set
            {
                DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
                ptnProperties.dwFlags = DwmThumbnailFlags.Opacity;
                ptnProperties.opacity = value;
                if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
                {
                    throw new DwmCompositionException("DWMThumbnailUpdateFailure");
                }
            }
        }

        public bool ShowOnlyClientArea
        {
            set
            {
                DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
                ptnProperties.dwFlags = DwmThumbnailFlags.SourceClientAreaOnly;
                ptnProperties.fSourceClientAreaOnly = value;
                if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
                {
                    throw new DwmCompositionException("DWMThumbnailUpdateFailure");
                }
            }
        }

        public Rectangle SourceRectangle
        {
            set
            {
                DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
                ptnProperties.dwFlags = DwmThumbnailFlags.RectSource;
                ptnProperties.rcSource = new RECT(value);
                if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
                {
                    throw new DwmCompositionException("DWMThumbnailUpdateFailure");
                }
            }
        }

        public Size SourceSize
        {
            get
            {
                DwmSize size;
                if (Win32API.DwmQueryThumbnailSourceSize(this, out size) != 0)
                {
                    throw new DwmCompositionException("DWMThumbnailQueryFailure");
                }
                return size.ToSize();
            }
        }

        public bool Visible
        {
            set
            {
                DwmThumbnailProperties ptnProperties = new DwmThumbnailProperties();
                ptnProperties.dwFlags = DwmThumbnailFlags.Visible;
                ptnProperties.fVisible = value;
                if (Win32API.DwmUpdateThumbnailProperties(this, ref ptnProperties) != 0)
                {
                    throw new DwmCompositionException("DWMThumbnailUpdateFailure");
                }
            }
        }
    }
}

