using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x02000084 RID: 132
	public abstract class JSONNode
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000357 RID: 855
		public abstract JSONNodeType Tag { get; }

		// Token: 0x17000026 RID: 38
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000027 RID: 39
		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600035C RID: 860 RVA: 0x000070DB File Offset: 0x000052DB
		// (set) Token: 0x0600035D RID: 861 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600035E RID: 862 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600035F RID: 863 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000360 RID: 864 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000361 RID: 865 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000362 RID: 866 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000363 RID: 867 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000364 RID: 868 RVA: 0x000070C7 File Offset: 0x000052C7
		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000365 RID: 869 RVA: 0x000070C7 File Offset: 0x000052C7
		// (set) Token: 0x06000366 RID: 870 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000367 RID: 871 RVA: 0x000070E2 File Offset: 0x000052E2
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000368 RID: 872 RVA: 0x000070EB File Offset: 0x000052EB
		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode jsonnode in this.Children)
				{
					foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
					{
						yield return jsonnode2;
					}
					IEnumerator<JSONNode> enumerator2 = null;
				}
				IEnumerator<JSONNode> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000369 RID: 873 RVA: 0x000070FB File Offset: 0x000052FB
		public IEnumerable<KeyValuePair<string, JSONNode>> Linq
		{
			get
			{
				return new JSONNode.LinqEnumerator(this);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600036A RID: 874 RVA: 0x00007103 File Offset: 0x00005303
		public JSONNode.KeyEnumerator Keys
		{
			get
			{
				return new JSONNode.KeyEnumerator(this.GetEnumerator());
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600036B RID: 875 RVA: 0x00007110 File Offset: 0x00005310
		public JSONNode.ValueEnumerator Values
		{
			get
			{
				return new JSONNode.ValueEnumerator(this.GetEnumerator());
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600036C RID: 876 RVA: 0x0002E2BC File Offset: 0x0002C4BC
		// (set) Token: 0x0600036D RID: 877 RVA: 0x0000711D File Offset: 0x0000531D
		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600036E RID: 878 RVA: 0x0000712C File Offset: 0x0000532C
		// (set) Token: 0x0600036F RID: 879 RVA: 0x00007135 File Offset: 0x00005335
		public virtual int AsInt
		{
			get
			{
				return (int)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000370 RID: 880 RVA: 0x0000713F File Offset: 0x0000533F
		// (set) Token: 0x06000371 RID: 881 RVA: 0x00007135 File Offset: 0x00005335
		public virtual float AsFloat
		{
			get
			{
				return (float)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000372 RID: 882 RVA: 0x0002E2F0 File Offset: 0x0002C4F0
		// (set) Token: 0x06000373 RID: 883 RVA: 0x00007148 File Offset: 0x00005348
		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(this.Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = (value ? "true" : "false");
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0000715F File Offset: 0x0000535F
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000375 RID: 885 RVA: 0x00007167 File Offset: 0x00005367
		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000376 RID: 886 RVA: 0x0000716F File Offset: 0x0000536F
		internal static StringBuilder EscapeBuilder
		{
			get
			{
				if (JSONNode.m_EscapeBuilder == null)
				{
					JSONNode.m_EscapeBuilder = new StringBuilder();
				}
				return JSONNode.m_EscapeBuilder;
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00007187 File Offset: 0x00005387
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000070D8 File Offset: 0x000052D8
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000070D8 File Offset: 0x000052D8
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00007195 File Offset: 0x00005395
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0002E320 File Offset: 0x0002C520
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0002E344 File Offset: 0x0002C544
		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		// Token: 0x0600037E RID: 894
		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		// Token: 0x0600037F RID: 895
		public abstract JSONNode.Enumerator GetEnumerator();

		// Token: 0x06000380 RID: 896 RVA: 0x00007198 File Offset: 0x00005398
		public static implicit operator JSONNode(string s)
		{
			return new JSONString(s);
		}

		// Token: 0x06000381 RID: 897 RVA: 0x000071A0 File Offset: 0x000053A0
		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x000071B3 File Offset: 0x000053B3
		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		// Token: 0x06000383 RID: 899 RVA: 0x000071BB File Offset: 0x000053BB
		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x000071D6 File Offset: 0x000053D6
		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x000071DF File Offset: 0x000053DF
		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x000071D6 File Offset: 0x000053D6
		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000071F6 File Offset: 0x000053F6
		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00007209 File Offset: 0x00005409
		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00007211 File Offset: 0x00005411
		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00007224 File Offset: 0x00005424
		public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
		{
			return aKeyValue.Value;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0002E368 File Offset: 0x0002C568
		public static bool operator ==(JSONNode a, object b)
		{
			if (a == b)
			{
				return true;
			}
			bool flag = a is JSONNull || a == null || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
			return (flag && flag2) || (!flag && a.Equals(b));
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000722D File Offset: 0x0000542D
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00007239 File Offset: 0x00005439
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0000723F File Offset: 0x0000543F
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0002E3C0 File Offset: 0x0002C5C0
		internal static string Escape(string aText)
		{
			StringBuilder escapeBuilder = JSONNode.EscapeBuilder;
			escapeBuilder.Length = 0;
			if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				escapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			int i = 0;
			while (i < aText.Length)
			{
				char c = aText[i];
				switch (c)
				{
				case '\b':
					escapeBuilder.Append("\\b");
					break;
				case '\t':
					escapeBuilder.Append("\\t");
					break;
				case '\n':
					escapeBuilder.Append("\\n");
					break;
				case '\v':
					goto IL_E2;
				case '\f':
					escapeBuilder.Append("\\f");
					break;
				case '\r':
					escapeBuilder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_E2;
						}
						escapeBuilder.Append("\\\\");
					}
					else
					{
						escapeBuilder.Append("\\\"");
					}
					break;
				}
				IL_121:
				i++;
				continue;
				IL_E2:
				if (c < ' ' || (JSONNode.forceASCII && c > '\u007f'))
				{
					ushort num = (ushort)c;
					escapeBuilder.Append("\\u").Append(num.ToString("X4"));
					goto IL_121;
				}
				escapeBuilder.Append(c);
				goto IL_121;
			}
			string result = escapeBuilder.ToString();
			escapeBuilder.Length = 0;
			return result;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0002E510 File Offset: 0x0002C710
		private static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}
			string a = token.ToLower();
			if (a == "false" || a == "true")
			{
				ctx.Add(tokenName, a == "true");
				return;
			}
			if (a == "null")
			{
				ctx.Add(tokenName, null);
				return;
			}
			double n;
			if (double.TryParse(token, out n))
			{
				ctx.Add(tokenName, n);
				return;
			}
			ctx.Add(tokenName, token);
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0002E5A4 File Offset: 0x0002C7A4
		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			bool flag = false;
			bool flag2 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				if (c <= ',')
				{
					if (c <= ' ')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
						case '\r':
							goto IL_33E;
						case '\v':
						case '\f':
							goto IL_330;
						default:
							if (c != ' ')
							{
								goto IL_330;
							}
							break;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
					}
					else if (c != '"')
					{
						if (c != ',')
						{
							goto IL_330;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
						}
						else
						{
							if (stringBuilder.Length > 0 || flag2)
							{
								JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							}
							text = "";
							stringBuilder.Length = 0;
							flag2 = false;
						}
					}
					else
					{
						flag = !flag;
						flag2 = (flag2 || flag);
					}
				}
				else
				{
					if (c <= ']')
					{
						if (c != ':')
						{
							switch (c)
							{
							case '[':
								if (flag)
								{
									stringBuilder.Append(aJSON[i]);
									goto IL_33E;
								}
								stack.Push(new JSONArray());
								if (jsonnode != null)
								{
									jsonnode.Add(text, stack.Peek());
								}
								text = "";
								stringBuilder.Length = 0;
								jsonnode = stack.Peek();
								goto IL_33E;
							case '\\':
								i++;
								if (flag)
								{
									char c2 = aJSON[i];
									if (c2 <= 'f')
									{
										if (c2 == 'b')
										{
											stringBuilder.Append('\b');
											goto IL_33E;
										}
										if (c2 == 'f')
										{
											stringBuilder.Append('\f');
											goto IL_33E;
										}
									}
									else
									{
										if (c2 == 'n')
										{
											stringBuilder.Append('\n');
											goto IL_33E;
										}
										switch (c2)
										{
										case 'r':
											stringBuilder.Append('\r');
											goto IL_33E;
										case 't':
											stringBuilder.Append('\t');
											goto IL_33E;
										case 'u':
										{
											string s = aJSON.Substring(i + 1, 4);
											stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
											i += 4;
											goto IL_33E;
										}
										}
									}
									stringBuilder.Append(c2);
									goto IL_33E;
								}
								goto IL_33E;
							case ']':
								break;
							default:
								goto IL_330;
							}
						}
						else
						{
							if (flag)
							{
								stringBuilder.Append(aJSON[i]);
								goto IL_33E;
							}
							text = stringBuilder.ToString();
							stringBuilder.Length = 0;
							flag2 = false;
							goto IL_33E;
						}
					}
					else if (c != '{')
					{
						if (c != '}')
						{
							goto IL_330;
						}
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
							goto IL_33E;
						}
						stack.Push(new JSONObject());
						if (jsonnode != null)
						{
							jsonnode.Add(text, stack.Peek());
						}
						text = "";
						stringBuilder.Length = 0;
						jsonnode = stack.Peek();
						goto IL_33E;
					}
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (stringBuilder.Length > 0 || flag2)
						{
							JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							flag2 = false;
						}
						text = "";
						stringBuilder.Length = 0;
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
				}
				IL_33E:
				i++;
				continue;
				IL_330:
				stringBuilder.Append(aJSON[i]);
				goto IL_33E;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		// Token: 0x040005B3 RID: 1459
		public static bool forceASCII;

		// Token: 0x040005B4 RID: 1460
		[ThreadStatic]
		private static StringBuilder m_EscapeBuilder;

		// Token: 0x02000085 RID: 133
		public struct Enumerator
		{
			// Token: 0x1700003D RID: 61
			// (get) Token: 0x06000393 RID: 915 RVA: 0x00007247 File Offset: 0x00005447
			public bool IsValid
			{
				get
				{
					return this.type > JSONNode.Enumerator.Type.None;
				}
			}

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x06000394 RID: 916 RVA: 0x0002E910 File Offset: 0x0002CB10
			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					if (this.type == JSONNode.Enumerator.Type.Array)
					{
						return new KeyValuePair<string, JSONNode>(string.Empty, this.m_Array.Current);
					}
					if (this.type == JSONNode.Enumerator.Type.Object)
					{
						return this.m_Object.Current;
					}
					return new KeyValuePair<string, JSONNode>(string.Empty, null);
				}
			}

			// Token: 0x06000395 RID: 917 RVA: 0x00007252 File Offset: 0x00005452
			public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this.type = JSONNode.Enumerator.Type.Array;
				this.m_Object = default(Dictionary<string, JSONNode>.Enumerator);
				this.m_Array = aArrayEnum;
			}

			// Token: 0x06000396 RID: 918 RVA: 0x0000726E File Offset: 0x0000546E
			public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this.type = JSONNode.Enumerator.Type.Object;
				this.m_Object = aDictEnum;
				this.m_Array = default(List<JSONNode>.Enumerator);
			}

			// Token: 0x06000397 RID: 919 RVA: 0x0000728A File Offset: 0x0000548A
			public bool MoveNext()
			{
				if (this.type == JSONNode.Enumerator.Type.Array)
				{
					return this.m_Array.MoveNext();
				}
				return this.type == JSONNode.Enumerator.Type.Object && this.m_Object.MoveNext();
			}

			// Token: 0x040005B5 RID: 1461
			private JSONNode.Enumerator.Type type;

			// Token: 0x040005B6 RID: 1462
			private Dictionary<string, JSONNode>.Enumerator m_Object;

			// Token: 0x040005B7 RID: 1463
			private List<JSONNode>.Enumerator m_Array;

			// Token: 0x02000086 RID: 134
			private enum Type
			{
				// Token: 0x040005B9 RID: 1465
				None,
				// Token: 0x040005BA RID: 1466
				Array,
				// Token: 0x040005BB RID: 1467
				Object
			}
		}

		// Token: 0x02000087 RID: 135
		public struct ValueEnumerator
		{
			// Token: 0x1700003F RID: 63
			// (get) Token: 0x06000398 RID: 920 RVA: 0x0002E95C File Offset: 0x0002CB5C
			public JSONNode Current
			{
				get
				{
					KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
					return keyValuePair.Value;
				}
			}

			// Token: 0x06000399 RID: 921 RVA: 0x000072B7 File Offset: 0x000054B7
			public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aArrayEnum));
			}

			// Token: 0x0600039A RID: 922 RVA: 0x000072CA File Offset: 0x000054CA
			public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aDictEnum));
			}

			// Token: 0x0600039B RID: 923 RVA: 0x000072DD File Offset: 0x000054DD
			public ValueEnumerator(JSONNode.Enumerator aEnumerator)
			{
				this.m_Enumerator = aEnumerator;
			}

			// Token: 0x0600039C RID: 924 RVA: 0x000072E6 File Offset: 0x000054E6
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			// Token: 0x0600039D RID: 925 RVA: 0x000072F3 File Offset: 0x000054F3
			public JSONNode.ValueEnumerator GetEnumerator()
			{
				return this;
			}

			// Token: 0x040005BC RID: 1468
			private JSONNode.Enumerator m_Enumerator;
		}

		// Token: 0x02000088 RID: 136
		public struct KeyEnumerator
		{
			// Token: 0x17000040 RID: 64
			// (get) Token: 0x0600039E RID: 926 RVA: 0x0002E97C File Offset: 0x0002CB7C
			public JSONNode Current
			{
				get
				{
					KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
					return keyValuePair.Key;
				}
			}

			// Token: 0x0600039F RID: 927 RVA: 0x000072FB File Offset: 0x000054FB
			public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aArrayEnum));
			}

			// Token: 0x060003A0 RID: 928 RVA: 0x0000730E File Offset: 0x0000550E
			public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aDictEnum));
			}

			// Token: 0x060003A1 RID: 929 RVA: 0x00007321 File Offset: 0x00005521
			public KeyEnumerator(JSONNode.Enumerator aEnumerator)
			{
				this.m_Enumerator = aEnumerator;
			}

			// Token: 0x060003A2 RID: 930 RVA: 0x0000732A File Offset: 0x0000552A
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			// Token: 0x060003A3 RID: 931 RVA: 0x00007337 File Offset: 0x00005537
			public JSONNode.KeyEnumerator GetEnumerator()
			{
				return this;
			}

			// Token: 0x040005BD RID: 1469
			private JSONNode.Enumerator m_Enumerator;
		}

		// Token: 0x02000089 RID: 137
		public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerator, IDisposable, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
		{
			// Token: 0x17000041 RID: 65
			// (get) Token: 0x060003A4 RID: 932 RVA: 0x0000733F File Offset: 0x0000553F
			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			// Token: 0x17000042 RID: 66
			// (get) Token: 0x060003A5 RID: 933 RVA: 0x0000734C File Offset: 0x0000554C
			object IEnumerator.Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			// Token: 0x060003A6 RID: 934 RVA: 0x0000735E File Offset: 0x0000555E
			internal LinqEnumerator(JSONNode aNode)
			{
				this.m_Node = aNode;
				if (this.m_Node != null)
				{
					this.m_Enumerator = this.m_Node.GetEnumerator();
				}
			}

			// Token: 0x060003A7 RID: 935 RVA: 0x0000738C File Offset: 0x0000558C
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			// Token: 0x060003A8 RID: 936 RVA: 0x00007399 File Offset: 0x00005599
			public void Dispose()
			{
				this.m_Node = null;
				this.m_Enumerator = default(JSONNode.Enumerator);
			}

			// Token: 0x060003A9 RID: 937 RVA: 0x000073AE File Offset: 0x000055AE
			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
			{
				return new JSONNode.LinqEnumerator(this.m_Node);
			}

			// Token: 0x060003AA RID: 938 RVA: 0x000073BB File Offset: 0x000055BB
			public void Reset()
			{
				if (this.m_Node != null)
				{
					this.m_Enumerator = this.m_Node.GetEnumerator();
				}
			}

			// Token: 0x060003AB RID: 939 RVA: 0x000073AE File Offset: 0x000055AE
			IEnumerator IEnumerable.GetEnumerator()
			{
				return new JSONNode.LinqEnumerator(this.m_Node);
			}

			// Token: 0x040005BE RID: 1470
			private JSONNode m_Node;

			// Token: 0x040005BF RID: 1471
			private JSONNode.Enumerator m_Enumerator;
		}
	}
}
