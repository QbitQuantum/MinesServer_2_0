using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class SoundManager : MonoBehaviour
{
	// Token: 0x06000291 RID: 657 RVA: 0x00006883 File Offset: 0x00004A83
	public bool isSlow(int num)
	{
		return num == 6 || num == 2 || num == 3 || num == 11 || num == 4 || num == 15;
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00028218 File Offset: 0x00026418
	private void Start()
	{
		SoundManager.THIS = this;
		SoundManager.SoundOn = false;
		SoundManager.MusicOn = false;
		this.musicSource = base.gameObject.AddComponent<AudioSource>();
		for (int i = 0; i < 10; i++)
		{
			this.fast_sources[i] = base.gameObject.AddComponent<AudioSource>();
			this.fast_sources[i].bypassEffects = true;
			this.fast_sources[i].bypassListenerEffects = true;
			this.fast_sources[i].bypassReverbZones = true;
		}
		for (int j = 0; j < 10; j++)
		{
			this.slow_sources[j] = base.gameObject.AddComponent<AudioSource>();
			this.slow_sources[j].bypassEffects = true;
			this.slow_sources[j].bypassListenerEffects = true;
			this.slow_sources[j].bypassReverbZones = true;
		}
		this.musicSource.bypassEffects = true;
		this.musicSource.bypassListenerEffects = true;
		this.musicSource.bypassReverbZones = true;
	}

	// Token: 0x06000293 RID: 659 RVA: 0x000068A1 File Offset: 0x00004AA1
	public void UpdateMusic()
	{
		if (this.musicPlaying)
		{
			if (!SoundManager.MusicOn)
			{
				this.musicSource.Pause();
				return;
			}
			this.musicSource.UnPause();
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00028300 File Offset: 0x00026500
	public void PlayMusic()
	{
		if (!this.musicPlaying)
		{
			this.musicSource.clip = this.music;
			this.musicSource.loop = true;
			this.musicSource.volume = 0.1f;
			this.musicSource.Play();
			this.musicSource.Pause();
			this.musicPlaying = true;
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00028360 File Offset: 0x00026560
	public void PlayBibika()
	{
		if (this.slow_sources[0].isPlaying && this.slow_sources[0].clip == this.sounds[1])
		{
			return;
		}
		this.slow_sources[0].clip = this.sounds[1];
		this.slow_sources[0].volume = 1f;
		this.slow_sources[0].PlayDelayed(UnityEngine.Random.value * 0.05f);
		this.slow_sources[0].playOnAwake = false;
	}

	// Token: 0x06000296 RID: 662 RVA: 0x000283E8 File Offset: 0x000265E8
	public void PlaySound(int num, float volume = 1f)
	{
		if (!SoundManager.SoundOn)
		{
			return;
		}
		if (this.lastPlays.ContainsKey(num) && this.lastPlays[num] >= Time.unscaledTime - 0.05f)
		{
			return;
		}
		this.lastPlays[num] = Time.unscaledTime;
		if (this.isSlow(num))
		{
			this.last_slow++;
			this.last_slow %= 10;
			this.slow_sources[this.last_slow].clip = this.sounds[num];
			this.slow_sources[this.last_slow].volume = volume;
			this.slow_sources[this.last_slow].PlayDelayed(UnityEngine.Random.value * 0.05f);
			this.slow_sources[this.last_slow].playOnAwake = false;
			return;
		}
		this.last_fast++;
		this.last_fast %= 10;
		this.fast_sources[this.last_fast].clip = this.sounds[num];
		this.fast_sources[this.last_fast].volume = volume;
		this.fast_sources[this.last_fast].PlayDelayed(UnityEngine.Random.value * 0.05f);
		this.fast_sources[this.last_fast].playOnAwake = false;
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x040004E2 RID: 1250
	public const int SOUND_BASKET = 0;

	// Token: 0x040004E3 RID: 1251
	public const int SOUND_SIGNAL = 1;

	// Token: 0x040004E4 RID: 1252
	public const int SOUND_BOMB = 2;

	// Token: 0x040004E5 RID: 1253
	public const int SOUND_BOMBTICK = 3;

	// Token: 0x040004E6 RID: 1254
	public const int SOUND_DEATH = 4;

	// Token: 0x040004E7 RID: 1255
	public const int SOUND_DESTROY = 5;

	// Token: 0x040004E8 RID: 1256
	public const int SOUND_EMI = 6;

	// Token: 0x040004E9 RID: 1257
	public const int SOUND_GEOLOGY = 7;

	// Token: 0x040004EA RID: 1258
	public const int SOUND_HEAL = 8;

	// Token: 0x040004EB RID: 1259
	public const int SOUND_HURT = 9;

	// Token: 0x040004EC RID: 1260
	public const int SOUND_MINING = 10;

	// Token: 0x040004ED RID: 1261
	public const int SOUND_DIZZ = 11;

	// Token: 0x040004EE RID: 1262
	public const int SOUND_TP_IN = 12;

	// Token: 0x040004EF RID: 1263
	public const int SOUND_TP_OUT = 13;

	// Token: 0x040004F0 RID: 1264
	public const int SOUND_VOLC = 14;

	// Token: 0x040004F1 RID: 1265
	public const int SOUND_C190 = 15;

	// Token: 0x040004F2 RID: 1266
	public static bool SoundOn = true;

	// Token: 0x040004F3 RID: 1267
	public static bool MusicOn = true;

	// Token: 0x040004F4 RID: 1268
	public AudioClip[] sounds;

	// Token: 0x040004F5 RID: 1269
	public AudioClip music;

	// Token: 0x040004F6 RID: 1270
	private AudioSource[] fast_sources = new AudioSource[10];

	// Token: 0x040004F7 RID: 1271
	private AudioSource[] slow_sources = new AudioSource[10];

	// Token: 0x040004F8 RID: 1272
	private Dictionary<int, float> lastPlays = new Dictionary<int, float>();

	// Token: 0x040004F9 RID: 1273
	private AudioSource musicSource;

	// Token: 0x040004FA RID: 1274
	public static SoundManager THIS;

	// Token: 0x040004FB RID: 1275
	private bool musicPlaying;

	// Token: 0x040004FC RID: 1276
	private int last_fast;

	// Token: 0x040004FD RID: 1277
	private int last_slow;
}
