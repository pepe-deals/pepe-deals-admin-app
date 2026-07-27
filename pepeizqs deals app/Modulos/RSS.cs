using FontAwesome6.Fonts;
using Interfaz;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text.Json;
using static pepeizqs_deals_app.MainWindow;

namespace Modulos
{
	public static class RSS
	{
		public static string dominio = "https://pepe.deals";

		public static void Cargar()
		{
			List<Noticia> noticias = new List<Noticia>();

			using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
			{
				conexion.Open();

				if (conexion.State == System.Data.ConnectionState.Open)
				{
					string sqlBuscar = "SELECT TOP(10) * FROM noticias ORDER BY id DESC ";

					using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion))
					{
						using (SqlDataReader lector = comando.ExecuteReader())
						{
							while (lector.Read())
							{
								Noticia noticia = new Noticia();

								noticia.Id = lector.GetInt32(0);
								noticia.Tipo = int.Parse(lector.GetString(1));

								if (lector.IsDBNull(2) == false)
								{
									noticia.Imagen = lector.GetString(2);
								}

								if (lector.IsDBNull(3) == false)
								{
									noticia.Enlace = lector.GetString(3);
								}

								if (lector.IsDBNull(4) == false)
								{
									noticia.Juegos = lector.GetString(4);
								}

								if (lector.IsDBNull(5) == false)
								{
									noticia.FechaEmpieza = lector.GetDateTime(5);
								}

								if (lector.IsDBNull(6) == false)
								{
									noticia.FechaTermina = lector.GetDateTime(6);
								}

								if (lector.IsDBNull(7) == false)
								{
									noticia.BundleTipo = int.Parse(lector.GetString(7));
								}

								if (lector.IsDBNull(8) == false)
								{
									noticia.GratisTipo = int.Parse(lector.GetString(8));
								}

								if (lector.IsDBNull(9) == false)
								{
									noticia.SuscripcionTipo = int.Parse(lector.GetString(9));
								}

								if (lector.IsDBNull(10) == false)
								{
									noticia.TituloEn = lector.GetString(10);
								}

								if (lector.IsDBNull(11) == false)
								{
									noticia.TituloEs = lector.GetString(11);
								}

								if (lector.IsDBNull(12) == false)
								{
									noticia.ContenidoEn = lector.GetString(12);
								}

								if (lector.IsDBNull(13) == false)
								{
									noticia.ContenidoEs = lector.GetString(13);
								}

								if (lector.IsDBNull(18) == false)
								{
									noticia.SteamEn = JsonSerializer.Deserialize<NoticiaSteam>(lector.GetString(18));
								}

								if (lector.IsDBNull(19) == false)
								{
									noticia.SteamEs = JsonSerializer.Deserialize<NoticiaSteam>(lector.GetString(19));
								}

								noticias.Add(noticia);
							}
						}
					}
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
