using System;
using System.Collections.Generic;
using System.Text;
using SevenZip.Compression.LZMA;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200005C RID: 92
public class ProgrammatorView : MonoBehaviour
{
	// Token: 0x0600021C RID: 540 RVA: 0x00020970 File Offset: 0x0001EB70
	public void UpdateIconsWithoutSaving()
	{
		int current = this.pageSelector.current;
		for (int i = 0; i < ProgrammatorView.ROWS; i++)
		{
			for (int j = 0; j < ProgrammatorView.COLS; j++)
			{
				int num = i * ProgrammatorView.COLS + j;
				int num2 = current * ProgrammatorView.TOKENS;
				ProgAction progAction = ProgrammatorView.actions[num];
				int id = ProgrammatorView.codes[num2 + num];
				string @string = ProgrammatorView.code_labels[num2 + num];
				int num3 = ProgrammatorView.nums[num2 + num];
				progAction.setString(@string);
				progAction.setNum(num3);
				progAction.ChangeTo(id);
			}
		}
		this.prevPage = current;
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00020A08 File Offset: 0x0001EC08
	public void UpdateIcons()
	{
		int current = this.pageSelector.current;
		for (int i = 0; i < ProgrammatorView.ROWS; i++)
		{
			for (int j = 0; j < ProgrammatorView.COLS; j++)
			{
				int num = i * ProgrammatorView.COLS + j;
				int num2 = this.prevPage * ProgrammatorView.TOKENS;
				int num3 = current * ProgrammatorView.TOKENS;
				ProgAction progAction = ProgrammatorView.actions[num];
				ProgrammatorView.codes[num2 + num] = progAction.id;
				ProgrammatorView.code_labels[num2 + num] = progAction.getString();
				ProgrammatorView.nums[num2 + num] = progAction.getNum();
				int id = ProgrammatorView.codes[num3 + num];
				int num4 = ProgrammatorView.nums[num3 + num];
				string @string = ProgrammatorView.code_labels[num3 + num];
				progAction.setString(@string);
				progAction.setNum(num4);
				progAction.ChangeTo(id);
			}
		}
		this.prevPage = current;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00020AF0 File Offset: 0x0001ECF0
	public void ClearSource()
	{
		for (int i = 0; i < ProgrammatorView.PAGES; i++)
		{
			for (int j = 0; j < ProgrammatorView.ROWS; j++)
			{
				for (int k = 0; k < ProgrammatorView.COLS; k++)
				{
					int num = i * ProgrammatorView.TOKENS + j * ProgrammatorView.COLS + k;
					ProgrammatorView.codes[num] = 0;
					ProgrammatorView.nums[num] = 0;
					ProgrammatorView.code_labels[num] = "0";
				}
			}
		}
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00020B5C File Offset: 0x0001ED5C
	private void Start()
	{
		this.commandSelector = base.gameObject.AddComponent<RuntimeCommandSelector>();
		ProgrammatorView.THIS = this;
		string[] array = new string[ProgrammatorView.PAGES];
		for (int i = 0; i < ProgrammatorView.PAGES; i++)
		{
			if (i < 10)
			{
				array[i] = "0" + i;
			}
			else
			{
				array[i] = string.Concat(i);
			}
		}
		this.pageSelector.SetStrings(array);
		this.ClearSource();
		for (int j = 0; j < ProgrammatorView.ROWS; j++)
		{
			for (int k = 0; k < ProgrammatorView.COLS; k++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.iconPrefab);
				gameObject.transform.SetParent(this.iconContainer.transform);
				Vector3 localPosition = new Vector3(16f + 32f * (float)k, -16f - 32f * (float)j, 0f);
				gameObject.GetComponent<RectTransform>().localPosition = localPosition;
				gameObject.GetComponent<ProgAction>().ChangeTo(ProgrammatorView.codes[j * ProgrammatorView.COLS + k]);
				ProgrammatorView.actions[j * ProgrammatorView.COLS + k] = gameObject.GetComponent<ProgAction>();
			}
		}
		this.pageSelector.ChangeEvent.AddListener(new UnityAction(this.OnPageChange));
		this.UpdateIcons();
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00006481 File Offset: 0x00004681
	private void OnPageChange()
	{
		this.UpdateIcons();
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00020CB0 File Offset: 0x0001EEB0
	private bool MakePosition()
	{
		Vector2 vector = default(Vector2);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out vector);
		int num = Mathf.FloorToInt(vector.x / 32f);
		int num2 = Mathf.FloorToInt(-vector.y / 32f);
		if (num < 0 || num > ProgrammatorView.COLS - 1)
		{
			return false;
		}
		if (num2 < 0 || num2 > ProgrammatorView.ROWS - 1)
		{
			return false;
		}
		this.position = num2 * ProgrammatorView.COLS + num;
		return true;
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00020D38 File Offset: 0x0001EF38
	private void ShiftCode(int dx, int dy)
	{
		if (this.MakePosition())
		{
			int num = this.position;
			int num2 = 0;
			int num3 = Mathf.FloorToInt((float)(this.position / ProgrammatorView.TOKENS));
			int num4 = Mathf.FloorToInt((float)((this.position - num3 * ProgrammatorView.TOKENS) / ProgrammatorView.COLS));
			int num5 = Mathf.FloorToInt((float)(this.position % ProgrammatorView.COLS));
			bool flag = false;
			for (int i = 0; i < 1000; i++)
			{
				if (ProgrammatorView.actions[num].id == 0)
				{
					flag = true;
					break;
				}
				num2++;
				num = num + dx + ProgrammatorView.COLS * dy;
				int num6 = Mathf.FloorToInt((float)(num / ProgrammatorView.TOKENS));
				int num7 = Mathf.FloorToInt((float)((num - num6 * ProgrammatorView.TOKENS) / ProgrammatorView.COLS));
				int num8 = Mathf.FloorToInt((float)(num % ProgrammatorView.COLS));
				if (num < 0 || num6 != num3 || (dx == 0 && num8 != num5) || (dy == 0 && num7 != num4))
				{
					break;
				}
			}
			if (flag)
			{
				for (int j = num2; j > 0; j--)
				{
					int num9 = this.position + (j - 1) * dx + (j - 1) * ProgrammatorView.COLS * dy;
					int num10 = this.position + j * dx + j * ProgrammatorView.COLS * dy;
					ProgAction progAction = ProgrammatorView.actions[num10];
					ProgAction progAction2 = ProgrammatorView.actions[num9];
					progAction.setString(progAction2.getString());
					progAction.setNum(progAction2.getNum());
					progAction.ChangeTo(progAction2.id);
				}
				if (num2 > 0)
				{
					ProgrammatorView.actions[this.position].ChangeTo(0);
					ProgrammatorView.unsaved = true;
					this.titleTF.text = ProgrammatorView.title + " [*]";
				}
			}
		}
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00006489 File Offset: 0x00004689
	private void MakeCycle(int[] cycle, int usePrev = -1)
	{
		if (this.MakePosition())
		{
			this.CyclicChange(cycle, usePrev);
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00020EE4 File Offset: 0x0001F0E4
	private void CyclicChange(int[] cycle, int usePrev)
	{
		int num = -1;
		for (int i = 0; i < cycle.Length; i++)
		{
			if (cycle[i] == ProgrammatorView.actions[this.position].id)
			{
				num = i;
			}
		}
		if (num == -1)
		{
			num = usePrev;
		}
		int num2 = cycle[(num + 1) % cycle.Length];
		if (ProgrammatorView.actions[this.position].id != num2)
		{
			ProgrammatorView.unsaved = true;
			this.titleTF.text = ProgrammatorView.title + " [*]";
		}
		ProgrammatorView.actions[this.position].ChangeTo(num2);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00020F70 File Offset: 0x0001F170
	public int BufShift(int code)
	{
		if (code == 0)
		{
			return 0;
		}
		if (code == 1)
		{
			return 0;
		}
		if (code == 40)
		{
			return 0;
		}
		if (code == 166)
		{
			return 3;
		}
		if (code == 24)
		{
			return 3;
		}
		if (code == 140)
		{
			return 3;
		}
		if (code == 139)
		{
			return 3;
		}
		if (code == 25)
		{
			return 3;
		}
		if (code == 26)
		{
			return 3;
		}
		if (code == 137)
		{
			return 3;
		}
		if (code == 120)
		{
			return 7;
		}
		if (code == 119)
		{
			return 7;
		}
		if (code == 123)
		{
			return 7;
		}
		if (code == 181)
		{
			return 4;
		}
		if (code == 182)
		{
			return 4;
		}
		return 1;
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00020FF8 File Offset: 0x0001F1F8
	public void LoadFromString(string source)
	{
		this.ClearSource();
		if (source[0] == 'X')
		{
			byte[] array = SevenZipHelper.Decompress(Convert.FromBase64String(source));
			Debug.Log("da");
			this.ClearSource();
			int num = BitConverter.ToInt32(array, 0);
			for (int i = 0; i < num; i++)
			{
				ProgrammatorView.codes[i] = (int)Convert.ToInt16(array[i + 4]);
			}
			string[] array2 = Encoding.UTF8.GetString(array, num + 4, array.Length - num - 4).Split(new char[]
			{
				':'
			});
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].Contains("@"))
				{
					string[] array3 = array2[j].Split(new char[]
					{
						'@'
					});
					ProgrammatorView.code_labels[j] = array3[0];
					ProgrammatorView.nums[j] = int.Parse(array3[1]);
				}
				else
				{
					ProgrammatorView.code_labels[j] = array2[j];
					ProgrammatorView.nums[j] = 0;
				}
			}
		}
		else if (source[0] == '$')
		{
			source = source.Replace("$", "");
			source = source.Replace("\r", "");
			source = source.Replace("_", "   ");
			source = source.Replace(".0.", "\n\n\n\n\n\n\n\n\n");
			source = source.Replace(".9.", "\n\n\n\n\n\n\n\n");
			source = source.Replace(".8.", "\n\n\n\n\n\n\n");
			source = source.Replace(".7.", "\n\n\n\n\n\n");
			source = source.Replace(".6.", "\n\n\n\n\n");
			source = source.Replace(".5.", "\n\n\n\n");
			source = source.Replace("...", "\n\n\n");
			source = source.Replace("..", "\n\n");
			source = source.Replace(".", "\n");
			this.str200 = source;
			string text = source;
			int k = 0;
			int num2 = 0;
			int num3 = ProgrammatorView.COLS;
			int num4 = ProgrammatorView.COLS * ProgrammatorView.ROWS;
			while (k < text.Length)
			{
				string a = "";
				string text2 = text.Substring(k);
				for (int l = 0; l < this.strmas2.Length; l++)
				{
					if (text2.StartsWith(this.strmas2[l]))
					{
						k += this.strmas2[l].Length;
						a = this.strmas2[l];
						break;
					}
				}
				k++;
				int num5 = -1;
				if (a == " ")
				{
					num5 = 0;
				}
				else if (a == ",")
				{
					num5 = 1;
				}
				else if (a == "#S")
				{
					num5 = 2;
				}
				else if (a == "#E")
				{
					num5 = 3;
				}
				else if (a == "^W")
				{
					num5 = 4;
				}
				else if (a == "^A")
				{
					num5 = 5;
				}
				else if (a == "^S")
				{
					num5 = 6;
				}
				else if (a == "^D")
				{
					num5 = 7;
				}
				else if (a == "z")
				{
					num5 = 8;
				}
				else if (a == "w")
				{
					num5 = 9;
				}
				else if (a == "a")
				{
					num5 = 10;
				}
				else if (a == "s")
				{
					num5 = 11;
				}
				else if (a == "d")
				{
					num5 = 12;
				}
				else if (a == "l")
				{
					num5 = 13;
				}
				else if (a == "^F")
				{
					num5 = 14;
				}
				else if (a == "CCW;")
				{
					num5 = 15;
				}
				else if (a == "CW;")
				{
					num5 = 16;
				}
				else if (a == "b")
				{
					num5 = 17;
				}
				else if (a == "g")
				{
					num5 = 18;
				}
				else if (a == "r")
				{
					num5 = 19;
				}
				else if (a == "h")
				{
					num5 = 20;
				}
				else if (a == "q")
				{
					num5 = 21;
				}
				else if (a == "RAND;")
				{
					num5 = 22;
				}
				else if (a == "BEEP;")
				{
					num5 = 23;
				}
				else if (a == ">")
				{
					num5 = 24;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('|', ref k);
				}
				else if (a == ":>")
				{
					num5 = 25;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('>', ref k);
				}
				else if (a == "->")
				{
					num5 = 26;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('>', ref k);
				}
				else if (a == "<|")
				{
					num5 = 27;
				}
				else if (a == "<-|")
				{
					num5 = 28;
				}
				else if (a == "[WA]")
				{
					num5 = 29;
				}
				else if (a == "[SD]")
				{
					num5 = 30;
				}
				else if (a == "[W]")
				{
					num5 = 31;
				}
				else if (a == "[DW]")
				{
					num5 = 32;
				}
				else if (a == "[A]")
				{
					num5 = 33;
				}
				else if (a == "[D]")
				{
					num5 = 35;
				}
				else if (a == "[AS]")
				{
					num5 = 36;
				}
				else if (a == "[S]")
				{
					num5 = 37;
				}
				else if (a == "OR")
				{
					num5 = 38;
				}
				else if (a == "AND")
				{
					num5 = 39;
				}
				else if (a == "|")
				{
					num5 = 40;
					string text3 = this.NewProgramm(':', ref k);
					ProgrammatorView.code_labels[num2] = text3;
				}
				else if (a == "=n")
				{
					num5 = 43;
				}
				else if (a == "=e")
				{
					num5 = 44;
				}
				else if (a == "=f")
				{
					num5 = 45;
				}
				else if (a == "=c")
				{
					num5 = 46;
				}
				else if (a == "=a")
				{
					num5 = 47;
				}
				else if (a == "=b")
				{
					num5 = 48;
				}
				else if (a == "=s")
				{
					num5 = 49;
				}
				else if (a == "=k")
				{
					num5 = 50;
				}
				else if (a == "=d")
				{
					num5 = 51;
				}
				else if (a == "=K")
				{
					num5 = 52;
				}
				else if (a == "=B")
				{
					num5 = 53;
				}
				else if (a == "=A")
				{
					num5 = 54;
				}
				else if (a == "=q")
				{
					num5 = 57;
				}
				else if (a == "=R")
				{
					num5 = 58;
				}
				else if (a == "=r")
				{
					num5 = 59;
				}
				else if (a == "=y")
				{
					num5 = 60;
				}
				else if (a == "=x")
				{
					num5 = 74;
				}
				else if (a == "=o")
				{
					num5 = 76;
				}
				else if (a == "=g")
				{
					num5 = 77;
				}
				else if (a == "[a]")
				{
					num5 = 132;
				}
				else if (a == "[s]")
				{
					num5 = 133;
				}
				else if (a == "[d]")
				{
					num5 = 134;
				}
				else if (a == "[F]")
				{
					num5 = 135;
				}
				else if (a == "[f]")
				{
					num5 = 136;
				}
				else if (a == "=>")
				{
					num5 = 137;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('>', ref k);
				}
				else if (a == "<=|")
				{
					num5 = 138;
				}
				else if (a == "?")
				{
					num5 = 139;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('<', ref k);
				}
				else if (a == "!?")
				{
					num5 = 140;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('<', ref k);
				}
				else if (a == "DIGG;")
				{
					num5 = 141;
				}
				else if (a == "BUILD;")
				{
					num5 = 142;
				}
				else if (a == "HEAL;")
				{
					num5 = 143;
				}
				else if (a == "FLIP;")
				{
					num5 = 144;
				}
				else if (a == "MINE;")
				{
					num5 = 145;
				}
				else if (a == "=G")
				{
					num5 = 146;
				}
				else if (a == "FILL;")
				{
					num5 = 147;
				}
				else if (a == "=hp-")
				{
					num5 = 148;
				}
				else if (a == "=hp50")
				{
					num5 = 149;
				}
				else if (a == "[r]")
				{
					num5 = 156;
				}
				else if (a == "[l]")
				{
					num5 = 157;
				}
				else if (a == "AUT+")
				{
					num5 = 158;
				}
				else if (a == "AUT-")
				{
					num5 = 159;
				}
				else if (a == "AGR+")
				{
					num5 = 160;
				}
				else if (a == "AGR-")
				{
					num5 = 161;
				}
				else if (a == "B1;")
				{
					num5 = 162;
				}
				else if (a == "B2;")
				{
					num5 = 163;
				}
				else if (a == "B3;")
				{
					num5 = 164;
				}
				else if (a == "VB;")
				{
					num5 = 165;
				}
				else if (a == "#R")
				{
					num5 = 166;
				}
				else if (a == "GEO;")
				{
					num5 = 167;
				}
				else if (a == "ZZ;")
				{
					num5 = 168;
				}
				else if (a == "C190;")
				{
					num5 = 169;
				}
				else if (a == "POLY;")
				{
					num5 = 170;
				}
				else if (a == "UP;")
				{
					num5 = 171;
				}
				else if (a == "CRAFT;")
				{
					num5 = 172;
				}
				else if (a == "NANO;")
				{
					num5 = 173;
				}
				else if (a == "REM;")
				{
					num5 = 174;
				}
				else if (a == "iw")
				{
					num5 = 175;
				}
				else if (a == "ia")
				{
					num5 = 176;
				}
				else if (a == "is")
				{
					num5 = 177;
				}
				else if (a == "id")
				{
					num5 = 178;
				}
				else if (a == "Hand+")
				{
					num5 = 179;
				}
				else if (a == "Hand-")
				{
					num5 = 180;
				}
				else if (a == "!{")
				{
					num5 = 181;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('}', ref k);
				}
				else if (a == "{")
				{
					num5 = 182;
					ProgrammatorView.code_labels[num2] = this.NewProgramm('}', ref k);
				}
				else if (a == "RESTART;")
				{
					num5 = 200;
				}
				else if (a == "\n")
				{
					num2 += num3;
					num4 -= num3;
					num3 = ProgrammatorView.COLS;
				}
				else if (a == "~\n")
				{
					num2 += num4;
					num3 = ProgrammatorView.COLS;
					num4 = ProgrammatorView.COLS * ProgrammatorView.ROWS;
					Debug.Log("next page!!!");
				}
				else if (a == "(")
				{
					string text4 = this.NewProgramm(')', ref k);
					string[] array4;
					if (text4.Contains("="))
					{
						array4 = text4.Split(new char[]
						{
							'='
						});
						num5 = 123;
					}
					else if (text4.Contains("<"))
					{
						array4 = text4.Split(new char[]
						{
							'<'
						});
						num5 = 120;
					}
					else
					{
						if (!text4.Contains(">"))
						{
							return;
						}
						array4 = text4.Split(new char[]
						{
							'>'
						});
						num5 = 119;
					}
					ProgrammatorView.code_labels[num2] = array4[0];
					int num6 = 0;
					if (!int.TryParse(array4[1], out num6))
					{
						num6 = 0;
					}
					ProgrammatorView.nums[num2] = num6;
					Debug.Log(array4[0]);
				}
				if (num5 != -1)
				{
					ProgrammatorView.codes[num2] = num5;
					num2++;
					num3--;
					num4--;
				}
				if (num2 > ProgrammatorView.COLS * ProgrammatorView.ROWS * ProgrammatorView.PAGES)
				{
					break;
				}
			}
		}
		for (int m = 0; m < 10; m++)
		{
			Debug.Log(ProgrammatorView.nums[m]);
		}
		this.UpdateIconsWithoutSaving();
	}

	// Token: 0x06000227 RID: 551 RVA: 0x00021E90 File Offset: 0x00020090
	public string SaveToString()
	{
		this.UpdateIcons();
		if (!ClientConfig.OLD_PROGRAM_FORMAT)
		{
			return this.SaveToStringNew();
		}
		int num = 0;
		for (int i = 0; i < ProgrammatorView.codes.Length; i++)
		{
			if (ProgrammatorView.codes[i] != 0)
			{
				num = i;
			}
		}
		if (num == 0)
		{
			num = 1;
		}
		int num2 = 0;
		for (int j = 0; j < ProgrammatorView.code_labels.Length; j++)
		{
			if (ProgrammatorView.code_labels[j] != "0" || ProgrammatorView.nums[j] != 0)
			{
				num2 = j;
			}
		}
		if (num2 == 0)
		{
			num2 = 1;
		}
		string[] array = new string[num2 + 1];
		Array.Copy(ProgrammatorView.code_labels, array, num2 + 1);
		for (int k = 0; k < array.Length; k++)
		{
			if (ProgrammatorView.nums[k] != 0)
			{
				string[] array2 = array;
				int num3 = k;
				array2[num3] = array2[num3] + "@" + ProgrammatorView.nums[k];
			}
		}
		string s = string.Join(":", array);
		byte[] array3 = new byte[num + 1];
		for (int l = 0; l < array3.Length; l++)
		{
			array3[l] = Convert.ToByte(ProgrammatorView.codes[l]);
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = BitConverter.GetBytes(array3.Length);
		byte[] array4 = new byte[bytes2.Length + array3.Length + bytes.Length];
		Buffer.BlockCopy(bytes2, 0, array4, 0, bytes2.Length);
		Buffer.BlockCopy(array3, 0, array4, bytes2.Length, array3.Length);
		Buffer.BlockCopy(bytes, 0, array4, bytes2.Length + array3.Length, bytes.Length);
		return Convert.ToBase64String(SevenZipHelper.Compress(array4));
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00022020 File Offset: 0x00020220
	public void SendAndStartProgram()
	{
		byte[] array = this.CompileProgram();
		string s = this.SaveToString();
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = BitConverter.GetBytes(array.Length);
		byte[] bytes3 = BitConverter.GetBytes(ProgrammatorView.programId);
		byte[] array2 = new byte[bytes2.Length + bytes3.Length + array.Length + bytes.Length];
		Buffer.BlockCopy(bytes2, 0, array2, 0, bytes2.Length);
		Buffer.BlockCopy(bytes3, 0, array2, bytes2.Length, bytes3.Length);
		Buffer.BlockCopy(array, 0, array2, bytes2.Length + bytes3.Length, array.Length);
		Buffer.BlockCopy(bytes, 0, array2, bytes2.Length + array.Length + bytes3.Length, bytes.Length);
		ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "PROG", 0, 0, array2);
	}

	// Token: 0x06000229 RID: 553 RVA: 0x000220DC File Offset: 0x000202DC
	public byte[] CompileProgram()
	{
		this.UpdateIcons();
		this.labels = new Dictionary<string, int>();
		int num = 0;
		for (int i = 0; i < ProgrammatorView.PAGES; i++)
		{
			for (int j = 0; j < ProgrammatorView.ROWS; j++)
			{
				bool flag = true;
				bool flag2 = false;
				for (int k = 0; k < ProgrammatorView.COLS; k++)
				{
					if (!flag2)
					{
						int num2 = ProgrammatorView.codes[i * ProgrammatorView.TOKENS + j * ProgrammatorView.COLS + k];
						if (num2 != 0)
						{
							flag = false;
						}
						if (num2 == 1)
						{
							flag2 = true;
						}
						else if (num2 == 40)
						{
							this.labels[ProgrammatorView.code_labels[i * ProgrammatorView.TOKENS + j * ProgrammatorView.COLS + k]] = num;
						}
						num += this.BufShift(num2);
					}
				}
				if (!flag && !flag2)
				{
					num++;
				}
			}
		}
		List<int> list = new List<int>();
		for (int l = 0; l < ProgrammatorView.PAGES; l++)
		{
			for (int m = 0; m < ProgrammatorView.ROWS; m++)
			{
				bool flag3 = true;
				bool flag4 = false;
				for (int n = 0; n < ProgrammatorView.COLS; n++)
				{
					if (!flag4)
					{
						int num3 = ProgrammatorView.codes[l * ProgrammatorView.TOKENS + m * ProgrammatorView.COLS + n];
						if (num3 != 0)
						{
							if (num3 == 1)
							{
								flag4 = true;
							}
							else if (num3 == 40)
							{
								flag3 = false;
							}
							else if (num3 == 24 || num3 == 140 || num3 == 166 || num3 == 139 || num3 == 25 || num3 == 26 || num3 == 137)
							{
								list.Add(num3);
								string key = ProgrammatorView.code_labels[l * ProgrammatorView.TOKENS + m * ProgrammatorView.COLS + n];
								int num4 = 0;
								if (this.labels.ContainsKey(key))
								{
									num4 = this.labels[key];
								}
								int item = num4 / 256;
								int item2 = num4 % 256;
								list.Add(item);
								list.Add(item2);
								flag3 = false;
							}
							else if (num3 == 123 || num3 == 119 || num3 == 120)
							{
								list.Add(num3);
								string text2;
								string text = text2 = ProgrammatorView.code_labels[l * ProgrammatorView.TOKENS + m * ProgrammatorView.COLS + n];
								if (text.Length == 0)
								{
									text2 += "   ";
								}
								if (text.Length == 1)
								{
									text2 += "  ";
								}
								if (text.Length == 2)
								{
									text2 += " ";
								}
								char[] array = text2.ToCharArray();
								list.Add((int)array[0]);
								list.Add((int)array[1]);
								list.Add((int)array[2]);
								int num5 = ProgrammatorView.nums[l * ProgrammatorView.TOKENS + m * ProgrammatorView.COLS + n];
								if (num5 < 0)
								{
									num5 += 16777216;
								}
								list.Add(num5 / 65536);
								list.Add(num5 / 256 % 256);
								list.Add(num5 % 256);
								flag3 = false;
							}
							else if (num3 == 181 || num3 == 182)
							{
								list.Add(num3);
								string text4;
								string text3 = text4 = ProgrammatorView.code_labels[l * ProgrammatorView.TOKENS + m * ProgrammatorView.COLS + n];
								if (text3.Length == 0)
								{
									text4 += "   ";
								}
								if (text3.Length == 1)
								{
									text4 += "  ";
								}
								if (text3.Length == 2)
								{
									text4 += " ";
								}
								char[] array2 = text4.ToCharArray();
								list.Add((int)array2[0]);
								list.Add((int)array2[1]);
								list.Add((int)array2[2]);
								flag3 = false;
							}
							else
							{
								list.Add(num3);
								flag3 = false;
							}
						}
					}
				}
				if (!flag3 && !flag4)
				{
					list.Add(200);
				}
			}
		}
		int[] array3 = list.ToArray();
		byte[] array4 = new byte[array3.Length];
		for (int num6 = 0; num6 < array3.Length; num6++)
		{
			array4[num6] = (byte)array3[num6];
		}
		return array4;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000649B File Offset: 0x0000469B
	public void Show()
	{
		GUIManager.THIS.m_EventSystem.SetSelectedGameObject(null);
		ProgrammatorView.opened = true;
		this.UpdateIcons();
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00022500 File Offset: 0x00020700
	private void Update()
	{
		if (ProgrammatorView.active && !this.commandSelector.enabled)
		{
			if (Input.GetMouseButtonDown(0) && this.MakePosition())
			{
				//base.GetComponent<RuntimeCommandSelector>().Show(this.position);
			}
			if (GUIManager.THIS.m_EventSystem.currentSelectedGameObject == null)
			{
				if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
				{
					GUIUtility.systemCopyBuffer = this.SaveToString();
					return;
				}
				if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
				{
					AYSWindowManager.THIS.Show("ЗАГРУЗКА СТРОКИ ИЗ ПРОГРАММЫ", "Вы собираетесь загрузить программу из буффера обмена.\nТекущая программа будет потеряна.\nУверены?", delegate
					{
						this.LoadFromString(GUIUtility.systemCopyBuffer);
					});
					return;
				}
				if (Input.GetKeyDown(KeyCode.C))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shc_button, -1);
					}
					else
					{
						this.MakeCycle(this.c_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.W))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shw_button, 5);
					}
					else
					{
						this.MakeCycle(this.w_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.A))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.sha_button, 5);
					}
					else
					{
						this.MakeCycle(this.a_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.S))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shs_button, 5);
					}
					else
					{
						this.MakeCycle(this.s_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.D))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shd_button, 5);
					}
					else
					{
						this.MakeCycle(this.d_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.F))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shf_button, -1);
					}
					else
					{
						this.MakeCycle(this.f_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.X))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shx_button, -1);
					}
					else
					{
						this.MakeCycle(this.x_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.V))
				{
					this.MakeCycle(this.v_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.R))
				{
					this.MakeCycle(this.r_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.E))
				{
					this.MakeCycle(this.e_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.Z))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shz_button, -1);
					}
					else
					{
						this.MakeCycle(this.z_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.T))
				{
					this.MakeCycle(this.t_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.L))
				{
					this.MakeCycle(this.l_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.G))
				{
					this.MakeCycle(this.g_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.Q))
				{
					this.MakeCycle(this.q_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.O))
				{
					this.MakeCycle(this.o_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.I))
				{
					this.MakeCycle(this.i_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.H))
				{
					this.MakeCycle(this.h_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.J))
				{
					this.MakeCycle(this.j_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.B))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.MakeCycle(this.shb_button, -1);
					}
					else
					{
						this.MakeCycle(this.b_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.M))
				{
					this.MakeCycle(this.m_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.Y))
				{
					if (Input.GetKey(KeyCode.LeftCommand))
					{
						this.NewProgramm3();
					}
					else
					{
						this.MakeCycle(this.y_button, -1);
					}
				}
				if (Input.GetKeyDown(KeyCode.Delete))
				{
					this.MakeCycle(this.del_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.Backspace))
				{
					this.MakeCycle(this.back_button, -1);
				}
				if (Input.GetKeyDown(KeyCode.DownArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					this.ShiftCode(0, 1);
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					this.ShiftCode(-1, 0);
				}
				if (Input.GetKeyDown(KeyCode.RightArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					this.ShiftCode(1, 0);
				}
				if (Input.GetKeyDown(KeyCode.UpArrow) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					this.ShiftCode(0, -1);
					return;
				}
			}
			else if (GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "ProgInput")
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					GUIManager.THIS.m_EventSystem.SetSelectedGameObject(null);
					return;
				}
			}
			else if (GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "RenameButton" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "CopyButton" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "ClearButton" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "ToMenuButton" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "LessButton" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "MoreButton")
			{
				GUIManager.THIS.m_EventSystem.SetSelectedGameObject(null);
			}
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00022B5C File Offset: 0x00020D5C
	public ProgrammatorView()
	{
		int[] array = new int[2];
		array[0] = 1;
		this.back_button = array;
		this.t_button = new int[]
		{
			141,
			142,
			143,
			145
		};
		this.v_button = new int[]
		{
			123,
			120,
			119
		};
		this.g_button = new int[]
		{
			24,
			25,
			26,
			137
		};
		this.h_button = new int[]
		{
			148,
			149
		};
		this.j_button = new int[]
		{
			146,
			147
		};
		this.q_button = new int[]
		{
			27,
			28,
			138
		};
		this.position = -1;
		this.str200 = "";
		this.strmas2 = new string[]
		{
			" ",
			"\n",
			"~\n",
			",",
			"#S",
			"#E",
			"^W",
			"^A",
			"^S",
			"^D",
			"z",
			"w",
			"a",
			"s",
			"d",
			"l",
			"^F",
			"CCW;",
			"CW;",
			"b",
			"g",
			"r",
			"h",
			"q",
			"RAND;",
			"BEEP;",
			"B1;",
			"B2;",
			"B3;",
			"VB;",
			"GEO;",
			"ZZ;",
			"C190;",
			"POLY;",
			"UP;",
			"CRAFT;",
			"NANO;",
			"REM;",
			"iw",
			"ia",
			"is",
			"id",
			"|",
			">",
			":>",
			"->",
			"=>",
			"<|",
			"<-|",
			"<=|",
			"[W]",
			"[WA]",
			"[A]",
			"[AS]",
			"[S]",
			"[SD]",
			"[D]",
			"[DW]",
			"[w]",
			"[a]",
			"[s]",
			"[d]",
			"[F]",
			"[f]",
			"=n",
			"=e",
			"=f",
			"=c",
			"=a",
			"=b",
			"=s",
			"=k",
			"=d",
			"=K",
			"=B",
			"=A",
			"=q",
			"=R",
			"=r",
			"=y",
			"=g",
			"=o",
			"=x",
			"OR",
			"AND",
			"AUT+",
			"AUT-",
			"AGR+",
			"AGR-",
			"(",
			"?",
			"!?",
			"DIGG;",
			"BUILD;",
			"HEAL;",
			"MINE;",
			"[r]",
			"[l]",
			"=G",
			"FILL;",
			"=hp-",
			"=hp50",
			"FLIP;",
			"#R",
			"RESTART;",
			"Hand+",
			"Hand-",
			"!{",
			"{"
		};
		this.str201 = "XQAAgACaAQAAAAAAAAA+gDAFAhG0ciqkm4PJHRcPgIruSXXpLO0McQ2EaD0CLDkX1Bwh+LW9OM3QB1T+3qVdAUpFp581o584qDxt6eJa5ZmL9IccwWgVwreNz8sedV9O4qdfTvr9Fq21T0SXWLC0hOZTGUlNudYz1VsnqRCp4i7gf/AWcIHs2KgLmwS8IEgJkaNgAA==";
		this.str202 = "XQAAgADOAQAAAAAAAABKADAFCPBEHWF/wkqwvU4Eb7ohVxyHTQCnB1tuOQ/mId9a0zVcyCiQh32E9c7xYZu2Pl8fEh/P3fjzX7/iO1MmqYc/qeocZ8w1lXWa7RaZcFhX7GuXWTLBsXzjlA0KeZpby+P8vwIXwiaA4LU3j9kw1D7zQzt/VRLMs5eYauhxvlEpC/VRXtxodxe8aLB9QD25fpbplLo/R0aUG1SxETQdFrKdpAzY2gA=";
		this.str203 = "XQAAgADPLQAAAAAAAAAAA221b5AWlMuE8z/Xt7w74lgUrnx+dRhKi4sEAcSMKXqeXUPStyWpBdDNlMOEQCIkQWrLhbzOttujqROPHMDvK+I5gscHJvyAUWhNYRkC0+wB8elhb/lKccCjj+uzMXVRLRHVY80PwQEZ65yDOYlCQBaPwcfbnJsTzuveQiBOy+EGsxJKX/cNTlXoKsaUgABWRCeUr9kyB1cWIsSWIZkbQxgrbIVRFPdPOp03bh/dh54copSiGmct01ZOlbNbMHIzNBC5aSo/qdeGuorVulJx2emBzmSdZ3mtBMtTCnM3DL87FrRZRvloJN8+cCIxLtbVLfCVt6c63NPBIVzCaCitfIvr3b6JNaGOCjzuqKnpBFOLNV2fTK7P2cQY9HAiysW3YJubG/Aav6zajPcfYyj3Vh+xj6IaynIi0+3rPTH/kyCEa+luX7ZXFY2WKfcUiYGeZxc7UFWGrxNerLJQhIFCdFDZHmQsZgu1LtYQnFCcyiUNBWMfuVCL+x687Cf0Q+aCnU2ZY+/9Jf8qmcaqBztpRN5q3v80rD/5nOrbEw0gXhwYUj23BFn6frybu4llJfJ8X/WkDRFR/GLhrX4CiGZ8E9KjFbYV4zqtnkOrS4pB9BN17cumWLTLIJYHm9nAc0s1RthJWL//MQYQy3lF5+YW7wj0a/j9Pk27JC6jarObe+qNUoYrVQYnlDtHC4hnBDOeu39QTDyPUXWgxNuR0YaC05Orb1ybW4PnF5GRiu1/injOo88bsqiOg27NGk580QQ8z2/0sG2kkceuKBjFGFjMwaJ98N8zAHP+ibMj9KDFBzw8B4VleITAvstFp9fnfnunOg3EJ+28CB9Z0JhyOeW4l0YiVIBqhisfwMgKt8WDBdZG+blxKXcpDaJENz/1Xx6P6AtlTQvmTAwHKZhwvCc7vOuojUnypMuAYVs+faX15ZripHnT2M8c+ynsBD3p8oIhgAKVlIzQihael+JTu1u0DnWsuyKib5l3nc4EA0XRizR6MYYeelvcLeVWdPo2KDHrfb+USorE47+2f/3D/B7ZEUOrYYqeOjSdrFRMqTsvPPDbffp+wpZJC+dS+8nFge17F7zRiXj8fTsbUTFyBLDt7bt3sIBD6nen7UYGTW//hiy5AkycPeYoE6phXD3u37TBJ0OY0Ci7eOIDTST3ftBMC5zjzptFe0Wt8hYAyfsIuuCHJeeBSsLv1eRmDKkKvvAGLNnnlTCzsMH8RtWWtZSgLwKDAUSWXvMyXdDnquSko2aJNbpEYKpXlpIZg6HSVx4YOLU6bwNdxoiCh6j+OnOTtgSY3RWQJw6oQadjKIvCM8RCGBXmZymS8KngjVPhr0+NPFXu98MS2sqKTINYhaezLtvjbeJSUHR//VxgvlRFtqDntq1vbei/nyCOGjWuCRGABllv3kuYX3+wSLrGJV6YObTkwxX+jKlmYs3EB2VUJBrmZ5pUcQum0mcHPduJBtqkqE9ArkdFkyb4A7GDuVj2oFf4uxjGcWFkvUIhL8dtk1kTf86G3q48EnzH8rRHDXscoTiKxcAhmdnOAY2Kp5lQmkpHSgJn12a/eIkMAQViI/cLemwb6Hgn7wL1miyeT+vqmkERJ3sX9dZZ+k5dk0KmB0301LC2dZieZyn0UAP4XtcsRWzjU9AxrJak+Ql8xK5P9TypwzEyTlkL22g/ncUDb5IN+daeSael90SCVn3dSQ1jGUhjxd3lMMSfz7j52XnZboNEPb/zktlAJ8LyjaBONvtwqaZMzFoF8mS9p0vPmeS6hXH/7WavrFRad6ZV2kiW/D0oGL5w24SxL4a+xxpJjN2Tu591mzXy31PgXjhOVt4911/KLriXKTxWeeCCuv8IortQO0rZIR/q+7MJ9blqr0r0WCLzilDjD9b6XaTzAxJ0mstha7s3BNGMEvOj/qY8xryN4eE2MHqd0toCOPVhrDaCo6txzudt0rAeIlfZksgnecCaUdEZWfSL5b43RrKsHhSeoxzFZcIqu6yTPzLf6mrRBTjrdDrYlWDz3uz487RifwUQbs5MZYJ9uxsk97poBiYxtHZYWnHyEnt0A030uCavacgH86JVLFr15kNlD2b+uATl5QeNTi161ah4hkcmSjYRC4f9iPpTk2pHKaRv9DvvDp3ovI+i/IJpdFR2aXXLJnlHJpHAlhKaWQ1otkEEjmnSOcNlbfsgjDdvQte3daGwKx5LgmBwgxt2rJz0E1VE5btbKjeAypXOyUmHo3yTFeWgfbAp5MDWO7eVF+ed6ebYVLDPqzwx1Mp/eoAsVEVDtycOkvJLL2AklZ6KaqUQSatbBwdJjZP9dnXbq1s2cNuYQhj+ZVkRL0f/hg0xgyKQFHqdZownpfk1U/oXEtzvGcbUcPXXn0/l0P4YHGinDR+XwMalq9lEqa+wcYzrCJUD6ljk/eNSc4jvrti4pdliAoWoSRqJtHbekeAAISEoP6iMPy4HXDJd+vsdy2G5SzopOIWP5puknmlLoA4kqo1XP7dpn2K8UAL/X1uEweOqHW0BWY6AqsSaI0N7EN27HKl0isEI5voDY71hor98zUlyYHmP7wPAJNQI6WvNqziBxh5Wg/EyQ5jdj/TdkqoPOwPpAM1WyIqVgYR63MlNKTbXFhsb4/Qj3HzjF19T/jzttgXpIiO/LV2Q+Rf+SEMqJ7IGvJ+XrrvYvIVdhW7JMjYYZ54hVkC82E0EdrB7jcKFN0O1dxFMWQO+CMuzEcwd5+iY2jXVvj3onrYJwaVoD7+JdvRYBZooRA2n2bpZOg5ycYxHUeicO00XaK2glWOFf8WQTAgv09xmZr0gSS+XAFnWVbA+I512Z9+dZ9mTgN+681jq3PIhIweb1Ri1cwjqvHphxPLSQG31gXjjb/iDeVaHV5JBZNX2RPTDxzli16HUfh3cfmhhJkX9PSVscXbwuiO4t6Gq6YnqJaF5tTYiRN105N7Y4RivzlZgai0eLblmOoCGmBT89BPMkAIEdxCb0XSnT6k4eDXAWGTeUBtbTKtsz7CGdbt3zF2CMNlmBQAxOCovUiSKrGCF3MiF+QLcvcsrv2PmSrJM2wugXAmH0znCeUn9KKCL8Y9KdgpgdE6hJkHY9lIi8qgzkKgML9r/zCvnkWrBRN6Oh1RmS/sf0gMbhnZv4v4Z20kyXCNImeY6sL99OEmvD6ZNRaV7ACkc/Az5avCUNYhqxIaAbGKpIivgU+bbMfE/Ckl7RSmSMVO11GBUAwUI2bb2fRFFanIh1p6pi+afBWyuLZ43Onryh3MvR4rajE1fTNJjRz/eO2KdWezYTjeTN34Vy8TeQkLJK0D6he/6f+rx8EdvmNb2GltIuOiRGFVfknWw4KwFfpp3+XQ2sVxLzcaWePOodYAjZoseCa0Axyk3Wl7STgO6pchvulLjG418u1ZGiMGvIu0eXQKhjsqnvAnTufDab/dBXM5c1KAaWNC6Jy6HW3itGTXj2uF4WF2U83079IPyQ95Adt4gYHoO7agqd5jPq51GvZmGR78XvECWaUCNqru6jJeQtKKDvxIdQd/Fl7WG1RIdSj1D8ylMZA/+7eVy4uh2rtayQaVN574DtDE+";
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void NewProgramm3()
	{
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00023268 File Offset: 0x00021468
	public string SaveToStringNew()
	{
		string text = "";
		for (int i = 0; i < ProgrammatorView.codes.Length; i++)
		{
			int num = i % ProgrammatorView.COLS;
			int num2 = Mathf.FloorToInt((float)(i / ProgrammatorView.COLS)) % ProgrammatorView.ROWS;
			Mathf.FloorToInt((float)(i / ProgrammatorView.TOKENS));
			switch (ProgrammatorView.codes[i])
			{
			case 0:
				text += " ";
				break;
			case 1:
				text += ",";
				break;
			case 2:
				text += "#S";
				break;
			case 3:
				text += "#E";
				break;
			case 4:
				text += "^W";
				break;
			case 5:
				text += "^A";
				break;
			case 6:
				text += "^S";
				break;
			case 7:
				text += "^D";
				break;
			case 8:
				text += "z";
				break;
			case 9:
				text += "w";
				break;
			case 10:
				text += "a";
				break;
			case 11:
				text += "s";
				break;
			case 12:
				text += "d";
				break;
			case 13:
				text += "l";
				break;
			case 14:
				text += "^F";
				break;
			case 15:
				text += "CCW;";
				break;
			case 16:
				text += "CW;";
				break;
			case 17:
				text += "b";
				break;
			case 18:
				text += "g";
				break;
			case 19:
				text += "r";
				break;
			case 20:
				text += "h";
				break;
			case 21:
				text += "q";
				break;
			case 22:
				text += "RAND;";
				break;
			case 23:
				text += "BEEP;";
				break;
			case 24:
				text += ">";
				text += ProgrammatorView.code_labels[i];
				text += "|";
				break;
			case 25:
				text += ":>";
				text += ProgrammatorView.code_labels[i];
				text += ">";
				break;
			case 26:
				text += "->";
				text += ProgrammatorView.code_labels[i];
				text += ">";
				break;
			case 27:
				text += "<|";
				break;
			case 28:
				text += "<-|";
				break;
			case 29:
				text += "[WA]";
				break;
			case 30:
				text += "[SD]";
				break;
			case 31:
				text += "[W]";
				break;
			case 32:
				text += "[DW]";
				break;
			case 33:
				text += "[A]";
				break;
			case 35:
				text += "[D]";
				break;
			case 36:
				text += "[AS]";
				break;
			case 37:
				text += "[S]";
				break;
			case 38:
				text += "OR";
				break;
			case 39:
				text += "AND";
				break;
			case 40:
				text += "|";
				text += ProgrammatorView.code_labels[i];
				text += ":";
				break;
			case 43:
				text += "=n";
				break;
			case 44:
				text += "=e";
				break;
			case 45:
				text += "=f";
				break;
			case 46:
				text += "=c";
				break;
			case 47:
				text += "=a";
				break;
			case 48:
				text += "=b";
				break;
			case 49:
				text += "=s";
				break;
			case 50:
				text += "=k";
				break;
			case 51:
				text += "=d";
				break;
			case 52:
				text += "=K";
				break;
			case 53:
				text += "=B";
				break;
			case 54:
				text += "=A";
				break;
			case 57:
				text += "=q";
				break;
			case 58:
				text += "=R";
				break;
			case 59:
				text += "=r";
				break;
			case 60:
				text += "=y";
				break;
			case 74:
				text += "=x";
				break;
			case 76:
				text += "=o";
				break;
			case 77:
				text += "=g";
				break;
			case 119:
				text += "(";
				text += ProgrammatorView.code_labels[i];
				text += ">";
				text += ProgrammatorView.nums[i];
				text += ")";
				break;
			case 120:
				text += "(";
				text += ProgrammatorView.code_labels[i];
				text += "<";
				text += ProgrammatorView.nums[i];
				text += ")";
				break;
			case 123:
				text += "(";
				text += ProgrammatorView.code_labels[i];
				text += "=";
				text += ProgrammatorView.nums[i];
				text += ")";
				break;
			case 131:
				text += "[w]";
				break;
			case 132:
				text += "[a]";
				break;
			case 133:
				text += "[s]";
				break;
			case 134:
				text += "[d]";
				break;
			case 135:
				text += "[F]";
				break;
			case 136:
				text += "[f]";
				break;
			case 137:
				text += "=>";
				text += ProgrammatorView.code_labels[i];
				text += ">";
				break;
			case 138:
				text += "<=|";
				break;
			case 139:
				text += "?";
				text += ProgrammatorView.code_labels[i];
				text += "<";
				break;
			case 140:
				text += "!?";
				text += ProgrammatorView.code_labels[i];
				text += "<";
				break;
			case 141:
				text += "DIGG;";
				break;
			case 142:
				text += "BUILD;";
				break;
			case 143:
				text += "HEAL;";
				break;
			case 144:
				text += "FLIP;";
				break;
			case 145:
				text += "MINE;";
				break;
			case 146:
				text += "=G";
				break;
			case 147:
				text += "FILL;";
				break;
			case 148:
				text += "=hp-";
				break;
			case 149:
				text += "=hp50";
				break;
			case 156:
				text += "[r]";
				break;
			case 157:
				text += "[l]";
				break;
			case 158:
				text += "AUT+";
				break;
			case 159:
				text += "AUT-";
				break;
			case 160:
				text += "AGR+";
				break;
			case 161:
				text += "AGR-";
				break;
			case 162:
				text += "B1;";
				break;
			case 163:
				text += "B2;";
				break;
			case 164:
				text += "B3;";
				break;
			case 165:
				text += "VB;";
				break;
			case 166:
				text += "#R";
				text += ProgrammatorView.code_labels[i];
				text += "<";
				break;
			case 167:
				text += "GEO;";
				break;
			case 168:
				text += "ZZ;";
				break;
			case 169:
				text += "C190;";
				break;
			case 170:
				text += "POLY;";
				break;
			case 171:
				text += "UP;";
				break;
			case 172:
				text += "CRAFT;";
				break;
			case 173:
				text += "NANO;";
				break;
			case 174:
				text += "REM;";
				break;
			case 175:
				text += "iw";
				break;
			case 176:
				text += "ia";
				break;
			case 177:
				text += "is";
				break;
			case 178:
				text += "id";
				break;
			case 179:
				text += "Hand+";
				break;
			case 180:
				text += "Hand-";
				break;
			case 181:
				text += "!{";
				text += ProgrammatorView.code_labels[i];
				text += "}";
				break;
			case 182:
				text += "{";
				text += ProgrammatorView.code_labels[i];
				text += "}";
				break;
			case 200:
				text += "RESTART;";
				break;
			}
			if (num == ProgrammatorView.COLS - 1)
			{
				text += "\n";
			}
			if (num == ProgrammatorView.COLS - 1 && num2 == ProgrammatorView.ROWS - 1)
			{
				text += "~";
			}
		}
		text += "$";
		for (int j = 0; j < ProgrammatorView.COLS; j++)
		{
			text = text.Replace(" \n", "\n");
		}
		for (int k = 0; k < ProgrammatorView.ROWS; k++)
		{
			text = text.Replace("\n~", "~");
		}
		for (int l = 0; l < ProgrammatorView.PAGES; l++)
		{
			text = text.Replace("~$", "$");
		}
		text = text.Replace("\n\n\n\n\n\n\n\n\n\n\n", "\n.0.\n");
		text = text.Replace("\n\n\n\n\n\n\n\n\n\n", "\n.9.\n");
		text = text.Replace("\n\n\n\n\n\n\n\n\n", "\n.8.\n");
		text = text.Replace("\n\n\n\n\n\n\n\n", "\n.7.\n");
		text = text.Replace("\n\n\n\n\n\n\n", "\n.6.\n");
		text = text.Replace("\n\n\n\n\n\n", "\n.5.\n");
		text = text.Replace("\n\n\n\n\n", "\n...\n");
		text = text.Replace("\n\n\n\n", "\n..\n");
		text = text.Replace("\n\n\n", "\n.\n");
		text = text.Replace("~", "~\n");
		text = text.Replace("   ", "_");
		text = text.Replace("$", "");
		return "$" + text;
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00024058 File Offset: 0x00022258
	static ProgrammatorView()
	{
		ProgrammatorView.PAGES = 16;
		ProgrammatorView.codes = new int[ProgrammatorView.PAGES * ProgrammatorView.TOKENS];
		ProgrammatorView.nums = new int[ProgrammatorView.PAGES * ProgrammatorView.TOKENS];
		ProgrammatorView.code_labels = new string[ProgrammatorView.PAGES * ProgrammatorView.TOKENS];
		ProgrammatorView.actions = new ProgAction[ProgrammatorView.TOKENS];
		ProgrammatorView.active = false;
		ProgrammatorView.programId = 0;
		ProgrammatorView.title = "";
	}

	// Token: 0x06000231 RID: 561 RVA: 0x000240FC File Offset: 0x000222FC
	private string NewProgramm(char ch2, ref int Lint1)
	{
		int num = 0;
		while (num < 13 && Lint1 + num < this.str200.Length)
		{
			if (this.str200[Lint1 + num] == ch2)
			{
				string result = this.str200.Substring(Lint1, num);
				Lint1 += num + 1;
				return result;
			}
			num++;
		}
		Lint1++;
		return "";
	}

	// Token: 0x04000382 RID: 898
	public GameObject iconPrefab;

	// Token: 0x04000383 RID: 899
	public GameObject iconContainer;

	// Token: 0x04000384 RID: 900
	public StringSelectorScript pageSelector;

	// Token: 0x04000385 RID: 901
	public Text titleTF;

	// Token: 0x04000386 RID: 902
	public static bool opened = false;

	// Token: 0x04000387 RID: 903
	public static bool unsaved = false;

	// Token: 0x04000388 RID: 904
	public const int EMPTY = 0;

	// Token: 0x04000389 RID: 905
	public const int BACK = 1;

	// Token: 0x0400038A RID: 906
	public const int START = 2;

	// Token: 0x0400038B RID: 907
	public const int END = 3;

	// Token: 0x0400038C RID: 908
	public const int MOVE_W = 4;

	// Token: 0x0400038D RID: 909
	public const int MOVE_A = 5;

	// Token: 0x0400038E RID: 910
	public const int MOVE_S = 6;

	// Token: 0x0400038F RID: 911
	public const int MOVE_D = 7;

	// Token: 0x04000390 RID: 912
	public const int DIGG = 8;

	// Token: 0x04000391 RID: 913
	public const int LOOK_W = 9;

	// Token: 0x04000392 RID: 914
	public const int LOOK_A = 10;

	// Token: 0x04000393 RID: 915
	public const int LOOK_S = 11;

	// Token: 0x04000394 RID: 916
	public const int LOOK_D = 12;

	// Token: 0x04000395 RID: 917
	public const int LAST = 13;

	// Token: 0x04000396 RID: 918
	public const int MOVE_F = 14;

	// Token: 0x04000397 RID: 919
	public const int ROTATE_CCW = 15;

	// Token: 0x04000398 RID: 920
	public const int ROTATE_CW = 16;

	// Token: 0x04000399 RID: 921
	public const int ACTION_BUILD = 17;

	// Token: 0x0400039A RID: 922
	public const int ACTION_GEO = 18;

	// Token: 0x0400039B RID: 923
	public const int ACTION_ROAD = 19;

	// Token: 0x0400039C RID: 924
	public const int ACTION_HEAL = 20;

	// Token: 0x0400039D RID: 925
	public const int ACTION_QUADRO = 21;

	// Token: 0x0400039E RID: 926
	public const int ACTION_RANDOM = 22;

	// Token: 0x0400039F RID: 927
	public const int ACTION_BIBIKA = 23;

	// Token: 0x040003A0 RID: 928
	public const int ACTION_B1 = 162;

	// Token: 0x040003A1 RID: 929
	public const int ACTION_B3 = 163;

	// Token: 0x040003A2 RID: 930
	public const int ACTION_B2 = 164;

	// Token: 0x040003A3 RID: 931
	public const int ACTION_WB = 165;

	// Token: 0x040003A4 RID: 932
	public const int ACTION_GEOPACK = 167;

	// Token: 0x040003A5 RID: 933
	public const int ACTION_ZM = 168;

	// Token: 0x040003A6 RID: 934
	public const int ACTION_C190 = 169;

	// Token: 0x040003A7 RID: 935
	public const int ACTION_POLY = 170;

	// Token: 0x040003A8 RID: 936
	public const int ACTION_UP = 171;

	// Token: 0x040003A9 RID: 937
	public const int ACTION_CRAFT = 172;

	// Token: 0x040003AA RID: 938
	public const int ACTION_NANO = 173;

	// Token: 0x040003AB RID: 939
	public const int ACTION_REMBOT = 174;

	// Token: 0x040003AC RID: 940
	public const int INVDIR_W = 175;

	// Token: 0x040003AD RID: 941
	public const int INVDIR_A = 176;

	// Token: 0x040003AE RID: 942
	public const int INVDIR_S = 177;

	// Token: 0x040003AF RID: 943
	public const int INVDIR_D = 178;

	// Token: 0x040003B0 RID: 944
	public const int LABEL = 40;

	// Token: 0x040003B1 RID: 945
	public const int GOTO = 24;

	// Token: 0x040003B2 RID: 946
	public const int GOSUB = 25;

	// Token: 0x040003B3 RID: 947
	public const int GOSUB1 = 26;

	// Token: 0x040003B4 RID: 948
	public const int GOSUBF = 137;

	// Token: 0x040003B5 RID: 949
	public const int RETURN = 27;

	// Token: 0x040003B6 RID: 950
	public const int RETURN1 = 28;

	// Token: 0x040003B7 RID: 951
	public const int RETURNF = 138;

	// Token: 0x040003B8 RID: 952
	public const int CELL_W = 31;

	// Token: 0x040003B9 RID: 953
	public const int CELL_WA = 29;

	// Token: 0x040003BA RID: 954
	public const int CELL_A = 33;

	// Token: 0x040003BB RID: 955
	public const int CELL_AS = 36;

	// Token: 0x040003BC RID: 956
	public const int CELL_S = 37;

	// Token: 0x040003BD RID: 957
	public const int CELL_SD = 30;

	// Token: 0x040003BE RID: 958
	public const int CELL_D = 35;

	// Token: 0x040003BF RID: 959
	public const int CELL_DW = 32;

	// Token: 0x040003C0 RID: 960
	public const int CELL_WW = 131;

	// Token: 0x040003C1 RID: 961
	public const int CELL_AA = 132;

	// Token: 0x040003C2 RID: 962
	public const int CELL_SS = 133;

	// Token: 0x040003C3 RID: 963
	public const int CELL_DD = 134;

	// Token: 0x040003C4 RID: 964
	public const int CELL_F = 135;

	// Token: 0x040003C5 RID: 965
	public const int CELL_FF = 136;

	// Token: 0x040003C6 RID: 966
	public const int CC_NOTEMPTY = 43;

	// Token: 0x040003C7 RID: 967
	public const int CC_EMPTY = 44;

	// Token: 0x040003C8 RID: 968
	public const int CC_GRAVITY = 45;

	// Token: 0x040003C9 RID: 969
	public const int CC_CRYSTALL = 46;

	// Token: 0x040003CA RID: 970
	public const int CC_ALIVE = 47;

	// Token: 0x040003CB RID: 971
	public const int CC_BOLDER = 48;

	// Token: 0x040003CC RID: 972
	public const int CC_SAND = 49;

	// Token: 0x040003CD RID: 973
	public const int CC_ROCK = 50;

	// Token: 0x040003CE RID: 974
	public const int CC_DEAD = 51;

	// Token: 0x040003CF RID: 975
	public const int CCC_REDROCK = 52;

	// Token: 0x040003D0 RID: 976
	public const int CCC_BLACKROCK = 53;

	// Token: 0x040003D1 RID: 977
	public const int CC_ACID = 54;

	// Token: 0x040003D2 RID: 978
	public const int CCC_QUADRO = 57;

	// Token: 0x040003D3 RID: 979
	public const int CCC_ROAD = 58;

	// Token: 0x040003D4 RID: 980
	public const int CCC_REDBLOCK = 59;

	// Token: 0x040003D5 RID: 981
	public const int CCC_YELLOWBLOCK = 60;

	// Token: 0x040003D6 RID: 982
	public const int CCC_GREENBLOCK = 77;

	// Token: 0x040003D7 RID: 983
	public const int CCC_OPOR = 76;

	// Token: 0x040003D8 RID: 984
	public const int CCC_BOX = 74;

	// Token: 0x040003D9 RID: 985
	public const int BOOLMODE_OR = 38;

	// Token: 0x040003DA RID: 986
	public const int BOOLMODE_AND = 39;

	// Token: 0x040003DB RID: 987
	public const int MODE_AUTODIGG_ON = 158;

	// Token: 0x040003DC RID: 988
	public const int MODE_AUTODIGG_OFF = 159;

	// Token: 0x040003DD RID: 989
	public const int MODE_AGR_ON = 160;

	// Token: 0x040003DE RID: 990
	public const int MODE_AGR_OFF = 161;

	// Token: 0x040003DF RID: 991
	public const int VAR_LESS = 120;

	// Token: 0x040003E0 RID: 992
	public const int VAR_MORE = 119;

	// Token: 0x040003E1 RID: 993
	public const int VAR_EQUAL = 123;

	// Token: 0x040003E2 RID: 994
	public const int IF_NOT_GOTO = 139;

	// Token: 0x040003E3 RID: 995
	public const int IF_GOTO = 140;

	// Token: 0x040003E4 RID: 996
	public const int STD_DIGG = 141;

	// Token: 0x040003E5 RID: 997
	public const int STD_BUILD = 142;

	// Token: 0x040003E6 RID: 998
	public const int STD_HEAL = 143;

	// Token: 0x040003E7 RID: 999
	public const int STD_MINE = 145;

	// Token: 0x040003E8 RID: 1000
	public const int CELL_RIGHT_HAND = 156;

	// Token: 0x040003E9 RID: 1001
	public const int CELL_LEFT_HAND = 157;

	// Token: 0x040003EA RID: 1002
	public const int CC_GUN = 146;

	// Token: 0x040003EB RID: 1003
	public const int FILL_GUN = 147;

	// Token: 0x040003EC RID: 1004
	public const int CB_HP = 148;

	// Token: 0x040003ED RID: 1005
	public const int CB_HP50 = 149;

	// Token: 0x040003EE RID: 1006
	public const int PROG_FLIP = 144;

	// Token: 0x040003EF RID: 1007
	public const int ON_RESP = 166;

	// Token: 0x040003F0 RID: 1008
	public const int RESTART = 200;

	// Token: 0x040003F1 RID: 1009
	public const int HANDMODE_ON = 179;

	// Token: 0x040003F2 RID: 1010
	public const int HANDMODE_OFF = 180;

	// Token: 0x040003F3 RID: 1011
	public const int DEBUG_BREAK = 181;

	// Token: 0x040003F4 RID: 1012
	public const int DEBUG_SET = 182;

	// Token: 0x040003F5 RID: 1013
	private int[] c_button = new int[]
	{
		43,
		44,
		45,
		46,
		47,
		48,
		49,
		50,
		51,
		54
	};

	// Token: 0x040003F6 RID: 1014
	private int[] shc_button = new int[]
	{
		53,
		52,
		77,
		60,
		59,
		76,
		57,
		58,
		74
	};

	// Token: 0x040003F7 RID: 1015
	private int[] w_button = new int[]
	{
		4,
		9,
		175
	};

	// Token: 0x040003F8 RID: 1016
	private int[] a_button = new int[]
	{
		5,
		10,
		176
	};

	// Token: 0x040003F9 RID: 1017
	private int[] s_button = new int[]
	{
		6,
		11,
		177
	};

	// Token: 0x040003FA RID: 1018
	private int[] d_button = new int[]
	{
		7,
		12,
		178
	};

	// Token: 0x040003FB RID: 1019
	private int[] b_button = new int[]
	{
		23,
		181,
		182
	};

	// Token: 0x040003FC RID: 1020
	private int[] shb_button = new int[]
	{
		179,
		180
	};

	// Token: 0x040003FD RID: 1021
	private int[] m_button = new int[]
	{
		158,
		159,
		160,
		161
	};

	// Token: 0x040003FE RID: 1022
	private int[] shw_button = new int[]
	{
		31,
		33,
		29,
		31,
		35,
		32,
		31,
		131
	};

	// Token: 0x040003FF RID: 1023
	private int[] sha_button = new int[]
	{
		33,
		31,
		29,
		33,
		37,
		36,
		33,
		132
	};

	// Token: 0x04000400 RID: 1024
	private int[] shs_button = new int[]
	{
		37,
		33,
		36,
		37,
		35,
		30,
		37,
		133
	};

	// Token: 0x04000401 RID: 1025
	private int[] shd_button = new int[]
	{
		35,
		37,
		30,
		35,
		31,
		32,
		35,
		134
	};

	// Token: 0x04000402 RID: 1026
	private int[] z_button = new int[]
	{
		8,
		17,
		18,
		19,
		20,
		21
	};

	// Token: 0x04000403 RID: 1027
	private int[] shz_button = new int[]
	{
		165,
		162,
		164,
		163
	};

	// Token: 0x04000404 RID: 1028
	private int[] x_button = new int[]
	{
		167,
		168,
		170,
		169
	};

	// Token: 0x04000405 RID: 1029
	private int[] shx_button = new int[]
	{
		172,
		171,
		173,
		174
	};

	// Token: 0x04000406 RID: 1030
	private int[] f_button = new int[]
	{
		14
	};

	// Token: 0x04000407 RID: 1031
	private int[] shf_button = new int[]
	{
		135,
		136,
		156,
		157
	};

	// Token: 0x04000408 RID: 1032
	private int[] e_button = new int[]
	{
		2,
		3,
		166
	};

	// Token: 0x04000409 RID: 1033
	private int[] r_button = new int[]
	{
		15,
		16,
		22
	};

	// Token: 0x0400040A RID: 1034
	private int[] l_button = new int[]
	{
		40
	};

	// Token: 0x0400040B RID: 1035
	private int[] y_button = new int[]
	{
		144
	};

	// Token: 0x0400040C RID: 1036
	private int[] o_button = new int[]
	{
		38,
		39
	};

	// Token: 0x0400040D RID: 1037
	private int[] i_button = new int[]
	{
		139,
		140
	};

	// Token: 0x0400040E RID: 1038
	private int[] del_button = new int[1];

	// Token: 0x0400040F RID: 1039
	private int[] back_button;

	// Token: 0x04000410 RID: 1040
	private int[] t_button;

	// Token: 0x04000411 RID: 1041
	private int[] v_button;

	// Token: 0x04000412 RID: 1042
	private int[] g_button;

	// Token: 0x04000413 RID: 1043
	private int[] h_button;

	// Token: 0x04000414 RID: 1044
	private int[] j_button;

	// Token: 0x04000415 RID: 1045
	private int[] q_button;

	// Token: 0x04000416 RID: 1046
	public static ProgrammatorView THIS;

	// Token: 0x04000417 RID: 1047
	private static int ROWS = 12;

	// Token: 0x04000418 RID: 1048
	private static int COLS = 16;

	// Token: 0x04000419 RID: 1049
	private static int PAGES;

	// Token: 0x0400041A RID: 1050
	public static int[] codes;

	// Token: 0x0400041B RID: 1051
	public static int[] nums;

	// Token: 0x0400041C RID: 1052
	public static string[] code_labels;

	// Token: 0x0400041D RID: 1053
	public static ProgAction[] actions;

	// Token: 0x0400041E RID: 1054
	public static bool active;

	// Token: 0x0400041F RID: 1055
	public static int programId;

	// Token: 0x04000420 RID: 1056
	public static string title;

	// Token: 0x04000421 RID: 1057
	private int prevPage;

	// Token: 0x04000422 RID: 1058
	private int position;

	// Token: 0x04000423 RID: 1059
	private Dictionary<string, int> labels;

	// Token: 0x04000424 RID: 1060
	private string str200;

	// Token: 0x04000425 RID: 1061
	private string[] strmas2;

	// Token: 0x04000426 RID: 1062
	private string str201;

	// Token: 0x04000427 RID: 1063
	private string str202;

	// Token: 0x04000428 RID: 1064
	private string str203;

	// Token: 0x04000429 RID: 1065
	private static int TOKENS = ProgrammatorView.ROWS * ProgrammatorView.COLS;

	// Token: 0x0400042A RID: 1066
	private RuntimeCommandSelector commandSelector;
}
