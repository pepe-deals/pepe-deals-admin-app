using Dapper;
using Herramientas;
using Interfaz;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static pepeizqs_deals_app.MainWindow;

namespace Modulos
{
	public static class Scraping
	{
		private static string html = string.Empty;

		public static void Cargar()
		{
			ObjetosVentana.botonAmazonLunaArrancar.Click += ArrancarClick;
			ObjetosVentana.botonAmazonLunaArrancar.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonAmazonLunaArrancar.PointerExited += Animaciones.SaleRatonBoton2;

			ObjetosVentana.wvAmazonLuna.NavigationCompleted += CompletarCarga;
		}

		private static void ArrancarClick(object sender, RoutedEventArgs e)
		{
			ObjetosVentana.botonAmazonLunaArrancar.IsEnabled = false;
			ObjetosVentana.wvAmazonLuna.Source = new Uri("https://luna.amazon.es/subscription/luna-standard");
		}

		private static async void CompletarCarga(object sender, object e)
		{
			WebView2 wv = (WebView2)sender;

			if (wv.Source.AbsoluteUri.Contains("https://luna.amazon.es/subscription/luna-standard") == true)
			{
				await Task.Delay(5000);

				html = await wv.ExecuteScriptAsync("document.documentElement.outerHTML");

				if (string.IsNullOrEmpty(html) == false)
				{
					html = Regex.Unescape(html);

					if (html.Contains(Strings.ChrW(34) + "collection_browse_channel_games_impression" + Strings.ChrW(34)) == true)
					{
						html = html.Substring(html.IndexOf(Strings.ChrW(34) + "collection_browse_channel_games_impression" + Strings.ChrW(34)));
					}

					List<AmazonLunaJuego> juegos = new List<AmazonLunaJuego>();

					int i = 0;
					while (i < 1000)
					{
						if (html.Contains("id=" + Strings.ChrW(34) + "game_tile_amzn1") == true)
						{
							int int1 = html.IndexOf("id=" + Strings.ChrW(34) + "game_tile_amzn1");
							string temp1 = html.Remove(0, int1 + 4);

							html = html.Remove(0, int1 + 4);

							int int2 = temp1.IndexOf(Strings.ChrW(34));
							string temp2 = temp1.Remove(int2, temp1.Length - int2);

							if (temp2.Contains("_impression") == false)
							{
								temp2 = temp2.Replace("game_tile_", null);

								int int3 = temp1.IndexOf("title=" + Strings.ChrW(34));
								string temp3 = temp1.Remove(0, int3 + 7);

								int int4 = temp3.IndexOf(Strings.ChrW(34));
								string temp4 = temp3.Remove(int4, temp3.Length - int4);

								AmazonLunaJuego juego = new AmazonLunaJuego
								{
									Id = temp2,
									Nombre = temp4
								};

								if (string.IsNullOrEmpty(juego.Id) == false)
								{
									juego.Id = juego.Id.Replace("_imageUrl", null);
								}

								bool añadir = true;

								if (juegos?.Count > 0)
								{
									foreach (AmazonLunaJuego juego2 in juegos)
									{
										if (juego2.Id == juego.Id)
										{
											añadir = false;
											break;
										}
									}
								}

								if (juego.Id.Contains("_test_id") == true)
								{
									añadir = false;
								}

								if (añadir == true)
								{
									juegos.Add(juego);
								}
							}
						}

						i += 1;
					}

					using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
					{
						try
						{
							var parametros = new
							{
								contenido = JsonSerializer.Serialize(juegos),
								fecha = DateTime.Now,
								enlace = "1"
							};

							conexion.Execute(
								"INSERT INTO temporallunastandardjson (contenido, fecha, enlace) VALUES (@contenido, @fecha, @enlace)",
								parametros
							);
						}
						catch (Exception ex)
						{
							
						}
					}

					ObjetosVentana.tbAmazonLuna.Text = ObjetosVentana.tbAmazonLuna.Text + juegos.Count.ToString() + " juegos (Luna standard) ";
				}

				await Task.Delay(5000);

				wv.Source = new Uri("https://luna.amazon.es/subscription/luna-premium/B085TRCCT6");
			}
			else if (wv.Source.AbsoluteUri.Contains("https://luna.amazon.es/subscription/luna-premium/B085TRCCT6") == true)
			{
				await Task.Delay(5000);

				html = await wv.ExecuteScriptAsync("document.documentElement.outerHTML");

				if (string.IsNullOrEmpty(html) == false)
				{
					html = Regex.Unescape(html);

					if (html.Contains(Strings.ChrW(34) + "collection_browse_channel_games_impression" + Strings.ChrW(34)) == true)
					{
						html = html.Substring(html.IndexOf(Strings.ChrW(34) + "collection_browse_channel_games_impression" + Strings.ChrW(34)));
					}

					List<AmazonLunaJuego> juegos = new List<AmazonLunaJuego>();

					int i = 0;
					while (i < 1000)
					{
						if (html.Contains("id=" + Strings.ChrW(34) + "game_tile_amzn1.adg.product.") == true)
						{
							int int1 = html.IndexOf("id=" + Strings.ChrW(34) + "game_tile_amzn1.adg.product.");
							string temp1 = html.Remove(0, int1 + 4);

							html = html.Remove(0, int1 + 4);

							int int2 = temp1.IndexOf(Strings.ChrW(34));
							string temp2 = temp1.Remove(int2, temp1.Length - int2);

							if (temp2.Contains("_impression") == false)
							{
								temp2 = temp2.Replace("game_tile_", null);

								int int3 = temp1.IndexOf("title=" + Strings.ChrW(34));
								string temp3 = temp1.Remove(0, int3 + 7);

								int int4 = temp3.IndexOf(Strings.ChrW(34));
								string temp4 = temp3.Remove(int4, temp3.Length - int4);

								AmazonLunaJuego juego = new AmazonLunaJuego
								{
									Id = temp2,
									Nombre = temp4
								};

								if (string.IsNullOrEmpty(juego.Id) == false)
								{
									juego.Id = juego.Id.Replace("_imageUrl", null);
								}

								bool añadir = true;

								if (juegos?.Count > 0)
								{
									foreach (AmazonLunaJuego juego2 in juegos)
									{
										if (juego2.Id == juego.Id)
										{
											añadir = false;
											break;
										}
									}
								}

								if (añadir == true)
								{
									juegos.Add(juego);
								}
							}
						}

						i += 1;
					}

					using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
					{
						try
						{
							conexion.Execute(
								"INSERT INTO temporallunapremiumjson (contenido, fecha, enlace) VALUES (@contenido, @fecha, @enlace)",
								new
								{
									contenido = JsonSerializer.Serialize(juegos),
									fecha = DateTime.Now,
									enlace = "1"
								}
							);
						}
						catch
						{
							// Sin notificación de error
						}
					}

					ObjetosVentana.tbAmazonLuna.Text = ObjetosVentana.tbAmazonLuna.Text + juegos.Count.ToString() + " juegos (Luna premium) ";
				}

				await Task.Delay(5000);

				wv.Source = new Uri("https://www.indiepass.com");
			}
			else if (wv.Source.AbsoluteUri.Contains("https://www.indiepass.com") == true)
			{
				await Task.Delay(5000);

				html = await wv.ExecuteScriptAsync("document.documentElement.outerHTML");

				if (string.IsNullOrEmpty(html) == false)
				{
					html = Regex.Unescape(html);

					string key = "\\\"games\\\"";

					int primero = html.IndexOf(key);

					if (primero == -1)
					{
						return;
					}

					int segundo = html.IndexOf(key, primero + key.Length);

					if (segundo == -1)
					{
						return;
					}

					int arranque = html.IndexOf('[', segundo);

					if (arranque == -1)
					{
						return;
					}

					int nivel = 0;
					int final = -1;

					for (int i = arranque; i < html.Length; i++)
					{
						if (html[i] == '[') nivel++;
						else if (html[i] == ']') nivel--;

						if (nivel == 0)
						{
							final = i;
							break;
						}
					}

					if (final == -1)
					{
						return;
					}

					string json = html.Substring(arranque, final - arranque + 1);

					json = json.Replace("\\\"", "\"");

					var juegos = JsonSerializer.Deserialize<List<IndiePassJuego>>(json);

					ObjetosVentana.tbAmazonLuna.Text = ObjetosVentana.tbAmazonLuna.Text + juegos.Count.ToString() + " juegos (Indie Pass) ";

					using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
					{
						try
						{
							conexion.Execute(
								"INSERT INTO temporalindiepassjson (contenido, fecha, enlace) VALUES (@contenido, @fecha, @enlace)",
								new
								{
									contenido = JsonSerializer.Serialize(juegos),
									fecha = DateTime.Now,
									enlace = "1"
								}
							);
						}
						catch
						{
							// Sin notificación de error
						}
					}

					ObjetosVentana.botonAmazonLunaArrancar.IsEnabled = true;
				}
			}
		}
	}

	public class AmazonLunaJuego
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
	}

	public class IndiePassJuego
	{
		public int id { get; set; }
		public string title { get; set; }
	}
}
