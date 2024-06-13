using System;
using System.Windows.Forms;
using CoreAudioApi;
using NAudio.Dsp;

namespace VolumeControl
{
    public partial class Form1 : Form
    {
        private MMDeviceEnumerator devEnum;
        private MMDevice renderDevice;
        private MMDevice captureDevice;
        private BiQuadFilter[] filters;

        public Form1()
        {
            InitializeComponent();
            InitializeVolumeControl();
            InitializeMicVolumeControl();
            InitializeEqualizer();
        }

        private void InitializeVolumeControl()
        {
            devEnum = new MMDeviceEnumerator();
            renderDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            // 트랙바 초기 설정
            trackBarVolume.Minimum = 0;
            trackBarVolume.Maximum = 100;
            trackBarVolume.Value = (int)(renderDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            labelVolume.Text = $"Volume: {trackBarVolume.Value}%";

            // 트랙바 이벤트 핸들러 연결
            trackBarVolume.Scroll += TrackBarVolume_Scroll;
        }

        private void TrackBarVolume_Scroll(object sender, EventArgs e)
        {
            // 볼륨 설정
            float volume = trackBarVolume.Value / 100.0f;
            renderDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            labelVolume.Text = $"Volume: {trackBarVolume.Value}%";
        }

        private void InitializeMicVolumeControl()
        {
            captureDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);

            // 마이크 볼륨 트랙바 초기 설정
            trackBarMicVolume.Minimum = 0;
            trackBarMicVolume.Maximum = 100;
            trackBarMicVolume.Value = (int)(captureDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            labelMicVolume.Text = $"Mic Volume: {trackBarMicVolume.Value}%";

            // 트랙바 이벤트 핸들러 연결
            trackBarMicVolume.Scroll += TrackBarMicVolume_Scroll;
        }

        private void TrackBarMicVolume_Scroll(object sender, EventArgs e)
        {
            // 마이크 볼륨 설정
            float volume = trackBarMicVolume.Value / 100.0f;
            captureDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            labelMicVolume.Text = $"Mic Volume: {trackBarMicVolume.Value}%";
        }

        private void InitializeEqualizer()
        {
            // 이퀄라이저 필터 초기화 (각 주파수 대역별로)
            filters = new BiQuadFilter[10];
            float[] frequencies = { 32, 64, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 };

            for (int i = 0; i < filters.Length; i++)
            {
                filters[i] = BiQuadFilter.PeakingEQ(44100, frequencies[i], 1.0f, 0);
                trackBars[i].Scroll += (s, e) => UpdateEqualizer(i, trackBars[i].Value);
            }
        }

        private void UpdateEqualizer(int band, int gain)
        {
            float gainFactor = gain / 10.0f;
            filters[band].Gain = gainFactor;
        }
    }
}
