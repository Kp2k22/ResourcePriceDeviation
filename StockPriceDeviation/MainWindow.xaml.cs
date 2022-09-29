using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;

namespace StockPriceDeviation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        BackgroundWorker bw;
        HtmlWeb web = new HtmlWeb();
        int s;
        bool sflag, gflag;
        double? silverPrice;
        double? goldPrice;
        string CSVPath = System.IO.Path.Combine(Environment.CurrentDirectory, $"T2.csv");

        private void xs_Click(object sender, RoutedEventArgs e)
        {
            sflag = true;
            silver.IsEnabled = false;
        }
        private void xg_Click(object sender, RoutedEventArgs e)
        {
            gflag = true;
            gold.IsEnabled = false;
        }
        private double? SilverValue()
        {
            var doc = web.Load("https://spotprice.pl/");
            var nodes = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/section/div/div/div/div/div/div/div/div/div/div/section[3]/div/div/div[2]/div/div/div[2]/div/ul/li[3]/a/div/div[2]/span/bdi/text()").InnerText.Remove(0, 3).Replace('.', ',');
            var node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/section/div/div/div/div/div/div/div/div/div/div/section[5]/div/div/div/div/div/section/div/div/div[2]/div/div/div[1]/div/div/div/h6/span[1]/bdi/text()").InnerText.Replace('.', ',');
            return silverPrice = ((Convert.ToDouble(nodes) / Convert.ToDouble(node)) * 100) - 100;
        }
       private double? GoldValue()
        {
            var doc = web.Load("https://spotprice.pl/");
            var nodes = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/section/div/div/div/div/div/div/div/div/div/div/section[3]/div/div/div[1]/div/div/div[2]/div/ul/li[2]/a/div/div[2]/span/bdi/text()").InnerText.Remove(0, 3).Replace('.', ',').Remove(5);
            var node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/section/div/div/div/div/div/div/div/div/div/div/section[5]/div/div/div/div/div/section/div/div/div[1]/div/div/div[1]/div/div/div/h6/span[1]/bdi/text()").InnerText.Replace('.', ',').Remove(5);
            return goldPrice = ((Convert.ToDouble(nodes) / Convert.ToDouble(node)) * 100) - 100;
        }

        private void Scrap(int x)
        {
            for (; ; )
            {
                if (sflag) SilverValue();
                if (gflag) GoldValue();
                Serialization(CSVPath);
                Thread.Sleep(1000 * x);
            }
        }
        private async  void BackgroundW(int times)
        {
            Scrap(s);
        }
        private int Iteration()
        {
            if (r1s.IsChecked == true) return s = 6;
            if (r2s.IsChecked == true) return s = 12;
            if (r3s.IsChecked == true) return s = 24;
            return s = 1;
        }   
        private void Serialization(string path)
        {
            try
            {
                long? length = new FileInfo(path).Length;
                if (length == 0)
                    using (StreamWriter stream = File.AppendText(path))
                    {
                        stream.WriteLine("Silver;Gold;Date");
                    }
            }
            catch (FileNotFoundException) 
            {
                using (StreamWriter stream = File.AppendText(path))
                {
                    stream.WriteLine("Silver;Gold;Date");
                }
            }
            using (StreamWriter stream = File.AppendText(path))
            {
                stream.WriteLine($"{silverPrice}%;" + $"{goldPrice}%;" +  DateTime.Now.ToString());
            }
        }
        private void start_Click(object sender, RoutedEventArgs e)
        {
            Iteration();
            bw = new BackgroundWorker();
            bw.DoWork += (obj, ea) => BackgroundW(1);
            bw.RunWorkerAsync();
        }
    }
}
