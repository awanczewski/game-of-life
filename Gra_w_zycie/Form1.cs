using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Gra_w_zycie
{
    public partial class Form1 : Form
    {
        
        private const int size = 100;
        private int[,] board = new int[size, size];
        private int[,] tempBoard = new int[size, size];
        private int cellSize = 1000 / size;
        private bool isStarted = false; //determines whether an infinite loop with time interval is started
     
        private Pen p = new Pen(Color.Black);
        private SolidBrush b = new SolidBrush(Color.Blue);
        public Data data = new Data(); //creating an object of Data class that i use for data serialization
        private StreamReader sr = null;
        private StreamWriter sw = null;



        public Form1()
        {
            InitializeComponent();
            initializeBoard();
        }

        private void drawGrid(Graphics g) // draws lines, vertically and horizontally. size + 1 to draw right and bottom border lines.
        {
            for(int i = 0; i < size + 1; i++)
            {                                
                g.DrawLine(p, i * cellSize, 0, i * cellSize, size * cellSize);
                g.DrawLine(p, 0, i * cellSize, size  * cellSize, i * cellSize);
            }
        }

        public void initializeBoard()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = 0;
                }
            }
        }

        private void makeStep() // making one step. Using tempBoard so that the change in the state of life of the earlier cell does not affect the number of neighbours of the next cell.
        {                       //it should affect only after the move.
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    tempBoard[i, j] = board[i, j];
                    checkNumOfNeighbours(i, j);
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = tempBoard[i, j];
                }
            }
            data.numberOfSteps++;
            label1.Text = data.numberOfSteps.ToString();
        }

        private void fillCell(int i, int j, Graphics g) // fills the rectangle with color of solidbrush
        {
            g.FillRectangle(b, j * cellSize, i * cellSize, cellSize, cellSize);      
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            drawGrid(e.Graphics);

            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if(board[i, j] == 1) // only if the cell is "alive" i fill that point with color.
                    {
                        fillCell(i, j, e.Graphics);
                    }
                }
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e) // after clicking the mouse i acquire which index suits this place.
        {

            int cordX = 0, cordY = 0;
            for(int i = 0; i < size - 1; i++)
            {
                if((e.Y >= (i * cellSize)) && (e.Y <= ((i + 1) * cellSize)))
                {
                    cordY = i;
                    break;
                }
            }

            for (int j = 0; j < size - 1; j++)
            {
                if ((e.X >= (j * cellSize)) && (e.X <= ((j + 1) * cellSize)))
                {
                    cordX = j;
                    break;
                }
            }
            if(board[cordY, cordX] == 1) // after clicking on the live cell, make it dead.
            {
                board[cordY, cordX] = 0;
            }
            else
            {
                board[cordY, cordX] = 1;
            }
            panel1.Refresh();
        }

        public void checkNumOfNeighbours(int i, int j)
        {
            int num = 0;

            if (i != 0 && j != 0) // check if i'm not going beyond the range of the array
            {
                num += board[i - 1, j - 1];
            }
            if (i != 0)
            {
                num += board[i - 1, j];
            }
            if (j != 0)
            {
                num += board[i, j - 1];
            }
            if (i != 99 && j != 99)
            {
                num += board[i + 1, j + 1];
            }
            if (i != 99)
            {
                num += board[i + 1, j];
            }
            if (j != 99)
            {
                num += board[i, j + 1];
            }
            if (i != 99 && j != 0)
            {
                num += board[i + 1, j - 1];
            }
            if (i != 0 && j != 99)
            {
                num += board[i - 1, j + 1];
            }

            if (num < 2) //dies by underpopulation
            {
                tempBoard[i, j] = 0;
            }
            else if (num > 3) //dies by overpopulation
            {
                tempBoard[i, j] = 0;
            }
            else if (num == 3) //becomse a live cell by reproduction
            {
                tempBoard[i, j] = 1;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            makeStep();
            panel1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        { 
            if(isStarted)  
            {
                timer1.Stop();
                button5.Text = "START";
                isStarted = false;
            }
            else
            {
                timer1.Start();
                button5.Text = "STOP";
                isStarted = true;
            }
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            data.sleepTime = (int)numericUpDown1.Value;
            timer1.Interval = data.sleepTime;
        }

        private void changeColor(int red, int green, int blue)
        {//using r g b because the brush or color is not serializable.
            data.r = red;
            data.g = green;
            data.b = blue;
            b.Color = Color.FromArgb(data.r, data.g, data.b);
        }

        private void button6_Click(object sender, EventArgs e) //RED
        {
            changeColor(255, 0, 0);
        }

        private void button7_Click(object sender, EventArgs e) //ORANGE
        {
            changeColor(255, 165, 0);
        }

        private void button8_Click(object sender, EventArgs e) //YELLOW
        {
            changeColor(255, 255, 0);
        }

        private void button9_Click(object sender, EventArgs e) //GREEN
        {
            changeColor(0, 128, 0);
        }

        private void button10_Click(object sender, EventArgs e) //BLUE
        {
            changeColor(0, 0, 255);
        }

        private void button11_Click(object sender, EventArgs e) //PURPLE
        {
            changeColor(128, 0, 128);
        }

        private void button12_Click(object sender, EventArgs e) //PINK
        {
            changeColor(255, 192, 203);
        }

        private void button13_Click(object sender, EventArgs e) //OLIVE
        {
            changeColor(128, 128, 0);
        }

        private void button14_Click(object sender, EventArgs e) //Cleans the board
        {
            initializeBoard();
            data.numberOfSteps = 0;
            label1.Text = data.numberOfSteps.ToString();
            panel1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            makeStep();
            makeStep();
            panel1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 5; i++)
            {
                makeStep();
            }
            panel1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e) 
        {
            for (int i = 0; i < 10; i++)
            {
                makeStep();
            }
            panel1.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //setting all controls as they were before closing the program
            try
            {
                sr = new StreamReader("data.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(Data));
                data = (Data)serializer.Deserialize(sr);

                label1.Text = data.numberOfSteps.ToString();
                numericUpDown1.Value = data.sleepTime;
                b.Color = Color.FromArgb(data.r, data.g, data.b);

                for(int i = 0; i < size; i++) // after form is loaded I get data from an serialized jagged array and assigns that data to a multidimensional array, because multidimensional is not supported in xml serialization.
                {
                    for(int j = 0; j < size; j++)
                    {
                        board[i, j] = data.board[i][j]; 
                    }
                }
            }
            catch(Exception err)
            {
                Text = err.ToString();
            }
            finally
            {
                sr.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
         
            for(int i = 0; i < size; i++) // saving board data to serialized jagged array so after form is loaded next time i have the same board as before closing
            {
                data.board[i] = new int[size]; 
                for(int j = 0; j < size; j++)
                {
                    data.board[i][j] = board[i, j];
                }
            }
       

            try
            {
                StreamWriter sw = new StreamWriter("data.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(Data));
                serializer.Serialize(sw, data);
                sw.Close();
            }
            catch(Exception err)
            {
                Text = err.ToString();
            }
            finally
            {
                if(sw != null)
                {
                    sw.Close();
                }
            }
        }
        // at first i used backgroundworker but then i realized that timer is much easier and better to make it done, although backgroundworker is an option too
        private void timer1_Tick(object sender, EventArgs e) // executes that method every single interval of time when timer is started.
        {
            makeStep();
            panel1.Refresh();
        }
    }

    public class Data
    {
        public int sleepTime = 1000;
        public int numberOfSteps = 0;
        public int[][] board = new int[100][];
        public int r = 0;
        public int g = 0;
        public int b = 255;
    }
}
