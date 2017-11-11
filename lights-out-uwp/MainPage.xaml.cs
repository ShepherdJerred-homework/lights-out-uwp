using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Newtonsoft.Json;

namespace lights_out_uwp
{
    public sealed partial class MainPage {
        private LightsOutGame _game;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _game = new LightsOutGame();
            InitGame();
        }

        private void InitGame() {
            _game.NewGame();
            CreateGrid();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            string json = JsonConvert.SerializeObject(_game);
            ApplicationData.Current.LocalSettings.Values["game"] = json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("username")) {
                string json = ApplicationData.Current.LocalSettings.Values["game"] as string;
                _game = JsonConvert.DeserializeObject<LightsOutGame>(json);
            }
        }

        private void CreateGrid() {
            int rectSize = (int)CanvasMain.Width / _game.NumCells;
            Debug.WriteLine("Size:" + rectSize);
            Debug.WriteLine("Width:" + CanvasMain.Width);
            Debug.WriteLine("Num:" + _game.NumCells);
            CanvasMain.Children.Clear();
            // Create rectangles for grid
            for (int r = 0; r < _game.NumCells; r++) {
                for (int c = 0; c < _game.NumCells; c++) {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    rect.Width = rectSize + 1;
                    rect.Height = rectSize + 1;
                    rect.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                    // Store each row and col as a Point
                    rect.Tag = new Point(r, c);
                    // Register event handler
                    rect.PointerPressed += Rect_PointerPressed;
                    // Put the rectangle at the proper location within the canvas
                    Canvas.SetTop(rect, r * rectSize);
                    Canvas.SetLeft(rect, c * rectSize);
                    // Add the new rectangle to the canvas' children
                    CanvasMain.Children.Add(rect);
                }
            }
            UpdateGridColors();
        }

        private void Rect_PointerPressed(object sender, PointerRoutedEventArgs e) {
            Rectangle rect = sender as Rectangle;
            if (rect != null) {
                var rowCol = (Point)rect.Tag;
                int row = (int)rowCol.X;
                int col = (int)rowCol.Y;
                _game.MakeMove(row, col);
            }
            UpdateGridColors();
            CheckWinCondition();
        }

        private async void CheckWinCondition() {
            if (_game.PlayerWon()) {
                MessageDialog msgDialog = new MessageDialog("Congratulations! You've won!", "Lights Out!");
                msgDialog.Commands.Add(new UICommand("OK"));
                await msgDialog.ShowAsync();
            }
        }

        private void UpdateGridColors() {
            int index = 0;

            // Set the colors of the rectangles
            for (int r = 0; r < _game.NumCells; r++) {
                for (int c = 0; c < _game.NumCells; c++) {
                    Rectangle rect = CanvasMain.Children[index] as Rectangle;
                    index++;
                    if (_game.GetGridValue(r, c)) {
                        rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                        rect.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                    } else {
                        rect.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                        rect.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
                    }
                }
            }
        }

        private void ButtonAbout_OnClick(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(About));
        }

        private void ButtonNewGame_OnClick(object sender, RoutedEventArgs e) {
            _game.NewGame();
            UpdateGridColors();
        }
    }
}
