using System;
using System.Collections;

namespace Classes
{
    public delegate ushort VmGetMemoryValue(ushort loc);

    public class CmdOperation
    {
        public class Operation
        {
            bool codeSet;
            bool lowSet;
            bool highSet;

            byte code;
            byte low;
            byte high;

            ushort word;

            string str = string.Empty;

            public VmGetMemoryValue getMemoryValue;

            public void Clear()
            {
                codeSet = false;
                lowSet = false;
                highSet = false;
                str = String.Empty;
            }

            public byte Code
            {
                set
                {
                    if (codeSet == false)
                    {
                        code = value;
                        codeSet = true;
                    }
                    else throw new InvalidOperationException();
                }
                get
                {
                    if (codeSet)
                        return code;
                    else throw new InvalidOperationException();
                }
            }

            public ushort Word
            {
                get
                {
                    if (highSet)
                        return word;
                    else throw new InvalidOperationException();
                }
            }

            public byte Low
            {
                set
                {
                    if (lowSet == false)
                    {
                        low = value;
                        lowSet = true;
                    }
                    else throw new InvalidOperationException();
                }
                get
                {
                    if (lowSet)
                        return low;
                    else throw new InvalidOperationException();
                }
            }

            public byte High
            {
                set
                {
                    if (highSet == false)
                    {
                        high = value;
                        highSet = true;
                        word = (ushort)(low + (high << 8));
                    }
                    else throw new InvalidOperationException();
                }
                get
                {
                    if (highSet)
                        return high;
                    else throw new InvalidOperationException();
                }
            }
            public string String
            {
                set
                {
                    if (Code == 0x80 || Code == 0x81)
                    {
                        str = value;
                    }
                    else throw new InvalidOperationException();
                }
                get
                {
                    if (Code == 0x80 || Code == 0x81)
                        return str;
                    else throw new InvalidOperationException();
                }
            }

            public ushort GetCmdValue()
            {
                if (codeSet)
                {
                    //System.Console.WriteLine("  GetCmdValue: code: {0:X}", code);

                    switch (code)
                    {
                        case 0x00:
                            if (lowSet)
                                return low;
                            else throw new InvalidOperationException();

                        case 0x01:
                        case 0x03:
                        case 0x80:
                            if (highSet)
                                return getMemoryValue(word);
                            else throw new InvalidOperationException();

                        case 0x02:
                        case 0x81:
                            if (highSet)
                                return word;
                            else throw new InvalidOperationException();

                        default:
                            throw new InvalidOperationException();
                            //return 0;
                    }
                }
                else throw new InvalidOperationException();
            }

            public string PrintCmd()
            {
                if (codeSet)
                {

                    switch (code)
                    {
                        case 0x00:
                            if (lowSet)
                                return String.Format("{0}", low);
                            else throw new InvalidOperationException();

                        case 0x01:
                        case 0x03:
                        case 0x80:
                            if (highSet)
                                return String.Format("<${0,4:X4}>", word);
                            else
                                return String.Format("\"{0}\"", str);

                        case 0x02:
                        case 0x81:
                            if (highSet)
                                return String.Format("{0}", word);
                            else throw new InvalidOperationException();

                        default:
                            throw new InvalidOperationException();
                            //return 0;
                    }
                }
                else throw new InvalidOperationException();
            }
            public bool IsPointer
            {
                get
                {
                    if (Code == 0x1 || Code == 0x3 || Code == 0x81)
                        return true;
                    else
                        return false;
                }
            }
        }
        private readonly Operation[] cmd_ops;

        public CmdOperation(int cmdOpsLimit)
        {
            cmd_ops = new Operation[cmdOpsLimit];

            for (int i = 0; i < cmdOpsLimit; i++)
            {
                cmd_ops[i] = new Operation();
            }
        }

        public void Init(VmGetMemoryValue vmGetMemoryValue)
        {
            Array.ForEach(cmd_ops, op => op.getMemoryValue = vmGetMemoryValue);
        }

        public void Clear()
        {
            Array.ForEach(cmd_ops, op => op.Clear());
        }

        public ref Operation this[int index]
        {
            get => ref cmd_ops[index-1];
            //set => cmd_ops[index-1] = value;
        }
    }
}
