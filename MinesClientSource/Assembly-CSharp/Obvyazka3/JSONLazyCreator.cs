using System;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x02000083 RID: 131
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600033D RID: 829 RVA: 0x00007047 File Offset: 0x00005247
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.None;
			}
		}

		// Token: 0x1700001D RID: 29
		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				JSONArray jsonarray = new JSONArray();
				jsonarray.Add(value);
				this.Set(jsonarray);
			}
		}

		// Token: 0x1700001E RID: 30
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.Add(aKey, value);
				this.Set(jsonobject);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000342 RID: 834 RVA: 0x0002E178 File Offset: 0x0002C378
		// (set) Token: 0x06000343 RID: 835 RVA: 0x0002E19C File Offset: 0x0002C39C
		public override int AsInt
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber((double)value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000344 RID: 836 RVA: 0x0002E1B8 File Offset: 0x0002C3B8
		// (set) Token: 0x06000345 RID: 837 RVA: 0x0002E19C File Offset: 0x0002C39C
		public override float AsFloat
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0f;
			}
			set
			{
				JSONNumber aVal = new JSONNumber((double)value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000346 RID: 838 RVA: 0x0002E1E0 File Offset: 0x0002C3E0
		// (set) Token: 0x06000347 RID: 839 RVA: 0x0002E20C File Offset: 0x0002C40C
		public override double AsDouble
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0.0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber(value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000348 RID: 840 RVA: 0x0002E228 File Offset: 0x0002C428
		// (set) Token: 0x06000349 RID: 841 RVA: 0x0002E244 File Offset: 0x0002C444
		public override bool AsBool
		{
			get
			{
				JSONBool aVal = new JSONBool(false);
				this.Set(aVal);
				return false;
			}
			set
			{
				JSONBool aVal = new JSONBool(value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600034A RID: 842 RVA: 0x0002E260 File Offset: 0x0002C460
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0002E27C File Offset: 0x0002C47C
		public override JSONObject AsObject
		{
			get
			{
				JSONObject jsonobject = new JSONObject();
				this.Set(jsonobject);
				return jsonobject;
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0002E118 File Offset: 0x0002C318
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00007053 File Offset: 0x00005253
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00007069 File Offset: 0x00005269
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000707F File Offset: 0x0000527F
		private void Set(JSONNode aVal)
		{
			if (this.m_Key == null)
			{
				this.m_Node.Add(aVal);
			}
			else
			{
				this.m_Node.Add(this.m_Key, aVal);
			}
			this.m_Node = null;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0002E298 File Offset: 0x0002C498
		public override void Add(JSONNode aItem)
		{
			JSONArray jsonarray = new JSONArray();
			jsonarray.Add(aItem);
			this.Set(jsonarray);
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0002E154 File Offset: 0x0002C354
		public override void Add(string aKey, JSONNode aItem)
		{
			JSONObject jsonobject = new JSONObject();
			jsonobject.Add(aKey, aItem);
			this.Set(jsonobject);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000070B0 File Offset: 0x000052B0
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000070BB File Offset: 0x000052BB
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000070B0 File Offset: 0x000052B0
		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x000070C7 File Offset: 0x000052C7
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x000070CA File Offset: 0x000052CA
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		// Token: 0x040005B1 RID: 1457
		private JSONNode m_Node;

		// Token: 0x040005B2 RID: 1458
		private string m_Key;
	}
}
