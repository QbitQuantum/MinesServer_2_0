using System;

namespace Obvyazka3
{
	// Token: 0x02000094 RID: 148
	public class ParameteredEvent<EventType>
	{
		// Token: 0x060003FD RID: 1021 RVA: 0x00007674 File Offset: 0x00005874
		public ParameteredEvent(string _name, EventType _msg)
		{
			this.name = _name;
			this.msg = _msg;
		}

		// Token: 0x040005E1 RID: 1505
		public string name;

		// Token: 0x040005E2 RID: 1506
		public EventType msg;
	}
}
