using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Utakotoha.Model.View
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            UtakotohaLink.ClickAsObservable().Subscribe(e =>
            {
                var task = new WebBrowserTask() { URL = (e.Sender as HyperlinkButton).Content.ToString() };
                task.Show();
            });
        }

    }
}