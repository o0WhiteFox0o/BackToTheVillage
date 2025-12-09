// 
// Member: LinhH
// Date: 09/12/2025
// 


using System.Collections.Generic;
using UnityEngine;


public class UI_MainMenuUIManager : MonoBehaviour
{
    [Header("Audio Clip")]
    [SerializeField] private AudioClip sfxButtonPress;

    private UI_SavedGameUIManager savedFarmUIManager;
    private UI_StartUIManager startUIManager;
    private UI_SettingUIManager settingUIManager;

    private Stack<GameObject> openedUIs = new Stack<GameObject>();          // Stack các UI đang được mở trong gameplay


    void Start()
    {
        savedFarmUIManager = GetComponentInChildren<UI_SavedGameUIManager>();
        startUIManager = GetComponentInChildren<UI_StartUIManager>();
        settingUIManager = GetComponentInChildren<UI_SettingUIManager>();

        if (savedFarmUIManager == null || startUIManager == null || settingUIManager == null)
        {
            Debug.LogError("Can't load component!!!");
        }
    }


    /// <summary>
    /// Tắt UI đầu tiên trong stack opened UI.
    /// </summary>
    public void DisableUI()
    {
        if (openedUIs.Count == 0) { return; }

        //ẩn UI đầu tiên trong stack opened UI
        openedUIs.Peek().SetActive(false);
        MGR_AudioManager.Instance.PlaySFX(sfxButtonPress);
        RefreshUILayer();
    }


    private void RefreshUILayer()
    {
        if (openedUIs.Count != 0) { openedUIs.Pop(); }
        if (openedUIs.Count == 0) { return; }

        openedUIs.Peek().transform.SetAsLastSibling();
    }


    public void EnableSavedFarmUI()
    {
        // nếu giao diện đang được mở thì không làm gì
        if (savedFarmUIManager.background.activeSelf) { return; }

        MGR_AudioManager.Instance.PlaySFX(sfxButtonPress);
        savedFarmUIManager.EnableSavedFarmUI();
        savedFarmUIManager.transform.SetAsLastSibling();

        openedUIs.Push(savedFarmUIManager.background);
    }


    public void EnableStartUI()
    {
        // nếu giao diện đang được mở thì không làm gì
        if (startUIManager.background.activeSelf) { return; }

        MGR_AudioManager.Instance.PlaySFX(sfxButtonPress);
        startUIManager.EnableStartUI();
        startUIManager.transform.SetAsLastSibling();

        openedUIs.Push(startUIManager.background);
    }


    public void EnableSettingUI()
    {
        // nếu giao diện đang được mở thì không làm gì
        if (settingUIManager.background.activeSelf) { return; }

        MGR_AudioManager.Instance.PlaySFX(sfxButtonPress);
        settingUIManager.EnableSettingUI();
        settingUIManager.transform.SetAsLastSibling();

        openedUIs.Push(settingUIManager.background);
    }
}
