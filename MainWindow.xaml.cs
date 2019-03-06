using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NAudio;
using NAudio.Wave;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace play_audio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // public ChartValues<Polyline> PolylineCollection;
        public MainWindow()
        {
            InitializeComponent();

           // StartRecording(5);
            playaudio.IsEnabled = true;
        }
        
        WaveFileReader wfr;
        Polyline pl;


        double canH = 0;
        double canW = 0;
        double plH = 0;
        double plW = 0;
        int time = 0;
        double seconds = 0;
        int numtodisplay = 2205; //No of samples displayed in a second

        public float[] getCoefficients()
        {
            string[] lines = System.IO.File.ReadAllLines(@"D:\GIT\aerowinrt\audio_use\coefficients.txt");


            string[] coefficients = new string[10];
            float[] coefficients1 = new float[10];
            foreach (string line in lines)
            {
                coefficients = line.Split(new char[] { ',' });

            }

            for (int i = 0; i < coefficients.Length; i++)
            {
                coefficients1[i] = float.Parse(coefficients[i]);
            }

            return (coefficients1);
        }


        //void StartRecording(int time)
        //{
           // wi = new WaveIn();
        //    wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailable);
        //    wi.RecordingStopped += new EventHandler<StoppedEventArgs>(wi_RecordingStopped);
        //    wi.WaveFormat = new WaveFormat(4000, 32, 2); //Downsampled audio from 44KHz to 4kHz 

        //    wfw = new WaveFileWriter(@"D:\GIT\aerowinrt\audio_use\record3.wav", wi.WaveFormat);

        //    canH = waveCanvas.Height;
        //    canW = waveCanvas.Width;

        //    pl = new Polyline();
        //    pl.Stroke = Brushes.Black;
        //    pl.Name = "waveform";
        //    pl.StrokeThickness = 1;
        //    pl.MaxHeight = canH - 4;
        //    pl.MaxWidth = canW - 4;
        //    // pl.m
        //    plH = pl.MaxHeight;
        //    plW = pl.MaxWidth;

        //    displaypts = new Queue<Point>();
        //    displaysht = new Queue<float>();

        //    wi.StartRecording();

        //    this.time = time;

        //}

        //void wi_RecordingStopped(object sender, StoppedEventArgs e)
        //{
        //    string idk;

        //    wi.Dispose();
        //    wi = null;
        //    idk = wfw.Filename;
        //    Debug.Print("idk : " + idk);
        //    //WaveFileWriter wfw2 = new WaveFileWriter(@"record3", wi.WaveFormat);
        //    //wfw.CopyTo(wfw2);
        //    wfw.Close();
        //    wfw.Dispose();
        //    wfw = null;

        //    playaudio.IsEnabled = true;
        //}

        private void playaudio_Click(object sender, RoutedEventArgs e)
        {
            float[] coefficients = new float[10];
            float[] a = new float[5];
            float[] b = new float[5];
            Queue<float> displaysht = new Queue<float>();

            canH = waveCanvas.Height;
            canW = waveCanvas.Width;

            pl = new Polyline();
            pl.Stroke = Brushes.Black;
            pl.Name = "waveform";
            pl.StrokeThickness = 1;
            pl.MaxHeight = canH - 4;
            pl.MaxWidth = canW - 4;
            // pl.m
            plH = pl.MaxHeight;
            plW = pl.MaxWidth;
            // seconds += (double)(1.0 * e.BytesRecorded / wi.WaveFormat.AverageBytesPerSecond * 1.0);

            //wfw.Write(e.Buffer, 0, e.BytesRecorded);
            //wfw.Flush();

            //if (seconds > time)
            //{
            //    wi.StopRecording();

            //    Debug.Print("stop recording");
            //}

            wfr = new WaveFileReader(@"D:\GIT\aerowinrt\audio_use\record3.wav");

            byte[] allBytes = File.ReadAllBytes(@"D:\GIT\aerowinrt\audio_use\record3.wav");
            Debug.Print("allBytes length : " + allBytes.Length);

            double secondsRecorded = (double)(1.0 * wfr.Length / wfr.WaveFormat.AverageBytesPerSecond * 1.0);
            

            byte[] shts = new byte[4];


            for (int i = 92; i < allBytes.Length - 1; i += 100)
            {
                shts[3] = allBytes[i];
                shts[2] = allBytes[i + 1];
                shts[1] = allBytes[i + 2];
                shts[0] = allBytes[i + 3];
                displaysht.Enqueue(BitConverter.ToInt32(shts, 0));
                //Debug.Print("shts[] : " + shts[0] + " " + shts[1]);
                //if (count < numtodisplay)
                //{
                //    displaysht.Enqueue(BitConverter.ToInt32(shts, 0));
                //    ++count;
                //}
                //else
                //{
                //    displaysht.Dequeue();
                //    displaysht.Enqueue(BitConverter.ToInt32(shts, 0));
                //}
            }
            this.waveCanvas.Children.Clear();
            pl.Points.Clear();
            float[] shts2 = displaysht.ToArray();
            float[] shts3 = displaysht.ToArray();

            coefficients = getCoefficients();
            for (int i = 0; i < 5; i++)
            {
                a[i] = coefficients[i];

            }
            for (int i = 5; i < coefficients.Length; i++)
            {
                b[i - 5] = coefficients[i];

            }

            for (Int32 x = 4; x < shts2.Length; x++)
            {
                //coefficients from file generated by MATLAB
                shts3[x] = ((b[0] * x) + (b[1] * (x - 1)) + (b[2] * (x - 2)) + (b[3] * (x - 3)) + (b[4] * (x - 4)) + (a[1] * shts2[x - 1]) + (a[2] * shts2[x - 2]) + (a[3] * shts2[x - 3]) + (a[4] * shts2[x - 4]));

            }




            for (Int32 x = 0; x < shts3.Length; ++x)
            {
                pl.Points.Add(Normalize(x, shts3[x]));
                //Point p1 = new Point();
                //p1.X = x;
                //p1.Y = shts3[x];
                //pl.Points.Add(p1);

            }

            this.waveCanvas.Children.Add(pl);
        }

        Point Normalize(Int32 x, float y)
        {
            Point p = new Point
            {

                X = 1.3 * x / numtodisplay * plW,
                Y = plH / 2.0 - y / (Math.Pow(2, 35.3) * 1.0) * (plH)
            };
            Debug.Print("Points added: " + p);

            return p;
        }

        //private void playaudio_Click(object sender, RoutedEventArgs e)
        //{
        //    playaudio.IsEnabled = false;
        //    waveCanvas.Children.Clear();
                        

        //}
    }
}
