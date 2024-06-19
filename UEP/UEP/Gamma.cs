using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UEP
{
    public class Gamma
    {
        private IntPtr createdDC;
        private Gamma.WinApi.RAMP? _orginalRamp;

        public float CurrentGamma { get; private set; } = float.NaN;

        public Gamma(string screenDeviceName)
        {
            this.createdDC = Gamma.WinApi.CreateDC((string)null, screenDeviceName, (string)null, IntPtr.Zero);
        }

        public bool Set(float gamma)
        {
            if (!this._orginalRamp.HasValue && !this.TrySetDefaultRamp() || (double)gamma > 5.0 || (double)gamma < 0.0)
                return false;
            Gamma.WinApi.RAMP lpRamp = new Gamma.WinApi.RAMP();
            lpRamp.Red = new ushort[256];
            lpRamp.Green = new ushort[256];
            lpRamp.Blue = new ushort[256];
            for (int index = 1; index < 256; ++index)
            {
                double num = Math.Pow((double)(index + 1) / 256.0, (double)gamma) * (double)ushort.MaxValue + 0.5;
                if (num > (double)ushort.MaxValue)
                    num = (double)ushort.MaxValue;
                lpRamp.Red[index] = lpRamp.Blue[index] = lpRamp.Green[index] = (ushort)num;
            }
            this.CurrentGamma = gamma;
            return Gamma.WinApi.SetDeviceGammaRamp(this.createdDC, ref lpRamp);
        }

        public bool Restore()
        {
            if (!this._orginalRamp.HasValue)
                return false;
            Gamma.WinApi.RAMP lpRamp = this._orginalRamp.Value;
            int num = Gamma.WinApi.SetDeviceGammaRamp(this.createdDC, ref lpRamp) ? 1 : 0;
            if (num == 0)
                return num != 0;
            this.CurrentGamma = float.NaN;
            return num != 0;
        }

        public void Dispose()
        {
            this.Restore();
            if (!(this.createdDC != IntPtr.Zero))
                return;
            Gamma.WinApi.DeleteDC(this.createdDC);
        }

        private bool TrySetDefaultRamp()
        {
            Gamma.WinApi.RAMP lpRamp = new Gamma.WinApi.RAMP();
            if (!Gamma.WinApi.GetDeviceGammaRamp(this.createdDC, ref lpRamp))
                return false;
            this._orginalRamp = new Gamma.WinApi.RAMP?(lpRamp);
            return true;
        }

        private class WinApi
        {
            [DllImport("gdi32.dll")]
            public static extern bool SetDeviceGammaRamp(IntPtr hdc, ref Gamma.WinApi.RAMP lpRamp);

            [DllImport("gdi32.dll")]
            public static extern bool GetDeviceGammaRamp(IntPtr hdc, ref Gamma.WinApi.RAMP lpRamp);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateDC(
              string lpszDriver,
              string lpszDevice,
              string lpszOutput,
              IntPtr lpInitData);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC([In] IntPtr hdc);

            public struct RAMP
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public ushort[] Red;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public ushort[] Green;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public ushort[] Blue;
            }
        }
    }
}
