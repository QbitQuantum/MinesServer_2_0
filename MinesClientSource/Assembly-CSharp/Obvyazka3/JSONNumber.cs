using System;
using System.Text;

namespace Obvyazka3
{
	// Token: 0x0200008E RID: 142
	public class JSONNumber : JSONNode
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060003CB RID: 971 RVA: 0x000074B2 File Offset: 0x000056B2
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060003CC RID: 972 RVA: 0x00006E6D File Offset: 0x0000506D
		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060003CD RID: 973 RVA: 0x000074B5 File Offset: 0x000056B5
		// (set) Token: 0x060003CE RID: 974 RVA: 0x0002EB88 File Offset: 0x0002CD88
		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				double data;
				if (double.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060003CF RID: 975 RVA: 0x000074C2 File Offset: 0x000056C2
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x000074CA File Offset: 0x000056CA
		public override double AsDouble
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

		// Token: 0x060003D1 RID: 977 RVA: 0x0002E118 File Offset: 0x0002C318
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x000074D3 File Offset: 0x000056D3
		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00006FEF File Offset: 0x000051EF
		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000074E2 File Offset: 0x000056E2
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0002EBA8 File Offset: 0x0002CDA8
		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0002EC10 File Offset: 0x0002CE10
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JSONNumber jsonnumber = obj as JSONNumber;
			if (jsonnumber != null)
			{
				return this.m_Data == jsonnumber.m_Data;
			}
			return JSONNumber.IsNumeric(obj) && Convert.ToDouble(obj) == this.m_Data;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x000074F1 File Offset: 0x000056F1
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x040005D4 RID: 1492
		private double m_Data;
	}
}
