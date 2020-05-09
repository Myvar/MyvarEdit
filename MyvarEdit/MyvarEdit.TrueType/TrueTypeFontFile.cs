using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DeepCopy;
using LibTessDotNet;
using MyvarEdit.TrueType.Internals;

namespace MyvarEdit.TrueType
{
    public class TrueTypeFontFile
    {
        public TrueTypeHeader Header { get; set; }
        public MaxP MaxP { get; set; }
        private Dictionary<int, int> _cMapIndexes = new Dictionary<int, int>();
        public Dictionary<int, Glyf> Glyfs { get; set; } = new Dictionary<int, Glyf>();

        public void Load(string file)
        {
            Load(File.OpenRead(file));
        }

        public void Load(byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                Load(mem);
            }
        }

        public void Load(Stream stream)
        {
            //NOTE: we do not want to store the stream in the class because i want to close the stream once the data is read
            var off = ReadStruct<OffsetTable>(stream);

            var glyfOffsets = new List<int>();
            var glyfOffset = 0;

            for (int i = 0; i < off.NumTables; i++)
            {
                var te = ReadStruct<TableEntry>(stream);
                var id = te.ToString();
                var oldPos = stream.Position;
                switch (id)
                {
                    case "head":
                        stream.Position = te.Offset;
                        Header = ReadStruct<TrueTypeHeader>(stream);

                        break;
                    case "maxp":
                        stream.Position = te.Offset;
                        MaxP = ReadStruct<MaxP>(stream);

                        break;
                    case "cmap":
                        stream.Position = te.Offset;
                        ReadCmap(stream);
                        break;
                    case "loca":
                        //@Hack should not do this but just to test for now
                        for (int charCode = 0; charCode < 10000; charCode++)
                            glyfOffsets.Add(GetGlyphOffset(te, stream, charCode));
                        break;
                    case "glyf":
                        glyfOffset = (int) te.Offset;
                        break;
                }

                stream.Position = oldPos;
            }

            for (int charCode = 0; charCode < 255; charCode++)
            {
                var maped = _cMapIndexes[charCode];
                stream.Position = glyfOffset + glyfOffsets[maped];
                Glyfs.Add(charCode, ReadGlyph(stream, (byte) charCode));
            }


            foreach (var (charcode, glyf) in Glyfs.ToArray())
            {
                if (glyf.Components.Count != 0)
                {
                    foreach (var component in glyf.Components)
                    {
                        var componentCharCode = _cMapIndexes.Values.ToList().IndexOf(component.GlyphIndex);

                        if (componentCharCode == -1) componentCharCode = 0;

                        if (!Glyfs.ContainsKey(componentCharCode))
                        {
                            var maped = _cMapIndexes[componentCharCode];
                            stream.Position = glyfOffset + glyfOffsets[maped];
                            Glyfs.Add(componentCharCode, ReadGlyph(stream, (byte) charcode));
                        }

                        var shapes = DeepCopier.Copy(Glyfs[componentCharCode].Shapes);


                        if (component.Flags.HasFlag(ComponentFlags.UseMyMetrics))
                        {
                            glyf.Xmax = Glyfs[componentCharCode].Xmax;
                            glyf.Xmin = Glyfs[componentCharCode].Xmin;
                            glyf.Ymax = Glyfs[componentCharCode].Ymax;
                            glyf.Ymin = Glyfs[componentCharCode].Ymin;
                        }

                        foreach (var shape in shapes)
                        {
                            foreach (var point in shape)
                            {
                                if (component.Flags.HasFlag(ComponentFlags.UnscaledComponentOffset))
                                {
                                    point.X += component.E;
                                    point.Y += component.F;
                                }
                                else
                                {
                                    point.X = component.A * point.X + component.B * point.Y + component.E;
                                    point.Y = component.C * point.X + component.D * point.Y + component.F;
                                }
                            }
                        }

                        glyf.Shapes.AddRange(shapes);
                    }
                }
            }
        }

        private static float Bezier(float p0, float p1, float p2, float t) // Parameter 0 <= t <= 1
        {
            //B(T) = P1 + (1 - t)^2 * (P0 - P1) + t^2 (P2 - P1)

            return p1 + MathF.Pow(1f - t, 2) * (p0 - p1) + MathF.Pow(t, 2) * (p2 - p1);
        }

        private Glyf ReadGlyph(Stream s, byte charcode)
        {
            var re = new Glyf();
            var gd = ReadStruct<GlyphDescription>(s);
            var topPos = s.Position;
            re.NumberOfContours = gd.numberOfContours;
            re.Xmax = gd.xMax;
            re.Xmin = gd.xMin;
            re.Ymax = gd.yMax;
            re.Ymin = gd.yMin;


            var tmpXPoints = new List<int>();
            var tmpYPoints = new List<int>();
            var lst = new List<bool>();
            var flags = new List<OutlineFlags>();
            var max = 0;

            if (gd.numberOfContours >= 0) //simple glyph
            {
                var endPtsOfContours = ReadArray<ushort>(s, gd.numberOfContours);

                for (int i = 0; i < gd.numberOfContours; i++)
                {
                    re.ContourEnds.Add(endPtsOfContours[i]);
                }

                var instructionLength = ReadArray<ushort>(s, 1)[0];
                var instructions = ReadArray<byte>(s, instructionLength);

                max = endPtsOfContours.Max() + 1;

                //NOTE: we are most probably reading junk because im reading to meany bytes
                var flagsRes = s.Position;
                var tmpflags = ReadArray<byte>(s, max * 2);


                var off = 0;


                for (int p = 0; p < max; p++)
                {
                    var f = (OutlineFlags) tmpflags[off++];

                    flags.Add(f);
                    lst.Add(f.HasFlag(OutlineFlags.OnCurve));

                    if (f.HasFlag(OutlineFlags.Repeat))
                    {
                        var z = tmpflags[off++];
                        p += z;

                        for (int i = 0; i < z; i++)
                        {
                            flags.Add(f);
                            lst.Add(f.HasFlag(OutlineFlags.OnCurve));
                        }
                    }
                }


                var xoff = 0;

                void IterPoints(byte[] arr, OutlineFlags byteFlag, OutlineFlags deltaFlag, List<int> tmp)
                {
                    xoff = 0;
                    var xVal = 0;

                    for (int i = 0; i < max; i++)
                    {
                        var flag = flags[i];

                        if (flag.HasFlag(byteFlag))
                        {
                            if (flag.HasFlag(deltaFlag))
                            {
                                xVal += arr[xoff++];
                            }
                            else
                            {
                                xVal -= arr[xoff++];
                            }
                        }
                        else if (!flag.HasFlag(deltaFlag) && !flag.HasFlag(byteFlag))
                        {
                            xVal += BitConverter.ToInt16(new[] {arr[xoff++], arr[xoff++]}.Reverse().ToArray());
                        }
                        else
                        {
                        }

                        tmp.Add(xVal);
                    }
                }


                s.Position = flagsRes + off;
                var resPoint = s.Position;
                var xPoints = ReadArray<byte>(s, max * 2);

                IterPoints(xPoints, OutlineFlags.XIsByte, OutlineFlags.XDelta, tmpXPoints);

                s.Position = flagsRes + off + xoff;
                var yPoints = ReadArray<byte>(s, max * 2);
                IterPoints(yPoints, OutlineFlags.YIsByte, OutlineFlags.YDelta, tmpYPoints);

                GlyfPoint MidpointRounding(GlyfPoint a, GlyfPoint b)
                {
                    return new GlyfPoint(
                        (a.X + b.X) / 2f,
                        (a.Y + b.Y) / 2f
                    );
                }


                re.Points.Add(new GlyfPoint(tmpXPoints[0], tmpYPoints[0]));
                re.Curves.Add(lst[0]);
                for (int i = 1; i < max; i++)
                {
                    re.Points.Add(new GlyfPoint(tmpXPoints[i], tmpYPoints[i])
                    {
                        IsOnCurve = lst[i]
                    });
                    re.Curves.Add(lst[i]);
                }

                var points = new List<GlyfPoint>();
                for (var i = 1; i < re.Points.Count; i++)
                {
                    var point = re.Points[i];

                    if (re.ContourEnds.Contains((ushort) i))
                    {
                        points.Add(point);


                        if (re.Shapes.Count == 0) points.Add(re.Points[0]);
                        re.Shapes.Add(points);
                        points = new List<GlyfPoint>();
                    }
                    else
                    {
                        points.Add(point);
                    }
                }

                foreach (var shape in re.Shapes)
                {
                    for (var i = 1; i < shape.Count; i++)
                    {
                        var a = shape[i];
                        var b = shape[i - 1];
                        if (!a.IsOnCurve && !b.IsOnCurve)
                        {
                            var midPoint = MidpointRounding(a, b);
                            midPoint.isMidpoint = true;
                            midPoint.IsOnCurve = false;
                            shape.Insert(i, midPoint);
                            i++;
                        }
                    }
                }

                // if (charcode == (byte) '8') Debugger.Break();

                foreach (var shape in re.Shapes)
                {
                    var shapes = shape.ToArray();
                    shape.Clear();
                    shape.Add(shapes[0]);
                    for (var i = 1; i < shapes.Length - 1; i++)
                    {
                        if (!shapes[i].IsOnCurve)
                        {
                            var res = 15f;

                            var a = i == 0 ? shapes[^1] : shapes[i - 1];
                            var b = shapes[i];
                            var c = i + 1 >= shapes.Length ? shapes[0] : shapes[i + 1];

                            for (int j = 0; j <= res; j++)
                            {
                                var t = j / res;
                                shape.Add(new GlyfPoint(
                                    Bezier(a.X, b.X, c.X, t),
                                    Bezier(a.Y, b.Y, c.Y, t))
                                {
                                    //isMidpoint = true
                                });
                            }
                        }
                        else
                        {
                            shape.Add(shapes[i]);
                        }
                    }

                    shape.Add(shapes.Last());
                }
            }
            else
            {
                s.Position = topPos;
                var components = new List<ComponentGlyph>();
                var flag = ComponentFlags.MoreComponents;

                while (flag.HasFlag(ComponentFlags.MoreComponents))
                {
                    var fval = ReadArray<ushort>(s, 1)[0];
                    flag = (ComponentFlags) (fval);
                    var component = new ComponentGlyph();
                    component.GlyphIndex = ReadArray<ushort>(s, 1)[0];

                    component.Flags = flag;

                    if (flag.HasFlag(ComponentFlags.Arg1And2AreWords))
                    {
                        component.Argument1 = ReadArray<short>(s, 1)[0];
                        component.Argument2 = ReadArray<short>(s, 1)[0];
                    }
                    else
                    {
                        component.Argument1 = ReadArray<byte>(s, 1)[0];
                        component.Argument2 = ReadArray<byte>(s, 1)[0];
                    }

                    if (flag.HasFlag(ComponentFlags.ArgsAreXyValues))
                    {
                        component.E = component.Argument1;
                        component.F = component.Argument2;
                    }
                    else
                    {
                        component.DestPointIndex = component.Argument1;
                        component.SrcPointIndex = component.Argument2;
                    }

                    if (flag.HasFlag(ComponentFlags.WeHaveAScale))
                    {
                        component.A = ReadArray<short>(s, 1)[0] / (1 << 14);
                        component.D = component.A;
                    }
                    else if (flag.HasFlag(ComponentFlags.WeHaveAnXAndYScale))
                    {
                        component.A = ReadArray<short>(s, 1)[0] / (1 << 14);
                        component.D = ReadArray<short>(s, 1)[0] / (1 << 14);
                    }
                    else if (flag.HasFlag(ComponentFlags.WeHaveATwoByTwo))
                    {
                        component.A = ReadArray<short>(s, 1)[0] / (1 << 14);
                        component.B = ReadArray<short>(s, 1)[0] / (1 << 14);
                        component.C = ReadArray<short>(s, 1)[0] / (1 << 14);
                        component.D = ReadArray<short>(s, 1)[0] / (1 << 14);
                    }


                    components.Add(component);
                }

                if (flag.HasFlag(ComponentFlags.WeHaveInstructions))
                {
                    var off = ReadArray<ushort>(s, 1)[0];
                    s.Position += off;
                }

                re.Components.AddRange(components);
            }

            //now triangulate glyf
            var tess = new LibTessDotNet.Tess();

            foreach (var shape in re.Shapes)
            {
                var contour = new LibTessDotNet.ContourVertex[shape.Count];
                for (var i = 0; i < shape.Count; i++)
                {
                    var point = shape[i];
                    contour[i] = new ContourVertex(new Vec3(point.X, point.Y, 0));
                }

                tess.AddContour(contour);
            }

            tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3);

            int numTriangles = tess.ElementCount;
            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;

                re.Triangles.Add(new ComponentTriangle()
                {
                    A = new GlyfPoint(v0.X, v0.Y),
                    B = new GlyfPoint(v1.X, v1.Y),
                    C = new GlyfPoint(v2.X, v2.Y),
                });
            }

            return re;
        }

        private int GetGlyphOffset(TableEntry te, Stream s, int index)
        {
            if (Header.IndexToLocFormat == 1)
            {
                s.Position = (int) te.Offset + index * 4;
                return (int) ReadArray<uint>(s, 1)[0];
            }

            s.Position = (int) te.Offset + index * 2;
            return (int) ReadArray<ushort>(s, 1)[0] * 2;
        }

        private void ReadCmap(Stream s)
        {
            var startPos = s.Position;
            var idx = ReadStruct<CmapIndex>(s);
            var subtablesStart = s.Position;
            for (int i = 0; i < idx.NumberSubtables; i++)
            {
                s.Position = subtablesStart + (i * 8);
                var encoding = ReadStruct<CmapEncoding>(s);


                s.Position = startPos + encoding.offset;
                var old = s.Position;
                var cmap = ReadStruct<Cmap>(s);

                if (encoding.platformID == 0 && cmap.format == 4)
                {
                    var range = cmap.searchRange;
                    var segcount = cmap.segCountX2 / 2;

                    var endCode = ReadArray<ushort>(s, segcount);
                    s.Position += 2;
                    var startCode = ReadArray<ushort>(s, segcount);
                    var idDelta = ReadArray<ushort>(s, segcount);
                    var idRangeOffsetptr = s.Position;
                    var idRangeOffset = ReadArray<ushort>(s, segcount * 8 * range);

                    var startOfIndexArray = s.Position;

                    //@Hack should not do this but just to test for now
                    for (int charCode = 0; charCode < 10000; charCode++)
                    {
                        var found = false;
                        for (int segIdx = 0; segIdx < segcount - 1; segIdx++)
                        {
                            if (endCode[segIdx] >= charCode && startCode[segIdx] <= charCode)
                            {
                                if (idRangeOffset[segIdx] != 0)
                                {
                                    var z =
                                        idRangeOffset[
                                            segIdx + idRangeOffset[segIdx] / 2 + (charCode - startCode[segIdx])];

                                    // var z = ReadArray<short>(s, 1)[0];

                                    var delta = (short) idDelta[segIdx];
                                    _cMapIndexes.Add(charCode, (short) (z) + delta);
                                }
                                else
                                {
                                    _cMapIndexes.Add(charCode, (short) idDelta[segIdx] + charCode);
                                }

                                found = true;
                            }
                        }

                        if (!found)
                        {
                            _cMapIndexes.Add(charCode, 0);
                        }
                    }

                    return;
                }
                else
                {
                    Console.WriteLine($"Only Cmap format  4 is Implemented, you tried using: {cmap.format}");
                }
            }
        }

        private T[] ReadArray<T>(Stream s, int leng, bool dontFlipBits = false)
        {
            var re = new T[leng];

            var elmSize = Marshal.SizeOf<T>();
            var size = elmSize * leng;

            //read the ptr into buf
            var buf = new byte[size];
            var readSize = s.Read(buf);

            if (readSize != size)
            {
                //@Error need to deal with this, not sure how this might be possible
            }

            if (typeof(T) == typeof(byte)) return buf as T[];

            var converter = FindOverload<T>();

            for (int i = 0; i < leng; i++)
            {
                var off = i * elmSize;
                var seg = dontFlipBits ? buf[off..(off + elmSize)] : buf[off..(off + elmSize)].Reverse().ToArray();
                re[i] = converter(seg);
            }

            return re;
        }

        private Func<byte[], T> FindOverload<T>()
        {
            foreach (var meth in typeof(BitConverter).GetMethods())
            {
                var paramz = meth.GetParameters();
                if (meth.Name.StartsWith("To") &&
                    meth.ReturnType == typeof(T) &&
                    paramz.Length == 2)
                {
                    return (x) =>
                        (T) meth.Invoke(null, BindingFlags.Static, null, new object[] {x, 0},
                            CultureInfo.CurrentCulture);
                }
            }

            return null;
        }

        private T ReadStruct<T>(Stream s) where T : struct
        {
            //get the size of the ptr
            var size = Marshal.SizeOf<T>();

            //read the ptr into buf
            var buf = new byte[size];
            var readSize = s.Read(buf);

            if (readSize != size)
            {
                //@Error need to deal with this, not sure how this might be possible
            }


            //The endianness is not correct so we need to swap it
            foreach (var field in typeof(T).GetFields())
            {
                var offset = Marshal.OffsetOf<T>(field.Name).ToInt32();

                var fsize = Marshal.SizeOf(field.FieldType);

                Array.Reverse(buf, offset, fsize);
            }

            //copy the buffer into a pointer and create the struct from that
            var re = new T();
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buf, 0, ptr, size);
            re = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);

            return re;
        }
    }
}