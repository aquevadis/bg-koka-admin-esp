using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

namespace AdminESP;

[StructLayout(LayoutKind.Sequential)]
public struct CCheckTransmitInfo
{
    public CFixedBitVecBase m_pTransmitEntity;
};