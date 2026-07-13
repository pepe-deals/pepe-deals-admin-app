using Interfaz;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.VisualBasic;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static pepeizqs_deals_app.MainWindow;

namespace Modulos
{
	public static class Descripciones
	{
		public static void Cargar()
		{
			ObjetosVentana.botonDescripcionesArrancar.Click += async (s, e) => await ArrancarJuegosClick(s, e);
			ObjetosVentana.botonDescripcionesArrancar.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonDescripcionesArrancar.PointerExited += Animaciones.SaleRatonBoton2;

			ObjetosVentana.botonDescripcionesArrancar2.Click += async (s, e) => await ArrancarNoticiasClick(s, e);
			ObjetosVentana.botonDescripcionesArrancar2.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonDescripcionesArrancar2.PointerExited += Animaciones.SaleRatonBoton2;

			ObjetosVentana.botonDescripcionesArrancar3.Click += async (s, e) => await ArrancarCuratorsClick(s, e);
			ObjetosVentana.botonDescripcionesArrancar3.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonDescripcionesArrancar3.PointerExited += Animaciones.SaleRatonBoton2;

			ObjetosVentana.botonDescripcionesArrancar4.Click += async (s, e) => await ArrancarBundlesClick(s, e);
			ObjetosVentana.botonDescripcionesArrancar4.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonDescripcionesArrancar4.PointerExited += Animaciones.SaleRatonBoton2;

			using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
			{
				conexion.Open();

				if (conexion.State == System.Data.ConnectionState.Open)
				{
					string sqlBuscar = @"SELECT 'juegos' AS tabla, CAST(COUNT(CASE WHEN descripcionSEO IS NOT NULL THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS porcentaje FROM juegos
										UNION ALL
										SELECT 'noticias', CAST(COUNT(CASE WHEN descripcionSEO IS NOT NULL THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) FROM noticias
										UNION ALL
										SELECT 'curators', CAST(COUNT(CASE WHEN descripcionSEO IS NOT NULL THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) FROM curators
										UNION ALL
										SELECT 'bundles', CAST(COUNT(CASE WHEN descripcionSEO IS NOT NULL THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) FROM bundles";

					using (SqlCommand comandoBuscar = new SqlCommand(sqlBuscar, conexion))
					{
						using (SqlDataReader lector = comandoBuscar.ExecuteReader())
						{
							while (lector.Read())
							{
								string tabla = lector.GetString(0);
								decimal porcentaje = lector.GetDecimal(1);

								if (tabla == "juegos")
								{
									ObjetosVentana.botonDescripcionesArrancarTexto.Text = "Arrancar Juegos - " + porcentaje.ToString() + "%";
								}
								else if (tabla == "noticias")
								{
									ObjetosVentana.botonDescripcionesArrancar2Texto.Text = "Arrancar Noticias - " + porcentaje.ToString() + "%";
								}
								else if (tabla == "curators")
								{
									ObjetosVentana.botonDescripcionesArrancar3Texto.Text = "Arrancar Curators - " + porcentaje.ToString() + "%";
								}
								else if (tabla == "bundles")
								{
									ObjetosVentana.botonDescripcionesArrancar4Texto.Text = "Arrancar Bundles - " + porcentaje.ToString() + "%";
								}
							}
						}
					}
				}
			}
		}

		private static async Task ArrancarJuegosClick(object sender, RoutedEventArgs e)
		{
			PowerManager.MantenerActivo();
			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = false;

			int cantidadAñadidos = 0;
			bool hayPendientes = true;

			while (hayPendientes == true)
			{
				hayPendientes = false;

				using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
				{
					conexion.Open();

					if (conexion.State == System.Data.ConnectionState.Open)
					{
						//string añadir = "ALTER TABLE juegos ADD steamMachineTokens NVARCHAR(1024) NULL";

						//using (SqlCommand comandoAñadir = new SqlCommand(añadir, conexion))
						//{
						//	try
						//	{
						//		comandoAñadir.CommandTimeout = 0;
						//		comandoAñadir.ExecuteNonQuery();
						//	}
						//	catch (SqlException ex)
						//	{
								
						//	}
						//}


						string sqlBuscar = @"SELECT id, tipo, nombre, JSON_VALUE(caracteristicas, '$.Descripcion'), categorias, etiquetas FROM juegos WHERE nombre IS NOT NULL AND nombre != ''
						AND caracteristicas IS NOT NULL AND JSON_VALUE(caracteristicas, '$.Descripcion') IS NOT NULL AND JSON_VALUE(caracteristicas, '$.Descripcion') != ''
						AND categorias IS NOT NULL AND categorias != ''
						AND etiquetas IS NOT NULL AND etiquetas != ''
						AND descripcionSEO IS NULL";

						using (SqlCommand comandoBuscar = new SqlCommand(sqlBuscar, conexion))
						{
							using (SqlDataReader lector = comandoBuscar.ExecuteReader())
							{
								while (lector.Read())
								{
									hayPendientes = true;
									ObjetosVentana.tbDescripciones2.Text = "Cantidad Añadidos: " + cantidadAñadidos.ToString();

									JuegoSEO juego = new JuegoSEO();

									juego.Id = lector.GetInt32(0);
									juego.Tipo = lector.GetString(1);
									juego.Nombre = lector.GetString(2);
									juego.Descripcion = lector.GetString(3);
									juego.Categorias = JsonSerializer.Deserialize<List<string>>(lector.GetString(4));
									juego.Etiquetas = JsonSerializer.Deserialize<List<string>>(lector.GetString(5));

									string tipoFinal = string.Empty;

									if (juego.Tipo == "0")
									{
										tipoFinal = "Game";
									}
									else if (juego.Tipo == "1")
									{
										tipoFinal = "DLC";
									}
									else if (juego.Tipo == "3")
									{
										tipoFinal = "Soundtrack";
									}
									else if (juego.Tipo == "4")
									{
										tipoFinal = "Software";
									}

									string prompt = $"""
									Write a SEO description for a deals {tipoFinal} page, and it is MANDATORY that it be between 150 and 160 characters.

									Name: {juego.Nombre}
									Old Description: {juego.Descripcion}
									Type: {tipoFinal}
									Categories {SteamCategorias.Resolver(juego.Categorias?.Take(6))}	
									Tags: {SteamEtiquetas.Resolver(juego.Etiquetas?.Take(6))}
									""";

									bool sensible = false;
									bool sensible2 = false;
									int i = 0;
									while (i < 10)
									{
										if (sensible == true)
										{
											prompt = $"""
											Write a SEO description for a deals {tipoFinal} page, and it is MANDATORY that it be between 150 and 160 characters.

											Name: {juego.Nombre}
											Type: {tipoFinal}
											Categories {SteamCategorias.Resolver(juego.Categorias?.Take(6).Where(i => i.StartsWith("Family") == false))}
											""";
										}

										if (sensible2 == true)
										{
											prompt = $"""
											Write a SEO description for a deals {tipoFinal} page, and it is MANDATORY that it be between 150 and 160 characters.
											
											Type: {tipoFinal}
											Categories {SteamCategorias.Resolver(juego.Categorias?.Take(6).Where(i => i.StartsWith("Family") == false))}
											""";
										}

										var ollama = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2:latest");

										var chat = new Chat(ollama, "You are a SEO description writer. You ONLY output the description text, which must be strictly between 150 and 160 characters. No character counts, no explanations, no quotes, no double quotes, no labels. Just the raw text.");
										string texto = string.Empty;

										await foreach (var mensaje in chat.SendAsync(prompt))
										{
											texto = texto + mensaje;
										}

										texto = texto.Replace(Strings.ChrW(34).ToString(), null);

										if (texto.Length > 160)
										{
											int ultimoEspacio = texto.LastIndexOf(' ', 156);
											texto = ultimoEspacio != -1 ? texto[..ultimoEspacio] + " ..." : texto[..156] + " ...";
										}

										if (texto.StartsWith("I cannot create") == true || texto.StartsWith("I can't create") == true)
										{
											if (i > 5)
											{
												sensible2 = true;
											}
											else
											{
												sensible = true;
											}
										}

										ObjetosVentana.tbDescripciones.Text = juego.Id.ToString() + " - " + juego.Nombre + "\n\n" + texto.Length.ToString() + "\n" + texto;

										if (texto.Length >= 150 && texto.Length <= 160 && texto.StartsWith("I can") == false)
										{
											using (SqlConnection conexionUpdate = new SqlConnection(DatosPersonales.Servidor))
											{
												conexionUpdate.Open();
												using (SqlCommand cmd = new SqlCommand("UPDATE juegos SET descripcionSEO = @desc WHERE id = @id", conexionUpdate))
												{
													cmd.Parameters.AddWithValue("@desc", texto);
													cmd.Parameters.AddWithValue("@id", juego.Id);
													cmd.ExecuteNonQuery();

													cantidadAñadidos += 1;
													break;
												}
											}
										}

										i += 1;
									}
								}
							}
						}
					}
				}
			}

			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = true;
			PowerManager.Liberar();
		}

		private static async Task ArrancarNoticiasClick(object sender, RoutedEventArgs e)
		{
			PowerManager.MantenerActivo();
			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = false;

			int cantidadAñadidos = 0;
			bool hayPendientes = true;

			while (hayPendientes == true)
			{
				hayPendientes = false;

				using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
				{
					conexion.Open();

					if (conexion.State == System.Data.ConnectionState.Open)
					{
						string sqlBuscar = @"SELECT id, tituloEn, contenidoEn FROM noticias WHERE 
						tituloEn IS NOT NULL
						AND contenidoEn IS NOT NULL
						AND descripcionSEO IS NULL";

						using (SqlCommand comandoBuscar = new SqlCommand(sqlBuscar, conexion))
						{
							using (SqlDataReader lector = comandoBuscar.ExecuteReader())
							{
								while (lector.Read())
								{
									hayPendientes = true;
									ObjetosVentana.tbDescripciones2.Text = "Cantidad Añadidos: " + cantidadAñadidos.ToString();

									NoticiaSEO noticia = new NoticiaSEO();

									noticia.Id = lector.GetInt32(0);
									noticia.TituloEn = lector.GetString(1);
									noticia.ContenidoEn = lector.GetString(2);

									string prompt = $"""
									Write a SEO description for a news page, and it is MANDATORY that it be between 150 and 160 characters.

									Title: {noticia.TituloEn}
									Old Description: {noticia.ContenidoEn}
									""";

									var ollama = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2:latest");

									var chat = new Chat(ollama, "You are a SEO description writer. You ONLY output the description text, which must be strictly between 150 and 160 characters. No character counts, no explanations, no quotes, no double quotes, no labels. Just the raw text.");
									string texto = string.Empty;

									await foreach (var mensaje in chat.SendAsync(prompt))
									{
										texto = texto + mensaje;
									}

									texto = texto.Replace(Strings.ChrW(34).ToString(), null);

									if (texto.Length > 160)
									{
										int ultimoEspacio = texto.LastIndexOf(' ', 156);
										texto = ultimoEspacio != -1 ? texto[..ultimoEspacio] + " ..." : texto[..156] + " ...";
									}

									ObjetosVentana.tbDescripciones.Text = noticia.Id.ToString() + " - " + noticia.TituloEn + "\n\n" + texto.Length.ToString() + "\n" + texto;

									if (texto.Length >= 150 && texto.Length <= 160)
									{
										using (SqlConnection conexionUpdate = new SqlConnection(DatosPersonales.Servidor))
										{
											conexionUpdate.Open();
											using (SqlCommand cmd = new SqlCommand("UPDATE noticias SET descripcionSEO = @desc WHERE id = @id", conexionUpdate))
											{
												cmd.Parameters.AddWithValue("@desc", texto);
												cmd.Parameters.AddWithValue("@id", noticia.Id);
												cmd.ExecuteNonQuery();

												cantidadAñadidos += 1;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = true;
			PowerManager.Liberar();
		}

		private static async Task ArrancarCuratorsClick(object sender, RoutedEventArgs e)
		{
			PowerManager.MantenerActivo();
			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = false;

			int cantidadAñadidos = 0;
			bool hayPendientes = true;

			while (hayPendientes == true)
			{
				hayPendientes = false;

				using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
				{
					conexion.Open();

					if (conexion.State == System.Data.ConnectionState.Open)
					{
						string sqlBuscar = @"SELECT id, nombre, descripcion FROM curators WHERE 
						nombre IS NOT NULL
						AND descripcionSEO IS NULL";

						using (SqlCommand comandoBuscar = new SqlCommand(sqlBuscar, conexion))
						{
							using (SqlDataReader lector = comandoBuscar.ExecuteReader())
							{
								while (lector.Read())
								{
									hayPendientes = true;
									ObjetosVentana.tbDescripciones2.Text = "Cantidad Añadidos: " + cantidadAñadidos.ToString();

									CuratorSEO curator = new CuratorSEO();

									curator.Id = lector.GetInt32(0);
									curator.Nombre = lector.GetString(1);

									if (lector.IsDBNull(2) == false)
									{
										curator.Descripcion = lector.GetString(2);
									}

									string prompt = $"""
									Write a SEO description in english for a curator page, and it is MANDATORY that it be between 150 and 160 characters.

									Name: {curator.Nombre}
									""";

									if (string.IsNullOrEmpty(curator.Descripcion) == false) 
									{ 
										prompt = prompt + $"Old Description: {curator.Descripcion}";
									}

									var ollama = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2:latest");

									var chat = new Chat(ollama, "You are a SEO description writer. You ONLY output the description text, which must be strictly between 150 and 160 characters. No character counts, no explanations, no quotes, no double quotes, no labels. Just the raw text.");
									string texto = string.Empty;

									await foreach (var mensaje in chat.SendAsync(prompt))
									{
										texto = texto + mensaje;
									}

									texto = texto.Replace(Strings.ChrW(34).ToString(), null);

									if (texto.Length > 160)
									{
										int ultimoEspacio = texto.LastIndexOf(' ', 156);
										texto = ultimoEspacio != -1 ? texto[..ultimoEspacio] + " ..." : texto[..156] + " ...";
									}

									ObjetosVentana.tbDescripciones.Text = curator.Id.ToString() + " - " + curator.Nombre + "\n\n" + texto.Length.ToString() + "\n" + texto;

									if (texto.Length >= 150 && texto.Length <= 160)
									{
										using (SqlConnection conexionUpdate = new SqlConnection(DatosPersonales.Servidor))
										{
											conexionUpdate.Open();
											using (SqlCommand cmd = new SqlCommand("UPDATE curators SET descripcionSEO = @desc WHERE id = @id", conexionUpdate))
											{
												cmd.Parameters.AddWithValue("@desc", texto);
												cmd.Parameters.AddWithValue("@id", curator.Id);
												cmd.ExecuteNonQuery();

												cantidadAñadidos += 1;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = true;
			PowerManager.Liberar();
		}

		private static async Task ArrancarBundlesClick(object sender, RoutedEventArgs e)
		{
			PowerManager.MantenerActivo();
			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = false;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = false;

			int cantidadAñadidos = 0;
			bool hayPendientes = true;

			while (hayPendientes == true)
			{
				hayPendientes = false;

				using (SqlConnection conexion = new SqlConnection(DatosPersonales.Servidor))
				{
					conexion.Open();

					if (conexion.State == System.Data.ConnectionState.Open)
					{
						string sqlBuscar = @"SELECT id, nombre, tienda, juegos FROM bundles WHERE 
						nombre IS NOT NULL
						AND tienda IS NOT NULL
						AND juegos IS NOT NULL
						AND descripcionSEO IS NULL";

						using (SqlCommand comandoBuscar = new SqlCommand(sqlBuscar, conexion))
						{
							using (SqlDataReader lector = comandoBuscar.ExecuteReader())
							{
								while (lector.Read())
								{
									hayPendientes = true;
									ObjetosVentana.tbDescripciones2.Text = "Cantidad Añadidos: " + cantidadAñadidos.ToString();

									BundleSEO bundle = new BundleSEO();

									bundle.Id = lector.GetInt32(0);
									bundle.Nombre = lector.GetString(1);
									bundle.Tienda = lector.GetString(2);

									var juegos = JsonSerializer.Deserialize<List<BundleJuego>>(lector.GetString(3));
									bundle.Juegos = string.Join(", ", juegos.Select(j => j.Nombre).ToList());

									string prompt = $"""
									Write a SEO description in english for a bundle page, and it is MANDATORY that it be between 150 and 160 characters.

									Name: {bundle.Nombre}
									Store: {bundle.Tienda}
									Games: {bundle.Juegos}
									""";

									var ollama = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2:latest");

									var chat = new Chat(ollama, "You are a SEO description writer in english. You ONLY output the description text, which must be strictly between 150 and 160 characters. No character counts, no explanations, no quotes, no double quotes, no labels. Just the raw text.");
									string texto = string.Empty;

									await foreach (var mensaje in chat.SendAsync(prompt))
									{
										texto = texto + mensaje;
									}

									texto = texto.Replace(Strings.ChrW(34).ToString(), null);

									if (texto.Length > 160)
									{
										int ultimoEspacio = texto.LastIndexOf(' ', 156);
										texto = ultimoEspacio != -1 ? texto[..ultimoEspacio] + " ..." : texto[..156] + " ...";
									}

									ObjetosVentana.tbDescripciones.Text = bundle.Id.ToString() + " - " + bundle.Nombre + "\n\n" + texto.Length.ToString() + "\n" + texto;

									if (texto.Length >= 150 && texto.Length <= 160)
									{
										using (SqlConnection conexionUpdate = new SqlConnection(DatosPersonales.Servidor))
										{
											conexionUpdate.Open();
											using (SqlCommand cmd = new SqlCommand("UPDATE bundles SET descripcionSEO = @desc WHERE id = @id", conexionUpdate))
											{
												cmd.Parameters.AddWithValue("@desc", texto);
												cmd.Parameters.AddWithValue("@id", bundle.Id);
												cmd.ExecuteNonQuery();

												cantidadAñadidos += 1;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			ObjetosVentana.botonDescripcionesArrancar.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar2.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar3.IsEnabled = true;
			ObjetosVentana.botonDescripcionesArrancar4.IsEnabled = true;
			PowerManager.Liberar();
		}
	}

	public class JuegoSEO
	{
		public int Id { get; set; }
		public string Tipo { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public List<string> Categorias { get; set; }
		public List<string> Etiquetas { get; set; }
	}

	public class NoticiaSEO
	{
		public int Id { get; set; }
		public string TituloEn { get; set; }
		public string ContenidoEn { get; set; }
	}

	public class CuratorSEO
	{
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
	}

	public class BundleSEO
	{
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string Tienda { get; set; }
		public string Juegos { get; set; }
	}

	public class BundleJuego
	{
		[JsonPropertyName("Nombre")]
		public string Nombre { get; set; }
	}

	public static class SteamCategorias
	{
		public static readonly Dictionary<int, string> Categorias = new()
	{
		{ 1, "Multi-player" },
		{ 2, "Single-player" },
		{ 8, "Valve Anti-Cheat enabled" },
		{ 9, "Co-op" },
		{ 13, "Captions available" },
		{ 14, "Commentary available" },
		{ 15, "Stats" },
		{ 16, "Includes Source SDK" },
		{ 17, "Includes level editor" },
		{ 18, "Partial Controller Support" },
		{ 20, "MMO" },
		{ 21, "Downloadable Content" },
		{ 22, "Steam Achievements" },
		{ 23, "Steam Cloud" },
		{ 24, "Shared/Split Screen" },
		{ 25, "Steam Leaderboards" },
		{ 27, "Cross-Platform Multiplayer" },
		{ 28, "Full controller support" },
		{ 29, "Steam Trading Cards" },
		{ 30, "Steam Workshop" },
		{ 31, "VR Support" },
		{ 32, "Steam Turn Notifications" },
		{ 33, "Native Steam Controller Support" },
		{ 35, "In-App Purchases" },
		{ 36, "Online PvP" },
		{ 37, "Shared/Split Screen PvP" },
		{ 38, "Online Co-op" },
		{ 39, "Shared/Split Screen Co-op" },
		{ 40, "SteamVR Collectibles" },
		{ 41, "Remote Play on Phone" },
		{ 42, "Remote Play on Tablet" },
		{ 43, "Remote Play on TV" },
		{ 44, "Remote Play Together" },
		{ 47, "LAN PvP" },
		{ 48, "LAN Co-op" },
		{ 49, "PvP" },
		{ 50, "Additional High-Quality Audio" },
		{ 51, "Steam China Workshop" },
		{ 52, "Tracked Controller Support" },
		{ 53, "VR Supported" },
		{ 54, "VR Only" },
		{ 61, "HDR available" },
		{ 62, "Family Sharing" },
		{ 63, "Steam Timeline" },
		{ 64, "Adjustable Text Size" },
		{ 65, "Subtitle Options" },
		{ 66, "Color Alternatives" },
		{ 67, "Camera Comfort" },
		{ 68, "Custom Volume Controls" },
		{ 69, "Stereo Sound" },
		{ 70, "Surround Sound" },
		{ 71, "Narrated Game Menus" },
		{ 72, "Chat Speech-to-text" },
		{ 73, "Chat Text-to-speech" },
		{ 74, "Playable without Timed Input" },
		{ 75, "Keyboard Only Option" },
		{ 76, "Mouse Only Option" },
		{ 77, "Touch Only Option" },
		{ 78, "Adjustable Difficulty" },
		{ 79, "Save Anytime" },
	};

		public static string Resolver(IEnumerable<string> ids) =>
			ids == null ? string.Empty :
			string.Join(", ", ids
				.Select(id => int.TryParse(id, out var num) && Categorias.TryGetValue(num, out var nombre) ? nombre : null)
				.Where(n => n != null));
	}

	public static class SteamEtiquetas
	{
		public static readonly Dictionary<int, string> Etiquetas = new()
	{
		{ 9, "Strategy" },
		{ 19, "Action" },
		{ 21, "Adventure" },
		{ 84, "Design & Illustration" },
		{ 87, "Utilities" },
		{ 113, "Free to Play" },
		{ 122, "RPG" },
		{ 128, "Massively Multiplayer" },
		{ 492, "Indie" },
		{ 493, "Early Access" },
		{ 597, "Casual" },
		{ 599, "Simulation" },
		{ 699, "Racing" },
		{ 701, "Sports" },
		{ 784, "Video Production" },
		{ 809, "Photo Editing" },
		{ 872, "Animation & Modeling" },
		{ 1027, "Audio Production" },
		{ 1036, "Education" },
		{ 1445, "Software Training" },
		{ 1616, "Trains" },
		{ 1621, "Music" },
		{ 1625, "Platformer" },
		{ 1628, "Metroidvania" },
		{ 1637, "Dogs" },
		{ 1643, "Building" },
		{ 1644, "Driving" },
		{ 1645, "Tower Defense" },
		{ 1646, "Hack and Slash" },
		{ 1647, "Western" },
		{ 1651, "Satire" },
		{ 1654, "Relaxing" },
		{ 1659, "Zombies" },
		{ 1662, "Survival" },
		{ 1663, "FPS" },
		{ 1664, "Puzzle" },
		{ 1665, "Match 3" },
		{ 1666, "Card Game" },
		{ 1667, "Horror" },
		{ 1669, "Moddable" },
		{ 1670, "4X" },
		{ 1671, "Superhero" },
		{ 1673, "Aliens" },
		{ 1674, "Typing" },
		{ 1676, "RTS" },
		{ 1677, "Turn-Based" },
		{ 1678, "War" },
		{ 1680, "Heist" },
		{ 1681, "Pirates" },
		{ 1684, "Fantasy" },
		{ 1685, "Co-op" },
		{ 1687, "Stealth" },
		{ 1688, "Ninja" },
		{ 1693, "Classic" },
		{ 1695, "Open World" },
		{ 1697, "Third Person" },
		{ 1698, "Point & Click" },
		{ 1702, "Crafting" },
		{ 1708, "Tactical" },
		{ 1710, "Surreal" },
		{ 1714, "Psychedelic" },
		{ 1716, "Roguelike" },
		{ 1717, "Hex Grid" },
		{ 1718, "MOBA" },
		{ 1719, "Comedy" },
		{ 1720, "Dungeon Crawler" },
		{ 1721, "Psychological Horror" },
		{ 1723, "Action RTS" },
		{ 1730, "Sokoban" },
		{ 1732, "Voxel" },
		{ 1734, "Fast-Paced" },
		{ 1738, "Hidden Object" },
		{ 1741, "Turn-Based Strategy" },
		{ 1742, "Story Rich" },
		{ 1743, "Fighting" },
		{ 1746, "Basketball" },
		{ 1751, "Comic Book" },
		{ 1752, "Rhythm" },
		{ 1753, "Skateboarding" },
		{ 1754, "MMORPG" },
		{ 1755, "Space" },
		{ 1756, "Great Soundtrack" },
		{ 1759, "Perma Death" },
		{ 1770, "Board Game" },
		{ 1773, "Arcade" },
		{ 1774, "Shooter" },
		{ 1775, "PvP" },
		{ 1776, "Espionage" },
		{ 1777, "Steampunk" },
		{ 3796, "Based On A Novel" },
		{ 3798, "Side Scroller" },
		{ 3799, "Visual Novel" },
		{ 3810, "Sandbox" },
		{ 3813, "Real Time Tactics" },
		{ 3814, "Third-Person Shooter" },
		{ 3834, "Exploration" },
		{ 3835, "Post-apocalyptic" },
		{ 3839, "First-Person" },
		{ 3841, "Local Co-Op" },
		{ 3843, "Online Co-Op" },
		{ 3854, "Lore-Rich" },
		{ 3859, "Multiplayer" },
		{ 3871, "2D" },
		{ 3877, "Precision Platformer" },
		{ 3878, "Competitive" },
		{ 3916, "Old School" },
		{ 3920, "Cooking" },
		{ 3934, "Immersive" },
		{ 3942, "Sci-fi" },
		{ 3952, "Gothic" },
		{ 3955, "Character Action Game" },
		{ 3959, "Roguelite" },
		{ 3964, "Pixel Graphics" },
		{ 3965, "Epic" },
		{ 3968, "Physics" },
		{ 3978, "Survival Horror" },
		{ 3987, "Historical" },
		{ 3993, "Combat" },
		{ 4004, "Retro" },
		{ 4026, "Difficult" },
		{ 4036, "Parkour" },
		{ 4046, "Dragons" },
		{ 4057, "Magic" },
		{ 4064, "Thriller" },
		{ 4085, "Anime" },
		{ 4094, "Minimalist" },
		{ 4102, "Combat Racing" },
		{ 4106, "Action-Adventure" },
		{ 4115, "Cyberpunk" },
		{ 4136, "Funny" },
		{ 4137, "Transhumanism" },
		{ 4145, "Cinematic" },
		{ 4150, "World War II" },
		{ 4155, "Class-Based" },
		{ 4158, "Beat 'em up" },
		{ 4161, "Real-Time" },
		{ 4166, "Atmospheric" },
		{ 4168, "Military" },
		{ 4172, "Medieval" },
		{ 4175, "Realistic" },
		{ 4182, "Singleplayer" },
		{ 4184, "Chess" },
		{ 4190, "Addictive" },
		{ 4191, "3D" },
		{ 4195, "Cartoony" },
		{ 4202, "Trading" },
		{ 4231, "Action RPG" },
		{ 4234, "Short" },
		{ 4236, "Loot" },
		{ 4242, "Episodic" },
		{ 4252, "Stylized" },
		{ 4255, "Shoot 'Em Up" },
		{ 4291, "Spaceships" },
		{ 4295, "Futuristic" },
		{ 4305, "Colorful" },
		{ 4325, "Turn-Based Combat" },
		{ 4328, "City Builder" },
		{ 4342, "Dark" },
		{ 4345, "Gore" },
		{ 4364, "Grand Strategy" },
		{ 4400, "Abstract" },
		{ 4434, "JRPG" },
		{ 4474, "CRPG" },
		{ 4486, "Choose Your Own Adventure" },
		{ 4508, "Co-op Campaign" },
		{ 4520, "Farming" },
		{ 4535, "Dwarves" },
		{ 4559, "Quick-Time Events" },
		{ 4562, "Cartoon" },
		{ 4598, "Alternate History" },
		{ 4604, "Dark Fantasy" },
		{ 4608, "Swordplay" },
		{ 4637, "Top-Down Shooter" },
		{ 4667, "Violent" },
		{ 4684, "Wargame" },
		{ 4695, "Economy" },
		{ 4711, "Replay Value" },
		{ 4726, "Cute" },
		{ 4736, "2D Fighter" },
		{ 4747, "Character Customization" },
		{ 4754, "Politics" },
		{ 4758, "Twin Stick Shooter" },
		{ 4777, "Spectacle fighter" },
		{ 4791, "Top-Down" },
		{ 4821, "Mechs" },
		{ 4835, "6DOF" },
		{ 4840, "4 Player Local" },
		{ 4845, "Capitalism" },
		{ 4852, "Billiards" },
		{ 4853, "Political" },
		{ 4878, "Parody" },
		{ 4885, "Bullet Hell" },
		{ 4947, "Romance" },
		{ 4975, "2.5D" },
		{ 4994, "Naval Combat" },
		{ 5030, "Dystopian" },
		{ 5055, "eSports" },
		{ 5125, "Procedural Generation" },
		{ 5154, "Score Attack" },
		{ 5160, "Dinosaurs" },
		{ 5179, "Cold War" },
		{ 5186, "Psychological" },
		{ 5230, "Sequel" },
		{ 5300, "God Game" },
		{ 5348, "Mod" },
		{ 5350, "Family Friendly" },
		{ 5363, "Destruction" },
		{ 5372, "Conspiracy" },
		{ 5379, "2D Platformer" },
		{ 5382, "World War I" },
		{ 5390, "Time Attack" },
		{ 5395, "3D Platformer" },
		{ 5407, "Benchmark" },
		{ 5411, "Beautiful" },
		{ 5432, "Programming" },
		{ 5502, "Hacking" },
		{ 5537, "Puzzle Platformer" },
		{ 5547, "Arena Shooter" },
		{ 5608, "Emotional" },
		{ 5613, "Detective" },
		{ 5652, "Collectathon" },
		{ 5673, "Modern" },
		{ 5708, "Remake" },
		{ 5711, "Team-Based" },
		{ 5716, "Mystery" },
		{ 5727, "Baseball" },
		{ 5752, "Robots" },
		{ 5765, "Gun Customization" },
		{ 5794, "Science" },
		{ 5796, "Bullet Time" },
		{ 5851, "Isometric" },
		{ 5900, "Walking Simulator" },
		{ 5914, "Tennis" },
		{ 5923, "Dark Humor" },
		{ 5941, "Reboot" },
		{ 5981, "Mining" },
		{ 6041, "Horses" },
		{ 6052, "Noir" },
		{ 6054, "Elves" },
		{ 6129, "Logic" },
		{ 6214, "Birds" },
		{ 6276, "Inventory Management" },
		{ 6310, "Diplomacy" },
		{ 6378, "Crime" },
		{ 6426, "Choices Matter" },
		{ 6506, "3D Fighter" },
		{ 6621, "Pinball" },
		{ 6625, "Time Manipulation" },
		{ 6650, "Nudity" },
		{ 6691, "1990's" },
		{ 6702, "Mars" },
		{ 6730, "PvE" },
		{ 6815, "Hand-drawn" },
		{ 6835, "Poker" },
		{ 6869, "Nonlinear" },
		{ 6910, "Naval" },
		{ 6915, "Martial Arts" },
		{ 6948, "Rome" },
		{ 6971, "Multiple Endings" },
		{ 7038, "Golf" },
		{ 7107, "Real-Time with Pause" },
		{ 7108, "Party" },
		{ 7178, "Party Game" },
		{ 7208, "Female Protagonist" },
		{ 7250, "Linear" },
		{ 7309, "Skiing" },
		{ 7328, "Bowling" },
		{ 7332, "Base Building" },
		{ 7368, "Local Multiplayer" },
		{ 7423, "Sniper" },
		{ 7432, "Lovecraftian" },
		{ 7481, "Controller" },
		{ 7556, "Dice" },
		{ 7569, "Grid-Based Movement" },
		{ 7622, "Offroad" },
		{ 7702, "Narrative" },
		{ 7743, "1980s" },
		{ 7926, "Artificial Intelligence" },
		{ 7948, "Soundtrack" },
		{ 8013, "Software" },
		{ 8075, "TrackIR" },
		{ 8093, "Minigames" },
		{ 8122, "Level Editor" },
		{ 8253, "Music-Based Procedural Generation" },
		{ 8369, "Investigation" },
		{ 8666, "Runner" },
		{ 8945, "Resource Management" },
		{ 9130, "Hentai" },
		{ 9157, "Underwater" },
		{ 9204, "Immersive Sim" },
		{ 9271, "Trading Card Game" },
		{ 9541, "Demons" },
		{ 9551, "Dating Sim" },
		{ 9564, "Hunting" },
		{ 9592, "Dynamic Narration" },
		{ 9626, "Animals" },
		{ 9803, "Snow" },
		{ 10235, "Life Sim" },
		{ 10383, "Transportation" },
		{ 10397, "Memes" },
		{ 10437, "Trivia" },
		{ 10617, "Samurai" },
		{ 10679, "Time Travel" },
		{ 10695, "Party-Based RPG" },
		{ 10808, "Supernatural" },
		{ 10816, "Split Screen" },
		{ 11014, "Interactive Fiction" },
		{ 11095, "Boss Rush" },
		{ 11104, "Vehicular Combat" },
		{ 11123, "Mouse Only" },
		{ 11333, "Villain Protagonist" },
		{ 11634, "Vikings" },
		{ 12057, "Tutorial" },
		{ 12095, "Sexual Content" },
		{ 12190, "Boxing" },
		{ 12472, "Management" },
		{ 12686, "Vampires" },
		{ 13070, "Solitaire" },
		{ 13276, "Tanks" },
		{ 13382, "Archery" },
		{ 13577, "Sailing" },
		{ 13782, "Experimental" },
		{ 13906, "Game Development" },
		{ 14139, "Turn-Based Tactics" },
		{ 14720, "Nostalgia" },
		{ 14906, "Intentionally Awkward Controls" },
		{ 15045, "Flight" },
		{ 15277, "Philosophical" },
		{ 15564, "Fishing" },
		{ 15868, "Motocross" },
		{ 15954, "Silent Protagonist" },
		{ 16094, "Mythology" },
		{ 16250, "Gambling" },
		{ 16598, "Space Sim" },
		{ 16689, "Time Management" },
		{ 17015, "Werewolves" },
		{ 17305, "Strategy RPG" },
		{ 17337, "Lemmings" },
		{ 17389, "Tabletop" },
		{ 17770, "Asynchronous Multiplayer" },
		{ 17894, "Cats" },
		{ 18594, "FMV" },
		{ 19568, "Cycling" },
		{ 19780, "Submarine" },
		{ 19995, "Dark Comedy" },
		{ 20486, "Wolves" },
		{ 21006, "Underground" },
		{ 21635, "Language Learning" },
		{ 21725, "Tactical RPG" },
		{ 21978, "VR" },
		{ 22602, "Agriculture" },
		{ 22955, "Mini Golf" },
		{ 23491, "Cleaning" },
		{ 24003, "Word Game" },
		{ 25085, "Touch-Friendly" },
		{ 25959, "Wuxia" },
		{ 26921, "Political Sim" },
		{ 27758, "Voice Control" },
		{ 28444, "Snowboarding" },
		{ 29482, "Souls-like" },
		{ 30358, "Nature" },
		{ 31275, "Text-Based" },
		{ 31579, "Otome" },
		{ 32322, "Deckbuilding" },
		{ 33572, "Mahjong" },
		{ 35079, "Job Simulator" },
		{ 37376, "Falling Blocks" },
		{ 42089, "Jump Scare" },
		{ 42152, "Dialogue Heavy" },
		{ 42804, "Action Roguelike" },
		{ 44868, "LGBTQ+" },
		{ 46348, "Zoo" },
		{ 47827, "Wrestling" },
		{ 49213, "Rugby" },
		{ 52406, "Cult" },
		{ 56690, "On-Rails Shooter" },
		{ 61357, "Electronic Music" },
		{ 71389, "Spelling" },
		{ 87918, "Farming Sim" },
		{ 91114, "Shop Keeper" },
		{ 96359, "Skating" },
		{ 97070, "Assassins" },
		{ 97376, "Cozy" },
		{ 117648, "8-bit Music" },
		{ 123332, "Bikes" },
		{ 129761, "ATV" },
		{ 150626, "Gaming" },
		{ 158638, "Cricket" },
		{ 176981, "Battle Royale" },
		{ 180368, "Faith" },
		{ 189941, "Instrumental Music" },
		{ 198631, "Mystery Dungeon" },
		{ 198913, "Motorbike" },
		{ 220585, "Colony Sim" },
		{ 252854, "BMX" },
		{ 255534, "Automation" },
		{ 323922, "Musou" },
		{ 324176, "Hockey" },
		{ 337964, "Rock Music" },
		{ 353880, "Looter Shooter" },
		{ 363767, "Snooker" },
		{ 454187, "Traditional Roguelike" },
		{ 507423, "Foxes" },
		{ 552282, "Wholesome" },
		{ 560542, "Incremental" },
		{ 603297, "Hardware" },
		{ 615955, "Idler" },
		{ 620519, "Hero Shooter" },
		{ 723991, "Bullet Heaven" },
		{ 745697, "Social Deduction" },
		{ 760247, "Xianxia" },
		{ 769306, "Escape Room" },
		{ 776177, "360 Video" },
		{ 791774, "Card Battler" },
		{ 847164, "Volleyball" },
		{ 856791, "Asymmetric VR" },
		{ 889937, "Decorating" },
		{ 916648, "Creature Collector" },
		{ 1023537, "Boomer Shooter" },
		{ 1084988, "Auto Battler" },
		{ 1091588, "Roguelike Deckbuilder" },
		{ 1100686, "Outbreak Sim" },
		{ 1100687, "Automobile Sim" },
		{ 1100688, "Medical Sim" },
		{ 1100689, "Open World Survival Craft" },
		{ 1199779, "Extraction Shooter" },
		{ 1220528, "Hobby Sim" },
		{ 1239876, "Organizing" },
		{ 1254546, "Football (Soccer)" },
		{ 1254552, "Football (American)" },
		{ 1320952, "Desktop Companion" },
		{ 1352486, "Capybaras" },
	};

		public static string Resolver(IEnumerable<string> ids) =>
			ids == null ? string.Empty :
			string.Join(", ", ids
				.Select(id => int.TryParse(id, out var num) && Etiquetas.TryGetValue(num, out var nombre) ? nombre : null)
				.Where(n => n != null));
	}

	public static class PowerManager
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint SetThreadExecutionState(uint esFlags);

		private const uint ES_CONTINUOUS = 0x80000000;
		private const uint ES_SYSTEM_REQUIRED = 0x00000001;
		private const uint ES_DISPLAY_REQUIRED = 0x00000002;

		public static void MantenerActivo()
		{
			SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
		}

		public static void Liberar()
		{
			SetThreadExecutionState(ES_CONTINUOUS);
		}
	}
}
