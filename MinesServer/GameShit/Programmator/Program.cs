using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Programmator.SevenZip.LZMA;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace MinesServer.GameShit.Programmator
{
    public class Program
    {
        private Program()
        {

        }
        public Program(Player P,string name,string data)
        {
            owner = P;
            this.name = name;this.data = data;
        }
        public int id { get; set; }
        public string name { get; set; }
        public string data { get; set; }
        public Player owner { get; set; }
        public Dictionary<string, PFunction> programm
        {
            get
            {
                _programm ??= parseNormal();
                return _programm;
            }
        }
        private Dictionary<string,PFunction> parseNormal()
        {
            Dictionary<string, PFunction> functions = new();
            functions[""] = new PFunction();
            string currentFunc = "";
            /*try
            {*/
                byte[] array = SevenZipHelper.Decompress(Convert.FromBase64String(data));
                int num = BitConverter.ToInt32(array, 0);
                var array2 = Encoding.UTF8.GetString(array, num + 4, array.Length - num - 4).Split(':');
                bool containsnextrow = false;
                int index = 0;
                for (int i = 0; i < num; i++)
                {
                    var atype = GetActionType(Convert.ToInt16(array[i + 4]));
                    Console.WriteLine(atype);
                    var name = "0";
                    var number = 0;
                    if (array2.Length > i)
                    {
                        if (array2[i].Contains('@'))
                        {
                            var a3 = array2[i].Split('@');
                            name = a3[0];
                            if (int.TryParse(a3[1], out var n))
                                number = n;
                        }
                        else
                            name = array2[i];
                    }
                    switch (atype)
                    {
                        case ActionType.NextRow:
                            containsnextrow = true;
                            break;
                        case ActionType.CreateFunction:
                            functions.Add(name, new PFunction());
                            currentFunc = name;
                            index = 0;
                            break;
                        case ActionType.WritableState or ActionType.WritableStateLower or ActionType.WritableStateMore:
                            functions[currentFunc] += new PAction(atype, name, number);
                            break;
                        case ActionType.RunFunction or ActionType.RunIfFalse or ActionType.RunIfTrue or ActionType.RunOnRespawn
                        or ActionType.RunState or ActionType.RunSub or ActionType.GoTo:
                            functions[currentFunc] += new PAction(atype, name);
                            break;
                        case ActionType.None:
                            break;
                        case 0 or _:
                            functions[currentFunc] += new PAction(atype);
                            break;
                    }
                    if (index > 0 && index % 15 == 0)
                    {
                        if (functions[currentFunc].actions.Count > 0 && functions[currentFunc].actions.Last().type is not ActionType.GoTo && !containsnextrow)
                            functions[currentFunc].actions.Add(new PAction(ActionType.GoTo, ""));
                        index = 0;
                        containsnextrow = false;
                    }
                    index++;
                }
            /*}catch(Exception ex)
            {
                Console.WriteLine(ex);
            }*/
            return functions;
        }
        private static ActionType GetActionType(int id)
        {
            return id switch
            {
                0 => ActionType.None,
                1 => ActionType.NextRow,
                2 => ActionType.Start,
                3 => ActionType.Stop,
                4 => ActionType.MoveUp,
                5 => ActionType.MoveLeft,
                6 => ActionType.MoveDown,
                7 => ActionType.MoveRight,
                8 => ActionType.Dig,
                9 => ActionType.RotateUp,
                10 => ActionType.RotateLeft,
                11 => ActionType.RotateDown,
                12 => ActionType.RotateRight,
                14 => ActionType.MoveForward,
                15 => ActionType.RotateLeftRelative,
                16 => ActionType.RotateRightRelative,
                17 => ActionType.BuildBlock,
                18 => ActionType.Geology,
                19 => ActionType.BuildRoad,
                20 => ActionType.Heal,
                21 => ActionType.BuildPillar,
                22 => ActionType.RotateRandom,
                23 => ActionType.Beep,
                24 => ActionType.GoTo,
                25 => ActionType.RunSub,
                26 => ActionType.RunFunction,
                27 => ActionType.Return,
                28 => ActionType.ReturnFunction,
                29 => ActionType.CheckUpLeft,
                30 => ActionType.CheckDownRight,
                31 => ActionType.CheckUp,
                32 => ActionType.CheckUpRight,
                33 => ActionType.CheckLeft,
                34 => ActionType.None,
                35 => ActionType.CheckRight,
                36 => ActionType.CheckDownLeft,
                37 => ActionType.CheckDown,
                38 => ActionType.Or,
                39 => ActionType.And,
                40 => ActionType.CreateFunction,
                41 => ActionType.None,
                42 => ActionType.None,
                43 => ActionType.IsNotEmpty,
                44 => ActionType.IsEmpty,
                45 => ActionType.IsFalling,
                46 => ActionType.IsCrystal,
                47 => ActionType.IsLivingCrystal,
                48 => ActionType.IsBoulder,
                49 => ActionType.IsSand,
                50 => ActionType.IsBreakableRock,
                51 => ActionType.IsUnbreakable,
                52 => ActionType.IsRedRock,
                53 => ActionType.IsBlackRock,
                54 => ActionType.IsAcid,
                55 => ActionType.None,
                56 => ActionType.None,
                57 => ActionType.IsQuadBlock,
                58 => ActionType.IsRoad,
                59 => ActionType.IsRedBlock,
                60 => ActionType.IsYellowBlock,
                61 => ActionType.None, //хуй знает чет с хп
                62 => ActionType.None, //хуй знает чет с хп
                74 => ActionType.IsBox,
                76 => ActionType.IsPillar,
                77 => ActionType.IsGreenBlock,
                119 => ActionType.WritableStateMore,
                120 => ActionType.WritableStateLower,
                123 => ActionType.WritableState,
                131 => ActionType.ShiftUp,
                132 => ActionType.ShiftLeft,
                133 => ActionType.ShiftDown,
                134 => ActionType.ShiftRight,
                135 => ActionType.CheckForward,
                136 => ActionType.ShiftForward,
                137 => ActionType.RunState,
                138 => ActionType.ReturnState,
                139 => ActionType.RunIfFalse,
                140 => ActionType.RunIfTrue,
                141 => ActionType.MacrosDig,
                142 => ActionType.MacrosBuild,
                143 => ActionType.MacrosHeal,
                144 => ActionType.Flip,
                145 => ActionType.MacrosMine,
                146 => ActionType.CheckGun,
                147 => ActionType.FillGun,
                148 => ActionType.IsHpLower100,
                149 => ActionType.IsHpLower50,
                156 => ActionType.CheckForwardLeft,
                157 => ActionType.CheckForwardRight,
                158 => ActionType.EnableAutoDig,
                159 => ActionType.DisableAutoDig,
                160 => ActionType.EnableAgression,
                161 => ActionType.DisableAgression,
                166 => ActionType.RunOnRespawn,
                _ => ActionType.None
            };
        }
        private Dictionary<string, PFunction> Parse()
        {
            Dictionary<string, PFunction> functions = new();
            functions[""] = new PFunction();
            string currentFunc = "";
            var index = data.IndexOf("$");
            var context = data.Substring(index + 1);
            for (int i = 0; i < context.Length; i++
                )
            {
                int next;
                switch (context[i])
                {
                    case 'w':
                        functions[currentFunc] += new PAction(ActionType.RotateUp);
                        break;
                    case 'a':
                        functions[currentFunc] += new PAction(ActionType.RotateLeft);
                        break;
                    case 's':
                        functions[currentFunc] += new PAction(ActionType.RotateDown);
                        break;
                    case 'd':
                        functions[currentFunc] += new PAction(ActionType.RotateRight);
                        break;
                    case 'z':
                        functions[currentFunc] += new PAction(ActionType.Dig);
                        break;
                    case 'b':
                        functions[currentFunc] += new PAction(ActionType.BuildBlock);
                        break;
                    case 'q':
                        functions[currentFunc] += new PAction(ActionType.BuildPillar);
                        break;
                    case 'r':
                        functions[currentFunc] += new PAction(ActionType.BuildRoad);
                        break;
                    case 'g':
                        functions[currentFunc] += new PAction(ActionType.Geology);
                        break;
                    case 'h':
                        functions[currentFunc] += new PAction(ActionType.Heal);
                        break;
                    case ',':
                        functions[currentFunc] += new PAction(ActionType.NextRow);
                        break;
                    case '?':
                        next = context[(i + 1)..].IndexOf('<');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.RunIfFalse, context[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '(':
                        next = context[(i + 1)..].IndexOf(')');
                        if (next != -1)
                        {
                            next++;
                            var lines = context[i..][1..next].Split('=');
                            if (lines.Length == 2 &&  int.TryParse(lines[1],out var n))
                            {
                                functions[currentFunc] += new PAction(ActionType.WritableState, lines[0],n);
                            }
                            i += next + 1;
                        }
                        break;
                    case '!':
                        i++;
                        if (context[i] == '?')
                        {
                            next = context[(i + 1)..].IndexOf('<');
                            if (next != -1)
                            {
                                next++;
                                functions[currentFunc] += new PAction(ActionType.RunIfTrue, context[i..][1..next]);
                                i += next;
                            }
                        }

                        break;
                    case '[':
                        i++;
                        next = context[i..].IndexOf(']');
                        var option = context[i..][..next];
                        i += next;
                        switch (option)
                        {
                            case "W":
                                functions[currentFunc] += new PAction(ActionType.CheckUp);
                                break;
                            case "A":
                                functions[currentFunc] += new PAction(ActionType.CheckLeft);
                                break;
                            case "S":
                                functions[currentFunc] += new PAction(ActionType.CheckDown);
                                break;
                            case "D":
                                functions[currentFunc] += new PAction(ActionType.CheckRight);
                                break;
                            case "w":
                                functions[currentFunc] += new PAction(ActionType.ShiftUp);
                                break;
                            case "a":
                                functions[currentFunc] += new PAction(ActionType.ShiftLeft);
                                break;
                            case "s":
                                functions[currentFunc] += new PAction(ActionType.ShiftDown);
                                break;
                            case "d":
                                functions[currentFunc] += new PAction(ActionType.ShiftRight);
                                break;
                            case "AS":
                                functions[currentFunc] += new PAction(ActionType.CheckDownLeft);
                                break;
                            case "WA":
                                functions[currentFunc] += new PAction(ActionType.CheckUpLeft);
                                break;
                            case "DW":
                                functions[currentFunc] += new PAction(ActionType.CheckUpRight);
                                break;
                            case "SD":
                                functions[currentFunc] += new PAction(ActionType.CheckDownRight);
                                break;
                            case "F":
                                functions[currentFunc] += new PAction(ActionType.CheckForward);
                                break;
                            case "f":
                                functions[currentFunc] += new PAction(ActionType.ShiftForward);
                                break;
                            case "r":
                                functions[currentFunc] += new PAction(ActionType.CheckRightRelative);
                                break;
                            case "l":
                                functions[currentFunc] += new PAction(ActionType.CheckLeftRelative);
                                break;
                        }

                        break;
                    case '#':
                        i++;
                        switch (context[i])
                        {
                            case 'S':
                                functions[currentFunc] += new PAction(ActionType.Stop);
                                break;
                            case 'E':
                                functions[currentFunc] += new PAction(ActionType.Start);
                                break;
                            case 'R':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunOnRespawn, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (context[i])
                        {
                            case '>':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunSub, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '-':
                        i++;
                        switch (context[i])
                        {
                            case '>':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunFunction, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '=':
                        i++;
                        switch (context[i])
                        {
                            case '>':
                                next = context[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    functions[currentFunc] += new PAction(ActionType.RunState, context[i..][1..next]);
                                    i += next;
                                }

                                break;
                            case 'n':
                                functions[currentFunc] += new PAction(ActionType.IsNotEmpty);
                                break;
                            case 'e':
                                functions[currentFunc] += new PAction(ActionType.IsEmpty);
                                break;
                            case 'f':
                                functions[currentFunc] += new PAction(ActionType.IsFalling);
                                break;
                            case 'c':
                                functions[currentFunc] += new PAction(ActionType.IsCrystal);
                                break;
                            case 'a':
                                functions[currentFunc] += new PAction(ActionType.IsLivingCrystal);
                                break;
                            case 'b':
                                functions[currentFunc] += new PAction(ActionType.IsBoulder);
                                break;
                            case 's':
                                functions[currentFunc] += new PAction(ActionType.IsSand);
                                break;
                            case 'k':
                                functions[currentFunc] += new PAction(ActionType.IsBreakableRock);
                                break;
                            case 'd':
                                functions[currentFunc] += new PAction(ActionType.IsUnbreakable);
                                break;
                            case 'A':
                                functions[currentFunc] += new PAction(ActionType.IsAcid);
                                break;
                            case 'B':
                                functions[currentFunc] += new PAction(ActionType.IsRedRock);
                                break;
                            case 'K':
                                functions[currentFunc] += new PAction(ActionType.IsBlackRock);
                                break;
                            case 'g':
                                functions[currentFunc] += new PAction(ActionType.IsGreenBlock);
                                break;
                            case 'y':
                                functions[currentFunc] += new PAction(ActionType.IsYellowBlock);
                                break;
                            case 'r':
                                functions[currentFunc] += new PAction(ActionType.IsRedRock);
                                break;
                            case 'o':
                                functions[currentFunc] += new PAction(ActionType.IsPillar);
                                break;
                            case 'q':
                                functions[currentFunc] += new PAction(ActionType.IsQuadBlock);
                                break;
                            case 'R':
                                functions[currentFunc] += new PAction(ActionType.IsRoad);
                                break;
                            case 'x':
                                functions[currentFunc] += new PAction(ActionType.IsBox);
                                break;
                        }

                        break;
                    case '>':
                        next = context[(i + 1)..].IndexOf('|');
                        if (next != -1)
                        {
                            next++;
                            functions[currentFunc] += new PAction(ActionType.GoTo, context[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '|':
                        next = context[(i + 1)..].IndexOf(':');
                        if (next != -1)
                        {
                            next++;
                            currentFunc = context[i..][1..next];
                            functions[currentFunc] = new PFunction();
                            i += next;
                        }

                        break;
                    case '<':
                        i++;
                        switch (context[i])
                        {
                            case '|':
                                functions[currentFunc] += new PAction(ActionType.Return);
                                break;
                            case '-':
                                i++;
                                if (context[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnFunction);
                                }

                                break;
                            case '=':
                                i++;
                                if (context[i] == '|')
                                {
                                    functions[currentFunc] += new PAction(ActionType.ReturnState);
                                }

                                break;
                        }

                        break;
                    case '^':
                        i++;
                        switch (context[i])
                        {
                            case 'W':
                                functions[currentFunc] += new PAction(ActionType.MoveUp);
                                break;
                            case 'A':
                                functions[currentFunc] += new PAction(ActionType.MoveLeft);
                                break;
                            case 'S':
                                functions[currentFunc] += new PAction(ActionType.MoveDown);
                                break;
                            case 'D':
                                functions[currentFunc] += new PAction(ActionType.MoveRight);
                                break;
                            case 'F':
                                functions[currentFunc] += new PAction(ActionType.MoveForward);
                                break;
                        }

                        break;
                    default:
                        var currentdata = context[i..];
                        if (currentdata.StartsWith("CCW;"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.RotateLeftRelative);
                        }
                        else if (currentdata.StartsWith("CW;"))
                        {
                            i += 2;
                            functions[currentFunc] += new PAction(ActionType.RotateRightRelative);
                        }
                        else if (currentdata.StartsWith("RAND;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.RotateRandom);
                        }
                        else if (currentdata.StartsWith("VB;"))
                        {
                            i += 2;
                            functions[currentFunc] += new PAction(ActionType.BuildMilitaryBlock);
                        }
                        else if (currentdata.StartsWith("DIGG;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.MacrosDig);
                        }
                        else if (currentdata.StartsWith("BUILD;"))
                        {
                            i += 5;
                            functions[currentFunc] += new PAction(ActionType.MacrosBuild);
                        }
                        else if (currentdata.StartsWith("HEAL;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.MacrosHeal);
                        }
                        else if (currentdata.StartsWith("MINE;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.MacrosMine);
                        }
                        else if (currentdata.StartsWith("FLIP;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.Flip);
                        }
                        else if (currentdata.StartsWith("BEEP;"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.Beep);
                        }
                        else if (currentdata.StartsWith("OR"))
                        {
                            i += 1;
                            functions[currentFunc] += new PAction(ActionType.Or);
                        }
                        else if (currentdata.StartsWith("AND"))
                        {
                            i += 2;
                            functions[currentFunc] += new PAction(ActionType.And);
                        }
                        else if (currentdata.StartsWith("AUT+"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.EnableAutoDig);
                        }
                        else if (currentdata.StartsWith("AUT-"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.DisableAutoDig);
                        }
                        else if (currentdata.StartsWith("ARG+"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.EnableAgression);
                        }
                        else if (currentdata.StartsWith("ARG-"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.DisableAgression);
                        }
                        else if (currentdata.StartsWith("=hp-"))
                        {
                            i += 3;
                            functions[currentFunc] += new PAction(ActionType.IsHpLower100);
                        }
                        else if (currentdata.StartsWith("=hp50"))
                        {
                            i += 4;
                            functions[currentFunc] += new PAction(ActionType.IsHpLower50);
                        }
                        break;
                }
            }
            return functions;
        }
        private Dictionary<string, PFunction> _programm;
    }
}
