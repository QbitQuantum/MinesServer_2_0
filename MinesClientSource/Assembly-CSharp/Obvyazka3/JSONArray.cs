using System;
using System.Collections.Generic;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x02000080 RID: 128
	public class JSONArray : JSONNode
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000318 RID: 792 RVA: 0x00006E5C File Offset: 0x0000505C
		// (set) Token: 0x06000319 RID: 793 RVA: 0x00006E64 File Offset: 0x00005064
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600031A RID: 794 RVA: 0x00006E6D File Offset: 0x0000506D
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Array;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600031B RID: 795 RVA: 0x00006E6D File Offset: 0x0000506D
		public override bool IsArray
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000012 RID: 18
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					return new JSONLazyCreator(this);
				}
				return this.m_List[aIndex];
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					this.m_List.Add(value);
					return;
				}
				this.m_List[aIndex] = value;
			}
		}

		// Token: 0x17000013 RID: 19
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				this.m_List.Add(value);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000320 RID: 800 RVA: 0x00006EFB File Offset: 0x000050FB
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000321 RID: 801 RVA: 0x00006F08 File Offset: 0x00005108
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (JSONNode jsonnode in this.m_List)
				{
					yield return jsonnode;
				}
				List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00006F18 File Offset: 0x00005118
		public override JSONNode.Enumerator GetEnumerator()
		{
			return new JSONNode.Enumerator(this.m_List.GetEnumerator());
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00006EDD File Offset: 0x000050DD
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = JSONNull.CreateOrGet();
			}
			this.m_List.Add(aItem);
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00006F2A File Offset: 0x0000512A
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				return null;
			}
			JSONNode result = this.m_List[aIndex];
			this.m_List.RemoveAt(aIndex);
			return result;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00006F58 File Offset: 0x00005158
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0002DF38 File Offset: 0x0002C138
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('[');
			int count = this.m_List.Count;
			if (this.inline)
			{
				aMode = JSONTextMode.Compact;
			}
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					aSB.Append(',');
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				this.m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JSONTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append(']');
		}

		// Token: 0x040005A9 RID: 1449
		private List<JSONNode> m_List = new List<JSONNode>();

		// Token: 0x040005AA RID: 1450
		private bool inline;
	}
}
