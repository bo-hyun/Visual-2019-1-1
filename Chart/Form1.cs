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
using System.Windows.Forms.DataVisualization.Charting;

namespace Chart
{
    public partial class Form1 : Form
    {//Data를 배열에 넣자.
        double[] ecg = new double[50000];
        double[] ppg = new double[50000];
        int ecgCount;   //ecg 데이터 개수
        int ppgCount;   //ppg 데이터 개수
        Timer t = new Timer();  //timer 객체 생성(그래프를 움직이게 하기 위해서)
        //화면이 바뀌어야한다.(화면이 움직여야 한다.)
        //그래서 timer가 필요하다.
        //min,max과 Zoom을 통해서 화면을 이동시킨다.
        //t.interval=10;   
        //이 클래스의 멤버 변수에 작성하면 안된다,
        //t.interval은 멤버 변수, 멤버 함수도 아니기 때문에 멤버 변수로 작성하면 안된다.
        //그러므로 생성사 Form1()안에 작성한다.
        //클래스 멤버 변수를 다른 말로 '필드'라고 한다.
        public Form1()
        {
            InitializeComponent();
            this.Text = "ECG/PPG";
            this.WindowState = FormWindowState.Maximized;
            //시작할 때부터 window가 꽉차게 한다.

            EcgRead();  //ecg데이터를 읽어옴.
            PpgRead();   //ppg데이터를 읽어옴.

            t.Interval = 10;   //0.001초
            t.Tick += T_Tick;
        }
        int cursorX = 0;  //보이는 영역에서 맨처음의 x값(보이는 영역 중 가장 작은 값)
                          //cursorX: 현재 디스플레이되는 데이터의 시작점을 말한다.
        bool scrolling;  //false로 초기화
        private void T_Tick(object sender, EventArgs e)
        {
            //0.001초마다 보여주는 영역을 바꾸어주는 함수
            //(1)한 영역에서 얼만큼 보여줄 것인지 정해야한다.
            //(2)한번 실행될 때 얼마나 움직일 때 몇 칸씩 움직일 것인지 정해야한다.
            //(3)범례를 넘어가지 않도록 지정
            
            //cf. 지역변수는 항상 초기화를 시켜주어야한다.
            if (cursorX + 500 <= ecgCount)
            {
                chart1.ChartAreas["Draw"].AxisX.ScaleView.Zoom(cursorX, cursorX + 500);
            }
            else
                t.Stop();  //그렇지 않으면 화면을 정지시킨다.
            cursorX += 2;    //그래프의 이동이 2칸씩 이동한다.

        }

        private void PpgRead()
        {
            //파일에서 읽어오기
            string fileName = "../../Data/ppg.txt";
            string[] lines = File.ReadAllLines(fileName);  //파일에서 읽어와서 lines배열에 저장해라.

            //최소,최대값을 알아야함.
            //차트의 범위를 지정하기 위해
            double min = double.MaxValue;   //double에서 가장 큰 값
            double max = double.MinValue;

            //데이터의 개수가 몇개인지 모르기 때문에 for문 말고 foreach를 사용

            int i = 0;
            foreach (var line in lines)
            {
                ppg[i] = double.Parse(line);   //그래프가 겹쳐서 보이는 것을 방지하려고
                if (min > ppg[i])
                    min = ppg[i];
                if (max < ppg[i])
                    max = ppg[i];
                i++;
            }
            ppgCount = i;
            string s = string.Format("PPG: Count = {0} min = {1} max = {2}",
                ppgCount, min, max);
            MessageBox.Show(s);
        }

        private void EcgRead()
        {
            //파일에서 읽어오기
            string fileName = "../../Data/ecg.txt";
            string[] lines = File.ReadAllLines(fileName);  //파일에서 읽어와서 lines배열에 저장해라.

            //최소,최대값을 알아야함.
            //차트의 범위를 지정하기 위해
            double min = double.MaxValue;   //double에서 가장 큰 값
            double max = double.MinValue;

            //데이터의 개수가 몇개인지 모르기 때문에 for문 말고 foreach를 사용

            int i = 0;
            foreach(var line in lines)
            {
                ecg[i] = double.Parse(line)+3;
                if (min > ecg[i])
                    min = ecg[i];
                if (max < ecg[i])
                    max = ecg[i];
                i++;
            }
            ecgCount = i;
            string s = string.Format("ECG: Count = {0} min = {1} max = {2}",
                ecgCount, min, max);
            MessageBox.Show(s);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            chart1.ChartAreas.Clear();
            chart1.Series.Clear();

            chart1.ChartAreas.Add("Draw");

            chart1.ChartAreas["Draw"].BackColor = Color.White;    //차트 영역 바탕색 지정

            chart1.ChartAreas["Draw"].AxisX.Minimum = 0;   //x축의 최솟값
            chart1.ChartAreas["Draw"].AxisX.Maximum = ecgCount;   //ecg의 데이터 개수가 더 적기 때문에 데이터가  두개가 다 나올 수 있도록
            chart1.ChartAreas["Draw"].AxisX.Interval = 50;   //x축의 간격
            chart1.ChartAreas["Draw"].AxisX.MajorGrid.LineColor = Color.Black;   //Line 색깔 지정
            chart1.ChartAreas["Draw"].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;   //선의 종류

            chart1.ChartAreas["Draw"].AxisY.Minimum = -2;   //y축의 최솟값
            chart1.ChartAreas["Draw"].AxisY.Maximum = 6;   //y축의 최댓값
            chart1.ChartAreas["Draw"].AxisY.Interval =0.5 ;  //y축의 간격
            chart1.ChartAreas["Draw"].AxisY.MajorGrid.LineColor = Color.Gray;
            chart1.ChartAreas["Draw"].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            chart1.Series.Add("ECG");
            chart1.Series["ECG"].ChartType = SeriesChartType.Line;
            chart1.Series["ECG"].Color = Color.Orange;
            chart1.Series["ECG"].BorderWidth = 2;
            chart1.Series["ECG"].LegendText = "ECG";

            chart1.Series.Add("PPG");
            chart1.Series["PPG"].ChartType = SeriesChartType.Line;
            chart1.Series["PPG"].Color = Color.LightGreen;
            chart1.Series["PPG"].BorderWidth = 2;
            chart1.Series["PPG"].LegendText = "PPG";

            foreach (var v in ecg)
            {
                chart1.Series["ECG"].Points.Add(v);
            }
            foreach (var v in ppg)
            {
                chart1.Series["PPG"].Points.Add(v);
            }
        }

        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t.Start();
            scrolling = true;
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            if (scrolling == true)
            {
                t.Stop();
                scrolling = false;
            }
            else
            {
                t.Start();
                scrolling = true;
            }
        }

        private void viewAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas["Draw"].AxisX.ScaleView.Zoom(0,ecgCount);
            t.Stop();
            scrolling = false;
        }
    }
}
