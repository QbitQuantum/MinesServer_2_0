using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapModel
{
	public int width
	{
		get
		{
			return this._width;
		}
	}
	public int height
	{
		get
		{
			return this._height;
		}
	}
	public void CreateNewMap()
	{
		int index = MapModel._blocksH * MapModel._blocksW;
		this.indexes = new int[index];
		this.backIndex = new int[index];
		for (int i = 0; i < index; i++)
		{
			this.indexes[i] = -1;
			this.backIndex[i] = -1;
		}
		MapModel._indexByteSize = index * 4;
		MapModel._indexSize = 0;
		this._Blocks = new MapBlock[index];
	}

	public MapModel(int width, int height, string name)
	{
		this._width = width;
		this._height = height;
		this.name = name;
		MapModel._blocksW = Mathf.FloorToInt((float)(width / 32));
		MapModel._blocksH = Mathf.FloorToInt((float)(height / 32));
		this.CreateNewMap();
		this.mapFileName = Application.persistentDataPath + "/" + name + "_v2.map";
		this.isNewFile = !File.Exists(this.mapFileName);
		this.fs = File.Open(this.mapFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		try
		{
			if (File.Exists(Application.persistentDataPath + "/saved.map"))
			{
				this.ReopenEmptyFile();
				BinaryReader binaryReader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/saved.map"));
				int num = binaryReader.ReadInt32();
				int num2 = binaryReader.ReadInt32();
				if (width == num && height == num2)
				{
					while (binaryReader.PeekChar() >= 0)
					{
						uint num3 = binaryReader.ReadUInt32();
						if (this._Blocks[(int)num3] == null)
						{
							this._Blocks[(int)num3] = new MapBlock();
							this.indexes[(int)num3] = MapModel._indexSize;
							this.backIndex[MapModel._indexSize] = (int)num3;
							MapModel._indexSize++;
							this.updateIndex = true;
						}
						if (!this._Blocks[(int)num3].isLoaded)
						{
							this._Blocks[(int)num3].Init();
						}
						byte[] array = binaryReader.ReadBytes(4096);
						for (int i = 0; i < 32; i++)
						{
							for (int j = 0; j < 32; j++)
							{
								this._Blocks[(int)num3].data[j + 32 * i] = array[(j + 32 * i) * 4];
							}
						}
					}
				}
				binaryReader.Close();
				if (File.Exists(Application.persistentDataPath + "/saved.map.old"))
				{
					File.Delete(Application.persistentDataPath + "/saved.map.old");
				}
				File.Move(Application.persistentDataPath + "/saved.map", Application.persistentDataPath + "/saved.map.old");
				this.SaveMapV2();
			}
			else
			{
				this.LoadMapV2();
				this.fs.Close();
				if (File.Exists(this.mapFileName + ".bk"))
				{
					File.Delete(this.mapFileName + ".bk");
				}
				File.Copy(this.mapFileName, this.mapFileName + ".bk");
				this.fs = File.Open(this.mapFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
		}
		catch (Exception)
        {
			this.CreateNewMap();
			this.ReopenEmptyFile();
		}
	}

	public int GetCell(int x, int y)
	{
		if (y < 0)
		{
			return 1;
		}
		if (x < 0)
		{
			return 117;
		}
		if (x >= this._width)
		{
			return 117;
		}
		if (y >= this._height)
		{
			return 117;
		}
		int num = (x >> 5) + (y >> 5) * MapModel._blocksW;
		if (this._Blocks[num] == null)
		{
			return 0;
		}
		if (!this._Blocks[num].isLoaded)
		{
			this.blocksToLoad.Add(num);
			this.needToLoad = true;
			return 0;
		}
		return (int)this._Blocks[num].data[x % 32 + 32 * (y % 32)];
	}
	public void SetCell(int x, int y, byte cell)
	{
		if (x < 0)
		{
			return;
		}
		if (x >= this._width)
		{
			return;
		}
		if (y < 0)
		{
			return;
		}
		if (y >= this._height)
		{
			return;
		}
		int num = (x >> 5) + (y >> 5) * MapModel._blocksW;
		if (this._Blocks[num] == null)
		{
			this._Blocks[num] = new MapBlock();
			this.indexes[num] = MapModel._indexSize;
			this.backIndex[MapModel._indexSize] = num;
			MapModel._indexSize++;
			this.updateIndex = true;
		}
		if (!this._Blocks[num].isLoaded)
		{
			this._Blocks[num].Init();
		}
		this._Blocks[num].data[x % 32 + 32 * (y % 32)] = cell;
		this._Blocks[num].notSaved = true;
	}

	public void LoadBlockFromFile(int block)
	{
		if (this._Blocks[block].isLoaded)
		{
			return;
		}
		this._Blocks[block].Init();
		this.fs.Seek((long)(8 + MapModel._indexByteSize + 1024 * this.indexes[block]), SeekOrigin.Begin);
		this.fs.Read(this._Blocks[block].data, 0, 1024);
		this._Blocks[block].notSaved = false;
	}

	public void SaveMapV2()
	{
		if (this.isNewFile)
		{
			this.fs.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[8];
			Buffer.BlockCopy(new int[]
			{
				this.width,
				this.height
			}, 0, array, 0, 8);
			this.fs.Write(array, 0, array.Length);
		}
		if (this.updateIndex)
		{
			this.fs.Seek(8L, SeekOrigin.Begin);
			byte[] array2 = new byte[MapModel._indexByteSize];
			Buffer.BlockCopy(this.indexes, 0, array2, 0, MapModel._indexByteSize);
			this.fs.Write(array2, 0, MapModel._indexByteSize);
		}
		for (int i = 0; i < MapModel._indexSize; i++)
		{
			if (this._Blocks[this.backIndex[i]].notSaved)
			{
				this.fs.Seek((long)(8 + MapModel._indexByteSize + 1024 * i), SeekOrigin.Begin);
				this._Blocks[this.backIndex[i]].notSaved = false;
				this.fs.Write(this._Blocks[this.backIndex[i]].data, 0, 1024);
			}
		}
	}

	public void Destroy()
	{
		if (this.fs != null)
		{
			this.fs.Close();
			this.fs.Dispose();
		}
	}

	public void ReopenEmptyFile()
	{
		this.fs.Close();
		this.fs.Dispose();
		File.Delete(this.mapFileName);
		this.fs = File.Open(this.mapFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		this.isNewFile = true;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00017E24 File Offset: 0x00016024
	public void LoadBlocks()
	{
		if (this.needToLoad)
		{
			foreach (int block in this.blocksToLoad)
			{
				this.LoadBlockFromFile(block);
			}
			this.blocksToLoad.Clear();
			this.needToLoad = false;
		}
	}

	public void LoadMapV2()
	{
		if (this.isNewFile)
		{
			return;
		}
		this.fs.Seek(0L, SeekOrigin.Begin);
		byte[] array = new byte[8];
		int[] array2 = new int[2];
		this.fs.Read(array, 0, array.Length);
		Buffer.BlockCopy(array, 0, array2, 0, 8);
		if (this.width != array2[0] || this.height != array2[1])
		{
			this.ReopenEmptyFile();
			return;
		}
		this.fs.Seek(8L, SeekOrigin.Begin);
		byte[] array3 = new byte[MapModel._indexByteSize];
		this.fs.Read(array3, 0, MapModel._indexByteSize);
		Buffer.BlockCopy(array3, 0, this.indexes, 0, MapModel._indexByteSize);
		MapModel._indexSize = 0;
		for (int i = 0; i < MapModel._blocksW * MapModel._blocksH; i++)
		{
			if (this.indexes[i] >= 0)
			{
				this.backIndex[this.indexes[i]] = i;
			}
			if (this.indexes[i] + 1 > MapModel._indexSize)
			{
				MapModel._indexSize = this.indexes[i] + 1;
			}
		}
		for (int j = 0; j < MapModel._indexSize; j++)
		{
			this._Blocks[this.backIndex[j]] = new MapBlock();
		}
	}

	private int _width;

	private int _height;

	private MapBlock[] _Blocks;

	private int[] indexes;

	private int[] backIndex;

	public static int _blocksW;

	public static int _blocksH;

	public static int _indexByteSize;

	public static int _indexSize;

	private bool updateIndex;

	private bool isNewFile;

	private bool needToLoad;

	private string name;

	private string mapFileName;

	private FileStream fs;

	private HashSet<int> blocksToLoad = new HashSet<int>();
}
