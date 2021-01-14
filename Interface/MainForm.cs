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

namespace Interface
{
    public partial class MainForm : Form
    {
        //List of sting used in adjusting timers
        List<string> lines;
        List<string> newLines;
        //File name and parent path
        string fileName;
        string parentPath;
        //Number of updated properly and failed rows with time stamps
        int counter;
        int skipped;

        //Form load
        public MainForm()
        {
            InitializeComponent();
        }

        //On button click functions
        private void OpenFile_Click(object sender, EventArgs e)
        {
            LoadSubtitles();
        }

        private void AdjustSubs_Click(object sender, EventArgs e)
        {
            if (lines == null || lines?.Count == 0)
            {
                MessageBox.Show("Wybierz plik", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                AdjustSubtitles();
            }
        }

        private void saveButon_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Length == 0)
            {
                MessageBox.Show("Wpisz nazwę pliku", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                SaveToFile();
            }
        }


        //Function which loads .srt file
        private void LoadSubtitles()
        {
            lines = new List<string>();

            string filePath = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //file dialog settings
                openFileDialog.Filter = "srt files (*.srt)|*.srt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    //Get parent directory 
                    GetParentPath(filePath);
                    //Name of adjusted file 
                    textBox2.Text = fileName;
                    //Putting string lines into list
                    lines = File.ReadAllLines(filePath).ToList();
                    //Setting up textBox with loaded file
                    UpdateTextBox(lines);
                }
            }
        }

        private void AdjustSubtitles()
        {

            newLines = new List<string>();
            //counters setup
            counter = 0;
            skipped = 0;
            //Sign check if true sign is plus , false is minus
            bool sign = true;
            //Return if time to adjust isnt proper
            bool goB = false;
            //Adjust time
            int miliseconds;
            string milis = textBox3.Text;
            //output
            string outputLine = "";

            //Check sign
            if (milis.Length > 1)
            {
                if (milis[0] == '-')
                {
                    milis = milis.Substring(1, milis.Length - 1);
                    sign = false;
                }
            }

            //Parsing to int string from textBox3
            goB = int.TryParse(milis, out miliseconds);

            //If something is wrong drop Message box and return from method
            if (!goB || milis.Length == 0)
            {
                MessageBox.Show("Wpisz poprawny czas", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Looping through every line in file
            foreach (string line in lines.ToList())
            {
                outputLine = line;

                //Values to adjust (hours, minutes, seconds, miliseconds)
                int h1;
                int h2;
                int m1;
                int m2;
                int s1;
                int s2;
                int ms1;
                int ms2;

                //Strings to paste in adjusted line
                string Sh1;
                string Sh2;
                string Sm1;
                string Sm2;
                string Ss1;
                string Ss2;
                string Sms1;
                string Sms2;

                //For calculations
                int calc;

                //If line contain time stamps, looks like: 00:46:49,256 --> 00:46:50,915
                if (line.Contains(@"-->") && line.Length == 29)
                {
                    //Counter for time stamps
                    counter++;

                    //Parsing lines to int
                    int.TryParse(line.Substring(0, 2), out h1);
                    int.TryParse(line.Substring(3, 2), out m1);
                    int.TryParse(line.Substring(6, 2), out s1);
                    int.TryParse(line.Substring(9, 3), out ms1);
                    int.TryParse(line.Substring(17, 2), out h2);
                    int.TryParse(line.Substring(20, 2), out m2);
                    int.TryParse(line.Substring(23, 2), out s2);
                    int.TryParse(line.Substring(26, 3), out ms2);

                    //Calculations
                    if (sign)
                    {
                        ms1 += miliseconds;

                        if (ms1 >= 1000)
                        {
                            calc = ms1 / 1000;
                            ms1 = ms1 % 1000;
                            s1 += calc;
                        }
                        if (s1 >= 60)
                        {
                            calc = s1 / 60;
                            s1 = s1 % 60;
                            m1 += calc;
                        }
                        if (m1 >= 60)
                        {
                            calc = m1 / 60;
                            m1 = m1 % 60;
                            h1 += calc;
                        }


                        ms2 += miliseconds;

                        if (ms2 >= 1000)
                        {
                            calc = ms2 / 1000;
                            ms2 = ms2 % 1000;
                            s2 += calc;
                        }
                        if (s2 >= 60)
                        {
                            calc = s2 / 60;
                            s2 = s2 % 60;
                            m2 += calc;
                        }
                        if (m2 >= 60)
                        {
                            calc = m2 / 60;
                            m2 = m2 % 60;
                            h2 += calc;
                        }
                    }

                    else
                    {
                        ms1 -= miliseconds;

                        if (ms1 < 0)
                        {
                            calc = -ms1 / 1000;
                            ms1 = 1000 + (ms1 % 1000);
                            s1 -= 1 + calc;
                        }
                        if (ms1 == 1000)
                        {
                            ms1 = 0;
                            s1++;
                        }
                        if (s1 < 0)
                        {
                            calc = -s1 / 60;
                            s1 = 60 + (s1 % 60);
                            m1 -= 1 + calc;
                        }
                        if (s1 == 60)
                        {
                            s1 = 0;
                            m1++;
                        }
                        if (m1 < 0)
                        {
                            calc = -m1 / 60;
                            m1 = 60 + (m1 % 60);
                            h1 -= 1 + calc;
                        }
                        if (m1 == 60)
                        {
                            m1 = 0;
                            h1++;
                        }

                        ms2 -= miliseconds;

                        if (ms2 < 0)
                        {
                            calc = -ms2 / 1000;
                            ms2 = 1000 + (ms2 % 1000);
                            s2 -= 1 + calc;
                        }
                        if (ms2 == 1000)
                        {
                            ms2 = 0;
                            s2++;
                        }
                        if (s2 < 0)
                        {
                            calc = -s2 / 60;
                            s2 = 60 + (s2 % 60);
                            m2 -= 1 + calc;
                        }
                        if (s2 == 60)
                        {
                            s2 = 0;
                            m2++;
                        }
                        if (m2 < 0)
                        {
                            calc = -m2 / 60;
                            m2 = 60 + (m2 % 60);
                            h2 -= 1 + calc;
                        }
                        if (m2 == 60)
                        {
                            m2 = 0;
                            h2++;
                        }
                    }

                    //If any of time stamps have negative time skip iteration
                    if (h1 < 0 || h2 < 0 || m1 < 0 || m2 < 0 || s1 < 0 || s2 < 0 || ms1 < 0 || ms2 < 0)
                    {
                        skipped++;
                        newLines.Add(line);
                        continue;
                    }

                    //Adding zeros for proper .srt format
                    Sh1 = AddZero(1, h1);
                    Sh2 = AddZero(1, h2);
                    Sm1 = AddZero(1, m1);
                    Sm2 = AddZero(1, m2);
                    Ss1 = AddZero(1, s1);
                    Ss2 = AddZero(1, s2);
                    Sms1 = AddZero(2, ms1);
                    Sms2 = AddZero(2, ms2);

                    //Adjusted line
                    outputLine = $"{Sh1}:{Sm1}:{Ss1},{Sms1} --> {Sh2}:{Sm2}:{Ss2},{Sms2}";
                }
                //Adding line to list
                newLines.Add(outputLine);
            }
            //List reset
            lines = new List<string>();
            lines = newLines;
            UpdateTextBox(lines);
        }

        //Function for proper format
        private string AddZero(int zeros, int number)
        {
            string output = "";

            //For hours, minutes, seconds
            if (zeros == 1)
            {
                if (number < 10)
                {
                    output = $"0{number}";
                }
                else
                {
                    output = $"{number}";
                }
            }
            //For miliseconds
            else if (zeros == 2)
            {
                if (number < 100)
                {
                    output = $"00{number}";
                }
                else if (number < 10)
                {
                    output = $"0{number}";
                }
                else
                {
                    output = $"{number}";
                }
            }

            return output;
        }


        //Updating big text box with loaded file
        private void UpdateTextBox(List<string> lineList)
        {
            string text = "";
            foreach (string line in lineList)
            {
                text += line + "\r\n";
            }
            textBox1.Text = text;
        }

        //Extracting parent file path from file path of loaded file
        private void GetParentPath(string filePath)
        {
            string[] parts = filePath.Split('\\');

            int p = parts.Length;

            parentPath = "";

            for (int i = 0; i < p - 1; i++)
            {
                if (i == p - 2)
                {
                    parentPath += parts[i];
                }
                else
                {
                    parentPath += parts[i] + "\\";
                }
            }
            fileName = parts[parts.Length - 1];
        }

        //Saving subtitles to new file in the same directions
        private void SaveToFile()
        {
            string path = parentPath + "\\" + textBox4.Text + ".srt";
            string messge = $"{counter - skipped} rekordów zostało zaktualizowanych" + "\r\n" + $"{skipped} rekordów nie udało się zaktualizować";
            File.AppendAllText(path, textBox1.Text);
            MessageBox.Show(messge, "Yay", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
