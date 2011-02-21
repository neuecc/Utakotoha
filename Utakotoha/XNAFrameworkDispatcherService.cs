using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace Utakotoha
{
    /// <see cref="http://msdn.microsoft.com/library/ff842408.aspx"/>
    public class XNAFrameworkDispatcherService : IApplicationService
    {
        DispatcherTimer frameworkDispatcherTimer;

        public XNAFrameworkDispatcherService()
        {
            frameworkDispatcherTimer = new DispatcherTimer();
            frameworkDispatcherTimer.Interval = TimeSpan.FromTicks(333333);
            frameworkDispatcherTimer.Tick += frameworkDispatcherTimer_Tick;
            FrameworkDispatcher.Update();
        }

        void frameworkDispatcherTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }

        void IApplicationService.StartService(ApplicationServiceContext context)
        {
            this.frameworkDispatcherTimer.Start();
        }

        void IApplicationService.StopService()
        {
            this.frameworkDispatcherTimer.Stop();
        }
    }
}
