using Dapper;
using Interfaz;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static pepeizqs_deals_app.MainWindow;

namespace Modulos
{
	public static class Humble
	{
        private static string html = string.Empty;
		private static int pagina = 0;
		private static int numPaginas = 0;

        public static void Cargar()
		{
			ObjetosVentana.botonHumbleArrancar.Click += ArrancarClick;
			ObjetosVentana.botonHumbleArrancar.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonHumbleArrancar.PointerExited += Animaciones.SaleRatonBoton2;

			ObjetosVentana.wvHumbleAPI.NavigationCompleted += CompletarCarga;
		}

		private static void ArrancarClick(object sender, RoutedEventArgs e)
		{
			PowerManager.MantenerActivo();
			ObjetosVentana.botonHumbleArrancar.IsEnabled = false;
			ObjetosVentana.wvHumbleAPI.Source = new Uri("https://www.humblebundle.com/store/api/search?filter=onsale&sort=discount&request=2&page_size=20&page=0");
		}

		private static async void CompletarCarga(object sender, object e)
		{
			ObjetosVentana.tbHumblePaginas.Text = pagina.ToString() + "/" + numPaginas.ToString();

            WebView2 wv = (WebView2)sender;

			if (wv.Source.AbsoluteUri.Contains("https://www.humblebundle.com/store/api/") == true)
			{
				html = await wv.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");

				if (string.IsNullOrEmpty(html) == false)
				{
					if (html.Contains("{"))
					{
						int int1 = html.IndexOf("{");
						html = html.Remove(0, int1);
					}

					if (html.Contains("}]}") == true)
					{
						int int1 = html.LastIndexOf("}]}");
						html = html.Remove(int1, html.Length - int1);

						html = html + "}]}";
					}

					if (html != "null")
					{
						string temp1 = html;

						int int2 = temp1.IndexOf("num_pages");

						if (int2 > -1)
						{
							string temp2 = temp1.Remove(0, int2);

							int int3 = temp2.IndexOf(":");
							string temp3 = temp2.Remove(0, int3 + 1);

							int int4 = temp3.IndexOf(",");
							string temp4 = temp3.Remove(int4, temp3.Length - int4);

							numPaginas = int.Parse(temp4);
						}
					}
                }

				Random azar = new Random();

				if (numPaginas < 100)
				{
					await Task.Delay(azar.Next(2000, 5000));
				}
				else 
				{
					await Task.Delay(azar.Next(5000, 10000));
				}

				if (pagina < numPaginas)
				{
					if (string.IsNullOrEmpty(html) == false)
					{
						html = html.Replace("'", "");

						if (html == "null")
						{
							pagina -= 1;
						}
						else
						{
							HumbleJuegos juegos = null;

							try
							{
								juegos = JsonSerializer.Deserialize<HumbleJuegos>(System.Text.RegularExpressions.Regex.Unescape(html));
							}
							catch { }

							if (juegos?.Resultados?.Count > 0)
							{
								using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
								{
									conexion.Open();

									if (conexion.State == System.Data.ConnectionState.Open)
									{
										string sqlAñadir = "INSERT INTO temporalhumble " +
													"(contenido, fecha, enlace) VALUES " +
													"(@contenido, @fecha, @enlace) ";

										try
										{
											await conexion.ExecuteAsync(sqlAñadir, new
											{
												contenido = html,
												fecha = DateTime.Now,
												enlace = pagina
											});
										}
										catch
										{

										}
									}
								}
							}

							html = null;
							pagina += 1;
							wv.Source = new Uri("https://www.humblebundle.com/store/api/search?filter=onsale&sort=discount&request=2&page_size=20&page=" + pagina.ToString());
						}
					}
				}
				else
				{
					ObjetosVentana.botonHumbleArrancar.IsEnabled = true;
					PowerManager.Liberar();
				}
			}
        }
	}

	public class HumbleJuegos
	{
		[JsonPropertyName("num_pages")]
		public int Numero { get; set; }

		[JsonPropertyName("results")]
		public List<HumbleJuego> Resultados { get; set; }
	}

	public class HumbleJuego
	{
		[JsonPropertyName("human_name")]
		public string Nombre { get; set; }

		[JsonPropertyName("machine_name")]
		public string Id { get; set; }

		[JsonPropertyName("standard_carousel_image")]
		public string ImagenPequeña { get; set; }

		[JsonPropertyName("large_capsule")]
		public string ImagenGrande { get; set; }

		[JsonPropertyName("current_price")]
		public HumbleJuegoPrecio PrecioRebajado { get; set; }

		[JsonPropertyName("full_price")]
		public HumbleJuegoPrecio PrecioBase { get; set; }

		[JsonPropertyName("human_url")]
		public string Enlace { get; set; }

		[JsonPropertyName("delivery_methods")]
		public List<string> DRMs { get; set; }

		[JsonPropertyName("platforms")]
		public List<string> Sistemas { get; set; }

		[JsonPropertyName("sale_end")]
		public double FechaTermina { get; set; }

		[JsonPropertyName("rewards_split")]
		public double DescuentoChoice { get; set; }

		[JsonPropertyName("incompatible_features")]
		public List<string> CosasIncompatibles { get; set; }
	}

	public class HumbleJuegoPrecio
	{
		[JsonPropertyName("currency")]
		public string Moneda { get; set; }

		[JsonPropertyName("amount")]
		public object Cantidad { get; set; }
	}
}
