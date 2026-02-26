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
        private Dictionary<string, PFunction> _programm;
    }
}
