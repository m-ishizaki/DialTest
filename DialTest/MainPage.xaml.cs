using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DialTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DialSettings();
        }

        private void DialSettings()
        {
            // コントローラー
            var controller = RadialController.CreateForCurrentView();

            // メニューの設定
            var configration = RadialControllerConfiguration.GetForCurrentView();
            var menuItem = RadialControllerMenuItem.CreateFromKnownIcon("Item Name", RadialControllerMenuKnownIcon.InkColor);
            controller.Menu.Items.Add(menuItem);
            configration.SetDefaultMenuItems(Enumerable.Empty<RadialControllerSystemMenuItemKind>());
            configration.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, RadialControllerSystemMenuItemKind.Scroll });

            // イベント
            menuItem.Invoked += async (sender, args) =>
            {
                var selected = controller.Menu.GetSelectedMenuItem();
                var md = new Windows.UI.Popups.MessageDialog(selected?.DisplayText, sender?.DisplayText);
                await md.ShowAsync();
            };

            controller.ControlLost += async (sender, arg) =>
            {
                var selected = controller.Menu.GetSelectedMenuItem();
                var md = new Windows.UI.Popups.MessageDialog(selected?.DisplayText, sender?.Menu?.GetSelectedMenuItem()?.DisplayText);
                await md.ShowAsync();
            };

            controller.RotationChanged += async (sender, arg) =>
            {
                var selected = controller.Menu.GetSelectedMenuItem();
                var md = new Windows.UI.Popups.MessageDialog(selected?.DisplayText, arg?.RotationDeltaInDegrees.ToString());
                await md.ShowAsync();
            };

            controller.ButtonClicked += async (sender, arg) =>
            {
                var selected = controller.Menu.GetSelectedMenuItem();
                var md = new Windows.UI.Popups.MessageDialog(selected?.DisplayText, arg?.ToString());
                await md.ShowAsync();
            };
        }
    }
}
