// ------------------------------------------------------------------------------------------------------
// Sounds.CS
//
// Sounds validated by this wave file generator, after being generated with the FileStream code
// https://indiehd.com/auxiliary/flac-validator/progress/?X-Progress-ID=606ebbf2c76df
// 
// Sound class approach to forming the WAVE FILE stream leveraged from this article:
// https://www.codeguru.com/columns/dotnet/making-sounds-with-waves-using-c.html
//
// Finally, the square wave function approach to building the sound pattern in the buffer was obtained from this 
// Stack Overflow question's answer:
// https://stackoverflow.com/questions/6168954/playing-sound-byte-in-c-sharp
// 
// }
// ------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Threading;
//using Microsoft.DirectX.DirectSound;

namespace ASTEROIDS
{
    public class Sounds
    {
        private frmAsteroids canvas;
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, IntPtr alwaysNull, int bufferSize, IntPtr hwndCallback);

        
        public enum SOUNDS
        {
            PURETONE = 0,
            BGMUSICLOW = 1,
            BGMUSICHIGH = 2,
            THRUST = 3,
            PROJECTILE = 4,
            DESTROYEDASTEROIDSMALL = 5,
            DESTROYEDASTEROIDMEDIUM = 6,
            DESTROYEDASTEROIDLARGE = 7,
            SPACESHIPLARGE = 8,
            SPACESHIPSMALL = 9
        }
        public Dictionary<SOUNDS, FX> library = new Dictionary<SOUNDS, FX>();

        public Sounds( frmAsteroids rootForm )
        {
            canvas = rootForm;
            //FileStream fs = new FileStream("SHIT2.WAV", FileMode.OpenOrCreate);
            //fs.Write(generateAudio(SOUNDS.PURETONE), 0, generateAudio(SOUNDS.PURETONE).Length);
            //fs.Close();

            library.Add(SOUNDS.PURETONE, generateAudio(SOUNDS.PURETONE));
            library.Add(SOUNDS.BGMUSICLOW, generateAudio(SOUNDS.BGMUSICLOW));
            library.Add(SOUNDS.BGMUSICHIGH, generateAudio(SOUNDS.BGMUSICHIGH));
            library.Add(SOUNDS.THRUST, generateAudio(SOUNDS.THRUST));
            library.Add(SOUNDS.PROJECTILE, generateAudio(SOUNDS.PROJECTILE));
            library.Add(SOUNDS.DESTROYEDASTEROIDSMALL, generateAudio(SOUNDS.DESTROYEDASTEROIDSMALL));
            library.Add(SOUNDS.DESTROYEDASTEROIDMEDIUM, generateAudio(SOUNDS.DESTROYEDASTEROIDMEDIUM));
            library.Add(SOUNDS.DESTROYEDASTEROIDLARGE, generateAudio(SOUNDS.DESTROYEDASTEROIDLARGE));
            library.Add(SOUNDS.SPACESHIPLARGE, generateAudio(SOUNDS.SPACESHIPLARGE));
            library.Add(SOUNDS.SPACESHIPSMALL, generateAudio(SOUNDS.SPACESHIPSMALL));
        }

        ~Sounds()
        {
            mciSendString("close all", IntPtr.Zero, 0, IntPtr.Zero);
        }

        private FX generateAudio(SOUNDS curSnd)
        {
            string sFileName = "";
            byte[] buff = null;
            List<short> ssData = new List<short>();

            switch (curSnd)
            {
                case SOUNDS.PURETONE:
                    ssData.AddRange(ShapeSoundSquare(255, 1000, 20000));
                    sFileName = "PURETONE.WAV";
                    break;
                case SOUNDS.BGMUSICLOW:
                    ssData.AddRange(ShapeSoundSquare(81, 100, 2500));
                    sFileName = "BGMUSICLOW.WAV";
                    break;
                case SOUNDS.BGMUSICHIGH:
                    ssData.AddRange(ShapeSoundSquare(87, 100, 2500));
                    sFileName = "BGMUSICHIGH.WAV";
                    break;
                case SOUNDS.THRUST:
                    ssData.AddRange(ShapeSoundThrust());
                    sFileName = "THRUST.WAV";
                    break;
                case SOUNDS.PROJECTILE:
                    buff = new byte[AsteroidsResources.Projectile.Length];
                    AsteroidsResources.Projectile.Read(buff, 0, (int) AsteroidsResources.Projectile.Length );
                    sFileName = "Projectile.wav";
                    break;
                case SOUNDS.DESTROYEDASTEROIDSMALL:
                    buff = new byte[AsteroidsResources.DestroyedAsteroidSmall.Length];
                    AsteroidsResources.DestroyedAsteroidSmall.Read(buff, 0, (int)AsteroidsResources.DestroyedAsteroidSmall.Length);
                    sFileName = "DestroyedAsteroidSmall.wav";
                    break;
                case SOUNDS.DESTROYEDASTEROIDMEDIUM:
                    buff = new byte[AsteroidsResources.DestroyedAsteroidMedium.Length];
                    AsteroidsResources.DestroyedAsteroidMedium.Read(buff, 0, (int)AsteroidsResources.DestroyedAsteroidMedium.Length);
                    sFileName = "DestroyedAsteroidMedium.wav";
                    break;
                case SOUNDS.DESTROYEDASTEROIDLARGE:
                    buff = new byte[AsteroidsResources.DestroyedAsteroidLarge.Length];
                    AsteroidsResources.DestroyedAsteroidLarge.Read(buff, 0, (int)AsteroidsResources.DestroyedAsteroidLarge.Length);
                    sFileName = "DestroyedAsteroidlarge.wav";
                    break;
                case SOUNDS.SPACESHIPLARGE:
                    buff = new byte[AsteroidsResources.SpaceShipLarge.Length];
                    AsteroidsResources.SpaceShipLarge.Read(buff, 0, (int)AsteroidsResources.SpaceShipLarge.Length);
                    sFileName = "SpaceShipLarge.wav";
                    break;
                case SOUNDS.SPACESHIPSMALL:
                    buff = new byte[AsteroidsResources.SpaceShipSmall.Length];
                    AsteroidsResources.SpaceShipSmall.Read(buff, 0, (int)AsteroidsResources.SpaceShipSmall.Length);
                    sFileName = "SpaceShipSmall.wav";
                    break;
            }

            List<byte> data = new List<byte>();
            WaveHeader header = new WaveHeader();
            FormatChunk format = new FormatChunk();
            DataChunk dc = new DataChunk();
            List<byte> tempBytes = new List<byte>();

            dc.AddSampleDataStereo(ssData.ToArray(), ssData.ToArray());
            header.FileLength += format.Length() + dc.Length();
            tempBytes.AddRange(header.GetBytes());
            tempBytes.AddRange(format.GetBytes());
            tempBytes.AddRange(dc.GetBytes());

            string sFullPath = createWaveOutput(curSnd, sFileName, (buff==null)? tempBytes.ToArray(): buff);

            return new FX(canvas.Handle, curSnd, sFullPath); //  SoundPlayer(ms);// SecondaryBuffer(ms, device);
        }

        private string createWaveOutput(Sounds.SOUNDS curSnd, string sFileName, byte[] data)
        {
            string sFULL = String.Format("{0}{1}", Path.GetTempPath(), sFileName);
            if (!File.Exists(sFULL)) // only attempt to create the file if it's not already there.
                File.WriteAllBytes(sFULL, data);

            return sFULL;

        }

        public List<short> ShapeSoundSquare(UInt16 frequency, int msDuration, UInt16 volume)
        {
            List<short> data = new List<short>();

            const double TAU = 2 * Math.PI;
            int samplesPerSecond = 44100;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);

            double theta = frequency * TAU / (double)samplesPerSecond;
            // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
            // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
            double amp = volume >> 2; // so we simply set amp = volume / 2
            for (int step = 0; step < samples; step++)
            {
                //short s = (short)(amp * Math.Sin(theta * (double)step));
                short s = (short)(amp * Math.Sin(theta * (double)step));
                data.Add(Convert.ToInt16(amp * Math.Sign(Math.Sin(theta * step))));
            }

            return data;
        }

        public List<short> ShapeSoundThrust()
        {
            UInt16 frequency = 92; //  53; //  415 / 2 / 2 / 2 /2 /2 /2; // 53;
            int msDuration = 500; //  287; // 60 seconds, a huge buffer for the sound, but it will sound continuous... 
            List<short> data = new List<short>();

            const double TAU = 2 * Math.PI;
            int samplesPerSecond = 44100;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);

            double theta = frequency * TAU / (double)samplesPerSecond;
            double amp = 2500;

            Random r = new Random();
            for (int step = 0; step < samples; step++)
            {
                // "Close enough" approximation to the original noise without killing myself 
                if (step % 150 == 0)
                    amp = r.Next(200, 1000);

                short s = (short)(amp * Math.Sin(theta * (double)step));
                data.Add(s);
            }

            return data;
        }

        // TODO: Can't get this one working.... 
        // need to understand sound better... ARGHHHH
        public List<short> ShapeSoundProjectile()
        {
            int msDuration = 258; //  287; // 60 seconds, a huge buffer for the sound, but it will sound continuous... 
            List<short> data = new List<short>();

            const double TAU = 2 * Math.PI;
            int samplesPerSecond = 44100;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);

            double amp = 3000;
            Random r = new Random();

            double mult = 1.0;
            double fShift = -0.6;

            int stopBumpUpFreq = samples / 4;
            int curSign = 1;
            int nToggleStep = 0;
            int currentStep = 0;
            bool bOnDecline = false;
            int noteshift = 0;
            int curModulusOperator = 19;
            float shiftLowEnd = 1.0f;
            for (int step = 0; step < samples; step++)
            {
                string s;
                // "Close enough" approximation to the original noise without killing myself 
                if (step % 400   == 0)
                {
                    mult -= 0.015;
                    //amp = mult * r.Next(250, 1000);
                }
                if (stopBumpUpFreq > step && ((step % 40) == 0) && fShift <= 1.0)
                    fShift += 0.015;
                else if (stopBumpUpFreq > step && ((step % 40) == 0) && fShift <= 1.7)
                    fShift += 0.005;
                else if (((step % 10) == 0) && (fShift > 0.0))
                {
                    bOnDecline = true;
                    fShift -= 0.0005;
                }
                else if ( (fShift <= 0.0) && (bOnDecline == true)) 
                {
                    fShift = 0.0;
                    s = "here";
                    //if ( (step % 25) == 0 )
                    //    curModulusOperator += 1;

                }

                nToggleStep = (( step+1 ) % curModulusOperator);
                if (nToggleStep == 0)
                {
                    currentStep += 1;
                    if (currentStep == 3)
                    {
                        curSign = 1;
                        currentStep = 0;
                    }
                    else
                        curSign = -1;
                }

                short curNote = Convert.ToInt16( (mult * amp * curSign));
                if (curSign < 0 && bOnDecline)
                {
                    //shiftLowEnd += 0.001f;
                    curNote = (short) ((double) -200.0 * (double) shiftLowEnd); //  Convert.ToInt16((shiftLowEnd)*(mult * amp * curSign) + (fShift * 10000));
                }

                data.Add(curNote);

            }

            return data;
        }
    }

    public class FX
    {
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, IntPtr buffPtr, int bufferSize, IntPtr hwndCallback);

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr hwndCallback);

        private bool m_bIsStopped = false;
        private Sounds.SOUNDS m_SoundType;

        private Dictionary<Sounds.SOUNDS, string> m_soundDictionary = new Dictionary<Sounds.SOUNDS, string>();
        private string m_sFileName;

        private string m_curAlias = "";
        private IntPtr hwndCanvas;

        private long medialengthInTicks;
        private long canFireNext;
        private string m_sFullOpenText;


        [System.Flags]
        public enum PlaySoundFlags : uint
        {
            SND_SYNC = 0x0000,
            SND_ASYNC = 0x0001,
            SND_NODEFAULT = 0x0002,
            SND_MEMORY = 0x0004,
            SND_LOOP = 0x0008,
            SND_NOSTOP = 0x0010,
            SND_NOWAIT = 0x00002000,
            SND_FILENAME = 0x00020000,
            SND_RESOURCE = 0x00040004
        }

        public enum PlaySoundStatus: uint
        {
            CLOSED = 0,
            NOTREADY = 1,
            PAUSED = 2,
            PLAYING = 3,
            STOPPED = 4
        }

        public FX(IntPtr hwnd, Sounds.SOUNDS curSoundType, string fileName )
        {
            hwndCanvas = hwnd;
            m_sFileName = fileName;

            m_curAlias = getUniqueAlias();
            m_sFullOpenText = String.Format("open {0} type waveaudio alias {1}", m_sFileName, m_curAlias);
            mciSendString(m_sFullOpenText, IntPtr.Zero, 0, IntPtr.Zero);

            StringBuilder strReturn = new StringBuilder(256, 256);
            int ret;
            ret = mciSendString("status " + m_curAlias + " length", strReturn, strReturn.Capacity, IntPtr.Zero);
            medialengthInTicks = new TimeSpan(0,0,0,0, System.Convert.ToInt32(strReturn.ToString())).Ticks;
            canFireNext = System.DateTime.Now.Ticks;

            m_SoundType = curSoundType;
            m_bIsStopped = true;
        }

        private static int m_nUID; 
        private static int getUID { get { m_nUID = (m_nUID < int.MaxValue) ? m_nUID + 1 : 0; return m_nUID; }  }
        private string getUniqueAlias() { return string.Format("temp{0}", getUID ); }

        public void Stop()
        {
            canFireNext = System.DateTime.Now.Ticks;
            mciSendString(("stop " + m_curAlias), IntPtr.Zero, 0, IntPtr.Zero);
            mciSendString((String.Format("seek {0} to start", m_curAlias)), IntPtr.Zero, 0, IntPtr.Zero);
            m_bIsStopped = true;
            return;
        }

        public bool IsPlaying() { return canFireNext > System.DateTime.Now.Ticks; }

        public PlaySoundStatus getStatus()
        {
            StringBuilder strReturn = new StringBuilder(256, 256);
            int ret;
            ret = mciSendString("status " + m_curAlias  + " mode", strReturn, strReturn.Capacity, IntPtr.Zero);
            switch(strReturn.ToString())
            {
                case "stopped":
                    return PlaySoundStatus.STOPPED;
                case "playing":
                    return PlaySoundStatus.PLAYING;
                case "paused":
                    return PlaySoundStatus.PAUSED;
                case "not ready":
                    return PlaySoundStatus.NOTREADY;
                default:
                    return PlaySoundStatus.CLOSED;
            }
        }

        private Thread myThread = null;
        private string m_sSeekString;
        private string m_sPlayData;
        private List<Thread> m_Threads = new List<Thread>(10); // Specific for projectile use. 

        public void Play()
        {
            if (myThread == null)
            {
                m_sSeekString = (String.Format("seek {0} to start", m_curAlias));
                m_sPlayData = (String.Format("play {0}", m_curAlias));
                myThread = new Thread(() => playSound(m_sFullOpenText, m_sSeekString, m_sPlayData));
            }

            if (m_SoundType == Sounds.SOUNDS.PROJECTILE)
            {
                Thread cur = new Thread(() => playSound(m_sFullOpenText, m_sSeekString, m_sPlayData));
                cur.Start();
                m_Threads.Add(cur);
                for (int x = m_Threads.Count-1; x >= 0; x--)
                {
                    if (m_Threads[x].ThreadState == ThreadState.Stopped)
                        m_Threads.RemoveAt(x);
                }
            }
            else if ( !IsPlaying() && ( myThread.ThreadState == ThreadState.Unstarted  || myThread.ThreadState == ThreadState.Stopped ) )
            {
                myThread = new Thread(() => playSound(m_sFullOpenText, m_sSeekString, m_sPlayData));
                myThread.Start();
                canFireNext = System.DateTime.Now.AddTicks(medialengthInTicks).Ticks;
            }

        }

        private static void playSound( string cmdOpenText, string cmdSeek, string cmdPlay )
        {
            mciSendString(cmdOpenText, IntPtr.Zero, 0, IntPtr.Zero);
            mciSendString(cmdSeek, IntPtr.Zero, 0, IntPtr.Zero);
            mciSendString(cmdPlay, IntPtr.Zero, 0, IntPtr.Zero);
        }
    }

    public class WaveHeader
    {
        private const string FILE_TYPE_ID = "RIFF";
        private const string MEDIA_TYPE_ID = "WAVE";

        public string FileTypeId { get; private set; }
        public UInt32 FileLength { get; set; }
        public string MediaTypeId { get; private set; }

        public WaveHeader()
        {
            FileTypeId = FILE_TYPE_ID;
            MediaTypeId = MEDIA_TYPE_ID;
            // Minimum size is always 4 bytes
            FileLength = 4;
        }

        public byte[] GetBytes()
        {
            List<Byte> chunkData = new List<byte>();
            chunkData.AddRange(Encoding.ASCII.GetBytes(FileTypeId));
            chunkData.AddRange(BitConverter.GetBytes(FileLength));
            chunkData.AddRange(Encoding.ASCII.GetBytes(MediaTypeId));

            return chunkData.ToArray();
        }
    }


    public class DataChunk
    {
        private const string CHUNK_ID = "data";

        public string ChunkId { get; private set; }
        public UInt32 ChunkSize { get; set; }
        public short[] WaveData; //  = new List<short>(); // { get; set; }

        public DataChunk()
        {
            ChunkId = CHUNK_ID;
            ChunkSize = 0;  // Until we add some data
        }

        public UInt32 Length()
        {
            return (UInt32)GetBytes().Length;
        }

        public byte[] GetBytes()
        {
            List<Byte> chunkBytes = new List<Byte>();

            chunkBytes.AddRange(Encoding.ASCII.GetBytes(ChunkId));
            chunkBytes.AddRange(BitConverter.GetBytes(ChunkSize));
            byte[] bufferBytes = new byte[WaveData.Length * 2];
            System.Buffer.BlockCopy(WaveData.ToArray(), 0, bufferBytes, 0,
               bufferBytes.Length);
            chunkBytes.AddRange(bufferBytes.ToList());

            return chunkBytes.ToArray();
        }

        public void AddSampleDataStereo(short[] leftBuffer, short[] rightBuffer)
        {
            WaveData = new short[leftBuffer.Length + rightBuffer.Length];
            int bufferOffset = 0;
            for (int index = 0; index < WaveData.Length; index += 2)
            {
                WaveData[index] = leftBuffer[bufferOffset];
                WaveData[index + 1] = rightBuffer[bufferOffset];
                bufferOffset++;
            }
            ChunkSize = (UInt32)WaveData.Length * 2;
        }
    }

    public class FormatChunk
    {
        private ushort _bitsPerSample;
        private ushort _channels;
        private uint _frequency;
        private const string CHUNK_ID = "fmt ";

        public string ChunkId { get; private set; }
        public UInt32 ChunkSize { get; private set; }
        public UInt16 FormatTag { get; private set; }

        public UInt16 Channels
        {
            get { return _channels; }
            set { _channels = value; RecalcBlockSizes(); }
        }

        public UInt32 Frequency
        {
            get { return _frequency; }
            set { _frequency = value; RecalcBlockSizes(); }
        }

        public UInt32 AverageBytesPerSec { get; private set; }
        public UInt16 BlockAlign { get; private set; }

        public UInt16 BitsPerSample
        {
            get { return _bitsPerSample; }
            set { _bitsPerSample = value; RecalcBlockSizes(); }
        }

        public FormatChunk()
        {
            ChunkId = CHUNK_ID;
            ChunkSize = 16;
            FormatTag = 1;       // MS PCM (Uncompressed wave file)
            Channels = 2;        // 2 = stereo
            Frequency = 44100;   // Default to 44100hz
            BitsPerSample = 16;  // Default to 16bits
            RecalcBlockSizes();
        }

        private void RecalcBlockSizes()
        {
            BlockAlign = (UInt16)(_channels * (_bitsPerSample / 8));
            AverageBytesPerSec = _frequency * BlockAlign;
        }

        public byte[] GetBytes()
        {
            List<Byte> chunkBytes = new List<byte>();

            chunkBytes.AddRange(Encoding.ASCII.GetBytes(ChunkId));
            chunkBytes.AddRange(BitConverter.GetBytes(ChunkSize)); // writer.Write(formatChunkSize);
            chunkBytes.AddRange(BitConverter.GetBytes(FormatTag)); // writer.Write(formatType);
            chunkBytes.AddRange(BitConverter.GetBytes(Channels)); //writer.Write(tracks);
            chunkBytes.AddRange(BitConverter.GetBytes(Frequency)); // writer.Write(samplesPerSecond);
            chunkBytes.AddRange(BitConverter.GetBytes(AverageBytesPerSec)); // writer.Write(bytesPerSecond);
            chunkBytes.AddRange(BitConverter.GetBytes(BlockAlign)); // writer.Write(frameSize);
            chunkBytes.AddRange(BitConverter.GetBytes(BitsPerSample)); // writer.Write(bitsPerSample);

            return chunkBytes.ToArray();
        }

        public UInt32 Length()
        {
            return (UInt32)GetBytes().Length;
        }

    }
}
