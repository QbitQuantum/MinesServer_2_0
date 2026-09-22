using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MyUI
{
	// Token: 0x02000098 RID: 152
	public class MyInputField : Selectable, IUpdateSelectedHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, ILayoutElement
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x000077A2 File Offset: 0x000059A2
		private BaseInput input
		{
			get
			{
				if (EventSystem.current && EventSystem.current.currentInputModule)
				{
					return EventSystem.current.currentInputModule.input;
				}
				return null;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x000077D2 File Offset: 0x000059D2
		private string compositionString
		{
			get
			{
				if (!(this.input != null))
				{
					return Input.compositionString;
				}
				return this.input.compositionString;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x000077F3 File Offset: 0x000059F3
		protected Mesh mesh
		{
			get
			{
				if (this.m_Mesh == null)
				{
					this.m_Mesh = new Mesh();
				}
				return this.m_Mesh;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x00007814 File Offset: 0x00005A14
		protected TextGenerator cachedInputTextGenerator
		{
			get
			{
				if (this.m_InputTextCache == null)
				{
					this.m_InputTextCache = new TextGenerator();
				}
				return this.m_InputTextCache;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x0002F890 File Offset: 0x0002DA90
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x0000782F File Offset: 0x00005A2F
		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform <= RuntimePlatform.Android)
				{
					if (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android)
					{
						return true;
					}
				}
				else if (platform != RuntimePlatform.TizenPlayer && platform != RuntimePlatform.tvOS)
				{
					return true;
				}
				return this.m_HideMobileInput;
			}
			set
			{
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x0000783E File Offset: 0x00005A3E
		private bool shouldActivateOnSelect
		{
			get
			{
				return Application.platform != RuntimePlatform.tvOS;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x0000784C File Offset: 0x00005A4C
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x0002F8C8 File Offset: 0x0002DAC8
		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (this.text == value)
				{
					return;
				}
				if (value == null)
				{
					value = "";
				}
				value = value.Replace("\0", string.Empty);
				if (this.m_LineType == MyInputField.LineType.SingleLine)
				{
					value = value.Replace("\n", "").Replace("\t", "");
				}
				if (this.onValidateInput != null || this.characterValidation != MyInputField.CharacterValidation.None)
				{
					this.m_Text = "";
					MyInputField.OnValidateInput onValidateInput = this.onValidateInput ?? new MyInputField.OnValidateInput(this.Validate);
					this.m_CaretPosition = (this.m_CaretSelectPosition = value.Length);
					int num = (this.characterLimit > 0) ? Math.Min(this.characterLimit, value.Length) : value.Length;
					for (int i = 0; i < num; i++)
					{
						char c = onValidateInput(this.m_Text, this.m_Text.Length, value[i]);
						if (c != '\0')
						{
							this.m_Text += c.ToString();
						}
					}
				}
				else
				{
					this.m_Text = ((this.characterLimit > 0 && value.Length > this.characterLimit) ? value.Substring(0, this.characterLimit) : value);
				}
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.text = this.m_Text;
				}
				if (this.m_CaretPosition > this.m_Text.Length)
				{
					this.m_CaretPosition = (this.m_CaretSelectPosition = this.m_Text.Length);
				}
				else if (this.m_CaretSelectPosition > this.m_Text.Length)
				{
					this.m_CaretSelectPosition = this.m_Text.Length;
				}
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00007854 File Offset: 0x00005A54
		public bool isFocused
		{
			get
			{
				return this.m_AllowInput;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x0000785C File Offset: 0x00005A5C
		// (set) Token: 0x0600042B RID: 1067 RVA: 0x00007864 File Offset: 0x00005A64
		public float caretBlinkRate
		{
			get
			{
				return this.m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
				{
					this.SetCaretActive();
				}
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x00007882 File Offset: 0x00005A82
		// (set) Token: 0x0600042D RID: 1069 RVA: 0x0000788A File Offset: 0x00005A8A
		public int caretWidth
		{
			get
			{
				return this.m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CaretWidth, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x000078A0 File Offset: 0x00005AA0
		// (set) Token: 0x0600042F RID: 1071 RVA: 0x0002FA80 File Offset: 0x0002DC80
		public Text textComponent
		{
			get
			{
				return this.m_TextComponent;
			}
			set
			{
				if (this.m_TextComponent != null)
				{
					this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
					this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
					this.m_TextComponent.UnregisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
				}
				if (SetPropertyUtility.SetClass<Text>(ref this.m_TextComponent, value))
				{
					this.EnforceTextHOverflow();
					if (this.m_TextComponent != null)
					{
						this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
						this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
						this.m_TextComponent.RegisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
					}
				}
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x000078A8 File Offset: 0x00005AA8
		// (set) Token: 0x06000431 RID: 1073 RVA: 0x000078B0 File Offset: 0x00005AB0
		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x000078BF File Offset: 0x00005ABF
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x000078DB File Offset: 0x00005ADB
		public Color caretColor
		{
			get
			{
				if (!this.customCaretColor)
				{
					return this.textComponent.color;
				}
				return this.m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_CaretColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x000078F1 File Offset: 0x00005AF1
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x000078F9 File Offset: 0x00005AF9
		public bool customCaretColor
		{
			get
			{
				return this.m_CustomCaretColor;
			}
			set
			{
				if (this.m_CustomCaretColor != value)
				{
					this.m_CustomCaretColor = value;
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x00007911 File Offset: 0x00005B11
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x00007919 File Offset: 0x00005B19
		public Color selectionColor
		{
			get
			{
				return this.m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_SelectionColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x0000792F File Offset: 0x00005B2F
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x00007937 File Offset: 0x00005B37
		public MyInputField.SubmitEvent onEndEdit
		{
			get
			{
				return this.m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass<MyInputField.SubmitEvent>(ref this.m_OnEndEdit, value);
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00007946 File Offset: 0x00005B46
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x0000794E File Offset: 0x00005B4E
		[Obsolete("onValueChange has been renamed to onValueChanged")]
		public MyInputField.OnChangeEvent onValueChange
		{
			get
			{
				return this.onValueChanged;
			}
			set
			{
				this.onValueChanged = value;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00007957 File Offset: 0x00005B57
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x0000795F File Offset: 0x00005B5F
		public MyInputField.OnChangeEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<MyInputField.OnChangeEvent>(ref this.m_OnValueChanged, value);
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0000796E File Offset: 0x00005B6E
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x00007976 File Offset: 0x00005B76
		public MyInputField.OnValidateInput onValidateInput
		{
			get
			{
				return this.m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass<MyInputField.OnValidateInput>(ref this.m_OnValidateInput, value);
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00007985 File Offset: 0x00005B85
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x0000798D File Offset: 0x00005B8D
		public int characterLimit
		{
			get
			{
				return this.m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, Math.Max(0, value)))
				{
					this.UpdateLabel();
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x000079A9 File Offset: 0x00005BA9
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x000079B1 File Offset: 0x00005BB1
		public MyInputField.ContentType contentType
		{
			get
			{
				return this.m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<MyInputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x000079C7 File Offset: 0x00005BC7
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x000079CF File Offset: 0x00005BCF
		public MyInputField.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<MyInputField.LineType>(ref this.m_LineType, value))
				{
					this.SetToCustomIfContentTypeIsNot(new MyInputField.ContentType[]
					{
						MyInputField.ContentType.Standard,
						MyInputField.ContentType.Autocorrected
					});
					this.EnforceTextHOverflow();
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x000079F5 File Offset: 0x00005BF5
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x000079FD File Offset: 0x00005BFD
		public MyInputField.InputType inputType
		{
			get
			{
				return this.m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<MyInputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00007A13 File Offset: 0x00005C13
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x00007A1B File Offset: 0x00005C1B
		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.m_KeyboardType;
			}
			set
			{
				if (value == TouchScreenKeyboardType.NintendoNetworkAccount)
				{
					Debug.LogWarning("Invalid InputField.keyboardType value set. TouchScreenKeyboardType.NintendoNetworkAccount only applies to the Wii U. InputField.keyboardType will default to TouchScreenKeyboardType.Default .");
				}
				if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
				{
					this.SetToCustom();
				}
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00007A3F File Offset: 0x00005C3F
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x00007A47 File Offset: 0x00005C47
		public MyInputField.CharacterValidation characterValidation
		{
			get
			{
				return this.m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<MyInputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x00007A5D File Offset: 0x00005C5D
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x00007A65 File Offset: 0x00005C65
		public bool readOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x00007A6E File Offset: 0x00005C6E
		public bool multiLine
		{
			get
			{
				return this.m_LineType == MyInputField.LineType.MultiLineNewline || this.lineType == MyInputField.LineType.MultiLineSubmit;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x00007A84 File Offset: 0x00005C84
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x00007A8C File Offset: 0x00005C8C
		public char asteriskChar
		{
			get
			{
				return this.m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value))
				{
					this.UpdateLabel();
				}
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x00007AA2 File Offset: 0x00005CA2
		public bool wasCanceled
		{
			get
			{
				return this.m_WasCanceled;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x00007AAA File Offset: 0x00005CAA
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x00007ABE File Offset: 0x00005CBE
		protected int caretPositionInternal
		{
			get
			{
				return this.m_CaretPosition + this.compositionString.Length;
			}
			set
			{
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x00007AD3 File Offset: 0x00005CD3
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x00007AE7 File Offset: 0x00005CE7
		protected int caretSelectPositionInternal
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x00007AFC File Offset: 0x00005CFC
		private bool hasSelection
		{
			get
			{
				return this.caretPositionInternal != this.caretSelectPositionInternal;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000457 RID: 1111 RVA: 0x00007AD3 File Offset: 0x00005CD3
		// (set) Token: 0x06000458 RID: 1112 RVA: 0x00007B0F File Offset: 0x00005D0F
		public int caretPosition
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				this.selectionAnchorPosition = value;
				this.selectionFocusPosition = value;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000459 RID: 1113 RVA: 0x00007AAA File Offset: 0x00005CAA
		// (set) Token: 0x0600045A RID: 1114 RVA: 0x00007B1F File Offset: 0x00005D1F
		public int selectionAnchorPosition
		{
			get
			{
				return this.m_CaretPosition + this.compositionString.Length;
			}
			set
			{
				if (this.compositionString.Length == 0)
				{
					this.m_CaretPosition = value;
					this.ClampPos(ref this.m_CaretPosition);
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600045B RID: 1115 RVA: 0x00007AD3 File Offset: 0x00005CD3
		// (set) Token: 0x0600045C RID: 1116 RVA: 0x00007B41 File Offset: 0x00005D41
		public int selectionFocusPosition
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				if (this.compositionString.Length == 0)
				{
					this.m_CaretSelectPosition = value;
					this.ClampPos(ref this.m_CaretSelectPosition);
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600045D RID: 1117 RVA: 0x00007B63 File Offset: 0x00005D63
		// (set) Token: 0x0600045E RID: 1118 RVA: 0x00007B6A File Offset: 0x00005D6A
		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x00007B72 File Offset: 0x00005D72
		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x0002FB48 File Offset: 0x0002DD48
		public virtual float preferredWidth
		{
			get
			{
				if (this.textComponent == null)
				{
					return 0f;
				}
				TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(Vector2.zero);
				return this.textComponent.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x00007B79 File Offset: 0x00005D79
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x00007B72 File Offset: 0x00005D72
		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x0002FBA0 File Offset: 0x0002DDA0
		public virtual float preferredHeight
		{
			get
			{
				if (this.textComponent == null)
				{
					return 0f;
				}
				TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(new Vector2(this.textComponent.rectTransform.rect.size.x, 0f));
				return this.textComponent.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00007B79 File Offset: 0x00005D79
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x00006E6D File Offset: 0x0000506D
		public virtual int layoutPriority
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0002FC18 File Offset: 0x0002DE18
		protected MyInputField()
		{
			this.EnforceTextHOverflow();
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00007B80 File Offset: 0x00005D80
		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
				return;
			}
			if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0002FCC0 File Offset: 0x0002DEC0
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_DrawStart = 0;
			this.m_DrawEnd = this.m_Text.Length;
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.m_TextComponent.RegisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
				this.UpdateLabel();
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0002FD88 File Offset: 0x0002DF88
		protected override void OnDisable()
		{
			this.m_BlinkCoroutine = null;
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.m_TextComponent.UnregisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.Clear();
			}
			if (this.m_Mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
			base.OnDisable();
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00007BA7 File Offset: 0x00005DA7
		private IEnumerator CaretBlink()
		{
			this.m_CaretVisible = true;
			yield return null;
			while (this.isFocused && this.m_CaretBlinkRate > 0f)
			{
				float num = 1f / this.m_CaretBlinkRate;
				bool flag = (Time.unscaledTime - this.m_BlinkStartTime) % num < num / 2f;
				if (this.m_CaretVisible != flag)
				{
					this.m_CaretVisible = flag;
					if (!this.hasSelection)
					{
						this.MarkGeometryAsDirty();
					}
				}
				yield return null;
			}
			this.m_BlinkCoroutine = null;
			yield break;
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00007BB6 File Offset: 0x00005DB6
		private void SetCaretVisible()
		{
			if (this.m_AllowInput)
			{
				this.m_CaretVisible = true;
				this.m_BlinkStartTime = Time.unscaledTime;
				this.SetCaretActive();
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00007BD8 File Offset: 0x00005DD8
		private void SetCaretActive()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			if (this.m_CaretBlinkRate > 0f)
			{
				if (this.m_BlinkCoroutine == null)
				{
					this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
					return;
				}
			}
			else
			{
				this.m_CaretVisible = true;
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00007C12 File Offset: 0x00005E12
		private void UpdateCaretMaterial()
		{
			if (this.m_TextComponent != null && this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
			}
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00007C50 File Offset: 0x00005E50
		protected void OnFocus()
		{
			this.SelectAll();
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00007C58 File Offset: 0x00005E58
		protected void SelectAll()
		{
			this.caretPositionInternal = this.text.Length;
			this.caretSelectPositionInternal = 0;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0002FE3C File Offset: 0x0002E03C
		public void MoveTextEnd(bool shift)
		{
			int length = this.text.Length;
			if (shift)
			{
				this.caretSelectPositionInternal = length;
			}
			else
			{
				this.caretPositionInternal = length;
				this.caretSelectPositionInternal = this.caretPositionInternal;
			}
			this.UpdateLabel();
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0002FE7C File Offset: 0x0002E07C
		public void MoveTextStart(bool shift)
		{
			int num = 0;
			if (shift)
			{
				this.caretSelectPositionInternal = num;
			}
			else
			{
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = this.caretPositionInternal;
			}
			this.UpdateLabel();
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00007C72 File Offset: 0x00005E72
		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0002FEB0 File Offset: 0x0002E0B0
		private void UpdateCaretFromKeyboard()
		{
			RangeInt selection = this.m_Keyboard.selection;
			int start = selection.start;
			int end = selection.end;
			bool flag = false;
			if (this.caretPositionInternal != start)
			{
				flag = true;
				this.caretPositionInternal = start;
			}
			if (this.caretSelectPositionInternal != end)
			{
				this.caretSelectPositionInternal = end;
				flag = true;
			}
			if (flag)
			{
				this.m_BlinkStartTime = Time.unscaledTime;
				this.UpdateLabel();
			}
		}

        // Token: 0x06000474 RID: 1140 RVA: 0x0002FF14 File Offset: 0x0002E114
        protected virtual void LateUpdate()
        {
            if (this.m_ShouldActivateNextUpdate)
            {
                if (!this.isFocused)
                {
                    this.ActivateInputFieldInternal();
                    this.m_ShouldActivateNextUpdate = false;
                    return;
                }
                this.m_ShouldActivateNextUpdate = false;
            }

            if (this.InPlaceEditing() || !this.isFocused)
            {
                return;
            }

            this.AssignPositioningIfNeeded();

            if (this.m_Keyboard == null || this.m_Keyboard.done)
            {
                if (this.m_Keyboard != null)
                {
                    if (!this.m_ReadOnly)
                    {
                        this.text = this.m_Keyboard.text;
                    }
                    if (this.m_Keyboard.wasCanceled)
                    {
                        this.m_WasCanceled = true;
                    }
                }
                this.OnDeselect(null);
                return;
            }

            string text = this.m_Keyboard.text;
            if (this.m_Text != text)
            {
                if (this.m_ReadOnly)
                {
                    this.m_Keyboard.text = this.m_Text;
                }
                else
                {
                    this.m_Text = "";
                    foreach (char ch in text)  // используем временную переменную ch
                    {
                        char c = ch;  // создаем изменяемую копию
                        if (c == '\r' || c == '\u0003')
                        {
                            c = '\n';
                        }
                        if (this.onValidateInput != null)
                        {
                            c = this.onValidateInput(this.m_Text, this.m_Text.Length, c);
                        }
                        else if (this.characterValidation != MyInputField.CharacterValidation.None)
                        {
                            c = this.Validate(this.m_Text, this.m_Text.Length, c);
                        }
                        if (this.lineType == MyInputField.LineType.MultiLineSubmit && c == '\n')
                        {
                            this.m_Keyboard.text = this.m_Text;
                            this.OnDeselect(null);
                            return;
                        }
                        if (c != '\0')
                        {
                            this.m_Text += c.ToString();
                        }
                    }
                    if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
                    {
                        this.m_Text = this.m_Text.Substring(0, this.characterLimit);
                    }
                    if (!this.m_Keyboard.canGetSelection)
                    {
                        this.caretPositionInternal = (this.caretSelectPositionInternal = this.m_Text.Length);
                    }
                    else
                    {
                        this.UpdateCaretFromKeyboard();
                    }
                    if (this.m_Text != text)
                    {
                        this.m_Keyboard.text = this.m_Text;
                    }
                    this.SendOnValueChangedAndUpdateLabel();
                }
            }
            else if (this.m_Keyboard.canGetSelection)
            {
                this.UpdateCaretFromKeyboard();
            }
            if (this.m_Keyboard.done)
            {
                if (this.m_Keyboard.wasCanceled)
                {
                    this.m_WasCanceled = true;
                }
                this.OnDeselect(null);
            }
        }

		// Token: 0x06000476 RID: 1142 RVA: 0x0003023C File Offset: 0x0002E43C
		private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
		{
			if (!this.multiLine)
			{
				return 0;
			}
			float num = pos.y * this.m_TextComponent.pixelsPerUnit;
			float num2 = 0f;
			int i = 0;
			while (i < generator.lineCount)
			{
				float topY = generator.lines[i].topY;
				float num3 = topY - (float)generator.lines[i].height;
				if (num > topY)
				{
					float num4 = topY - num2;
					if (num > topY - 0.5f * num4)
					{
						return i - 1;
					}
					return i;
				}
				else
				{
					if (num > num3)
					{
						return i;
					}
					num2 = num3;
					i++;
				}
			}
			return generator.lineCount;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x000302D4 File Offset: 0x0002E4D4
		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			if (cachedTextGenerator.lineCount == 0)
			{
				return 0;
			}
			int unclampedCharacterLineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
			if (unclampedCharacterLineFromPosition < 0)
			{
				return 0;
			}
			if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
			{
				return cachedTextGenerator.characterCountVisible;
			}
			int startCharIdx = cachedTextGenerator.lines[unclampedCharacterLineFromPosition].startCharIdx;
			int lineEndPosition = MyInputField.GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
			int num = startCharIdx;
			while (num < lineEndPosition && num < cachedTextGenerator.characterCountVisible)
			{
				UICharInfo uicharInfo = cachedTextGenerator.characters[num];
				Vector2 vector = uicharInfo.cursorPos / this.m_TextComponent.pixelsPerUnit;
				float num2 = pos.x - vector.x;
				float num3 = vector.x + uicharInfo.charWidth / this.m_TextComponent.pixelsPerUnit - pos.x;
				if (num2 < num3)
				{
					return num;
				}
				num++;
			}
			return lineEndPosition;
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00007C7C File Offset: 0x00005E7C
		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left && this.m_TextComponent != null && this.m_Keyboard == null;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00007CAF File Offset: 0x00005EAF
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				this.m_UpdateDrag = true;
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x000303A4 File Offset: 0x0002E5A4
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				Vector2 pos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out pos);
				this.caretSelectPositionInternal = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
				this.MarkGeometryAsDirty();
				this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera);
				if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
				{
					this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
				}
				eventData.Use();
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00007CC1 File Offset: 0x00005EC1
		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			while (this.m_UpdateDrag && this.m_DragPositionOutOfBounds)
			{
				Vector2 vector;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out vector);
				Rect rect = this.textComponent.rectTransform.rect;
				if (this.multiLine)
				{
					if (vector.y > rect.yMax)
					{
						this.MoveUp(true, true);
					}
					else if (vector.y < rect.yMin)
					{
						this.MoveDown(true, true);
					}
				}
				else if (vector.x < rect.xMin)
				{
					this.MoveLeft(true, false);
				}
				else if (vector.x > rect.xMax)
				{
					this.MoveRight(true, false);
				}
				this.UpdateLabel();
				float time = this.multiLine ? 0.1f : 0.05f;
				yield return new WaitForSecondsRealtime(time);
			}
			this.m_DragCoroutine = null;
			yield break;
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00007CD7 File Offset: 0x00005ED7
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				this.m_UpdateDrag = false;
			}
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00030444 File Offset: 0x0002E644
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
			bool allowInput = this.m_AllowInput;
			base.OnPointerDown(eventData);
			if (!this.InPlaceEditing() && (this.m_Keyboard == null || !this.m_Keyboard.active))
			{
				this.OnSelect(eventData);
				return;
			}
			if (allowInput)
			{
				Vector2 pos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out pos);
				this.caretSelectPositionInternal = (this.caretPositionInternal = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart);
			}
			this.UpdateLabel();
			eventData.Use();
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x000304EC File Offset: 0x0002E6EC
		protected MyInputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			bool flag = (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX) ? ((modifiers & EventModifiers.Command) > EventModifiers.None) : ((modifiers & EventModifiers.Control) > EventModifiers.None);
			bool flag2 = (modifiers & EventModifiers.Shift) > EventModifiers.None;
			bool flag3 = (modifiers & EventModifiers.Alt) > EventModifiers.None;
			bool flag4 = flag && !flag3 && !flag2;
			KeyCode keyCode = evt.keyCode;
			if (keyCode <= KeyCode.A)
			{
				if (keyCode <= KeyCode.Return)
				{
					if (keyCode == KeyCode.Backspace)
					{
						this.Backspace();
						return MyInputField.EditState.Continue;
					}
					if (keyCode != KeyCode.Return)
					{
						goto IL_1B5;
					}
				}
				else
				{
					if (keyCode == KeyCode.Escape)
					{
						this.m_WasCanceled = true;
						return MyInputField.EditState.Finish;
					}
					if (keyCode != KeyCode.A)
					{
						goto IL_1B5;
					}
					if (flag4)
					{
						this.SelectAll();
						return MyInputField.EditState.Continue;
					}
					goto IL_1B5;
				}
			}
			else if (keyCode <= KeyCode.V)
			{
				if (keyCode != KeyCode.C)
				{
					if (keyCode != KeyCode.V)
					{
						goto IL_1B5;
					}
					if (flag4)
					{
						this.Append(MyInputField.clipboard);
						return MyInputField.EditState.Continue;
					}
					goto IL_1B5;
				}
				else
				{
					if (flag4)
					{
						if (this.inputType != MyInputField.InputType.Password)
						{
							MyInputField.clipboard = this.GetSelectedString();
						}
						else
						{
							MyInputField.clipboard = "";
						}
						return MyInputField.EditState.Continue;
					}
					goto IL_1B5;
				}
			}
			else if (keyCode != KeyCode.X)
			{
				if (keyCode == KeyCode.Delete)
				{
					this.ForwardSpace();
					return MyInputField.EditState.Continue;
				}
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
					break;
				case KeyCode.KeypadEquals:
				case KeyCode.Insert:
					goto IL_1B5;
				case KeyCode.UpArrow:
					this.MoveUp(flag2);
					return MyInputField.EditState.Continue;
				case KeyCode.DownArrow:
					this.MoveDown(flag2);
					return MyInputField.EditState.Continue;
				case KeyCode.RightArrow:
					this.MoveRight(flag2, flag);
					return MyInputField.EditState.Continue;
				case KeyCode.LeftArrow:
					this.MoveLeft(flag2, flag);
					return MyInputField.EditState.Continue;
				case KeyCode.Home:
					this.MoveTextStart(flag2);
					return MyInputField.EditState.Continue;
				case KeyCode.End:
					this.MoveTextEnd(flag2);
					return MyInputField.EditState.Continue;
				default:
					goto IL_1B5;
				}
			}
			else
			{
				if (flag4)
				{
					if (this.inputType != MyInputField.InputType.Password)
					{
						MyInputField.clipboard = this.GetSelectedString();
					}
					else
					{
						MyInputField.clipboard = "";
					}
					this.Delete();
					this.SendOnValueChangedAndUpdateLabel();
					return MyInputField.EditState.Continue;
				}
				goto IL_1B5;
			}
			if (this.lineType != MyInputField.LineType.MultiLineNewline)
			{
				return MyInputField.EditState.Finish;
			}
			IL_1B5:
			char c = evt.character;
			if (!this.multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return MyInputField.EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (this.IsValidChar(c))
			{
				this.Append(c);
			}
			if (c == '\0' && this.compositionString.Length > 0)
			{
				this.UpdateLabel();
			}
			return MyInputField.EditState.Continue;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0003070C File Offset: 0x0002E90C
		private bool IsValidChar(char c)
		{
			if (c <= '\n')
			{
				if (c == '\t' || c == '\n')
				{
					return true;
				}
			}
			else
			{
				if (c == '\u007f')
				{
					return false;
				}
				switch (c)
				{
				case 'А':
				case 'Б':
				case 'В':
				case 'Г':
				case 'Д':
				case 'Е':
				case 'Ж':
				case 'З':
				case 'И':
				case 'Й':
				case 'К':
				case 'Л':
				case 'М':
				case 'Н':
				case 'О':
				case 'П':
				case 'Р':
				case 'С':
				case 'Т':
				case 'У':
				case 'Ф':
				case 'Х':
				case 'Ц':
				case 'Ч':
				case 'Ш':
				case 'Щ':
				case 'Ъ':
				case 'Ы':
				case 'Ь':
				case 'Э':
				case 'Ю':
				case 'Я':
					return true;
				}
			}
			return this.m_TextComponent.font.HasCharacter(c);
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00007CE9 File Offset: 0x00005EE9
		public void ProcessEvent(Event e)
		{
			this.KeyPressed(e);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x000307E0 File Offset: 0x0002E9E0
		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (!this.isFocused)
			{
				return;
			}
			bool flag = false;
			while (Event.PopEvent(this.m_ProcessingEvent))
			{
				if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
				{
					flag = true;
					if (this.KeyPressed(this.m_ProcessingEvent) == MyInputField.EditState.Finish)
					{
						this.DeactivateInputField();
						break;
					}
				}
				if (this.m_ProcessingEvent.type - EventType.ValidateCommand <= 1 && this.m_ProcessingEvent.commandName == "SelectAll")
				{
					this.SelectAll();
					flag = true;
				}
			}
			if (flag)
			{
				this.UpdateLabel();
			}
			eventData.Use();
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00030870 File Offset: 0x0002EA70
		private string GetSelectedString()
		{
			if (!this.hasSelection)
			{
				return "";
			}
			int num = this.caretPositionInternal;
			int num2 = this.caretSelectPositionInternal;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return this.text.Substring(num, num2 - num);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x000308B0 File Offset: 0x0002EAB0
		private int FindtNextWordBegin()
		{
			if (this.caretSelectPositionInternal + 1 >= this.text.Length)
			{
				return this.text.Length;
			}
			int num = this.text.IndexOfAny(MyInputField.kSeparators, this.caretSelectPositionInternal + 1);
			if (num == -1)
			{
				return this.text.Length;
			}
			return num + 1;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0003090C File Offset: 0x0002EB0C
		private void MoveRight(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal));
				return;
			}
			int num = (!ctrl) ? (this.caretSelectPositionInternal + 1) : this.FindtNextWordBegin();
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00030978 File Offset: 0x0002EB78
		private int FindtPrevWordBegin()
		{
			if (this.caretSelectPositionInternal - 2 < 0)
			{
				return 0;
			}
			int num = this.text.LastIndexOfAny(MyInputField.kSeparators, this.caretSelectPositionInternal - 2);
			if (num == -1)
			{
				return 0;
			}
			return num + 1;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x000309B4 File Offset: 0x0002EBB4
		private void MoveLeft(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal));
				return;
			}
			int num = (!ctrl) ? (this.caretSelectPositionInternal - 1) : this.FindtPrevWordBegin();
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00030A20 File Offset: 0x0002EC20
		private int DetermineCharacterLine(int charPos, TextGenerator generator)
		{
			for (int i = 0; i < generator.lineCount - 1; i++)
			{
				if (generator.lines[i + 1].startCharIdx > charPos)
				{
					return i;
				}
			}
			return generator.lineCount - 1;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00030A60 File Offset: 0x0002EC60
		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.cachedInputTextGenerator.characters.Count)
			{
				return 0;
			}
			UICharInfo uicharInfo = this.cachedInputTextGenerator.characters[originalPos];
			int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
			if (num > 0)
			{
				int num2 = this.cachedInputTextGenerator.lines[num].startCharIdx - 1;
				for (int i = this.cachedInputTextGenerator.lines[num - 1].startCharIdx; i < num2; i++)
				{
					if (this.cachedInputTextGenerator.characters[i].cursorPos.x >= uicharInfo.cursorPos.x)
					{
						return i;
					}
				}
				return num2;
			}
			if (!goToFirstChar)
			{
				return originalPos;
			}
			return 0;
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00030B14 File Offset: 0x0002ED14
		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.cachedInputTextGenerator.characterCountVisible)
			{
				return this.text.Length;
			}
			UICharInfo uicharInfo = this.cachedInputTextGenerator.characters[originalPos];
			int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
			if (num + 1 < this.cachedInputTextGenerator.lineCount)
			{
				int lineEndPosition = MyInputField.GetLineEndPosition(this.cachedInputTextGenerator, num + 1);
				for (int i = this.cachedInputTextGenerator.lines[num + 1].startCharIdx; i < lineEndPosition; i++)
				{
					if (this.cachedInputTextGenerator.characters[i].cursorPos.x >= uicharInfo.cursorPos.x)
					{
						return i;
					}
				}
				return lineEndPosition;
			}
			if (!goToLastChar)
			{
				return originalPos;
			}
			return this.text.Length;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00007CF3 File Offset: 0x00005EF3
		private void MoveDown(bool shift)
		{
			this.MoveDown(shift, true);
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00030BDC File Offset: 0x0002EDDC
		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int caretSelectPositionInternal = this.multiLine ? this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar) : this.text.Length;
			if (shift)
			{
				this.caretSelectPositionInternal = caretSelectPositionInternal;
				return;
			}
			this.caretPositionInternal = (this.caretSelectPositionInternal = caretSelectPositionInternal);
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00007CFD File Offset: 0x00005EFD
		private void MoveUp(bool shift)
		{
			this.MoveUp(shift, true);
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00030C58 File Offset: 0x0002EE58
		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = this.multiLine ? this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar) : 0;
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00030CC8 File Offset: 0x0002EEC8
		private void Delete()
		{
			if (!this.m_ReadOnly && this.caretPositionInternal != this.caretSelectPositionInternal)
			{
				if (this.caretPositionInternal < this.caretSelectPositionInternal)
				{
					this.m_Text = this.text.Substring(0, this.caretPositionInternal) + this.text.Substring(this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
					this.caretSelectPositionInternal = this.caretPositionInternal;
					return;
				}
				this.m_Text = this.text.Substring(0, this.caretSelectPositionInternal) + this.text.Substring(this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
				this.caretPositionInternal = this.caretSelectPositionInternal;
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00030D98 File Offset: 0x0002EF98
		private void ForwardSpace()
		{
			if (!this.m_ReadOnly)
			{
				if (this.hasSelection)
				{
					this.Delete();
					this.SendOnValueChangedAndUpdateLabel();
					return;
				}
				if (this.caretPositionInternal < this.text.Length)
				{
					this.m_Text = this.text.Remove(this.caretPositionInternal, 1);
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00030DF4 File Offset: 0x0002EFF4
		private void Backspace()
		{
			if (!this.m_ReadOnly)
			{
				if (this.hasSelection)
				{
					this.Delete();
					this.SendOnValueChangedAndUpdateLabel();
					return;
				}
				if (this.caretPositionInternal > 0)
				{
					this.m_Text = this.text.Remove(this.caretPositionInternal - 1, 1);
					this.caretSelectPositionInternal = --this.caretPositionInternal;
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00030E60 File Offset: 0x0002F060
		private void Insert(char c)
		{
			if (!this.m_ReadOnly)
			{
				string text = c.ToString();
				this.Delete();
				if (this.characterLimit <= 0 || this.text.Length < this.characterLimit)
				{
					this.m_Text = this.text.Insert(this.m_CaretPosition, text);
					this.caretSelectPositionInternal = (this.caretPositionInternal += text.Length);
					this.SendOnValueChanged();
				}
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00007D07 File Offset: 0x00005F07
		private void SendOnValueChangedAndUpdateLabel()
		{
			this.SendOnValueChanged();
			this.UpdateLabel();
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00007D15 File Offset: 0x00005F15
		private void SendOnValueChanged()
		{
			UISystemProfilerApi.AddMarker("InputField.value", this);
			if (this.onValueChanged != null)
			{
				this.onValueChanged.Invoke(this.text);
			}
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x00007D3B File Offset: 0x00005F3B
		protected void SendOnSubmit()
		{
			UISystemProfilerApi.AddMarker("InputField.onSubmit", this);
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x00030EDC File Offset: 0x0002F0DC
		protected virtual void Append(string input)
		{
			if (this.m_ReadOnly || !this.InPlaceEditing())
			{
				return;
			}
			int i = 0;
			int length = input.Length;
			while (i < length)
			{
				char c = input[i];
				if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
				{
					this.Append(c);
				}
				i++;
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00030F38 File Offset: 0x0002F138
		protected virtual void Append(char input)
		{
			if (!this.m_ReadOnly && this.text.Length < 16382 && this.InPlaceEditing())
			{
				int num = Math.Min(this.selectionFocusPosition, this.selectionAnchorPosition);
				if (this.onValidateInput != null)
				{
					input = this.onValidateInput(this.text, num, input);
				}
				else if (this.characterValidation != MyInputField.CharacterValidation.None)
				{
					input = this.Validate(this.text, num, input);
				}
				if (input != '\0')
				{
					this.Insert(input);
				}
			}
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x00030FBC File Offset: 0x0002F1BC
		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.font != null && !this.m_PreventFontCallback)
			{
				this.m_PreventFontCallback = true;
				string text = (this.compositionString.Length <= 0) ? this.text : (this.text.Substring(0, this.m_CaretPosition) + this.compositionString + this.text.Substring(this.m_CaretPosition));
				string text2 = (this.inputType != MyInputField.InputType.Password) ? text : new string(this.asteriskChar, text.Length);
				bool flag = string.IsNullOrEmpty(text);
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.enabled = flag;
				}
				if (!this.m_AllowInput)
				{
					this.m_DrawStart = 0;
					this.m_DrawEnd = this.m_Text.Length;
				}
				if (!flag)
				{
					Vector2 size = this.m_TextComponent.rectTransform.rect.size;
					TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(size);
					generationSettings.generateOutOfBounds = true;
					this.cachedInputTextGenerator.PopulateWithErrors(text2, generationSettings, base.gameObject);
					this.SetDrawRangeToContainCaretPosition(this.caretSelectPositionInternal);
					text2 = text2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, text2.Length) - this.m_DrawStart);
					this.SetCaretVisible();
				}
				this.m_TextComponent.text = text2;
				this.MarkGeometryAsDirty();
				this.m_PreventFontCallback = false;
			}
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x00007D61 File Offset: 0x00005F61
		private bool IsSelectionVisible()
		{
			return this.m_DrawStart <= this.caretPositionInternal && this.m_DrawStart <= this.caretSelectPositionInternal && this.m_DrawEnd >= this.caretPositionInternal && this.m_DrawEnd >= this.caretSelectPositionInternal;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00007DA0 File Offset: 0x00005FA0
		private static int GetLineStartPosition(TextGenerator gen, int line)
		{
			line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
			return gen.lines[line].startCharIdx;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00007DC9 File Offset: 0x00005FC9
		private static int GetLineEndPosition(TextGenerator gen, int line)
		{
			line = Mathf.Max(line, 0);
			if (line + 1 < gen.lines.Count)
			{
				return gen.lines[line + 1].startCharIdx - 1;
			}
			return gen.characterCountVisible;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00031140 File Offset: 0x0002F340
		private void SetDrawRangeToContainCaretPosition(int caretPos)
		{
			if (this.cachedInputTextGenerator.lineCount <= 0)
			{
				return;
			}
			Vector2 size = this.cachedInputTextGenerator.rectExtents.size;
			if (!this.multiLine)
			{
				IList<UICharInfo> characters = this.cachedInputTextGenerator.characters;
				if (this.m_DrawEnd > this.cachedInputTextGenerator.characterCountVisible)
				{
					this.m_DrawEnd = this.cachedInputTextGenerator.characterCountVisible;
				}
				float num = 0f;
				if (caretPos > this.m_DrawEnd || (caretPos == this.m_DrawEnd && this.m_DrawStart > 0))
				{
					this.m_DrawEnd = caretPos;
					this.m_DrawStart = this.m_DrawEnd - 1;
					while (this.m_DrawStart >= 0 && num + characters[this.m_DrawStart].charWidth <= size.x)
					{
						num += characters[this.m_DrawStart].charWidth;
						this.m_DrawStart--;
					}
					this.m_DrawStart++;
				}
				else
				{
					if (caretPos < this.m_DrawStart)
					{
						this.m_DrawStart = caretPos;
					}
					this.m_DrawEnd = this.m_DrawStart;
				}
				while (this.m_DrawEnd < this.cachedInputTextGenerator.characterCountVisible)
				{
					num += characters[this.m_DrawEnd].charWidth;
					if (num > size.x)
					{
						break;
					}
					this.m_DrawEnd++;
				}
				return;
			}
			IList<UILineInfo> lines = this.cachedInputTextGenerator.lines;
			int num2 = this.DetermineCharacterLine(caretPos, this.cachedInputTextGenerator);
			if (caretPos > this.m_DrawEnd)
			{
				this.m_DrawEnd = MyInputField.GetLineEndPosition(this.cachedInputTextGenerator, num2);
				float num3 = lines[num2].topY - (float)lines[num2].height;
				if (num2 == lines.Count - 1)
				{
					num3 += lines[num2].leading;
				}
				int num4 = num2;
				while (num4 > 0 && lines[num4 - 1].topY - num3 <= size.y)
				{
					num4--;
				}
				this.m_DrawStart = MyInputField.GetLineStartPosition(this.cachedInputTextGenerator, num4);
				return;
			}
			if (caretPos < this.m_DrawStart)
			{
				this.m_DrawStart = MyInputField.GetLineStartPosition(this.cachedInputTextGenerator, num2);
			}
			int i = this.DetermineCharacterLine(this.m_DrawStart, this.cachedInputTextGenerator);
			int j = i;
			float topY = lines[i].topY;
			float num5 = lines[j].topY - (float)lines[j].height;
			if (j == lines.Count - 1)
			{
				num5 += lines[j].leading;
			}
			while (j < lines.Count - 1)
			{
				num5 = lines[j + 1].topY - (float)lines[j + 1].height;
				if (j + 1 == lines.Count - 1)
				{
					num5 += lines[j + 1].leading;
				}
				if (topY - num5 > size.y)
				{
					break;
				}
				j++;
			}
			this.m_DrawEnd = MyInputField.GetLineEndPosition(this.cachedInputTextGenerator, j);
			while (i > 0)
			{
				topY = lines[i - 1].topY;
				if (topY - num5 > size.y)
				{
					break;
				}
				i--;
			}
			this.m_DrawStart = MyInputField.GetLineStartPosition(this.cachedInputTextGenerator, i);
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00007E00 File Offset: 0x00006000
		public void ForceLabelUpdate()
		{
			this.UpdateLabel();
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00007E08 File Offset: 0x00006008
		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x00007E10 File Offset: 0x00006010
		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				this.UpdateGeometry();
			}
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual void LayoutComplete()
		{
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual void GraphicUpdateComplete()
		{
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x00031494 File Offset: 0x0002F694
		private void UpdateGeometry()
		{
			if (this.shouldHideMobileInput)
			{
				if (this.m_CachedInputRenderer == null && this.m_TextComponent != null)
				{
					GameObject gameObject = new GameObject(base.transform.name + " Input Caret", new Type[]
					{
						typeof(RectTransform),
						typeof(CanvasRenderer)
					});
					gameObject.hideFlags = HideFlags.DontSave;
					gameObject.transform.SetParent(this.m_TextComponent.transform.parent);
					gameObject.transform.SetAsFirstSibling();
					gameObject.layer = base.gameObject.layer;
					this.caretRectTrans = gameObject.GetComponent<RectTransform>();
					this.m_CachedInputRenderer = gameObject.GetComponent<CanvasRenderer>();
					this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
					gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
					this.AssignPositioningIfNeeded();
				}
				if (!(this.m_CachedInputRenderer == null))
				{
					this.OnFillVBO(this.mesh);
					this.m_CachedInputRenderer.SetMesh(this.mesh);
				}
			}
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x000315C0 File Offset: 0x0002F7C0
		private void AssignPositioningIfNeeded()
		{
			if (this.m_TextComponent != null && this.caretRectTrans != null && (this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition || this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation || this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale || this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin || this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax || this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition || this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta || this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot))
			{
				this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
				this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
				this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
				this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
				this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
				this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
				this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
				this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
			}
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x000317E8 File Offset: 0x0002F9E8
		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!this.isFocused)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					Vector2 roundingOffset = this.m_TextComponent.PixelAdjustPoint(Vector2.zero);
					if (!this.hasSelection)
					{
						this.GenerateCaret(vertexHelper, roundingOffset);
					}
					else
					{
						this.GenerateHightlight(vertexHelper, roundingOffset);
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0003185C File Offset: 0x0002FA5C
		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (!this.m_CaretVisible)
			{
				return;
			}
			if (this.m_CursorVerts == null)
			{
				this.CreateCursorVerts();
			}
			float num = (float)this.m_CaretWidth;
			int num2 = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			if (cachedTextGenerator == null || cachedTextGenerator.lineCount == 0)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			if (num2 < cachedTextGenerator.characters.Count)
			{
				UICharInfo uicharInfo = cachedTextGenerator.characters[num2];
				zero.x = uicharInfo.cursorPos.x;
			}
			zero.x /= this.m_TextComponent.pixelsPerUnit;
			if (zero.x > this.m_TextComponent.rectTransform.rect.xMax)
			{
				zero.x = this.m_TextComponent.rectTransform.rect.xMax;
			}
			int index = this.DetermineCharacterLine(num2, cachedTextGenerator);
			zero.y = cachedTextGenerator.lines[index].topY / this.m_TextComponent.pixelsPerUnit;
			float num3 = (float)cachedTextGenerator.lines[index].height / this.m_TextComponent.pixelsPerUnit;
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i].color = this.caretColor;
			}
			this.m_CursorVerts[0].position = new Vector3(zero.x, zero.y - num3, 0f);
			this.m_CursorVerts[1].position = new Vector3(zero.x + num, zero.y - num3, 0f);
			this.m_CursorVerts[2].position = new Vector3(zero.x + num, zero.y, 0f);
			this.m_CursorVerts[3].position = new Vector3(zero.x, zero.y, 0f);
			if (roundingOffset != Vector2.zero)
			{
				for (int j = 0; j < this.m_CursorVerts.Length; j++)
				{
					UIVertex uivertex = this.m_CursorVerts[j];
					uivertex.position.x = uivertex.position.x + roundingOffset.x;
					uivertex.position.y = uivertex.position.y + roundingOffset.y;
				}
			}
			vbo.AddUIVertexQuad(this.m_CursorVerts);
			int num4 = Screen.height;
			int targetDisplay = this.m_TextComponent.canvas.targetDisplay;
			if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
			{
				num4 = Display.displays[targetDisplay].renderingHeight;
			}
			zero.y = (float)num4 - zero.y;
			this.input.compositionCursorPos = zero;
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x00031B2C File Offset: 0x0002FD2C
		private void CreateCursorVerts()
		{
			this.m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i] = UIVertex.simpleVert;
				this.m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

        // Token: 0x060004A6 RID: 1190 RVA: 0x00031B80 File Offset: 0x0002FD80
        private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
        {
            int num = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
            int num2 = Mathf.Max(0, this.caretSelectPositionInternal - this.m_DrawStart);
            if (num > num2)
            {
                (num2, num) = (num, num2);
            }
            num2--;

            TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
            if (cachedTextGenerator.lineCount <= 0)
            {
                return;
            }

            int num4 = this.DetermineCharacterLine(num, cachedTextGenerator);
            int lineEndPosition = MyInputField.GetLineEndPosition(cachedTextGenerator, num4);
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.uv0 = Vector2.zero;
            simpleVert.color = this.selectionColor;

            int num5 = num;
            while (num5 <= num2 && num5 < cachedTextGenerator.characterCount)
            {
                if (num5 == lineEndPosition || num5 == num2)
                {
                    UICharInfo uicharInfo = cachedTextGenerator.characters[num];
                    UICharInfo uicharInfo2 = cachedTextGenerator.characters[num5];
                    Vector2 vector = new Vector2(uicharInfo.cursorPos.x / this.m_TextComponent.pixelsPerUnit, cachedTextGenerator.lines[num4].topY / this.m_TextComponent.pixelsPerUnit);
                    Vector2 vector2 = new Vector2((uicharInfo2.cursorPos.x + uicharInfo2.charWidth) / this.m_TextComponent.pixelsPerUnit, vector.y - (float)cachedTextGenerator.lines[num4].height / this.m_TextComponent.pixelsPerUnit);

                    if (vector2.x > this.m_TextComponent.rectTransform.rect.xMax || vector2.x < this.m_TextComponent.rectTransform.rect.xMin)
                    {
                        vector2.x = this.m_TextComponent.rectTransform.rect.xMax;
                    }

                    int currentVertCount = vbo.currentVertCount;

                    simpleVert.position = new Vector3(vector.x, vector2.y, 0f) + new Vector3(roundingOffset.x, roundingOffset.y, 0f);
                    vbo.AddVert(simpleVert);

                    simpleVert.position = new Vector3(vector2.x, vector2.y, 0f) + new Vector3(roundingOffset.x, roundingOffset.y, 0f);
                    vbo.AddVert(simpleVert);

                    simpleVert.position = new Vector3(vector2.x, vector.y, 0f) + new Vector3(roundingOffset.x, roundingOffset.y, 0f);
                    vbo.AddVert(simpleVert);

                    simpleVert.position = new Vector3(vector.x, vector.y, 0f) + new Vector3(roundingOffset.x, roundingOffset.y, 0f);
                    vbo.AddVert(simpleVert);

                    vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
                    vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);

                    num = num5 + 1;
                    num4++;
                    lineEndPosition = MyInputField.GetLineEndPosition(cachedTextGenerator, num4);
                }
                num5++;
            }
        }

        // Token: 0x060004A7 RID: 1191 RVA: 0x00031E48 File Offset: 0x00030048
        protected char Validate(string text, int pos, char ch)
		{
			if (this.characterValidation == MyInputField.CharacterValidation.None || !base.enabled)
			{
				return ch;
			}
			if (this.characterValidation == MyInputField.CharacterValidation.Integer || this.characterValidation == MyInputField.CharacterValidation.Decimal)
			{
				int num = (pos == 0 && text.Length > 0 && text[0] == '-') ? 1 : 0;
				bool flag = text.Length > 0 && text[0] == '-' && ((this.caretPositionInternal == 0 && this.caretSelectPositionInternal > 0) || (this.caretSelectPositionInternal == 0 && this.caretPositionInternal > 0));
				bool flag2 = this.caretPositionInternal == 0 || this.caretSelectPositionInternal == 0;
				if (num == 0 || flag)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && (pos == 0 || flag2))
					{
						return ch;
					}
					if (ch == '.' && this.characterValidation == MyInputField.CharacterValidation.Decimal && !text.Contains("."))
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == MyInputField.CharacterValidation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (this.characterValidation == MyInputField.CharacterValidation.Name)
			{
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && (pos == 0 || text[pos - 1] == ' '))
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && pos > 0 && text[pos - 1] != ' ' && text[pos - 1] != '\'')
					{
						return char.ToLower(ch);
					}
					return ch;
				}
				else
				{
					if (ch == '\'' && !text.Contains("'") && (pos <= 0 || (text[pos - 1] != ' ' && text[pos - 1] != '\'')) && (pos >= text.Length || (text[pos] != ' ' && text[pos] != '\'')))
					{
						return ch;
					}
					if (ch == ' ' && (pos <= 0 || (text[pos - 1] != ' ' && text[pos - 1] != '\'')) && (pos >= text.Length || (text[pos] != ' ' && text[pos] != '\'')))
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == MyInputField.CharacterValidation.EmailAddress)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '@' && text.IndexOf('@') == -1)
				{
					return ch;
				}
				if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
				{
					return ch;
				}
				if (ch == '.')
				{
					int num2 = (int)((text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ');
					char c = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
					if (num2 != 46 && c != '.')
					{
						return ch;
					}
				}
			}
			return '\0';
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0003211C File Offset: 0x0003031C
		public void ActivateInputField()
		{
			if (!(this.m_TextComponent == null) && !(this.m_TextComponent.font == null) && this.IsActive() && this.IsInteractable())
			{
				if (this.isFocused && this.m_Keyboard != null && !this.m_Keyboard.active)
				{
					this.m_Keyboard.active = true;
					this.m_Keyboard.text = this.m_Text;
				}
				this.m_ShouldActivateNextUpdate = true;
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0003219C File Offset: 0x0003039C
		private void ActivateInputFieldInternal()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			if (EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			if (TouchScreenKeyboard.isSupported)
			{
				if (this.input.touchSupported)
				{
					TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
				}
				this.m_Keyboard = ((this.inputType == MyInputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == MyInputField.InputType.AutoCorrect, this.multiLine));
				this.MoveTextEnd(false);
			}
			else
			{
				this.input.imeCompositionMode = IMECompositionMode.On;
				this.OnFocus();
			}
			this.m_AllowInput = true;
			this.m_OriginalText = this.text;
			this.m_WasCanceled = false;
			this.SetCaretVisible();
			this.UpdateLabel();
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00007E1C File Offset: 0x0000601C
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (this.shouldActivateOnSelect)
			{
				this.ActivateInputField();
			}
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00007E33 File Offset: 0x00006033
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.ActivateInputField();
			}
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00032288 File Offset: 0x00030488
		public void DeactivateInputField()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_HasDoneFocusTransition = false;
			this.m_AllowInput = false;
			if (this.m_Placeholder != null)
			{
				this.m_Placeholder.enabled = string.IsNullOrEmpty(this.m_Text);
			}
			if (this.m_TextComponent != null && this.IsInteractable())
			{
				if (this.m_WasCanceled)
				{
					this.text = this.m_OriginalText;
				}
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.active = false;
					this.m_Keyboard = null;
				}
				this.m_CaretPosition = (this.m_CaretSelectPosition = 0);
				this.SendOnSubmit();
				this.input.imeCompositionMode = IMECompositionMode.Auto;
			}
			this.MarkGeometryAsDirty();
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00007E43 File Offset: 0x00006043
		public override void OnDeselect(BaseEventData eventData)
		{
			this.DeactivateInputField();
			base.OnDeselect(eventData);
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00007E52 File Offset: 0x00006052
		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (this.IsActive() && this.IsInteractable() && !this.isFocused)
			{
				this.m_ShouldActivateNextUpdate = true;
			}
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00032340 File Offset: 0x00030540
		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case MyInputField.ContentType.Standard:
				this.m_InputType = MyInputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = MyInputField.CharacterValidation.None;
				break;
			case MyInputField.ContentType.Autocorrected:
				this.m_InputType = MyInputField.InputType.AutoCorrect;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = MyInputField.CharacterValidation.None;
				break;
			case MyInputField.ContentType.IntegerNumber:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = MyInputField.CharacterValidation.Integer;
				break;
			case MyInputField.ContentType.DecimalNumber:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = MyInputField.CharacterValidation.Decimal;
				break;
			case MyInputField.ContentType.Alphanumeric:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				this.m_CharacterValidation = MyInputField.CharacterValidation.Alphanumeric;
				break;
			case MyInputField.ContentType.Name:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NamePhonePad;
				this.m_CharacterValidation = MyInputField.CharacterValidation.Name;
				break;
			case MyInputField.ContentType.EmailAddress:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				this.m_CharacterValidation = MyInputField.CharacterValidation.EmailAddress;
				break;
			case MyInputField.ContentType.Password:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = MyInputField.CharacterValidation.None;
				break;
			case MyInputField.ContentType.Pin:
				this.m_LineType = MyInputField.LineType.SingleLine;
				this.m_InputType = MyInputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = MyInputField.CharacterValidation.Integer;
				break;
			}
			this.EnforceTextHOverflow();
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00007E73 File Offset: 0x00006073
		private void EnforceTextHOverflow()
		{
			if (this.m_TextComponent != null)
			{
				if (this.multiLine)
				{
					this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
					return;
				}
				this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
			}
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00032494 File Offset: 0x00030694
		private void SetToCustomIfContentTypeIsNot(params MyInputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType == MyInputField.ContentType.Custom)
			{
				return;
			}
			for (int i = 0; i < allowedContentTypes.Length; i++)
			{
				if (this.contentType == allowedContentTypes[i])
				{
					return;
				}
			}
			this.contentType = MyInputField.ContentType.Custom;
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x00007EA4 File Offset: 0x000060A4
		private void SetToCustom()
		{
			if (this.contentType != MyInputField.ContentType.Custom)
			{
				this.contentType = MyInputField.ContentType.Custom;
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00007EB8 File Offset: 0x000060B8
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.m_HasDoneFocusTransition)
			{
				state = Selectable.SelectionState.Highlighted;
			}
			else if (state == Selectable.SelectionState.Pressed)
			{
				this.m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x00004B5F File Offset: 0x00002D5F
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00007EF2 File Offset: 0x000060F2

		// Token: 0x040005FC RID: 1532
		protected TouchScreenKeyboard m_Keyboard;

		// Token: 0x040005FD RID: 1533
		private static readonly char[] kSeparators = new char[]
		{
			' ',
			'.',
			',',
			'\t',
			'\r',
			'\n'
		};

		// Token: 0x040005FE RID: 1534
		[SerializeField]
		[FormerlySerializedAs("text")]
		protected Text m_TextComponent;

		// Token: 0x040005FF RID: 1535
		[SerializeField]
		protected Graphic m_Placeholder;

		// Token: 0x04000600 RID: 1536
		[SerializeField]
		private MyInputField.ContentType m_ContentType;

		// Token: 0x04000601 RID: 1537
		[FormerlySerializedAs("inputType")]
		[SerializeField]
		private MyInputField.InputType m_InputType;

		// Token: 0x04000602 RID: 1538
		[FormerlySerializedAs("asteriskChar")]
		[SerializeField]
		private char m_AsteriskChar = '*';

		// Token: 0x04000603 RID: 1539
		[FormerlySerializedAs("keyboardType")]
		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		// Token: 0x04000604 RID: 1540
		[SerializeField]
		private MyInputField.LineType m_LineType;

		// Token: 0x04000605 RID: 1541
		[FormerlySerializedAs("hideMobileInput")]
		[SerializeField]
		private bool m_HideMobileInput;

		// Token: 0x04000606 RID: 1542
		[FormerlySerializedAs("validation")]
		[SerializeField]
		private MyInputField.CharacterValidation m_CharacterValidation;

		// Token: 0x04000607 RID: 1543
		[FormerlySerializedAs("characterLimit")]
		[SerializeField]
		private int m_CharacterLimit;

		// Token: 0x04000608 RID: 1544
		[FormerlySerializedAs("onSubmit")]
		[FormerlySerializedAs("m_OnSubmit")]
		[FormerlySerializedAs("m_EndEdit")]
		[SerializeField]
		private MyInputField.SubmitEvent m_OnEndEdit = new MyInputField.SubmitEvent();

		// Token: 0x04000609 RID: 1545
		[FormerlySerializedAs("onValueChange")]
		[FormerlySerializedAs("m_OnValueChange")]
		[SerializeField]
		private MyInputField.OnChangeEvent m_OnValueChanged = new MyInputField.OnChangeEvent();

		// Token: 0x0400060A RID: 1546
		[FormerlySerializedAs("onValidateInput")]
		[SerializeField]
		private MyInputField.OnValidateInput m_OnValidateInput;

		// Token: 0x0400060B RID: 1547
		[SerializeField]
		private Color m_CaretColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

		// Token: 0x0400060C RID: 1548
		[SerializeField]
		private bool m_CustomCaretColor;

		// Token: 0x0400060D RID: 1549
		[FormerlySerializedAs("selectionColor")]
		[SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.80784315f, 1f, 0.7529412f);

		// Token: 0x0400060E RID: 1550
		[SerializeField]
		[FormerlySerializedAs("mValue")]
		protected string m_Text = string.Empty;

		// Token: 0x0400060F RID: 1551
		[SerializeField]
		[Range(0f, 4f)]
		private float m_CaretBlinkRate = 0.85f;

		// Token: 0x04000610 RID: 1552
		[SerializeField]
		[Range(1f, 5f)]
		private int m_CaretWidth = 1;

		// Token: 0x04000611 RID: 1553
		[SerializeField]
		private bool m_ReadOnly;

		// Token: 0x04000612 RID: 1554
		protected int m_CaretPosition;

		// Token: 0x04000613 RID: 1555
		protected int m_CaretSelectPosition;

		// Token: 0x04000614 RID: 1556
		private RectTransform caretRectTrans;

		// Token: 0x04000615 RID: 1557
		protected UIVertex[] m_CursorVerts;

		// Token: 0x04000616 RID: 1558
		private TextGenerator m_InputTextCache;

		// Token: 0x04000617 RID: 1559
		private CanvasRenderer m_CachedInputRenderer;

		// Token: 0x04000618 RID: 1560
		private bool m_PreventFontCallback;

		// Token: 0x04000619 RID: 1561
		[NonSerialized]
		protected Mesh m_Mesh;

		// Token: 0x0400061A RID: 1562
		private bool m_AllowInput;

		// Token: 0x0400061B RID: 1563
		private bool m_ShouldActivateNextUpdate;

		// Token: 0x0400061C RID: 1564
		private bool m_UpdateDrag;

		// Token: 0x0400061D RID: 1565
		private bool m_DragPositionOutOfBounds;

		// Token: 0x0400061E RID: 1566
		private const float kHScrollSpeed = 0.05f;

		// Token: 0x0400061F RID: 1567
		private const float kVScrollSpeed = 0.1f;

		// Token: 0x04000620 RID: 1568
		protected bool m_CaretVisible;

		// Token: 0x04000621 RID: 1569
		private Coroutine m_BlinkCoroutine;

		// Token: 0x04000622 RID: 1570
		private float m_BlinkStartTime;

		// Token: 0x04000623 RID: 1571
		protected int m_DrawStart;

		// Token: 0x04000624 RID: 1572
		protected int m_DrawEnd;

		// Token: 0x04000625 RID: 1573
		private Coroutine m_DragCoroutine;

		// Token: 0x04000626 RID: 1574
		private string m_OriginalText = "";

		// Token: 0x04000627 RID: 1575
		private bool m_WasCanceled;

		// Token: 0x04000628 RID: 1576
		private bool m_HasDoneFocusTransition;

		// Token: 0x04000629 RID: 1577
		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		// Token: 0x0400062A RID: 1578
		private Event m_ProcessingEvent = new Event();

		// Token: 0x0400062B RID: 1579
		private const int k_MaxTextLength = 16382;

		// Token: 0x02000099 RID: 153
		public enum ContentType
		{
			// Token: 0x0400062D RID: 1581
			Standard,
			// Token: 0x0400062E RID: 1582
			Autocorrected,
			// Token: 0x0400062F RID: 1583
			IntegerNumber,
			// Token: 0x04000630 RID: 1584
			DecimalNumber,
			// Token: 0x04000631 RID: 1585
			Alphanumeric,
			// Token: 0x04000632 RID: 1586
			Name,
			// Token: 0x04000633 RID: 1587
			EmailAddress,
			// Token: 0x04000634 RID: 1588
			Password,
			// Token: 0x04000635 RID: 1589
			Pin,
			// Token: 0x04000636 RID: 1590
			Custom
		}

		// Token: 0x0200009A RID: 154
		public enum InputType
		{
			// Token: 0x04000638 RID: 1592
			Standard,
			// Token: 0x04000639 RID: 1593
			AutoCorrect,
			// Token: 0x0400063A RID: 1594
			Password
		}

		// Token: 0x0200009B RID: 155
		public enum CharacterValidation
		{
			// Token: 0x0400063C RID: 1596
			None,
			// Token: 0x0400063D RID: 1597
			Integer,
			// Token: 0x0400063E RID: 1598
			Decimal,
			// Token: 0x0400063F RID: 1599
			Alphanumeric,
			// Token: 0x04000640 RID: 1600
			Name,
			// Token: 0x04000641 RID: 1601
			EmailAddress
		}

		// Token: 0x0200009C RID: 156
		public enum LineType
		{
			// Token: 0x04000643 RID: 1603
			SingleLine,
			// Token: 0x04000644 RID: 1604
			MultiLineSubmit,
			// Token: 0x04000645 RID: 1605
			MultiLineNewline
		}

		// Token: 0x0200009D RID: 157
		// (Invoke) Token: 0x060004B9 RID: 1209
		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		// Token: 0x0200009E RID: 158
		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		// Token: 0x0200009F RID: 159
		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		// Token: 0x020000A0 RID: 160
		protected enum EditState
		{
			// Token: 0x04000647 RID: 1607
			Continue,
			// Token: 0x04000648 RID: 1608
			Finish
		}
	}
}
