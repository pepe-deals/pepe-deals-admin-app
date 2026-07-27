using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.UI;
using static pepeizqs_deals_app.MainWindow;

namespace Interfaz
{
    public static class Pestañas
    {
        public static void Cargar()
        {
            //ResourceLoader recursos = new ResourceLoader();

            ObjetosVentana.nvPrincipal.MenuItems.RemoveAt(0);
            ObjetosVentana.nvPrincipal.MenuItems.Insert(0, ObjetosVentana.nvItemMenu);

            ObjetosVentana.nvItemMenu.PointerEntered += EntraRatonNvItemMenu;
            ObjetosVentana.nvItemMenu.PointerEntered += Animaciones.EntraRatonNvItem2;
            ObjetosVentana.nvItemMenu.PointerExited += Animaciones.SaleRatonNvItem2;

            //TextBlock tbOpcionesTt = new TextBlock
            //{
            //    Text = recursos.GetString("Options")
            //};

            //ToolTipService.SetToolTip(ObjetosVentana.nvItemOpciones, tbOpcionesTt);
            //ToolTipService.SetPlacement(ObjetosVentana.nvItemOpciones, PlacementMode.Bottom);

            ObjetosVentana.nvItemOpciones.PointerEntered += Animaciones.EntraRatonNvItem2;
            ObjetosVentana.nvItemOpciones.PointerExited += Animaciones.SaleRatonNvItem2;
        }

        public static void Visibilidad(Grid grid, bool nv, StackPanel sp, bool mostrarNombre)
        {
            foreach (var item in ObjetosVentana.nvPrincipal.MenuItems)
            {
                if (item.GetType() == typeof(StackPanel2))
                {
                    StackPanel2 spItem = item as StackPanel2;

                    if (spItem.Children.Count > 0)
                    {
                        if (spItem.Children[1] != null)
                        {
                            if (spItem.Children[1].GetType() == typeof(TextBlock))
                            {
                                TextBlock tb = spItem.Children[1] as TextBlock;
                                tb.Visibility = Visibility.Collapsed;
                            }
                        }
                    }                   
                }
            }

            ObjetosVentana.nvItemSubirArriba.Visibility = Visibility.Collapsed;

            ObjetosVentana.nvPrincipal.Visibility = Visibility.Collapsed;
            ObjetosVentana.gridHumble.Visibility = Visibility.Collapsed;
			ObjetosVentana.gridEpic.Visibility = Visibility.Collapsed;
			ObjetosVentana.gridAmazonLuna.Visibility = Visibility.Collapsed;
			ObjetosVentana.gridRSS.Visibility = Visibility.Collapsed;
			ObjetosVentana.gridSteam.Visibility = Visibility.Collapsed;
            ObjetosVentana.gridDescripciones.Visibility = Visibility.Collapsed;
			ObjetosVentana.gridBots.Visibility = Visibility.Collapsed;
			ObjetosVentana.gridOpciones.Visibility = Visibility.Collapsed;

			grid.Visibility = Visibility.Visible;

            if (nv == true)
            {
                ObjetosVentana.nvPrincipal.Visibility = Visibility.Visible;
            }
            else
            {
                ObjetosVentana.nvPrincipal.Visibility = Visibility.Collapsed;
            }

            if (mostrarNombre == true) 
            {
                if (sp.Children[1] != null)
                {
                    if (sp.Children[1].GetType() == typeof(TextBlock))
                    {
                        TextBlock tb = sp.Children[1] as TextBlock;
                        tb.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public static void CreadorItems(string nombre, string imagenEnlace = null)
        {
            StackPanel2 sp = new StackPanel2
            {
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5),
                Orientation = Orientation.Horizontal,
                Height = 30
            };

            sp.PointerEntered += Animaciones.EntraRatonStackPanel2;
            sp.PointerExited += Animaciones.SaleRatonStackPanel2;

            if (imagenEnlace != null)
            {
				Image imagen = new Image
				{
					Source = new BitmapImage(new Uri(imagenEnlace)),
					MaxHeight = 20,
					MaxWidth = 20,
					Margin = new Thickness(0, 0, 15, 0)
				};

				sp.Children.Add(imagen);
			}

            TextBlock tb = new TextBlock
            {
                Text = nombre,
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                
                VerticalAlignment = VerticalAlignment.Center
            };

            sp.Children.Add(tb);

            if (nombre != null)
            {
                TextBlock tbTt = new TextBlock
                {
                    Text = nombre
                };

                ToolTipService.SetToolTip(sp, tbTt);
                ToolTipService.SetPlacement(sp, PlacementMode.Bottom);
            }
            
            ObjetosVentana.nvPrincipal.MenuItems.Insert(1, sp);
        }

        public static void EntraRatonNvItemMenu(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as NavigationViewItem);
        }
    }
}
