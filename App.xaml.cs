using Autofac;
using charposition.Services;
using System;
using System.Data;
using System.Windows;

namespace charposition;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Default: dummy data
        string text = DummyData.Text;
        string title = DummyData.Title;
        string semantics = DummyData.Semantics;
        string error = string.Empty;

        // Services
        ContainerBuilder services = new();
        services.RegisterType<FileReader>().As<IFileReader>();
        services.RegisterType<LineSplitter>().As<ILineSplitter>();
        services.RegisterType<MainWindowModel>().SingleInstance();
        services.RegisterType<MainWindow>();

        var container = services.Build();

        var model = container.Resolve<MainWindowModel>();

        // read args
        var fileReader = container.Resolve<IFileReader>();
        if (e.Args.Length > 0)
        {
            text = string.Empty;
            semantics = string.Empty;
            try
            {
                title = e.Args[0];
                text = fileReader.ReadAllText(e.Args[0]);
                semantics = e.Args.Length == 1 ? string.Empty : fileReader.ReadAllText(e.Args[1]);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        model.LoadData(text, semantics);

        if (error != string.Empty)
        {
            model.ErrorMessage = error;
        }

        var window = container.Resolve<MainWindow>();
        window.Title = title;
        window.Show();
    }
}
