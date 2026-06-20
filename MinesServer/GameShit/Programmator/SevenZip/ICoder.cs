namespace MinesServer.GameShit.Programmator.SevenZip
{
    public interface ICoder
    {
        // Token: 0x060005A6 RID: 1446
        void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
    }
}
