using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KZ_CA_1D
{
    public partial class Form1 : Form
    {
        Bitmap image1;
        int rule;
        int meshSize;
        int maxTime;
        int binRuleLength = 8;
        bool[] binRuleArray;
        int[,] universe;
        

        bool[] makeBinRuleArray(String binaryString, int destArraySize)
        {
            if(binaryString.Length > destArraySize)
            {
                //Zrobić tu errora
                label1.Text = "error";
                
            }
            else if (binaryString.Length < destArraySize)
            {
                int tmp = destArraySize - binaryString.Length;
                for (int i = 0; i < tmp; i++)
                {
                    binaryString = "0" + binaryString;
                }

            }
            return binaryString.Select(c => c == '1').ToArray(); ;

        }

        void initEvolution1D()
        {
            rule = (int)numericUpDownRule.Value;
            meshSize = (int)numericUpDownMeshSize.Value;
            maxTime = (int)numericUpDownTime.Value;
            universe = new int[maxTime, meshSize + 2];//+2 na warunki brzegowe

            binRuleArray = makeBinRuleArray(Convert.ToString(rule, 2), binRuleLength);
            universe[0, (meshSize + 2) / 2] = 1;//ustawienie początkowego punktu

        }

        void checkRule(int time, int cell)//otrzymuje krok czasowy i numer komórki
        {
            //for (int i = 0; i < binRuleLength; i++)
            //{


            //}
            if (universe[time - 1, cell - 1] == 1 && universe[time - 1, cell] == 1 && universe[time - 1, cell + 1] == 1) universe[time, cell] = Convert.ToInt32(binRuleArray[0]);
            else if (universe[time - 1, cell - 1] == 1 && universe[time - 1, cell] == 1 && universe[time - 1, cell + 1] == 0) universe[time, cell] = Convert.ToInt32(binRuleArray[1]);
            else if (universe[time - 1, cell - 1] == 1 && universe[time - 1, cell] == 0 && universe[time - 1, cell + 1] == 1) universe[time, cell] = Convert.ToInt32(binRuleArray[2]);
            else if (universe[time - 1, cell - 1] == 1 && universe[time - 1, cell] == 0 && universe[time - 1, cell + 1] == 0) universe[time, cell] = Convert.ToInt32(binRuleArray[3]);
            else if (universe[time - 1, cell - 1] == 0 && universe[time - 1, cell] == 1 && universe[time - 1, cell + 1] == 1) universe[time, cell] = Convert.ToInt32(binRuleArray[4]);
            else if (universe[time - 1, cell - 1] == 0 && universe[time - 1, cell] == 1 && universe[time - 1, cell + 1] == 0) universe[time, cell] = Convert.ToInt32(binRuleArray[5]);
            else if (universe[time - 1, cell - 1] == 0 && universe[time - 1, cell] == 0 && universe[time - 1, cell + 1] == 1) universe[time, cell] = Convert.ToInt32(binRuleArray[6]);
            else if (universe[time - 1, cell - 1] == 0 && universe[time - 1, cell] == 0 && universe[time - 1, cell + 1] == 0) universe[time, cell] = Convert.ToInt32(binRuleArray[7]);
        }


        void startEvolution1D()
        {

            int switch_on = comboBox1.SelectedIndex;
            switch (switch_on)
            {
                case 0://Periodyczny
                    for (int time = 1; time < maxTime; time++)//time = 0 to krok początkowy, zaczynamy od 1
                    {
                        universe[time - 1, 0] = universe[time - 1, meshSize]; //do pierwszego elementu tablicy(lewego brzegu) przypisujemy wartość ostatniej komórki uniwersum,
                        universe[time - 1, meshSize + 1] = universe[time - 1, 1];//do ostatniego elementu tablicy(prawego brzegu) przypisujemy wartość pierwszej komórki uniwersum,
                        for (int cell = 1; cell < meshSize + 1; cell++)//sprawdzić czy dobrze przechodzi
                        {
                            checkRule(time, cell);
                        }
                    }
                    break;
                case 1://Pochłaniający 0
                    for (int time = 1; time < maxTime; time++)//time = 0 to krok początkowy, zaczynamy od 1
                    {
                        for (int cell = 1; cell < meshSize + 1; cell++)//sprawdzić czy dobrze przechodzi
                        {
                            checkRule(time, cell);
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Size = new Size(1000,500);
            numericUpDownRule.Minimum = 0;
            numericUpDownRule.Maximum = 255;
            numericUpDownTime.Minimum = 5;
            numericUpDownTime.Maximum = 500;
            numericUpDownTime.Value = 500;
            numericUpDownMeshSize.Minimum = 50;
            numericUpDownMeshSize.Maximum = 1000;
            comboBox1.SelectedIndex = 0;
        }

        void drawUniverse()
        {     
            int scale = 10;
            for (int time = 0; time < maxTime; time++)
            {
                for (int cell = 1; cell < meshSize + 1; cell++)
                {
                    if (universe[time, cell] == 1) image1.SetPixel(cell - 1, time, Color.Black);
                    else image1.SetPixel(cell - 1, time, Color.Yellow);
                }
            }

            Bitmap picOriginal = image1;
            int wid = (int)(picOriginal.Width * scale);
            int hgt = (int)(picOriginal.Height * scale);
            Bitmap bm = new Bitmap(wid, hgt);

            using (Graphics gr = Graphics.FromImage(bm))
            {
                // No smoothing.
                gr.InterpolationMode = InterpolationMode.NearestNeighbor;

                Point[] dest =
                {
            new Point(0, 0),
            new Point(wid, 0),
            new Point(0, hgt),
        };
                Rectangle source = new Rectangle(
                    0, 0,
                    picOriginal.Width,
                    picOriginal.Height);
                gr.DrawImage(picOriginal,
                    dest, source, GraphicsUnit.Pixel);
            }
            image1 = bm;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            initEvolution1D();
            startEvolution1D();
            image1 = new Bitmap(meshSize, maxTime);
            drawUniverse();

            pictureBox1.Image = image1;
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            buttonStart.Text = "Koniec";

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
