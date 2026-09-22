using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyUI
{
	// Token: 0x020000A3 RID: 163
	internal static class SetPropertyUtility
	{
		// Token: 0x060004CA RID: 1226 RVA: 0x000326CC File Offset: 0x000308CC
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00007F28 File Offset: 0x00006128
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0003271C File Offset: 0x0003091C
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
