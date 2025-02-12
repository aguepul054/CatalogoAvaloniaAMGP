using System;
using System.Linq;
using Avalonia.Controls.Chrome;

namespace Catalogo_Avalonia_AMGP.Models;

// Definición de la clase Anime, representando un anime con varios atributos
[Serializable] // Atributo que indica que la clase puede ser serializada (convertida en un formato que se pueda guardar o transmitir)
public class Anime
{
    // Campos privados para los atributos de la clase
    private string _title;
    private string _tipo;
    private bool _isAired; // Indica si el anime ya se ha emitido (true/false)
    private string _genero;
    private byte[] _image; // Imagen representada como un arreglo de bytes

    // Propiedades públicas que permiten acceder y modificar los atributos privados
    public string Title { get { return _title; } set { _title = value; } } // Propiedad para el título del anime
    public string Tipo { get { return _tipo; } set { _tipo = value; } } // Propiedad para el tipo del anime (Ej. "TV", "Película")
    public bool IsAired { get { return _isAired; } set { _isAired = value; } } // Propiedad para saber si el anime se ha emitido
    public string Genero { get { return _genero; } set { _genero = value; } } // Propiedad para el género del anime
    public byte[] Image { get { return _image; } set { _image = value; } } // Propiedad para la imagen del anime (almacenada como byte array)

    // Constructor de la clase Anime que recibe valores para cada uno de los atributos y los asigna.
    public Anime(string title, string tipo, bool isAired, string genero, byte[] image)
    {
        Title = title;     // Asigna el título al campo privado _title
        Tipo = tipo;       // Asigna el tipo al campo privado _tipo
        IsAired = isAired; // Asigna si se ha emitido o no al campo privado _isAired
        Genero = genero;   // Asigna el género al campo privado _genero
        Image = image;     // Asigna la imagen al campo privado _image
    }

    // Sobrescritura del método Equals para comparar dos objetos Anime
    public override bool Equals(object? obj)
    {
        // Comprobamos si el objeto es del tipo Anime
        if (obj is Anime otherAnime)
        {
            // Comparamos los atributos relevantes para determinar si los objetos son iguales
            return _title == otherAnime._title &&
                   _tipo == otherAnime._tipo &&
                   _isAired == otherAnime._isAired &&
                   _genero == otherAnime._genero &&
                   _image.SequenceEqual(otherAnime._image); // Compara los arrays de bytes
        }
        return false;
    }
}
