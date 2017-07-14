using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace ktt3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            var appSetting = ConfigurationManager.AppSettings["mdffiledir"];
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDir, appSetting);
            var fullPath = Path.GetFullPath(path);
            AppDomain.CurrentDomain.SetData("DataDirectory", fullPath);
            base.OnStartup(e);
        }

    }






}
