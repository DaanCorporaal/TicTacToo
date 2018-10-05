using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace Monopoly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        #region Private Members

        /// <summary>
        /// holds the current results of cells in the active game 
        /// </summary>

        private MarkType[] mResults;

        /// <summary>
        /// True if it is the player 1's turn (X) of player 2's turn (O) 
        /// </summary>
        private bool mPlayer1Turn;

        /// <summary>
        /// True if the game has ended
        /// </summary>
        private bool mGameEnded;

        /// <summary>
        /// True if you play agaist a computer bot
        /// </summary>
        private bool mComputerAi = false;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            NewGame();
            
        }

        #endregion

        /// <summary>
        /// Start new game and clear all values to default values 
        /// </summary>
        private void NewGame()
        {
            // Create a new blank array of free cells
            mResults = new MarkType[9];

            for (var i = 0; i < mResults.Length; i++)
                mResults[i] = MarkType.Free;
            
            // Make sure Player 1 starts the game
            mPlayer1Turn = true;
            
            // Interate every button on the grid...
            GameField.Children.Cast<Button>().ToList().ForEach(button =>{
                // change background and content to default values
                button.Content = string.Empty;
                button.Background = Brushes.White;
                button.Foreground = Brushes.Blue;
            });            

            // Make sure the game hasn't finished
            mGameEnded = false;

            // Set Content computer ai button
            Computer.Content = mComputerAi ? "Computer!" : "2 Players!";
            
        }
        /// <summary>
        /// Check if ther is a winner of a 3 line straigt 
        /// </summary>
        private void CheckForWinner()
        {
            #region Horizontal wins
            // check for horizontal wins
            // row 0
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[1] & mResults[2]) == mResults[0])
            {
                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button0_0.Background = button1_0.Background = button2_0.Background = Brushes.Green;
            }
            // row 1
            if (mResults[3] != MarkType.Free && (mResults[3] & mResults[4] & mResults[5]) == mResults[3])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button0_1.Background = button1_1.Background = button2_1.Background = Brushes.Green;
            }
            // row 2
            if (mResults[6] != MarkType.Free && (mResults[6] & mResults[7] & mResults[8]) == mResults[6])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button0_2.Background = button1_2.Background = button2_2.Background = Brushes.Green;
            }
            #endregion

            #region vertical wins
            // check for vetical wins
            // collom 0
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[3] & mResults[6]) == mResults[0])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button0_0.Background = button0_1.Background = button0_2.Background = Brushes.Green;
            }
            // collom 1
            if (mResults[1] != MarkType.Free && (mResults[1] & mResults[4] & mResults[7]) == mResults[1])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button1_0.Background = button1_1.Background = button1_2.Background = Brushes.Green;
            }
            // collom 2
            if (mResults[2] != MarkType.Free && (mResults[2] & mResults[5] & mResults[8]) == mResults[2])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button2_0.Background = button2_1.Background = button2_2.Background = Brushes.Green;
            }
            #endregion

            #region diagonal wins
            // check fore diagonal wins
            // top left bottem right
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[4] & mResults[8]) == mResults[0])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button0_0.Background = button1_1.Background = button2_2.Background = Brushes.Green;
            }
            // top right bottem left
            if (mResults[2] != MarkType.Free && (mResults[2] & mResults[4] & mResults[6]) == mResults[2])
            {

                // Game ends 
                mGameEnded = true;

                // Highlight winning blocks in green 
                button2_0.Background = button1_1.Background = button0_2.Background = Brushes.Green;
            }
            #endregion

            // Check for no winner and full board 
            if (!mGameEnded)
                if (!mResults.Any(f => f == MarkType.Free))
                {
                    // Game ended 
                    mGameEnded = true;

                    // turn all cells orange
                    GameField.Children.Cast<Button>().ToList().ForEach(button => {
                        // change background to orange
                        button.Background = Brushes.Orange;
                    });
                }
        }
        /// <summary>
        /// Handles a button click event
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">the event of the click</param>
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            // start a new game after the game has finished
            if (mGameEnded){
                NewGame();
                return;
            }

            // cast the sender to a button
            var button = (Button)sender;

            // find the button position in the array
            var colom = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            var index = colom + (row * 3);

            // Dont do anythink if the cell already has somting in it
            if (mResults[index] != MarkType.Free)
                return;

            // Set the cell value based on witch player turn it is 
            mResults[index] = mPlayer1Turn ? MarkType.Cross : MarkType.Nought;

            // Set button text to the result 
            button.Content = mPlayer1Turn ? "X" : "O";

            // Change nought to red
            if (mPlayer1Turn)
                button.Foreground = Brushes.Red;

            // Check for a winner
            CheckForWinner();

            // toggle players turns 
            mPlayer1Turn ^= true;


            //Console.Beep();
            //Task.Delay(2000);
            //System.Threading.Thread.Sleep(1000);
            //for (int i = 0; i < 2; i++)
            //{
            //Console.WriteLine("Sleep for 2 seconds.");
            //Thread.Sleep(2000);
            

            //}

            // Check of playing against pc
            if ((!mPlayer1Turn) && (mComputerAi) && (!mGameEnded))
            {
                await Task.Delay(1000);
                Computer_Ai_Move();
                CheckForWinner();

                // toggle players turns 
                mPlayer1Turn ^= true;
            }
        }
        
        #region Computer_Ai
        /// <summary>
        /// List fucktion to move lerning
        /// </summary>
        private void Computer_Ai_Move()
        {
            bool move = false;

            // look fore tic tac to opportunitys
            move = Search_Win();
            if(move == false){
                move = Search_Block();
                if(move == false){
                    move = Check_middel();
                    if(move == false){
                        move = Check_SideMiddel();
                        if (move == false){
                            move = Check_Corner();
                        }
                    }
                }
            }
           
        }
        // check if ai can win
        private bool Search_Win()
        {
            #region Horizontal
            //Check row 0
            if (mResults[0] == MarkType.Free && (mResults[1] & mResults[2]) == MarkType.Nought)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[1] == MarkType.Free && (mResults[2] & mResults[0]) == MarkType.Nought)
            {
                mResults[1] = MarkType.Nought;
                button1_0.Content = "O";
                return true;
            }
            else if (mResults[2] == MarkType.Free && (mResults[0] & mResults[1]) == MarkType.Nought)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            //Check row 1
            else if (mResults[3] == MarkType.Free && (mResults[4] & mResults[5]) == MarkType.Nought)
            {
                mResults[3] = MarkType.Nought;
                button0_1.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[5] & mResults[3]) == MarkType.Nought)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[5] == MarkType.Free && (mResults[3] & mResults[4]) == MarkType.Nought)
            {
                mResults[5] = MarkType.Nought;
                button2_1.Content = "O";
                return true;
            }
            //Check row 2
            else if (mResults[6] == MarkType.Free && (mResults[7] & mResults[8]) == MarkType.Nought)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            else if (mResults[7] == MarkType.Free && (mResults[8] & mResults[6]) == MarkType.Nought)
            {
                mResults[7] = MarkType.Nought;
                button1_2.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free && (mResults[7] & mResults[6]) == MarkType.Nought)
            {
                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            #endregion
            #region Vertical 
            // Collom 0
            else if (mResults[0] == MarkType.Free && (mResults[3] & mResults[6]) == MarkType.Nought)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[3] == MarkType.Free && (mResults[6] & mResults[0]) == MarkType.Nought)
            {
                mResults[3] = MarkType.Nought;
                button0_1.Content = "O";
                return true;
            }
            else if (mResults[6] == MarkType.Free && (mResults[0] & mResults[3]) == MarkType.Nought)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            // Collom 1 
            else if (mResults[1] == MarkType.Free && (mResults[4] & mResults[7]) == MarkType.Nought)
            {
                mResults[1] = MarkType.Nought;
                button1_0.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[7] & mResults[1]) == MarkType.Nought)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[7] == MarkType.Free && (mResults[1] & mResults[4]) == MarkType.Nought)
            {
                mResults[7] = MarkType.Nought;
                button1_2.Content = "O";
                return true;
            }
            // collom 2 
            else if (mResults[2] == MarkType.Free && (mResults[5] & mResults[8]) == MarkType.Nought)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            else if (mResults[5] == MarkType.Free && (mResults[8] & mResults[2]) == MarkType.Nought)
            {
                mResults[5] = MarkType.Nought;
                button2_1.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free && (mResults[2] & mResults[5]) == MarkType.Nought)
            {

                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            #endregion
            #region Diagonal
            // left up right down
            else if (mResults[0] == MarkType.Free && (mResults[4] & mResults[8]) == MarkType.Nought)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[0] & mResults[8]) == MarkType.Nought)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free && (mResults[0] & mResults[4]) == MarkType.Nought)
            {
                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            // right up left down
            else if (mResults[2] == MarkType.Free && (mResults[4] & mResults[6]) == MarkType.Nought)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[6] & mResults[2]) == MarkType.Nought)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[6] == MarkType.Free && (mResults[4] & mResults[2]) == MarkType.Nought)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            #endregion
            else { return false; }
        }

        // check if ai can block plyer one
        private bool Search_Block()
        {
            #region Horizontal
            //Check row 0
            if (mResults[0] == MarkType.Free && (mResults[1] & mResults[2]) == MarkType.Cross)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[1] == MarkType.Free && (mResults[2] & mResults[0]) == MarkType.Cross)
            {
                mResults[1] = MarkType.Nought;
                button1_0.Content = "O";
                return true;
            }
            else if (mResults[2] == MarkType.Free && (mResults[0] & mResults[1]) == MarkType.Cross)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            //Check row 1
            else if (mResults[3] == MarkType.Free && (mResults[4] & mResults[5]) == MarkType.Cross)
            {
                mResults[3] = MarkType.Nought;
                button0_1.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[5] & mResults[3]) == MarkType.Cross)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[5] == MarkType.Free && (mResults[3] & mResults[4]) == MarkType.Cross)
            {
                mResults[5] = MarkType.Nought;
                button2_1.Content = "O";
                return true;
            }
            //Check row 2
            else if (mResults[6] == MarkType.Free && (mResults[7] & mResults[8]) == MarkType.Cross)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            else if (mResults[7] == MarkType.Free && (mResults[8] & mResults[6]) == MarkType.Cross)
            {
                mResults[7] = MarkType.Nought;
                button1_2.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free && (mResults[7] & mResults[6]) == MarkType.Cross)
            {
                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            #endregion
            #region Vertical 
            // Collom 0
            else if (mResults[0] == MarkType.Free && (mResults[3] & mResults[6]) == MarkType.Cross)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[3] == MarkType.Free && (mResults[6] & mResults[0]) == MarkType.Cross)
            {
                mResults[3] = MarkType.Nought;
                button0_1.Content = "O";
                return true;
            }
            else if (mResults[6] == MarkType.Free && (mResults[0] & mResults[3]) == MarkType.Cross)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            // Collom 1 
            else if (mResults[1] == MarkType.Free && (mResults[4] & mResults[7]) == MarkType.Cross)
            {
                mResults[1] = MarkType.Nought;
                button1_0.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[7] & mResults[1]) == MarkType.Cross)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[7] == MarkType.Free && (mResults[1] & mResults[4]) == MarkType.Cross)
            {
                mResults[7] = MarkType.Nought;
                button1_2.Content = "O";
                return true;
            }
            // collom 2 
            else if (mResults[2] == MarkType.Free && (mResults[5] & mResults[8]) == MarkType.Cross)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            else if (mResults[5] == MarkType.Free && (mResults[8] & mResults[2]) == MarkType.Cross)
            {
                mResults[5] = MarkType.Nought;
                button2_1.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free && (mResults[2] & mResults[5]) == MarkType.Cross)
            {

                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            #endregion
            #region Diagonal
            // left up right down
            else if (mResults[0] == MarkType.Free && (mResults[4] & mResults[8]) == MarkType.Cross)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[0] & mResults[8]) == MarkType.Cross)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free && (mResults[0] & mResults[4]) == MarkType.Cross)
            {
                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            // right up left down
            else if (mResults[2] == MarkType.Free && (mResults[4] & mResults[6]) == MarkType.Cross)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Free && (mResults[6] & mResults[2]) == MarkType.Cross)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[6] == MarkType.Free && (mResults[4] & mResults[2]) == MarkType.Cross)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            #endregion
            else { return false; }
        }

        // check if middel is taken 
        private bool Check_middel()
        {
            if (mResults[4] == MarkType.Free)
            {
                mResults[4] = MarkType.Nought;
                button1_1.Content = "O";
                return true;
            }
            else if (mResults[4] == MarkType.Cross)
            {
                Check_Corner();
                return true;
            }
            else { return false; }
        }

        // check for cornor is free
        private bool Check_Corner()
        {
            if (mResults[0] == MarkType.Free)
            {
                mResults[0] = MarkType.Nought;
                button0_0.Content = "O";
                return true;
            }
            else if (mResults[2] == MarkType.Free)
            {
                mResults[2] = MarkType.Nought;
                button2_0.Content = "O";
                return true;
            }
            else if (mResults[6] == MarkType.Free)
            {
                mResults[6] = MarkType.Nought;
                button0_2.Content = "O";
                return true;
            }
            else if (mResults[8] == MarkType.Free)
            {
                mResults[8] = MarkType.Nought;
                button2_2.Content = "O";
                return true;
            }
            else { return false; }
        }

        // check for side middel is free
        private bool Check_SideMiddel()
        {
            if (mResults[1] == MarkType.Free)
            {
                mResults[1] = MarkType.Nought;
                button1_0.Content = "O";
                return true;
            }
            else if (mResults[3] == MarkType.Free)
            {
                mResults[3] = MarkType.Nought;
                button0_1.Content = "O";
                return true;
            }
            else if (mResults[5] == MarkType.Free)
            {
                mResults[5] = MarkType.Nought;
                button2_1.Content = "O";
                return true;
            }
            else if (mResults[7] == MarkType.Free)
            {
                mResults[7] = MarkType.Nought;
                button1_2.Content = "O";
                return true;
            }
            else { return false; }
        }
        #endregion
        
        /// <summary>
        /// Connection to database to save data to database
        /// </summary>
        private void ConnectionDB()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;database=test;username=root;password=";
            MySqlConnection DB_Connection = new MySqlConnection(connectionString);

        }
        /// <summary>
        /// Change second player to computer ai and player 2
        /// </summary>
        /// <param name="sender">The button thad was clicked</param>
        /// <param name="e">the event that was chlicked</param>
        private void Computer_Ai(object sender, RoutedEventArgs e)
        {

            mComputerAi ^= true;
            Computer.Content = mComputerAi ? "Computer!" : "2 Players!";
        }
    }
}
