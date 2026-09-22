using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;

// Token: 0x02000062 RID: 98
public class SkillButtonScript : MonoBehaviour
{
	// Token: 0x06000285 RID: 645 RVA: 0x00027758 File Offset: 0x00025958
	public static void InitColors()
	{
		if (!SkillButtonScript.inited)
		{
			SkillButtonScript.sprites = ResourcesManager.LoadAll<Sprite>("skills");
			SkillButtonScript.inited = true;
		}
		SkillButtonScript.colors[0] = SkillButtonScript.SKILL_COLOR_PINK;
		SkillButtonScript.colors[10] = SkillButtonScript.SKILL_COLOR_PINK;
		SkillButtonScript.colors[13] = SkillButtonScript.SKILL_COLOR_PINK;
		SkillButtonScript.colors[55] = SkillButtonScript.SKILL_COLOR_CYAN;
		SkillButtonScript.colors[16] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[11] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[20] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[17] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[47] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[48] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[50] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[49] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[51] = SkillButtonScript.SKILL_COLOR_GREY;
		SkillButtonScript.colors[3] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[33] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[42] = SkillButtonScript.SKILL_COLOR_RED;
		SkillButtonScript.colors[19] = SkillButtonScript.SKILL_COLOR_CYAN;
		SkillButtonScript.colors[24] = SkillButtonScript.SKILL_COLOR_CYAN;
		SkillButtonScript.colors[25] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[29] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[40] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[38] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[30] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[32] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[39] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[31] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[21] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[27] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[46] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[58] = SkillButtonScript.SKILL_COLOR_GREEN;
		SkillButtonScript.colors[22] = SkillButtonScript.SKILL_COLOR_BLUE;
		SkillButtonScript.colors[35] = SkillButtonScript.SKILL_COLOR_BLUE;
		SkillButtonScript.colors[14] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[15] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[4] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[28] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[23] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[5] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[37] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[2] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[34] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[44] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[41] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[18] = SkillButtonScript.SKILL_COLOR_ORANGE;
		SkillButtonScript.colors[56] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[57] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[43] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[54] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[53] = SkillButtonScript.SKILL_COLOR_WHITE;
		SkillButtonScript.colors[12] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[26] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[9] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[8] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[36] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[1] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[7] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[6] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[45] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.colors[52] = SkillButtonScript.SKILL_COLOR_YELLOW;
		SkillButtonScript.skillShorts["d"] = 12;
		SkillButtonScript.skillShorts["L"] = 16;
		SkillButtonScript.skillShorts["F"] = 22;
		SkillButtonScript.skillShorts["M"] = 19;
		SkillButtonScript.skillShorts["t"] = 24;
		SkillButtonScript.skillShorts["p"] = 29;
		SkillButtonScript.skillShorts["l"] = 13;
		SkillButtonScript.skillShorts["b"] = 30;
		SkillButtonScript.skillShorts["c"] = 31;
		SkillButtonScript.skillShorts["g"] = 40;
		SkillButtonScript.skillShorts["r"] = 38;
		SkillButtonScript.skillShorts["v"] = 32;
		SkillButtonScript.skillShorts["w"] = 39;
		SkillButtonScript.skillShorts["m"] = 14;
		SkillButtonScript.skillShorts["G"] = 5;
		SkillButtonScript.skillShorts["B"] = 4;
		SkillButtonScript.skillShorts["R"] = 15;
		SkillButtonScript.skillShorts["W"] = 37;
		SkillButtonScript.skillShorts["V"] = 28;
		SkillButtonScript.skillShorts["C"] = 23;
		SkillButtonScript.skillShorts["Y"] = 20;
		SkillButtonScript.skillShorts["E"] = 11;
		SkillButtonScript.skillShorts["O"] = 47;
		SkillButtonScript.skillShorts["Q"] = 17;
		SkillButtonScript.skillShorts["A"] = 48;
		SkillButtonScript.skillShorts["a"] = 0;
		SkillButtonScript.skillShorts["k"] = 1;
		SkillButtonScript.skillShorts["z"] = 9;
		SkillButtonScript.skillShorts["o"] = 41;
		SkillButtonScript.skillShorts["q"] = 18;
		SkillButtonScript.skillShorts["j"] = 2;
		SkillButtonScript.skillShorts["u"] = 10;
		SkillButtonScript.skillShorts["x"] = 7;
		SkillButtonScript.skillShorts["P"] = 21;
		SkillButtonScript.skillShorts["h"] = 27;
		SkillButtonScript.skillShorts["H"] = 46;
		SkillButtonScript.skillShorts["y"] = 8;
		SkillButtonScript.skillShorts["Z"] = 26;
		SkillButtonScript.skillShorts["D"] = 6;
		SkillButtonScript.skillShorts["f"] = 45;
		SkillButtonScript.skillShorts["U"] = 3;
		SkillButtonScript.skillShorts["S"] = 35;
		SkillButtonScript.skillShorts["X"] = 36;
		SkillButtonScript.skillShorts["e"] = 42;
		SkillButtonScript.skillShorts["J"] = 34;
		SkillButtonScript.skillShorts["i"] = 44;
		SkillButtonScript.skillShorts["*U"] = 25;
		SkillButtonScript.skillShorts["*M"] = 33;
		SkillButtonScript.skillShorts["*L"] = 50;
		SkillButtonScript.skillShorts["*D"] = 43;
		SkillButtonScript.skillShorts["*B"] = 49;
		SkillButtonScript.skillShorts["*A"] = 51;
		SkillButtonScript.skillShorts["*T"] = 52;
		SkillButtonScript.skillShorts["*J"] = 54;
		SkillButtonScript.skillShorts["*u"] = 53;
		SkillButtonScript.skillShorts["*I"] = 55;
		SkillButtonScript.skillShorts["*a"] = 56;
		SkillButtonScript.skillShorts["*d"] = 57;
		SkillButtonScript.skillShorts["*g"] = 58;
	}

	// Token: 0x06000286 RID: 646 RVA: 0x000067CB File Offset: 0x000049CB
	public void SetIcon(int type, bool isUp, int level, bool lockbool)
	{
		this.viewUpdated = false;
		this.type = type;
		this.isUp = isUp;
		this.level = level;
		this.lockbool = lockbool;
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Start()
	{
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00027F48 File Offset: 0x00026148
	private void Update()
	{
		if (!this.viewUpdated)
		{
			if (this.type == -1)
			{
				this.icon.gameObject.SetActive(false);
				this.up.gameObject.SetActive(false);
				this.levelTF.gameObject.SetActive(false);
				this.levelImage.gameObject.SetActive(false);
			}
			else
			{
                this.icon.gameObject.SetActive(true);
                this.levelTF.gameObject.SetActive(true);
                this.levelImage.gameObject.SetActive(true);
                this.icon.sprite = SkillButtonScript.sprites[this.type];
                this.up.gameObject.SetActive(this.isUp);
                if (this.lockbool)
                {
                    this.up.sprite = this.lockedSprite;
                }
                else
                {
                    this.up.sprite = this.upSprite;
                }
                this.levelTF.text = this.level.ToString();
                this.levelImage.color = SkillButtonScript.colors[this.type];
            }
			this.viewUpdated = true;
		}
		if (this.blinking)
		{
			base.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f + 0.5f * Mathf.Sin(15f * Time.time));
		}
	}

	// Token: 0x0400047C RID: 1148
	public static Sprite[] sprites;

	// Token: 0x0400047D RID: 1149
	public static bool inited = false;

	// Token: 0x0400047E RID: 1150
	private const int SKILL_AACD = 0;

	// Token: 0x0400047F RID: 1151
	public const int SKILL_ABLK = 1;

	// Token: 0x04000480 RID: 1152
	public const int SKILL_ADJA = 2;

	// Token: 0x04000481 RID: 1153
	public const int SKILL_AGUN = 10;

	// Token: 0x04000482 RID: 1154
	public const int SKILL_ANIG = 7;

	// Token: 0x04000483 RID: 1155
	public const int SKILL_BLDG = 16;

	// Token: 0x04000484 RID: 1156
	public const int SKILL_BLDQ = 17;

	// Token: 0x04000485 RID: 1157
	public const int SKILL_BLDR = 11;

	// Token: 0x04000486 RID: 1158
	public const int SKILL_BLDY = 20;

	// Token: 0x04000487 RID: 1159
	public const int SKILL_COMP = 21;

	// Token: 0x04000488 RID: 1160
	public const int SKILL_CRYS = 8;

	// Token: 0x04000489 RID: 1161
	public const int SKILL_DEAC = 26;

	// Token: 0x0400048A RID: 1162
	public const int SKILL_DECN = 9;

	// Token: 0x0400048B RID: 1163
	public const int SKILL_DEST = 6;

	// Token: 0x0400048C RID: 1164
	public const int SKILL_DETE = 18;

	// Token: 0x0400048D RID: 1165
	public const int SKILL_DIGG = 12;

	// Token: 0x0400048E RID: 1166
	public const int SKILL_FRAC = 45;

	// Token: 0x0400048F RID: 1167
	public const int SKILL_FRIG = 22;

	// Token: 0x04000490 RID: 1168
	public const int SKILL_GEOL = 3;

	// Token: 0x04000491 RID: 1169
	public const int SKILL_HCMP = 27;

	// Token: 0x04000492 RID: 1170
	public const int SKILL_LIVE = 13;

	// Token: 0x04000493 RID: 1171
	public const int SKILL_MAGN = 36;

	// Token: 0x04000494 RID: 1172
	public const int SKILL_MINB = 4;

	// Token: 0x04000495 RID: 1173
	public const int SKILL_MINC = 23;

	// Token: 0x04000496 RID: 1174
	public const int SKILL_MINE = 14;

	// Token: 0x04000497 RID: 1175
	public const int SKILL_MING = 5;

	// Token: 0x04000498 RID: 1176
	public const int SKILL_MINR = 15;

	// Token: 0x04000499 RID: 1177
	public const int SKILL_MINV = 28;

	// Token: 0x0400049A RID: 1178
	public const int SKILL_MINW = 37;

	// Token: 0x0400049B RID: 1179
	public const int SKILL_MORO = 24;

	// Token: 0x0400049C RID: 1180
	public const int SKILL_X_UPGR = 25;

	// Token: 0x0400049D RID: 1181
	public const int SKILL_MOTO = 19;

	// Token: 0x0400049E RID: 1182
	public const int SKILL_NANO = 46;

	// Token: 0x0400049F RID: 1183
	public const int SKILL_OPOR = 47;

	// Token: 0x040004A0 RID: 1184
	public const int SKILL_PACK = 29;

	// Token: 0x040004A1 RID: 1185
	public const int SKILL_PAKB = 30;

	// Token: 0x040004A2 RID: 1186
	public const int SKILL_PAKC = 31;

	// Token: 0x040004A3 RID: 1187
	public const int SKILL_PAKG = 40;

	// Token: 0x040004A4 RID: 1188
	public const int SKILL_PAKR = 38;

	// Token: 0x040004A5 RID: 1189
	public const int SKILL_PAKV = 32;

	// Token: 0x040004A6 RID: 1190
	public const int SKILL_PAKW = 39;

	// Token: 0x040004A7 RID: 1191
	public const int SKILL_RECO = 41;

	// Token: 0x040004A8 RID: 1192
	public const int SKILL_X_MONY = 33;

	// Token: 0x040004A9 RID: 1193
	public const int SKILL_REPA = 42;

	// Token: 0x040004AA RID: 1194
	public const int SKILL_ROAD = 48;

	// Token: 0x040004AB RID: 1195
	public const int SKILL_X_BLDU = 49;

	// Token: 0x040004AC RID: 1196
	public const int SKILL_X_MINE = 43;

	// Token: 0x040004AD RID: 1197
	public const int SKILL_SORT = 34;

	// Token: 0x040004AE RID: 1198
	public const int SKILL_SUBL = 35;

	// Token: 0x040004AF RID: 1199
	public const int SKILL_WASH = 44;

	// Token: 0x040004B0 RID: 1200
	public const int SKILL_X_WARB = 50;

	// Token: 0x040004B1 RID: 1201
	public const int SKILL_X_ARCH = 51;

	// Token: 0x040004B2 RID: 1202
	public const int SKILL_X_TODS = 52;

	// Token: 0x040004B3 RID: 1203
	public const int SKILL_X_ULTR = 53;

	// Token: 0x040004B4 RID: 1204
	public const int SKILL_X_JEWL = 54;

	// Token: 0x040004B5 RID: 1205
	public const int SKILL_X_INDU = 55;

	// Token: 0x040004B6 RID: 1206
	public const int SKILL_X_ACID = 56;

	// Token: 0x040004B7 RID: 1207
	public const int SKILL_X_DEEP = 57;

	// Token: 0x040004B8 RID: 1208
	public const int SKILL_X_GLUO = 58;

	// Token: 0x040004B9 RID: 1209
	public static Color SKILL_COLOR_ORANGE = new Color(1f, 0.398f, 0f);

	// Token: 0x040004BA RID: 1210
	public static Color SKILL_COLOR_YELLOW = new Color(1f, 1f, 0f);

	// Token: 0x040004BB RID: 1211
	public static Color SKILL_COLOR_RED = new Color(1f, 0.33f, 0.33f);

	// Token: 0x040004BC RID: 1212
	public static Color SKILL_COLOR_WHITE = new Color(0.93f, 0.93f, 0.93f);

	// Token: 0x040004BD RID: 1213
	public static Color SKILL_COLOR_PINK = new Color(1f, 0f, 1f);

	// Token: 0x040004BE RID: 1214
	public static Color SKILL_COLOR_GREEN = new Color(0f, 1f, 0f);

	// Token: 0x040004BF RID: 1215
	public static Color SKILL_COLOR_BLUE = new Color(0.16f, 0.5f, 1f);

	// Token: 0x040004C0 RID: 1216
	public static Color SKILL_COLOR_CYAN = new Color(0f, 1f, 0.9f);

	// Token: 0x040004C1 RID: 1217
	public static Color SKILL_COLOR_GREY = new Color(0.57421875f, 0.671875f, 0.65234375f);

	// Token: 0x040004C2 RID: 1218
	public Image icon;

	// Token: 0x040004C3 RID: 1219
	public Image up;

	// Token: 0x040004C4 RID: 1220
	public Sprite lockedSprite;

	// Token: 0x040004C5 RID: 1221
	public Sprite upSprite;

	// Token: 0x040004C6 RID: 1222
	public Image levelImage;

	// Token: 0x040004C7 RID: 1223
	public Text levelTF;

	// Token: 0x040004C8 RID: 1224
	private bool viewUpdated = true;

	// Token: 0x040004C9 RID: 1225
	private int type;

	// Token: 0x040004CA RID: 1226
	private bool isUp;

	// Token: 0x040004CB RID: 1227
	private bool lockbool;

	// Token: 0x040004CC RID: 1228
	private int level;

	// Token: 0x040004CD RID: 1229
	public static Dictionary<string, int> skillShorts = new Dictionary<string, int>();

	// Token: 0x040004CE RID: 1230
	public static Dictionary<int, Color> colors = new Dictionary<int, Color>();

	// Token: 0x040004CF RID: 1231
	public bool blinking;
}
