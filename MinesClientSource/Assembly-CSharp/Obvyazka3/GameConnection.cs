using System;

namespace Obvyazka3
{
	// Token: 0x0200007E RID: 126
	public abstract class GameConnection
	{
		// Token: 0x0600030B RID: 779
		public abstract void Connect(int port, string host);

		// Token: 0x0600030C RID: 780
		public abstract void Destroy();

		// Token: 0x0600030D RID: 781
		public abstract void Translate(string type, string eventName, string data);

		// Token: 0x0600030E RID: 782
		public abstract void Update();

		// Token: 0x0600030F RID: 783
		public abstract void ForceDisconnect();

		// Token: 0x06000310 RID: 784
		public abstract void SendJ(string eventName, JSONNode message);

		// Token: 0x06000311 RID: 785
		public abstract void SendU(string eventName, string message);

		// Token: 0x06000312 RID: 786
		public abstract void SendB(string eventName, byte[] message);

		// Token: 0x06000313 RID: 787
		public abstract void OnJ(string eventName, TypedCallback<JSONNode> cb, bool once = false);

		// Token: 0x06000314 RID: 788
		public abstract void OnU(string eventName, TypedCallback<string> cb, bool once = false);

		// Token: 0x06000315 RID: 789
		public abstract void OnB(string eventName, TypedCallback<byte[]> cb, bool once = false);
	}
}
