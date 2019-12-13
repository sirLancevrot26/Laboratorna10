using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OMultMediaLab1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddCBXItems();

            dictMedianIntersectColorCount.Add("2", 1);

            dictMedianIntersectColorCount.Add("8", 3);

            dictMedianIntersectColorCount.Add("16", 4);

            dictMedianIntersectColorCount.Add("32", 5);

            dictMedianIntersectColorCount.Add("64", 6);

            dictMedianIntersectColorCount.Add("128", 7);
        }

        private Uri imgSource = null;

        public Bitmap b = null;

        private int amountOfAllColors = 0, colorsBit;

        List<System.Drawing.Color> listColors = null;

        Dictionary<string, int> dictMedianIntersectColorCount = new Dictionary<string, int>();

        private const int OldImageShaders = 256;

        private void EraseTBProgress()
        {
            PGBAmountColors.Value = 0;

            TBLProgress.Text = "0%";
        }

        private void AmountOfColors()
        {
            List<System.Drawing.Color> listColRes = new List<System.Drawing.Color>();

            listColRes.Add(listColors[0]);

            int color1Per = listColors.Count() / 1000;

            int countColor = 0;

            double x = 0;

            foreach (System.Drawing.Color c in listColors)
            {
                bool verColor = false;

                countColor++;

                if (countColor == color1Per)
                {
                    x += 0.1;
                    Dispatcher.Invoke((ThreadStart)delegate { PGBAmountColors.Value += 0.1; });
                    Dispatcher.Invoke((ThreadStart)delegate { TBLProgress.Text = Math.Round(x, 1).ToString() + "%"; });

                    countColor = 0;
                }

                foreach (System.Drawing.Color cRes in listColRes)
                {
                    if (cRes == c)
                    {
                        verColor = true;
                    }
                }

                if (verColor == false)
                {
                    listColRes.Add(c);
                }
            }

            amountOfAllColors = listColRes.Count();

            Dispatcher.Invoke((ThreadStart)delegate { LBLResAmountColors.Content = amountOfAllColors; });

            Dispatcher.Invoke((ThreadStart)delegate { PGBAmountColors.Visibility = Visibility.Hidden; });

            Dispatcher.Invoke((ThreadStart)delegate { TBLProgress.Visibility = Visibility.Hidden; });

            Dispatcher.Invoke((ThreadStart)delegate { BTNCountColors.IsEnabled = true; });

            Dispatcher.Invoke((ThreadStart)delegate { EraseTBProgress(); LBLResAmountColors.Visibility = Visibility.Visible; });
        }

        private async void CountAsyncCol(List<System.Drawing.Color> listColors)
        {
            await Task.Run(() => AmountOfColors());
        }

        private List<System.Drawing.Color> listColorsFirst(Bitmap b)
        {
            listColors = new List<System.Drawing.Color>();

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    listColors.Add(b.GetPixel(i, j));
                }
            }

            return listColors;
        }

        private void AddCBXItems()
        {
            for (int i = 4; i <= 256; i++)
            {
                ComboBoxItem c = new ComboBoxItem();

                c.Content = i.ToString();

                CBXSelectedBit.Items.Add(c);
            }

            for (int i = 0; i < 256; i++)
            {
                ComboBoxItem c = new ComboBoxItem();

                c.Content = i.ToString();

                CBXSelectedBinPorigMin.Items.Add(c);

                ComboBoxItem x = new ComboBoxItem();

                x.Content = i.ToString();

                CBXSelectedBinPorigMax.Items.Add(x);
            }
        }

        private void ChoseImageBTN(object sender, RoutedEventArgs e)
        {
            MERes.Visibility = Visibility.Hidden;

            OpenFileDialog fileDialog = new OpenFileDialog();

            var res = fileDialog.ShowDialog();

            if (res == true)
            {
                imgSource = new Uri(fileDialog.FileName);

                if (imgSource != null)
                {
                    ME.Source = imgSource;

                    b = new Bitmap(fileDialog.FileName);

                    LBLSizeImage.Content = "Розміри зображення(ширина*висота): " + b.Width + "x" + b.Height;

                    Regex reg = new Regex(@"\d{1,}");

                    colorsBit = int.Parse(reg.Match(b.PixelFormat.ToString()).ToString());

                    LBLColorDepth.Content = "Глибина кольору: " + colorsBit + " біт.";

                    LBLColorDepth.Visibility = Visibility.Visible;

                    listColorsFirst(b);

                    GRDSElectBitKvant.Visibility = Visibility.Visible;
                    BTNCountColors.Visibility = Visibility.Visible;
                    BTNAlgRivnKvant.Visibility = Visibility.Visible;
                    BTNAlgMedianIntersection.Visibility = Visibility.Visible;
                    GRDSelectMedianInter.Visibility = Visibility.Visible;
                    GRDBin.Visibility = Visibility.Visible;
                    GRDBinPorogi.Visibility = Visibility.Visible;
                    BTNBinPorogi.Visibility = Visibility.Visible;
                    GRDFloydSteynberg.Visibility = Visibility.Visible;
                    GRDGauss.Visibility = Visibility.Visible;
                }
            }
        }

        private void BTNAmountColors(object sender, RoutedEventArgs e)
        {
            GRDAmountColors.Visibility = Visibility.Visible;
            PGBAmountColors.Visibility = Visibility.Visible;
            TBLProgress.Visibility = Visibility.Visible;
            LBLResAmountColors.Visibility = Visibility.Hidden;
            BTNCountColors.IsEnabled = false;
            CountAsyncCol(listColorsFirst(b));
        }

        private void RivnKvantAlgorithm(int NewCountShaders)
        {
            Bitmap imgRivnKvant = new Bitmap(b.Width, b.Height);

            Color c;

            List<int> pallete = new List<int>();

            for (int i = 0; i < NewCountShaders; i++)
            {
                pallete.Add(i * (OldImageShaders / NewCountShaders));
            }

            int Red = 0;

            int Green = 0;

            int Blue = 0;

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double x = 0;

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    bool flagRed = false, flagGreen = false, flagBlue = false;

                    cou++;
                    if (cou == count0_1Per)
                    {
                        x += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (x >= 100)
                            {
                                x = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(x, 1) + "%";
                        });

                        cou = 0;
                    }

                    for (int p = 0; p <= NewCountShaders; p++)
                    {
                        if (p < NewCountShaders - 1)
                        {
                            if (b.GetPixel(i, j).R > pallete[p] && b.GetPixel(i, j).R < (pallete[p] + pallete[p + 1]) / 2 && flagRed == false)
                            {
                                Red = pallete[p];

                                flagRed = true;
                            }

                            if (b.GetPixel(i, j).R <= pallete[p + 1] && b.GetPixel(i, j).R >= (pallete[p] + pallete[p + 1]) / 2 && flagRed == false)
                            {
                                Red = pallete[p + 1];

                                flagRed = true;
                            }

                            if (b.GetPixel(i, j).G > pallete[p] && b.GetPixel(i, j).G < (pallete[p] + pallete[p + 1]) / 2 && flagGreen == false)
                            {
                                Green = pallete[p];

                                flagGreen = true;
                            }

                            if (b.GetPixel(i, j).G <= pallete[p + 1] && b.GetPixel(i, j).G >= (pallete[p] + pallete[p + 1]) / 2 && flagGreen == false)
                            {
                                Green = pallete[p + 1];

                                flagGreen = true;
                            }

                            if (b.GetPixel(i, j).B > pallete[p] && b.GetPixel(i, j).B < (pallete[p] + pallete[p + 1]) / 2 && flagBlue == false)
                            {
                                Blue = pallete[p];

                                flagBlue = true;
                            }

                            if (b.GetPixel(i, j).B <= pallete[p + 1] && b.GetPixel(i, j).B >= (pallete[p] + pallete[p + 1]) / 2 && flagBlue == false)
                            {
                                Blue = pallete[p + 1];

                                flagBlue = true;
                            }

                            if (flagBlue == true && flagGreen == true && flagRed == true)
                            {
                                break;
                            }

                        }
                    }

                    c = Color.FromArgb(255, Red, Green, Blue);

                    imgRivnKvant.SetPixel(i, j, c);
                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgRivnKvant.Save("D:/file.jpg");

            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму рівномірного квантування";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {

                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });

        }

        private async void AsyncRivnKvantAlgorithm(int newShaders)
        {
            await Task.Run(() => RivnKvantAlgorithm(newShaders));
        }

        private void BTNRivnKvant(object sender, RoutedEventArgs e)
        {
            int NewCountShaders = int.Parse(CBXSelectedBit.Text);
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            AsyncRivnKvantAlgorithm(NewCountShaders);

        }

        private void MedianIntersection(string newShaders)
        {
            Bitmap imgMedianInt = new Bitmap(b.Width, b.Height);

            List<int> listRed = new List<int>();

            List<int> listGreen = new List<int>();

            List<int> listBlue = new List<int>();

            List<System.Drawing.Color> listColor = new List<Color>();

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    listRed.Add(b.GetPixel(i, j).R);
                    listGreen.Add(b.GetPixel(i, j).G);
                    listBlue.Add(b.GetPixel(i, j).B);

                    Color c = Color.FromArgb(255, b.GetPixel(i, j).R, b.GetPixel(i, j).G, b.GetPixel(i, j).B);

                    listColor.Add(c);


                }
            }

            int diapRed = listRed.Max() - listRed.Min();
            int diapGreen = listGreen.Max() - listGreen.Min();
            int diapBlue = listBlue.Max() - listBlue.Min();

            bool diapRedMaxValueFlag = diapRed >= diapGreen && diapRed >= diapBlue ? true : false;
            bool diapGreenMaxValueFlag = diapGreen >= diapRed && diapGreen >= diapBlue ? true : false;
            bool diapBlueMaxValueFlag = diapBlue >= diapRed && diapBlue >= diapGreen ? true : false;

            List<System.Drawing.Color> listColorSorted = new List<Color>();

            if (diapRedMaxValueFlag == true)
            {
                listColorSorted = listColor.OrderBy(x => x.R).ToList();
            }

            if (diapGreenMaxValueFlag == true && diapRedMaxValueFlag == false)
            {
                listColorSorted = listColor.OrderBy(x => x.G).ToList();
            }

            if (diapBlueMaxValueFlag == true && diapGreenMaxValueFlag == false && diapRedMaxValueFlag == false)
            {
                listColorSorted = listColor.OrderBy(x => x.B).ToList();
            }


            List<System.Drawing.Color> pallete = new List<Color>();

            List<List<System.Drawing.Color>> listSegmentsOfColor = new List<List<Color>>();

            int NewCountColors = dictMedianIntersectColorCount.Where(c => c.Key == newShaders).Select(x => x.Value).SingleOrDefault();

            listSegmentsOfColor.Add(listColorSorted);

            List<List<System.Drawing.Color>> listSegmentsOfColorAdditional = new List<List<Color>>();


            for (int i = 0; i < NewCountColors; i++)
            {
                listSegmentsOfColorAdditional = new List<List<Color>>();

                foreach (var s in listSegmentsOfColor)
                {
                    int serIndex = s.Count / 2;
                    List<System.Drawing.Color> list = new List<Color>();

                    for (int i1 = 0; i1 < serIndex; i1++)
                    {
                        list.Add(s[i1]);
                    }

                    listSegmentsOfColorAdditional.Add(list);

                    list = new List<Color>();

                    for (int i1 = serIndex; i1 < s.Count; i1++)
                    {
                        list.Add(s[i1]);
                    }

                    listSegmentsOfColorAdditional.Add(list);
                }

                listSegmentsOfColor = listSegmentsOfColorAdditional;

            }

            foreach (var segment in listSegmentsOfColor)
            {
                int avgRedSegment = 0, sumRed = 0;
                int avgGreenSegment = 0, sumGreen = 0;
                int avgBlueSegment = 0, sumBlue = 0;

                for (int r = 0; r < segment.Count; r++)
                {
                    sumRed += segment[r].R;

                    sumGreen += segment[r].G;

                    sumBlue += segment[r].B;
                }

                avgRedSegment = sumRed / segment.Count;

                avgGreenSegment = sumGreen / segment.Count;

                avgBlueSegment = sumBlue / segment.Count;

                Color c = Color.FromArgb(255, avgRedSegment, avgGreenSegment, avgBlueSegment);

                pallete.Add(c);

                MessageBox.Show("Green: " + c.G);
            }

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            int Red = 0, Green = 0, Blue = 0;

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    bool flagRed = false, flagGreen = false, flagBlue = false;

                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    for (int p = 0; p < pallete.Count; p++)
                    {
                        if (p < pallete.Count - 1)
                        {
                            if (diapRedMaxValueFlag == true)
                            {
                                if (b.GetPixel(i, j).R > pallete[p].R && b.GetPixel(i, j).R < (pallete[p].R + pallete[p + 1].R) / 2 && flagRed == false)
                                {
                                    Red = pallete[p].R;
                                    Green = pallete[p].G;
                                    Blue = pallete[p].B;

                                    flagRed = true;
                                }

                                if (b.GetPixel(i, j).R <= pallete[p + 1].R && b.GetPixel(i, j).R >= (pallete[p].R + pallete[p + 1].R) / 2 && flagRed == false)
                                {
                                    Red = pallete[p + 1].R;
                                    Green = pallete[p + 1].G;
                                    Blue = pallete[p + 1].B;

                                    flagRed = true;
                                }

                                if (flagRed == true)
                                {
                                    break;
                                }
                            }

                            if (diapGreenMaxValueFlag == true)
                            {
                                if (b.GetPixel(i, j).G > pallete[p].G && b.GetPixel(i, j).G < (pallete[p].G + pallete[p + 1].G) / 2 && flagRed == false)
                                {
                                    Red = pallete[p].R;
                                    Green = pallete[p].G;
                                    Blue = pallete[p].B;

                                    flagGreen = true;
                                }

                                if (b.GetPixel(i, j).G <= pallete[p + 1].G && b.GetPixel(i, j).G >= (pallete[p].G + pallete[p + 1].G) / 2 && flagRed == false)
                                {
                                    Red = pallete[p + 1].R;
                                    Green = pallete[p + 1].G;
                                    Blue = pallete[p + 1].B;

                                    flagGreen = true;
                                }

                                if (flagGreen == true)
                                {
                                    break;
                                }
                            }

                            if (diapBlueMaxValueFlag == true)
                            {
                                if (b.GetPixel(i, j).B > pallete[p].B && b.GetPixel(i, j).B < (pallete[p].B + pallete[p + 1].B) / 2 && flagRed == false)
                                {
                                    Red = pallete[p].R;
                                    Green = pallete[p].G;
                                    Blue = pallete[p].B;

                                    flagBlue = true;
                                }

                                if (b.GetPixel(i, j).B <= pallete[p + 1].B && b.GetPixel(i, j).B >= (pallete[p].B + pallete[p + 1].B) / 2 && flagRed == false)
                                {
                                    Red = pallete[p + 1].R;
                                    Green = pallete[p + 1].G;
                                    Blue = pallete[p + 1].B;

                                    flagBlue = true;
                                }

                                if (flagBlue == true)
                                {
                                    break;
                                }
                            }

                        }
                    }

                    Color c = Color.FromArgb(255, Red, Green, Blue);

                    imgMedianInt.SetPixel(i, j, c);
                }
            }


            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgMedianInt.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {

                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }

        private async void AsyncMedianIntAlgorithm(string newShaders)
        {
            await Task.Run(() => MedianIntersection(newShaders));
        }

        private void BTNAlgMedianIntersection_Click(object sender, RoutedEventArgs e)
        {
            string newAmountColor = CBXSelectedMedianInter.Text;

            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;
            AsyncMedianIntAlgorithm(newAmountColor);
        }

        public void WhiteNoise()
        {
            Random r = new Random();

            Bitmap imgMedianInt = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }


                    int newRand = r.Next(0, 256);

                    int Red = (b.GetPixel(i, j).R + newRand) / 2;

                    int Green = (b.GetPixel(i, j).G + newRand) / 2;

                    int Blue = (b.GetPixel(i, j).B + newRand) / 2;

                    Color c = Color.FromArgb(255, Red, Green, Blue);

                    imgMedianInt.SetPixel(i, j, c);
                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgMedianInt.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {

                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }

        private async void AsyncWhiteNoiseAlgorithm()
        {
            await Task.Run(() => WhiteNoise());
        }

        private void BTNWhiteNoise(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;
            AsyncWhiteNoiseAlgorithm();
        }

        private void BinPogori(int minPorog, int maxPorog)
        {
            Bitmap imgMedianInt = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    int Red = 0, Green = 0, Blue = 0;

                    int AVGValue = (b.GetPixel(i, j).R + b.GetPixel(i, j).G + b.GetPixel(i, j).B) / 3;

                    if (AVGValue < maxPorog && AVGValue > minPorog)
                    {
                        AVGValue = 0;
                    }

                    else
                    {
                        AVGValue = 255;
                    }

                    Color c = Color.FromArgb(255, AVGValue, AVGValue, AVGValue);

                    imgMedianInt.SetPixel(i, j, c);
                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgMedianInt.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {

                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }

        private async void AsyncBinPorogi(int minPorog, int maxPorog)
        {
            await Task.Run(() => BinPogori(minPorog, maxPorog));
        }

        private void BTNBinPorogi_Click(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            int min = int.Parse(CBXSelectedBinPorigMin.Text);
            int max = int.Parse(CBXSelectedBinPorigMax.Text);

            AsyncBinPorogi(min, max);
        }

        private void AlgorithmFloydSteynberg()
        {
            Bitmap imgMedianInt = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;


            int porigIntens = 128;

            for (int i = 0; i < b.Width - 1; i += 1)
            {
                for (int j = 0; j < b.Height - 1; j += 1)
                {

                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    Color c;

                    #region First

                    int intenceRed = b.GetPixel(i, j).R;

                    int intenceGreen = b.GetPixel(i, j).G;

                    int intenceBlue = b.GetPixel(i, j).B;

                    int ErrorRed = 0, ErrorGreen = 0, ErrorBlue = 0;

                    //if (intensMax < porigIntens)
                    //{
                    //    //c = Color.FromArgb(255, 0, 0, 0);

                    //    Error = porigIntens;

                    //    //imgMedianInt.SetPixel(i, j, c);
                    //}

                    //if (intensMax > porigIntens)
                    //{
                    //    //c = Color.FromArgb(255, 255, 255, 255);

                    //    Error = porigIntens - intensMax;

                    //    //imgMedianInt.SetPixel(i, j, c);
                    //}


                    if (intenceRed > porigIntens)
                    {
                        ErrorRed = intenceRed;
                    }

                    if (intenceRed < porigIntens)
                    {
                        ErrorRed = intenceRed - porigIntens;
                    }

                    if (intenceGreen > porigIntens)
                    {
                        ErrorGreen = intenceGreen;
                    }

                    if (intenceGreen < porigIntens)
                    {
                        ErrorGreen = intenceGreen - porigIntens;
                    }

                    if (intenceBlue > porigIntens)
                    {
                        ErrorBlue = intenceBlue;
                    }

                    if (intenceBlue < porigIntens)
                    {
                        ErrorBlue = intenceBlue - porigIntens;
                    }


                    if (b.Width - 1 > i)
                    {
                        int Red = b.GetPixel(i + 1, j).R + ErrorRed / 16 * 7;

                        int Green = b.GetPixel(i + 1, j).G + ErrorGreen / 16 * 7;

                        int Blue = b.GetPixel(i + 1, j).B + ErrorBlue / 16 * 7;

                        // MessageBox.Show("Error: " + intenceRed / 16 * 7 + " Red: " + Red);

                        //MessageBox.Show("Red: " + Red + " Green: " + Green + " Blue: " + Blue);


                        if (Red > 255)
                        {
                            Red = 255;
                        }

                        if (Green > 255)
                        {
                            Green = 255;
                        }

                        if (Blue > 255)
                        {
                            Blue = 255;
                        }

                        if (Red < 0)
                        {
                            Red = 0;
                        }

                        if (Green < 0)
                        {
                            Green = 0;
                        }

                        if (Blue < 0)
                        {
                            Blue = 0;
                        }


                        c = Color.FromArgb(255, Red, Green, Blue);

                        imgMedianInt.SetPixel(i + 1, j, c);
                    }

                    if (b.Height - 1 > j)
                    {
                        int Red = b.GetPixel(i, j + 1).R + ErrorRed / 16 * 5;

                        int Green = b.GetPixel(i, j + 1).G + ErrorGreen / 16 * 5;

                        int Blue = b.GetPixel(i, j + 1).B + ErrorBlue / 16 * 5;

                        if (Red > 255)
                        {
                            Red = 255;
                        }

                        if (Green > 255)
                        {
                            Green = 255;
                        }

                        if (Blue > 255)
                        {
                            Blue = 255;
                        }

                        if (Red < 0)
                        {
                            Red = 0;
                        }

                        if (Green < 0)
                        {
                            Green = 0;
                        }

                        if (Blue < 0)
                        {
                            Blue = 0;
                        }

                        c = Color.FromArgb(255, Red, Green, Blue);

                        imgMedianInt.SetPixel(i, j + 1, c);
                    }

                    if (b.Height - 1 > j && b.Width - 1 > i)
                    {

                        int Red = b.GetPixel(i + 1, j + 1).R + ErrorRed / 16;

                        int Green = b.GetPixel(i + 1, j + 1).G + ErrorGreen / 16;

                        int Blue = b.GetPixel(i + 1, j + 1).B + ErrorBlue / 16;

                        if (Red > 255)
                        {
                            Red = 255;
                        }

                        if (Green > 255)
                        {
                            Green = 255;
                        }

                        if (Blue > 255)
                        {
                            Blue = 255;
                        }

                        if (Red < 0)
                        {
                            Red = 0;
                        }

                        if (Green < 0)
                        {
                            Green = 0;
                        }

                        if (Blue < 0)
                        {
                            Blue = 0;
                        }

                        c = Color.FromArgb(255, Red, Green, Blue);

                        imgMedianInt.SetPixel(i + 1, j + 1, c);
                    }

                    if (j > 0 && i < b.Height - 1)
                    {

                        int Red = b.GetPixel(i + 1, j - 1).R + ErrorRed / 16 * 3;

                        int Green = b.GetPixel(i + 1, j - 1).G + ErrorGreen / 16 * 3;

                        int Blue = b.GetPixel(i + 1, j - 1).B + ErrorBlue / 16 * 3;

                        if (Red > 255)
                        {
                            Red = 255;
                        }

                        if (Green > 255)
                        {
                            Green = 255;
                        }

                        if (Blue > 255)
                        {
                            Blue = 255;
                        }

                        if (Red < 0)
                        {
                            Red = 0;
                        }

                        if (Green < 0)
                        {
                            Green = 0;
                        }

                        if (Blue < 0)
                        {
                            Blue = 0;
                        }

                        c = Color.FromArgb(255, Red, Green, Blue);

                        imgMedianInt.SetPixel(i + 1, j - 1, c);
                    }
                    #endregion

                    #region Second

                    //if (intensMax < porigIntens)
                    //{
                    //    //c = Color.FromArgb(255, 0, 0, 0);

                    //    Error = porigIntens;

                    //    //imgMedianInt.SetPixel(i, j, c);
                    //}

                    //if (intensMax > porigIntens)
                    //{
                    //    //c = Color.FromArgb(255, 255, 255, 255);

                    //    Error = porigIntens - intensMax;

                    //    //imgMedianInt.SetPixel(i, j, c);
                    //}



                    //if (b.Width - 1 > i)
                    //{
                    //    int intenceRed = b.GetPixel(i + 1, j).R;

                    //    int intenceGreen = b.GetPixel(i + 1, j).G;

                    //    int intenceBlue = b.GetPixel(i + 1, j).B;

                    //    int ErrorRed = 0, ErrorGreen = 0, ErrorBlue = 0;

                    //    if (intenceRed > porigIntens)
                    //    {
                    //        ErrorRed = intenceRed;
                    //    }

                    //    if (intenceRed < porigIntens)
                    //    {
                    //        ErrorRed = intenceRed - porigIntens;
                    //    }

                    //    if (intenceGreen > porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen;
                    //    }

                    //    if (intenceGreen < porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen - porigIntens;
                    //    }

                    //    if (intenceBlue > porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue;
                    //    }

                    //    if (intenceBlue < porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue - porigIntens;
                    //    }

                    //    int Red = b.GetPixel(i + 1, j).R + intenceRed / 16 * 7;

                    //    int Green = b.GetPixel(i + 1, j).G + intenceGreen / 16 * 7;

                    //    int Blue = b.GetPixel(i + 1, j).B + intenceBlue / 16 * 7;

                    //    // MessageBox.Show("Error: " + intenceRed / 16 * 7 + " Red: " + Red);

                    //    //MessageBox.Show("Red: " + Red + " Green: " + Green + " Blue: " + Blue);


                    //    if (Red > 255)
                    //    {
                    //        Red = 255;
                    //    }

                    //    if (Green > 255)
                    //    {
                    //        Green = 255;
                    //    }

                    //    if (Blue > 255)
                    //    {
                    //        Blue = 255;
                    //    }

                    //    if (Red < 0)
                    //    {
                    //        Red = 0;
                    //    }

                    //    if (Green < 0)
                    //    {
                    //        Green = 0;
                    //    }

                    //    if (Blue < 0)
                    //    {
                    //        Blue = 0;
                    //    }


                    //    c = Color.FromArgb(255, Red, Green, Blue);

                    //    imgMedianInt.SetPixel(i + 1, j, c);
                    //}

                    //if (b.Height - 1 > j)
                    //{
                    //    int intenceRed = b.GetPixel(i, j + 1).R;

                    //    int intenceGreen = b.GetPixel(i, j + 1).G;

                    //    int intenceBlue = b.GetPixel(i, j + 1).B;

                    //    int ErrorRed = 0, ErrorGreen = 0, ErrorBlue = 0;

                    //    if (intenceRed > porigIntens)
                    //    {
                    //        ErrorRed = intenceRed;
                    //    }

                    //    if (intenceRed < porigIntens)
                    //    {
                    //        ErrorRed = intenceRed - porigIntens;
                    //    }

                    //    if (intenceGreen > porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen;
                    //    }

                    //    if (intenceGreen < porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen - porigIntens;
                    //    }

                    //    if (intenceBlue > porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue;
                    //    }

                    //    if (intenceBlue < porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue - porigIntens;
                    //    }

                    //    int Red = b.GetPixel(i, j + 1).R + intenceRed / 16 * 5;

                    //    int Green = b.GetPixel(i, j + 1).G + intenceGreen / 16 * 5;

                    //    int Blue = b.GetPixel(i, j + 1).B + intenceBlue / 16 * 5;

                    //    if (Red > 255)
                    //    {
                    //        Red = 255;
                    //    }

                    //    if (Green > 255)
                    //    {
                    //        Green = 255;
                    //    }

                    //    if (Blue > 255)
                    //    {
                    //        Blue = 255;
                    //    }

                    //    if (Red < 0)
                    //    {
                    //        Red = 0;
                    //    }

                    //    if (Green < 0)
                    //    {
                    //        Green = 0;
                    //    }

                    //    if (Blue < 0)
                    //    {
                    //        Blue = 0;
                    //    }

                    //    c = Color.FromArgb(255, Red, Green, Blue);

                    //    imgMedianInt.SetPixel(i, j + 1, c);
                    //}

                    //if (b.Height - 1 > j && b.Width - 1 > i)
                    //{
                    //    int intenceRed = b.GetPixel(i + 1, j + 1).R;

                    //    int intenceGreen = b.GetPixel(i + 1, j + 1).G;

                    //    int intenceBlue = b.GetPixel(i + 1, j + 1).B;

                    //    int ErrorRed = 0, ErrorGreen = 0, ErrorBlue = 0;

                    //    if (intenceRed > porigIntens)
                    //    {
                    //        ErrorRed = intenceRed;
                    //    }

                    //    if (intenceRed < porigIntens)
                    //    {
                    //        ErrorRed = intenceRed - porigIntens;
                    //    }

                    //    if (intenceGreen > porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen;
                    //    }

                    //    if (intenceGreen < porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen - porigIntens;
                    //    }

                    //    if (intenceBlue > porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue;
                    //    }

                    //    if (intenceBlue < porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue - porigIntens;
                    //    }
                    //    int Red = b.GetPixel(i + 1, j + 1).R + intenceRed / 16;

                    //    int Green = b.GetPixel(i + 1, j + 1).G + intenceGreen / 16;

                    //    int Blue = b.GetPixel(i + 1, j + 1).B + intenceBlue / 16;

                    //    if (Red > 255)
                    //    {
                    //        Red = 255;
                    //    }

                    //    if (Green > 255)
                    //    {
                    //        Green = 255;
                    //    }

                    //    if (Blue > 255)
                    //    {
                    //        Blue = 255;
                    //    }

                    //    if (Red < 0)
                    //    {
                    //        Red = 0;
                    //    }

                    //    if (Green < 0)
                    //    {
                    //        Green = 0;
                    //    }

                    //    if (Blue < 0)
                    //    {
                    //        Blue = 0;
                    //    }

                    //    c = Color.FromArgb(255, Red, Green, Blue);

                    //    imgMedianInt.SetPixel(i + 1, j + 1, c);
                    //}

                    //if (j > 0 && i < b.Height - 1)
                    //{
                    //    int intenceRed = b.GetPixel(i + 1, j - 1).R;

                    //    int intenceGreen = b.GetPixel(i + 1, j - 1).G;

                    //    int intenceBlue = b.GetPixel(i + 1, j - 1).B;

                    //    int ErrorRed = 0, ErrorGreen = 0, ErrorBlue = 0;

                    //    if (intenceRed > porigIntens)
                    //    {
                    //        ErrorRed = intenceRed;
                    //    }

                    //    if (intenceRed < porigIntens)
                    //    {
                    //        ErrorRed = intenceRed - porigIntens;
                    //    }

                    //    if (intenceGreen > porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen;
                    //    }

                    //    if (intenceGreen < porigIntens)
                    //    {
                    //        ErrorGreen = intenceGreen - porigIntens;
                    //    }

                    //    if (intenceBlue > porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue;
                    //    }

                    //    if (intenceBlue < porigIntens)
                    //    {
                    //        ErrorBlue = intenceBlue - porigIntens;
                    //    }
                    //    int Red = b.GetPixel(i + 1, j - 1).R + intenceRed / 16 * 3;

                    //    int Green = b.GetPixel(i + 1, j - 1).G + intenceGreen / 16 * 3;

                    //    int Blue = b.GetPixel(i + 1, j - 1).B + intenceBlue / 16 * 3;

                    //    if (Red > 255)
                    //    {
                    //        Red = 255;
                    //    }

                    //    if (Green > 255)
                    //    {
                    //        Green = 255;
                    //    }

                    //    if (Blue > 255)
                    //    {
                    //        Blue = 255;
                    //    }

                    //    if (Red < 0)
                    //    {
                    //        Red = 0;
                    //    }

                    //    if (Green < 0)
                    //    {
                    //        Green = 0;
                    //    }

                    //    if (Blue < 0)
                    //    {
                    //        Blue = 0;
                    //    }

                    //    c = Color.FromArgb(255, Red, Green, Blue);

                    //    imgMedianInt.SetPixel(i + 1, j - 1, c);
                    //}
                    #endregion

                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgMedianInt.Save("D:/file.jpg");

            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {

                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }

        private async void AsyncFloydSt()
        {
            await Task.Run(() => AlgorithmFloydSteynberg());
        }

        private void BTNFloydSteynberg(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            AsyncFloydSt();
        }

        private void FilterGauss(int sizeMask)
        {
            double[,] mask = new double[sizeMask, sizeMask];

            double firstPart = 0, powE = 0, count = 0, sigma = 3;

            double sumMask = 0;

            for (int i = 0; i < sizeMask; i++)
            {
                for (int j = 0; j < sizeMask; j++)
                {
                    double x = 0, y = 0;

                    count++;

                    int center = sizeMask / 2;

                    if (j < center)
                    {
                        x = center - j;
                    }

                    if (j > center)
                    {
                        x = j - center;
                    }

                    if (y < center)
                    {
                        y = center - i;
                    }

                    if (i > center)
                    {
                        y = i - center;
                    }
                    powE = ((Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * Math.Pow(sigma, 2))) * -0.5;

                    firstPart = 1 / Math.Sqrt(2 * Math.PI * Math.Pow(sigma, 2));

                    mask[i, j] = firstPart * Math.Pow(Math.E, powE);

                    sumMask += mask[i, j];

                }
            }

            for (int i = 0; i < sizeMask; i++)
            {
                for (int j = 0; j < sizeMask; j++)
                {
                    mask[i, j] = mask[i, j] / sumMask;
                }
            }

            Bitmap imgGauss = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    int Red = 0;
                    int Green = 0;
                    int Blue = 0;

                    int limit = sizeMask / 2;

                    for (int h = 0; h < sizeMask; h++)
                    {
                        for (int w = 0; w < sizeMask; w++)
                        {
                            if (i + h - limit < b.Height && j + w - limit < b.Width && i + h > limit && j + w > limit)
                            {
                                Red += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).R * mask[h, w]);

                                Green += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).G * mask[h, w]);

                                Blue += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).B * mask[h, w]);
                            }
                        }
                    }

                    Color c = Color.FromArgb(255, Red, Green, Blue);

                    imgGauss.SetPixel(i, j, c);

                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgGauss.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {
                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }

        private async void AsyncGauss(int sizeM)
        {
            await Task.Run(() => FilterGauss(sizeM));
        }

        private void Button_ClickGauss(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            int sizeM = int.Parse(CBXFilterGauss.Text);

            AsyncGauss(sizeM);
        }


        private void FilterSobelya()
        {

            int[,] maskX = new int[3, 3]{ {-1, 0, 1 },
                                          {-2, 0, 2 },
                                          {-1, 0, 1 }};

            int[,] maskY = new int[3, 3] { {-1,-2,-1 },
                                           { 0, 0, 0 },
                                           { 1, 2, 1 } };


            Bitmap imgSobel = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            for (int i = 0; i < b.Width - 1; i++)
            {
                for (int j = 0; j < b.Height - 1; j++)
                {
                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    int RedX = 0, RedY = 0;
                    int GreenX = 0, GreenY = 0;
                    int BlueX = 0, BlueY = 0;
                    int CenterX = 0, CenterY = 0;
                    int limit = 1;

                    for (int h = 0; h < 3; h++)
                    {
                        for (int w = 0; w < 3; w++)
                        {
                            if (i + h - limit < b.Width && j + w - limit < b.Height && i + h > limit && j + w > limit)
                            {
                                RedX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).R * maskX[h, w]);

                                GreenX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).G * maskX[h, w]);

                                BlueX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).B * maskX[h, w]);


                                RedY += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).R * maskY[h, w]);

                                GreenY += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).G * maskY[h, w]);

                                BlueY += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).B * maskY[h, w]);
                            }

                        }
                    }


                    CenterX = (RedX + BlueX + GreenX) / 3;

                    CenterY = (RedY + BlueY + GreenY) / 3;


                    int G = (int)Math.Sqrt(Math.Pow(CenterX, 2) + Math.Pow(CenterY, 2));

                    if (G > 128)
                    {
                        G = 255;
                    }

                    else
                    {
                        G = 0;
                    }

                    Color c = Color.FromArgb(255, G, G, G);

                    imgSobel.SetPixel(i, j, c);

                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgSobel.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {
                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }

        private async void AsyncFilterSobelya()
        {
            await Task.Run(() => FilterSobelya());
        }

        private void BTNSobelya(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            AsyncFilterSobelya();
        }



        private void FilterPruitta()
        {
            int[,] maskX = new int[3, 3]{ {-1, 0, 1 },
                                          {-1, 0, 1 },
                                          {-1, 0, 1 }};

            int[,] maskY = new int[3, 3] { {-1,-1,-1 },
                                           { 0, 0, 0 },
                                           { 1, 1, 1 } };


            Bitmap imgSobel = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            for (int i = 0; i < b.Width - 1; i++)
            {
                for (int j = 0; j < b.Height - 1; j++)
                {
                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    int RedX = 0, RedY = 0;
                    int GreenX = 0, GreenY = 0;
                    int BlueX = 0, BlueY = 0;
                    int CenterX = 0, CenterY = 0;
                    int limit = 1;

                    for (int h = 0; h < 3; h++)
                    {
                        for (int w = 0; w < 3; w++)
                        {
                            if (i + h - limit < b.Width && j + w - limit < b.Height && i + h > limit && j + w > limit)
                            {
                                RedX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).R * maskX[h, w]);

                                GreenX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).G * maskX[h, w]);

                                BlueX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).B * maskX[h, w]);


                                RedY += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).R * maskY[h, w]);

                                GreenY += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).G * maskY[h, w]);

                                BlueY += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).B * maskY[h, w]);
                            }

                        }
                    }


                    CenterX = (RedX + BlueX + GreenX) / 3;

                    CenterY = (RedY + BlueY + GreenY) / 3;


                    int G = (int)Math.Sqrt(Math.Pow(CenterX, 2) + Math.Pow(CenterY, 2));

                    if (G > 128)
                    {
                        G = 255;
                    }

                    else
                    {
                        G = 0;
                    }

                    Color c = Color.FromArgb(255, G, G, G);

                    imgSobel.SetPixel(i, j, c);

                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgSobel.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {
                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }
        
        private async void AsyncFilterPruitta()
        {
            await Task.Run(() => FilterPruitta());
        }
        
        private void BTNPruit(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            AsyncFilterPruitta();
        }



        private void FilterLaplasa()
        {
            int[,] mask = new int[3, 3]{ {1, 1, 1 },
                                         {1, -8, 1 },
                                         {1, 1, 1 }};
            
            Bitmap imgSobel = new Bitmap(b.Width, b.Height);

            int count0_1Per = (b.Width * b.Height) / 1000;

            int cou = 0;

            double xx = 0;

            for (int i = 0; i < b.Width - 1; i++)
            {
                for (int j = 0; j < b.Height - 1; j++)
                {
                    cou++;
                    if (cou == count0_1Per)
                    {
                        xx += 0.1;
                        Dispatcher.Invoke((ThreadStart)delegate
                        {
                            PGBAlgorithms.Value += 0.1;
                            if (xx >= 100)
                            {
                                xx = 100;
                            }
                            TBLProgressAlg.Text = Math.Round(xx, 1) + "%";
                        });

                        cou = 0;
                    }

                    int RedX = 0;
                    int GreenX = 0;
                    int BlueX = 0;
                    int CenterX = 0;
                    int limit = 1;

                    for (int h = 0; h < 3; h++)
                    {
                        for (int w = 0; w < 3; w++)
                        {
                            if (i + h - limit < b.Width && j + w - limit < b.Height && i + h > limit && j + w > limit)
                            {
                                RedX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).R * mask[h, w]);

                                GreenX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).G * mask[h, w]);

                                BlueX += Convert.ToInt32(b.GetPixel(i + h - limit, j + w - limit).B * mask[h, w]);
                            }
                        
                        }
                    }


                    CenterX = (RedX + BlueX + GreenX) / 3;
                    
                    int G = Math.Abs(CenterX);

                    if (G > 255)
                    {
                        G = 255;
                    }

                    Color c = Color.FromArgb(255, G, G, G);

                    imgSobel.SetPixel(i, j, c);

                }
            }

            if (File.Exists("D:/file.jpg"))
            {
                File.Delete("D:/file.jpg");
            }

            imgSobel.Save("D:/file.jpg");


            Dispatcher.Invoke((ThreadStart)delegate
            {
                PGBAlgorithms.Visibility = Visibility.Hidden;

                TBLProgressAlg.Visibility = Visibility.Hidden;

                PGBAlgorithms.Value = 0;

                TBLProgressAlg.Text = "0%";

                LBLSizeImage.Visibility = Visibility.Visible;

                LBLSizeImage.Content = "Результат роботи алгоритму";
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Source = null; });

            Dispatcher.Invoke((ThreadStart)delegate {
                MERes.Source = new Uri("D:/file.jpg");
            });

            Dispatcher.Invoke((ThreadStart)delegate { MERes.Visibility = Visibility.Visible; });
        }
        

        private async void AsyncFilterLaplasa()
        {
            await Task.Run(() => FilterLaplasa());
        }

        private void BTNLaplasa(object sender, RoutedEventArgs e)
        {
            LBLColorDepth.Visibility = Visibility.Hidden;
            LBLSizeImage.Visibility = Visibility.Hidden;
            PGBAlgorithms.Visibility = Visibility.Visible;
            TBLProgressAlg.Visibility = Visibility.Visible;

            AsyncFilterLaplasa();
        }

    }
}
