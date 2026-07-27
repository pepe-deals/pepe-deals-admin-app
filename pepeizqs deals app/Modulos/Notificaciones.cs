using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Windows.Foundation.Metadata;

namespace Herramientas
{
    public static class Notificaciones
    {
        public static async void Ventana(string titulo, string contenido = null, string cerrar = null)
        {
            ContentDialog notificacion = new ContentDialog
            {
                Title = titulo,
                Content = contenido
            };

            if (cerrar == null)
            {
                notificacion.CloseButtonText = cerrar;
            }

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                //notificacion.XamlRoot = ObjetosVentana.ventana.Content.XamlRoot;
            }

            ContentDialogResult resultado = await notificacion.ShowAsync();
        }

        public static void Consola(string titulo)
        {
            Debug.WriteLine(titulo);
        }
    }
}
