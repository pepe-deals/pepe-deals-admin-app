using Dapper;
using FontAwesome6.Fonts;
using Interfaz;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static pepeizqs_deals_app.MainWindow;

namespace Modulos
{
	public static class RSS
	{
		public static string dominio = "https://pepe.deals";

		public static async Task Cargar()
		{
			List<Noticia> noticias = new List<Noticia>();

			using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
			{
				string sqlBuscar = "SELECT TOP(10) * FROM noticias ORDER BY id DESC";

				var filas = await conexion.QueryAsync(sqlBuscar);

				foreach (var fila in filas)
				{
					var valores = ((IDictionary<string, object>)fila).Values.ToList();

					Noticia noticia = new Noticia
					{
						Id = (int)valores[0],
						Tipo = int.Parse((string)valores[1])
					};

					if (valores[2] != null) noticia.Imagen = (string)valores[2];
					if (valores[3] != null) noticia.Enlace = (string)valores[3];
					if (valores[4] != null) noticia.Juegos = (string)valores[4];
					if (valores[5] != null) noticia.FechaEmpieza = (DateTime)valores[5];
					if (valores[6] != null) noticia.FechaTermina = (DateTime)valores[6];
					if (valores[7] != null) noticia.BundleTipo = int.Parse((string)valores[7]);
					if (valores[8] != null) noticia.GratisTipo = int.Parse((string)valores[8]);
					if (valores[9] != null) noticia.SuscripcionTipo = int.Parse((string)valores[9]);
					if (valores[10] != null) noticia.TituloEn = (string)valores[10];
					if (valores[11] != null) noticia.TituloEs = (string)valores[11];
					if (valores[12] != null) noticia.ContenidoEn = (string)valores[12];
					if (valores[13] != null) noticia.ContenidoEs = (string)valores[13];
					if (valores[18] != null) noticia.SteamEn = JsonSerializer.Deserialize<NoticiaSteam>((string)valores[18]);
					if (valores[19] != null) noticia.SteamEs = JsonSerializer.Deserialize<NoticiaSteam>((string)valores[19]);

					noticias.Add(noticia);
				}
			}

			if (noticias?.Count > 0)
			{
				ObjetosVentana.spRSSNoticias.Visibility = Visibility.Visible;
				ObjetosVentana.spRSSNoticias.Children.Clear();

				foreach (var noticia in noticias)
				{
					Grid grid = new Grid
					{
						Padding = new Thickness(20),
						BorderThickness = new Thickness(0, 0, 0, 1)
					};

					ColumnDefinition col1 = new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star)
					};

					grid.ColumnDefinitions.Add(col1);

					ColumnDefinition col2 = new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Auto)
					};

					grid.ColumnDefinitions.Add(col2);

					TextBlock tb = new TextBlock();
					tb.Text = noticia.TituloEn;

					tb.SetValue(Grid.ColumnProperty, 0);
					grid.Children.Add(tb);

					//--------------------------------------------

					StackPanel sp = new StackPanel
					{
						Orientation = Orientation.Horizontal
					};

					#region Todo
					TextBlock tbTodo = new TextBlock
					{
						Text = "Todo",
						Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["ColorFuente"])
					};

					Button2 botonTodo = new Button2
					{
						Tag = noticia,
						Content = tbTodo,
						Margin = new Thickness(20, 0, 0, 0),
						Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["ColorPrimario"])
					};

					botonTodo.Click += ClickTodo;
					botonTodo.PointerEntered += Animaciones.EntraRatonBoton2;
					botonTodo.PointerExited += Animaciones.SaleRatonBoton2;

					sp.Children.Add(botonTodo);
					#endregion

					#region Steam
					FontAwesome iconoSteam = new FontAwesome
					{
						Icon = FontAwesome6.EFontAwesomeIcon.Brands_Steam,
						Foreground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["ColorFuente"])
					};

					Button2 botonSteam = new Button2
					{
						Tag = noticia,
						Content = iconoSteam,
						Margin = new Thickness(20, 0, 0, 0),
						Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["ColorPrimario"])
					};

					botonSteam.Click += ClickSteam;
					botonSteam.PointerEntered += Animaciones.EntraRatonBoton2;
					botonSteam.PointerExited += Animaciones.SaleRatonBoton2;

					sp.Children.Add(botonSteam);
					#endregion

					sp.SetValue(Grid.ColumnProperty, 1);
					grid.Children.Add(sp);

					ObjetosVentana.spRSSNoticias.Children.Add(grid);
				}
			}
		}

		public static void ClickTodo(object sender, RoutedEventArgs e)
		{
			Button boton = (Button)sender;
			Noticia noticia = (Noticia)boton.Tag;
		
			try
			{
				RedesSociales.Steam.Enviar(noticia);
			}
			catch 
			{
				
			}
			
			Pestañas.Visibilidad(ObjetosVentana.gridSteam, true, null, false);
		}

		public static void ClickSteam(object sender, RoutedEventArgs e)
		{
			Button boton = (Button)sender;
			Noticia noticia = (Noticia)boton.Tag;

			RedesSociales.Steam.Enviar(noticia);
			Pestañas.Visibilidad(ObjetosVentana.gridSteam, true, null, false);
		}
	}

    public class Noticia
    {
        public int Id { get; set; }
		public int Tipo { get; set; }
		public int BundleTipo { get; set; }
		public int GratisTipo { get; set; }
		public int SuscripcionTipo { get; set; }
		public string TituloEn { get; set; }
		public string TituloEs { get; set; }
		public string ContenidoEn { get; set; }
		public string ContenidoEs { get; set; }
		public string Imagen { get; set; }
		public string Enlace { get; set; }
		public string Juegos { get; set; }
		public DateTime FechaEmpieza { get; set; }
		public DateTime FechaTermina { get; set; }
		public int BundleId { get; set; }
		public NoticiaSteam SteamEn { get; set; }
		public NoticiaSteam SteamEs { get; set; }
	}

	public class NoticiaSteam
	{
		public string Titulo { get; set; }
		public string Subtitulo { get; set; }
		public string Contenido { get; set; }
	}
}
