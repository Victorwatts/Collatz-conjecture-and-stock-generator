using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace collatz_conjecture
{
    public partial class Form1 : Form
    {
        public long Entered_Number;
        public int Terms = 0;
        public List<long> NumList = new List<long>();
        public List<DataPoint> PointA = new List<DataPoint>();
        public List<DataPoint> PointB = new List<DataPoint>();
        public int num;
        public int NumCheck = 1;
        
        public Form1()
        {
            InitializeComponent();
            txt_Number.Maximum = long.MaxValue;
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            NumList.Clear();
            lv_Terms.Clear();
            Entered_Number = (long)txt_Number.Value;
            lbl_EnteredNumber.Text = Entered_Number.ToString();
            num++;
            NumCheck = 1;
            SequenceCheck();

        }

        public void SequenceCheck()
        {
            bool found = false;

            if (dataGridView1 != null)
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells[0].Value != null && item.Cells[0].Value.ToString() == Entered_Number.ToString())
                    {
                        found = true;
                        break; //stop iteration here since it's already found
                    }
                }
                if (!found)
                {
                    NumCheck = 0;
                    ChartCollatz();
                }
                else
                {

                    DialogResult dialogResult = MessageBox.Show("Would you like to run the sequence again?", "Sequence already exists in the database!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        NumCheck = 1;
                        ChartCollatz();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //do something else
                    }
                }

            }

        }

        public void ChartCollatz()
        {
            long n = Entered_Number;
            NumList.Add(n);
            while (n != 1)
            {

                if (n % 2 == 0)
                {
                    n = n / 2;
                }
                else
                {
                    n = (3 * n) + 1;
                }
                NumList.Add(n);

            }

            lv_Terms.BeginUpdate();
            foreach (long item in NumList)
            {
                lv_Terms.Items.Add(item.ToString());
            }
            lv_Terms.EndUpdate();
            StringBuilder strBuilder = new StringBuilder();
            
            foreach (var item in NumList)
            {
                strBuilder.Append(item).Append(" , ");
            }
            string strFuntionResult = strBuilder.ToString();

            if (NumCheck == 0)
            {
                dataGridView1.Rows.Add(Entered_Number, NumList.Count, NumList.Max(), strFuntionResult);
            }


            lbl_Terms.Text = NumList.Count.ToString();
            Charter();
        }


        public void Charter()
        {

            int x = NumList.Count;
            var max = NumList.Max();
            var series = new Series("Series" + num + " No: " + Entered_Number + " Terms: " + x + " Heighest: " + max);
            series.ChartType = SeriesChartType.Line;
            series.MarkerStyle = MarkerStyle.Circle;
            ChartArea CA = chart1.ChartAreas[0];
            CA.AxisX.ScaleView.Zoomable = true;
            CA.AxisY.ScaleView.Zoomable = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            chart1.MouseWheel += chart1_MouseWheel;
            CA.RecalculateAxesScale();

            int i = 1;
            int y = 0;
            while (i != x)
            {

                series.Points.AddXY(i, NumList[y]);

                i++;
                y++;
            }
            series.Points.AddXY(i, NumList[y]);
            chart1.Series.Add(series);

        }

        public void Clear()
        {
            NumList.Clear();
            lv_Terms.Clear();
            chart1.Series.Clear();
            txt_Number.Value = 1;
            lbl_Terms.Text = "0";
            lbl_EnteredNumber.Text = "0";
        }
        #region graph zoom
        private class ZoomFrame
        {
            public double XStart { get; set; }
            public double XFinish { get; set; }
            public double YStart { get; set; }
            public double YFinish { get; set; }
        }

        private readonly Stack<ZoomFrame> _zoomFrames = new Stack<ZoomFrame>();
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            // Console.WriteLine("lagging");
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0)
                {
                    if (0 < _zoomFrames.Count)
                    {
                        var frame = _zoomFrames.Pop();
                        if (_zoomFrames.Count == 0)
                        {
                            xAxis.ScaleView.ZoomReset();
                            yAxis.ScaleView.ZoomReset();
                        }
                        else
                        {
                            xAxis.ScaleView.Zoom(frame.XStart, frame.XFinish);
                            yAxis.ScaleView.Zoom(frame.YStart, frame.YFinish);
                        }
                    }
                }
                else if (e.Delta > 0)
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    _zoomFrames.Push(new ZoomFrame { XStart = xMin, XFinish = xMax, YStart = yMin, YFinish = yMax });

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 2;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 2;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }

            }
            catch { Debug.Write("lagging"); }
        }
        #endregion

        private void Generate_Click(object sender, EventArgs e)
        {
            
            chartClear();
            StocksGenerator.RandNumGen();
            Charter2();
           
            if (cb_MA50.Checked)
            {
                MA50Charter();
            } 
            if (cb_MA200.Checked)
            {
                MA200Charter();
            }
        }


        public void chartClear()
        {
            chart2.Series.Clear();
            StocksGenerator.StockList.Clear();
            listView2.Clear();
        }

        public void AddToList()
        {
            
            listView2.BeginUpdate();
            //  foreach (double item in StocksGenerator.StockList)
            // {
            //      listView2.Items.Add(item.ToString());
            // }
            for (int i = 0; i < StocksGenerator.StockList.Count; i++)
            {
                listView2.Items.Add(StocksGenerator.StockList[i].ToString());
            }
            listView2.EndUpdate();
        }



        public void Charter2()
        {
           

            int StockCount = StocksGenerator.StockList.Count;

            var series = new Series("Stock Price");
            series.ChartType = SeriesChartType.Line;
            ChartArea CA = chart2.ChartAreas[0];
            CA.AxisX.ScaleView.Zoomable = true;
            CA.AxisY.ScaleView.Zoomable = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            chart2.MouseWheel += chart1_MouseWheel;


            int i = 1;
            int y = 0;
            while (i != StockCount)
            {

                series.Points.AddXY(i, StocksGenerator.StockList[y]);

                i++;
                y++;
            }

            series.Points.AddXY(i, StocksGenerator.StockList[y]);
            CA.RecalculateAxesScale();
            chart2.Series.Add(series);
            if (cb_StockValues.Checked)
            {
                AddToList();
            }
            PointB.Clear();
            //foreach (var item in series.Points)
           // {
           //     PointB.Add(item);
           // }

        }

        public void MA50Charter()
        {
            StocksGenerator.MA50List.Clear();
            StocksGenerator.MovingAvg50();
            var seriesMA50 = new Series("MA50");
            seriesMA50.ChartType = SeriesChartType.Line;
            int MA50Count = StocksGenerator.MA50List.Count;
            int i = 1;
            int y = 0;
            seriesMA50.Points.AddXY(i, StocksGenerator.MA50List[y]);
            while (i != MA50Count)
            {

                seriesMA50.Points.AddXY(i * 50, StocksGenerator.MA50List[y]);

                i++;
                y++;
            }
            seriesMA50.Points.AddXY(2000, StocksGenerator.MA50List[y]);
            seriesMA50.Color = Color.Red;
            chart2.Series.Add(seriesMA50);

           // double[] id1 = StocksGenerator.StockList.ToArray();
           // double[] id2 = StocksGenerator.MA50List.ToArray();
        }

        public void MA200Charter()
        {
            StocksGenerator.MA200List.Clear();
            StocksGenerator.MovingAvg200();
            var seriesMA200 = new Series("MA200");
            seriesMA200.ChartType = SeriesChartType.Line;
            int MA200List = StocksGenerator.MA200List.Count;
            int i = 1;
            int y = 0;
            seriesMA200.Points.AddXY(i, StocksGenerator.MA200List[y]);
            while (i != MA200List)
            {

                seriesMA200.Points.AddXY(i * 200, StocksGenerator.MA200List[y]);

                i++;
                y++;
            }
            seriesMA200.Points.AddXY(2000, StocksGenerator.MA200List[y]);
            seriesMA200.Color = Color.DarkMagenta;
            chart2.Series.Add(seriesMA200);
            PointA.Clear();
           // foreach (var item in seriesMA200.Points)
            //{
           //     PointA.Add(item);
            //    Console.WriteLine(item.ToString());
            //}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            chartClear();
            cb_MA50.Checked = false;
            cb_MA200.Checked = false;
            cb_StockValues.Checked = false;
        }

       
    }
}
