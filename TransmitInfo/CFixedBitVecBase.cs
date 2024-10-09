using System.Runtime.InteropServices;

namespace AdminESP;
    
[StructLayout(LayoutKind.Sequential)]
public unsafe struct CFixedBitVecBase
{
    private const int LOG2_BITS_PER_INT = 5;
    private const int MAX_EDICT_BITS = 14;
    private const int BITS_PER_INT = 32;
    private const int MAX_EDICTS = 1 << MAX_EDICT_BITS;

    private uint* m_Ints;

    public void Clear(int bitNum)
    {
        if (!(bitNum >= 0 && bitNum < MAX_EDICTS))
            return;

        uint* pInt = m_Ints + BitVec_Int(bitNum);
        *pInt &= ~(uint)BitVec_Bit(bitNum);
    }

    public bool IsBitSet(int bitNum)
    {
        if (!(bitNum >= 0 && bitNum < MAX_EDICTS))
            return false;

        uint* pInt = m_Ints + BitVec_Int(bitNum);
        return  ( *pInt & BitVec_Bit( bitNum ) ) != 0 ;
    }

    private int BitVec_Int(int bitNum) => bitNum >> LOG2_BITS_PER_INT;
    private int BitVec_Bit(int bitNum) => 1 << ((bitNum) & (BITS_PER_INT - 1));
}

