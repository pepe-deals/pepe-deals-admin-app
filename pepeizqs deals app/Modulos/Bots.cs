using Interfaz;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;
using static pepeizqs_deals_app.MainWindow;

namespace Modulos
{
	public static class Bots
    {
		public static void Cargar()
		{
			ObjetosVentana.botonBotsAbrirLog.Click += async (s, e) => await ArrancarClick(s, e);
			ObjetosVentana.botonBotsAbrirLog.PointerEntered += Animaciones.EntraRatonBoton2;
			ObjetosVentana.botonBotsAbrirLog.PointerExited += Animaciones.SaleRatonBoton2;
		}

		private static async Task ArrancarClick(object sender, RoutedEventArgs e)
		{
			var picker = new FileOpenPicker();
			picker.FileTypeFilter.Add(".log");

			var hwnd = WindowNative.GetWindowHandle(ObjetosVentana.ventana); 
			InitializeWithWindow.Initialize(picker, hwnd);

			var file = await picker.PickSingleFileAsync();

			if (file != null)
			{
				var stats = AnalizarUserAgents(file.Path);
				
				ObjetosVentana.tbBotsRegistro.Text = "Estadísticas de User Agents:\n\n";

				foreach (var stat in stats)
				{
					ObjetosVentana.tbBotsRegistro.Text += $"{stat.UserAgent}: {stat.Count}\n";
				}
			}
		}

		public record UserAgentStat(string UserAgent, int Count);

		public static List<UserAgentStat> AnalizarUserAgents(string rutaLog)
		{
			int idxUserAgent = -1;
			var conteo = new Dictionary<string, int>();

			foreach (var linea in File.ReadLines(rutaLog))
			{
				if (linea.Length == 0) continue;

				if (linea.StartsWith("#Fields:"))
				{
					var campos = linea.Substring(8).Trim().Split(' ');
					idxUserAgent = Array.IndexOf(campos, "cs(User-Agent)");
					continue;
				}

				if (linea[0] == '#' || idxUserAgent == -1) continue;

				var partes = linea.Split(' ');
				if (partes.Length <= idxUserAgent) continue;

				var ua = partes[idxUserAgent].Replace('+', ' ');
				if (ua == "-") ua = "(vacío)";

				conteo[ua] = conteo.GetValueOrDefault(ua) + 1;
			}

			return conteo
				.Select(kv => new UserAgentStat(kv.Key, kv.Value))
				.OrderByDescending(s => s.Count)
				.ToList();
		}
	}
}
