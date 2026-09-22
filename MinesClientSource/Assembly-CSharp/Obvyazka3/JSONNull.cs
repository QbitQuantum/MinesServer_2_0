using System;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x0200008D RID: 141
	public class JSONNull : JSONNode
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060003BE RID: 958 RVA: 0x00007469 File Offset: 0x00005669
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.NullValue;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060003BF RID: 959 RVA: 0x00006E6D File Offset: 0x0000506D
		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x0000746C File Offset: 0x0000566C
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x00004B5F File Offset: 0x00002D5F
		public override string Value
		{
			get
			{
				return "null";
			}
			set
			{
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x000070C7 File Offset: 0x000052C7
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x00004B5F File Offset: 0x00002D5F
		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00007473 File Offset: 0x00005673
		public static JSONNull CreateOrGet()
		{
			if (JSONNull.reuseSameInstance)
			{
				return JSONNull.m_StaticInstance;
			}
			return new JSONNull();
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00007487 File Offset: 0x00005687
		private JSONNull()
		{
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0002E118 File Offset: 0x0002C318
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0000748F File Offset: 0x0000568F
		public override bool Equals(object obj)
		{
			return this == obj || obj is JSONNull;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000070C7 File Offset: 0x000052C7
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x000070CA File Offset: 0x000052CA
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		// Token: 0x040005D2 RID: 1490
		private static JSONNull m_StaticInstance = new JSONNull();

		// Token: 0x040005D3 RID: 1491
		public static bool reuseSameInstance = true;
	}
}
