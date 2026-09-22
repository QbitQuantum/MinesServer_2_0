using System;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x02000082 RID: 130
	public class JSONBool : JSONNode
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000331 RID: 817 RVA: 0x00006FBF File Offset: 0x000051BF
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00006E6D File Offset: 0x0000506D
		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000333 RID: 819 RVA: 0x00006FC2 File Offset: 0x000051C2
		// (set) Token: 0x06000334 RID: 820 RVA: 0x0002E0F8 File Offset: 0x0002C2F8
		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				bool data;
				if (bool.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000335 RID: 821 RVA: 0x00006FCF File Offset: 0x000051CF
		// (set) Token: 0x06000336 RID: 822 RVA: 0x00006FD7 File Offset: 0x000051D7
		public override bool AsBool
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0002E118 File Offset: 0x0002C318
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00006FE0 File Offset: 0x000051E0
		public JSONBool(bool aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00006FEF File Offset: 0x000051EF
		public JSONBool(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00006FFE File Offset: 0x000051FE
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data ? "true" : "false");
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000701B File Offset: 0x0000521B
		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && this.m_Data == (bool)obj;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000703A File Offset: 0x0000523A
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x040005B0 RID: 1456
		private bool m_Data;
	}
}
