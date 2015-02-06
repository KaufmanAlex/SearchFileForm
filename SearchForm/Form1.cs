using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Timer = System.Threading.Timer;

namespace SearchForm
{
    public partial class Form1 : Form
    {
        public int ex_count; // количество исключений

        Timer tmr1;
        System.DateTime date1;

        // Конструктор
        public Form1()
        {
            InitializeComponent();
            
            // Загрузка введенных данных
            System.IO.StreamReader textFile = new System.IO.StreamReader("temp666.txt", Encoding.UTF8);

            tbFile.Text = textFile.ReadLine();
            tbWord.Text = textFile.ReadLine();
            tbStartDir.Text = textFile.ReadLine();
            textFile.Close();                                
        }

        // Функция поиска
        private void Search(string way, string mask)
        {
            string[] Dirs = null;
            string[] files = null;
            string cdir = null;
            try
            {
                files = Directory.GetFiles(way, mask);

                foreach (var f in files)
                {
                    if (checkBox1.Checked == false)
                    {
                        lbResults.Items.Add(f);
                    }
                    else
                    {
                        var word = File.ReadAllLines(f);
                        var LogHashSet = new HashSet<string>(word);
                        if (LogHashSet.Contains(tbWord.Text) == true)
                            lbResults.Items.Add(f);
                        else { }
                    }
                }
                Dirs = Directory.GetDirectories(way, "*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
                ex_count++;
            }
            if ((Dirs != null))
            {
                foreach (string cdir_loopVariable in Dirs)
                {
                    cdir = cdir_loopVariable;
                    Search(cdir, mask);
                }
            }
        }

        // Кнопка "Обзор"
        private void button1_Click(object sender, EventArgs e)
        {
            if (BrausDlg.ShowDialog() == DialogResult.OK)
            {
                tbStartDir.Text = BrausDlg.SelectedPath;
            }
        }

        // Кнопка "Поиск"
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button3_Click(sender, e);

                //   string [] mas = new string [2];
                //   string [] mas = {Convert.ToString(sender), Convert.ToString(e)};

                timer1.Start();
                tmr1 = new Timer(Tick1, "tick1...", 0, 1000);

                Search(tbStartDir.Text, tbFile.Text);

                string infostr = "Поиск завершен. Найдено " + lbResults.Items.Count + " файлов." + ". Обработано " + ex_count + " исключений.";
                labKolRez.Text = Convert.ToString(lbResults.Items.Count);
                MessageBox.Show(infostr);
            }
            catch 
            {
                MessageBox.Show("Возникла ошибка", "Error");
            }
        }

        private void Tick1(object data)   // выполнение пула потока
        {
            date1 = date1.AddSeconds(1);
        }

        // Кнопка "Очистить все"
        private void button3_Click(object sender, EventArgs e)
        {
            lbResults.Items.Clear();
            treeView1.Nodes.Clear();

            ex_count = 0;
            date1 = new DateTime(2015, 1, 27, 0, 0, 0);
            labTime.Text = "00:00";
            labKolRez.Text = "0";
        }

        // Двойной клик мыши по результатам для открытия
        private void Results_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                Process.Start(lbResults.SelectedItem.ToString());
            }
            catch { }
        }

        // Одиночный клик мыши по результатам для отображения дерева
        private void lbResults_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                treeView1.Nodes.Clear();
                string path = lbResults.SelectedItem.ToString();
                string[] y = path.Split('\\');

                TreeNode[] mas = new TreeNode[y.Count()];

                mas[0] = treeView1.Nodes.Add(y[0]);           
                for (int i = 1; i < y.Count(); i++)
                {
                    mas[i] = mas[i - 1].Nodes.Add(y[i]);
                }
                mas[0].ExpandAll();
            }
            catch { }
        }

        // Закрытие формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Сохранение введенных данных
            System.IO.StreamWriter textFile = new System.IO.StreamWriter("temp666.txt");
            textFile.WriteLine(tbFile.Text);
            textFile.WriteLine(tbWord.Text);
            textFile.WriteLine(tbStartDir.Text);
            textFile.Close();                       
        }
//-----------------------------------------------------------------------------------------
        // Чекбокс (интерфейс)
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) tbWord.Enabled = true;
            else tbWord.Enabled = false;
        }

        // Таймер
        private void timer1_Tick(object sender, EventArgs e)
        {
            labTime.Text = date1.ToString("mm:ss");

            timer1.Stop();
            tmr1.Dispose();
        }
    }
}
