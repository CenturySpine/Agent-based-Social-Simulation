using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SocialSimulation.Core
{
    static class UiRenderer
    {
        public static void Render(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }
    }
}
