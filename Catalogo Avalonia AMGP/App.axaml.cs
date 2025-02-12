using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes; 
using Avalonia.Markup.Xaml; 
using Catalogo_Avalonia_AMGP.ViewModel; 

namespace Catalogo_Avalonia_AMGP;

public partial class App : Application
{
    private MainViewModel _viewModel; // Variable privada que almacenará el ViewModel de la aplicación

    // Este método inicializa la aplicación cargando el archivo XAML asociado
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this); // Carga el archivo XAML para la interfaz de usuario
    }

    // Este método se llama después de que la aplicación ha sido inicializada
    // y está lista para ser presentada al usuario.
    public override void OnFrameworkInitializationCompleted()
    {
        // Verifica si la aplicación está utilizando el tipo de vida de la aplicación clásica (es decir, una aplicación de escritorio)
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _viewModel = new MainViewModel(); // Crear una instancia del ViewModel, que contiene la lógica de la aplicación

            // Asigna la ventana principal de la aplicación a la propiedad MainWindow
            desktop.MainWindow = new Views.MainView();
            
            // Establece el DataContext de la ventana principal con el ViewModel,
            // permitiendo que la ventana se vincule a los datos del ViewModel
            desktop.MainWindow.DataContext = _viewModel;

            // Suscribe un evento de cierre de la aplicación para realizar acciones antes de que la aplicación termine
            // En este caso, se guarda la lista antes de que la aplicación se cierre
            desktop.ShutdownRequested += (sender, e) => 
            {
                Console.WriteLine("Guardando lista antes de salir..."); // Mensaje para indicar que la aplicación va a guardar datos
                _viewModel.GuardarListaEnArchivo(); // Llama al método GuardarListaEnArchivo para guardar los datos
            };
        }

        base.OnFrameworkInitializationCompleted(); // Llama a la implementación base para completar la inicialización
    }

}
