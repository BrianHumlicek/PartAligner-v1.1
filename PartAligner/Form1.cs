using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Globalization;
//using Microsoft.WindowsCE.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //point_object class allows read and write of position data in either
        //polar or cartesion format.
        class point_object
        {
            private double X, Y, THETA, R;
            public point_object()
            {
                X = 0;  //Cartesion X coordinate
                Y = 0;  //Cartesion Y coordinate
                THETA = 0;  //Polar Angle (Radians)
                R = 0;      //Polar radius
            }
            public double x
            {
                get
                {
                    return X;
                }
                set
                {
                    X = value;
                    cart2polar();
                }
            }
            public double y
            {
                get
                {
                    return Y;
                }
                set
                {
                    Y = value;
                    cart2polar();
                }
            }
            public double theta
            {
                get
                {
                    return THETA;
                }
                set
                {
                    THETA = value;
                    polar2cart();
                }
 
            }
            public double r
            {
                get
                {
                    return R;
                }
                set
                {
                    R = value;
                    polar2cart();
                }
             }
            private void cart2polar()
            {
                R = Math.Sqrt(X * X + Y * Y);
                if (X == 0)
                {
                    if (Y == 0)
                        THETA = 0;
                    else if (Y > 0)
                        THETA = Math.PI * 0.5;  //90 degrees
                    else
                        THETA = Math.PI * 1.5;  //270 degrees
                }
                else if (Y == 0)
                {
                    if (X > 0)
                        THETA = 0;  //0 degrees
                    else
                        THETA = Math.PI;    //180 degrees
                }
                else
                {
                    if ((X > 0) & (Y > 0))  //Quadrent 1
                        THETA = Math.Atan(Y / X);
                    else if (X > 0) //Quadrent 4
                        THETA = Math.Atan(Y / X) + (Math.PI * 2); //+360 degrees
                    else  //Quadrent 2 or 3
                        THETA = Math.Atan(Y / X)  + Math.PI;    //+180 degrees
                }
            }
            private void polar2cart()
            {
                X = Math.Cos(THETA) * R;
                Y = Math.Sin(THETA) * R;
            }

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            point_object base_point = new point_object();
            point_object measured_point = new point_object();
            point_object drawn_point = new point_object();
            point_object base_translate = new point_object();
            double theta,base_delta_x,base_delta_y;

            if ((basepointX.Text == "") || (basepointY.Text == ""))
            {
                MessageBox.Show("Enter a base point!");
                return;
            }
            if ((measuredpointX.Text == "") || (measuredpointY.Text == ""))
            {
                MessageBox.Show("Enter a measured point!");
                return;
            }
            if ((drawnpointX.Text == "") || (drawnpointY.Text == ""))
            {
                MessageBox.Show("Enter a drawn point!");
                return;
            }
            try
            {
                base_point.x = double.Parse(basepointX.Text);
                base_point.y = double.Parse(basepointY.Text);
                measured_point.x = double.Parse(measuredpointX.Text) - base_point.x;    //base_point is subtracted to make the measured and drawn points relative to base_point.
                measured_point.y = double.Parse(measuredpointY.Text) - base_point.y;
                drawn_point.x = double.Parse(drawnpointX.Text) - base_point.x;
                drawn_point.y = double.Parse(drawnpointY.Text) - base_point.y;
            }
            catch
            {
                MessageBox.Show("Make sure all data entered is numeric!");
                return;
            }
            textBox1.Text = String.Format("{0:0.0###}", measured_point.r);
            textBox2.Text = String.Format("{0:0.0###}", drawn_point.r);
            textBox3.Text = String.Format("{0:0.0###}", measured_point.r - drawn_point.r);

            theta = measured_point.theta - drawn_point.theta;   //calculate difference between two angles
            textBox4.Text = String.Format("{0:0.0###}", (theta * (180 / Math.PI)));

            if (theta < 0)
                theta += (Math.PI * 2); //Only allow counter clockwise rotations
            base_translate.r = base_point.r;
            base_translate.theta = base_point.theta + theta;    //compensate the base point for the rotation
            if (split_difference_button.Checked)
            {
                //instead of creating point objects to hold temporary data,
                //measured_point and drawn_point are repurposed as temp vars in this block
                //because they have already served their purpose above and are no longer needed.
                //In this block, treat measured_point and drawn_point as
                //temp_point_a and temp_point_b
                measured_point.r = (measured_point.r + drawn_point.r) / 2; //This clobbers measured_point and drawn_point's data
                drawn_point.theta = measured_point.theta;
                base_translate.x += drawn_point.x - measured_point.x;
                base_translate.y += drawn_point.y - measured_point.y;
            }
            theta *= 180 / Math.PI; //Convert radians to degrees
            base_delta_x = base_point.x - base_translate.x;
            base_delta_y = base_point.y - base_translate.y;
            textBox7.Text = String.Format("G73 A{0:0.0###}\r\n", theta) +
                            String.Format("G97 X{0:0.0###}", base_delta_x) + String.Format(" Y{0:0.0###}", base_delta_y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            point_object base_point = new point_object();
            point_object base_translate = new point_object();
            double theta, base_delta_x, base_delta_y;

            if ((basepointX2.Text == "") || (basepointY2.Text == ""))
            {
                MessageBox.Show("Enter a base point!");
                return;
            }
            if (angle2.Text == "")
            {
                MessageBox.Show("Enter a rotation angle!");
                return;
            }
            try
            {
                base_point.x = double.Parse(basepointX2.Text);
                base_point.y = double.Parse(basepointY2.Text);
                theta = double.Parse(angle2.Text) * (Math.PI / 180);    //convert degrees to radians
            }
            catch
            {
                MessageBox.Show("Make sure all data entered is numeric!");
                return;
            }

            if (theta < 0)
                theta += (Math.PI * 2); //Only allow counter clockwise rotations
            base_translate.r = base_point.r;
            base_translate.theta = base_point.theta + theta;    //compensate the base point for the rotation
            theta *= 180 / Math.PI; //Convert radians to degrees
            base_delta_x = base_point.x - base_translate.x;
            base_delta_y = base_point.y - base_translate.y;
            textBox8.Text = String.Format("G73 A{0:0.0###}\r\n", theta) +
                            String.Format("G97 X{0:0.0###}", base_delta_x) + String.Format(" Y{0:0.0###}", base_delta_y);

        }
    }
}
