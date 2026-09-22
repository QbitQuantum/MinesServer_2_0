using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class CellRender
{
	// Token: 0x0600002E RID: 46 RVA: 0x0000995C File Offset: 0x00007B5C
	public static CellRenderConfig DefaultCRC()
	{
		return new CellRenderConfig
		{
			textureType = 0f,
			animType = 0f,
			animSpeed = 50f,
			wx = 30,
			wy = 20,
			dx = 1,
			dy = 1,
			wx2 = 30,
			wy2 = 20,
			dx2 = 1,
			dy2 = 1,
			col = new Color(0.1f, 0f, 0f)
		};
	}

	// Token: 0x0600002F RID: 47 RVA: 0x000099F8 File Offset: 0x00007BF8
	public static void InitCaches()
	{
		CellRender.empty = CellModel.empty;
		CellRender.isSandOrBaseCache = new bool[127];
		CellRender.isSandCache = new bool[127];
		CellRender.isEmptyCache = new bool[127];
		CellRender.receiveShadowCache = new bool[127];
		CellRender.haveShadowCache = new bool[127];
		CellRender.reliefTypeCache = new int[127];
		CellRender.isDistortionCache = new bool[127];
		CellRender.isNoDistortionCache = new bool[127];
		CellRender.UNBREAKABLE = new bool[127];
		CellRender.BZCOST = new int[127];
		CellRender.cellConfigs = new CellRenderConfig[127];
		for (int i = 0; i < 127; i++)
		{
			CellRender.isSandOrBaseCache[i] = false;
			CellRender.isSandCache[i] = false;
			CellRender.receiveShadowCache[i] = false;
			CellRender.haveShadowCache[i] = false;
			CellRender.reliefTypeCache[i] = 0;
			CellRender.isDistortionCache[i] = false;
			CellRender.isNoDistortionCache[i] = false;
			CellRender.cellConfigs[i] = CellRender.DefaultCRC();
			CellRender.UNBREAKABLE[i] = false;
			CellRender.BZCOST[i] = 1;
		}
		CellRender.UNBREAKABLE[0] = true;
		CellRender.UNBREAKABLE[1] = true;
		CellRender.UNBREAKABLE[50] = true;
		CellRender.UNBREAKABLE[51] = true;
		CellRender.UNBREAKABLE[52] = true;
		CellRender.UNBREAKABLE[53] = true;
		CellRender.UNBREAKABLE[54] = true;
		CellRender.UNBREAKABLE[55] = true;
		CellRender.UNBREAKABLE[116] = true;
		CellRender.UNBREAKABLE[38] = true;
		CellRender.UNBREAKABLE[83] = true;
		CellRender.UNBREAKABLE[87] = true;
		CellRender.UNBREAKABLE[88] = true;
		CellRender.UNBREAKABLE[104] = true;
		CellRender.UNBREAKABLE[106] = true;
		CellRender.UNBREAKABLE[114] = true;
		CellRender.UNBREAKABLE[115] = true;
		CellRender.UNBREAKABLE[117] = true;
		CellRender.UNBREAKABLE[119] = true;
		CellRender.BZCOST[40] = 100;
		CellRender.BZCOST[41] = 100;
		CellRender.BZCOST[42] = 100;
		CellRender.BZCOST[43] = 200;
		CellRender.BZCOST[44] = 200;
		CellRender.BZCOST[45] = 200;
		CellRender.BZCOST[48] = 360;
		CellRender.BZCOST[49] = 2;
		CellRender.BZCOST[50] = 1000;
		CellRender.BZCOST[51] = 1000;
		CellRender.BZCOST[52] = 1000;
		CellRender.BZCOST[53] = 1000;
		CellRender.BZCOST[54] = 1000;
		CellRender.BZCOST[55] = 1000;
		CellRender.BZCOST[60] = 10;
		CellRender.BZCOST[61] = 10;
		CellRender.BZCOST[62] = 20;
		CellRender.BZCOST[63] = 20;
		CellRender.BZCOST[64] = 40;
		CellRender.BZCOST[65] = 40;
		CellRender.BZCOST[66] = 800;
		CellRender.BZCOST[67] = 700;
		CellRender.BZCOST[68] = 17;
		CellRender.BZCOST[69] = 2000;
		CellRender.BZCOST[70] = 3000;
		CellRender.BZCOST[71] = 500;
		CellRender.BZCOST[72] = 700;
		CellRender.BZCOST[73] = 100;
		CellRender.BZCOST[74] = 200;
		CellRender.BZCOST[75] = 120;
		CellRender.BZCOST[76] = 1200;
		CellRender.BZCOST[77] = 1200;
		CellRender.BZCOST[80] = 40;
		CellRender.BZCOST[81] = 2000;
		CellRender.BZCOST[82] = 400;
		CellRender.BZCOST[86] = 40;
		CellRender.BZCOST[90] = 1;
		CellRender.BZCOST[91] = 3;
		CellRender.BZCOST[92] = 50;
		CellRender.BZCOST[93] = 50;
		CellRender.BZCOST[94] = 50;
		CellRender.BZCOST[95] = 40;
		CellRender.BZCOST[96] = 40;
		CellRender.BZCOST[97] = 5;
		CellRender.BZCOST[98] = 5;
		CellRender.BZCOST[99] = 3;
		CellRender.BZCOST[100] = 3;
		CellRender.BZCOST[101] = 40;
		CellRender.BZCOST[102] = 50;
		CellRender.BZCOST[103] = 3;
		CellRender.BZCOST[105] = 670;
		CellRender.BZCOST[107] = 10;
		CellRender.BZCOST[108] = 50;
		CellRender.BZCOST[109] = 40;
		CellRender.BZCOST[110] = 40;
		CellRender.BZCOST[111] = 40;
		CellRender.BZCOST[112] = 40;
		CellRender.BZCOST[113] = 15;
		CellRender.BZCOST[116] = 1000;
		CellRender.BZCOST[118] = 25;
		CellRender.BZCOST[120] = 90;
		CellRender.BZCOST[121] = 175;
		CellRender.BZCOST[122] = 3500;
		for (int j = 0; j < CellRender.sands.Length; j++)
		{
			CellRender.isSandOrBaseCache[CellRender.sands[j]] = true;
			CellRender.isSandCache[CellRender.sands[j]] = true;
			CellRender.haveShadowCache[CellRender.sands[j]] = true;
			CellRender.cellConfigs[CellRender.sands[j]].textureType = 16f;
			CellRender.cellConfigs[CellRender.sands[j]].wx2 = 1;
			CellRender.cellConfigs[CellRender.sands[j]].wy2 = 1;
		}
		for (int k = 0; k < CellRender.bolders.Length; k++)
		{
			CellRender.isSandOrBaseCache[CellRender.bolders[k]] = true;
			CellRender.haveShadowCache[CellRender.bolders[k]] = true;
			CellRender.cellConfigs[CellRender.bolders[k]].textureType = 32f;
			CellRender.cellConfigs[CellRender.bolders[k]].wx2 = 1;
			CellRender.cellConfigs[CellRender.bolders[k]].wy2 = 1;
		}
		for (int l = 0; l < CellRender.empty.Length; l++)
		{
			CellRender.receiveShadowCache[CellRender.empty[l]] = true;
			CellRender.isEmptyCache[CellRender.empty[l]] = true;
			CellRender.cellConfigs[CellRender.empty[l]].wx = 30;
			CellRender.cellConfigs[CellRender.empty[l]].wy = 20;
		}
		for (int m = 0; m < CellRender.crysy.Length; m++)
		{
			CellRender.isSandOrBaseCache[CellRender.crysy[m]] = true;
			CellRender.haveShadowCache[CellRender.crysy[m]] = true;
			CellRender.reliefTypeCache[CellRender.crysy[m]] = 1;
			CellRender.isDistortionCache[CellRender.crysy[m]] = true;
			CellRender.cellConfigs[CellRender.crysy[m]].textureType = 1f;
			CellRender.cellConfigs[CellRender.crysy[m]].wx2 = 10;
			CellRender.cellConfigs[CellRender.crysy[m]].wy2 = 8;
			CellRender.cellConfigs[CellRender.crysy[m]].dx2 = 25;
			CellRender.cellConfigs[CellRender.crysy[m]].dy2 = 23;
			CellRender.cellConfigs[CellRender.crysy[m]].wx = 10;
			CellRender.cellConfigs[CellRender.crysy[m]].wy = 8;
			CellRender.cellConfigs[CellRender.crysy[m]].animType = 2f;
		}
		for (int n = 0; n < CellRender.rocky.Length; n++)
		{
			CellRender.isSandOrBaseCache[CellRender.rocky[n]] = true;
			CellRender.haveShadowCache[CellRender.rocky[n]] = true;
			CellRender.reliefTypeCache[CellRender.rocky[n]] = 2;
			CellRender.isDistortionCache[CellRender.rocky[n]] = true;
			CellRender.cellConfigs[CellRender.rocky[n]].textureType = 1f;
			CellRender.cellConfigs[CellRender.rocky[n]].wx = 10;
			CellRender.cellConfigs[CellRender.rocky[n]].wy = 8;
		}
		for (int num = 0; num < CellRender.blocky.Length; num++)
		{
			CellRender.isSandOrBaseCache[CellRender.blocky[num]] = true;
			CellRender.haveShadowCache[CellRender.blocky[num]] = true;
			CellRender.isNoDistortionCache[CellRender.blocky[num]] = true;
			CellRender.reliefTypeCache[CellRender.blocky[num]] = 2;
			CellRender.cellConfigs[CellRender.blocky[num]].textureType = 4f;
			CellRender.cellConfigs[CellRender.blocky[num]].wx = 1;
			CellRender.cellConfigs[CellRender.blocky[num]].wy = 1;
		}
		CellRender.isNoDistortionCache[37] = true;
		CellRender.isNoDistortionCache[38] = true;
		CellRender.isNoDistortionCache[39] = true;
		CellRender.isNoDistortionCache[106] = true;
		CellRender.cellConfigs[0].col = new Color(1f, 1f, 1f);
		CellRender.cellConfigs[0].wx2 = 10;
		CellRender.cellConfigs[0].wy2 = 8;
		CellRender.cellConfigs[0].dx2 = 85;
		CellRender.cellConfigs[0].dy2 = 13;
		CellRender.cellConfigs[0].animType = 2f;
		CellRender.cellConfigs[0].animSpeed = 200f;
		CellRender.cellConfigs[1].textureType = 3f;
		CellRender.cellConfigs[1].col = new Color(0f, 0f, 0f, 0f);
		CellRender.cellConfigs[100].dx2 = 33;
		CellRender.cellConfigs[100].dy2 = 5;
		CellRender.cellConfigs[99].dx2 = 35;
		CellRender.cellConfigs[99].dy2 = 5;
		CellRender.cellConfigs[98].dx2 = 37;
		CellRender.cellConfigs[98].dy2 = 5;
		CellRender.cellConfigs[97].dx2 = 39;
		CellRender.cellConfigs[97].dy2 = 5;
		CellRender.cellConfigs[60].dx2 = 37;
		CellRender.cellConfigs[60].dy2 = 3;
		CellRender.cellConfigs[61].dx2 = 39;
		CellRender.cellConfigs[61].dy2 = 3;
		CellRender.cellConfigs[62].dx2 = 37;
		CellRender.cellConfigs[62].dy2 = 7;
		CellRender.cellConfigs[63].dx2 = 39;
		CellRender.cellConfigs[63].dy2 = 7;
		CellRender.cellConfigs[64].dx2 = 37;
		CellRender.cellConfigs[64].dy2 = 1;
		CellRender.cellConfigs[65].dx2 = 39;
		CellRender.cellConfigs[65].dy2 = 1;
		CellRender.cellConfigs[82].dx2 = 43;
		CellRender.cellConfigs[82].dy2 = 1;
		CellRender.cellConfigs[82].wx2 = 1;
		CellRender.cellConfigs[82].col = new Color(1f, 0f, 0f);
		CellRender.cellConfigs[82].animType = 3f;
		CellRender.cellConfigs[82].animSpeed = 0.5f;
		CellRender.cellConfigs[30].dx2 = 102;
		CellRender.cellConfigs[30].dy2 = 33;
		CellRender.cellConfigs[30].wx2 = 4;
		CellRender.cellConfigs[30].wy2 = 8;
		CellRender.cellConfigs[30].animType = 4f;
		CellRender.cellConfigs[30].animSpeed = 0.7f;
		CellRender.haveShadowCache[30] = false;
		CellRender.cellConfigs[91].dx2 = 43;
		CellRender.cellConfigs[91].dy2 = 5;
		CellRender.cellConfigs[91].wx2 = 1;
		CellRender.cellConfigs[91].col = new Color(1f, 0.5f, 0f);
		CellRender.cellConfigs[91].animType = 3f;
		CellRender.cellConfigs[91].animSpeed = 0.5f;
		CellRender.cellConfigs[86].dx2 = 43;
		CellRender.cellConfigs[86].dy2 = 3;
		CellRender.cellConfigs[86].wx2 = 1;
		CellRender.cellConfigs[86].animType = 3f;
		CellRender.cellConfigs[86].animSpeed = 1f;
		CellRender.cellConfigs[95] = CellRender.cellConfigs[86];
		CellRender.cellConfigs[96] = CellRender.cellConfigs[86];
		CellRender.cellConfigs[66].dx2 = 43;
		CellRender.cellConfigs[66].dy2 = 7;
		CellRender.cellConfigs[66].wx2 = 1;
		CellRender.cellConfigs[66].animType = 3f;
		CellRender.cellConfigs[66].animSpeed = 1f;
		CellRender.cellConfigs[67].dx2 = 43;
		CellRender.cellConfigs[67].dy2 = 9;
		CellRender.cellConfigs[67].wx2 = 1;
		CellRender.cellConfigs[67].animType = 3f;
		CellRender.cellConfigs[67].animSpeed = 1f;
		CellRender.cellConfigs[92].dx2 = 43;
		CellRender.cellConfigs[92].dy2 = 11;
		CellRender.cellConfigs[93].dx2 = 45;
		CellRender.cellConfigs[93].dy2 = 11;
		CellRender.cellConfigs[94].dx2 = 47;
		CellRender.cellConfigs[94].dy2 = 11;
		CellRender.cellConfigs[40].dx2 = 43;
		CellRender.cellConfigs[40].dy2 = 13;
		CellRender.cellConfigs[40].animType = 4f;
		CellRender.cellConfigs[40].animSpeed = 0.3f;
		CellRender.cellConfigs[41].dx2 = 43;
		CellRender.cellConfigs[41].dy2 = 15;
		CellRender.cellConfigs[41].animType = 4f;
		CellRender.cellConfigs[41].animSpeed = 0.3f;
		CellRender.cellConfigs[42].dx2 = 43;
		CellRender.cellConfigs[42].dy2 = 17;
		CellRender.cellConfigs[42].animType = 4f;
		CellRender.cellConfigs[42].animSpeed = 0.3f;
		CellRender.cellConfigs[43].dx2 = 51;
		CellRender.cellConfigs[43].dy2 = 1;
		CellRender.cellConfigs[43].animType = 4f;
		CellRender.cellConfigs[43].animSpeed = 0.3f;
		CellRender.cellConfigs[44].dx2 = 51;
		CellRender.cellConfigs[44].dy2 = 3;
		CellRender.cellConfigs[44].animType = 4f;
		CellRender.cellConfigs[44].animSpeed = 0.3f;
		CellRender.cellConfigs[45].dx2 = 51;
		CellRender.cellConfigs[45].dy2 = 5;
		CellRender.cellConfigs[45].animType = 4f;
		CellRender.cellConfigs[45].animSpeed = 0.3f;
		CellRender.cellConfigs[70].dx2 = 51;
		CellRender.cellConfigs[70].dy2 = 9;
		CellRender.cellConfigs[70].animType = 4f;
		CellRender.cellConfigs[70].animSpeed = 0.3f;
		CellRender.cellConfigs[68].dx2 = 34;
		CellRender.cellConfigs[68].dy2 = 10;
		CellRender.cellConfigs[68].wx2 = 6;
		CellRender.cellConfigs[68].wy2 = 4;
		CellRender.cellConfigs[68].animType = 2f;
		CellRender.cellConfigs[68].col = new Color(0.5f, 0.4f, 0.5f);
		CellRender.cellConfigs[68].animSpeed = 100f;
		CellRender.cellConfigs[69].dx2 = 51;
		CellRender.cellConfigs[69].dy2 = 7;
		CellRender.cellConfigs[69].wx2 = 1;
		CellRender.cellConfigs[69].col = new Color(1f, 0.5f, 1f);
		CellRender.cellConfigs[69].animType = 3f;
		CellRender.cellConfigs[69].animSpeed = 0.5f;
		CellRender.cellConfigs[107].dx = 85;
		CellRender.cellConfigs[107].dy = 23;
		CellRender.cellConfigs[108].dx = 14;
		CellRender.cellConfigs[108].dy = 23;
		CellRender.cellConfigs[108].col = new Color(1f, 0f, 0f);
		CellRender.cellConfigs[109].dx = 97;
		CellRender.cellConfigs[109].dy = 23;
		CellRender.cellConfigs[103].dx = 1;
		CellRender.cellConfigs[103].dy = 23;
		CellRender.cellConfigs[113].dx = 37;
		CellRender.cellConfigs[113].dy = 23;
		CellRender.cellConfigs[114].dx = 33;
		CellRender.cellConfigs[114].dy = 59;
		CellRender.cellConfigs[115].dx = 33;
		CellRender.cellConfigs[115].dy = 59;
		CellRender.cellConfigs[120].dx = 49;
		CellRender.cellConfigs[120].dy = 23;
		CellRender.cellConfigs[121].dx = 61;
		CellRender.cellConfigs[121].dy = 23;
		CellRender.cellConfigs[122].dx = 73;
		CellRender.cellConfigs[122].dy = 23;
		CellRender.cellConfigs[122].wx2 = 6;
		CellRender.cellConfigs[122].wy2 = 4;
		CellRender.cellConfigs[122].dx2 = 34;
		CellRender.cellConfigs[122].dy2 = 16;
		CellRender.cellConfigs[122].animType = 2f;
		CellRender.cellConfigs[122].animSpeed = 25f;
		CellRender.cellConfigs[76].dx = 79;
		CellRender.cellConfigs[76].dy = 33;
		CellRender.cellConfigs[76].wx2 = 6;
		CellRender.cellConfigs[76].wy2 = 4;
		CellRender.cellConfigs[76].dx2 = 34;
		CellRender.cellConfigs[76].dy2 = 16;
		CellRender.cellConfigs[76].animType = 2f;
		CellRender.cellConfigs[76].animSpeed = 25f;
		CellRender.cellConfigs[77].dx = 91;
		CellRender.cellConfigs[77].dy = 33;
		CellRender.cellConfigs[77].wx2 = 6;
		CellRender.cellConfigs[77].wy2 = 4;
		CellRender.cellConfigs[77].dx2 = 34;
		CellRender.cellConfigs[77].dy2 = 16;
		CellRender.cellConfigs[77].animType = 2f;
		CellRender.cellConfigs[77].animSpeed = 25f;
		CellRender.cellConfigs[78].dx = 25;
		CellRender.cellConfigs[78].dy = 23;
		CellRender.cellConfigs[78].wx2 = 6;
		CellRender.cellConfigs[78].wy2 = 4;
		CellRender.cellConfigs[78].dx2 = 25;
		CellRender.cellConfigs[78].dy2 = 23;
		CellRender.cellConfigs[78].animType = 2f;
		CellRender.cellConfigs[78].animSpeed = 25f;
		CellRender.cellConfigs[79].dx = 97;
		CellRender.cellConfigs[79].dy = 13;
		CellRender.cellConfigs[79].wx2 = 6;
		CellRender.cellConfigs[79].wy2 = 4;
		CellRender.cellConfigs[79].dx2 = 25;
		CellRender.cellConfigs[79].dy2 = 23;
		CellRender.cellConfigs[79].animType = 7f;
		CellRender.cellConfigs[79].animSpeed = 10f;
		CellRender.cellConfigs[116].dx = 57;
		CellRender.cellConfigs[116].dy = 43;
		CellRender.cellConfigs[116].wx2 = 6;
		CellRender.cellConfigs[116].wy2 = 4;
		CellRender.cellConfigs[116].dx2 = 34;
		CellRender.cellConfigs[116].dy2 = 16;
		CellRender.cellConfigs[116].animType = 2f;
		CellRender.cellConfigs[116].animSpeed = 195f;
		CellRender.cellConfigs[116].col = new Color(0f, 0f, 1f);
		CellRender.cellConfigs[110].dx = 109;
		CellRender.cellConfigs[110].dy = 23;
		CellRender.cellConfigs[110].wx = 8;
		CellRender.cellConfigs[110].wy = 6;
		CellRender.cellConfigs[110].col = new Color(1f, 0.5f, 1f);
		CellRender.cellConfigs[111].dx = 61;
		CellRender.cellConfigs[111].dy = 15;
		CellRender.cellConfigs[111].wx = 10;
		CellRender.cellConfigs[111].wy = 5;
		CellRender.cellConfigs[111].col = new Color(0.9f, 0.9f, 0.9f);
		CellRender.cellConfigs[112].dx = 73;
		CellRender.cellConfigs[112].dy = 14;
		CellRender.cellConfigs[112].wx = 10;
		CellRender.cellConfigs[112].wy = 6;
		CellRender.cellConfigs[112].col = new Color(0f, 1f, 1f);
		CellRender.cellConfigs[71].dx = 80;
		CellRender.cellConfigs[71].dy = 6;
		CellRender.cellConfigs[71].wx = 5;
		CellRender.cellConfigs[71].wy = 5;
		CellRender.cellConfigs[71].animType = 5f;
		CellRender.cellConfigs[71].animSpeed = 50f;
		CellRender.cellConfigs[71].col = new Color(0.2f, 1f, 0.2f);
		CellRender.cellConfigs[72].dx = 87;
		CellRender.cellConfigs[72].dy = 6;
		CellRender.cellConfigs[72].wx = 5;
		CellRender.cellConfigs[72].wy = 5;
		CellRender.cellConfigs[72].animType = 5f;
		CellRender.cellConfigs[72].animSpeed = 50f;
		CellRender.cellConfigs[72].col = new Color(0.2f, 0.2f, 1f);
		CellRender.cellConfigs[73].dx = 94;
		CellRender.cellConfigs[73].dy = 6;
		CellRender.cellConfigs[73].wx = 5;
		CellRender.cellConfigs[73].wy = 5;
		CellRender.cellConfigs[73].animType = 5f;
		CellRender.cellConfigs[73].animSpeed = 50f;
		CellRender.cellConfigs[73].col = new Color(1f, 1f, 1f);
		CellRender.cellConfigs[75].dx = 101;
		CellRender.cellConfigs[75].dy = 6;
		CellRender.cellConfigs[75].wx = 5;
		CellRender.cellConfigs[75].wy = 5;
		CellRender.cellConfigs[75].animType = 5f;
		CellRender.cellConfigs[75].animSpeed = 50f;
		CellRender.cellConfigs[75].col = new Color(0.1f, 1f, 1f);
		CellRender.cellConfigs[74].dx = 108;
		CellRender.cellConfigs[74].dy = 6;
		CellRender.cellConfigs[74].wx = 5;
		CellRender.cellConfigs[74].wy = 5;
		CellRender.cellConfigs[74].animType = 5f;
		CellRender.cellConfigs[74].animSpeed = 50f;
		CellRender.cellConfigs[74].col = new Color(1f, 0f, 0f);
		CellRender.cellConfigs[117].dx = 33;
		CellRender.cellConfigs[117].dy = 33;
		CellRender.cellConfigs[117].animType = 6f;
		CellRender.cellConfigs[117].dx2 = 8;
		CellRender.cellConfigs[117].dy2 = 3;
		CellRender.cellConfigs[117].animSpeed = 0.2f;
		CellRender.cellConfigs[118].dx = 45;
		CellRender.cellConfigs[118].dy = 33;
		CellRender.cellConfigs[118].animType = 6f;
		CellRender.cellConfigs[118].dx2 = 8;
		CellRender.cellConfigs[118].dy2 = 4;
		CellRender.cellConfigs[118].animSpeed = 0.4f;
		CellRender.cellConfigs[119].dx = 57;
		CellRender.cellConfigs[119].dy = 33;
		CellRender.cellConfigs[119].animType = 7f;
		CellRender.cellConfigs[119].animSpeed = 15f;
		CellRender.cellConfigs[31].dx = 1;
		CellRender.cellConfigs[31].dy = 1;
		CellRender.cellConfigs[32].dx = 1;
		CellRender.cellConfigs[32].dy = 1;
		CellRender.cellConfigs[33].dx = 1;
		CellRender.cellConfigs[33].dy = 78;
		CellRender.cellConfigs[34].dx = 1;
		CellRender.cellConfigs[34].dy = 100;
		CellRender.cellConfigs[35].dx = 1;
		CellRender.cellConfigs[35].dy = 33;
		CellRender.cellConfigs[36].dx = 1;
		CellRender.cellConfigs[36].dy = 55;
		CellRender.cellConfigs[50].dx = 69;
		CellRender.cellConfigs[50].dy = 33;
		CellRender.cellConfigs[50].wx = 2;
		CellRender.cellConfigs[50].wy = 2;
		CellRender.cellConfigs[50].animType = 8f;
		CellRender.cellConfigs[50].dx2 = 2;
		CellRender.cellConfigs[50].dy2 = 4;
		CellRender.cellConfigs[50].animSpeed = 0.6f;
		CellRender.cellConfigs[51].dx = 69;
		CellRender.cellConfigs[51].dy = 37;
		CellRender.cellConfigs[51].wx = 2;
		CellRender.cellConfigs[51].wy = 2;
		CellRender.cellConfigs[51].animType = 8f;
		CellRender.cellConfigs[51].dx2 = 2;
		CellRender.cellConfigs[51].dy2 = 4;
		CellRender.cellConfigs[51].animSpeed = 0.6f;
		CellRender.cellConfigs[52].dx = 69;
		CellRender.cellConfigs[52].dy = 41;
		CellRender.cellConfigs[52].wx = 2;
		CellRender.cellConfigs[52].wy = 2;
		CellRender.cellConfigs[52].animType = 8f;
		CellRender.cellConfigs[52].dx2 = 2;
		CellRender.cellConfigs[52].dy2 = 4;
		CellRender.cellConfigs[52].animSpeed = 0.6f;
		CellRender.cellConfigs[53].dx = 69;
		CellRender.cellConfigs[53].dy = 49;
		CellRender.cellConfigs[53].wx = 3;
		CellRender.cellConfigs[53].wy = 3;
		CellRender.cellConfigs[53].animType = 8f;
		CellRender.cellConfigs[53].dx2 = 2;
		CellRender.cellConfigs[53].dy2 = 4;
		CellRender.cellConfigs[53].animSpeed = 0.5f;
		CellRender.cellConfigs[54].dx = 69;
		CellRender.cellConfigs[54].dy = 45;
		CellRender.cellConfigs[54].wx = 2;
		CellRender.cellConfigs[54].wy = 2;
		CellRender.cellConfigs[54].animType = 8f;
		CellRender.cellConfigs[54].dx2 = 2;
		CellRender.cellConfigs[54].dy2 = 4;
		CellRender.cellConfigs[54].animSpeed = 0.3f;
		CellRender.cellConfigs[55].dx = 14;
		CellRender.cellConfigs[55].dy = 23;
		CellRender.cellConfigs[55].animType = 7f;
		CellRender.cellConfigs[55].animSpeed = 15f;
		CellRender.cellConfigs[101].dx = 65;
		CellRender.cellConfigs[101].dy = 2;
		CellRender.cellConfigs[102].dx = 68;
		CellRender.cellConfigs[102].dy = 2;
		CellRender.cellConfigs[105].dx = 71;
		CellRender.cellConfigs[105].dy = 2;
		CellRender.cellConfigs[39].dx2 = 86;
		CellRender.cellConfigs[39].dy2 = 2;
		CellRender.cellConfigs[39].textureType = 32f;
		CellRender.cellConfigs[80].dx2 = 80;
		CellRender.cellConfigs[80].dy2 = 2;
		CellRender.cellConfigs[80].textureType = 32f;
		CellRender.cellConfigs[81].dx = 74;
		CellRender.cellConfigs[81].dy = 2;
		CellRender.cellConfigs[81].animType = 8f;
		CellRender.cellConfigs[81].dx2 = 1;
		CellRender.cellConfigs[81].dy2 = 4;
		CellRender.cellConfigs[81].wx = 1;
		CellRender.cellConfigs[81].wy = 1;
		CellRender.cellConfigs[81].animSpeed = 0.2f;
		CellRender.cellConfigs[83].textureType = 4f;
		CellRender.cellConfigs[83].wx = 1;
		CellRender.cellConfigs[83].wy = 1;
		CellRender.cellConfigs[83].dx = 68;
		CellRender.cellConfigs[83].dy = 8;
		CellRender.cellConfigs[83].animType = 7f;
		CellRender.cellConfigs[83].animSpeed = 15f;
		CellRender.cellConfigs[48].dx = 65;
		CellRender.cellConfigs[48].dy = 5;
		CellRender.cellConfigs[49].dx = 68;
		CellRender.cellConfigs[49].dy = 5;
		CellRender.cellConfigs[104].dx = 65;
		CellRender.cellConfigs[104].dy = 8;
		CellRender.cellConfigs[90].dx2 = 60;
		CellRender.cellConfigs[90].dy2 = 11;
		CellRender.cellConfigs[90].animType = 4f;
		CellRender.cellConfigs[90].animSpeed = 0.3f;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00004C9B File Offset: 0x00002E9B
	public static bool isSandOrBase(int cell)
	{
		return CellRender.isSandOrBaseCache[cell];
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00004CA4 File Offset: 0x00002EA4
	public static bool isSand(int cell)
	{
		return CellRender.isSandCache[cell];
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00004CAD File Offset: 0x00002EAD
	public static bool receiveShadow(int cell)
	{
		return CellRender.receiveShadowCache[cell];
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00004CB6 File Offset: 0x00002EB6
	public static bool haveShadow(int cell)
	{
		return CellRender.haveShadowCache[cell];
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00004CBF File Offset: 0x00002EBF
	public static int reliefType(int cell)
	{
		return CellRender.reliefTypeCache[cell];
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00004CC8 File Offset: 0x00002EC8
	public static bool isDistortion(int cell)
	{
		return CellRender.isDistortionCache[cell];
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00004CD1 File Offset: 0x00002ED1
	public static bool isNoDistortion(int cell)
	{
		return CellRender.isNoDistortionCache[cell];
	}

	// Token: 0x0400002A RID: 42
	public static bool[] isSandOrBaseCache;

	// Token: 0x0400002B RID: 43
	public static bool[] isEmptyCache;

	// Token: 0x0400002C RID: 44
	public static bool[] isSandCache;

	// Token: 0x0400002D RID: 45
	public static bool[] receiveShadowCache;

	// Token: 0x0400002E RID: 46
	public static bool[] haveShadowCache;

	// Token: 0x0400002F RID: 47
	public static int[] reliefTypeCache;

	// Token: 0x04000030 RID: 48
	public static bool[] isDistortionCache;

	// Token: 0x04000031 RID: 49
	public static bool[] isNoDistortionCache;

	// Token: 0x04000032 RID: 50
	public static bool[] UNBREAKABLE;

	// Token: 0x04000033 RID: 51
	public static int[] BZCOST;

	// Token: 0x04000034 RID: 52
	public static int[] sands = new int[]
	{
		82,
		91,
		97,
		98,
		99,
		100,
		86,
		66,
		67,
		95,
		96,
		60,
		61,
		62,
		63,
		64,
		65,
		68,
		69
	};

	// Token: 0x04000035 RID: 53
	public static int[] bolders = new int[]
	{
		30,
		92,
		93,
		94,
		40,
		41,
		42,
		43,
		44,
		45,
		90,
		80,
		39,
		70
	};

	// Token: 0x04000036 RID: 54
	public static int[] empty;

	// Token: 0x04000037 RID: 55
	public static int[] crysy = new int[]
	{
		71,
		72,
		73,
		74,
		75,
		76,
		77,
		78,
		79,
		50,
		51,
		52,
		53,
		54,
		55,
		56,
		57,
		58,
		59,
		108,
		110,
		111,
		112
	};

	// Token: 0x04000038 RID: 56
	public static int[] rocky = new int[]
	{
		107,
		109,
		103,
		113,
		114,
		115,
		116,
		117,
		118,
		119,
		120,
		121,
		122
	};

	// Token: 0x04000039 RID: 57
	public static int[] blocky = new int[]
	{
		48,
		49,
		80,
		81,
		83,
		87,
		88,
		101,
		102,
		104,
		105,
		106
	};

	// Token: 0x0400003A RID: 58
	public static CellRenderConfig[] cellConfigs;
}
