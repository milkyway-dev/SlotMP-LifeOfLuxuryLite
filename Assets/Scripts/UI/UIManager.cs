using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private Button Paytable_Button;

    [Header("Popus UI")]
    [SerializeField] private GameObject MainPopup_Object;

    [Header("Paytable Popup")]
    [SerializeField] private GameObject[] Pages;
    [SerializeField] private GameObject PaytablePopup_Object;
    [SerializeField] private Button PaytableExit_Button;
    [SerializeField] private Button PaytableLeft_Button;
    [SerializeField] private Button PaytableRight_Button;
    [SerializeField] private TMP_Text[] SymbolsText;
    [SerializeField] private TMP_Text FreeSpin_Text;
    [SerializeField] private TMP_Text Scatter_Text;
    [SerializeField] private TMP_Text Jackpot_Text;
    [SerializeField] private TMP_Text Bonus_Text;
    [SerializeField] private TMP_Text Wild_Text;

    [Header("Sound/Music Ref")]
    [SerializeField] private Button Sound_Button;
    [SerializeField] private Button Music_Button;
    [SerializeField] private Sprite SoundOff_Sprite;
    [SerializeField] private Sprite SoundOn_Sprite;
    [SerializeField] private Sprite MusicOff_Sprite;
    [SerializeField] private Sprite MusicOn_Sprite;
    
    [Header("Win Popup")]
    [SerializeField] private Sprite BigWin_Sprite;
    [SerializeField] private Sprite HugeWin_Sprite;
    [SerializeField] private Sprite MegaWin_Sprite;
    [SerializeField] private Sprite Jackpot_Sprite;
    [SerializeField] private Image Win_Image;
    [SerializeField] private GameObject WinPopup_Object;
    [SerializeField] private TMP_Text Win_Text;
    [SerializeField] private Button SkipWinAnimation;

    [Header("FreeSpins Popup")]
    [SerializeField] private GameObject FreeSpinPopup_Object;
    [SerializeField] private TMP_Text Free_Text;

    [Header("Disconnection Popup")]
    [SerializeField] private Button CloseDisconnect_Button;
    [SerializeField] private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField] private Button CloseAD_Button;
    [SerializeField] private GameObject ADPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField] private Button LBExit_Button;
    [SerializeField] private GameObject LBPopup_Object;

    [Header("Quit Popup")]
    [SerializeField] private GameObject QuitPopup_Object;
    [SerializeField] private Button YesQuit_Button;
    [SerializeField] private Button NoQuit_Button;
    [SerializeField] private Button CrossQuit_Button;
    [SerializeField] private Button GameExit_Button;

    [Header("Script Ref")]
    [SerializeField] private AudioController audioController;
    [SerializeField] private SlotBehaviour slotManager;
    [SerializeField] private SocketIOManager socketManager;
    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;
    private Tween WinPopupTextTween;
    private Tween ClosePopupTween;
    internal int FreeSpins;
    private int paytablePageCounter;
    private void Start()
    {
        isMusic = true;
        isSound = true;

        if (audioController) audioController.ToggleMute(false);

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate {OpenPatable();});

        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if(PaytableLeft_Button) PaytableLeft_Button.onClick.RemoveAllListeners();
        if(PaytableLeft_Button) PaytableLeft_Button.onClick.AddListener(()=>{SwitchPages(false);});

        if(PaytableRight_Button) PaytableRight_Button.onClick.RemoveAllListeners();
        if(PaytableRight_Button) PaytableRight_Button.onClick.AddListener(()=>{SwitchPages(true);});

        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate { 
            OpenPopup(QuitPopup_Object);
            });

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate { if (!isExit) { 
            ClosePopup(QuitPopup_Object);
            } });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate { if (!isExit) { 
            ClosePopup(QuitPopup_Object);
            } });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate{
            CallOnExitFunction();
            });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
        if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

        if (Music_Button) Music_Button.onClick.RemoveAllListeners();
        if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

        if(SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
        if(SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);
    }

    private void OpenPatable(){
        audioController.PlayButtonAudio();
        foreach (GameObject gameObject in Pages)
        {
            gameObject.SetActive(false);
        }
        paytablePageCounter=0;
        Pages[0].SetActive(true);
        MainPopup_Object.SetActive(true);
        PaytablePopup_Object.SetActive(true);
    }

    private void SwitchPages(bool IncDec){
        if(IncDec){
            paytablePageCounter++;
            if(paytablePageCounter==Pages.Length){
                paytablePageCounter=0;
            }
            foreach (GameObject gameObject in Pages)
            {
                gameObject.SetActive(false);
            }
            Pages[paytablePageCounter].SetActive(true);
        }
        else{
            paytablePageCounter--;
            if(paytablePageCounter==-1){
                paytablePageCounter=Pages.Length-1;
            }
            foreach (GameObject gameObject in Pages)
            {
                gameObject.SetActive(false);
            }
            Pages[paytablePageCounter].SetActive(true);
        }
    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup(bool isReconnection)
    {
        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void PopulateWin(int value, double amount)
    {
        switch(value)
        {
            case 1:
                if (Win_Image) Win_Image.sprite = BigWin_Sprite;
                break;
            case 2:
                if (Win_Image) Win_Image.sprite = HugeWin_Sprite;
                break;
            case 3:
                if (Win_Image) Win_Image.sprite = MegaWin_Sprite;
                break;
            case 4:
                if (Win_Image) Win_Image.sprite = Jackpot_Sprite;
                break;
        }

        StartPopupAnim(amount);
    }

    private void StartFreeSpins(int spins)
    {
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(false);
        slotManager.FreeSpin(spins);
    }

    internal void FreeSpinProcess(int spins)
    {
        int ExtraSpins=spins-FreeSpins;
        FreeSpins=spins;
        Debug.Log("ExtraSpins: " +ExtraSpins);
        Debug.Log("Total Spins: " +spins);
        if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(true);           
        if (Free_Text) Free_Text.text = ExtraSpins.ToString() + " Free spins awarded.";
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        DOVirtual.DelayedCall(2f, ()=>{
            StartFreeSpins(spins);
        });
    }

    void SkipWin(){
        Debug.Log("Skip win called");
        if(ClosePopupTween!=null){
            ClosePopupTween.Kill();
            ClosePopupTween=null;
        }
        if(WinPopupTextTween!=null){
            WinPopupTextTween.Kill();
            WinPopupTextTween=null;
        }
        ClosePopup(WinPopup_Object);
        slotManager.CheckPopups = false;
    }

    private void StartPopupAnim(double amount)
    {
        double initAmount = 0;
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        WinPopupTextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("F3");
        });

        ClosePopupTween = DOVirtual.DelayedCall(6f, () =>
        {
            ClosePopup(WinPopup_Object);
            slotManager.CheckPopups = false;
        });
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object); 
    }

    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {
            string text = null;
            if (paylines.symbols[i].Multiplier[0][0] != 0)
            {
                text += "5x - " + paylines.symbols[i].Multiplier[0][0]+"x";
            }
            if (paylines.symbols[i].Multiplier[1][0] != 0)
            {
                text += "\n4x - " + paylines.symbols[i].Multiplier[1][0]+"x";
            }
            if (paylines.symbols[i].Multiplier[2][0] != 0)
            {
                text += "\n3x - " + paylines.symbols[i].Multiplier[2][0]+"x";
            }
            if (SymbolsText[i]) SymbolsText[i].text = text;
        }

        for (int i = 0; i < paylines.symbols.Count; i++)
        {
            if (paylines.symbols[i].Name.ToUpper() == "FREESPIN")
            {
                if (FreeSpin_Text) FreeSpin_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].Name.ToUpper() == "SCATTER")
            {
                if (Scatter_Text) Scatter_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].Name.ToUpper() == "JACKPOT")
            {
                if (Jackpot_Text) Jackpot_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].Name.ToUpper() == "BONUS")
            {
                if (Bonus_Text) Bonus_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].Name.ToUpper() == "WILD")
            {
                if (Wild_Text) Wild_Text.text = paylines.symbols[i].description.ToString();
            }
        }
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        slotManager.CallCloseSocket();
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf) 
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    private void ToggleMusic()
    {
        if (isMusic)
        {
            Music_Button.GetComponent<Image>().sprite=MusicOff_Sprite;
            audioController.ToggleMute(false, "bg");
            isMusic=false;
        }
        else
        {
            Music_Button.GetComponent<Image>().sprite=MusicOn_Sprite;
            audioController.ToggleMute(true, "bg");
            isMusic=true;
        }
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private void ToggleSound()
    {
        if (isSound)
        {
            Sound_Button.GetComponent<Image>().sprite=SoundOff_Sprite;
            if (audioController) audioController.ToggleMute(false,"button");
            if (audioController) audioController.ToggleMute(false,"wl");
            isSound=false;
        }
        else
        {
            Sound_Button.GetComponent<Image>().sprite=SoundOn_Sprite;
            if(audioController) audioController.ToggleMute(true,"button");
            if (audioController) audioController.ToggleMute(true,"wl");
            isSound=true;
        }
    }
}
