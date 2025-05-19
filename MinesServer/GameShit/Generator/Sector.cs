using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.TypicalEvents;

namespace MinesServer.GameShit.Generator
{
    public class Sector
    {
        private static Random r = new Random();
        public CellType[] GenerateInsides()
        {
            var gig = r.Next(1, 101) >= 80 ? true : false;
            if (types == null)
            {
                crys = depth switch
                {
                    < 500 => gig ? [CellType.Green, CellType.Blue] : [CellType.Green, CellType.Blue],
                    < 1000 => gig ? [CellType.Green, CellType.Blue, CellType.XBlue, CellType.XGreen] : [CellType.Green, CellType.Blue],
                    < 2000 => gig ? [CellType.Blue, CellType.XBlue, CellType.Green, CellType.XGreen] : [CellType.Green, CellType.Blue, CellType.XBlue],
                    < 3000 => gig ? [CellType.Blue, CellType.XBlue, CellType.Red, CellType.XRed] : [CellType.Blue, CellType.XBlue, CellType.Red, CellType.XRed],
                    < 4000 => gig ? [CellType.Blue, CellType.XBlue, CellType.Red, CellType.XRed] : [CellType.Red, CellType.Blue],
                    < 6000 => gig ? [CellType.Red, CellType.Violet, CellType.XRed, CellType.XViolet] : [CellType.Red, CellType.Violet, CellType.XRed],
                    < 7000 => gig ? [CellType.XViolet] : [CellType.XViolet, CellType.Violet],
                    < 8000 => gig ? [CellType.XViolet] : [CellType.Violet, CellType.Violet, CellType.White],
                    < 10000 => gig ? [CellType.XCyan, CellType.Cyan, CellType.White] : [CellType.Cyan, CellType.White, CellType.XCyan],
                    < 11000 => gig ? [CellType.XBlue, CellType.XCyan] : [CellType.Cyan, CellType.XBlue, CellType.XGreen, CellType.XCyan],
                    < 13000 => gig ? [CellType.XGreen, CellType.XBlue, CellType.XViolet] : [CellType.XGreen, CellType.XBlue, CellType.XViolet],
                    < 15000 => gig ? [CellType.XRed, CellType.White] : [CellType.XRed, CellType.White],
                    < 17000 => gig ? [CellType.XGreen, CellType.XCyan] : [CellType.XGreen, CellType.XCyan],
                    < 18000 => gig ? [CellType.XRed, CellType.XViolet, CellType.XCyan] : [CellType.XRed, CellType.XViolet, CellType.XCyan],
                    _ => gig ? [CellType.XGreen, CellType.XBlue, CellType.XRed, CellType.XViolet, CellType.XCyan] : [CellType.XGreen, CellType.XBlue, CellType.XRed, CellType.XViolet, CellType.XCyan]
                };
                types = depth switch
                {
                    < 500 => [CellType.YellowSand, CellType.Rock, CellType.DarkYellowSand, CellType.HeavyRock, CellType.Rock, CellType.Lava],
                    < 1000 => [CellType.Rock, CellType.HeavyRock, CellType.YellowSand, CellType.DarkYellowSand, CellType.Lava, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3],
                    < 2000 => [CellType.Rock, CellType.HeavyRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.BlueSand, CellType.DarkBlueSand, CellType.Lava],
                    < 3000 => [CellType.Rock, CellType.HeavyRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.BlueSand, CellType.DarkBlueSand, CellType.Lava],
                    < 4000 => [CellType.Rock, CellType.HeavyRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.BlueSand, CellType.DarkBlueSand, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.Lava],
                    < 6000 => [CellType.Rock, CellType.HeavyRock, CellType.GoldenRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.BlueSand, CellType.DarkBlueSand, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.Lava],
                    < 7000 => [CellType.Rock, CellType.HeavyRock, CellType.GoldenRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.BlueSand, CellType.DarkBlueSand, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.Lava, CellType.PassiveAcid],
                    < 8000 => [CellType.HeavyRock, CellType.GoldenRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.BlueSand, CellType.DarkBlueSand, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.Lava, CellType.PassiveAcid],
                    < 10000 => [CellType.HeavyRock, CellType.GoldenRock, CellType.Boulder1, CellType.Boulder2, CellType.Boulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.PassiveAcid],
                    < 11000 => [CellType.DeepRock, CellType.GoldenRock, CellType.BlackBoulder1, CellType.BlackBoulder2, CellType.BlackBoulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.PassiveAcid],
                    < 13000 => [CellType.DeepRock, CellType.GoldenRock, CellType.BlackBoulder1, CellType.BlackBoulder2, CellType.BlackBoulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.PassiveAcid],
                    < 15000 => [CellType.DeepRock, CellType.GoldenRock, CellType.BlackBoulder1, CellType.BlackBoulder2, CellType.BlackBoulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid],
                    < 17000 => [CellType.DeepRock, CellType.GRock, CellType.BlackBoulder1, CellType.BlackBoulder2, CellType.BlackBoulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid],
                    < 18000 => [CellType.DeepRock, CellType.GRock, CellType.BlackBoulder1, CellType.BlackBoulder2, CellType.BlackBoulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid],
                    _ => [CellType.GRock, CellType.MetalBoulder1, CellType.MetalBoulder2, CellType.MetalBoulder3, CellType.WhiteSand, CellType.DarkWhiteSand, CellType.RustySand, CellType.DarkRustySand, CellType.GrayAcid, CellType.Pearl],
                };
            }

            var re = new CellType[0];
            if (gig)
            {
                re = re.Concat(crys).ToArray();
                return re;
            }
            var lenm = r.Next(1, types.Length);
            var lencry = r.Next(1, crys.Length);
            for (int i = 0; i < lenm; i++)
            {
                var j = types[r.Next(0, types.Length)];
                if (!re.Contains(j))
                {
                    re = re.Append(j).ToArray();
                    continue;
                }
                i--;
            }
            for (int i = 0; i < lencry; i++)
            {
                var j = crys[r.Next(0, crys.Length)];
                if (!re.Contains(j))
                {
                    re = re.Append(j).ToArray();
                    continue;
                }
                i--;
            }
            return re;
        }


        public List<SectorCell> seccells;
        public List<Cell> cells;
        public int height;
        public int width;
        public int depth;
        public CellType[] crys;
        public CellType[] types;
    }
}
