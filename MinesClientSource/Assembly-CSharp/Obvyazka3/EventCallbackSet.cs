using System;
using System.Collections.Generic;

namespace Obvyazka3
{
	// Token: 0x0200007C RID: 124
	public class EventCallbackSet<EventType>
	{
		// Token: 0x06000303 RID: 771 RVA: 0x0002DDB4 File Offset: 0x0002BFB4
		public void addEventCallback(string type, TypedCallback<EventType> cb, bool once = false)
		{
			EventPair<EventType> eventPair = new EventPair<EventType>();
			eventPair.name = type;
			eventPair.cb = cb;
			eventPair.once = once;
			this.cbs.Add(eventPair);
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00006E1D File Offset: 0x0000501D
		public void delayedEmitEvent(string type, ref EventType msg)
		{
			this.queue.Enqueue(new ParameteredEvent<EventType>(type, msg));
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0002DDE8 File Offset: 0x0002BFE8
		public void emitFromQueue()
		{
			while (this.queue.Count > 0)
			{
				ParameteredEvent<EventType> parameteredEvent = this.queue.Dequeue();
				this.emitEvent(parameteredEvent.name, ref parameteredEvent.msg);
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0002DE24 File Offset: 0x0002C024
		public void emitEvent(string type, ref EventType msg)
		{
			for (int i = 0; i < this.cbs.Count; i++)
			{
				EventPair<EventType> eventPair = this.cbs[i];
				if (eventPair.name == type)
				{
					eventPair.cb(ref msg);
					if (eventPair.once)
					{
						this.cbs.Remove(eventPair);
						i--;
					}
				}
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0002DE88 File Offset: 0x0002C088
		public void removeEventCallback(string type, TypedCallback<EventType> cb)
		{
			for (int i = 0; i < this.cbs.Count; i++)
			{
				EventPair<EventType> eventPair = this.cbs[i];
				if (eventPair.name == type && cb == eventPair.cb)
				{
					this.cbs.Remove(eventPair);
					i--;
				}
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0002DEE8 File Offset: 0x0002C0E8
		public void removeAllCallbacks(string type)
		{
			for (int i = 0; i < this.cbs.Count; i++)
			{
				EventPair<EventType> eventPair = this.cbs[i];
				if (eventPair.name == type)
				{
					this.cbs.Remove(eventPair);
					i--;
				}
			}
		}

		// Token: 0x040005A4 RID: 1444
		public Queue<ParameteredEvent<EventType>> queue = new Queue<ParameteredEvent<EventType>>();

		// Token: 0x040005A5 RID: 1445
		public List<EventPair<EventType>> cbs = new List<EventPair<EventType>>();
	}
}
