using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Policy;
using MyUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000045 RID: 69
public class PopupManager : MonoBehaviour
{
    // Token: 0x060001BF RID: 447 RVA: 0x0001C1B8 File Offset: 0x0001A3B8
    private void Start()
    {
        PopupManager.THIS = this;
        this.GUIWindow.SetActive(false);
        for (int i = 0; i < this.buttons.Length; i++)
        {
            string buttonName = this.str112;
            int buttonNum = i;
            EventTrigger eventTrigger = this.buttons[i].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entry.callback.AddListener(delegate (BaseEventData e)
            {
                this.CommonButtonListener(buttonName, buttonNum);
            });
            eventTrigger.triggers.Add(entry);
        }
        this.backButton.onClick.AddListener(new UnityAction(this.onBack));
        this.exitButton.onClick.AddListener(new UnityAction(this.onExit));
        this.upButton.onClick.AddListener(new UnityAction(this.OnUpButton));
        this.deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteButton));
        this.lockButton.onClick.AddListener(new UnityAction(this.onLock));
        this.upExitButton.onClick.AddListener(new UnityAction(this.OnUpExitButton));
        this.adminButton.onClick.AddListener(new UnityAction(this.OnAdmin));
    }

    private string ConcatString(params object[] strings)
    {
        return this.str101 + string.Concat(strings) + this.str110;
    }

    private void ServerSendlMessage(string message)
    {
        ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), this.str102, 0, 0, message);
    }

    // Token: 0x060001C0 RID: 448 RVA: 0x00005EFD File Offset: 0x000040FD
    private void onBack()
    {
        this.CommonButtonListener(this.str104, 0);
    }

    // Token: 0x060001C1 RID: 449 RVA: 0x00005F0C File Offset: 0x0000410C
    private void onExit()
    {
        this.CommonButtonListener(this.str103, 0);
    }

    // Token: 0x060001C2 RID: 450 RVA: 0x0001C310 File Offset: 0x0001A510
    private void onLock()
    {
        ServerSendlMessage(ConcatString("lock:", this.upcfg.slot));
    }

    // Token: 0x060001C3 RID: 451 RVA: 0x00005F1B File Offset: 0x0000411B
    private void OnAdmin()
    {
        this.fixScrollSave();
        ServerSendlMessage(ConcatString("_"));
    }

    // Token: 0x060001C4 RID: 452 RVA: 0x0001C374 File Offset: 0x0001A574
    private string escapeString(string inStr)
    {
        inStr = inStr.Replace(this.str106, this.str107);
        inStr = inStr.Replace(this.str108, this.str109);
        inStr = inStr.Replace("\\", "\\\\");
        inStr = inStr.Replace("\n", "\\n");
        return inStr.Replace("\"", "\\\"");
    }

    // Token: 0x060001C5 RID: 453 RVA: 0x0001C3E0 File Offset: 0x0001A5E0
    private string MacroReplacer(string inStr)
    {
        string text = this.escapeString(this.inputText.text);
        if (inStr.Contains("%I%") && text.StartsWith("&"))
        {
            text = text.Substring(1);
        }
        inStr = inStr.Replace("%I%", text);
        inStr = inStr.Replace("%M%", this.crystallSection.GetComponent<CrystallSection>().GetValuesInString());
        if (inStr.IndexOf("%B%") != -1)
        {
            string text2 = "";
            Toggle[] componentsInChildren = this.paint.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i].isOn)
                {
                    text2 += "1";
                }
                else
                {
                    text2 += "0";
                }
            }
            inStr = inStr.Replace("%B%", text2);
        }
        if (inStr.IndexOf("%R%") != -1)
        {
            string text3 = "";
            if (this.horbcfg.richList != null)
            {
                for (int j = 0; j < this.horbcfg.richList.Length / 5; j++)
                {
                    string text4 = this.horbcfg.richList[5 * j];
                    string a = this.horbcfg.richList[5 * j + 1];
                    string text5 = this.horbcfg.richList[5 * j + 2];
                    string str = this.horbcfg.richList[5 * j + 3];
                    string text6 = this.horbcfg.richList[5 * j + 4];

                    if (a == "text")
                    {
                        text3 += str;
                        text3 += this.str111;
                        text3 += "0";
                    }
                    else if (a == "uint")
                    {
                        text3 += str;
                        text3 += this.str111;
                        text3 += this.richListObjects[j].GetComponentInChildren<MyInputField>().text;
                    }
                    else if (a == "drop")
                    {
                        text3 += str;
                        text3 += this.str111;
                        string[] array = text5.Split(new char[]
                        {
                                '#'
                        });
                        int value = this.richListObjects[j].GetComponentInChildren<Dropdown>().value;
                        string[] array2 = array[value].Split(new char[]
                        {
                                ':'
                        });
                        text3 += array2[0];
                    }
                    else if (a == "bool")
                    {
                        text3 += str;
                        text3 += this.str111;
                        text3 += (this.richListObjects[j].GetComponentInChildren<Toggle>().isOn ? "1" : "0");
                    }

                    text3 += PopupManager.statstr1;
                }
                inStr = inStr.Replace("%R%", text3);
            }
        }
        return inStr;
    }

    // Token: 0x060001C6 RID: 454 RVA: 0x00005F50 File Offset: 0x00004150
    private void SimpleButtonListener(string action)
    {
        ServerSendlMessage(ConcatString(action));
        this.fixScrollSave();
    }

    // Token: 0x060001C7 RID: 455 RVA: 0x0001C6CC File Offset: 0x0001A8CC
    private void CommonButtonListener(string buttonType, int num)
    {
        ClientController.CanGoto = false;
        this.fixScrollSave();
        string str;
        if (buttonType == this.str112)
        {
            string text = this.MacroReplacer(this.horbcfg.buttons[2 * num + 1]);
            if (text.Length > 0 && text.Substring(0, 1) == "@")
            {
                text = text.Substring(1);
                if (!this.buttons[num].interactable)
                {
                    return;
                }
                this.buttons[num].interactable = false;
            }
            if (text.Length > 0 && text.Substring(0, 1) == "&")
            {
                Application.OpenURL(text.Substring(1));
                return;
            }
            str = text;
        }
        else if (buttonType == this.str103)
        {
            str = this.str103;
        }
        else if (buttonType == this.str104)
        {
            str = this.MacroReplacer(this.backButtonAction);
        }
        else if (buttonType == "l")
        {
            str = this.MacroReplacer(this.horbcfg.list[3 * num + 2]);
        }
        else if (buttonType == "th")
        {
            str = this.MacroReplacer(this.horbcfg.tabs[num]);
        }
        else
        {
            return;
        }
        ServerSendlMessage(ConcatString(str));
    }

    // Token: 0x060001C8 RID: 456 RVA: 0x00005F87 File Offset: 0x00004187
    public void CloseWindow()
    {
        this.GUIWindow.SetActive(false);
        GUIManager.THIS.ClearFocus();
    }

    // Token: 0x060001C9 RID: 457 RVA: 0x0001C858 File Offset: 0x0001AA58
    public void ShowUP(UPConfig cfg)
    {
        this.GUIWindow.SetActive(true);
        this.mode = "up";
        this.upcfg = cfg;
        this.disableKeyboard = false;
        this.blinkingObject = null;
        this.canvasGUI.SetActive(false);
        this.cardView.SetActive(false);
        this.upView.SetActive(true);
        this.backButton.gameObject.SetActive(false);
        this.scrollView.SetActive(false);
        this.insideTF.gameObject.SetActive(false);
        this.inputText.gameObject.SetActive(false);
        this.buttonRow.gameObject.SetActive(false);
        this.crystallSection.gameObject.SetActive(false);
        this.inventory.gameObject.SetActive(false);
        this.richContent.gameObject.SetActive(false);
        Vector2 sizeDelta = this.scrollView.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 216f;
        this.scrollView.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        this.adminButton.gameObject.SetActive(cfg.admin);
        ClientController.THIS.stopAutoMove();
        bool flag = cfg.tabs != null && cfg.tabs.Length != 0;
        bool flag2 = flag;
        bool flag3 = flag2;
        bool flag4 = flag3;
        bool flag5 = flag4;
        if (flag5)
        {
            this.tabs.gameObject.SetActive(true);
            foreach (object obj in this.tabsRow.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj).gameObject);
            }
            for (int i = 0; i < cfg.tabs.Length; i += 2)
            {
                bool flag6 = cfg.tabs[i + 1] == "";
                bool flag7 = flag6;
                bool flag8 = flag7;
                bool flag9 = flag8;
                bool flag10 = flag9;
                if (flag10)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.openedTabPrefab);
                    gameObject.transform.SetParent(this.tabsRow.transform, false);
                    gameObject.GetComponentInChildren<Text>().text = cfg.tabs[i];
                }
                else
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.closedTabPrefab);
                    gameObject2.transform.SetParent(this.tabsRow.transform, false);
                    gameObject2.GetComponentInChildren<Text>().text = cfg.tabs[i];
                    int actionLabel = i + 1;
                    gameObject2.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        this.CommonButtonListener("tu", actionLabel);
                    });
                }
            }
        }
        else
        {
            this.tabs.gameObject.SetActive(false);
        }
        this.deleteButton.gameObject.SetActive(cfg.canDelete);
        this.lockButton.gameObject.SetActive(cfg.canDelete);
        this.lockButton.gameObject.GetComponentInChildren<Image>().sprite = ((cfg.lockState == 1) ? this.lockButtonLocked : this.lockButtonUnlocked);
        foreach (object obj2 in this.upRoboView.transform)
        {
            UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
        }
        for (int j = cfg.skills.Length - 1; j >= 0; j--)
        {
            GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.skillButtonPrefab);
            gameObject3.transform.SetParent(this.upRoboView.transform, false);
            gameObject3.transform.localPosition = new Vector3((float)PopupManager.slotPositions[2 * j], -(float)PopupManager.slotPositions[2 * j + 1]);
            gameObject3.GetComponent<SkillButtonScript>().SetIcon(cfg.skills[j].type, cfg.skills[j].isUp, cfg.skills[j].level, cfg.skills[j].isLocked);
            int num = j;
            gameObject3.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                this.OnSkill(num);
            });
            bool flag11 = cfg.slot == j;
            bool flag12 = flag11;
            bool flag13 = flag12;
            bool flag14 = flag13;
            bool flag15 = flag14;
            if (flag15)
            {
                gameObject3.GetComponent<SkillButtonScript>().blinking = true;
            }
        }
        bool flag16 = cfg.toInstall == null;
        bool flag17 = flag16;
        bool flag18 = flag17;
        bool flag19 = flag18;
        bool flag20 = flag19;
        if (flag20)
        {
            this.upSkillView.gameObject.SetActive(true);
            this.upInstallView.gameObject.SetActive(false);
            bool flag21 = cfg.button != "";
            bool flag22 = flag21;
            bool flag23 = flag22;
            bool flag24 = flag23;
            bool flag25 = flag24;
            if (flag25)
            {
                this.upButton.gameObject.SetActive(true);
                this.upButton.GetComponentInChildren<Text>().text = cfg.button;
            }
            else
            {
                this.upButton.gameObject.SetActive(false);
            }
            bool flag26 = cfg.skillIcon == -1;
            bool flag27 = flag26;
            bool flag28 = flag27;
            bool flag29 = flag28;
            bool flag30 = flag29;
            if (flag30)
            {
                this.upImage.gameObject.SetActive(false);
            }
            else
            {
                this.upImage.gameObject.SetActive(true);
                this.upImage.sprite = SkillButtonScript.sprites[cfg.skillIcon];
            }
        }
        else
        {
            this.upSkillView.gameObject.SetActive(false);
            this.upInstallView.gameObject.SetActive(true);
            foreach (object obj3 in this.upInstallContent.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj3).gameObject);
            }
            for (int k = 0; k < cfg.toInstall.Length; k++)
            {
                Button button = UnityEngine.Object.Instantiate<Button>(this.upInstallPrefab);
                button.gameObject.transform.SetParent(this.upInstallContent.transform, false);
                string shortCode = cfg.toInstall[k];
                bool flag31 = cfg.toInstall[k].StartsWith("_");
                bool flag32 = flag31;
                bool flag33 = flag32;
                bool flag34 = flag33;
                bool flag35 = flag34;
                if (flag35)
                {
                    shortCode = cfg.toInstall[k].Substring(1);
                    button.GetComponent<Image>().sprite = SkillButtonScript.sprites[SkillButtonScript.skillShorts[shortCode]];
                    button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
                }
                else
                {
                    button.GetComponent<Image>().sprite = SkillButtonScript.sprites[SkillButtonScript.skillShorts[shortCode]];
                }
                button.onClick.AddListener(delegate ()
                {
                    this.OnInstall(shortCode, cfg.slot);
                });
            }
        }
        this.titleTF.text = cfg.title;
        this.upText.text = cfg.text;
        this.UpdateLayout();
    }

    // Token: 0x060001CA RID: 458 RVA: 0x0001CFEC File Offset: 0x0001B1EC
    private void UpdateFixScroll()
    {
        if (this.fixScroll)
        {
            float num = this.scrollView.GetComponent<ScrollRect>().content.GetComponent<RectTransform>().sizeDelta.y - this.scrollView.GetComponent<RectTransform>().sizeDelta.y;
            if (num < 0f)
            {
                num = 1f;
            }
            float num2 = 1f - this.fixScrolls[this.fixScrollTag] / num;
            if (num2 > 1f)
            {
                num2 = 1f;
            }
            if (num2 < 0f)
            {
                num2 = 0f;
            }
            this.scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = num2;
        }
    }

    // Token: 0x060001CB RID: 459 RVA: 0x0001D090 File Offset: 0x0001B290
    private void fixScrollSave()
    {
        if (this.fixScroll)
        {
            float num = this.scrollView.GetComponent<ScrollRect>().content.GetComponent<RectTransform>().sizeDelta.y - this.scrollView.GetComponent<RectTransform>().sizeDelta.y;
            if (num < 0f)
            {
                num = 1f;
            }
            this.fixScrolls[this.fixScrollTag] = num * (1f - this.scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition);
        }
    }

    // Token: 0x060001CC RID: 460 RVA: 0x0001D114 File Offset: 0x0001B314
    private void OnInstall(string code, int slot)
    {
        ServerSendlMessage(ConcatString("install", this.str111, code, PopupManager.statstr1, slot));
    }

    // Token: 0x060001CD RID: 461 RVA: 0x00005F9F File Offset: 0x0000419F
    private void OnUpExitButton()
    {
        // Проблема в том что функции имеет формат this.str101 + string.Concat(strings) + this.str110;
        // str110 != ":0\"}"
        string temp = this.str110;
        this.str110 = ":0\"}";
        ServerSendlMessage(ConcatString(this.str103));
        this.str110 = temp;
    }

    // Token: 0x060001CE RID: 462 RVA: 0x0001D184 File Offset: 0x0001B384
    private void OnDeleteButton()
    {
        ServerSendlMessage(ConcatString("delete:", this.upcfg.slot));
    }

    // Token: 0x060001CF RID: 463 RVA: 0x00005FD4 File Offset: 0x000041D4
    private void OnUpButton()
    {
        ServerSendlMessage(ConcatString(this.upcfg.buttonAction));
    }

    // Token: 0x060001D0 RID: 464 RVA: 0x0001D1E8 File Offset: 0x0001B3E8
    private void OnSkill(int slotNum)
    {
        ServerSendlMessage(ConcatString("skill", this.str111, slotNum));
    }

    // Token: 0x060001D1 RID: 465 RVA: 0x0001D24C File Offset: 0x0001B44C
    public void ShowHORB(HORBConfig cfg)
    {
        this.GUIWindow.SetActive(true);
        this.mode = "horb";
        this.horbcfg = cfg;
        this.disableKeyboard = false;
        this.blinkingObject = null;
        this.bigInput = false;
        this.canvasGUI.SetActive(false);
        this.upView.SetActive(false);
        this.adminButton.gameObject.SetActive(cfg.admin);
        this.richContent.gameObject.SetActive(false);
        ClientController.THIS.stopAutoMove();
        this.fixScroll = false;
        this.inventory.GetComponent<RectTransform>().sizeDelta = new Vector2(498f, 55f);
        this.inventory.GetComponent<GridLayoutGroup>().cellSize = new Vector2(55f, 55f);
        this.canvasGUI.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 245f);
        float num29 = 0f;
        float y = 216f;
        this.backButtonAction = this.str104;
        bool flag = false;
        this.invbutton = "choose";
        Vector2 vector;
        if (cfg.css != null)
        {
            string[] array = cfg.css.Split(new char[]
            {
            ';'
            });
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(new char[]
                {
                '='
                });
                string text = array2[0];
                string s = (array2.Length > 1) ? array2[1] : "";
                string a = "";
                string a2 = "";
                if (text.IndexOf('-') != -1)
                {
                    string[] array3 = text.Split(new char[]
                    {
                    '-'
                    });
                    text = "param";
                    a = array3[0];
                    a2 = array3[1];
                }
                GameObject gameObject = null;
                if (a == "inv")
                {
                    gameObject = this.inventory;
                }
                else if (a == "canv")
                {
                    gameObject = this.canvasGUI;
                }
                if (gameObject != null)
                {
                    if (a2 == "ch")
                    {
                        vector = gameObject.GetComponent<GridLayoutGroup>().cellSize;
                        vector.y = float.Parse(s);
                        gameObject.GetComponent<GridLayoutGroup>().cellSize = vector;
                    }
                    else if (a2 == "w")
                    {
                        vector = gameObject.GetComponent<RectTransform>().sizeDelta;
                        vector.x = float.Parse(s);
                        gameObject.GetComponent<RectTransform>().sizeDelta = vector;
                    }
                    else if (a2 == "h")
                    {
                        vector = gameObject.GetComponent<RectTransform>().sizeDelta;
                        vector.y = float.Parse(s);
                        gameObject.GetComponent<RectTransform>().sizeDelta = vector;
                    }
                }
                if (text == "fixScroll")
                {
                    this.fixScroll = true;
                    this.fixScrollTag = s;
                    if (!this.fixScrolls.ContainsKey(this.fixScrollTag))
                    {
                        this.fixScrolls[this.fixScrollTag] = 1f;
                    }
                }
                else if (text == "space")
                {
                    num29 = float.Parse(s);
                }
                else if (text == "scrollh")
                {
                    y = float.Parse(s);
                }
                else if (text == "invButton")
                {
                    this.invbutton = s;
                }
                else if (text == "keysOff")
                {
                    this.disableKeyboard = true;
                }
                else if (text == "biginput")
                {
                    this.bigInput = true;
                }
            }
        }
        vector = this.scrollView.GetComponent<RectTransform>().sizeDelta;
        vector.y = y;
        this.scrollView.GetComponent<RectTransform>().sizeDelta = vector;
        int num2 = 0;
        int num3 = 0;
        if (cfg.canvas != null && cfg.canvas.Length != 0)
        {
            this.canvasGUI.SetActive(true);
            foreach (object obj in this.canvasGUI.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj).gameObject);
            }
            for (int j = 0; j < cfg.canvas.Length; j++)
            {
                string text2 = cfg.canvas[j];
                text2 = text2.Replace(this.str107, this.str106);
                text2 = text2.Replace(this.str109, this.str108);
                string text3 = text2;
                string text4 = "";
                if (text2.IndexOf(PopupManager.statstr1) != -1)
                {
                    text3 = text2.Substring(0, text2.IndexOf(PopupManager.statstr1));
                    text4 = text2.Substring(text2.IndexOf(PopupManager.statstr1) + 1);
                }
                int num4 = 0;
                int num5 = 1;
                int num6 = 0;
                int num7 = 0;
                int num8 = 1;
                int num9 = 1;
                int num10 = 1;
                bool flag2 = false;
                int num11 = 255;
                int num12 = 0;
                for (int k = 0; k < text3.Length; k++)
                {
                    char c = text3[k];
                    if (c <= 'Y')
                    {
                        if (c <= 'L')
                        {
                            switch (c)
                            {
                                case '-':
                                    num5 = -1;
                                    break;
                                case '.':
                                case '/':
                                case ':':
                                case ';':
                                case '<':
                                case '>':
                                case '?':
                                case '@':
                                    break;
                                case '0':
                                    num4 *= 10;
                                    break;
                                case '1':
                                    num4 *= 10;
                                    num4++;
                                    break;
                                case '2':
                                    num4 *= 10;
                                    num4 += 2;
                                    break;
                                case '3':
                                    num4 *= 10;
                                    num4 += 3;
                                    break;
                                case '4':
                                    num4 *= 10;
                                    num4 += 4;
                                    break;
                                case '5':
                                    num4 *= 10;
                                    num4 += 5;
                                    break;
                                case '6':
                                    num4 *= 10;
                                    num4 += 6;
                                    break;
                                case '7':
                                    num4 *= 10;
                                    num4 += 7;
                                    break;
                                case '8':
                                    num4 *= 10;
                                    num4 += 8;
                                    break;
                                case '9':
                                    num4 *= 10;
                                    num4 += 9;
                                    break;
                                case '=':
                                    {
                                        string a3 = text3.Substring(k + 1);
                                        if (a3 == "I")
                                        {
                                            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.canvasWebImagePrefab);
                                            gameObject2.transform.SetParent(this.canvasGUI.transform, false);
                                            if (num12 != 0)
                                            {
                                                gameObject2.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject2.transform.localPosition = new Vector3((float)(num2 + num6), (float)(num3 + num7));
                                            gameObject2.GetComponent<WebImage>().SetSizeAndUrl(num8, num9, text4);
                                            if (flag2)
                                            {
                                                this.blinkingObject = gameObject2;
                                            }
                                        }
                                        else if (a3 == "L")
                                        {
                                            GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.canvasLinePrefab);
                                            gameObject3.transform.SetParent(this.canvasGUI.transform, false);
                                            if (num12 != 0)
                                            {
                                                gameObject3.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject3.transform.localPosition = new Vector3((float)(num2 + (num6 + num8) / 2), (float)(num3 + (num7 + num9) / 2));
                                            int num13 = Mathf.FloorToInt(Mathf.Sqrt((float)((num6 - num8) * (num6 - num8) + (num7 - num9) * (num7 - num9))));
                                            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num10, (float)num13);
                                            Quaternion rotation = default(Quaternion);
                                            rotation.eulerAngles = new Vector3(0f, 0f, 90f + 180f * Mathf.Atan2((float)(num7 - num9), (float)(num6 - num8)) / 3.1415927f);
                                            gameObject3.transform.rotation = rotation;
                                            gameObject3.GetComponent<Image>().color = PopupManager.hexToColor(text4, (byte)num11);
                                            if (flag2)
                                            {
                                                this.blinkingObject = gameObject3;
                                            }
                                        }
                                        else if (a3 == "B")
                                        {
                                            GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.canvasButtonPrefab);
                                            gameObject4.transform.SetParent(this.canvasGUI.transform, false);
                                            if (num12 != 0)
                                            {
                                                gameObject4.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject4.transform.localPosition = new Vector3((float)(num2 + num6), (float)(num3 + num7));
                                            gameObject4.GetComponentInChildren<Text>().text = text4;
                                            string action2 = cfg.canvas[j + 1];
                                            gameObject4.GetComponent<Button>().onClick.AddListener(delegate ()
                                            {
                                                this.SimpleButtonListener(action2);
                                            });
                                            j++;
                                        }
                                        else if (a3 == "R")
                                        {
                                            GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(this.canvasRectPrefab);
                                            gameObject5.transform.SetParent(this.canvasGUI.transform, false);
                                            if (num12 != 0)
                                            {
                                                gameObject5.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject5.transform.localPosition = new Vector3((float)(num2 + num6), (float)(num3 + num7));
                                            gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num8, (float)num9);
                                            gameObject5.GetComponent<Image>().color = PopupManager.hexToColor(text4, (byte)num11);
                                            if (flag2)
                                            {
                                                this.blinkingObject = gameObject5;
                                            }
                                        }
                                        else if (a3 == "T")
                                        {
                                            GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(this.canvasTextFieldPrefab);
                                            if (num12 != 0)
                                            {
                                                gameObject6.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject6.transform.SetParent(this.canvasGUI.transform, false);
                                            gameObject6.transform.localPosition = new Vector3((float)(num2 + num6), (float)(num3 + num7));
                                            gameObject6.GetComponent<Text>().text = text4;
                                            if (flag2)
                                            {
                                                this.blinkingObject = gameObject6;
                                            }
                                        }
                                        else if (a3 == "t")
                                        {
                                            GameObject gameObject7 = UnityEngine.Object.Instantiate<GameObject>(this.canvasTPButtonPrefab);
                                            gameObject7.transform.SetParent(this.canvasGUI.transform, false);
                                            if (num12 != 0)
                                            {
                                                gameObject7.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject7.transform.localPosition = new Vector3((float)(num2 + num6), (float)(num3 + num7));
                                            string actiontp = cfg.canvas[j + 1];
                                            gameObject7.GetComponent<Button>().onClick.AddListener(delegate ()
                                            {
                                                this.SimpleButtonListener(actiontp);
                                            });
                                            j++;
                                        }
                                        else if (a3 == "b")
                                        {
                                            GameObject gameObject8 = UnityEngine.Object.Instantiate<GameObject>(this.canvasMicroButtonPrefab);
                                            gameObject8.transform.SetParent(this.canvasGUI.transform, false);
                                            if (num12 != 0)
                                            {
                                                gameObject8.GetComponent<RectTransform>().pivot = new Vector2(0.5f - (float)num12 * 0.5f, 1f);
                                            }
                                            gameObject8.transform.localPosition = new Vector3((float)(num2 + num6), (float)(num3 + num7));
                                            gameObject8.GetComponentInChildren<Text>().text = text4;
                                            string action = cfg.canvas[j + 1];
                                            gameObject8.GetComponent<Button>().onClick.AddListener(delegate ()
                                            {
                                                this.SimpleButtonListener(action);
                                            });
                                            j++;
                                        }
                                        k = text3.Length;
                                        break;
                                    }
                                case 'A':
                                    num11 = num5 * num4;
                                    num4 = 0;
                                    num5 = 1;
                                    break;
                                case 'B':
                                    flag2 = true;
                                    num4 = 0;
                                    num5 = 1;
                                    break;
                                default:
                                    if (c == 'L')
                                    {
                                        num10 = num5 * num4;
                                        num4 = 0;
                                        num5 = 1;
                                    }
                                    break;
                            }
                        }
                        else if (c != 'X')
                        {
                            if (c == 'Y')
                            {
                                num7 = num5 * num4;
                                num4 = 0;
                                num5 = 1;
                            }
                        }
                        else
                        {
                            num6 = num5 * num4;
                            num4 = 0;
                            num5 = 1;
                        }
                    }
                    else if (c <= 'l')
                    {
                        if (c != 'h')
                        {
                            if (c == 'l')
                            {
                                num12 = 1;
                                num4 = 0;
                                num5 = 1;
                            }
                        }
                        else
                        {
                            num9 = num5 * num4;
                            num4 = 0;
                            num5 = 1;
                        }
                    }
                    else if (c != 'r')
                    {
                        switch (c)
                        {
                            case 'w':
                                num8 = num5 * num4;
                                num4 = 0;
                                num5 = 1;
                                break;
                            case 'x':
                                num2 += num5 * num4;
                                num4 = 0;
                                num5 = 1;
                                break;
                            case 'y':
                                num3 += num5 * num4;
                                num4 = 0;
                                num5 = 1;
                                break;
                        }
                    }
                    else
                    {
                        num12 = -1;
                        num4 = 0;
                        num5 = 1;
                    }
                }
            }
        }
        if (cfg.inv != null)
        {
            this.inventory.SetActive(true);
            flag = true;
            foreach (object obj2 in this.inventory.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
            }
            string[] array4 = cfg.inv.Split(new char[]
            {
            ':'
            });
            if (array4.Length > 1)
            {
                for (int l = 0; l < array4.Length; l += 2)
                {
                    GameObject gameObject9 = UnityEngine.Object.Instantiate<GameObject>(this.inventoryItemPrefab);
                    gameObject9.transform.SetParent(this.inventory.transform, false);
                    int num14 = 0;
                    bool frame = false;
                    string upstr = "";
                    string downstr = "";
                    if (array4[l + 1].Substring(0, 1) == "f")
                    {
                        upstr = "@";
                    }
                    else if (array4[l + 1].IndexOf(';') != -1)
                    {
                        string[] array5 = array4[l + 1].Split(new char[]
                        {
                        ';'
                        });
                        upstr = array5[0];
                        downstr = array5[1];
                    }
                    else
                    {
                        num14 = int.Parse(array4[l + 1]);
                    }
                    if (array4[l].Substring(0, 1) == "s")
                    {
                        int num15 = SkillButtonScript.skillShorts[array4[l].Substring(1)];
                        gameObject9.GetComponent<InventoryItem>().Setup(2000 + num15, num14, frame, upstr, downstr);
                        string type = array4[l].Substring(1);
                        gameObject9.GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), this.str102, 0, 0, string.Concat(new string[]
                            {
                            this.str101,
                            this.invbutton,
                            this.str111,
                            type,
                            this.str110
                            }));
                        });
                    }
                    else
                    {
                        int.Parse(array4[l]);
                        gameObject9.GetComponent<InventoryItem>().Setup(int.Parse(array4[l]), num14, frame, upstr, downstr);
                        int type = int.Parse(array4[l]);
                        gameObject9.GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), this.str102, 0, 0, string.Concat(new object[]
                            {
                            this.str101,
                            this.invbutton,
                            this.str111,
                            type,
                            this.str110
                            }));
                        });
                    }
                }
            }
            int num16 = array4.Length / 2;
            int num17 = num16 - 9 * (num16 / 9);
            num17 = 0;
            for (int m = 0; m < num17; m++)
            {
                GameObject gameObject10 = UnityEngine.Object.Instantiate<GameObject>(this.inventoryItemPrefab);
                gameObject10.transform.SetParent(this.inventory.transform, false);
                gameObject10.GetComponent<InventoryItem>().Setup(-1, 0, false, "", "");
            }
        }
        else
        {
            this.inventory.SetActive(false);
        }
        if (cfg.card != null && cfg.card != "")
        {
            this.cardView.SetActive(true);
            string text5 = cfg.card.Substring(0, cfg.card.IndexOf(':'));
            string a4 = text5.Substring(0, 1);
            string text6 = text5.Substring(1);
            string text7 = cfg.card.Substring(cfg.card.IndexOf(':') + 1);
            Text componentInChildren = this.cardView.GetComponentInChildren<Text>();
            WebImage componentInChildren2 = this.cardView.GetComponentInChildren<WebImage>();
            Image componentInChildren3 = this.cardView.GetComponentInChildren<Image>();
            componentInChildren.text = text7;
            componentInChildren2.off = true;
            if (a4 == "s")
            {
                Sprite sprite = SkillButtonScript.sprites[SkillButtonScript.skillShorts[text6]];
                componentInChildren3.sprite = sprite;
                componentInChildren3.SetNativeSize();
            }
            else if (a4 == "i")
            {
                Sprite sprite2 = InventoryItem.sprites[(int)short.Parse(text6)];
                componentInChildren3.sprite = sprite2;
                componentInChildren3.SetNativeSize();
            }
            else if (a4 == "c")
            {
                Sprite sprite3 = ClanSpriteScript.sprites[(int)(short.Parse(text6) - 1)];
                componentInChildren3.sprite = sprite3;
                componentInChildren3.SetNativeSize();
                componentInChildren3.rectTransform.sizeDelta = 4f * componentInChildren3.rectTransform.sizeDelta;
            }
            else if (a4 == "w")
            {
                this.cardView.GetComponentInChildren<WebImage>().off = false;
                string[] array6 = text6.Replace("%", this.str111).Split(new char[]
                {
                '#'
                });
                this.cardView.GetComponentInChildren<WebImage>().SetSizeAndUrl(int.Parse(array6[0]), int.Parse(array6[1]), array6[2]);
            }
        }
        else
        {
            this.cardView.SetActive(false);
        }
        if (cfg.paint)
        {
            this.paint.SetActive(true);
            flag = true;
        }
        else
        {
            this.paint.SetActive(false);
        }
        if (cfg.tabs != null && cfg.tabs.Length != 0)
        {
            this.tabs.gameObject.SetActive(true);
            foreach (object obj3 in this.tabsRow.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj3).gameObject);
            }
            for (int n = 0; n < cfg.tabs.Length; n += 2)
            {
                if (cfg.tabs[n + 1] == "")
                {
                    GameObject gameObject11 = UnityEngine.Object.Instantiate<GameObject>(this.openedTabPrefab);
                    gameObject11.transform.SetParent(this.tabsRow.transform, false);
                    gameObject11.GetComponentInChildren<Text>().text = cfg.tabs[n];
                }
                else
                {
                    GameObject gameObject12 = UnityEngine.Object.Instantiate<GameObject>(this.closedTabPrefab);
                    gameObject12.transform.SetParent(this.tabsRow.transform, false);
                    gameObject12.GetComponentInChildren<Text>().text = cfg.tabs[n];
                    int actionLabel = n + 1;
                    gameObject12.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        this.CommonButtonListener("th", actionLabel);
                    });
                }
            }
        }
        else
        {
            this.tabs.gameObject.SetActive(false);
        }
        if (cfg.crys_lines != null && cfg.crys_lines.Length == 6)
        {
            flag = true;
            if (cfg.crys_buy)
            {
                CrystalScroller.BUY_LOGIC = true;
            }
            else
            {
                CrystalScroller.BUY_LOGIC = false;
            }
            this.crystallSection.gameObject.SetActive(true);
            if (cfg.crys_left != null && cfg.crys_left != "")
            {
                this.crystallSection.GetComponent<CrystallSection>().leftDesc.gameObject.SetActive(true);
                this.crystallSection.GetComponent<CrystallSection>().leftDesc.text = cfg.crys_left;
            }
            else
            {
                this.crystallSection.SetActive(false);
            }
            if (cfg.crys_right != null && cfg.crys_right != "")
            {
                this.crystallSection.GetComponent<CrystallSection>().rightDesc.gameObject.SetActive(true);
                this.crystallSection.GetComponent<CrystallSection>().rightDesc.text = cfg.crys_right;
            }
            else
            {
                this.crystallSection.SetActive(false);
            }
            for (int num18 = 0; num18 < cfg.crys_lines.Length; num18++)
            {
                string[] array7 = cfg.crys_lines[num18].Split(new char[]
                {
                ':'
                });
                long leftMin = long.Parse(array7[0]);
                long rightMin = long.Parse(array7[1]);
                long d = long.Parse(array7[2]);
                long value = long.Parse(array7[3]);
                string descText = array7[4];
                CrystalScroller component = this.crystallSection.GetComponent<CrystallSection>().lines[num18].GetComponent<CrystalScroller>();
                component.leftMin = leftMin;
                component.rightMin = rightMin;
                component.d = d;
                component.value = value;
                component.descText = descText;
                component.UpdateFromModel();
            }
        }
        else
        {
            this.crystallSection.gameObject.SetActive(false);
        }
        if (cfg.input_place != null)
        {
            flag = true;
            if (cfg.input_len > 0)
            {
                this.inputText.characterLimit = cfg.input_len;
            }
            else
            {
                this.inputText.characterLimit = 35;
            }
            this.inputText.gameObject.SetActive(true);
            if (this.bigInput)
            {
                this.inputText.characterLimit = 1000;
                this.inputText.lineType = MyInputField.LineType.MultiLineNewline;
                this.inputText.placeholder.GetComponent<Text>().text = "";
                this.inputText.text = cfg.input_place;
                this.inputText.GetComponent<RectTransform>().sizeDelta = new Vector2(596f, 264f);
            }
            else
            {
                this.inputText.placeholder.GetComponent<Text>().text = cfg.input_place;
                this.inputText.text = "";
                this.inputText.lineType = MyInputField.LineType.SingleLine;
                this.inputText.GetComponent<RectTransform>().sizeDelta = new Vector2(596f, 23f);
            }
            if (cfg.input_console)
            {
                GUIManager.THIS.ClearFocus();
                GUIManager.THIS.m_EventSystem.SetSelectedGameObject(this.inputText.gameObject, null);
            }
        }
        else
        {
            this.inputText.gameObject.SetActive(false);
        }
        if (cfg.back)
        {
            this.backButton.gameObject.SetActive(true);
            this.backButtonAction = this.str104;
        }
        else
        {
            this.backButton.gameObject.SetActive(false);
        }
        if (cfg.clanlist != null && cfg.clanlist.Length != 0)
        {
            flag = true;
            this.listContent.GetComponent<VerticalLayoutGroup>().spacing = 26f;
            this.scrollView.SetActive(true);
            foreach (object obj4 in this.listContent.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj4).gameObject);
            }
            for (int num19 = 0; num19 < cfg.clanlist.Length / 4; num19++)
            {
                int num20 = int.Parse(cfg.clanlist[4 * num19]);
                string text8 = cfg.clanlist[4 * num19 + 1];
                string text9 = cfg.clanlist[4 * num19 + 2];
                string action = cfg.clanlist[4 * num19 + 3];
                GameObject gameObject13 = UnityEngine.Object.Instantiate<GameObject>(this.clanLinePrefab);
                gameObject13.transform.SetParent(this.listContent.transform, false);
                if (num20 > 0)
                {
                    gameObject13.GetComponentsInChildren<Image>()[0].sprite = ClanSpriteScript.sprites[num20 - 1];
                }
                else
                {
                    gameObject13.GetComponentsInChildren<Image>()[0].gameObject.SetActive(false);
                }
                gameObject13.GetComponentsInChildren<Text>()[0].text = text8;
                gameObject13.GetComponentsInChildren<Text>()[1].text = text9;
                gameObject13.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    this.SimpleButtonListener(action);
                });
            }
        }
        else if (cfg.richList != null && cfg.richList.Length != 0)
        {
            flag = true;
            this.listContent.GetComponent<VerticalLayoutGroup>().spacing = 44f;
            GameObject gameObject14 = this.listContent;
            if (cfg.rich_no_scroll)
            {
                this.scrollView.SetActive(false);
                gameObject14 = this.richContent;
                this.richContent.SetActive(true);
                this.listContent.GetComponent<VerticalLayoutGroup>().spacing = 35f;
            }
            else
            {
                this.scrollView.SetActive(true);
            }
            this.richListObjects = new GameObject[cfg.richList.Length / 5];
            foreach (object obj5 in gameObject14.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj5).gameObject);
            }
            for (int num21 = 0; num21 < cfg.richList.Length / 5; num21++)
            {
                string text10 = cfg.richList[5 * num21];
                string text11 = cfg.richList[5 * num21 + 1];
                string text12 = cfg.richList[5 * num21 + 2];
                string resp = cfg.richList[5 * num21 + 3];
                string text13 = cfg.richList[5 * num21 + 4];
                Debug.Log(text11);
                if (text11 == "drop")
                {
                    GameObject gameObject15 = UnityEngine.Object.Instantiate<GameObject>(this.dropdownLinePrefab);
                    gameObject15.transform.SetParent(gameObject14.transform, false);
                    gameObject15.GetComponentsInChildren<Text>()[0].text = text10;
                    string[] array8 = text12.Split(new char[]
                    {
                    '#'
                    });
                    gameObject15.GetComponentInChildren<Dropdown>().options.Clear();
                    for (int num22 = 0; num22 < array8.Length - 1; num22++)
                    {
                        string[] array9 = array8[num22].Split(new char[]
                        {
                        ':'
                        });
                        gameObject15.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData(array9[1]));
                    }
                    gameObject15.GetComponentInChildren<Dropdown>().value = int.Parse(text13);
                    this.richListObjects[num21] = gameObject15;
                }
                else if (text11 == "button")
                {
                    GameObject gameObject16 = UnityEngine.Object.Instantiate<GameObject>(this.buttonLinePrefab);
                    gameObject16.transform.SetParent(gameObject14.transform, false);
                    gameObject16.GetComponentsInChildren<Text>()[1].text = text10;
                    gameObject16.GetComponentsInChildren<Text>()[0].text = text12;
                    gameObject16.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        this.SimpleButtonListener(resp);
                    });
                    if (text12 == "")
                    {
                        gameObject16.GetComponentInChildren<Button>().gameObject.SetActive(false);
                    }
                    this.richListObjects[num21] = gameObject16;
                }
                else if (text11 == "3card")
                {
                    string[] array10 = text10.Split(new char[]
                    {
                    '&'
                    });
                    string[] array11 = text12.Split(new char[]
                    {
                    '&'
                    });
                    string[] array12 = resp.Split(new char[]
                    {
                    '&'
                    });
                    string[] array13 = text13.Split(new char[]
                    {
                    '&'
                    });
                    GameObject gameObject17 = UnityEngine.Object.Instantiate<GameObject>(this.cardLinePrefab);
                    gameObject17.transform.SetParent(gameObject14.transform, false);
                    for (int num23 = 0; num23 < array10.Length; num23++)
                    {
                        GameObject gameObject18 = UnityEngine.Object.Instantiate<GameObject>(this.cardPrefab);
                        gameObject18.transform.SetParent(gameObject17.transform, false);
                        string[] array14 = array13[num23].Split(new char[]
                        {
                        '%'
                        });
                        gameObject18.GetComponent<WebImage>().SetSizeAndUrl(int.Parse(array14[1]), int.Parse(array14[2]), array14[0]);
                        Text[] componentsInChildren = gameObject18.GetComponentsInChildren<Text>();
                        componentsInChildren[0].text = array10[num23];
                        componentsInChildren[1].text = array11[num23];
                        if (array11[num23] == "" || array11[num23] == " ")
                        {
                            Image[] componentsInChildren2 = gameObject18.GetComponentsInChildren<Image>();
                            componentsInChildren2[1].color = new Color(0f, 0f, 0f, 0.7f);
                            componentsInChildren2[2].color = new Color(0f, 0f, 0f, 0f);
                        }
                        string buttonResp = array12[num23];
                        gameObject18.GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            this.SimpleButtonListener(buttonResp);
                        });
                    }
                    this.richListObjects[num21] = gameObject17;
                }
                else if (text11 == "text")
                {
                    GameObject gameObject19 = UnityEngine.Object.Instantiate<GameObject>(this.textLinePrefab);
                    gameObject19.transform.SetParent(gameObject14.transform, false);
                    gameObject19.GetComponentInChildren<Text>().text = text10;
                    this.richListObjects[num21] = gameObject19;
                }
                else if (text11 == "fill")
                {
                    GameObject gameObject20 = UnityEngine.Object.Instantiate<GameObject>(this.fillLinePrefab);
                    gameObject20.transform.SetParent(gameObject14.transform, false);
                    string[] fillparts = text12.Split(new char[]
                    {
                    '#'
                    });
                    gameObject20.GetComponent<FuelLineScript>().Setup(int.Parse(fillparts[0]), text10, fillparts[1], int.Parse(fillparts[2]), fillparts[3] != "", fillparts[4] != "", fillparts[5] != "");
                    Button[] componentsInChildren3 = gameObject20.GetComponentsInChildren<Button>();
                    componentsInChildren3[0].onClick.AddListener(delegate ()
                    {
                        this.SimpleButtonListener(fillparts[3]);
                    });
                    componentsInChildren3[1].onClick.AddListener(delegate ()
                    {
                        this.SimpleButtonListener(fillparts[4]);
                    });
                    componentsInChildren3[2].onClick.AddListener(delegate ()
                    {
                        this.SimpleButtonListener(fillparts[5]);
                    });
                    this.richListObjects[num21] = gameObject20;
                }
                else if (text11 == "uint")
                {
                    GameObject gameObject21 = UnityEngine.Object.Instantiate<GameObject>(this.uintLinePrefab);
                    gameObject21.transform.SetParent(gameObject14.transform, false);
                    gameObject21.GetComponentsInChildren<Text>()[0].text = text10;
                    gameObject21.GetComponentsInChildren<MyInputField>()[0].text = text13;
                    this.richListObjects[num21] = gameObject21;
                }
                else if (text11 == "bool")
                {
                    GameObject gameObject22 = UnityEngine.Object.Instantiate<GameObject>(this.toggleLinePrefab);
                    gameObject22.transform.SetParent(gameObject14.transform, false);
                    gameObject22.GetComponentInChildren<Text>().text = text10;
                    gameObject22.GetComponentInChildren<Toggle>().isOn = (int.Parse(text13) != 0);
                    this.richListObjects[num21] = gameObject22;
                }
            }
        }
        else if (cfg.list == null || cfg.list.Length == 0)
        {
            this.scrollView.SetActive(false);
        }
        else
        {
            flag = true;
            this.scrollView.SetActive(true);
            this.listContent.GetComponent<VerticalLayoutGroup>().spacing = 44f;
            foreach (object obj6 in this.listContent.transform)
            {
                UnityEngine.Object.Destroy(((Transform)obj6).gameObject);
            }
            for (int num24 = 0; num24 < cfg.list.Length / 3; num24++)
            {
                GameObject gameObject23 = UnityEngine.Object.Instantiate<GameObject>(this.buttonLinePrefab);
                gameObject23.transform.SetParent(this.listContent.transform, false);
                gameObject23.GetComponentsInChildren<Text>()[0].text = cfg.list[3 * num24 + 1];
                gameObject23.GetComponentsInChildren<Text>()[1].text = cfg.list[3 * num24];
                int num = num24;
                if (cfg.list[3 * num24 + 1] == "")
                {
                    gameObject23.GetComponentInChildren<Button>().gameObject.SetActive(false);
                }
                else
                {
                    gameObject23.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        this.CommonButtonListener("l", num);
                    });
                }
            }
        }
        if (cfg.title != "")
        {
            this.titleTF.gameObject.SetActive(true);
            this.titleTF.text = cfg.title;
        }
        else
        {
            this.titleTF.gameObject.SetActive(false);
        }
        if (cfg.text != null && cfg.text != "")
        {
            if (cfg.text.Length > 2 && cfg.text.Substring(0, 2) == "%%")
            {
                Vector2 sizeDelta = this.scrollView.GetComponent<RectTransform>().sizeDelta;
                sizeDelta.y = 400f;
                this.scrollView.GetComponent<RectTransform>().sizeDelta = sizeDelta;
                GameObject gameObject24 = this.listContent;
                foreach (object obj7 in gameObject24.transform)
                {
                    UnityEngine.Object.Destroy(((Transform)obj7).gameObject);
                }
                this.listContent.GetComponent<VerticalLayoutGroup>().spacing = 5f;
                this.scrollView.SetActive(true);
                foreach (string text14 in cfg.text.Substring(2).Split(new char[]
                {
                '§'
                }))
                {
                    if (text14.StartsWith("="))
                    {
                        string[] array16 = text14.Substring(1).Split(new char[]
                        {
                        '#'
                        });
                        GameObject gameObject25 = UnityEngine.Object.Instantiate<GameObject>(this.centeredImagePrefab);
                        gameObject25.GetComponentInChildren<WebImage>().SetSizeAndUrl(int.Parse(array16[0]), int.Parse(array16[1]), array16[2]);
                        gameObject25.GetComponentInChildren<LayoutElement>().minHeight = (float)gameObject25.GetComponentInChildren<WebImage>().GetHeight();
                        gameObject25.transform.SetParent(gameObject24.transform, false);
                    }
                    else if (text14.StartsWith(">"))
                    {
                        string[] array17 = text14.Substring(1).Split(new char[]
                        {
                        '|'
                        });
                        GameObject gameObject26 = UnityEngine.Object.Instantiate<GameObject>(this.clanLinePrefab);
                        gameObject26.transform.SetParent(this.listContent.transform, false);
                        gameObject26.GetComponentsInChildren<Image>()[0].gameObject.SetActive(false);
                        gameObject26.GetComponentsInChildren<Text>()[0].text = array17[0];
                        gameObject26.GetComponentsInChildren<Text>()[1].text = array17[1];
                        gameObject26.GetComponentsInChildren<Text>()[0].color = new Color(0.8f, 0.8f, 0.5f);
                        string action = array17[2];
                        gameObject26.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                        {
                            this.SimpleButtonListener(action);
                        });
                    }
                    else
                    {
                        GameObject gameObject27 = UnityEngine.Object.Instantiate<GameObject>(this.multitextPrefab);
                        gameObject27.transform.SetParent(gameObject24.transform, false);
                        gameObject27.GetComponent<Text>().text = text14;
                    }
                }
            }
            else if (cfg.text.Length > 1 && cfg.text.Substring(0, 1) == "@")
            {
                this.insideTF.gameObject.SetActive(true);
                RectOffset padding = this.insideContainer.padding;
                padding.left = 20;
                padding.right = 20;
                this.insideContainer.padding = padding;
                this.insideTF.alignment = TextAnchor.UpperLeft;
                this.insideTF.text = cfg.text.Substring(2);
            }
            else
            {
                this.insideTF.gameObject.SetActive(true);
                RectOffset padding2 = this.insideContainer.padding;
                padding2.left = 0;
                padding2.right = 0;
                this.insideContainer.padding = padding2;
                this.insideTF.alignment = TextAnchor.MiddleCenter;
                this.insideTF.text = cfg.text;
            }
        }
        else
        {
            this.insideTF.gameObject.SetActive(false);
        }
        if (cfg.buttons != null)
        {
            this.buttonRow.gameObject.SetActive(true);
            for (int num26 = 0; num26 < this.buttons.Length; num26++)
            {
                this.buttons[num26].gameObject.SetActive(false);
            }
            int num27 = 0;
            for (int num28 = 0; num28 < cfg.buttons.Length / 2; num28++)
            {
                string text15 = cfg.buttons[2 * num28];
                string text16 = cfg.buttons[2 * num28 + 1];
                if (text16.Substring(0, 1) == "<")
                {
                    this.backButton.gameObject.SetActive(true);
                    this.backButtonAction = text16.Substring(1);
                }
                else if (text16 != this.str103)
                {
                    this.buttons[num28].gameObject.SetActive(true);
                    this.buttons[num28].interactable = true;
                    num27++;
                }
                this.buttons[num28].GetComponentInChildren<Text>().text = text15;
            }
            if (num27 == 0)
            {
                this.buttonRow.gameObject.SetActive(false);
            }
            else
            {
                flag = true;
            }
        }
        else
        {
            this.buttonRow.gameObject.SetActive(false);
        }
        if (cfg.text != "" && !flag && !this.insideTF.text.EndsWith("\n"))
        {
            this.insideTF.text = this.insideTF.text + "\n";
        }
        if (cfg.text == "##")
        {
            this.insideTF.gameObject.SetActive(false);
        }
        if (num29 != 0f)
        {
            this.listContent.GetComponent<VerticalLayoutGroup>().spacing = 26f;
        }
        if (this.fixScroll)
        {
            this.UpdateFixScroll();
            base.Invoke("UpdateFixScroll", 0.1f);
        }
        else
        {
            this.scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        }
        this.UpdateLayout();
    }


    // Token: 0x060001D2 RID: 466 RVA: 0x0000600F File Offset: 0x0000420F
    private void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.buttonRow.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.tabsRow.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GUIWindow.GetComponent<RectTransform>());
    }

    // Token: 0x060001D3 RID: 467 RVA: 0x00006041 File Offset: 0x00004241
    public static string colorToHex(Color32 color)
    {
        return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    // Token: 0x060001D4 RID: 468 RVA: 0x0001F870 File Offset: 0x0001DA70
    public static Color hexToColor(string hex, byte alpha = 255)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace(PopupManager.statstr1, "");
        byte a = alpha;
        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    // Token: 0x060001D5 RID: 469 RVA: 0x0001F904 File Offset: 0x0001DB04
    private void Update()
    {
        if (this.GUIWindow.activeSelf)
        {
            if (this.inputText.gameObject.activeSelf && this.scrollView.activeSelf && this.inputText.text != this.lastInputString)
            {
                this.lastInputString = this.inputText.text;
                string text = this.lastInputString.ToLower();
                Debug.Log("search -> " + text);
                HashSet<Transform> hashSet = new HashSet<Transform>();
                foreach (object obj in this.listContent.transform)
                {
                    Transform transform = (Transform)obj;
                    if (!(transform.parent != this.listContent.transform))
                    {
                        transform.gameObject.SetActive(false);
                        Text[] componentsInChildren = transform.GetComponentsInChildren<Text>();
                        for (int i = 0; i < componentsInChildren.Length; i++)
                        {
                            if (this.lastInputString == "")
                            {
                                hashSet.Add(transform);
                            }
                            else if (componentsInChildren[i].text.ToLower().Contains(text))
                            {
                                hashSet.Add(transform);
                            }
                        }
                    }
                }
                foreach (Transform transform2 in hashSet)
                {
                    transform2.gameObject.SetActive(true);
                }
            }
            if (this.blinkingObject != null)
            {
                this.blinkingObject.SetActive(Mathf.FloorToInt(5f * Time.time) % 2 == 0);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this.mode == "up")
                {
                    this.OnUpExitButton();
                }
                else if (this.mode == "horb" && this.horbcfg.buttons != null && this.horbcfg.buttons.Length != 0)
                {
                    this.CommonButtonListener(this.str112, this.horbcfg.buttons.Length / 2 - 1);
                }
            }
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && (!(GUIManager.THIS.m_EventSystem.currentSelectedGameObject != null) || (!(GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "ChatField") && !(GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "LocalChat"))) && !this.disableKeyboard && !this.bigInput)
            {
                if (this.mode == "up")
                {
                    this.OnUpExitButton();
                }
                else if (this.mode == "horb" && this.horbcfg.buttons != null && this.horbcfg.buttons.Length != 0)
                {
                    this.CommonButtonListener(this.str112, 0);
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftCommand) && this.canvasGUI.activeSelf)
            {
                string text2 = "";
                foreach (object obj2 in this.canvasGUI.transform)
                {
                    Transform transform3 = (Transform)obj2;
                    int num = (int)transform3.localPosition.x;
                    int num2 = (int)transform3.localPosition.y;
                    if (transform3.gameObject.tag == this.str112)
                    {
                        text2 = string.Concat(new object[]
                        {
                            text2,
                            "\"",
                            num,
                            "X",
                            num2,
                            "Y=b#",
                            this.escapeString(transform3.GetComponentInChildren<Text>().text),
                            "\",\"action\","
                        });
                    }
                    if (transform3.gameObject.tag == this.str112)
                    {
                        text2 = string.Concat(new object[]
                        {
                            text2,
                            "\"",
                            num,
                            "X",
                            num2,
                            "Y=B#",
                            this.escapeString(transform3.GetComponentInChildren<Text>().text),
                            "\",\"action\","
                        });
                    }
                    if (transform3.gameObject.tag == "T")
                    {
                        text2 = string.Concat(new object[]
                        {
                            text2,
                            "\"",
                            num,
                            "X",
                            num2,
                            "Y=T#",
                            this.escapeString(transform3.GetComponent<Text>().text),
                            "\","
                        });
                    }
                    if (transform3.gameObject.tag == "I")
                    {
                        int w = transform3.GetComponent<WebImage>().w;
                        int h = transform3.GetComponent<WebImage>().h;
                        string url = transform3.GetComponent<WebImage>().url;
                        text2 = string.Concat(new object[]
                        {
                            text2,
                            "\"",
                            w,
                            "w",
                            h,
                            "h",
                            num,
                            "X",
                            num2,
                            "Y=I#",
                            url,
                            "\","
                        });
                    }
                    if (transform3.gameObject.tag == "R")
                    {
                        int num3 = (int)transform3.GetComponent<RectTransform>().sizeDelta.x;
                        int num4 = (int)transform3.GetComponent<RectTransform>().sizeDelta.y;
                        string text3 = PopupManager.colorToHex(transform3.GetComponent<Image>().color);
                        text2 = string.Concat(new object[]
                        {
                            text2,
                            "\"",
                            num3,
                            "w",
                            num4,
                            "h",
                            num,
                            "X",
                            num2,
                            "Y=R#",
                            text3,
                            "\","
                        });
                    }
                }
                Debug.Log(text2);
            }
        }
    }

    // Token: 0x040002E4 RID: 740
    public GameObject GUIWindow;

    // Token: 0x040002E5 RID: 741
    public Text titleTF;

    // Token: 0x040002E6 RID: 742
    public Text insideTF;

    // Token: 0x040002E7 RID: 743
    public VerticalLayoutGroup insideContainer;

    // Token: 0x040002E8 RID: 744
    public GameObject openedTabPrefab;

    // Token: 0x040002E9 RID: 745
    public GameObject closedTabPrefab;

    // Token: 0x040002EA RID: 746
    public GameObject cardView;

    // Token: 0x040002EB RID: 747
    public GameObject tabs;

    // Token: 0x040002EC RID: 748
    public GameObject tabsRow;

    // Token: 0x040002ED RID: 749
    public GameObject buttonRow;

    // Token: 0x040002EE RID: 750
    public GameObject upView;

    // Token: 0x040002EF RID: 751
    public GameObject upRoboView;

    // Token: 0x040002F0 RID: 752
    public GameObject upSkillView;

    // Token: 0x040002F1 RID: 753
    public GameObject upInstallView;

    // Token: 0x040002F2 RID: 754
    public GameObject upInstallContent;

    // Token: 0x040002F3 RID: 755
    public GameObject crystallSection;

    // Token: 0x040002F4 RID: 756
    public GameObject canvasGUI;

    // Token: 0x040002F5 RID: 757
    public GameObject paint;

    // Token: 0x040002F6 RID: 758
    public GameObject inventory;

    // Token: 0x040002F7 RID: 759
    public GameObject inventoryItemPrefab;

    // Token: 0x040002F8 RID: 760
    public Button upInstallPrefab;

    // Token: 0x040002F9 RID: 761
    public Button upButton;

    // Token: 0x040002FA RID: 762
    public Button adminButton;

    // Token: 0x040002FB RID: 763
    public Button deleteButton;

    // Token: 0x040002FC RID: 764
    public Button lockButton;

    // Token: 0x040002FD RID: 765
    public Sprite lockButtonLocked;

    // Token: 0x040002FE RID: 766
    public Sprite lockButtonUnlocked;

    // Token: 0x040002FF RID: 767
    public Button upExitButton;

    // Token: 0x04000300 RID: 768
    public Button exitButton;

    // Token: 0x04000301 RID: 769
    public Image upImage;

    // Token: 0x04000302 RID: 770
    public Text upText;

    // Token: 0x04000303 RID: 771
    public MyInputField inputText;

    // Token: 0x04000304 RID: 772
    public GameObject fillLinePrefab;

    // Token: 0x04000305 RID: 773
    public GameObject buttonLinePrefab;

    // Token: 0x04000306 RID: 774
    public GameObject clanLinePrefab;

    // Token: 0x04000307 RID: 775
    public GameObject toggleLinePrefab;

    // Token: 0x04000308 RID: 776
    public GameObject uintLinePrefab;

    // Token: 0x04000309 RID: 777
    public GameObject dropdownLinePrefab;

    // Token: 0x0400030A RID: 778
    public GameObject textLinePrefab;

    // Token: 0x0400030B RID: 779
    public GameObject cardLinePrefab;

    // Token: 0x0400030C RID: 780
    public GameObject cardPrefab;

    // Token: 0x0400030D RID: 781
    public GameObject canvasButtonPrefab;

    // Token: 0x0400030E RID: 782
    public GameObject canvasMicroButtonPrefab;

    // Token: 0x0400030F RID: 783
    public GameObject canvasTextFieldPrefab;

    // Token: 0x04000310 RID: 784
    public GameObject canvasWebImagePrefab;

    // Token: 0x04000311 RID: 785
    public GameObject canvasRectPrefab;

    // Token: 0x04000312 RID: 786
    public GameObject canvasLinePrefab;

    // Token: 0x04000313 RID: 787
    public GameObject canvasTPButtonPrefab;

    // Token: 0x04000314 RID: 788
    public GameObject multitextPrefab;

    // Token: 0x04000315 RID: 789
    public GameObject centeredImagePrefab;

    // Token: 0x04000316 RID: 790
    public GameObject listContent;

    // Token: 0x04000317 RID: 791
    public GameObject richContent;

    // Token: 0x04000318 RID: 792
    public GameObject scrollView;

    // Token: 0x04000319 RID: 793
    public GameObject skillButtonPrefab;

    // Token: 0x0400031A RID: 794
    public GameObject pad340;

    // Token: 0x0400031B RID: 795
    public Button[] buttons;

    // Token: 0x0400031C RID: 796
    public Button backButton;

    // Token: 0x0400031D RID: 797
    public static PopupManager THIS;

    // Token: 0x0400031E RID: 798
    private bool disableKeyboard;

    // Token: 0x0400031F RID: 799
    private string backButtonAction = "";

    // Token: 0x04000320 RID: 800
    private string str101 = "{\"b\":\"";

    // Token: 0x04000321 RID: 801
    private string str102 = "GUI_";

    // Token: 0x04000322 RID: 802
    private string str103 = "exit";

    // Token: 0x04000323 RID: 803
    private string str104 = "back";

    // Token: 0x04000324 RID: 804
    private string str105 = "ADMN";

    // Token: 0x04000325 RID: 805
    private string str106 = "</color>";

    // Token: 0x04000326 RID: 806
    private string str107 = "%C%";

    // Token: 0x04000327 RID: 807
    private string str108 = "</size>";

    // Token: 0x04000328 RID: 808
    private string str109 = "%S%";

    // Token: 0x04000329 RID: 809
    private string str110 = "\"}";

    // Token: 0x0400032A RID: 810
    private static string statstr1 = "#";

    // Token: 0x0400032B RID: 811
    private string str111 = ":";

    // Token: 0x0400032C RID: 812
    private string str112 = "";

    // Token: 0x0400032E RID: 814
    private HORBConfig horbcfg;

    // Token: 0x0400032F RID: 815
    private UPConfig upcfg;

    // Token: 0x04000330 RID: 816
    private string mode = "horb";

    // Token: 0x04000331 RID: 817
    public static int[] slotPositions = new int[]
    {
        20,
        110,
        21,
        153,
        38,
        75,
        38,
        191,
        74,
        94,
        74,
        169,
        59,
        132,
        100,
        132,
        51,
        22,
        93,
        36,
        51,
        241,
        93,
        226,
        281,
        50,
        256,
        121,
        312,
        23,
        307,
        87,
        377,
        27,
        376,
        76,
        374,
        137,
        375,
        176,
        165,
        46,
        145,
        131,
        185,
        142,
        332,
        153,
        203,
        24,
        248,
        76,
        341,
        57,
        343,
        103,
        138,
        87,
        248,
        17,
        297,
        133,
        214,
        115,
        175,
        98,
        205,
        68
    };

    // Token: 0x04000332 RID: 818
    public GameObject[] richListObjects;

    // Token: 0x04000333 RID: 819
    public GameObject blinkingObject;

    // Token: 0x04000334 RID: 820
    private bool fixScroll;

    // Token: 0x04000335 RID: 821
    private string fixScrollTag = "";

    private string invbutton = "";

    // Token: 0x04000336 RID: 822
    private bool bigInput;

    // Token: 0x04000337 RID: 823
    private Dictionary<string, float> fixScrolls = new Dictionary<string, float>();

    // Token: 0x04000338 RID: 824
    private string lastInputString = "";

}
