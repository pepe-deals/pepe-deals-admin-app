using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using static pepeizqs_deals_app.MainWindow;

namespace Interfaz
{
    public static class ScrollViewers
    {
        public static void Cargar()
        {
            ObjetosVentana.nvItemSubirArriba.PointerPressed += SubirArriba;
            ObjetosVentana.nvItemSubirArriba.PointerEntered += Animaciones.EntraRatonNvItem2;
            ObjetosVentana.nvItemSubirArriba.PointerExited += Animaciones.SaleRatonNvItem2;

            ObjetosVentana.svHumble.ViewChanging += svScroll;
			ObjetosVentana.svEpic.ViewChanging += svScroll;
			ObjetosVentana.svAmazonLuna.ViewChanging += svScroll;
			ObjetosVentana.svRSS.ViewChanging += svScroll;
			ObjetosVentana.svSteam.ViewChanging += svScroll;
            ObjetosVentana.svDescripciones.ViewChanging += svScroll;
			ObjetosVentana.svOpciones.ViewChanging += svScroll;
		}

        private static void svScroll(object sender, ScrollViewerViewChangingEventArgs args)
        {
            ScrollViewer sv = sender as ScrollViewer;

            if (sv.VerticalOffset > 150)
            {
                ObjetosVentana.nvItemSubirArriba.Visibility = Visibility.Visible;
            }
            else
            {
                ObjetosVentana.nvItemSubirArriba.Visibility = Visibility.Collapsed;
            }
        }

        public static void SubirArriba(object sender, RoutedEventArgs e)
        {
            NavigationViewItem nvItem = sender as NavigationViewItem;
            nvItem.Visibility = Visibility.Collapsed;

            Grid grid = nvItem.Content as Grid;
            grid.Background = new SolidColorBrush(Colors.Transparent);

            if (ObjetosVentana.gridHumble.Visibility == Visibility.Visible)
            {
                ObjetosVentana.svHumble.ChangeView(null, 0, null);
            }
            else if (ObjetosVentana.gridEpic.Visibility == Visibility.Visible)
			{
				ObjetosVentana.svEpic.ChangeView(null, 0, null);
			}
			else if (ObjetosVentana.gridAmazonLuna.Visibility == Visibility.Visible)
			{
				ObjetosVentana.svAmazonLuna.ChangeView(null, 0, null);
			}
			else if (ObjetosVentana.gridRSS.Visibility == Visibility.Visible)
            {
                ObjetosVentana.svRSS.ChangeView(null, 0, null);
            }
            else if (ObjetosVentana.gridSteam.Visibility == Visibility.Visible)
            {
                ObjetosVentana.svSteam.ChangeView(null, 0, null);
            }
			else if (ObjetosVentana.gridDescripciones.Visibility == Visibility.Visible)
			{
				ObjetosVentana.svDescripciones.ChangeView(null, 0, null);
			}
			else if (ObjetosVentana.gridBots.Visibility == Visibility.Visible)
			{
				ObjetosVentana.svBots.ChangeView(null, 0, null);
			}
			else if (ObjetosVentana.gridOpciones.Visibility == Visibility.Visible)
            {
                ObjetosVentana.svOpciones.ChangeView(null, 0, null);
            }
		}

        public static void EnseñarSubir(ScrollViewer sv)
        {
            if (sv.VerticalOffset > 50)
            {
                ObjetosVentana.nvItemSubirArriba.Visibility = Visibility.Visible;
            }
            else
            {
                ObjetosVentana.nvItemSubirArriba.Visibility = Visibility.Collapsed;
            }
        }
    }
}
