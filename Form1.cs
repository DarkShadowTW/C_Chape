using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;                //for save file
using System.Data;              //for debug
using System.Diagnostics;       //for debug

using NAudio.Wave;              //for recording music
using NAudio.CoreAudioApi;      //for recording music

namespace NicknameReConnectIDriver
{
    public partial class frmRecording : Form
    {
        static private NAudio.Wave.WaveFileWriter waveWriter = null;
        static public int sampleLocation = 0;
        static string outputFolder = Path.Combine("c:\\", "temp");
        static string outputFilename;
        static string outputFilePath;
        static int deviceNumber = -1;
        static int sampleRate = 44100;
        static int inChannels = NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels;

        static NAudio.Wave.WaveIn sourceStream = null;
        static NAudio.Wave.DirectSoundOut waveOut = null;
        //NAudio.Wave.WaveFileWriter waveWriter = null;
        //NAudio.Wave.WaveFileReader waveReader = null;
        //NAudio.Wave.DirectSoundOut output = null;

        public frmRecording()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            outputFilename = String.Format("Clip {0:yyy-MM-dd HH-mm-ss}.wav", DateTime.Now);
            outputFilePath = Path.Combine(outputFolder, outputFilename);
            Debug.Print(outputFilePath);
            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(sampleRate, inChannels);
            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
            waveWriter = new NAudio.Wave.WaveFileWriter(outputFilePath, sourceStream.WaveFormat);

            sourceStream.StartRecording();
        }

        static void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (waveWriter == null) return;

            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            sampleLocation += e.BytesRecorded / 4;
            Debug.Print(((double)sampleLocation / (double)sampleRate).ToString());
            waveWriter.Flush();
        }

        private void frmRecording_Load(object sender, EventArgs e)
        {

        }

        private void frmRecording_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }
    }
}
