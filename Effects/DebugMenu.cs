using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using RepoDice.Dice;
using RepoDice.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RepoDice;

public class DebugMenu : MonoBehaviour
{
    public static Color mainColor = new Color(0.15294117647058825f, 0f, 0.3176470588235294f, 1f);
    public static Color ButtonColor = new Color(0.6430231f, 0.2783019f, 1, 1);
    public static Color TextColor = new Color(0.9607844f, 0.5058824f, 0.9803922f, 1);
    public static Color FavoriteTextColor = new Color(0.9607844f, 0.2058824f, 0.2803922f, 1);
    public static Color BackgroundColor = new Color(0.6039216f, 0.2392157f, 1f, 0.2f);
    public static Color AccentColor = new Color(0.6078432f, 0.2588235f, 0.9921569f, 0.4627451f);
    
    public static GameObject EffectMenu = null!;
    public static bool ran;
    public static List<Image> backgroundImages = new List<Image>();
    public static List<Image> accentImages = new List<Image>();
    public static List<Image> buttonImages = new List<Image>();
    public static List<TMP_Text> textElements = new List<TMP_Text>();
    public static List<Text> textElements2 = new List<Text>();
    public static Transform mainScrollContent;
    public static Transform subScrollContent;
    
    public static void SetupColors()
    {
        if (!RepoDice.DebugMenuColorUsesPlayerColor.Value)
        { 
            if (!ColorUtility.TryParseHtmlString(RepoDice.DebugMenuColor.Value, out mainColor))
                ColorUtility.TryParseHtmlString("#270051", out mainColor);
            BackgroundColor = mainColor;
            BackgroundColor.a = 0.46f;
        }
        else
        {
            mainColor = PlayerAvatar.instance.playerAvatarVisuals.color;
            BackgroundColor = PlayerAvatar.instance.playerAvatarVisuals.color;
            BackgroundColor.a = 0.46f;
        }
        (var brighter, var darker) = ColorHelper.GenerateColors(mainColor);
        ButtonColor = brighter;
        AccentColor = darker;

       
        TextColor = ColorHelper.GetReadableTextColor(mainColor);
        FavoriteTextColor = ColorHelper.GetEmphasisTextColor(mainColor);
    }
    public static void setupElements()
    {
        buttonImages.Add(RepoDice.DebugSubButtonPrefab.GetComponent<Image>());
        buttonImages.Add(RepoDice.DebugMenuButtonPrefab.GetComponent<Image>());
        textElements.Add(RepoDice.DebugSubButtonPrefab.GetComponentInChildren<TMP_Text>());
        textElements.Add(RepoDice.DebugMenuButtonPrefab.GetComponentInChildren<TMP_Text>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/Scroll View/Scrollbar Vertical").GetComponent<Image>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/ClearButton").GetComponent<Image>());
        backgroundImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/TopPart").GetComponent<Image>());
        backgroundImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/Image").GetComponent<Image>());
        backgroundImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background").GetComponent<Image>());
        accentImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/Border").GetComponent<Image>());
        accentImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/Scroll View").GetComponent<Image>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/TopPart/Border/Title").GetComponent<TMP_Text>());
        accentImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/TopPart/Border").GetComponent<Image>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Select Effect").GetComponent<Image>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Select Effect/Text (TMP)").GetComponent<TMP_Text>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/ClearButton/Text (TMP)").GetComponent<TMP_Text>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Spawn Menu").GetComponent<Image>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Spawn Menu/Text (TMP)").GetComponent<TMP_Text>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Player Functions").GetComponent<Image>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Player Functions/Text (TMP)").GetComponent<TMP_Text>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Special Functions").GetComponent<Image>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Special Functions/Text (TMP)").GetComponent<TMP_Text>());
        buttonImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Grant Admin").GetComponent<Image>());
        textElements.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Grant Admin/Text (TMP)").GetComponent<TMP_Text>());
        accentImages.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Scroll View").GetComponent<Image>());
        textElements2.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Toggle/Label").GetComponent<Text>());
        textElements2.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Bald/Label").GetComponent<Text>());
        setupSelectElements();
        ran = true;
    }
    public static void setupSelectElements()
    {
        //PrintHierarchy(RepoDice.NewSelectMenuPrefab.transform);
        buttonImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/Scroll View/Scrollbar Vertical").GetComponent<Image>());
        buttonImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/ClearButton").GetComponent<Image>());
        backgroundImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/TopPart").GetComponent<Image>());
        backgroundImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/Image").GetComponent<Image>());
        backgroundImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background").GetComponent<Image>());
        accentImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/Border").GetComponent<Image>());
        accentImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/Scroll View").GetComponent<Image>());
        textElements.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/TopPart/Border/Title").GetComponent<TMP_Text>());
        accentImages.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/TopPart/Border").GetComponent<Image>());
        textElements.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/ClearButton/Text (TMP)").GetComponent<TMP_Text>());
        textElements2.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Bald/Label").GetComponent<Text>());
        textElements.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/KeepOpen/Label").GetComponent<TMP_Text>());
        textElements.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/ColorPlayer/Label").GetComponent<TMP_Text>());
    }
    public static void setColors()
    {
        var colorBlock = RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Background/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().colors;
        var colorBlock2 = RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/Background/Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>().colors;
        colorBlock.normalColor = TextColor;
        colorBlock.highlightedColor = TextColor;
        colorBlock.pressedColor = TextColor;
        colorBlock.selectedColor = TextColor;
        colorBlock2.normalColor = TextColor;
        colorBlock2.highlightedColor = TextColor;
        colorBlock2.pressedColor = TextColor;
        colorBlock2.selectedColor = TextColor;

        foreach (var b in buttonImages)
        {
            b.color = ButtonColor;
        }
        foreach (var text in textElements)
        {
            text.color = TextColor;
        }
        foreach (var text2 in textElements2)
        {
            text2.color = TextColor;
        }
        foreach (var accent in accentImages)
        {
            accent.color = AccentColor;
        }
        foreach (var background in backgroundImages)
        {
            background.color = BackgroundColor;
        }
    }
    
    public static void showDebugMenu(bool fulls, bool completes, bool sus = false)
    {

        SetupColors();
        if (!ran) setupElements();
        if (ran) setColors();
        if (EffectMenu != null)
        {
            GameObject.Destroy(EffectMenu);
            EffectMenu = null;
        }

        EffectMenu = GameObject.Instantiate(RepoDice.DebugMenuPrefab);
    }
     public static void ShowSelectEffectMenu(bool fromSaint = false)
    {   
        SetupColors();
        if(!ran) setupElements();
        if(ran) setColors();
        
        if (EffectMenu != null)
        {
            GameObject.Destroy(EffectMenu);
            EffectMenu = null;
        }
        
        EffectMenu = GameObject.Instantiate(RepoDice.SelectMenuPrefab, null, false);
        Canvas canvas = EffectMenu.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 999;
        EffectMenu.transform.localPosition = Vector3.zero;
        EffectMenu.transform.localScale = Vector3.one;
        EffectMenu.transform.localRotation = Quaternion.identity;
        
        mainScrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");
        InputAction escAction = new InputAction(binding: "<Keyboard>/escape");
        escAction.performed += ctx => CloseSelectMenu();
        escAction.Enable();

        TMP_InputField searchInput = EffectMenu.transform.Find("DebugMenu/Background/SearchField").GetComponent<TMP_InputField>();
        searchInput.onValueChanged.AddListener(FilterItems);
        Button ClearButton = EffectMenu.transform.Find("DebugMenu/Background/ClearButton").GetComponent<Button>();
        ClearButton.onClick.AddListener(() =>
        {
            searchInput.text = "";
        });
        if (EffectMenu == null)
        {
            Debug.LogError("EffectMenu is null after instantiation!");
            return;
        }
        DebugMenuController controller = EffectMenu.AddComponent<DebugMenuController>();
        controller.EffectMenu = EffectMenu;
        
        Toggle ColorToggle = EffectMenu.transform.Find("DebugMenu/ColorPlayer").GetComponent<Toggle>();
        ColorToggle.isOn = RepoDice.DebugMenuColorUsesPlayerColor.Value;
        Toggle KeepOpenToggle = EffectMenu.transform.Find("DebugMenu/KeepOpen").GetComponent<Toggle>();
        if(fromSaint)KeepOpenToggle.gameObject.SetActive(false);
        KeepOpenToggle.isOn = !RepoDice.DebugMenuClosesAfter.Value;
        ColorToggle.onValueChanged.AddListener((bool isOn) =>
        {
            RepoDice.DebugMenuColorUsesPlayerColor.Value = isOn;
        });
        if (KeepOpenToggle.gameObject.activeInHierarchy)
        {
            KeepOpenToggle.onValueChanged.AddListener((bool isOn) =>
            {
                RepoDice.DebugMenuClosesAfter.Value = !isOn;
            });
        }
       
        
        Toggle valueChanged2 = EffectMenu.transform.Find("DebugMenu/Bald").GetComponent<Toggle>();
        valueChanged2.isOn = RepoDice.Bald.Value;
        GameObject BaldImage = EffectMenu.transform.Find("DebugMenu/Background/Image").gameObject;
        valueChanged2.onValueChanged.AddListener((bool isOn) =>
        {
            RepoDice.Bald.Value = isOn;
            BaldImage.SetActive(isOn);
        });
        if (RepoDice.Bald.Value)
        {
            BaldImage.SetActive(true);
        }
        else
        {
            BaldImage.SetActive(false);
        }
        
        Button exitButton = EffectMenu.transform.Find("DebugMenu/CloseButton").GetComponent<Button>();
        exitButton.onClick.AddListener(() =>
        {
            CloseSelectMenu(true);
        });
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ShowSelectMenu(fromSaint);
    }
    public static void CloseSelectMenu(bool force = false) // 
    {
        if (RepoDice.DebugMenuClosesAfter.Value || force)
        {
            if (EffectMenu != null)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                GameObject.Destroy(EffectMenu);
            }
        }
    } 
    static void FilterItems(string searchText)
    {
        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");
        foreach (Transform child in scrollContent)
        {
            var textComponent = child.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent)
            {
                bool matches = string.IsNullOrEmpty(searchText) || textComponent.text.ToLower().Contains(searchText.ToLower());
                child.gameObject.SetActive(matches);
            }
        }
    }

    public static void clearMainViewport()
    {
        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");
        foreach (Transform obj in scrollContent)
        {
            Destroy(obj.gameObject);
        }
    }

    public static void clearSubViewport()
    {
        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Scroll View/Viewport/Content");
        foreach (Transform obj in scrollContent)
        {
            Destroy(obj.gameObject);
        }
    }
    public static List<IEffect> getOrdered()
    {
        List<IEffect> effects = new List<IEffect>(DieBehaviour.AllowedEffects);

        if(Misc.GetLocalPlayer().steamID == RepoDice.slayerSteamID) effects.Add(new InstantReroll());
        FavoriteEffectManager.FavoriteData favoritesData = FavoriteEffectManager.LoadFavorites();
        List<string> favoriteEffectNames = favoritesData.Favorites;

        List<IEffect> sortedEffects = effects
            .OrderByDescending(effect => favoriteEffectNames.Contains(effect.Name))  // Favorites first
            .ThenBy(effect => effect.Name, StringComparer.OrdinalIgnoreCase)         // Sort alphabetically
            .ToList();

        return sortedEffects;
    }

       public static void ShowSelectMenu(bool fromSaint = false)
    {
        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");

        List<IEffect> effects = getOrdered();
        FavoriteEffectManager.FavoriteData favoritesData = FavoriteEffectManager.LoadFavorites();
        List<string> favoriteEffectNames = favoritesData.Favorites;

        foreach (IEffect effect in effects)
        {
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, scrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();

            bool isFavorite = favoriteEffectNames.Contains(effect.Name);
            string favoriteMarker = isFavorite ? "*" : "";

            buttonText.text = $"{favoriteMarker} {effect.Name} [{effect.Outcome}] {favoriteMarker}";
            buttonText.color = isFavorite ? FavoriteTextColor : TextColor;
            buttonText.outlineWidth = 1;

            if (buttonText.text.Length > 20)
            {
                buttonText.fontSize = 12;
            }
            else
            {
                buttonText.fontSize = 16;
            }

            Button button = effectObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                CloseSelectMenu(fromSaint);
                if (SemiFunc.IsMasterClientOrSingleplayer())
                {
                    Networker.Instance.SelectEffectMenuRPC(fromSaint, PlayerAvatar.instance.photonView.ViewID, effect.Name);
                }
                else
                {
                    Networker.Instance.photonView.RPC("SelectEffectMenuRPC",RpcTarget.MasterClient, fromSaint, PlayerAvatar.instance.photonView.ViewID, effect.Name);
                }
                bool useCustom = false;
                string nameToUse = "";
            });
            RightClickHandler2 rightClickHandler = effectObj.AddComponent<RightClickHandler2>();
            rightClickHandler.effectName = effect.Name;
            rightClickHandler.category = "Favorites";
            rightClickHandler.fromSaint = fromSaint;
        }
    } 
    
public class RightClickHandler2 : MonoBehaviour, IPointerClickHandler
    {
    public string effectName;
    public string category;
    public bool fromSaint = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var favorites = FavoriteEffectManager.LoadFavorites();

            switch (category)
            {
                case "Favorites":  
                    ToggleFavorite(effectName, favorites.Favorites);
                    break;
                // case "FavoriteEnemies":
                //     ToggleFavorite(effectName, favorites.FavoriteEnemies);
                //     break;
                // case "FavoriteTraps":
                //     ToggleFavorite(effectName, favorites.FavoriteTraps);
                //     break;
                // case "FavoriteScraps":
                //     ToggleFavorite(effectName, favorites.FavoriteScraps);
                //     break;
                // case "FavoriteShopItems":
                //     ToggleFavorite(effectName, favorites.FavoriteShopItems);
                //     break;
                // case "FavoriteItems":
                //     ToggleFavorite(effectName, favorites.FavoriteItems);
                //     break;
            }

            FavoriteEffectManager.SaveFavorites(favorites);
            clearMainViewport();

         
            switch (category)
            {
                case "Favorites":
                    ShowSelectMenu(fromSaint);
                    break;
                // case "FavoriteEnemies":
                //     spawnEnemy();
                //     break;
                // case "FavoriteTraps":
                //     spawnTrap();
                //     break;
                // case "FavoriteScraps":
                //     spawnScrap();
                //     break;
                // case "FavoriteShopItems":
                //     spawnShopItems();
                //     break;
                // case "FavoriteItems":
                //     spawnAnyItem();
                //     break;
            }
        }
    }


        private void ToggleFavorite(string name, List<string> favorites)
        {
            if (favorites.Contains(name))
            {
                favorites.Remove(name);
            }
            else
            {
                favorites.Add(name);
            }
        }
    }
}

 public static class FavoriteEffectManager
    {
        private static readonly string directoryPath = Path.Combine(Application.persistentDataPath, "EmergencyDice");

        private static readonly string filePath = Path.Combine(directoryPath, "Favorites.json");

        public static FavoriteData LoadFavorites()
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Favorites file not found at {filePath}. Returning empty data.");
                return new FavoriteData();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<FavoriteData>(json) ?? new FavoriteData();
            }
            catch
            {
                Debug.LogError($"Failed to load favorites from {filePath}. Returning empty data.");
                return new FavoriteData();
            }
        }

        public static void SaveFavorites(FavoriteData favorites)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            try
            {
                string json = JsonConvert.SerializeObject(favorites, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Debug.Log($"Favorites successfully saved to {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save favorites to {filePath}. Error: {ex.Message}");
            }
        }

        public class FavoriteData
        {
            public List<string> Favorites { get; set; } = new List<string>();
            public List<string> FavoriteEnemies { get; set; } = new List<string>();
            public List<string> FavoriteTraps { get; set; } = new List<string>();
            public List<string> FavoriteScraps { get; set; } = new List<string>();
            public List<string> FavoriteShopItems { get; set; } = new List<string>();
            public List<string> FavoriteItems { get; set; } = new List<string>();
        }
    }
public class DebugMenuController : MonoBehaviour
{
    public GameObject EffectMenu;
    private InputAction escAction;

    private void Awake()
    {
        escAction = new InputAction(binding: "<Keyboard>/escape");
        escAction.performed += ctx => CloseMenu();
        escAction.Enable();
    }

    private void OnDestroy()
    {
        escAction.Disable();
    }

    public void CloseMenu()
    {
        if (EffectMenu != null)
        {
            Destroy(EffectMenu);
            EffectMenu = null;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

public class ColorHelper
{
    
    public static string AppendTransparency(string hexColor, int percentage)
    {
        int alpha = (int)Math.Round((percentage / 100.0) * 255);
        string alphaHex = alpha.ToString("X2");
        return hexColor + alphaHex;
    }
    
    public static (Color brighter, Color darker) GenerateColors(Color primary)
    {
        Color.RGBToHSV(primary, out float h, out float s, out float v);

        float brightV = Mathf.Clamp01(v + 0.3f);
        float darkV = Mathf.Clamp01(v * 0.5f);

        Color brighter = Color.HSVToRGB(h, s, brightV);
        Color darker = Color.HSVToRGB(h, s, darkV);

        return (brighter, darker);
    }
    public static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;
        else
            throw new System.Exception("Invalid hex color");
    }
    
    public static string ColorToHex(Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }
    public static Color GetEmphasisTextColor(Color background)
    {
        Color.RGBToHSV(background, out float h, out float s, out float v);

        // Shift hue 180 degrees
        h = (h + 0.5f) % 1.0f;

        // Emphasis should pop, so keep saturation high and control value
        s = Mathf.Clamp01(s + 0.3f);
        v = v < 0.6f ? 0.8f : Mathf.Clamp01(1f - v * 0.5f); // balance against brightness

        return Color.HSVToRGB(h, s, v);
    }
    public static Color GetReadableTextColor(Color background)
    {
        float luminance = GetLuminance(background);
        return luminance > 0.5f ? HexToColor("#1a1100") : Color.white;
    }
    public static float GetLuminance(Color color)
    {
        float R = Linearize(color.r);
        float G = Linearize(color.g);
        float B = Linearize(color.b);
        return 0.2126f * R + 0.7152f * G + 0.0722f * B;
    }

    private static float Linearize(float channel)
    {
        return (channel <= 0.03928f) ? (channel / 12.92f) : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
    }
}