using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace mEdit.Core.Editor
{
    public class Workspace
    {
        public Dictionary<string, (Func<object> get, Action<object> set)> Variables { get; set; }
            = new Dictionary<string, (Func<object> get, Action<object> set)>();

        public Dictionary<string, List<Func<object[], object[]>>> Functions { get; set; } =
            new Dictionary<string, List<Func<object[], object[]>>>();


        public Workspace()
        {
            var assm = Assembly.GetExecutingAssembly();


            foreach (var type in assm.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic && method.CustomAttributes.Any())
                    {
                        var initAtt = method.GetCustomAttribute<CommandInitAttribute>();
                        if (initAtt != null)
                        {
                            method.Invoke(null, new object[0]);
                        }

                        var cmdAtt = method.GetCustomAttribute<CommandAttribute>();
                        if (cmdAtt != null)
                        {
                            RegisterFunction(cmdAtt.Target, objects =>
                            {
                                if (method.ReturnType != typeof(void))
                                {
                                    return (object[]) method.Invoke(null, objects);
                                }
                                else
                                {
                                    method.Invoke(null, objects);
                                }

                                return null;
                            });
                        }
                    }
                }
            }
        }

        public void RegisterFunction(string name, Func<object[], object[]> f)
        {
            if (!Functions.ContainsKey(name))
            {
                Functions.Add(name, new List<Func<object[], object[]>>());
            }

            var lst = Functions[name];
            lst.Add(f);
        }


        public void InvokeAllFunction(string name, params object[] args)
        {
            if (Functions.ContainsKey(name))
            {
                var lst = Functions[name];
                foreach (var func in lst)
                {
                    func(args);
                }
            }
        }

        public void Eval(string raw)
        {
            raw = " " + raw + "  ";

            var args = new List<object>();

            var sb = new StringBuilder();

            bool inString = false;

            for (int i = 1; i < raw.Length - 1; i++)
            {
                var c = raw[i];
                var b = raw[i - 1];
                var a = raw[i + 1];

                if (!inString)
                {
                    if (c == '"')
                    {
                        inString = true;
                        continue;
                    }

                    if (c == ' ')
                    {
                        var s = sb.ToString();
                        sb.Clear();

                        if (string.IsNullOrEmpty(s)) continue;

                        if (char.IsNumber(s[0]))
                        {
                            if (s.Contains("."))
                            {
                                args.Add(float.Parse(s));
                            }
                            else
                            {
                                args.Add(int.Parse(s));
                            }
                        }
                        else
                        {
                            args.Add(s);
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '\\' && a == '"') continue;

                    if (c == '"' && b != '\\')
                    {
                        inString = false;

                        args.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            var name = args[0].ToString();
            args.RemoveAt(0);

            InvokeAllFunction(name, args.ToArray());
        }
    }
}