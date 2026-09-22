using System;
using System.Collections.Generic;

namespace HealthCalculator
{
	// Token: 0x020000A4 RID: 164
	public static class HealthCalculator
	{
		// Token: 0x060004CD RID: 1229 RVA: 0x00007F47 File Offset: 0x00006147
		public static void Add(int bid)
		{
			if (!HealthCalculator.states.ContainsKey(bid))
			{
				HealthCalculator.states.Add(bid, new HealthCalculator.HealthState());
			}
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00007F66 File Offset: 0x00006166
		public static void Remove(int bid)
		{
			if (HealthCalculator.states.ContainsKey(bid) && HealthCalculator.states[bid].HasPrediction())
			{
				HealthCalculator.log.Add(new KeyValuePair<int, HealthCalculator.HealthState>(bid, HealthCalculator.states[bid]));
			}
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0003276C File Offset: 0x0003096C
		public static void Healed(int bid)
		{
			if (HealthCalculator.states.ContainsKey(bid))
			{
				if (HealthCalculator.states[bid].HasPrediction())
				{
					HealthCalculator.log.Add(new KeyValuePair<int, HealthCalculator.HealthState>(bid, HealthCalculator.states[bid]));
				}
				HealthCalculator.states[bid] = new HealthCalculator.HealthState();
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x00007FA2 File Offset: 0x000061A2
		public static void SetHP(int bid, int percent)
		{
			if (HealthCalculator.states.ContainsKey(bid))
			{
				HealthCalculator.states[bid].SetPercent(percent);
			}
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00007FC2 File Offset: 0x000061C2
		public static void Damage(int bid)
		{
			if (HealthCalculator.states.ContainsKey(bid))
			{
				HealthCalculator.states[bid].AddDamage(HealthCalculator.damage);
			}
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00007FE6 File Offset: 0x000061E6
		public static void SetDamageScale(int dmg)
		{
			HealthCalculator.damage = dmg;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x00007FEE File Offset: 0x000061EE
		public static float GetMaxPrediction(int bid)
		{
			if (HealthCalculator.states.ContainsKey(bid))
			{
				return HealthCalculator.states[bid].maxHealthPrediction;
			}
			return -1f;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x00008013 File Offset: 0x00006213
		public static float GetCurrentPrediction(int bid)
		{
			if (HealthCalculator.states.ContainsKey(bid))
			{
				return HealthCalculator.states[bid].currentHealthPrediction;
			}
			return -1f;
		}

		// Token: 0x04000650 RID: 1616
		public static Dictionary<int, HealthCalculator.HealthState> states = new Dictionary<int, HealthCalculator.HealthState>();

		// Token: 0x04000651 RID: 1617
		public static int damage = 1;

		// Token: 0x04000652 RID: 1618
		public static List<KeyValuePair<int, HealthCalculator.HealthState>> log = new List<KeyValuePair<int, HealthCalculator.HealthState>>();

		// Token: 0x020000A5 RID: 165
		public class HealthState
		{
			// Token: 0x060004D6 RID: 1238 RVA: 0x000327C4 File Offset: 0x000309C4
			public void AddDamage(int dmg)
			{
				this.damageDealt += dmg;
				this.tmpDmgPerPercent += dmg;
				float num = this.currentPercent - 1f / (float)this.dmgPerPercent * (float)this.tmpDmgPerPercent;
				float num2 = 100f * (float)this.damageDealt / (this.startPercent - num);
				if (this.maxHealthPrediction < num2)
				{
					this.maxHealthPrediction = num2;
				}
				this.currentHealthPrediction = (float)((int)(num / 100f * this.maxHealthPrediction));
			}

			// Token: 0x060004D7 RID: 1239 RVA: 0x00032848 File Offset: 0x00030A48
			public void SetPercent(int percent)
			{
				if (this.startPercent == -1f)
				{
					this.startPercent = (float)percent;
				}
				if (this.currentPercent != (float)percent)
				{
					if (this.dmgPerPercent < this.tmpDmgPerPercent)
					{
						this.dmgPerPercent = this.tmpDmgPerPercent;
					}
					this.tmpDmgPerPercent = 1;
				}
				this.currentPercent = (float)percent;
			}

			// Token: 0x060004D8 RID: 1240 RVA: 0x00008054 File Offset: 0x00006254
			public bool HasPrediction()
			{
				return this.startPercent > this.currentPercent && this.maxHealthPrediction != -1f && this.currentHealthPrediction != -1f;
			}

			// Token: 0x04000653 RID: 1619
			public int damageDealt;

			// Token: 0x04000654 RID: 1620
			public float currentHealthPrediction = -1f;

			// Token: 0x04000655 RID: 1621
			public float maxHealthPrediction = -1f;

			// Token: 0x04000656 RID: 1622
			public float startPercent = -1f;

			// Token: 0x04000657 RID: 1623
			public float currentPercent = -1f;

			// Token: 0x04000658 RID: 1624
			public int dmgPerPercent = 1;

			// Token: 0x04000659 RID: 1625
			public int tmpDmgPerPercent = 1;
		}
	}
}
