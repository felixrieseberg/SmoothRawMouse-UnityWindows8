using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MousePlugin;

namespace Template
{
    static class MouseManagement
    {
        private static CoreCursor defaultCursor;

        private static bool _showCursor;
        public static bool showCursor
        {
            get { return _showCursor; }
            set
            {
                if (value == false)
                {
                    Window.Current.CoreWindow.PointerCursor = null;
                    _showCursor = false;
                }
                else if (value == true && defaultCursor != null)
                {
                    Window.Current.CoreWindow.PointerCursor = defaultCursor;
                    _showCursor = true;
                }
            }
        }

        public static void Load() {
            if (defaultCursor == null && Window.Current.CoreWindow.PointerCursor != null) {
                defaultCursor = Window.Current.CoreWindow.PointerCursor;
            }

            Windows.Devices.Input.MouseDevice.GetForCurrentView().MouseMoved += MouseMoved;
        }


        public static void MouseMoved(Windows.Devices.Input.MouseDevice sender, Windows.Devices.Input.MouseEventArgs args)
        {
            global::MouseLooking.windowsMouseMoveX += 0.1f * args.MouseDelta.X;
            global::MouseLooking.windowsMouseMoveY += 0.1f * args.MouseDelta.Y;
        }

    }
}
