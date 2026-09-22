using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x0200008F RID: 143
	public class JSONObject : JSONNode
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x000074FE File Offset: 0x000056FE
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x00007506 File Offset: 0x00005706
		public override bool Inline
		{
			get
			{
				return this.inline;
			}
			set
			{
				this.inline = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060003DA RID: 986 RVA: 0x0000750F File Offset: 0x0000570F
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Object;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060003DB RID: 987 RVA: 0x00006E6D File Offset: 0x0000506D
		public override bool IsObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000052 RID: 82
		public override JSONNode this[string aKey]
		{
			get
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					return this.m_Dict[aKey];
				}
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = value;
					return;
				}
				this.m_Dict.Add(aKey, value);
			}
		}

		// Token: 0x17000053 RID: 83
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return null;
				}
				return this.m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				if (aIndex >= 0 && aIndex < this.m_Dict.Count)
				{
					string key = this.m_Dict.ElementAt(aIndex).Key;
					this.m_Dict[key] = value;
				}
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x00007571 File Offset: 0x00005771
		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x0000757E File Offset: 0x0000577E
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
				{
					yield return keyValuePair.Value;
				}
				Dictionary<string, JSONNode>.Enumerator enumerator = default(Dictionary<string, JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0000758E File Offset: 0x0000578E
		public override JSONNode.Enumerator GetEnumerator()
		{
			return new JSONNode.Enumerator(this.m_Dict.GetEnumerator());
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0002ECF0 File Offset: 0x0002CEF0
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = JSONNull.CreateOrGet();
			}
			if (string.IsNullOrEmpty(aKey))
			{
				this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
				return;
			}
			if (this.m_Dict.ContainsKey(aKey))
			{
				this.m_Dict[aKey] = aItem;
				return;
			}
			this.m_Dict.Add(aKey, aItem);
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x000075A0 File Offset: 0x000057A0
		public override JSONNode Remove(string aKey)
		{
			if (!this.m_Dict.ContainsKey(aKey))
			{
				return null;
			}
			JSONNode result = this.m_Dict[aKey];
			this.m_Dict.Remove(aKey);
			return result;
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0002ED60 File Offset: 0x0002CF60
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_Dict.Count)
			{
				return null;
			}
			KeyValuePair<string, JSONNode> keyValuePair = this.m_Dict.ElementAt(aIndex);
			this.m_Dict.Remove(keyValuePair.Key);
			return keyValuePair.Value;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0002EDA8 File Offset: 0x0002CFA8
		public override JSONNode Remove(JSONNode aNode)
		{
			JSONNode result;
			try
			{
				KeyValuePair<string, JSONNode> keyValuePair = this.m_Dict.Where(delegate(KeyValuePair<string, JSONNode> k)
				{
					KeyValuePair<string, JSONNode> keyValuePair2 = k;
					return keyValuePair2.Value == aNode;
				}).First<KeyValuePair<string, JSONNode>>();
				this.m_Dict.Remove(keyValuePair.Key);
				result = aNode;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0002EE14 File Offset: 0x0002D014
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('{');
			bool flag = true;
			if (this.inline)
			{
				aMode = JSONTextMode.Compact;
			}
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				if (!flag)
				{
					aSB.Append(',');
				}
				flag = false;
				if (aMode == JSONTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				aSB.Append('"').Append(JSONNode.Escape(keyValuePair.Key)).Append('"');
				if (aMode == JSONTextMode.Compact)
				{
					aSB.Append(':');
				}
				else
				{
					aSB.Append(" : ");
				}
				keyValuePair.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JSONTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append('}');
		}

		// Token: 0x040005D5 RID: 1493
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

		// Token: 0x040005D6 RID: 1494
		private bool inline;
	}
}
