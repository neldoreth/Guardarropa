using System.Globalization;
using System.Threading;
using System.Windows;

namespace Guardarropa;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var cultura = new CultureInfo("es-ES");
        Thread.CurrentThread.CurrentCulture = cultura;
        Thread.CurrentThread.CurrentUICulture = cultura;
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                System.Windows.Markup.XmlLanguage.GetLanguage(cultura.IetfLanguageTag)));

        base.OnStartup(e);
    }

    public static void CambiarTema(bool oscuro)
    {
        var dict = new ResourceDictionary
        {
            Source = new Uri(oscuro
                ? "Assets/TemaOscuro.xaml"
                : "Assets/TemaClaro.xaml", UriKind.Relative)
        };

        var merged = Current.Resources.MergedDictionaries;
        if (merged.Count > 0)
            merged[0] = dict;
        else
            merged.Insert(0, dict);
    }
}
