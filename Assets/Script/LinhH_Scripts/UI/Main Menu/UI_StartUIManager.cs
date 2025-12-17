// 
// Member: LinhH
// Date: 09/12/2025
// 


using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public GameObject background;

    [Header("Buttons")]
    [SerializeField] public Button backButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button maleSelectionButton;
    [SerializeField] private Button femaleSelectionButton;

    [Header("Appearance Buttons")]
    [SerializeField] private Button previousHatButton;
    [SerializeField] private Button nextHatButton;
    [SerializeField] private Button previousFaceButton;
    [SerializeField] private Button nextFaceButton;
    [SerializeField] private Button previousHairButton;
    [SerializeField] private Button nextHairButton;
    [SerializeField] private Button previousClothesButton;
    [SerializeField] private Button nextClothesButton;

    [Header("Appearance Display")]
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image hairImage;
    [SerializeField] private Image clothesImage;
    [SerializeField] private Image faceImage;
    [SerializeField] private Image hatImage;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField farmNameInput;
    [SerializeField] private TMP_InputField characterNameInput;
    [SerializeField] private TMP_InputField favoriteInput;

    // [Header("Scriptable Objects")]

    private UI_MainMenuUIManager mainMenuUIManager;

    private ClothSetSO[] maleClothesSets;
    private HairSetSO[] maleHairSets;
    private FaceSetSO[] maleFaceSets;
    // TODO: hat

    private ClothSetSO[] femaleClothesSets;
    private HairSetSO[] femaleHairSets;
    private FaceSetSO[] femaleFaceSets;
    // TODO: hat

    private List<ClothSetSO> cacheClothesSets = new List<ClothSetSO>();
    private List<HairSetSO> cacheHairSets = new List<HairSetSO>();
    private List<FaceSetSO> cacheFaceSets = new List<FaceSetSO>();
    // TODO: hat

    private int currentClothesIndex;
    private int currentFaceIndex;
    private int currentHairIndex;


    private MGR_MainMenuManager mainMenuManager;


    void Start()
    {
        mainMenuUIManager = GetComponentInParent<UI_MainMenuUIManager>();
        mainMenuManager = FindObjectOfType<MGR_MainMenuManager>();

        if (mainMenuUIManager == null || mainMenuManager == null)
        {
            Debug.LogError("Can't load component!!!");
        }

        AddListenerForButtons();
        LoadCharacterAppearance();

        startButton.onClick.AddListener(StartGame);
    }


    void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();
        maleSelectionButton.onClick.RemoveAllListeners();
        femaleSelectionButton.onClick.RemoveAllListeners();

        nextClothesButton.onClick.RemoveAllListeners();
        nextFaceButton.onClick.RemoveAllListeners();
        nextHairButton.onClick.RemoveAllListeners();
        nextHatButton.onClick.RemoveAllListeners();

        previousClothesButton.onClick.RemoveAllListeners();
        previousFaceButton.onClick.RemoveAllListeners();
        previousHairButton.onClick.RemoveAllListeners();
        previousHatButton.onClick.RemoveAllListeners();

        startButton.onClick.RemoveAllListeners();
    }


    public void EnableStartUI()
    {
        background.SetActive(true);
        SelectGender(true);
        RefreshCharacterAppearance();
    }


    private void AddListenerForButtons()
    {
        backButton.onClick.AddListener(mainMenuUIManager.DisableUI);

        maleSelectionButton.onClick.AddListener(() => SelectGender(true));
        femaleSelectionButton.onClick.AddListener(() => SelectGender(false));

        nextClothesButton.onClick.AddListener(() => ChangeNextAppearance(AppearanceType.Clothes));
        nextFaceButton.onClick.AddListener(() => ChangeNextAppearance(AppearanceType.Face));
        nextHairButton.onClick.AddListener(() => ChangeNextAppearance(AppearanceType.Hair));
        nextHatButton.onClick.AddListener(() => ChangeNextAppearance(AppearanceType.Hat));

        previousClothesButton.onClick.AddListener(() => ChangePreviousAppearance(AppearanceType.Clothes));
        previousFaceButton.onClick.AddListener(() => ChangePreviousAppearance(AppearanceType.Face));
        previousHairButton.onClick.AddListener(() => ChangePreviousAppearance(AppearanceType.Hair));
        previousHatButton.onClick.AddListener(() => ChangePreviousAppearance(AppearanceType.Hat));
    }


    private void LoadCharacterAppearance()
    {
        maleClothesSets = Resources.LoadAll<ClothSetSO>("Appearance/Cloth/Male/");
        maleHairSets = Resources.LoadAll<HairSetSO>("Appearance/Hair/Male/");
        maleFaceSets = Resources.LoadAll<FaceSetSO>("Appearance/Face/Male/");
        // TODO: hat

        femaleClothesSets = Resources.LoadAll<ClothSetSO>("Appearance/Cloth/Female/");
        femaleHairSets = Resources.LoadAll<HairSetSO>("Appearance/Hair/Female/");
        femaleFaceSets = Resources.LoadAll<FaceSetSO>("Appearance/Face/Female/");
        // TODO: hat

        if (maleClothesSets.Length == 0 || maleFaceSets.Length == 0 || maleHairSets.Length == 0 ||
            femaleClothesSets.Length == 0 || femaleFaceSets.Length == 0 || femaleHairSets.Length == 0)
        {
            Debug.LogError("Can't load character appearance from Resources!!!");
        }
    }


    private void RefreshCharacterAppearance()
    {
        currentClothesIndex = 0;
        currentFaceIndex = 0;
        currentHairIndex = 0;
        // TODO: hat

        clothesImage.sprite = cacheClothesSets[currentClothesIndex].RD_idle[0];
        faceImage.sprite = cacheFaceSets[currentFaceIndex].RD_idle[0];
        hairImage.sprite = cacheHairSets[currentHairIndex].RD_idle[0];
        // TODO: hat
    }


    private void SelectGender(bool isMale)
    {
        cacheClothesSets.Clear();
        cacheFaceSets.Clear();
        cacheHairSets.Clear();
        // TODO: hat

        if (isMale)
        {
            // bật highlight của male button và tắt highlight female button
            maleSelectionButton.transform.GetChild(0).GetComponent<Image>().enabled = true;
            femaleSelectionButton.transform.GetChild(0).GetComponent<Image>().enabled = false;

            cacheClothesSets = maleClothesSets.ToList();
            cacheFaceSets = maleFaceSets.ToList();
            cacheHairSets = maleHairSets.ToList();
            // TODO: hat
        }
        else
        {
            // bật highlight của female button và tắt highlight male button
            maleSelectionButton.transform.GetChild(0).GetComponent<Image>().enabled = false;
            femaleSelectionButton.transform.GetChild(0).GetComponent<Image>().enabled = true;

            cacheClothesSets = femaleClothesSets.ToList();
            cacheFaceSets = femaleFaceSets.ToList();
            cacheHairSets = femaleHairSets.ToList();
            // TODO: hat
        }
    }


    private void ChangeNextAppearance(AppearanceType appearanceType)
    {
        switch (appearanceType)
        {
            case AppearanceType.Clothes:
                ChangeClothes(1);
                break;

            case AppearanceType.Face:
                ChangeFace(1);
                break;

            case AppearanceType.Hair:
                ChangeHair(1);
                break;

            case AppearanceType.Hat:
                ChangeHat(1);
                break;
        }
    }


    private void ChangePreviousAppearance(AppearanceType appearanceType)
    {
        switch (appearanceType)
        {
            case AppearanceType.Clothes:
                ChangeClothes(-1);
                break;

            case AppearanceType.Face:
                ChangeFace(-1);
                break;

            case AppearanceType.Hair:
                ChangeHair(-1);
                break;

            case AppearanceType.Hat:
                ChangeHat(-1);
                break;
        }
    }


    /// <param name="state"> Có hai giá trị hợp lệ là 1 và -1, đại diện cho lựa chọn next hoặc prrevious của người chơi. </param>
    private void ChangeClothes(int state)
    {
        // không làm gì nếu giá trị truyền vào không hợp lệ
        if (state != 1 && state != -1) { return; }

        // cập nhật giá trị của current clothes index
        currentClothesIndex += state;
        if (currentClothesIndex >= cacheClothesSets.Count) { currentClothesIndex = 0; }
        else if (currentClothesIndex < 0) { currentClothesIndex = cacheClothesSets.Count - 1; }

        // cập nhật UI dựa trên current clothes index
        clothesImage.sprite = cacheClothesSets[currentClothesIndex].RD_idle[0];
    }


    /// <param name="state"> Có hai giá trị hợp lệ là 1 và -1, đại diện cho lựa chọn next hoặc prrevious của người chơi. </param>
    private void ChangeFace(int state)
    {
        if (state != 1 && state != -1) { return; }


    }


    /// <param name="state"> Có hai giá trị hợp lệ là 1 và -1, đại diện cho lựa chọn next hoặc prrevious của người chơi. </param>
    private void ChangeHair(int state)
    {
        if (state != 1 && state != -1) { return; }


    }


    /// <param name="state"> Có hai giá trị hợp lệ là 1 và -1, đại diện cho lựa chọn next hoặc prrevious của người chơi. </param>
    private void ChangeHat(int state)
    {
        if (state != 1 && state != -1) { return; }


    }


    private void StartGame()
    {
        if (farmNameInput.text.Length == 0 || characterNameInput.text.Length == 0 || favoriteInput.text.Length == 0)
        {
            Debug.Log("Nhập thiếu thông tin. Hãy nhập lại đầy đủ!!!");

            // TODO: hiển thị thông báo người chơi nhập thiếu thông tin
            return;
        }

        mainMenuManager.StartNewGame(farmNameInput.text, characterNameInput.text);
    }
}