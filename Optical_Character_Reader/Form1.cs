using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Optical_Character_Reader
{
    public partial class Form1 : Form
    {
        int total = 0;
        int[] secttotal = new int[8];
        int subsect = 0;
        int subtrac = 0;
        int[] tracktotal = new int[5];
        public Form1()
        {
            InitializeComponent();
        }

        int cntr = 0;                   // variable to count black pixel
        int[,] arr = new int[15, 15];   // used to store data for blocks to compare with tamplates

        int[,] matcharr = new int[8, 5];
        private void btnClr_Click(object sender, EventArgs e)
        {
           for (int i = 0; i < 15; i++)                // pixels rows
            {
                for (int j = 0; j < 15; j++)            // pixels columns
                {
                    arr[i, j] = 0;
                }
            }
            richTextBox1.Text = "";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matcharr[i, j] = 0;
                }
            }
            richTextBox2.Text = "";
            total = 0;
            subsect = 0;
            subtrac = 0;
        }

        // function used to count black pixel in a block
        private void btnPixelate_Click(object sender, EventArgs e)
        {
            int flag = 0, flag2 = 0;
            int count2 = 0;
            Color a=new Color();
            Color b=new Color();
            Color c=new Color();
            Color d=new Color();
            Bitmap bmp_proc = new Bitmap(pictureBox1.Image);    // new bitmap to save the image from picturebox
            
            for(int l = 0; l < 15; l++)                         // blocks rows
            {
                flag2 = 0;
                count2 = 0;
                for (int k = 0; k < 15; k++)                    // blocks columns
                {
                    if (flag2 == 0)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            a = bmp_proc.GetPixel((l * 20) + i, (k * 20) + 3);
                            b = bmp_proc.GetPixel((l * 20) + i, (k * 20) + 6);
                            c = bmp_proc.GetPixel((l * 20) + i, (k * 20) + 3);
                            d = bmp_proc.GetPixel((l * 20) + i, (k * 20) + 13);
                            if ((a.R < 100 && a.G < 100 && a.B < 100) || (b.R < 100 && b.G < 100 && b.B < 100) || (c.R < 100 && c.G < 100 && c.B < 100) || (d.R < 100 && d.G < 100 && d.B < 100))
                            {
                                count2++;
                                if (count2 > 10)
                                {
                                    flag2 = 1;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag2 == 1)
                    {
                        for (int i = 0; i < 20; i++)                // pixels rows
                        {
                            for (int j = 0; j < 20; j++)            // pixels columns
                            {
                                // read pixel and store its color (black or white)
                                Color orgnClr = bmp_proc.GetPixel((l * 20) + i, (k * 20) + j);
                                textBox1.Text = Convert.ToString(orgnClr);  // show data in textbox

                                if (orgnClr == Color.FromArgb(0, 0, 0))     // if color is black
                                {
                                    //arr[l, k] = 1;
                                    cntr++;                                 // increment balck pixel counter
                                    // as one block contains 400 pixels cause our block size is 20x20 pixels
                                    // so we will check atleast half region of block should be filled with black color
                                    if (cntr > 199)
                                    {
                                        arr[k, l] = 1;  // if total pixels are 200 or more than that then array will store value 1 for respective block
                                        flag = 1;
                                        break;
                                    }
                                    else
                                    {
                                        flag2 = 0;
                                    }
                                    if (flag == 1)
                                        break;
                                }

                                if (flag == 1)
                                    break;
                            }

                            if (flag == 1)
                                break;
                        }
                        cntr = 0;   // counter variable reset
                        flag = 0;
                    }
                }
            }
            int[] ari=new int[300];
            int[] arj=new int[300];
            int pi=0;
            for (int i = 1; i < 14; i++)
            {
                for (int j = 1; j < 14; j++)
                {
                    if ((arr[i, j] == 1 && arr[i + 1, j] == 1 && arr[i - 1, j] == 1 && arr[i, j - 1] == 1 && arr[i, j + 1] == 1 && arr[i + 1, j + 1] == 1 && arr[i + 1, j - 1] == 1 && arr[i - 1, j + 1] == 1 && arr[i - 1, j - 1] == 1) || (arr[i, j] == 1 && arr[i + 1, j] == 1 && arr[i - 1, j] == 1 && arr[i, j - 1] == 1 && arr[i, j + 1] == 1 && arr[i + 1, j + 1] == 1 && arr[i + 1, j - 1] == 1 && arr[i - 1, j + 1] == 1 && arr[i - 1, j - 1] == 0) || (arr[i, j] == 1 && arr[i + 1, j] == 1 && arr[i - 1, j] == 1 && arr[i, j - 1] == 1 && arr[i, j + 1] == 1 && arr[i + 1, j + 1] == 0 && arr[i + 1, j - 1] == 1 && arr[i - 1, j + 1] == 1 && arr[i - 1, j - 1] == 1) || (arr[i, j] == 1 && arr[i + 1, j] == 1 && arr[i - 1, j] == 1 && arr[i, j - 1] == 1 && arr[i, j + 1] == 1 && arr[i + 1, j + 1] == 1 && arr[i + 1, j - 1] == 1 && arr[i - 1, j + 1] == 0 && arr[i - 1, j - 1] == 1) || (arr[i, j] == 1 && arr[i + 1, j] == 1 && arr[i - 1, j] == 1 && arr[i, j - 1] == 1 && arr[i, j + 1] == 1 && arr[i + 1, j + 1] == 1 && arr[i + 1, j - 1] == 0 && arr[i - 1, j + 1] == 1 && arr[i - 1, j - 1] == 1))
                    {
                        ari[pi] = i;
                        arj[pi] =j;
                        pi++;
                    }
                }
            }
            for (int i = 0; i < pi; i++)
            {
                arr[ari[i], arj[i]] = 0;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add ("Ariel");
            comboBox1.Items.Add("Times new roman");
            comboBox1.Items.Add("Ariel Black");

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    arr[i, j] = 0;              // initial values should be zero in all elements of an array
                }
            }
        }

        // used to display the data what we extracted from image
        private void btnReadPxl_Click(object sender, EventArgs e)
        {
            int lo=0;
            richTextBox1.Text = "";         // clear the richtextbox
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    richTextBox1.AppendText(Convert.ToString(arr[i, j]) + "  ");    // Add the element as a column in single row
                }
                richTextBox1.AppendText("\n");  // change the line or next row
            }
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (arr[i,j]==1)
                        lo++;
                }
            }
            richTextBox1.AppendText(lo.ToString());
        }

        Bitmap bmp;     // new bitmap object to read the picture file
        public static Image ScaleImage(Image img, int width, int height)
        {
            var newimg = new Bitmap(img, 300, 300);
            var cropped = new Bitmap(300, 300);
            int x1=0,y1=0,x2=0,y2=0,f=0;
            Graphics.FromImage(newimg).DrawImage(img, 0, 0,300,300);
            GraphicsUnit units=GraphicsUnit.Pixel;

            for (int i = 0; i < newimg.Width; i++)          //greyscale 
            {
                for (int j = 0; j < newimg.Height; j++)
                {
                    
                    if (newimg.GetPixel(i, j).R <= 100 && newimg.GetPixel(i, j).G <= 100 && newimg.GetPixel(i, j).G <= 100)
                    {

                        newimg.SetPixel(i, j, Color.Black);
                    }
           
                    Color a = newimg.GetPixel(i, j);
                    double l = 0.216 * a.R + 0.712 * a.G + 0.0722 * a.B;
                    newimg.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(l), Convert.ToInt32(l), Convert.ToInt32(l)));


                }
            }
            for (int i = 0; i < newimg.Width; i++)              
            {
                for (int j = 0; j < newimg.Height; j++)
                {
                    if (newimg.GetPixel(i,j).R<30&&newimg.GetPixel(i,j).G <30&&newimg.GetPixel(i,j).B<30) 
                    {
                        x1 = i;
                        f = 1;
                        break;
                    }
                }
                if (f==1)
                    break;
            }
            f=0;
                for (int j = 0; j < newimg.Height; j++)
                {
                    for (int i = 0; i < newimg.Width; i++)
                    {
                        if ((newimg.GetPixel(i, j).R < 30 && newimg.GetPixel(i, j).G < 30 && newimg.GetPixel(i, j).B < 30))
                        {
                            y1 = j;
                            f=1;
                            break;
                        }
                    }
                    if(f==1)
                        break;
                }

                f = 0;
                for (int j = newimg.Height-1; j >=0 ; j--)
                {
                    for (int i = newimg.Width-1; i >=0; i--)
                    {
                        if (newimg.GetPixel(i, j).R < 30 && newimg.GetPixel(i, j).G < 30 && newimg.GetPixel(i, j).B < 30)
                        {
                            y2 = j;
                            
                            f = 1;
                            break;
                        }
                    }
                    if (f == 1)
                        break;
                }
                f = 0;
               for (int j = newimg.Width - 1; j >= 0; j--)
                {
                    for (int i = newimg.Height - 1; i >= 0; i--)
                    {
                        if (newimg.GetPixel(j,i).R < 30 && newimg.GetPixel(j,i).G < 30 && newimg.GetPixel(j,i).B < 30)
                        {
                            x2 = j;
                            f = 1;
                            break;
                        }
                    }
                    if (f == 1)
                        break;
                }
            Rectangle rectcrop = new Rectangle(x1,y1,x2-x1,y2-y1);
            Rectangle destrecta=new Rectangle(0,0,300,300);
            Graphics.FromImage(cropped).DrawImage(newimg, 0, 0, rectcrop,units);

          //  Bitmap newImage = new Bitmap(newWidth, newHeight);
         /*   using (Graphics gr = Graphics.FromImage(cropped))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(newimg, new Rectangle(0, 0, 300, 300));
            }*/

            Graphics.FromImage(newimg).DrawImage(cropped, destrecta, 0, 0, x2 - x1, y2 - y1, units);

            return newimg;
        }
        // this function allows user to browse & scale required image file
        private void btnBrowse_Click(object sender, EventArgs e)
        {

            richTextBox1.Clear();
            richTextBox2.Clear();

            OpenFileDialog fl = new OpenFileDialog();       // file browser object

            // if user will select a required file then that file should be store in picturebox
            if (fl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var img = Image.FromFile(fl.FileName);
                var newimg = ScaleImage(img, 300, 300);
                bmp = new Bitmap(newimg); // get filename and then save it as bitmap image
               
                pictureBox1.Image = bmp;            // pass this bitmap image to picturebox
            }
        }
           private void btnRecChar_Click(object sender, EventArgs e)
        {
            for (int l = 0; l < 8; l++)
            {
                subsect = 0;
                for (int m = 0; m < 5; m++)
                {
                    subsect = subsect + matcharr[l, m];
                    total = total + matcharr[l, m];

                }
                secttotal[l] = subsect;
            }
            for (int m = 0; m < 5; m++)
            {
                subtrac = 0;
                for (int l = 0; l < 8; l++)
                {
                    subtrac = subtrac + matcharr[l, m];
                }
                tracktotal[m] = subtrac;
            }
            
            
               try
            {
                MySqlConnection myconn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=root");
                MySqlCommand command = new MySqlCommand("select * from (select * from ( select * from charactersocr.char where total between " + (total - 15) + " and " + (total + 15) + " ) as x where ( sect1 between " + (secttotal[1] - 6) + " and " + (secttotal[1] + 6) + ") and ( sect2 between " + (secttotal[2] - 6) + " and " + (secttotal[2] + 6) + " ) and ( sect3 between " + (secttotal[3] - 6) + " and " + (secttotal[3] + 6) + ") and (sect4 between " + (secttotal[4] - 6) + " and " + (secttotal[4] + 6) + ") and (sect5  between " + (secttotal[5] - 6) + " and " + (secttotal[5] + 6) + ") and ( sect6 between " + (secttotal[6] - 6) + " and " + (secttotal[6] + 6) + ") and ( sect7 between " + (secttotal[7] - 6) + " and " + (secttotal[7] + 6) + ") and ( sect0 between " + (secttotal[0] - 6) + " and " + (secttotal[0] + 6) + "))as y where" /*( (track0 between " + (tracktotal[0] - 3) + " and " + (tracktotal[0] + 3) + ") or( track1 between " + (tracktotal[1] - 3) + " and " + (tracktotal[1] + 3) + ")) and( track2 between " + (tracktotal[2] - 3) + " and " + (tracktotal[2] + 3) + ") and( track3 between " + (tracktotal[3] - 3) + " and " + (tracktotal[3] + 3) + ") and( track4 between " + (tracktotal[4] - 3) + " and " + (tracktotal[4] + 3) + ") or ( track0 between " + (tracktotal[0] - 3) + " and " + (tracktotal[0] + 3) + ") and(( track1 between " + (tracktotal[1] - 3) + " and " + (tracktotal[1] + 3) + ") or ( track2 between " + (tracktotal[2] - 3) + " and " + (tracktotal[2] + 3) + ")) and( track3 between " + (tracktotal[3] - 3) + " and " + (tracktotal[3] + 3) + ") and( track4 between " + (tracktotal[4] - 3) + " and " + (tracktotal[4] + 3) + ") or ( track0 between " + (tracktotal[0] - 3) + " and " + (tracktotal[0] + 3) + ") and( track1 between " + (tracktotal[1] - 3) + " and " + (tracktotal[1] + 3) + ") and( (track2 between " + (tracktotal[2] - 3) + " and " + (tracktotal[2] + 3) + ") or ( track3 between " + (tracktotal[3] - 3) + " and " + (tracktotal[3] + 3) + ")) and( track4 between " + (tracktotal[4] - 9) + " and " + (tracktotal[4] + 3) + ") or ( track0 between " + (tracktotal[0] - 3) + " and " + (tracktotal[0] + 3) + ") and( track1 between " + (tracktotal[1] - 3) + " and " + (tracktotal[1] + 3) + ") and( track2 between " + (tracktotal[2] - 3) + " and " + (tracktotal[2] + 3) + ") and( ( track3 between " + (tracktotal[3] - 3) + " and " + (tracktotal[3] + 3) + ") or ( track4 between " + (tracktotal[4] - 3) + " and " + (tracktotal[4] + 3) + ")) or*/ +"( track0 between " + (tracktotal[0] - 3) + " and " + (tracktotal[0] + 3) + ") and( track1 between " + (tracktotal[1] - 3) + " and " + (tracktotal[1] + 3) + ") and( track2 between " + (tracktotal[2] - 3) + " and " + (tracktotal[2] + 3) + ") and( track3 between " + (tracktotal[3] - 3) + " and " + (tracktotal[3] + 3) + ") and( track4 between " + (tracktotal[4] - 3) + " and " + (tracktotal[4] + 3) + ")", myconn);
                myconn.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                   MessageBox.Show(reader.GetString("charec"));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i,j;
            int vcenter,hcenter;
            double rad = 0 ;
            double tl,rl,bl,ll;
            double track=0;
          /*  for (i = 0; i < 15; i++)
            {
                for (j = 0; j < 15; j++)
                {
                    if (arr[i, j] == 1)
                    {
                        topi = i;
                        topj = j;
                        break;
                    }
                    if (topi != 0)
                        break;
                }
            }
            for (i = 14; i >=0; i--)
            {
                for (j = 14; j >=0; j--)
                {
                    if (arr[i, j] == 1)
                    {
                        boti = i;
                        botj = j;
                        break;
                    }
                }
                if (boti != 0)
                    break;
            }
            for(j=0;j<15;j++)
                for(i=0;i<15;i++)
                {
                    if(arr[i,j]==1)
                    {
                        leftj = j;
                        lefti = i;
                        break;
                    }
                    if (leftj != 0)
                        break;
                }
            for(j=14;j>=0;j--)
                for (i = 0; i < 15; i++)
                {
                    if (arr[i, j] == 1)
                    {
                        rightj = j;
                        righti = i;
                        break;
                    }
                    if (rightj != 0)
                        break;
                }*/
          //  vcenter = (topi + boti) / 2;
          //  hcenter = (leftj + rightj) / 2;
            vcenter = 7;
            hcenter = 7;
            tl=Math .Sqrt (Math.Pow((vcenter-15),2)+Math.Pow((hcenter-15),2));
            rl=Math .Sqrt (Math.Pow((vcenter-0),2)+Math.Pow((hcenter-0),2));
            ll=Math .Sqrt (Math.Pow((vcenter-0),2)+Math.Pow((hcenter-15),2));
            bl=Math .Sqrt (Math.Pow((vcenter-15),2)+Math.Pow((hcenter-0),2));
            
            rad=Math.Max(Math .Max (tl,bl),Math.Max(ll,rl));
            track = rad / 5;
           
 for (int l = 0; l < 8; l++)
            {
                for (int m = 0; m < 5; m++)
                {
                    matcharr[l, m] = 0;
                }
            }
            
          //  MessageBox.Show(rad.ToString()+"   "+hcenter.ToString() + "   " +vcenter.ToString()+"   "+topi.ToString()+"  "+topj.ToString()+"  "+boti.ToString()+"  "+botj.ToString()+"  "+lefti.ToString()+"  "+leftj.ToString()+"  "+righti.ToString()+"  "+rightj.ToString());
         // MessageBox.Show(hcenter.ToString() + "  " + vcenter.ToString()+"  " );
            
           for (i=0 ;i<=14 ;i++)
           {
               for(j=0 ;j<=14 ;j++)
               {
                   if (arr[i, j] == 1)
                   {
                     int sectornum = thetacal(i, j, hcenter, vcenter);
                     int tracknum = traccal(i, j, track, hcenter, vcenter);
                     if (tracknum != -1)
                     {
                         matcharr[sectornum, tracknum]++;
                     }
                   }
                }
           }
  
               

           richTextBox2.Text = "";         // clear the richtextbox
           int lo=0;
           for (int l=0;l<8;l++)
           {
               for (int m = 0; m< 5; m ++)
               {
                   richTextBox2.AppendText(Convert.ToString(matcharr[l, m]) + "  ");    // Add the element as a column in single row
               }
               richTextBox2.AppendText("\n");  // change the line or next row
           }
           
        }
        int[,] sectorval=new int[5,6];
        public int thetacal(int x, int y,int cx,int cy)
        {
            if (y >cy)
            {
                //right
                if (x<cx)
                {
                    //top
                    if (x+y<=cx+cy)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    //bottom
                      
                    if (x <= y)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }
            else
            {
               //left
                if (x < cx)
                {
                    //top

                    if (x <= y)
                    {
                        return 7;
                    }
                    else
                    {
                        return 6;
                    } 
                }
                else
                {
                    //bottom

                    if (x + y >= cx + cy)
                    {
                        return 4;
                    }
                    else
                    {
                        return 5;
                    }
                }
            }
            /*for (int i = topj; i < cy; i++)
            {
                for (int j = i; j < cy; j++)
                {
                    if (arr[x,y]==arr[i,j])
                    {
                        return 7;//left first
                    }
                }
            }
            for (int i = righti; i >=cy; i--)
            {
                for (int j =i ; j >=cy; j--)
                {
                    if (arr[x, y] == arr[i, j])
                    {
                        return 0;//right first
                    }
                }
            }
            for (int i = topj+1; i <cx; i--)
            {
                for (int j = i; j<cx ; j--)
                {
                    if (arr[x, y] == arr[i, j])
                    {
                        return 1;//right second
                    }
                }
            }
            for (int i = righti+1; i <cy; i--)
            {
                for (int j = i ; j < cy; j--)
                {
                    if (arr[x, y] == arr[i, j])
                    {
                        return 2;//right third
                    }
                }
            }
            for (int i = righti; i <=cy; i--)
            {
                for (int j = i; j<=cy; j--)
                {
                    if (arr[x, y] == arr[i, j])
                    {
                        return 3;//right fourth
                    }
                }
            }
            for (int i = lefti; i <cy; i++)
            {
                for (int j = i; j < cy; j++)
                {
                    if (arr[x, y] == arr[i, j])
                    {
                        return 4;//left fourth
                    }
                }
            }*/

            /*  double theta = 0;
            int sectstep = 360 / 6;
            if (y != cy && x != cx)
            {
               
                theta = (Math.Atan((y - cy) / (x - cx)) * 180) / Math.PI;
                /*if (theta > 0 && theta <= 60)
                {
                    //right first
                    return 0;
                }
                else if (theta > 60 && theta <= 90 )
                {
                    
                    //right second 1st half
                    return 1;
                }
                else if (theta > -30 && theta < 0 )
                {
                    //Right second 2nd half half
                    return 1;
                }
                else if (theta < -30 && theta > -90 )
                {
                    //Right third
                    return 2;
                }
                else if (theta > -60 && theta < 0 )
                {
                    //Left first
                    return 5;
                }
                else if (theta > -90 && theta < -60 )
                {
                    //Left second 1st half
                    return 4;
                }
                else if (theta > 0 && theta < 30 )
                {
                    // left second 2nd half
                    return 4;

                }
                else if (theta < 30 && theta > 90 )
                {
                    //left third
                    return 3;
                }
            }
            int a = Convert.ToInt16(theta) / sectstep;
            MessageBox.Show(a.ToString());
                return 0;
        */
      }



        public int traccal(int x, int y, double tracksize, int cx, int cy)
        {

            float dist = Convert.ToInt16(Math.Sqrt(Math.Pow((cx - x), 2) + Math.Pow((cy - y), 2)));
            for (int i = 0; i < 5 ; i++)
                if (tracksize * i < dist && dist <= (tracksize * (i + 1)))
                    return (i);
            return -1;
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            for (int l = 0; l < 8; l++)
            {
                subsect = 0;
                for (int m = 0; m < 5; m++)
                {
                    subsect = subsect + matcharr[l, m];
                    total = total + matcharr[l, m];
                    
                }
                secttotal[l]=subsect;
            }
            for (int m = 0; m < 5; m++)
            {
                subtrac = 0;
                for (int l = 0; l < 8; l++)
                {
                    subtrac = subtrac + matcharr[l, m];
                }
                tracktotal[m] = subtrac;
            }
            richTextBox2.AppendText("" + total.ToString());
            try
            {
                MySqlConnection myconn = new MySqlConnection("datasource=localhost;port=3306;username=root;password=root");
                MySqlCommand command = new MySqlCommand("insert into charactersocr.char (c00,c01,c02,c03,c04,c10,c11,c12,c13,c14,c20,c21,c22,c23,c24,c30,c31,c32,c33,c34,c40,c41,c42,c43,c44,c50,c51,c52,c53,c54,c60,c61,c62,c63,c64,c70,c71,c72,c73,c74,total,sect0,sect1,sect2,sect3,sect4,sect5,sect6,sect7,track0,track1,track2,track3,track4,Font,charec,size) values('" + matcharr[0, 0] + "','" + matcharr[0, 1] + "','" + matcharr[0, 2] + "','" + matcharr[0, 3] + "','" + matcharr[0, 4] + "','" + matcharr[1, 0] + "','" + matcharr[1, 1] + "','" + matcharr[1, 2] + "','" + matcharr[1, 3] + "','" + matcharr[1, 4] + "','" + matcharr[2, 0] + "','" + matcharr[2, 1] + "','" + matcharr[2, 2] + "','" + matcharr[2, 3] + "','" + matcharr[2, 4] + "','" + matcharr[3, 0] + "','" + matcharr[3, 1] + "','" + matcharr[3, 2] + "','" + matcharr[3, 3] + "','" + matcharr[3, 4] + "','" + matcharr[4, 0] + "','" + matcharr[4, 1] + "','" + matcharr[4, 2] + "','" + matcharr[4, 3] + "','" + matcharr[4, 4] + "','" + matcharr[5, 0] + "','" + matcharr[5, 1] + "','" + matcharr[5, 2] + "','" + matcharr[5, 3] + "','" + matcharr[5, 4] + "','" + matcharr[6, 0] + "','" + matcharr[6, 1] + "','" + matcharr[6, 2] + "','" + matcharr[6, 3] + "','" + matcharr[6, 4] + "','" + matcharr[7, 0] + "','" + matcharr[7, 1] + "','" + matcharr[7, 2] + "','" + matcharr[7, 3] + "','" + matcharr[7, 4] + "','" + total + "','" + secttotal[0] + "','" + secttotal[1]+ "','"+secttotal[2]+ "','"+secttotal[3]+ "','"+secttotal[4]+ "','"+secttotal[5]+ "','"+secttotal[6]+ "','"+secttotal[7]+ "','"+tracktotal[0]+ "','"+tracktotal[1]+ "','"+tracktotal[2]+ "','"+tracktotal[3]+ "','"+tracktotal[4]+"','" + comboBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "')", myconn);
                myconn.Open();
                MySqlDataReader reader = command.ExecuteReader();
               
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            OpenFileDialog fl = new OpenFileDialog();       // file browser object

            // if user will select a required file then that file should be store in picturebox
            if (fl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var img = Image.FromFile(fl.FileName);
                var newimg = ScaleTotalImage(img, 300, 300);
                bmp = new Bitmap(newimg); // get filename and then save it as bitmap image

                pictureBox2.Image = bmp;            // pass this bitmap image to picturebox
            }
        }
        public Image ScaleTotalImage(Image img,int width ,int height)
        {
            int top=0, bot=0,right=0,left=0;
            var newimg = new Bitmap(img.Width, img.Height);
            MessageBox.Show(img.Width.ToString() + "  " + img.Height.ToString()+"  "+newimg.Width.ToString()+"  "+newimg.Height.ToString());
            Graphics.FromImage(newimg).DrawImage(img, 0, 0, img.Width,img.Height);

            
            for (int i = 0; i < newimg.Width; i++)          //greyscale 
            {
                for (int j = 0; j < newimg.Height; j++)
                {


                    if (newimg.GetPixel(i, j).R <= 100 && newimg.GetPixel(i, j).G <= 100 && newimg.GetPixel(i, j).G <= 100)
                    {

                        newimg.SetPixel(i, j, Color.Black);
                    }
                }
            }
            for (int i = 0; i < newimg.Width; i++)          //greyscale 
            {
                for (int j = 0; j < newimg.Height; j++)
                {
                    Color a = newimg.GetPixel(i, j);
                    double l = 0.216 * a.R + 0.712 * a.G + 0.0722 * a.B;
                    newimg.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(l), Convert.ToInt32(l), Convert.ToInt32(l)));
                }
            }
                for (int i = 0; i < newimg.Height; i++)
                {
                    for (int j = 0; j < newimg.Width; j++)
                    {
                        if (newimg.GetPixel(j, i).R < 30 && newimg.GetPixel(j,i).G < 30 && newimg.GetPixel(j,i).B < 30)
                        {
                            top = i;
                            break;
                        }
                    }
                    if (top != 0)
                        break;
                }
                for (int i = top; i < newimg.Height; i++)
                {
                    for (int j = 0; j < newimg.Width; j++)
                    {
                        if (newimg.GetPixel(j,i).R < 30 && newimg.GetPixel(j,i).G < 30 && newimg.GetPixel(j,i).B < 30)
                        {
                            break;
                        }
                        else if (j == newimg.Width-1) 
                        {
                            bot = i;
                            break;
                        }
                    }
                        if (bot != 0)
                            break;
                    
                    
                }
                int flag = 0,flag2=0;
                left = right;
                for (int i = right; i < newimg.Width; i++)
                {
                    
                    for (int j = top; j <= bot; j++)
                    {
                        if (newimg.GetPixel(i, j).R < 30 && newimg.GetPixel(i, j).G < 30 && newimg.GetPixel(i, j).B < 30)
                        {
                            flag = 1;
                            break;
                        }
                        else
                        {
                            if(flag==1 && j==bot)
                            {
                                right = i;
                                flag=0;
                                flag2 = 1;
                                break;
                            }

                        }
                      
                    }
                    if(flag2==1)
                    {
                        flag2 = 0;
                        break;
                    }
                }


                MessageBox.Show(top.ToString()+"  "+bot.ToString()+"  "+left.ToString()+"  "+right.ToString());
                var cropped = new Bitmap(300 ,300);
                int x1 = left, y1 = top, x2 = right, y2 = bot;
               // Graphics.FromImage(newimg).DrawImage(img, 0, 0, right - left, bot - top);
                GraphicsUnit units = GraphicsUnit.Pixel;
                Rectangle rectcrop = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                Rectangle destrecta = new Rectangle(0, 0, right - left, bot - top);
                Graphics.FromImage(cropped).DrawImage(newimg, 50, 50, rectcrop, units);
               // Graphics.FromImage(newimg).DrawImage(cropped, destrecta, 0, 0, x2 - x1, y2 - y1, units);

            return cropped;
        }
      
    }


}