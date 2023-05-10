using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace SpaceInvaders
{
    public partial class MainWindow : Window
    {
        bool goLeft, goRight;

        List<Rectangle> itemsToRemove = new List<Rectangle>();

        int enemyImages = 0;
        int bulletTimer = 0;
        int bulletTimerLimit = 90;
        int totalEnemies = 0;
        int enemySpeed = 7;
        bool gameOver = false;

        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();

        private void enemyBulletMaker(double x, double y)
        {
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 15,
                Width = 5,
                Fill = Brushes.Azure,
                Stroke = Brushes.Red,
                StrokeThickness = 5
            };

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);

            myCanvas.Children.Add(enemyBullet);
        }

        private void makeEnemies(int limit)
        {
            int left = 0;

            totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();

                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };

                Canvas.SetTop(newEnemy, 50);
                Canvas.SetLeft(newEnemy, left);

                myCanvas.Children.Add(newEnemy);
                left -= 60;

                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                switch (enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/assault_ship.png"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/assault_ship.png"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/assault_ship.png"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/defensive_ship.png"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/defensive_ship.png"));
                        break;
                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/scout_ship.png"));
                        break;
                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/scout_ship.png"));
                        break;
                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/mother_ship.png"));
                        break;
                }
            }
        }

        private void showGameOver(string msg)
        {
            gameOver = true;
            gameTimer.Stop();
            enemiesLeft.Content += " " + msg + " Нажми *Ввод* чтобы повторить";
        }

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/main_ship.png"));
            player.Fill = playerSkin;

            myCanvas.Focus();

            makeEnemies(50);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            enemiesLeft.Content = "Захватчиков осталось: " + totalEnemies;

            if (goLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (goRight == true && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }

            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                enemyBulletMaker(Canvas.GetLeft(player) + 20, 100);

                bulletTimer = bulletTimerLimit;
            }

            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                totalEnemies -= 1;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if (Canvas.GetLeft(x) > 1020)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        showGameOver("Ты был убит захватчиком!");
                    }
                }
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if (Canvas.GetTop(x) > 760)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox))
                    {
                        showGameOver("Ты был убит пулей захватчика!");
                    }
                }
            }

            foreach (Rectangle i in itemsToRemove)
            {
                myCanvas.Children.Remove(i);
            }

            if (totalEnemies < 25)
            {
                enemySpeed = 12;
            }

            if (totalEnemies < 1)
            {
                showGameOver("Победа, ты спас мир!");
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
        }

        private void closeGame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.LimeGreen,
                    Stroke = Brushes.Azure
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);

                myCanvas.Children.Add(newBullet);
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }
    }
}
