using System.Text;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Programmator.SevenZip.LZMA;

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

        private static (byte[] DecompressedData, int NumBit, string[] ArrayStrings) Decode(string data) 
        {
            byte[] DecompressedData = SevenZipHelper.Decompress(Convert.FromBase64String(data));
            int NumBit = BitConverter.ToInt32(DecompressedData, 0);
            string[] ArrayStrings = Encoding.UTF8.GetString(DecompressedData, NumBit + 4, DecompressedData.Length - NumBit - 4).Split(':');
            return (DecompressedData, NumBit, ArrayStrings);
        }

        private Dictionary<string,PFunction> parseNormal()
        {
            Dictionary<string, PFunction> functions = [];
            functions[""] = new PFunction();
            string currentFunc = "";
            var (DecompressedData, NumBit, ArrayStrings) = Decode(data);
            int index = 0;
            for (int i = 0; i < NumBit; i++)
            {
                var atype = GetActionType(Convert.ToInt16(DecompressedData[i + 4]));

                var name = "0";
                var number = 0;
                if (ArrayStrings.Length > i)
                {
                    if (ArrayStrings[i].Contains('@'))
                    {
                        var Label = ArrayStrings[i].Split('@');
                        name = Label[0];
                        if (int.TryParse(Label[1], out var n))
                            number = n;
                    }
                    else name = ArrayStrings[i];
                }

                // Добавляем команду в текущую функцию
                switch (atype)
                {
                    case ActionType.NextRow:
                        // Сбрасываем счетчик строки
                        index = 0;
                        continue;

                    // Управление потоком
                    case ActionType.CreateFunction:
                        functions.Add(name, new PFunction());
                        currentFunc = name;
                        index = 0;
                        break;

                    // Команды проверки состояния (без параметров)
                    case ActionType.IsNotEmpty:
                    case ActionType.IsEmpty:
                    case ActionType.IsFalling:
                    case ActionType.IsCrystal:
                    case ActionType.IsLivingCrystal:
                    case ActionType.IsBoulder:
                    case ActionType.IsSand:
                    case ActionType.IsBreakableRock:
                    case ActionType.IsUnbreakable:
                    case ActionType.IsRedRock:
                    case ActionType.IsBlackRock:
                    case ActionType.IsAcid:
                    case ActionType.IsQuadBlock:
                    case ActionType.IsRoad:
                    case ActionType.IsRedBlock:
                    case ActionType.IsYellowBlock:
                    case ActionType.IsBox:
                    case ActionType.IsPillar:
                    case ActionType.IsGreenBlock:
                    case ActionType.CheckGun:

                    // Команды перемещения и вращения
                    case ActionType.MoveUp:
                    case ActionType.MoveLeft:
                    case ActionType.MoveDown:
                    case ActionType.MoveRight:
                    case ActionType.MoveForward:
                    case ActionType.RotateUp:
                    case ActionType.RotateLeft:
                    case ActionType.RotateDown:
                    case ActionType.RotateRight:
                    case ActionType.RotateLeftRelative:
                    case ActionType.RotateRightRelative:
                    case ActionType.RotateRandom:

                    // Команды проверки направления
                    case ActionType.CheckUp:
                    case ActionType.CheckLeft:
                    case ActionType.CheckDown:
                    case ActionType.CheckRight:
                    case ActionType.CheckForward:
                    case ActionType.CheckUpLeft:
                    case ActionType.CheckUpRight:
                    case ActionType.CheckDownLeft:
                    case ActionType.CheckDownRight:
                    case ActionType.CheckForwardLeft:
                    case ActionType.CheckForwardRight:

                    // Команды сдвига
                    case ActionType.ShiftUp:
                    case ActionType.ShiftLeft:
                    case ActionType.ShiftDown:
                    case ActionType.ShiftRight:
                    case ActionType.ShiftForward:

                    // Логические операторы
                    case ActionType.Or:
                    case ActionType.And:

                    // Действия
                    case ActionType.Dig:
                    case ActionType.BuildBlock:
                    case ActionType.Geology:
                    case ActionType.BuildRoad:
                    case ActionType.Heal:
                    case ActionType.BuildPillar:
                    case ActionType.Beep:

                    // Макросы
                    case ActionType.MacrosDig:
                    case ActionType.MacrosBuild:
                    case ActionType.MacrosHeal:
                    case ActionType.MacrosMine:

                    // Специальные команды
                    case ActionType.Flip:
                    case ActionType.FillGun:

                    // Режимы
                    case ActionType.EnableAutoDig:
                    case ActionType.DisableAutoDig:
                    case ActionType.EnableAgression:
                    case ActionType.DisableAgression:
                    case ActionType.EnableHandMode:
                    case ActionType.DisableHandMode:

                    // Специальные действия
                    case ActionType.BOOM:
                    case ActionType.DISCHARGE:
                    case ActionType.PROTON:
                    case ActionType.VB:
                    case ActionType.Geopack:
                    case ActionType.ZZ:
                    case ActionType.C190:
                    case ActionType.Poly:
                    case ActionType.Up:
                    case ActionType.Craft:
                    case ActionType.Nano:
                    case ActionType.Rembot:
                    case ActionType.InvDirUp:
                    case ActionType.InvDirLeft:
                    case ActionType.InvDirDown:
                    case ActionType.InvDirRight:

                    // Старт/Стоп
                    case ActionType.Start:
                    case ActionType.Stop:
                    case ActionType.Last:
                        functions[currentFunc].AddAction(new PAction(atype));
                        break;

                    case ActionType.GoTo:
                    case ActionType.RunSub:
                    case ActionType.RunFunction:
                    case ActionType.RunState:
                    case ActionType.RunIfFalse:
                    case ActionType.RunIfTrue:
                    case ActionType.RunOnRespawn:
                    case ActionType.Return:
                    case ActionType.ReturnFunction:
                    case ActionType.ReturnState:
                    case ActionType.Restart:

                    // Отладка
                    case ActionType.DebugBreak:
                    case ActionType.DebugSet:
                        functions[currentFunc].AddAction(new PAction(atype, name));
                        break;

                    // Команды с числовым параметром
                    case ActionType.IsHpLower100:
                    case ActionType.IsHpLower50:
                        functions[currentFunc].AddAction(new PAction(atype, number));
                        break;

                    // Команды с меткой и числовым параметром
                    case ActionType.WritableState:
                    case ActionType.WritableStateLower:
                    case ActionType.WritableStateMore:
                        functions[currentFunc].AddAction(new PAction(atype, name, number));
                        break;

                    // None или неизвестные команды
                    case ActionType.None:
                    default:
                        if (atype != ActionType.None)
                        {
                            Console.WriteLine($"Unknown action ID: {Convert.ToInt16(DecompressedData[i + 4])}");
                            functions[currentFunc].AddAction(new PAction(atype));
                        }
                        break;
                }

                index++;

                // Проверяем, нужно ли обработать конец строки
                if (index >= 15) index = 0;
            }
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
                13 => ActionType.Last,                    // LAST
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
                35 => ActionType.CheckRight,
                36 => ActionType.CheckDownLeft,
                37 => ActionType.CheckDown,
                38 => ActionType.Or,
                39 => ActionType.And,
                40 => ActionType.CreateFunction,
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
                57 => ActionType.IsQuadBlock,
                58 => ActionType.IsRoad,
                59 => ActionType.IsRedBlock,
                60 => ActionType.IsYellowBlock,
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
                162 => ActionType.BOOM,                    // ACTION_BOOM
                163 => ActionType.DISCHARGE,               // ACTION_DISCHARGE
                164 => ActionType.PROTON,                  // ACTION_PROTON
                165 => ActionType.VB,                      // ACTION_WB
                166 => ActionType.RunOnRespawn,
                167 => ActionType.Geopack,                  // ACTION_GEOPACK
                168 => ActionType.ZZ,                       // ACTION_ZZ
                169 => ActionType.C190,                     // ACTION_C190
                170 => ActionType.Poly,                      // ACTION_POLY
                171 => ActionType.Up,                        // ACTION_UP
                172 => ActionType.Craft,                     // ACTION_CRAFT
                173 => ActionType.Nano,                      // ACTION_NANO
                174 => ActionType.Rembot,                    // ACTION_REMBOT
                175 => ActionType.InvDirUp,                  // INVDIR_W
                176 => ActionType.InvDirLeft,                // INVDIR_A
                177 => ActionType.InvDirDown,                // INVDIR_S
                178 => ActionType.InvDirRight,               // INVDIR_D
                179 => ActionType.EnableHandMode,            // HANDMODE_ON
                180 => ActionType.DisableHandMode,           // HANDMODE_OFF
                181 => ActionType.DebugBreak,                // DEBUG_BREAK
                182 => ActionType.DebugSet,                  // DEBUG_SET
                200 => ActionType.Restart,                    // RESTART
                _ => ActionType.None
            };
        }
        private Dictionary<string, PFunction> _programm;
    }
}
