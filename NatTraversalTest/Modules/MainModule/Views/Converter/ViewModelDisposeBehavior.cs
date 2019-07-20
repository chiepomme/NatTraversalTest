using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NatTraversalTest.MainModule.Views
{
    public class ViewModelDisposeBehavior : Behavior<UserControl>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Dispatcher.ShutdownStarted += OnShutdownStarted;
        }

        void OnShutdownStarted(object sender, EventArgs e)
        {
            AssociatedObject.Dispatcher.ShutdownStarted -= OnShutdownStarted;
            (AssociatedObject.DataContext as IDisposable)?.Dispose();
        }
    }
}
