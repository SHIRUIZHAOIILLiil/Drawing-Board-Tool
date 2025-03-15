using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;



namespace _1_26_ZhaoShiruiOutcome3
{
    /// <summary>
    /// Programer: Zhao Shirui.
    /// Create Date:2022-05-25.
    /// Last Up Date:2022-06-08
    /// Description: This is a Drawing_Board Class.
    ///              Users can draw straight lines, curves, isosceles triangles, 
    ///              right angle triangles, rectangles 
    ///              and ellipses in this interface (press and hold shift 
    ///              to draw circles at the same time). 
    ///              The drawing tool can erase the content 
    ///              with an eraser or clear the screen with one click. 
    ///              The drawing tool can select the color 
    ///              and fill color of different strokes, and can also adjust the size of strokes.
    ///              This class can also perform undo operations, and open and save pictures.
    /// </summary>

    //Description:
    //Call the function to initialize the form control in the Drawing_form class.
    public partial class FrmDrawing_Board : Form
    {
        public FrmDrawing_Board()
        {
            InitializeComponent();

        }
        //Global variable

        //Boolean variable,In order, 
        //it controls whether the canvas size changes, 
        //whether the user can draw, and whether help clauses are visible.
        bool change = false, draw = false, flag1 = true, flag2 = false;
        //Integer variable, which controls the moving direction, vertical coordinate,
        //horizontal coordinate and drawing type by times.
        int flag, top, left, type = 0;
        //The abstract base class of a graph. Apply for an abstract class named G for drawing.
        Graphics g;
        //Pen class, used to control the size and color of strokes when drawing.
        Pen pen, eraser;
        // Point class, used to determine the start and end coordinates when drawing.
        Point startpt, endpt;
        //The bitmap class is used to display drawing shapes, tracks, and undo operations.
        Bitmap im, imtemp, now;
        //List class, used to store points of curve track.
        List<Point> plist;
        //The brush class, which controls the color of the filled drawing,
        Brush brush;
        //The stack class stores bitmaps and is used to undo operations.
        Stack<Bitmap> undo;
        //String class, used for the path and name of the music file.
        string home, mp3File;



        //The sketchpad move function. 
        //When the sketchpad moves, 
        //the buttons controlling the movement of the sketchpad will move together.
        private void DBLocation()
        {
            //Get the horizontal and vertical coordinates of the drawing board.
            top = PcbDB.Location.Y;
            left = PcbDB.Location.X;
            //Control is located at the bottom of the middle of the drawing board.
            BtnNSC.Location = new Point(left + PcbDB.Width / 2, top + PcbDB.Height);
            //Control is located in the lower right corner of the palette.
            BtnNWSEC.Location = new Point(left + PcbDB.Width, top + PcbDB.Height);
            //Control is located in the middle of the left side of the palette.
            BtnWEC.Location = new Point(left + PcbDB.Width, top + PcbDB.Height / 2);
        }
        //Drawing board size function.
        //When the drawing board size changes, the drawing area also changes.
        private void DBSize()
        {
            //Instantiate a bitmap and graphics for canvas size change.
            Bitmap bit = new Bitmap(PcbDB.Width, PcbDB.Height);
            Graphics gr = Graphics.FromImage(bit);
            gr.FillRectangle(new SolidBrush(Color.White), 0, 0, PcbDB.Width, PcbDB.Height);
            gr.DrawImage(im, 0, 0);
            gr = PcbDB.CreateGraphics();
            gr.DrawImage(bit, 0, 0);
            im = bit;
        }

        //Sketchpad move function. 
        //When the palette moves along the we, NWSE and NS directions, it will be repositioned.
        //Parameter: the shaping variable flag is used to control which direction to move.
        private void DBMove(int flag)
        {
            //Instantiate a point. The position of the point is the position of the mouse.
            Point p = PointToClient(Control.MousePosition);

            switch (flag)
            {
                case 1:
                    PcbDB.Width = p.X - left;
                    DBSize();
                    break;
                case 2:
                    PcbDB.Width = p.X - left;
                    PcbDB.Height = p.Y - top;
                    DBSize();
                    break;
                case 3:
                    PcbDB.Height = p.Y - top;
                    DBSize();
                    break;

            }

        }
        //Color function. When the color changes, the RGB value will be represented.
        private void color()
        {
            LblR.Text = pen.Color.R.ToString();
            LblG.Text = pen.Color.G.ToString();
            LblB.Text = pen.Color.B.ToString();
            LblColor1.BackColor = pen.Color;

        }

        //Adjust the horizontal scroll bar function.
        //When the brush color changes, the value of the horizontal scroll bar also changes.
        private void HScolor()
        {
            HsbR.Value = pen.Color.R;
            HsbG.Value = pen.Color.G;
            HsbB.Value = pen.Color.B;

        }

        //Description:
        //This is Drawing_Board load event.
        // First, determine the position of the control panel moving control.
        //Secondly, initialize the created object.
        //Then assign a value to the music file path 
        //to display the initial color of the brush and fill.
        //Finally, set the undo control to unavailable.

        private void Drawing_Board_Load(object sender, EventArgs e)
        {
            DBLocation();
            im = new Bitmap(PcbDB.Width, PcbDB.Height);
            g = Graphics.FromImage(im);
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            pen = new Pen(Color.Black, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            pen.LineJoin = LineJoin.Round;
            plist = new List<Point>();
            brush = new SolidBrush(Color.Red);
            eraser = new Pen(Color.White, 5);
            undo = new Stack<Bitmap>();
            home = Application.StartupPath;
            mp3File = home + @"/reverse.flac";
            WmpPlayer.URL = mp3File;
            WmpPlayer.Ctlcontrols.stop();
            RtbHelp.Visible = false;
            color();
            LblColor2.BackColor = Color.Red;


            if (undo.Count == 0)
            {
                TsbUndo.Enabled = false;
            }

        }
        //Description:
        //Mouse hover event: when the mouse hovers over this control, 
        //the style of the mouse will be changed to the style in the WE direction.
        private void BtnWEC_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.SizeWE;
        }
        //Description:
        //Mouse leave event.
        //When the mouse leaves the control, the mouse returns to the original style.
        private void BtnWEC_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        //Description:
        //Mouse down event: when the left mouse button is pressed, 
        //the boolean variable change is changed to true 
        //and the integer variable flag is changed to 1.
        private void BtnWEC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                change = true;
                flag = 1;
            }
        }
        //Description:
        //Mouse down event: when the left mouse button is pressed, 
        //the boolean variable change is changed to true 
        //and the integer variable flag is changed to 2.
        private void BtnNWSEC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                change = true;
                flag = 2;
            }
        }
        //Description:
        //Mouse hover event: when the mouse hovers over this control, 
        //the style of the mouse will be changed to the style in the NWSE direction.
        private void BtnNWSEC_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.SizeNWSE;
        }
        //Descripetion:
        //Mouse leave event.
        //When the mouse leaves the control, the mouse returns to the original style.
        private void BtnNWSEC_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        //Description:
        //The mouse movement event can only be triggered
        //when the left mouse button is pressed and the boolean variable is true.
        //The changed flag is passed to and the control for moving the canvas is located.
        private void BtnNWSEC_MouseMove(object sender, MouseEventArgs e)
        {
            if (change && e.Button == MouseButtons.Left)
            {
                DBMove(flag);
                DBLocation();
            }
        }
        //Description:
        //The mouse up event. 
        //When the mouse is up, 
        //the boolean variable that controls the size of the drawing board changes to false.
        private void BtnNWSEC_MouseUp(object sender, MouseEventArgs e)
        {
            change = false;
        }
        //Description:
        //Mouse down event: when the left mouse button is pressed, 
        //the boolean variable change is changed to true 
        //and the integer variable flag is changed to 1.
        private void BtnNSC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                change = true;
                flag = 3;
            }
        }
        //Description:
        //Mouse hover event: when the mouse hovers over this control, 
        //the style of the mouse will be changed to the style in the NS direction.
        private void BtnNSC_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.SizeNS;
        }
        //Description:
        //Mouse leave event.
        //When the mouse leaves the control, the mouse returns to the original style.
        private void BtnNSC_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        //Description:
        //Click the line event, click the button,
        //the value of type will change to 1, 
        //and the status bar will prompt that the current drawing mode is to draw a straight line.
        private void BtnLine_Click(object sender, EventArgs e)
        {
            type = 1;
            TslShape.Text = "Current Shape: Line.";
        }
        //Description:
        //Click the rectangle event, click the button,
        //the value of type will change to 2, 
        //and the status bar will prompt that the current drawing mode is to draw a rectangle.
        private void BtnRectangle_Click(object sender, EventArgs e)
        {
            type = 2;
            TslShape.Text = "Current Shape: Rectangle.";
        }
        //Descripetion:
        //Click the triangle event, click the button,
        //the value of type will change to 3, 
        //and the status bar will prompt that the current drawing mode is to draw a triangle.
        private void BtnRTriangle_Click(object sender, EventArgs e)
        {
            type = 3;
            TslShape.Text = "Current Shape: Right Triangle.";
        }
        //Description:
        //Click the clear event and click this button. The drawing board will be cleared.
        //The value of type will change to 0.
        private void BtnClear_Click(object sender, EventArgs e)
        {
            type = 0;
            g.Clear(Color.White);
            PcbDB.BackgroundImage = im;
            PcbDB.Refresh();
        }
        //Description:
        //Sketchpad mouse down event: when the mouse is pressed on the sketchpad, 
        //if the value of type is not 0 and the left mouse button is pressed, 
        //Set the boolean variable draw to true,
        //the starting position of the drawing can be recorded, 
        //and the bitmap of now can be initialized and stored.
        //If the value of type is 4 or 7,
        //the coordinates of the starting point are also recorded, 
        //and the bitmap is initialized and stored.
        private void PcbDB_MouseDown(object sender, MouseEventArgs e)
        {
            if (type != 0 && e.Button == MouseButtons.Left)
            {
                draw = true;
                startpt = e.Location;
                now = new Bitmap(im);
                undo.Push(now);

            }
            if (type == 4 || type == 7)
            {
                plist.Add(e.Location);
                now = new Bitmap(im);
                undo.Push(now);
            }


        }
        //Description:
        // For the mouse movement event on the drawing board, 
        //when the draw value is true, 
        //the position of the mouse will be updated in real time each time it is moved, 
        //and a temporary class diagram imtemp will be instantiated at the same time.
        //If both the brush and fill options are selected, the drawn figure is the filled figure.
        //If type is not of types 4 and 7, G is instantiated from imtemp.
        //Then, different types of pictures are drawn 
        //according to the values of different types (from 1 to 7).
        //Finally, set the image of pcbdb to imtemp and refresh it in real time.
        private void PcbDB_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                endpt = e.Location;
                imtemp = new Bitmap(im);
                if (type != 4 && type != 7)
                {
                    g = Graphics.FromImage(imtemp);

                }
                switch (type)
                {
                    case 1:
                        g.DrawLine(pen, startpt, endpt);
                        break;
                    case 2:
                        if (CbxOutline.Checked)
                        {
                            g.DrawRectangle(pen, startpt.X, startpt.Y, endpt.X - startpt.X, endpt.Y - startpt.Y);
                            g.DrawRectangle(pen, endpt.X, endpt.Y, startpt.X - endpt.X, startpt.Y - endpt.Y);
                            g.DrawRectangle(pen, startpt.X, endpt.Y, endpt.X - startpt.X, startpt.Y - endpt.Y);
                            g.DrawRectangle(pen, endpt.X, startpt.Y, startpt.X - endpt.X, endpt.Y - startpt.Y);
                        }
                        if (CbxFill.Checked)
                        {
                            g.FillRectangle(brush, startpt.X, startpt.Y, endpt.X - startpt.X, endpt.Y - startpt.Y);
                            g.FillRectangle(brush, endpt.X, endpt.Y, startpt.X - endpt.X, startpt.Y - endpt.Y);
                            g.FillRectangle(brush, startpt.X, endpt.Y, endpt.X - startpt.X, startpt.Y - endpt.Y);
                            g.FillRectangle(brush, endpt.X, startpt.Y, startpt.X - endpt.X, endpt.Y - startpt.Y);
                        }
                        break;
                    case 3:
                        Point p1 = new Point(startpt.X, startpt.Y);
                        Point p2 = new Point(startpt.X, endpt.Y);
                        Point p3 = new Point(endpt.X, endpt.Y);
                        Point[] parray = { p1, p2, p3 };
                        if (CbxOutline.Checked)
                        {
                            g.DrawPolygon(pen, parray);
                        }
                        if (CbxFill.Checked)
                        {
                            g.FillPolygon(brush, parray);
                        }
                        break;
                    case 4:
                        plist.Add(e.Location);
                        g.DrawCurve(pen, plist.ToArray());
                        break;
                    case 5:
                        p1 = new Point(startpt.X, startpt.Y);
                        p2 = new Point((endpt.X + startpt.X) / 2, endpt.Y);
                        p3 = new Point(endpt.X, startpt.Y);
                        Point[] parray1 = { p1, p2, p3 };
                        if (CbxOutline.Checked)
                        {
                            g.DrawPolygon(pen, parray1);
                        }
                        if (CbxFill.Checked)
                        {
                            g.FillPolygon(brush, parray1);
                        }
                        break;
                    case 6:
                        if (CbxOutline.Checked)
                        {
                            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left)
                            {
                                g.DrawEllipse(pen, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.X - startpt.X));
                            }
                            else
                                g.DrawEllipse(pen, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.Y - startpt.Y));
                        }
                        if (CbxFill.Checked)
                        {
                            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left)
                            {
                                g.FillEllipse(brush, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.X - startpt.X));
                            }
                            else
                                g.FillEllipse(brush, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.Y - startpt.Y));
                        }
                        break;
                    case 7:
                        plist.Add(e.Location);
                        g.DrawCurve(eraser, plist.ToArray());
                        break;

                }

                PcbDB.Image = imtemp;
                PcbDB.Refresh();

            }
        }
        //Description:
        //Click the isosceles triangle event, click the button,
        //the value of type will change to 5, 
        //and the status bar will prompt that the current drawing mode is to draw an isosceles triangle.
        private void BtnTriangle_Click(object sender, EventArgs e)
        {
            type = 5;
            TslShape.Text = "Current Shape: Isosceles Triangle.";
        }

        //Description:
        //Click the ellipse event, click the button,
        //the value of type will change to 6, 
        //and the status bar will prompt that the current drawing mode is to draw an ellipse.

        private void BtnEllipse_Click(object sender, EventArgs e)
        {
            type = 6;
            TslShape.Text = "Current Shape: Ellipse.";
        }

        //Description:
        //Adjust the horizontal scroll bar value event.
        //When the horizontal scroll bar value changes,
        //the color will be displayed in RGB,
        //and the current pen color and RGB specific value will be prompted.
        private void HsbR_Scroll(object sender, ScrollEventArgs e)
        {
            LblR.Text = HsbR.Value.ToString();
            pen.Color = Color.FromArgb(HsbR.Value, HsbG.Value, HsbB.Value);
            LblColor1.BackColor = Color.FromArgb(HsbR.Value, HsbG.Value, HsbB.Value);
        }

        //Description:
        //Adjust the horizontal scroll bar value event.
        //When the horizontal scroll bar value changes,
        //the color will be displayed in RGB,
        //and the current pen color and RGB specific value will be prompted.
        private void HsbG_Scroll(object sender, ScrollEventArgs e)
        {
            LblG.Text = HsbG.Value.ToString();
            pen.Color = Color.FromArgb(HsbR.Value, HsbG.Value, HsbB.Value);
            LblColor1.BackColor = Color.FromArgb(HsbR.Value, HsbG.Value, HsbB.Value);
        }

        //Description:
        //Adjust the horizontal scroll bar value event.
        //When the horizontal scroll bar value changes,
        //the color will be displayed in RGB,
        //and the current pen color and RGB specific value will be prompted.
        private void HsbB_Scroll(object sender, ScrollEventArgs e)
        {
            LblB.Text = HsbB.Value.ToString();
            pen.Color = Color.FromArgb(HsbR.Value, HsbG.Value, HsbB.Value);
            LblColor1.BackColor = Color.FromArgb(HsbR.Value, HsbG.Value, HsbB.Value);
        }

        //Description:
        //Adjust the horizontal scroll bar value event.
        //When the horizontal scroll bar value changes,
        //the color will be displayed in RGB,
        //and the current brush color and RGB specific value will be prompted.
        private void HsbRF_Scroll(object sender, ScrollEventArgs e)
        {
            LblRF.Text = HsbRF.Value.ToString();
            brush = new SolidBrush(Color.FromArgb(HsbRF.Value, HsbGF.Value, HsbBF.Value));
            LblColor2.BackColor = Color.FromArgb(HsbRF.Value, HsbGF.Value, HsbBF.Value);
        }

        //Description:
        //Adjust the horizontal scroll bar value event.
        //When the horizontal scroll bar value changes,
        //the color will be displayed in RGB,
        //and the current brush color and RGB specific value will be prompted.
        private void HsbGF_Scroll(object sender, ScrollEventArgs e)
        {
            LblGF.Text = HsbGF.Value.ToString();
            brush = new SolidBrush(Color.FromArgb(HsbRF.Value, HsbGF.Value, HsbBF.Value));
            LblColor2.BackColor = Color.FromArgb(HsbRF.Value, HsbGF.Value, HsbBF.Value);
        }

        //Description:
        //Adjust the horizontal scroll bar value event.
        //When the horizontal scroll bar value changes,
        //the color will be displayed in RGB,
        //and the current brush color and RGB specific value will be prompted.
        private void HsbBF_Scroll(object sender, ScrollEventArgs e)
        {
            LblBF.Text = HsbBF.Value.ToString();
            brush = new SolidBrush(Color.FromArgb(HsbRF.Value, HsbGF.Value, HsbBF.Value));
            LblColor2.BackColor = Color.FromArgb(HsbRF.Value, HsbGF.Value, HsbBF.Value);
        }

        //Description:
        //Stroke size adjustment event. 
        //This is a combo box selection box.
        //The user can select different stroke sizes,
        //and the status prompt bar will prompt the current stroke size.
        private void CmxThickness_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (CmxThickness.Text)
            {
                case "1px":
                    pen.Width = 1;
                    TslStrokesize.Text = "1 px";
                    break;
                case "3px":
                    pen.Width = 3;
                    TslStrokesize.Text = "3 px";
                    break;
                case "5px":
                    pen.Width = 5;
                    TslStrokesize.Text = "5 px";
                    break;
                case "8px":
                    pen.Width = 8;
                    TslStrokesize.Text = "8 px";
                    break;
                case "10px":
                    pen.Width = 10;
                    TslStrokesize.Text = "10 px";
                    break;
                case "15px":
                    pen.Width = 15;
                    TslStrokesize.Text = "15 px";
                    break;
                case "20px":
                    pen.Width = 20;
                    TslStrokesize.Text = "20 px";
                    break;
            }
        }

        //Description:
        //Rubber sizing event.
        //This is a combo box selection box. 
        //The user can select different stroke sizes,
        //and the status prompt bar will prompt the current rubber size.
        private void CmxES_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CmxES.Text)
            {
                case "Small":
                    eraser.Width = 5;
                    TslEraserSize.Text = "EraserSize: Small.";
                    break;
                case "Medium":
                    eraser.Width = 10;
                    TslEraserSize.Text = "EraserSize: Medium.";
                    break;
                case "Big":
                    eraser.Width = 20;
                    TslEraserSize.Text = "EraserSize: Big.";
                    break;
            }

        }
        //Description:
        //Quickly select the brush color event.
        //Click this button, and the pen color will switch to Red,
        //and the interface will display the current pen color.
        private void BtnRed_Click(object sender, EventArgs e)
        {
            pen.Color = Color.Red;
            color();
            HScolor();

        }

        //Description:
        //Quickly select the brush color event.
        //Click this button, and the pen color will switch to Pink,
        //and the interface will display the current pen color.
        private void BtnPink_Click(object sender, EventArgs e)
        {
            pen.Color = Color.Pink;
            color();
            HScolor();
        }

        //Description:
        //Quickly select the brush color event.
        //Click this button, and the pen color will switch to Black/
        //and the interface will display the current pen color.
        private void BtnBlack_Click(object sender, EventArgs e)
        {
            pen.Color = Color.Black;
            color();
            HScolor();
        }

        //Description:
        //Quickly select the brush color event.
        //Click this button, and the pen color will switch to Blue,
        //and the interface will display the current pen color.
        private void BtnBlue_Click(object sender, EventArgs e)
        {
            pen.Color = Color.Blue;
            color();
            HScolor();
        }

        //Description:
        //Quickly select the brush color event.
        //Click this button, and the pen color will switch to Green,
        //and the interface will display the current pen color.
        private void BtnGreen_Click(object sender, EventArgs e)
        {
            pen.Color = Color.Green;
            color();
            HScolor();
        }
        //Description:
        //Quickly select the brush color event.
        //Click this button, and the pen color will switch to Yellow,
        //and the interface will display the current pen color.
        private void BtnYellow_Click(object sender, EventArgs e)
        {
            pen.Color = Color.Yellow;
            color();
            HScolor();
        }
        //Description:
        //Quickly select the brush color event,
        //click this button, the brush color will switch to Red,
        //and the interface will display the current brush color.
        private void BtnRedF_Click(object sender, EventArgs e)
        {
            brush = new SolidBrush(Color.Red);
            LblColor2.BackColor = (Color.Red);
            HsbRF.Value = Color.Red.R;
            HsbGF.Value = Color.Red.G;
            HsbBF.Value = Color.Red.B;
            LblRF.Text = HsbRF.Value.ToString();
            LblGF.Text = HsbGF.Value.ToString();
            LblBF.Text = HsbBF.Value.ToString();
        }

        //Description:
        //Quickly select the brush color event,
        //click this button, the brush color will switch to Pink,
        //and the interface will display the current brush color.
        private void BtnPinkF_Click(object sender, EventArgs e)
        {
            brush = new SolidBrush(Color.Pink);
            LblColor2.BackColor = (Color.Pink);
            HsbRF.Value = Color.Pink.R;
            HsbGF.Value = Color.Pink.G;
            HsbBF.Value = Color.Pink.B;
            LblRF.Text = HsbRF.Value.ToString();
            LblGF.Text = HsbGF.Value.ToString();
            LblBF.Text = HsbBF.Value.ToString();
        }

        //Description:
        //Quickly select the brush color event,
        //click this button, the brush color will switch to Black,
        //and the interface will display the current brush color.
        private void BtnBlackF_Click(object sender, EventArgs e)
        {
            brush = new SolidBrush(Color.Black);
            LblColor2.BackColor = (Color.Black);
            HsbRF.Value = Color.Black.R;
            HsbGF.Value = Color.Black.G;
            HsbBF.Value = Color.Black.B;
            LblRF.Text = HsbRF.Value.ToString();
            LblGF.Text = HsbGF.Value.ToString();
            LblBF.Text = HsbBF.Value.ToString();
        }

        //Description:
        //Quickly select the brush color event,
        //click this button, the brush color will switch to Blue,
        //and the interface will display the current brush color.
        private void BtnBlueF_Click(object sender, EventArgs e)
        {
            brush = new SolidBrush(Color.Blue);
            LblColor2.BackColor = (Color.Blue);
            HsbRF.Value = Color.Blue.R;
            HsbGF.Value = Color.Blue.G;
            HsbBF.Value = Color.Blue.B;
            LblRF.Text = HsbRF.Value.ToString();
            LblGF.Text = HsbGF.Value.ToString();
            LblBF.Text = HsbBF.Value.ToString();
        }

        //Description:
        //Quickly select the brush color event,
        //click this button, the brush color will switch to Green,
        //and the interface will display the current brush color.
        private void BtnGreenF_Click(object sender, EventArgs e)
        {
            brush = new SolidBrush(Color.Green);
            LblColor2.BackColor = (Color.Green);
            HsbRF.Value = Color.Green.R;
            HsbGF.Value = Color.Green.G;
            HsbBF.Value = Color.Green.B;
            LblRF.Text = HsbRF.Value.ToString();
            LblGF.Text = HsbGF.Value.ToString();
            LblBF.Text = HsbBF.Value.ToString();
        }

        //Description:
        //Quickly select the brush color event,
        //click this button, the brush color will switch to Yellow,
        //and the interface will display the current brush color.
        private void BtnYellowF_Click(object sender, EventArgs e)
        {
            brush = new SolidBrush(Color.Yellow);
            LblColor2.BackColor = (Color.Yellow);
            HsbRF.Value = Color.Yellow.R;
            HsbGF.Value = Color.Yellow.G;
            HsbBF.Value = Color.Yellow.B;
            LblRF.Text = HsbRF.Value.ToString();
            LblGF.Text = HsbGF.Value.ToString();
            LblBF.Text = HsbBF.Value.ToString();
        }

        //Description:
        //Save the current picture event.
        //When you click this button,
        //if the picture on the canvas is blank,
        //it will not be saved. If it is a picture with drawing traces,
        //it will be saved and saved in a specific format.
        private void TsbSave_Click(object sender, EventArgs e)
        {
            PcbDB.Refresh();
            for (int i = 0; i < im.Width; i++)
            {
                for (int j = 0; j < im.Height; j++)
                {
                    if ((im.GetPixel(i, j).A + im.GetPixel(i, j).R + im.GetPixel(i, j).G
                        + im.GetPixel(i, j).B) != 1020)
                    {
                        SfdSave.Filter = "BMP file|*.bmp|JPG file|*.jpg|PNG file|*.png|" +
                 "TIFF file|*.tif|GIF file|*.gif";
                        SfdSave.Title = "Save this file";

                        if (SfdSave.ShowDialog() == DialogResult.OK)
                        {
                            string filename = SfdSave.FileName;
                            string filetext = filename.Remove(0, filename.Length - 3);
                            switch (filetext)
                            {
                                case "bmp":
                                    im.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                    break;
                                case "jpg":
                                    im.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    break;
                                case "png":
                                    im.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                                    break;
                                case "tif":
                                    im.Save(filename, System.Drawing.Imaging.ImageFormat.Tiff);
                                    break;
                                case "gif":
                                    im.Save(filename, System.Drawing.Imaging.ImageFormat.Gif);
                                    break;
                            }
                            goto flag;

                        }
                        else
                            goto flag;
                    }

                }
            }
        flag:;

        }
        //Description:
        //Whether to select events for filling.
        //If you are sure to select filling,
        //the status bar will prompt that it has been filled.
        //If no fill is selected, the status bar indicates that it is not filled.
        private void CbxFill_CheckedChanged(object sender, EventArgs e)
        {
            if (CbxFill.Checked)
            {
                TslWFill.Text = "Whether Fill: Yes. ";
            }
            if (CbxFill.Checked == false)
            {
                TslWFill.Text = "Whether Fill: No. ";
            }
        }

        private void lblStuInfo_Click(object sender, EventArgs e)
        {

        }

        //Description:
        //Event of music player status change.
        //If the current music is played,
        //the music player will repeat the second time after 1 second.
        private void WmpPlayer_StatusChange(object sender, EventArgs e)
        {
            if ((int)WmpPlayer.playState == 1)
            {
                System.Threading.Thread.Sleep(1000);
                WmpPlayer.Ctlcontrols.play();
            }
        }

        //Description:
        //Click the help event.
        //When you click this button,
        //a message prompt window will be displayed on the drawing board
        //to prompt the user how to use the drawing tool.
        //Click the drawing board again and the prompt window will disappear.
        private void TsbHelp_Click(object sender, EventArgs e)
        {

            if (flag1)
            {
                RtbHelp.Visible = true;
                flag1 = false;
                flag2 = true;
            }
            else if (flag2)
            {
                RtbHelp.Visible = false;
                flag1 = true;
                flag2 = false;
            }

        }
        //Description:
        //Picture saving event: when the user clicks this button,
        //if a picture already exists on the palette,
        //the user will be prompted to save the existing picture first.
        //If yes is selected, the current picture will be saved first,
        //and then a new picture will be opened.
        //If you click No, a new picture will be opened directly.
        private void TsbOpen_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < im.Width; i++)
            {
                for (int j = 0; j < im.Height; j++)
                {
                    if ((im.GetPixel(i, j).A + im.GetPixel(i, j).R + im.GetPixel(i, j).G
                        + im.GetPixel(i, j).B) != 1020)
                    {
                        if (MessageBox.Show("Are you sure to save the picture?", "Save file",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            TsbSave_Click(sender, e);
                            goto flag;
                        }
                        else
                        {
                            goto flag;
                        }
                    }
                }
            }
        flag:
            OfdOpen.Filter = "BMP file|*.bmp|JPG file|*.jpg|PNG file|*.png|" +
                "TIFF file|*.tif|GIF file|*.gif";
            OfdOpen.Title = "Open a picture";
            if (OfdOpen.ShowDialog() == DialogResult.OK)
            {
                string filename = OfdOpen.FileName;
                Image image = Image.FromFile(filename);
                g.Clear(System.Drawing.Color.White);
                g.DrawImage(image, 0, 0);
                PcbDB.Image = im;
                PcbDB.Refresh();
            }
        }
        //Description:
        //Trace cancellation event.
        //Click this button to cancel the previous drawing process.
        //When there is no picture on the current screen,
        //clicking this button will no longer be available.
        private void TsbUndo_Click(object sender, EventArgs e)
        {
            if (undo.Count == 0)
            {
                TsbUndo.Enabled = false;
            }
            if (undo.Count != 0)
            {
                TsbUndo.Enabled = true;
                im = undo.Pop();
                g = Graphics.FromImage(im);
                PcbDB.Image = im;
                PcbDB.Refresh();
            }


        }

        //Descripetion:
        //Click the eraser event, click the button,
        //the value of type will change to 7, 
        //and the status bar will prompt that the current drawing mode is to use eraser.
        private void BtnEraser_Click(object sender, EventArgs e)
        {
            type = 7;
            TslShape.Text = "Current Shape: Eraser.";
        }

        //Descripetion:
        //Click the curve event, click the button,
        //the value of type will change to 4, 
        //and the status bar will prompt that the current drawing mode is to draw the curve.
        private void btnCurve_Click(object sender, EventArgs e)
        {
            type = 4;
            TslShape.Text = "Current Shape: Curve.";
        }
        //Description:
        //Pcbdb mouse up event:
        //when the mouse is up,
        //the position of the mouse is automatically recorded,
        //and according to the drawing trace,
        //an identical figure is drawn in IM and stored in the pcbdb image.
        //When the left mouse button is raised, the boolean variable draw will become false.
        private void PcbDB_MouseUp(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                endpt = e.Location;
                g = Graphics.FromImage(im);

                switch (type)
                {
                    case 1:
                        g.DrawLine(pen, startpt, endpt);
                        break;
                    case 2:
                        if (CbxOutline.Checked)
                        {
                            g.DrawRectangle(pen, startpt.X, startpt.Y, endpt.X - startpt.X, endpt.Y - startpt.Y);
                            g.DrawRectangle(pen, endpt.X, endpt.Y, startpt.X - endpt.X, startpt.Y - endpt.Y);
                            g.DrawRectangle(pen, startpt.X, endpt.Y, endpt.X - startpt.X, startpt.Y - endpt.Y);
                            g.DrawRectangle(pen, endpt.X, startpt.Y, startpt.X - endpt.X, endpt.Y - startpt.Y);
                        }
                        if (CbxFill.Checked)
                        {
                            g.FillRectangle(brush, startpt.X, startpt.Y, endpt.X - startpt.X, endpt.Y - startpt.Y);
                            g.FillRectangle(brush, endpt.X, endpt.Y, startpt.X - endpt.X, startpt.Y - endpt.Y);
                            g.FillRectangle(brush, startpt.X, endpt.Y, endpt.X - startpt.X, startpt.Y - endpt.Y);
                            g.FillRectangle(brush, endpt.X, startpt.Y, startpt.X - endpt.X, endpt.Y - startpt.Y);
                        }
                        break;
                    case 3:
                        Point p1 = new Point(startpt.X, startpt.Y);
                        Point p2 = new Point(startpt.X, endpt.Y);
                        Point p3 = new Point(endpt.X, endpt.Y);
                        Point[] parray = { p1, p2, p3 };
                        if (CbxOutline.Checked)
                        {
                            g.DrawPolygon(pen, parray);
                        }
                        if (CbxFill.Checked)
                        {
                            g.FillPolygon(brush, parray);
                        }
                        break;
                    case 4:
                        plist.Clear();
                        break;
                    case 5:
                        p1 = new Point(startpt.X, startpt.Y);
                        p2 = new Point((endpt.X + startpt.X) / 2, endpt.Y);
                        p3 = new Point(endpt.X, startpt.Y);
                        Point[] parray1 = { p1, p2, p3 };
                        if (CbxOutline.Checked)
                        {
                            g.DrawPolygon(pen, parray1);
                        }
                        if (CbxFill.Checked)
                        {
                            g.FillPolygon(brush, parray1);
                        }
                        break;
                    case 6:
                        if (CbxOutline.Checked)
                        {
                            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left)
                            {
                                g.DrawEllipse(pen, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.X - startpt.X));
                            }
                            else
                                g.DrawEllipse(pen, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.Y - startpt.Y));
                        }
                        if (CbxFill.Checked)
                        {
                            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left)
                            {
                                g.FillEllipse(brush, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.X - startpt.X));
                            }
                            else
                                g.FillEllipse(brush, startpt.X, startpt.Y, (endpt.X - startpt.X), (endpt.Y - startpt.Y));
                        }
                        break;
                    case 7:
                        plist.Clear();
                        break;
                }

                PcbDB.Image = im;
                PcbDB.Refresh();
                draw = false;
                TsbUndo.Enabled = true;
            }
        }

        //Descripetion:
        //The mouse movement event can only be triggered
        //when the left mouse button is pressed and the boolean variable is true.
        //The changed flag is passed to and the control for moving the canvas is located.
        private void BtnNSC_MouseMove(object sender, MouseEventArgs e)
        {
            if (change && e.Button == MouseButtons.Left)
            {
                DBMove(flag);
                DBLocation();
            }
        }

        //Descripetion:
        //The mouse up event. 
        //When the mouse is up, 
        //the boolean variable that controls the size of the drawing board changes to false.
        private void BtnNSC_MouseUp(object sender, MouseEventArgs e)
        {
            change = false;
        }

        //Descripetion:
        //The mouse up event. 
        //When the mouse is up, 
        //the boolean variable that controls the size of the drawing board changes to false.
        private void BtnWEC_MouseUp(object sender, MouseEventArgs e)
        {
            change = false;

        }

        //Descripetion:
        //The mouse movement event can only be triggered
        //when the left mouse button is pressed and the boolean variable is true.
        //The changed flag is passed to and the control for moving the canvas is located.
        private void BtnWEC_MouseMove(object sender, MouseEventArgs e)
        {
            if (change && e.Button == MouseButtons.Left)
            {
                DBMove(flag);
                DBLocation();
            }
        }
    }


}
