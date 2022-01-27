using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Tetris.Classes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //The order of the URIs match the block IDs.
        private readonly ImageSource[] _tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        }; 
        
        private readonly ImageSource[] _blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        private readonly Image[,] _imageControls;

        private GameState _gameState = new GameState();
        private readonly int _maxDelay = 1000;
        private readonly int _minDelay = 75;
        private readonly int _delayDecrease = 25;



        public MainWindow()
        {
            InitializeComponent();
            _imageControls = SetupGameCanvas(_gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            NextImage.Source = _blockImages[blockQueue.NextBlock.ID];
        }

        private void DrawHeldBlock(Classes.Block heldBlock)
        {
            if(heldBlock is null)
            {
                HoldImage.Source = _blockImages[0];
            }
            else
            {
                HoldImage.Source = _blockImages[heldBlock.ID];
            }
        }

        private void DrawGhostBlock(Classes.Block block)
        {
            int drop = _gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                _imageControls[p.Row + drop, p.Column].Opacity = 0.25;
                _imageControls[p.Row + drop, p.Column].Source = _tileImages[block.ID];
            }
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    _imageControls[r, c].Source = _tileImages[id];
                    _imageControls[r, c].Opacity = 1;
                }
            }
        }

        private void DrawBlock(Classes.Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                _imageControls[p.Row, p.Column].Source = _tileImages[block.ID];
                _imageControls[p.Row, p.Column].Opacity = 1;
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }


        private async Task GameLoop()
        {
            Draw(_gameState);

            while (!_gameState.GameOver)
            {
                int delay = Math.Max(_minDelay, _maxDelay - (_gameState.Score * _delayDecrease));
                await Task.Delay(delay);
                _gameState.MoveBlockDown();
                Draw(_gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Final Score: {_gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    _gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    _gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    _gameState.MoveBlockDown();
                    break;
                case Key.NumPad0:
                    _gameState.RotateBlockCW();
                    break;
                case Key.NumPad1:
                    _gameState.RotateBlockCCW();
                    break;
                case Key.RightShift:
                    _gameState.HoldBlock();
                    break;
                case Key.Space:
                    _gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(_gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            _gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}
