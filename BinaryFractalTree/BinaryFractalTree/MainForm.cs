﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace BinaryFractalTree
{
    public partial class MainForm : Form
    {
        Pen mainPen = new Pen(Color.Black, 2); // Declares a black pen with a thickness of 2
        Graphics currentCanvas; // Declares the current drawing canvas
        List<FractalPoint> allPoints = new List<FractalPoint>(); // Declares a list with all the points to draw
        public static int fractalRuleA = 90; // Default rule for line A
        public static int fractalRuleB = 270; // Default rule for line B
        public static double sizeChangeRule = 0.5; // Default size change
        public int iterationCount = 2; // Default iteration count
        bool isDrawing = false; // Isn't currently drawing
        public MainForm()
        {
            InitializeComponent();
            currentCanvas = pictureBox1.CreateGraphics(); // Sets the current canvas to edit
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (isDrawing) // Only runs when the program is drawing
            {
                return; // Stops the flow of code at this point, to prevent issues with the list
            }
            isDrawing = true; // Starts drawing
            allPoints.Clear(); // Resets the list
            if (checkBox1.Checked) // Only runs if it's set to clear the image
            {
                pictureBox1.Image = null; // Clears the image
                Helpers.Sleep(1); // Delays operation for 1ms
            }
            FractalPoint basePoint = new FractalPoint(this); // Declares the starting point
            basePoint.currentBearing = 0; // Straight line
            basePoint.originPoint = new Point(Convert.ToInt32(pictureBox1.Width / 2), 0); // Start point centred on canvas, at the base
            basePoint.endPoint = new Point(Convert.ToInt32(pictureBox1.Width / 2), Convert.ToInt32(pictureBox1.Height / (1 / ((double)trackBar5.Value / (double)10)))); // Finds the location of the endpoint by dividing the canvas by the reciprocal of the base height setting
            basePoint.lineLength = Convert.ToInt32(pictureBox1.Height / (1 / ((double)trackBar5.Value / (double)10))); // Calculates the line length, using the same logic as above
            Draw(basePoint); // Draws the original point
            allPoints.Add(basePoint); // Adds the base to the 'endpoints' list
            for (int i = 0; i < iterationCount; i++) // Loops the specified number of times
            {
                FractalPoint[] pointCopy = allPoints.ToArray(); // Copies all the original points to an array
                allPoints.Clear(); // Clears the list
                foreach (FractalPoint singlePoint in pointCopy) // Does the following operations to each item in the list
                {
                    allPoints.Add(new FractalPoint(singlePoint, fractalRuleA)); // Adds a point with rule A
                    allPoints.Add(new FractalPoint(singlePoint, fractalRuleB)); // Adds a point with rule B
                }
                foreach (FractalPoint toDraw in allPoints) // Loops through each point
                {
                    Draw(toDraw); // Draws said point
                }
            }
            isDrawing = false; // Stops drawing
        }
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null; // Clears canvas
            textBox1.Text = null; // Clears log
        }
        private void button3_Click(object sender, EventArgs e)
        {
            WriteLine("This doesn't do anything. I used it to test logic."); // Read it
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            fractalRuleA = trackBar1.Value; // Sets rule A to the settings value
            label1.Text = fractalRuleA.ToString(); // Displays the new setting on the label
            RedrawImage(); // Redraws the image
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            sizeChangeRule = ((double)trackBar2.Value / (double)100); // Changes the size rule to allow for decimals
            label4.Text = sizeChangeRule.ToString(); // Displays the new setting on the label
            RedrawImage(); // Redraws the image
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            iterationCount = trackBar3.Value; // Changes iteration depth
            label6.Text = iterationCount.ToString(); // Displays the new setting on the label
            RedrawImage(); // Redraws the image
        }
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            fractalRuleB = trackBar4.Value; // Sets rule B to the settings value
            label7.Text = fractalRuleB.ToString(); // Displays the new setting on the label
            RedrawImage(); // Redraws the image
        }
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            label10.Text = ((double)trackBar5.Value / (double)10).ToString(); // Sets the start length
            RedrawImage(); // Redraws the image
        }
        private void RedrawImage()
        {
            if (checkBox2.Checked) // Only runs if it's set to redraw
            {
                button1_Click(this, null); // Runs the code in the button
            }
        }
        private void Draw(FractalPoint toDraw)
        {
            currentCanvas.DrawLine(mainPen, new Point(toDraw.originPoint.X, pictureBox1.Height - toDraw.originPoint.Y), new Point(toDraw.endPoint.X, pictureBox1.Height - toDraw.endPoint.Y)); // Flips all points along the X axis as (0, 0) is at top left, versus the bottom left (as is conventional)
        }
        public void WriteLine(string toWrite)
        {
            textBox1.Text += toWrite + Environment.NewLine; // Appends the text, and a new line
        }
        public void WriteLine(string formatString, params string[] writeParams)
        {
            WriteLine(string.Format(formatString, writeParams)); // Formats the string and then writes it
        }
    }
    public static class Helpers
    {
        public static double Degrees(this int inputDouble)
        {
            return inputDouble * (Math.PI / 180); // Converts from radians to degrees
        }
        public static void Sleep(int timeToSleep)
        {
            Stopwatch timeCount = new Stopwatch(); // Declares a new stopwatch
            timeCount.Start(); // Starts said stopwatch
            while (timeCount.ElapsedMilliseconds < timeToSleep) // Keeps running until time is up
            {
                Application.DoEvents(); // Prevents the program from freezing
            }
            timeCount.Stop(); // Stops said stopwatch
        }
    }
    public class FractalPoint
    {
        public Point originPoint;
        public Point endPoint;
        public int currentBearing;
        public int lineLength;
        public MainForm parentForm;
        public FractalPoint(FractalPoint originalPoint, int newBearing)
        {
            originPoint = originalPoint.endPoint; // Takes the origin point as the the endpoint of the parent point (trust me, this makes sense)
            parentForm = originalPoint.parentForm; // Sets the parent form from the parent point
            lineLength = Convert.ToInt32(originalPoint.lineLength * MainForm.sizeChangeRule); // Finds the length of this line
            currentBearing = originalPoint.currentBearing + newBearing; // Sets the current bearing to be the original bearing added onto the rule
            Calculate(); // Begins to calculate the location of the end point
        }
        public FractalPoint(MainForm parentForm)
        {
            this.parentForm = parentForm; // Sets the parent
        }
        private void Calculate()
        {
            int xAngle = (270 - currentBearing) * -1; // Finds the angle to determine the X coordinate
            int xPoint = originPoint.X - Convert.ToInt32(Math.Cos(xAngle.Degrees()) * lineLength); // Does the calculation for the X size, to calculate the new point
            int yAngle = 90 - xAngle; // Finds the angle to determine the Y coordinate
            int yPoint = originPoint.Y + Convert.ToInt32(Math.Cos((90 - xAngle).Degrees()) * lineLength); // Does the calculation for the Y size, to calculate the new point
            endPoint = new Point(xPoint, yPoint); // Creates a new endpoint
            parentForm.WriteLine("{0}, {1} [{2} {3}]", xPoint.ToString(), yPoint.ToString(), xAngle.ToString(), yAngle.ToString()); // Logs everything to the 'console'
        }
    }
}