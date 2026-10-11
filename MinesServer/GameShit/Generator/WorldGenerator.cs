namespace MinesServer.GameShit.Generator;

using System;
using System.Collections.Generic;
using MinesServer.GameShit.Enums;

public sealed class WorldGenerator
{
    private readonly int _width;
    private readonly int _height;
    private readonly GenerationParameters _parameters;

    public WorldGenerator(int width, int height, in GenerationParameters parameters)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        _width = width;
        _height = height;
        _parameters = parameters;
    }

    public GenerationResult Generate()
    {
        var cellGenerator = new CellGenerator(_width, _height, _parameters);
        CellType[] cells = cellGenerator.Generate();

        var detector = new SectorDetector(
            _width, _height,
            _parameters.MinSectorSize,
            _parameters.DiagonalConnectivity);
        List<GeneratedSector> sectors = detector.Detect(cells);

        var filler = new SectorFiller(_width, _height, _parameters.Seed, _parameters);
        filler.Fill(cells, sectors);

        return new GenerationResult(cells, sectors, _width, _height);
    }

    public GenerationResult GenerateAndWrite(int chunkWidth, int chunkHeight)
    {
        GenerationResult result = Generate();

        var writer = new WorldWriter(
            _width, _height,
            chunkWidth, chunkHeight);
        writer.Write(result);

        return result;
    }
}