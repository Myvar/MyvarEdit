using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MyvarEdit.TrueType.Internals
{
    [Flags]
    public enum OutlineFlags
    {
        OnCurve = 1,
        XIsByte = 2,
        YIsByte = 4,
        Repeat = 8,
        XDelta = 16,
        YDelta = 32,
    }

    [Flags]
    public enum ComponentFlags 
    {
        Arg1And2AreWords = 0x0001,
        ArgsAreXyValues = 0x0002,
        RoundXyToGrid = 0x0004,
        WeHaveAScale = 0x0008,

        Reserved = 0xE010,
        MoreComponents = 0x0020,
        WeHaveAnXAndYScale = 0x0040,
        WeHaveATwoByTwo = 0x0080,
        WeHaveInstructions = 0x0100,
        UseMyMetrics = 0x0200,
        OverlapComponent = 0x0400,
        ScaledComponentOffset = 0x0800,
        UnscaledComponentOffset = 0x1000,
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CmapEncoding
    {
        public ushort platformID;
        public ushort platformSpecificID;
        public uint offset;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct GlyphDescription
    {
        [FieldOffset(0)] public short numberOfContours;
        [FieldOffset(2)] public short xMin;
        [FieldOffset(3)] public short yMin;
        [FieldOffset(6)] public short xMax;
        [FieldOffset(8)] public short yMax;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Cmap
    {
        public ushort format;
        public ushort length;
        public ushort language;
        public ushort segCountX2;
        public ushort searchRange;
        public ushort entrySelector;
        public ushort rangeShift;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CmapIndex
    {
        public ushort Version;
        public ushort NumberSubtables;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TrueTypeHeader
    {
        public uint Version;
        public uint FontRevision;
        public uint CheckSumAdjustment;
        public uint MagicNumber;
        public ushort Flags;
        public ushort UnitsPerEm;
        public ulong Created;
        public ulong Modified;
        public short Xmin;
        public short Ymin;
        public short Xmax;
        public short Ymax;
        public ushort MacStyle;
        public ushort LowestRecPPEM;
        public short FontDirectionHint;
        public short IndexToLocFormat;
        public short GlyphDataFormat;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct longHorMetric
    {
        public ushort advanceWidth;
        public short leftSideBearing;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HorizontalHeaderTable
    {
        public uint Version;
        public short ascent;
        public short descent;
        public short lineGap;
        public ushort advanceWidthMax;
        public short minLeftSideBearing;
        public short minRightSideBearing;
        public short xMaxExtent;
        public short caretSlopeRise;
        public short caretSlopeRun;
        public short caretOffset;
        public short reserved;
        public short reserved1;
        public short reserved2;
        public short reserved4;
        public short metricDataFormat;
        public ushort numOfLongHorMetrics;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VerticalHeaderTable
    {
        public uint Version;
        public short vertTypoAscender;
        public short vertTypoDescender;
        public short vertTypoLineGap;
        public short advanceHeightMax;
        public short minTopSideBearing;
        public short minBottomSideBearing;
        public short yMaxExtent;
        public short caretSlopeRise;
        public short caretSlopeRun;
        public short caretOffset;
        public short reserved;
        public short reserved1;
        public short reserved2;
        public short reserved4;
        public short metricDataFormat;
        public ushort numOfLongVerMetrics;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MaxP    
    {
        public uint Version;
        public ushort numGlyphs;
        public ushort maxPoints;
        public ushort maxContours;
        public ushort maxComponentPoints;
        public ushort maxComponentContours;
        public ushort maxZones;
        public ushort maxTwilightPoints;
        public ushort maxStorage;
        public ushort maxFunctionDefs;
        public ushort maxInstructionDefs;
        public ushort maxStackElements;
        public ushort maxSizeOfInstructions;
        public ushort maxComponentElements;
        public ushort maxComponentDepth;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct TableEntry
    {
        [FieldOffset(0)] public uint Id;
        [FieldOffset(4)] public uint CheckSum;
        [FieldOffset(8)] public uint Offset;
        [FieldOffset(12)] public uint Length;

        public override string ToString()
        {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(Id).Reverse().ToArray());
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OffsetTable
    {
        public uint ScalerType;
        public ushort NumTables;
        public ushort SearchRange;
        public ushort EntrySelector;
        public ushort RangeShift;
    }
}