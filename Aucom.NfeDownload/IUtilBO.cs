using System;
using System.Runtime.InteropServices;
namespace Scire.NfeDownload
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUtilBO
    {
        string PeriodoSemana(DateTime data);
    }
}
