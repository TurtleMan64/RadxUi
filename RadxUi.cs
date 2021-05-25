using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

public class RadxUi
{
    public const int SW_HIDE = 0;

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static Form theForm = new Form();
    
    public static TextBox txtBoxFilename  = new TextBox();
    public static TextBox txtBoxLoopStart = new TextBox();
    public static TextBox txtBoxLoopEnd   = new TextBox();
    
    public static CheckBox chkBoxLoopEnabled = new CheckBox();
    
    public static Label labelLoopEnabled = new Label();
    public static Label labelLoopStart   = new Label();
    public static Label labelLoopEnd     = new Label();
    
    public static Bitmap imgWaveformL = new Bitmap(584, 128);
    public static PictureBox picBoxWaveformL = new PictureBox();
    
    public static Bitmap imgWaveformR = new Bitmap(584, 128);
    public static PictureBox picBoxWaveformR = new PictureBox();
    
    public static int numChannels = 1;
    public static int bitsPerSample = 16;
    public static int numSamples = 0;
    public static int sampleRate = 41000;
    public static short[] samplesL = new short[0];
    public static short[] samplesR = new short[0];
    
    public static string TMP_DIR = ".tmp/";
    public static string TMP_WAV_FILE = "tmp123abcJYfPEaQEFI.wav";
    public static string EXE_DIR = "";
    
    [STAThread]
    public static void Main()
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);
        
        EXE_DIR = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/";
        
        Directory.CreateDirectory(EXE_DIR + TMP_DIR);
        
        theForm = new Form();
        theForm.Text = "RADX UI";
        theForm.Size = new Size(600+7, 436);
        theForm.MaximizeBox = false;
        theForm.FormBorderStyle = FormBorderStyle.FixedSingle;
        theForm.AllowDrop = true;
        theForm.DragEnter += new DragEventHandler(eventDragEnter);
        theForm.DragDrop  += new DragEventHandler(eventDragDrop);
        try
        {
            theForm.Icon = new Icon(MyIcon.getIconStream());
        }
        catch {}
        
        txtBoxFilename.ReadOnly = true;
        txtBoxFilename.Enabled  = false;
        txtBoxFilename.AutoSize = false;
        txtBoxFilename.Font = new Font(txtBoxFilename.Font.FontFamily, 10);
        txtBoxFilename.Left = 8;
        txtBoxFilename.Top  = 8;
        txtBoxFilename.Size = new Size(584, 22);
        txtBoxFilename.Text = "";
        
        txtBoxLoopStart.Left = 8;
        txtBoxLoopStart.Top  = 72;
        txtBoxLoopStart.Size = new Size(96, 28);
        txtBoxLoopStart.ReadOnly = true;
        txtBoxLoopStart.Enabled  = false;
        txtBoxLoopStart.Text = "";
        txtBoxLoopStart.TextChanged += eventLoopTextChanged;
        txtBoxLoopStart.SelectionStart = 0;
        
        txtBoxLoopEnd.Left = 496;
        txtBoxLoopEnd.Top  = 72;
        txtBoxLoopEnd.ReadOnly = true;
        txtBoxLoopEnd.Enabled  = false;
        txtBoxLoopEnd.Size = new Size(96, 28);
        txtBoxLoopEnd.Text = "";
        txtBoxLoopEnd.TextChanged += eventLoopTextChanged;
        
        chkBoxLoopEnabled.Left = 295;
        chkBoxLoopEnabled.Top  = 67;
        chkBoxLoopEnabled.CheckedChanged += eventLoopEnabledChanged;
        chkBoxLoopEnabled.Size = new Size(32, 32);
    
        labelLoopEnabled.Left = 266;
        labelLoopEnabled.Top  = 56;
        labelLoopEnabled.Size = new Size(200, 32);
        labelLoopEnabled.Text = "Loop Enabled:";
        
        labelLoopStart.Left = 8;
        labelLoopStart.Top  = 56;
        labelLoopStart.Size = new Size(400, 32);
        labelLoopStart.Text = "Loop Start Sample:";
        
        labelLoopEnd.Left = 496;
        labelLoopEnd.Top  = 56;
        labelLoopEnd.Size = new Size(400, 32);
        labelLoopEnd.Text = "Loop End Sample:";
        
        picBoxWaveformL.Left = 8;
        picBoxWaveformL.Top = 112;
        picBoxWaveformL.Size = new Size(584, 128);
        picBoxWaveformL.Image = imgWaveformL;
        
        picBoxWaveformR.Left = 8;
        picBoxWaveformR.Top = 248;
        picBoxWaveformR.Size = new Size(584, 128);
        picBoxWaveformR.Image = imgWaveformR;
        

        theForm.Controls.Add(txtBoxFilename);
        theForm.Controls.Add(txtBoxLoopStart);
        theForm.Controls.Add(txtBoxLoopEnd);
        theForm.Controls.Add(chkBoxLoopEnabled);
        theForm.Controls.Add(labelLoopEnabled);
        theForm.Controls.Add(labelLoopStart);
        theForm.Controls.Add(labelLoopEnd);
        theForm.Controls.Add(picBoxWaveformL);
        theForm.Controls.Add(picBoxWaveformR);
        
        
        // Create an empty MainMenu.
        MainMenu mainMenu = new MainMenu();

        // File submenu
        MenuItem[] subMenuFile = new MenuItem[2];
        subMenuFile[0] = new MenuItem("Open ADX");
        subMenuFile[1] = new MenuItem("Open WAV");
        
        subMenuFile[0].Click += eventOpenAdx;
        subMenuFile[1].Click += eventOpenWav;
       
        MenuItem menuItemFile = new MenuItem("&File", subMenuFile);
        
        
        // Export submenu
        MenuItem[] subMenuExport = new MenuItem[2];
        subMenuExport[0] = new MenuItem("Export as ADX");
        subMenuExport[1] = new MenuItem("Export as WAV");
        
        subMenuExport[0].Click += eventExportAdx;
        subMenuExport[1].Click += eventExportWav;
       
        MenuItem menuItemExport = new MenuItem("&Export", subMenuExport);
        
        
        // Help submenu
        MenuItem[] subMenuHelp = new MenuItem[1];
        subMenuHelp[0] = new MenuItem("About");
        
        subMenuHelp[0].Click += eventHelpAbout;
        
        MenuItem menuItemHelp = new MenuItem("&Help", subMenuHelp);
        
        // Add three MenuItem objects to the MainMenu.
        mainMenu.MenuItems.Add(menuItemFile);
        mainMenu.MenuItems.Add(menuItemExport);
        mainMenu.MenuItems.Add(menuItemHelp);
        
        theForm.Menu = mainMenu;
        
        theForm.ShowDialog();
        
        try 
        {
            File.Delete(EXE_DIR + TMP_DIR + TMP_WAV_FILE);
        }
        catch {}
    }
    
    public static void eventDragEnter(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.None;
        
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (filePaths.Length > 0)
            {
                string pathLower = filePaths[0].ToLower();
            
                if (pathLower.EndsWith(".wav") || pathLower.EndsWith(".adx"))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }
    }

    public static void eventDragDrop(object sender, DragEventArgs e)
    {
        string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);

        if (filePaths.Length > 0)
        {
            string pathLower = filePaths[0].ToLower();
            
            if (pathLower.EndsWith(".wav"))
            {
                loadWav(filePaths[0]);
            }
            else if (pathLower.EndsWith(".adx"))
            {
                loadAdx(filePaths[0]);
            }
        }
    }
    
    public static void eventOpenAdx(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.ShowHelp = true;
        dlg.Filter  = "ADX files|*.adx";
        dlg.Title = "Open ADX file";

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            loadAdx(dlg.FileName);
        }
    }
    
    public static void eventOpenWav(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.ShowHelp = true;
        dlg.Filter  = "WAV files|*.wav";
        dlg.Title = "Open WAV file";

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            loadWav(dlg.FileName);
        }
    }
    
    public static void eventExportAdx(object sender, EventArgs e)
    {
        SaveFileDialog  dlg = new SaveFileDialog();
        dlg.ShowHelp = true;
        dlg.Filter  = "ADX files|*.adx";
        dlg.Title = "Export ADX file";

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            string newPath = dlg.FileName;
            
            string args = " ";
            if (chkBoxLoopEnabled.Checked)
            {
                int loopSampleStart = 0;
                int loopSampleEnd = 1;
                
                if (Int32.TryParse(txtBoxLoopStart.Text, out loopSampleStart))
                {
                    if (Int32.TryParse(txtBoxLoopEnd.Text, out loopSampleEnd))
                    {
                        if (loopSampleStart < 0 || loopSampleStart > numSamples)
                        {
                            MessageBox.Show("Loop start sample out of range.");
                            return;
                        }
                        
                        if (loopSampleStart >= loopSampleEnd)
                        {
                            MessageBox.Show("Loop start sample should not be behind the end loop sample.");
                            return;
                        }
                        
                        if (loopSampleEnd < 0 || loopSampleEnd > numSamples)
                        {
                            MessageBox.Show("Loop end sample out of range.");
                            return;
                        }
                        
                        args += " --start " + loopSampleStart.ToString() + " ";
                        args += " --end "   + loopSampleEnd  .ToString() + " ";
                    }
                    else
                    {
                        MessageBox.Show("Could not parse loop end sample.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Could not parse loop start sample.");
                    return;
                }
            }
            else
            {
                args += " --no-loop ";
            }
            
            args += " \"" + EXE_DIR + TMP_DIR + TMP_WAV_FILE + "\" ";
            args += " \"" + newPath + "\"";
            
            Console.WriteLine(args);
            
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "radx_encode.exe";
            startInfo.Arguments = args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string rawOutput = process.StandardOutput.ReadToEnd();
            
            Console.WriteLine(rawOutput);
        }
    }
    
    public static void eventExportWav(object sender, EventArgs e)
    {
        SaveFileDialog  dlg = new SaveFileDialog();
        dlg.ShowHelp = true;
        dlg.Filter  = "WAV files|*.wav";
        dlg.Title = "Export WAV file";

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            string newPath = dlg.FileName;
            
            if (!newPath.Contains(TMP_WAV_FILE))
            {
                File.Copy(EXE_DIR + TMP_DIR + TMP_WAV_FILE, newPath, true);
            }
        }
    }
    
    public static void eventHelpAbout(object sender, EventArgs e)
    {
        MessageBox.Show("RADX UI version 1.0\nRADX version 0.3.1\n\nDrag and drop files onto UI to open them.");
    }
    
    public static void eventLoopTextChanged(object sender, EventArgs e)
    {
        drawWaveform(picBoxWaveformL, samplesL);
        drawWaveform(picBoxWaveformR, samplesR);
    }
    
    public static void eventLoopEnabledChanged(object sender, EventArgs e)
    {
        txtBoxLoopEnd.ReadOnly = !chkBoxLoopEnabled.Checked;
        txtBoxLoopEnd.Enabled  =  chkBoxLoopEnabled.Checked;
        
        txtBoxLoopStart.ReadOnly = !chkBoxLoopEnabled.Checked;
        txtBoxLoopStart.Enabled  =  chkBoxLoopEnabled.Checked;
        
        drawWaveform(picBoxWaveformL, samplesL);
        drawWaveform(picBoxWaveformR, samplesR);
    }
    
    public static void drawWaveform(PictureBox picBox, short[] samples)
    {
        Bitmap img = (Bitmap)picBox.Image;
        
        if (samples.Length == 0)
        {
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color newColor = Color.FromArgb(128, 128, 128);
                    img.SetPixel(x, y, newColor);
                }
            }
        }
        else
        {
            float samplesPerPixel = ((float)samples.Length)/img.Width;
            for (int x = 0; x < img.Width; x++)
            {
                int max = 0;
                int min = 0;
                float ampTop = 0.0f;
                float ampBot = 0.0f;
            
                for (int s = 0; s < (int)samplesPerPixel; s++)
                {
                    short sample = samples[x*(int)samplesPerPixel + s];

                    if (sample > max)
                    {
                        max = sample;
                    }
                    
                    if (sample < min)
                    {
                        min = sample;
                    }
                }
                
                if (bitsPerSample == 8)
                {
                    ampTop = max/( 128.0f);
                    ampBot = min/(-128.0f);
                }
                else if (bitsPerSample == 16)
                {
                    ampTop = max/( 32768.0f);
                    ampBot = min/(-32768.0f);
                }
                
                int ampPixTop = (int)((img.Height*ampTop)/2);
                int ampPixBot = (int)((img.Height*ampBot)/2);
                
                for (int y = 0; y < img.Height; y++)
                {
                    Color newColor = Color.FromArgb(128, 128, 128);
                    img.SetPixel(x, y, newColor);
                }
                
                for (int y = img.Height/2; y >= (img.Height/2) - ampPixTop; y--)
                {
                    Color newColor = Color.FromArgb(0, 0, 200);
                    img.SetPixel(x, y, newColor);
                }
                
                for (int y = img.Height/2; y < (img.Height/2) + ampPixBot; y++)
                {
                    Color newColor = Color.FromArgb(0, 0, 200);
                    img.SetPixel(x, y, newColor);
                }
            }
            
            if (chkBoxLoopEnabled.Checked)
            {
                try
                {
                    int sample = Int32.Parse(txtBoxLoopStart.Text);
                    if (sample >= 0 && sample <= numSamples)
                    {
                        int xPix = (int)(sample/samplesPerPixel);
                        Color newColor = Color.FromArgb(200, 0, 0);
                        for (int y = 0; y < img.Height; y++)
                        {
                            img.SetPixel(xPix, y, newColor);
                        }
                    }
                }
                catch {}
    
                try
                {
                    int sample = Int32.Parse(txtBoxLoopEnd.Text);
                    if (sample >= 0 && sample <= numSamples)
                    {
                        int xPix = ((int)(sample/samplesPerPixel)) - 1;
                        Color newColor = Color.FromArgb(200, 0, 0);
                        for (int y = 0; y < img.Height; y++)
                        {
                            img.SetPixel(xPix, y, newColor);
                        }
                    }
                }
                catch {}
            }
        }
        
        picBox.Refresh();
    }
    
    public static bool loadWav(string path)
    {
        try
        {
            byte[] f = File.ReadAllBytes(path);

            if (f[0] != 'R' ||
                f[1] != 'I' ||
                f[2] != 'F' ||
                f[3] != 'F')
            {
                throw new Exception("RIFF not found");
            }

            int size1 = BitConverter.ToInt32(f, 4);

            if (f[ 8] != 'W' ||
                f[ 9] != 'A' ||
                f[10] != 'V' ||
                f[11] != 'E')
            {
                throw new Exception("WAVE not found");
            }

            if (f[12] != 'f' ||
                f[13] != 'm' ||
                f[14] != 't' ||
                f[15] != ' ')
            {
                throw new Exception("fmt not found");
            }

            int   chunkSize      = BitConverter.ToInt32(f, 16);
            short formatType     = BitConverter.ToInt16(f, 20);
            short channels       = BitConverter.ToInt16(f, 22);
            int   sr             = BitConverter.ToInt32(f, 24);
            int   avgBytesPerSec = BitConverter.ToInt32(f, 28);
            short bytesPS        = BitConverter.ToInt16(f, 32);
            short bitsPS         = BitConverter.ToInt16(f, 34);
            
            if (formatType != 1 || (bitsPS != 8 && bitsPS != 16))
            {
                MessageBox.Show("Cannot load data from WAV file. Change format of the file to 16 or 8 bit and try again.");
                return false;
            }

            if (f[36] != 'd' ||
                f[37] != 'a' ||
                f[38] != 't' ||
                f[39] != 'a')
            {
                throw new Exception("data not found");
            }

            int dataSizeBytes = BitConverter.ToInt32(f, 40);
            
            byte[] data = new byte[dataSizeBytes];
            Array.Copy(f, 44, data, 0, dataSizeBytes);
            
            if (!path.Contains(TMP_WAV_FILE))
            {
                File.Copy(path, EXE_DIR + TMP_DIR + TMP_WAV_FILE, true);
            }
            
            //We have all the data, now update UI and others
            numChannels   = (int)channels;
            bitsPerSample = (int)bitsPS;
            sampleRate    = sr;
            txtBoxFilename.Text = path;
            
            int bytesPerSample = bitsPerSample/8;
            numSamples = (dataSizeBytes/bytesPerSample)/channels;
            
            if (numChannels == 1)
            {
                samplesL = new short[dataSizeBytes/2];
                samplesR = new short[0];
                
                for (int i = 0; i < numSamples; i++)
                {
                    samplesL[i] = BitConverter.ToInt16(data, i*bytesPerSample);
                }
            }
            else if (numChannels == 2)
            {
                samplesL = new short[dataSizeBytes/4];
                samplesR = new short[dataSizeBytes/4];
                
                for (int i = 0; i < numSamples; i++)
                {
                    samplesL[i] = BitConverter.ToInt16(data, i*2*bytesPerSample);
                    samplesR[i] = BitConverter.ToInt16(data, i*2*bytesPerSample + 2);
                }
            }
            else
            {
                throw new Exception("numChannels " + numChannels.ToString() + " not supported");
            }
            
            txtBoxLoopStart.Text = "0";
            txtBoxLoopEnd  .Text = numSamples.ToString();
            
            chkBoxLoopEnabled.Checked = true;
            
            drawWaveform(picBoxWaveformL, samplesL);
            drawWaveform(picBoxWaveformR, samplesR);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error when loading WAV file: {0}", e.ToString());
            MessageBox.Show("Error when loading WAV file.");
            
            return false;
        }
        
        return true;
    }

    public static void loadAdx(string path)
    {
        txtBoxFilename.Text = path;
        
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "radx_decode.exe";
        startInfo.Arguments = " \"" + path + "\" \"" + EXE_DIR + TMP_DIR + TMP_WAV_FILE + "\"";
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string rawOutput = process.StandardOutput.ReadToEnd();
        
        string[] output = rawOutput.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        
        if (output.Length <= 4)
        {
            MessageBox.Show("Error when loading ADX file.");
            return;
        }
        
        if (loadWav(EXE_DIR + TMP_DIR + TMP_WAV_FILE))
        {
            try
            {
                txtBoxFilename.Text = path;
                
                if (!output[3].Contains("Non-looping ADX"))
                {
                    txtBoxLoopStart.Text = output[3].Substring(output[3].IndexOf("Loop start sample:") + 19);
                    txtBoxLoopEnd  .Text = output[4].Substring(output[4].IndexOf("Loop end sample:"  ) + 17);

                    chkBoxLoopEnabled.Checked = true;
                }
                else
                {
                    txtBoxLoopStart.Text = "0";
                    txtBoxLoopEnd  .Text = numSamples.ToString();

                    chkBoxLoopEnabled.Checked = false;
                }
                
                drawWaveform(picBoxWaveformL, samplesL);
                drawWaveform(picBoxWaveformR, samplesR);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when loading ADX file: {0}", e.ToString());
                MessageBox.Show("Error when loading ADX file.");
            }
        }
    }
}
