using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

/// <summary>
/// Graeme Cook
/// March 11, 2021
/// ICS3U
/// 
/// A program that mimics an airhockey table.
/// </summary>

namespace AirHockey
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Call the function that resets all on-screen objects
            ObjectReset();
        }

        //Initialization of all variables, graphics objects, random generators, etc.
        #region initialization
        //Misc variables
        int randMax = 10;
        int centerX = 225;
        int centerY = 325;

        string winner;

        //Player one initialization
        int player1X;
        int player1Y = 162;
        int p1Score = 0;

        //Player two initialization
        int player2X;
        int player2Y = 462;
        int p2Score = 0;

        //Mallet specs
        int malletSpeed = 9;
        int malletRadius = 46;

        //Puck initialization & specs
        int puckX;
        int puckY;

        int puckXSpeed = 0;
        int puckYSpeed = 0;

        int puckRadius = 20;

        //Player one key initialization
        bool wDown = false;
        bool aDown = false;
        bool sDown = false;
        bool dDown = false;

        //Player two key initialization
        bool upDown = false;
        bool downDown = false;
        bool leftDown = false;
        bool rightDown = false;

        //Pens
        new Pen thinBluePen = new Pen(Color.Blue, 6);
        new Pen thickBluePen = new Pen(Color.Blue, 12);
        new Pen redPen = new Pen(Color.Red, 6);

        //Brushes
        new SolidBrush blueBrush = new SolidBrush(Color.DodgerBlue);
        new SolidBrush redBrush = new SolidBrush(Color.Red);
        new SolidBrush blackBrush = new SolidBrush(Color.Black);

        //Random number generator
        Random randGen = new Random();

        //Stopwatches
        Stopwatch xWatch = new Stopwatch();
        Stopwatch yWatch = new Stopwatch();

        //Soundplayers
        SoundPlayer goalPlayer = new SoundPlayer(Properties.Resources.goal);
        SoundPlayer bouncePlayer = new SoundPlayer(Properties.Resources.bruh);
        #endregion

        //Where all key ups & downs are registered (W, A, S, D, Up, Right, Down, Left)
        #region keys
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //If a key is pressed, set the corresponding keyDown variable to true, in order to receive input
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.Up:
                    upDown = true;
                    break;
                case Keys.Down:
                    downDown = true;
                    break;
                case Keys.Left:
                    leftDown = true;
                    break;
                case Keys.Right:
                    rightDown = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //When the keys are released, track that they've been so, so that input can be stopped
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
                case Keys.Up:
                    upDown = false;
                    break;
                case Keys.Down:
                    downDown = false;
                    break;
                case Keys.Left:
                    leftDown = false;
                    break;
                case Keys.Right:
                    rightDown = false;
                    break;
            }
        }
        #endregion

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //Where movement of the mallets and puck is processed
            #region movement
            //Player 1 
            //If a key is pressed & the mallet is within the screen bounds: move it (both players) works in combinations
            if (wDown == true && player1Y > 0) { player1Y -= malletSpeed; }
            if (aDown == true && player1X > 0) { player1X -= malletSpeed; }
            if (sDown == true && player1Y < centerY - malletRadius) { player1Y += malletSpeed; }
            if (dDown == true && player1X < this.Width - malletRadius) { player1X += malletSpeed; }

            //Player 2 
            if (upDown == true && player2Y > centerY) { player2Y -= malletSpeed; }
            if (downDown == true && player2Y < this.Height - malletRadius) { player2Y += malletSpeed; }
            if (leftDown == true && player2X > 0) { player2X -= malletSpeed; }
            if (rightDown == true && player2X < this.Width - malletRadius) { player2X += malletSpeed; }

            //Puck
            //Add the current speed to the puck x and y
            puckX += puckXSpeed;
            puckY += puckYSpeed;

            //Grab a random number
            int randSpeed = randGen.Next(7, randMax);

            //If the puck hits a boundary, give it a random trajectory outwards, and play a sound
            if (puckY < 0) 
            {
                puckYSpeed = randSpeed;

                bouncePlayer.Play();
            }
            else if (puckY > this.Height - puckRadius) 
            {
                puckYSpeed = -randSpeed;

                bouncePlayer.Play();
            }
            else if (puckX < 0) 
            {
                puckXSpeed = randSpeed;

                bouncePlayer.Play();
            }
            else if (puckX > this.Width - puckRadius) 
            {
                puckXSpeed = -randSpeed;

                bouncePlayer.Play();
            }
            #endregion

            //Where hitboxes are defined
            #region hitBoxes
            //Draw a rectangle around the two mallets and the puck, define it as an object
            Rectangle player1Box = new Rectangle(player1X, player1Y, malletRadius, malletRadius);
            Rectangle player2Box = new Rectangle(player2X, player2Y, malletRadius, malletRadius);
            Rectangle puckBox = new Rectangle(puckX, puckY, puckRadius, puckRadius);
            #endregion

            //Where player to puck contact is checked for and processed
            #region MalletHitDetection
            //Player 1
            if (player1Box.IntersectsWith(puckBox))
            {
                //If a WASD or arrow key is pressed, check for combo presses, move the puck in a vector corresponding to the mallet's vector, and move the puck away from the mallet to prevent bouncing (same patter applies throughout, for both players)
                if (wDown == true)
                {
                    //W diagonal vectors
                    if (dDown == true) 
                    { 
                        puckYSpeed = -malletSpeed;
                        puckXSpeed = malletSpeed;
                        
                        puckY = player1Y - malletRadius - 1;
                        puckX = player1X + malletRadius + 1;
                    }
                    else if (aDown == true) 
                    { 
                        puckYSpeed = -malletSpeed; 
                        puckXSpeed = -malletSpeed;

                        puckY = player1Y - malletRadius - 1;
                        puckX = player1X - malletRadius - 1;
                    }
                    //W linear vectors
                    else 
                    {
                        puckYSpeed = -malletSpeed;
                        puckY = player1Y - malletRadius - 1;
                    }
                }
                else if (sDown == true)
                {
                    //S diagonal vectors
                    if (dDown == true) 
                    { 
                        puckYSpeed = malletSpeed; 
                        puckXSpeed = malletSpeed;

                        puckY = player1Y + malletRadius + 1;
                        puckX = player1X + malletRadius + 1;
                    }
                    if (aDown == true) 
                    { 
                        puckYSpeed = malletSpeed; 
                        puckXSpeed = -malletSpeed;

                        puckY = player1Y + malletRadius + 1;
                        puckX = player1X - malletRadius - 1;
                    }
                    //S linear vectors
                    else 
                    { 
                        puckYSpeed = malletSpeed;
                        puckY = player1Y + malletRadius + 1;
                    }
                }
                //A & D linear vectors
                else if (dDown == true) 
                { 
                    puckXSpeed = malletSpeed;
                    puckX = player1X + malletRadius + 1;
                }
                else if (aDown == true) 
                { 
                    puckXSpeed = -malletSpeed;
                    puckX = player1X - malletRadius - 1;
                }
                else 
                { 
                    puckXSpeed = -puckXSpeed; 
                    puckYSpeed = -puckYSpeed; 
                }
            }

            //Player 2
            if (player2Box.IntersectsWith(puckBox))
            {
                if (upDown == true)
                {
                    //Up diagonal vectors
                    if (rightDown == true) 
                    {
                        puckYSpeed = -malletSpeed; 
                        puckXSpeed = malletSpeed;

                        puckY = player2Y - malletRadius - 1;
                        puckX = player2X + malletRadius + 1;
                    }
                    else if (leftDown == true) 
                    {
                        puckYSpeed = -malletSpeed;
                        puckXSpeed = -malletSpeed;

                        puckY = player2Y - malletRadius - 1;
                        puckX = player2X - malletRadius - 1;
                    }
                    //Up linear vectors
                    else 
                    {
                        puckYSpeed = -malletSpeed;
                        puckY = player2Y - malletRadius - 1;
                    }
                }
                else if (downDown == true)
                {
                    //Down diagonal vectors
                    if (rightDown == true)
                    {
                        puckYSpeed = malletSpeed; 
                        puckXSpeed = malletSpeed;

                        puckY = player2Y + malletRadius + 1;
                        puckX = player2X + malletRadius + 1;
                    }
                    if (leftDown == true) 
                    {
                        puckYSpeed = malletSpeed;
                        puckXSpeed = -malletSpeed;

                        puckY = player2Y + malletRadius + 1;
                        puckX = player2X - malletRadius - 1;
                    }
                    //Down linear vectors
                    else 
                    { 
                        puckYSpeed = malletSpeed;
                        puckY = player2Y + malletRadius + 1;
                    }
                }
                //Left & Right linear vectors
                else if (rightDown == true) 
                {
                    puckXSpeed = malletSpeed;
                    puckX = player2X + malletRadius + 1;
                }
                else if (leftDown == true) 
                {
                    puckXSpeed = -malletSpeed;
                    puckX = player2X - malletRadius - 1;
                }
                else 
                {
                    puckXSpeed = -puckXSpeed; 
                    puckYSpeed = -puckYSpeed;
                }
            }
            #endregion

            //Where the puck is checked to see if it is in a "net" and corresponding processes
            #region scoring
            //Top
            //If the puck is in one of the nets, play a sound, add to the score & output it, reset the mallets & puck (applies for top & bottom).
            if (puckY < 0 && puckX > 149 && puckX < 301)
            {
                goalPlayer.Play();

                p2Score++;
                p2ScoreOutput.Text = $"Player 2: {p2Score}";

                ObjectReset();
            }
            //Bottom
            if (puckY >= this.Height - puckRadius && puckX > 149 && puckX < 301)
            {
                goalPlayer.Play();

                p1Score++;
                p1ScoreOutput.Text = $"Player 1: {p1Score}";

                ObjectReset();
            }
            #endregion

            //Where the puck is gradually slowed down over time
            #region puckSlowdown
            //If the pucks speed is not 0, slow it down by one every half-second.
            if (puckYSpeed != 0)
            {
                xWatch.Start();
                if (xWatch.ElapsedMilliseconds > 500)
                {
                    if (puckYSpeed > 0)
                    {
                        puckYSpeed--;
                        xWatch.Restart();
                    }
                    else
                    {
                        puckYSpeed++;
                        xWatch.Restart();
                    }
                }
            }
            if (puckXSpeed != 0)
            {
                yWatch.Start();
                if (yWatch.ElapsedMilliseconds > 500)
                {
                    if (puckXSpeed > 0)
                    {
                        puckXSpeed--;
                        yWatch.Restart();
                    }
                    else
                    {
                        puckXSpeed++;
                        yWatch.Restart();
                    }
                }
            }
            #endregion

            //Checks if either player has three points, and ends the game if either does.
            if (p1Score == 3 || p2Score == 3) { GameOver(); }

            //Redraw the screen
            Refresh();
        }

        private void GameOver()
        {
            //Where the winner of the match is determined
            #region determineWinner
            //Whoever has three points wins
            if (p1Score == 3) { winner = "Player 1"; }
            else { winner = "Player 2"; }

            //Display winner
            gameOverLabel.Text = $"{winner} wins!";
            #endregion

            //Where the end menu screen is displayed
            #region endScreen
            //Stop the game
            gameTimer.Enabled = false;

            //Make the menu & buttons visible
            gameOverLabel.Visible = true;
            exitButton.Visible = true;
            resetButton.Visible = true;

            //Enable the buttons
            exitButton.Enabled = true;
            resetButton.Enabled = true;
            #endregion
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Where the rink is drawn
            #region rinkDrawing
            //Centerline
            e.Graphics.DrawLine(redPen, 0, centerY, 450, centerY);

            //Center circle
            e.Graphics.DrawEllipse(thinBluePen, centerX - 38, centerY - 38, 76, 76);

            //Icing lines
            e.Graphics.DrawLine(thickBluePen, 0, 175, 450, 175);
            e.Graphics.DrawLine(thickBluePen, 0, 475, 450, 475);

            //End lines
            e.Graphics.DrawLine(redPen, 0, 3, 450, 3);
            e.Graphics.DrawLine(redPen, 0, 647, 450, 647);

            //Goal creases
            e.Graphics.DrawArc(redPen, centerX - 76, -76, 152, 152, 0, 180);
            e.Graphics.DrawArc(redPen, centerX - 76, 578, 152, 152, 180, 360);
            #endregion

            //Where the dynamic objects are drawn
            #region objectDrawing
            //Players
            e.Graphics.FillEllipse(blueBrush, player1X, player1Y, malletRadius, malletRadius);
            e.Graphics.FillEllipse(redBrush, player2X, player2Y, malletRadius, malletRadius);

            //Puck
            e.Graphics.FillEllipse(blackBrush, puckX, puckY, puckRadius, puckRadius);
            #endregion
        }

        private void ObjectReset()
        {
            //Where the dynamic objects are initialized
            #region dynamicInitialiaztion
            //Set the players and puck to the start positions, no matter the screen size or radius of the objects
            //Players
            player1X = centerX - malletRadius / 2;
            player1Y = 175 - puckRadius;

            player2X = centerX - malletRadius / 2;
            player2Y = 475 - puckRadius;

            //Puck
            puckX = centerX - puckRadius / 2;
            puckY = centerY - puckRadius / 2;

            //Set the puck speeds to zero
            puckXSpeed = 0;
            puckYSpeed = 0;
            #endregion
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            //Where the game is reset, on button press
            #region reset
            //Scores to zero
            p1Score = 0;
            p2Score = 0;

            //Outputs reset
            p1ScoreOutput.Text = "Player 1: 0";
            p2ScoreOutput.Text = "Player 2: 0";

            //Timer re-enabled
            gameTimer.Enabled = true;

            //Dynamic objects reset
            ObjectReset();

            //Buttons re-disabled
            resetButton.Enabled = false;
            exitButton.Enabled = false;

            //Game over menu & buttons made invisible again
            resetButton.Visible = false;
            exitButton.Visible = false;
            gameOverLabel.Visible = false;
            #endregion
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            //End the game on exit button click/press
            Application.Exit();
        }
    }
}