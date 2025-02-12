using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Catalogo_Avalonia_AMGP.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Catalogo_Avalonia_AMGP.ViewModel;

public partial class MainViewModel : ObservableObject
{
    // Propiedades observables para el control de la interfaz de usuario (UI)
    [ObservableProperty] private bool _esAnadido;
    [ObservableProperty] private bool _pantallaPrincipal = true;
    [ObservableProperty] private bool _pantallaAnadir;
    [ObservableProperty] private bool _esEnabled;

    // Propiedades para mostrar información de los animes en la pantalla principal
    [ObservableProperty] private string _tbtitulo;
    [ObservableProperty] private string _tbtipo;
    [ObservableProperty] private string _tbestado;
    [ObservableProperty] private string _tbgenero;
    [ObservableProperty] private Bitmap _imagen;
    [ObservableProperty] private bool _botonAtrasEnabled;
    [ObservableProperty] private bool _botonSiguienteEnabled = true;

    // Propiedades para controlar los colores de los bordes (en caso de errores)
    [ObservableProperty] private string _colorBordeTitulo = "White";
    [ObservableProperty] private string _colorBordeGenero = "White";

    // Variables privadas
    private int _numeroImagen; // Número que se asigna a la imagen al añadir un nuevo anime
    private int _indiceActual = 0; // Índice del anime actual mostrado

    // Propiedades para la segunda pantalla (añadir un nuevo anime)
    [ObservableProperty] private string _tbntitulo;
    [ObservableProperty] private int _tbntipo;
    [ObservableProperty] private int _tbnestado;
    [ObservableProperty] private string _tbngenero;

    // Lista que almacena los animes
    private List<Anime> listAnimeCollection;

    // Constructor
    public MainViewModel()
    {
        // Cargar la lista de animes desde un archivo JSON
        listAnimeCollection = LeerListaDesdeArchivo();

        // Si la lista está vacía, precargamos algunos animes de ejemplo
        if (listAnimeCollection.Count == 0)
        {
            PrecargarAnimes();
        }

        // Mostrar el primer anime al iniciar
        MostrarPrimero();

        // Asignar un número único a la próxima imagen
        _numeroImagen = listAnimeCollection.Count + 1;
    }

    // Método para precargar algunos animes de ejemplo
    private void PrecargarAnimes()
    {
        Anime a = new Anime("Blue Lock S2", "TV", false, "Deportes",
            BitMapToByte(new Bitmap("Resources/Images/1.jpg")));
        Anime a1 = new Anime("Dandadan", "TV", false, "Accion", BitMapToByte(new Bitmap("Resources/Images/2.jpg")));
        Anime a2 = new Anime("Blue Box", "TV", false, "Romance", BitMapToByte(new Bitmap("Resources/Images/3.jpg")));
        Anime a3 = new Anime("Vinland Saga", "TV", false, "Accion", BitMapToByte(new Bitmap("Resources/Images/4.jpg")));
        
        // Añadir los animes a la colección
        listAnimeCollection.Add(a);
        listAnimeCollection.Add(a1);
        listAnimeCollection.Add(a2);
        listAnimeCollection.Add(a3);
    }

    // Mostrar el primer anime de la lista
    private void MostrarPrimero()
    {
        _indiceActual = 0; // Índice inicial
        // Asignar los valores del primer anime a las propiedades
        Tbtitulo = listAnimeCollection[0].Title;
        Tbtipo = listAnimeCollection[0].Tipo;
        Tbestado = listAnimeCollection[0].IsAired ? "En emision" : "Finalizado";
        Tbgenero = listAnimeCollection[0].Genero;
        Imagen = ByteToBitMap(listAnimeCollection[0].Image);

        // Habilitar o deshabilitar los botones de navegación
        BotonAtrasEnabled = _indiceActual > 0;
        BotonSiguienteEnabled = _indiceActual < listAnimeCollection.Count - 1;
    }

    // Comando para mostrar el siguiente anime
    [RelayCommand]
    public void MostrarSiguiente()
    {
        if (_indiceActual < listAnimeCollection.Count - 1)
        {
            _indiceActual++; // Avanzar al siguiente anime
            ActualizarAnimeActual(); // Actualizar los datos mostrados
        }
    }

    // Comando para mostrar el anime anterior
    [RelayCommand]
    public void MostrarAnterior()
    {
        if (_indiceActual > 0)
        {
            _indiceActual--; // Retroceder al anime anterior
            ActualizarAnimeActual(); // Actualizar los datos mostrados
        }
    }

    // Método reutilizable para actualizar la información del anime actual
    private void ActualizarAnimeActual()
    {
        Tbtitulo = listAnimeCollection[_indiceActual].Title;
        Tbtipo = listAnimeCollection[_indiceActual].Tipo;
        Tbestado = listAnimeCollection[_indiceActual].IsAired ? "En emision" : "Finalizado";
        Tbgenero = listAnimeCollection[_indiceActual].Genero;
        Imagen = ByteToBitMap(listAnimeCollection[_indiceActual].Image);

        // Actualizar los botones de navegación
        BotonAtrasEnabled = _indiceActual > 0;
        BotonSiguienteEnabled = _indiceActual < listAnimeCollection.Count - 1;
    }

    // Convertir un array de bytes a un Bitmap (imagen)
    private Bitmap ByteToBitMap(byte[] fotoBin)
    {
        Stream st = new MemoryStream(fotoBin);
        Bitmap bitmap = new Bitmap(st);
        return bitmap;
    }

    // Convertir un Bitmap a un array de bytes (para almacenamiento)
    private byte[] BitMapToByte(Bitmap bitmap)
    {
        MemoryStream ms = new MemoryStream();
        bitmap.Save(ms);
        return ms.ToArray();
    }

    // Comando para ir a la pantalla de añadir un nuevo anime
    [RelayCommand]
    public void Anadir()
    {
        PantallaPrincipal = false;
        PantallaAnadir = true;
        string rutaImagen = Path.Combine(AppContext.BaseDirectory, "Resources/Images", "defaultImage.png");
        Console.WriteLine($"Ruta esperada: {rutaImagen}");
        Imagen = new Bitmap(rutaImagen);
    }

    // Comando para borrar el anime actual
    [RelayCommand]
    public void Borrar()
    {
        // Verificar si la lista está vacía o si el índice es inválido
        if (listAnimeCollection.Count == 0 || _indiceActual < 0 || _indiceActual >= listAnimeCollection.Count)
        {
            Console.WriteLine(" No hay elementos para borrar.");
            return;
        }

        // Obtener la ruta de la imagen del anime a borrar
        string rutaImagen = $"Resources/Images/{_indiceActual + 1}.jpg";

        // Eliminar el anime de la lista
        listAnimeCollection.RemoveAt(_indiceActual);

        // Eliminar la imagen del sistema de archivos si existe
        if (File.Exists(rutaImagen))
        {
            File.Delete(rutaImagen);
            Console.WriteLine($" Imagen eliminada: {rutaImagen}");
        }
        else
        {
            Console.WriteLine($" La imagen {rutaImagen} no existe.");
        }

        // Ajustar el índice actual si la lista está vacía
        if (_indiceActual >= listAnimeCollection.Count)
        {
            _indiceActual = listAnimeCollection.Count - 1;
        }

        // Actualizar la UI si aún quedan elementos
        if (listAnimeCollection.Count > 0)
        {
            ActualizarAnimeActual();
        }
        else
        {
            Console.WriteLine(" La lista está vacía.");
            Tbtitulo = "";
            Tbtipo = "";
            Tbestado = "";
            Tbgenero = "";
            Imagen = null;
        }
    }

    // Método para añadir un anime a la lista
    private void AnadirAnime(Anime a)
    {
        RealizarComprobaciones();
        listAnimeCollection.Add(a); // Añadir el anime a la lista
        _indiceActual = listAnimeCollection.Count - 1; // Actualizar el índice al nuevo anime
        ActualizarAnimeActual(); // Mostrar el nuevo anime
    }

    // Método para comprobar que los campos de entrada son válidos
    private bool RealizarComprobaciones()
    {
        if (string.IsNullOrWhiteSpace(Tbntitulo))
        {
            ColorBordeTitulo = "Red"; // Resaltar el borde del título en rojo si está vacío
            ColorBordeGenero = "White";
            return false;
        }

        if (string.IsNullOrEmpty(Tbngenero))
        {
            ColorBordeGenero = "Red"; // Resaltar el borde del género en rojo si está vacío
            ColorBordeTitulo = "White";
            return false;
        }

        return true; // Si no hay errores, retornar verdadero
    }

    // Método que retorna si el estado del anime es "En emisión"
    public bool Estado(string estado)
    {
        return estado == "En emision";
    }

    // Comando para confirmar la adición de un nuevo anime
    [RelayCommand]
    public void Confirmar()
    {
        // Validar que no haya errores en los campos
        if (!RealizarComprobaciones())
        {
            return; // No continuar si hay errores
        }

        // Asignar tipo y estado
        string tipo = Tbntipo == 0 ? "TV" : "Pelicula";
        string estado = Tbnestado == 0 ? "En emision" : "Finalizado";

        // Convertir la imagen a byte[] o asignar una predeterminada
        byte[] imagenBytes = Imagen != null
            ? BitMapToByte(Imagen)
            : BitMapToByte(new Bitmap("Resources/Images/backgroundImagenMenu.jpg"));

        // Crear el nuevo objeto Anime y añadirlo a la lista
        Anime a = new Anime(Tbntitulo, tipo, Estado(estado), Tbngenero, imagenBytes);
        AnadirAnime(a);
        ColorBordeTitulo = "White"; // Resetear los colores de borde
        ColorBordeGenero = "White";
        PantallaPrincipal = true; // Volver a la pantalla principal
        PantallaAnadir = false;
        MostrarPrimero(); // Mostrar el primer anime
        ObtenerNumeroImagenDisponible(); // Obtener el siguiente número de imagen disponible
    }

    // Método asíncrono para añadir una imagen desde el sistema de archivos
    [RelayCommand]
    public async Task AnadirImagen()
    {
        // Obtener la ventana principal de la aplicación
        var window = Avalonia.Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime lifetime
            ? lifetime.MainWindow
            : null;

        if (window == null)
        {
            return; // Si no se puede obtener la ventana, salir del método
        }

        // Crear el cuadro de diálogo para seleccionar una imagen
        var openFileDialog = new OpenFileDialog
        {
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter() { Name = "Imágenes", Extensions = {"jpg", "jpeg" } }
            }
        };

        // Mostrar el cuadro de diálogo para seleccionar el archivo de imagen
        var result = await openFileDialog.ShowAsync(window);

        if (result != null && result.Length > 0)
        {
            string filePath = result[0]; // Obtener la ruta del archivo seleccionado

            // Obtener la extensión del archivo
            string extension = Path.GetExtension(filePath);

            // Obtener el siguiente número disponible para la imagen
            _numeroImagen = ObtenerNumeroImagenDisponible();
            string nuevoNombre = $"{_numeroImagen}{extension}";
            Console.WriteLine(nuevoNombre);

            // Establecer la ruta de destino para guardar la imagen
            string carpetaDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Images");
            string rutaGuardado = Path.Combine(carpetaDestino, nuevoNombre);

            // Crear la carpeta si no existe
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            // Copiar el archivo de la imagen al destino
            File.Copy(filePath, rutaGuardado);

            // Cargar la nueva imagen en el Bitmap para mostrarla en la UI
            Imagen = new Bitmap(rutaGuardado);
        }
    }

    // Comando para cancelar la operación y volver a la pantalla principal
    [RelayCommand]
    public void Cancelar()
    {
        PantallaPrincipal = true;
        PantallaAnadir = false;
        MostrarPrimero();
    }

    // Guardar la lista de animes en un archivo JSON
    public void GuardarListaEnArchivo()
    {
        string ruta = "Resources/animes.json";
        string json = JsonSerializer.Serialize(listAnimeCollection, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ruta, json);
        Console.WriteLine("Lista guardada correctamente.");
    }

    // Leer la lista de animes desde un archivo JSON
    public static List<Anime> LeerListaDesdeArchivo()
    {
        string ruta = "Resources/animes.json";
        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            return JsonSerializer.Deserialize<List<Anime>>(json); // Deserializar el JSON
        }
        else
        {
            Console.WriteLine("El archivo no existe.");
            return new List<Anime>(); // Retornar una lista vacía si el archivo no existe
        }
    }

    // Obtener el siguiente número disponible para una nueva imagen
    private int ObtenerNumeroImagenDisponible()
    {
        int numero = 1;
        while (File.Exists($"Resources/Images/{numero}.jpg"))
        {
            numero++; // Buscar el siguiente número libre
        }
        return numero;
    }
}
