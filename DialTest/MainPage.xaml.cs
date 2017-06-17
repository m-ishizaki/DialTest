using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        // Surface Dial の Controller インスタンス
        private RadialController _controller;

        public MainPage()
        {
            this.InitializeComponent();

            DialSettings();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Dial の操作の記録
        /// </summary>
        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Dial の操作を記録します
        /// </summary>
        /// <param name="cont"></param>
        /// <param name="menu"></param>
        /// <param name="event"></param>
        /// <param name="arg"></param>
        private void AddItem(RadialControllerScreenContact cont, RadialControllerMenuItem menu, string @event, string arg)
        {
            Items.Insert(0, $"{cont?.Position.ToString()} : {menu?.DisplayText} : {@event} : {arg}");
            if (1000 < Items.Count) Items.RemoveAt(1000);
        }

        /// <summary>
        /// Dial の操作を記録します
        /// </summary>
        /// <param name="cont"></param>
        /// <param name="ctrl"></param>
        /// <param name="event"></param>
        /// <param name="arg"></param>
        private void AddItem(RadialControllerScreenContact cont, RadialController ctrl, string @event, string arg)
            => AddItem(cont, ctrl?.Menu?.GetSelectedMenuItem(), @event, arg);

        private void DialSettings()
        {
            // コントローラーを取得
            var controller = _controller = RadialController.CreateForCurrentView();

            {// アプリ独自メニューの設定
                var menuItems = new List<RadialControllerMenuItem>(){
                RadialControllerMenuItem.CreateFromKnownIcon("Item Name 1", RadialControllerMenuKnownIcon.InkColor),
                RadialControllerMenuItem.CreateFromKnownIcon("Item Name 2", RadialControllerMenuKnownIcon.PenType) };
                menuItems.ForEach(m => controller.Menu.Items.Add(m));
                // メニューが選択されたイベント
                menuItems.ForEach(m => m.Invoked += (sender, arg) => AddItem(
                    null,
                    sender,
                    nameof(RadialControllerMenuItem.Invoked),
                    arg?.ToString()
                    )
                );
            }
            {// 標準メニューの設定
                // ここでは音量とスクロールだけを設定している
               var configration = RadialControllerConfiguration.GetForCurrentView();
                //configration.SetDefaultMenuItems(Enumerable.Empty<RadialControllerSystemMenuItemKind>());
                configration.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, RadialControllerSystemMenuItemKind.Scroll });
            }

            // イベントハンドラの設定（イベントの記録）

            // 標準メニューを選択した際や
            // アプリがフォーカスを失った時にも発生
            controller.ControlLost += (sender, arg) => AddItem(
                null,
                sender,
                nameof(RadialController.ControlLost),
                arg?.ToString()
                );

            // Dial を回転したときに発生
            // arg.IsButtonPressed で押し込みながらの回転かを判別できる
            controller.RotationChanged += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.RotationChanged),
                $"{nameof(arg.IsButtonPressed)} : {arg.IsButtonPressed}, {nameof(arg.RotationDeltaInDegrees)} : {arg.RotationDeltaInDegrees}"
                );

            // Dial のクリック（押し込み）時に発生
            // クリック時、ButtonPressed ⇒ ButtonReleased ⇒ ButtonClicked と 3 つが続けて発生
            // 押し込みながら Dial 回転した後でも ButtonClicked が発生する
            controller.ButtonClicked += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ButtonClicked),
                arg?.ToString()
                );

            controller.ButtonHolding += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ButtonHolding),
                arg?.ToString()
                );

            // Dial を押し込んだ際に発生
            controller.ButtonPressed += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ButtonPressed),
                arg?.ToString()
            );

            // Dial を押し込んだ後、離した際に発生
            controller.ButtonReleased += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ButtonReleased),
                arg?.ToString()
                );

            // 標準メニュー選択状態から独自メニューを選択した際や
            // アプリがフォーカスを得た際にも発生
            controller.ControlAcquired += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ControlAcquired),
                arg?.ToString()
                );

            controller.ScreenContactContinued += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ScreenContactContinued),
                arg?.ToString()
                );

            controller.ScreenContactEnded += (sender, arg) => AddItem(
                null,
                sender,
                nameof(RadialController.ScreenContactEnded),
                arg?.ToString()
                );

            controller.ScreenContactStarted += (sender, arg) => AddItem(
                arg.Contact,
                sender,
                nameof(RadialController.ScreenContactStarted),
                $"{nameof(arg.IsButtonPressed)} : {arg?.IsButtonPressed}, {arg?.ToString()}"
                );
        }
    }
}
