using System;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x02000092 RID: 146
	public class JSONString : JSONNode
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00007622 File Offset: 0x00005822
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.String;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x00006E6D File Offset: 0x0000506D
		public override bool IsString
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00007625 File Offset: 0x00005825
		// (set) Token: 0x060003F7 RID: 1015 RVA: 0x0000762D File Offset: 0x0000582D
		public override string Value
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

		// Token: 0x060003F8 RID: 1016 RVA: 0x0002E118 File Offset: 0x0002C318
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00007636 File Offset: 0x00005836
		public JSONString(string aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00007645 File Offset: 0x00005845
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('"').Append(JSONNode.Escape(this.m_Data)).Append('"');
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0002F064 File Offset: 0x0002D264
		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			string text = obj as string;
			if (text != null)
			{
				return this.m_Data == text;
			}
			JSONString jsonstring = obj as JSONString;
			return jsonstring != null && this.m_Data == jsonstring.m_Data;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00007667 File Offset: 0x00005867
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x040005DD RID: 1501
		private string m_Data;
	}
}
