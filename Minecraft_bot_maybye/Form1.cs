using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace Minecraft_bot_maybye
{
    public partial class Form1 : Form
    {

        //if u will start using  image for training use Accord

        //simulating keys
        public string directory = @"C:\Users\micha\source\repos\Minecraft_bot_maybye\Minecraft_bot_maybye\trainig data\data.txt";
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int VK_A = 0x41;
        public const int VK_D = 0x44;
        public const int VK_W = 0x57;

        //simulating W
        public static void Straight()
        {
            keybd_event(VK_W, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(50);
            keybd_event(VK_W, 0, KEYEVENTF_KEYUP, 0);
        }

        //simulating a
        public static void Left()
        {
            keybd_event(VK_W, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_A, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(50);
            keybd_event(VK_W, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_A, 0, KEYEVENTF_KEYUP, 0);
        }

        //simulating d
        public static void Right()
        {
            keybd_event(VK_W, 0, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_D, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(50);
            keybd_event(VK_W, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_D, 0, KEYEVENTF_KEYUP, 0);
        }

        //public things
        private Thread t;
        public bool t_stop = false;
        public bool a = false;
        public bool d = false;
        public bool save = false;
        public bool isTrained = false;
        public string savee = "";
        public string all = "";
        public bool captureHasStarted = false;

        public double[] tonn;

        //low level key hook
        private KeyboardInterrupt _listener;

        public Form1()
        {
            InitializeComponent();
        }

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            this.textBox1.Text += e.KeyPressed.ToString();
            if (e.KeyPressed.ToString() == "A")
            {
                a = true;
            }
            else if (e.KeyPressed.ToString() == "D")
            {
                d = true;
            }else if(e.KeyPressed.ToString() == "J")
            {
                save = true;
            }
        }

        //recording
        void Capture()
        {

            //creating neural network
            var nn = new neural_network(8,1000, 2);
            while (captureHasStarted)
            {
                //recording
                Bitmap bm = new Bitmap(Screen.PrimaryScreen.Bounds.Width / 2,Screen.PrimaryScreen.Bounds.Height / 2);
                Graphics l = Graphics.FromImage(bm);
                l.CopyFromScreen(0, 0, 0, 0, bm.Size);
                //loading image (replace with recording screen)
                Image<Bgr, byte> img = new Image<Bgr, byte>(bm).Resize(107,60, Emgu.CV.CvEnum.Inter.Linear, false);

                //convertion to grayscale
                UMat uimage = new UMat();
                CvInvoke.CvtColor(img, uimage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                //removing noise
                UMat pyrDown = new UMat();
                CvInvoke.PyrDown(uimage, pyrDown);
                CvInvoke.PyrUp(pyrDown, uimage);

                Stopwatch watch = Stopwatch.StartNew();
                //edge detection
                UMat cannyEdges = new UMat();
                double cannyThreshold = 180.0;
                double cannyThresholdLinking = 100.0;
                CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);
                //finding lines
                LineSegment2D[] lines = CvInvoke.HoughLinesP(cannyEdges, 1, Math.PI / 45.0, 20, 30, 10);
                double[] line_arr = new double[lines.Length];
                for (int i = 0; i<lines.Length; i++)
                {
                    line_arr[i] = lines[i].Length;
                }

                //grabbing data
                //finding 2 longest lines
                if (line_arr.Length >= 2)
                {
                    double[] input;
                    int first_line = Array.IndexOf(line_arr, line_arr.Max());
                    Array.Clear(line_arr, first_line, 1);
                    int second_line = Array.IndexOf(line_arr, line_arr.Max());

                    //putting longer line on the right place in training
                    if(lines[second_line].P1.X >= lines[first_line].P1.X && lines[second_line].P1.X < lines[second_line].P2.X && lines[first_line].P1.X > lines[first_line].P2.X)
                    {
                        img.Draw(lines[first_line], new Bgr(Color.Green), 2);
                        img.Draw(lines[second_line], new Bgr(Color.Red), 2);
                        //first line is left
                        //second line should be right
                        input = new double[]{ lines[first_line].P1.X, lines[first_line].P1.Y, lines[first_line].P2.X, lines[first_line].P2.Y, lines[second_line].P1.X, lines[second_line].P1.Y, lines[second_line].P2.X, lines[second_line].P2.Y };
                    }
                    else 
                    {
                        img.Draw(lines[first_line], new Bgr(Color.Red), 2);
                        img.Draw(lines[second_line], new Bgr(Color.Green), 2);
                        //this sould be reverse
                        input = new double[] { lines[second_line].P1.X, lines[second_line].P1.Y, lines[second_line].P2.X, lines[second_line].P2.Y, lines[first_line].P1.X, lines[first_line].P1.Y, lines[first_line].P2.X, lines[first_line].P2.Y };
                    }
                    tonn = input;
                }
                //putting one line on the right place
                else if (line_arr.Length >= 1)
                {
                    double[] input;
                    int first_line = Array.IndexOf(line_arr, line_arr.Max());
                    if(lines[first_line].P1.X >= lines[first_line].P2.X)
                    {
                        img.Draw(lines[first_line], new Bgr(Color.Green), 2);
                        input = new double[]{ lines[first_line].P1.X, lines[first_line].P1.Y, lines[first_line].P2.X, lines[first_line].P2.Y, 0, 0, 0, 0 };
                    }
                    else
                    {
                        img.Draw(lines[first_line], new Bgr(Color.Red), 2);
                        input = new double[] { 0, 0, 0, 0 , lines[first_line].P1.X, lines[first_line].P1.Y, lines[first_line].P2.X, lines[first_line].P2.Y};
                    }
                    tonn = input;
                }
                else
                {
                    double[] input = { 0,0,0,0,0,0,0,0 };
                    tonn = input;
                }
                //ImageToArray conv = new ImageToArray(min: 0, max: 1);
                double[] target = Keys(a,d);
                a = false; d = false;

                //training model
                if(t_stop == false)
                {
                    all += ToStringos(target);
                    // nn.Train(tonn, target);
                    if (save == true)
                    {
                        string before  = File.ReadAllText(directory);
                        all += before;
                        File.WriteAllText(directory, all);
                        save = false;
                    }
                    else
                    {
                        //nn.Train(tonn, target);
                    }
                }else if(t_stop == true){
                    if (!isTrained)
                    {
                        //text to 2x double[]
                        string test = "";
                        //train data from file
                        string todo = File.ReadAllText(directory);
                        //method for unvrapping every element xd
                        string[] needAnotherAction = UnWrapp.ToIndividual(todo);
                        // just looping all of that 100 times to make it more precise

                        //debug
                        int counter = 0;
                        int TrainingSessions = 1;
                        Console.WriteLine("started training");
                        for (int iteration = 0; iteration < TrainingSessions; iteration++)
                        {
                            foreach (string s in needAnotherAction)
                            {
                                double a = UnWrapp.ToNum(s, 'a');
                                double b = UnWrapp.ToNum(s, 'b');
                                double c = UnWrapp.ToNum(s, 'c');
                                double d = UnWrapp.ToNum(s, 'd');
                                double e = UnWrapp.ToNum(s, 'e');
                                double f = UnWrapp.ToNum(s, 'f');
                                double g = UnWrapp.ToNum(s, 'g');
                                double h = UnWrapp.ToNum(s, 'h');
                                double i = UnWrapp.ToNum(s, 'i');
                                double j = UnWrapp.ToNum(s, 'j');
                                double[] inputs = { a, b, c, d, e, f, g, h };
                                double[] answers = { i, j };
                                nn.Train(inputs, answers);

                                //debug
                                counter++;
                                Console.WriteLine("trained " + counter + " out of " + needAnotherAction.Length * TrainingSessions);
                            }
                        }
                        isTrained = true;
                    }
                    //feedforwarding
                    else if (isTrained)
                    {
                        var donen = nn.feedforward(tonn).ToArray();
                        double[] vysledek = new double[2];
                        for (int i = 0; i < donen.Length; i++)
                        {
                            vysledek[i] = donen[i, 0];
                        }
                        if (vysledek.Max() >= 0.5)
                        {
                            //turning left
                            if (Array.IndexOf(vysledek, vysledek.Max()) == 0)
                            {
                                Left();
                            }
                            //turning right
                            else if (Array.IndexOf(vysledek, vysledek.Max()) == 1)
                            {
                                Right();
                            }
                        }
                        //going straight
                        else
                        {
                            Straight();
                        }
                    }
                    //t_stop = false;
                }
                //last things
                watch.Stop();
                pictureBox1.Image = img.Bitmap;
                Thread.Sleep(50);
            }
        }

        //Start button for recording
        private void button1_Click(object sender, EventArgs e)
        {
            t = new Thread(Capture);
            t.Start();
            captureHasStarted = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        //low level key hook *i du now ho tha work
        private void Form1_Load(object sender, EventArgs e)
        {
            _listener = new KeyboardInterrupt();
            _listener.OnKeyPressed += _listener_OnKeyPressed;

            _listener.HookKeyboard();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _listener.UnHookKeyboard();
            if (captureHasStarted)
            {
                captureHasStarted = false;
                try
                {
                    t.Abort();
                    t.Join();
                    t = null;
                }catch(Exception ex)
                {

                }
            }
            Application.Exit();
        }

        //key presses to int
        public static double[] Keys(bool a, bool d)
        {
            double a_ = (a == true) ? 1 : 0;
            double d_ = (d == true) ? 1 : 0;

            double[] fin = {a_, d_ };
            return fin;
        }

        //doubles to string
        public string ToStringos(double[] keys)
        {
            string first_hodnota = tonn[0].ToString();
            string second_hodnota = tonn[1].ToString();
            string third_hodnota = tonn[2].ToString();
            string fourth_hodnota = tonn[3].ToString();
            string fifth_hodnota = tonn[4].ToString();
            string sixth_hodnota = tonn[5].ToString();
            string seventh_hodnota = tonn[6].ToString();
            string eight_hodnota = tonn[7].ToString();

            //user inputs
            string nineth_hodnota = keys[0].ToString();
            string tenth_hodnota = keys[1].ToString();

            string s = first_hodnota +"/a" + second_hodnota+"/b"+ third_hodnota +"/c"+ fourth_hodnota +"/d"+ fifth_hodnota +"/e"+ sixth_hodnota +"/f"+ seventh_hodnota +"/g"+ eight_hodnota +"/h"+ nineth_hodnota +"/i"+ tenth_hodnota+"/j";

            return s;
        }

        //button to start training
        private void button2_Click(object sender, EventArgs e)
        {
            t_stop = true;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            if (captureHasStarted)
            {
                captureHasStarted = false;
                try
                {
                    t.Abort();
                    t.Join();
                    t = null;
                }
                catch (Exception ex)
                {

                }
            }
            Application.Exit();
        }
    }
}
