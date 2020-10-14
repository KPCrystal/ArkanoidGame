using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public partial class Arkanoid : Form
    {
        PictureBox _paddle, _ball;

        int _paddleDx = 10;                 //パドルポジション
        int dx = 2;                         //X ポジション
        int dy = 2;                         //Y ポジション
        int score = 0;                      //スコア
        int live = 3;                       //命

        List<PictureBox> _hearts;

        /// <summary>
        /// 初期表示
        /// </summary>
        public Arkanoid()
        {
            InitializeComponent();
            Intialize();
        }

        #region イベント
        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Arkanoid_KeyDown(object sender, KeyEventArgs e)
        {
            int left = _paddle.Location.X;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    left -= _paddleDx;
                    break;
                case Keys.Right:
                    left += _paddleDx;
                    break;
                case Keys.Space:
                    tm.Start();
                    break;
                case Keys.Escape:
                    tm.Stop();
                    DialogResult result = MessageBox.Show("Do you want to exit?", "Game Pause",
                        MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        tm.Start();
                    }
                    break;
            }
            if (left >= 0 && left <= Width - _paddle.Width)
            {
                _paddle.Left = left;
                if (!tm.Enabled)
                    _ball.Left = left + _paddle.Width / 2 - _ball.Width / 2;
            }
        }

        /// <summary>
        /// タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tm_Tick(object sender, EventArgs e)
        {
            dx = _ball.Left <= 0 || _ball.Left >= Width - _ball.Width ? -dx : dx;
            dy = _ball.Top <= 0 ? -dy : dy;

            // ボールをハンドル
            if (_ball.Top >= Height)
            {
                tm.Stop();
                _ball.Left = _paddle.Left + _paddle.Width / 2 - _ball.Width / 2;
                _ball.Top = _paddle.Top - _ball.Height;
                _hearts[live - 1].Image = Properties.Resources.dark_heart;
                live -= 1;
                if (live < 1)
                {
                    MessageBox.Show("Game over! You Lose.");
                    Controls.Clear();
                }
            }

            _ball.Left -= dx; _ball.Top -= dy;
            foreach (Control c in Controls)
                if (c.Tag != null && _ball.Bounds.IntersectsWith(c.Bounds))
                {
                    caldy(c); caldx(c);
                    Controls.Remove(c);
                    score += 10;
                    lblScore.Text = "Score: " + score.ToString();
                }

            if (_ball.Bounds.IntersectsWith(_paddle.Bounds))
            {
                dy = Math.Abs(dy); caldx(_paddle);
            }
            if (score == 9 * 5 * 10) win();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// ゲームの初期化設定
        /// </summary>
        void Intialize()
        {
            createPaddle();
            createBall();
            drawBlocks();
            createHearts();
        }

        /// <summary>
        /// パドルを作成
        /// </summary>
        void createPaddle()
        {
            _paddle = new PictureBox();
            _paddle.Size = new Size(140, 20);
            _paddle.SizeMode = PictureBoxSizeMode.StretchImage;
            _paddle.Image = Properties.Resources.paddle;
            _paddle.Location = new Point(Width / 2 - _paddle.Width / 2
                , Height - _paddle.Height);
            Controls.Add(_paddle);
        }

        /// <summary>
        /// ボールを作成
        /// </summary>
        void createBall()
        {
            _ball = new PictureBox();
            _ball.Size = new Size(16, 16);
            _ball.SizeMode = PictureBoxSizeMode.StretchImage;
            _ball.Image = Properties.Resources.ball;
            _ball.Location
                = new Point(_paddle.Left + _paddle.Width / 2 - _ball.Width / 2
                , _paddle.Top - _ball.Height);
            Controls.Add(_ball);
        }

        /// <summary>
        /// ブログを作成
        /// </summary>
        void drawBlocks()
        {
            Random rand = new Random();

            List<Bitmap> blockImages = new List<Bitmap>
            {
                Properties.Resources.block01, Properties.Resources.block02,
                Properties.Resources.block03, Properties.Resources.block04,
                Properties.Resources.block05
            };

            // ブログのサイズを設定
            blockResize(blockImages, 70, 30);

            //ループして複数のブログを作成
            for (int rows = 0; rows < 5; rows++)
                for (int cols = 0; cols < 9; cols++)
                {
                    Label lblBlock = new Label();
                    lblBlock.Tag = "brick";
                    lblBlock.Location = new Point(50 + cols * 70, rows * 30 + 50);
                    lblBlock.Size = new Size(70, 30);
                    lblBlock.Image = Properties.Resources.paddle;
                    lblBlock.Image
                        = blockImages[rand.Next() % blockImages.Count];
                    Controls.Add(lblBlock);
                }
        }

        /// <summary>
        /// 全ブログ打つしたら勝つ
        /// </summary>
        void win()
        {
            tm.Stop();
            Controls.Clear();
            MessageBox.Show("Congratulations, you win!");
            score = 0; lblScore.Text = "Score: 0";
            Controls.Add(lblScore);
            Intialize();
        }

        /// <summary>
        /// ボールのXポジションを設定
        /// </summary>
        /// <param name="c"></param>
        void caldx(Control c)
        {
            dx = c.Left + c.Width / 2 < _ball.Left ? -Math.Abs(dx) : Math.Abs(dx);
        }

        /// <summary>
        /// ボールのYポジションを設定
        /// </summary>
        /// <param name="c"></param>
        void caldy(Control c)
        {
            dy = c.Top + c.Height / 2 < _ball.Top ? -Math.Abs(dy) : Math.Abs(dy);
        }

        /// <summary>
        /// ブログのサイズを再設定
        /// </summary>
        /// <param name="images"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void blockResize(List<Bitmap> images, int width, int height)
        {
            Graphics graph;
            for (int i = 0; i < images.Count; i++)
            {
                Bitmap bmp = new Bitmap(width, height);
                graph = Graphics.FromImage(bmp);
                graph.DrawImage(images[i], 0, 0, width, height);
                graph.Dispose();
                images[i] = bmp;
            }
        }

        /// <summary>
        /// 命を表示するためハートを作成する
        /// </summary>
        void createHearts()
        {
            _hearts = new List<PictureBox>();
            for (int i = 0; i < 3; i++)
            {
                PictureBox box = new PictureBox();
                box.Size = new Size(30, 30);
                box.Location = new Point(Width - (3 - i) * box.Width, 0);
                box.SizeMode = PictureBoxSizeMode.StretchImage;
                box.Image = Properties.Resources.heart;
                Controls.Add(box); _hearts.Add(box);
            }
        }
        #endregion
    }
}
