using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_comp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void filesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|RTF files (*.rtf)|*.rtf|Image files|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|Hyper text files|*.htm;*.html";
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = openFileDialog1.FileNames.Length.ToString() + " file(s)";
                string[] files = openFileDialog1.FileNames;
                toolStripComboBox1.Items.Clear();
                toolStripComboBox1.Items.AddRange(files);
                //comboBox1.DataSource = files;
                checkedListBox1.Items.Clear();
                checkedListBox1.Items.AddRange(files);
            }
        }

        private void foldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = Directory.EnumerateFiles(folderBrowserDialog1.SelectedPath).Count().ToString() + " file(s)";
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                toolStripComboBox1.Items.Clear();
                toolStripComboBox1.Items.AddRange(files);
                //comboBox1.DataSource = files;
                checkedListBox1.Items.Clear();
                checkedListBox1.Items.AddRange(files);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Zip files (*.zip)|*.zip";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                try 
                {                    
                    //ZipFile.CreateFromDirectory(saveFileDialog1.InitialDirectory, zip_file.Name);
                    ZipArchive zipArchive=ZipFile.Open(saveFileDialog1.FileName, ZipArchiveMode.Create);
                    int amt = checkedListBox1.Items.Count, c_amt = checkedListBox1.CheckedItems.Count;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    toolStripProgressBar1.Maximum = c_amt;
                    foreach (string file in checkedListBox1.CheckedItems) 
                    {
                        ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(Path.GetFileName(file), CompressionLevel.Optimal);
                        byte[] f_b = File.ReadAllBytes(file);
                        Stream stream = zipArchiveEntry.Open();
                        stream.Write(f_b, 0, f_b.Length);
                        stream.Close();
                        toolStripProgressBar1.Value += 1;
                    }
                    stopwatch.Stop();
                    zipArchive.Dispose();
                    TimeSpan timeSpan = stopwatch.Elapsed;
                    toolStripStatusLabel1.Text = c_amt + " of " + amt + " was packed in " + timeSpan.Milliseconds;
                }
                catch(Exception g) { MessageBox.Show(g.Message); }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = Application.ProductName;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.KeyDown += (sen, h) => 
            {
                if (h.Modifiers == Keys.Control && h.KeyCode == Keys.O) 
                {
                    openFileDialog1.Filter = "Zip files (*.zip)|*.zip";
                    if (openFileDialog1.ShowDialog() == DialogResult.OK) 
                    {
                        try 
                        {
                            ZipArchive zipArchive = ZipFile.Open(openFileDialog1.FileName, ZipArchiveMode.Update);
                            int count =zipArchive.Entries.Count;
                            if (count == 0)
                            {
                                MessageBox.Show(Path.GetFileName(openFileDialog1.FileName) + " was not created with this program.");
                            }
                            else 
                            {
                                MessageBox.Show("the amount of entries is: " + count);                                
                                DataTable dt = new DataTable();
                                dt.Columns.Add("file");
                                foreach(ZipArchiveEntry entry in zipArchive.Entries) 
                                {
                                    dt.Rows.Add(entry.Name);
                                }
                                Form zip_data = new Form();
                                zip_data.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                zip_data.Text = "archive viewer";
                                Label lb = new Label();
                                lb.Text = "ctrl+c to copy";
                                lb.Parent = zip_data;
                                lb.Top = 150;
                                DataGridView zip_g = new DataGridView();
                                zip_g.DataSource = dt;
                                zip_g.Parent = zip_data;
                                Button ext = new Button();
                                ext.Text = "Extract";
                                ext.Location = new Point(0, 173);
                                ext.Click += (send, j) => 
                                {
                                    try 
                                    {
                                        string dir = @"c:\File_comp";
                                        if (Directory.Exists(dir) == false)
                                        {
                                            Directory.CreateDirectory(dir);
                                        }
                                        else { }
                                        string file = Interaction.InputBox("please enter the filename or 'all'.", "file extract", "put the filename here!");
                                        if (file == "all") 
                                        {
                                            zipArchive.ExtractToDirectory(dir);
                                            zipArchive.Dispose();
                                            //ZipFile.ExtractToDirectory(openFileDialog1.FileName, dir);
                                            MessageBox.Show("all files were extracted from " + Path.GetFileName(openFileDialog1.FileName) + " to " + dir);
                                        }
                                        else 
                                        {
                                            ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                            zipArchiveEntry.ExtractToFile(Path.Combine(dir, file), true);
                                            zipArchive.Dispose();
                                            MessageBox.Show(file + " was extracted from " + Path.GetFileName(openFileDialog1.FileName) + " to " + dir);
                                        }
                                    }
                                    catch(Exception k) { MessageBox.Show(k.Message); }
                                };
                                Button del = new Button();
                                del.Text = "Delete";
                                del.Location = new Point(77, 173);
                                del.Click += (send, j) => 
                                {
                                    try
                                    {
                                        string file = Interaction.InputBox("please enter the filename.", "file delete", "put the filename here!");
                                        ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                        zipArchiveEntry.Delete();
                                        zipArchive.Dispose();
                                    }
                                    catch (Exception k) { MessageBox.Show(k.Message); }
                                };
                                Button[] btns = { ext, del };
                                for (int i = 0; i < btns.Length; i++)
                                {
                                    btns[i].Size = new Size(75, 23);
                                }
                                zip_data.Controls.AddRange(btns);
                                zip_data.Show();
                            }
                        }
                        catch(Exception g) { MessageBox.Show(g.Message); }
                    }
                }
            };
            toolStripComboBox1.SelectedIndexChanged += (sen, h) =>
            {
                try
                {
                    FileInfo fi = new FileInfo(toolStripComboBox1.Text);
                    if (fi.Extension == ".txt")
                    {
                        string cont = File.ReadAllText(toolStripComboBox1.Text);
                        richTextBox1.Text = cont;
                    }
                    else if (fi.Extension == ".rtf")
                    {
                        richTextBox1.LoadFile(toolStripComboBox1.Text);
                    }
                    else if (fi.Extension == ".jpg" || fi.Extension == ".jpeg" || fi.Extension == ".png" || fi.Extension == ".bmp" || fi.Extension == ".tiff")
                    {
                        byte[] image = File.ReadAllBytes(toolStripComboBox1.Text);
                        MemoryStream stream = new MemoryStream(image);
                        pictureBox1.Image = Image.FromStream(stream);
                    }
                    else if (fi.Extension == ".htm" || fi.Extension == ".html")
                    {
                        webBrowser1.Url = new Uri(toolStripComboBox1.Text);
                    }
                    else { }
                    toolStripComboBox1.ToolTipText = Path.GetFileName(toolStripComboBox1.Text);
                }
                catch (Exception g) { MessageBox.Show(g.Message); }
            };
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string file = checkedListBox1.SelectedItem.ToString();
            toolTip1.SetToolTip(checkedListBox1, Path.GetFileName(file));
        }
    }
}
