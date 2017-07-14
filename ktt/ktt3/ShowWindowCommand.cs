using System;
using System.Windows;
using System.Windows.Input;

namespace ktt3
{

    /// <summary>
    /// Simple routed command to restore window
    /// </summary>
    public class ShowWindowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            (parameter as Window).WindowState = WindowState.Normal;
            (parameter as Window).Activate();
            //myWindow.TopMost = true; // http://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

    }






}
