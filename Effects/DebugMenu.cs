using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using RepoDice.Dice;
using RepoDice.Effects;
using REPOLib.Extensions;
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
        textElements2.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/KeepOpen/Label").GetComponent<Text>());
        textElements2.Add(RepoDice.DebugMenuPrefab.transform.Find("DebugMenu/Bald/Label").GetComponent<Text>());
        textElements.Add(RepoDice.SelectMenuPrefab.transform.Find("DebugMenu/ColorPlayer/Label").GetComponent<TMP_Text>());
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
    
    public static void showDebugMenu()
    {

        SetupColors();
        if (!ran) setupElements();
        if (ran) setColors();
        if (EffectMenu != null)
        {
            GameObject.Destroy(EffectMenu);
            EffectMenu = null;
        }
        InputAction escAction = new InputAction(binding: "<Keyboard>/escape");
        escAction.performed += ctx => CloseSelectMenu();
        escAction.Enable();
        
        EffectMenu = GameObject.Instantiate(RepoDice.DebugMenuPrefab);
        Canvas canvas = EffectMenu.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 999;
        EffectMenu.transform.localPosition = Vector3.zero;
        EffectMenu.transform.localScale = Vector3.one;
        EffectMenu.transform.localRotation = Quaternion.identity;
        
        if (EffectMenu == null)
        {
            Debug.LogError("EffectMenu is null after instantiation!");
            return;
        } 
        subScrollContent = EffectMenu.transform.Find("DebugMenu/Scroll View/Viewport/Content");
        mainScrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");
        
        TMP_InputField searchInput = EffectMenu.transform.Find("DebugMenu/Background/SearchField").GetComponent<TMP_InputField>();
        searchInput.onValueChanged.AddListener(FilterItems);
        
        Button ClearButton = EffectMenu.transform.Find("DebugMenu/Background/ClearButton").GetComponent<Button>();
        ClearButton.onClick.AddListener(() =>
        {
            searchInput.text = "";
        });

        DebugMenuController controller = EffectMenu.AddComponent<DebugMenuController>();
        controller.EffectMenu = EffectMenu;
        setupButtons();
        
        Button selectEffectButton = EffectMenu.transform.Find("DebugMenu/Select Effect").GetComponent<Button>();
        TMP_Text selectEffectText =
            EffectMenu.transform.Find("DebugMenu/Select Effect/Text (TMP)").GetComponent<TMP_Text>();
        selectEffectButton.onClick.AddListener(() =>
        {
            clearMainViewport();
            ShowSelectMenu();
        });
        
        Button spawnMenuButton = EffectMenu.transform.Find("DebugMenu/Spawn Menu").GetComponent<Button>();
        TMP_Text spawnMenuText = EffectMenu.transform.Find("DebugMenu/Spawn Menu/Text (TMP)").GetComponent<TMP_Text>();
        spawnMenuButton.onClick.AddListener(() =>
        {
            clearSubViewport();
            spawnFunctions();
        });
        
        Button PlayerButton = EffectMenu.transform.Find("DebugMenu/Player Functions").GetComponent<Button>();
        TMP_Text PlayerText = EffectMenu.transform.Find("DebugMenu/Player Functions/Text (TMP)").GetComponent<TMP_Text>();
        PlayerButton.onClick.AddListener(() =>
        {
            clearSubViewport();
            playerFunctions();
        });
        
        Button SpecialButton = EffectMenu.transform.Find("DebugMenu/Special Functions").GetComponent<Button>();
        SpecialButton.onClick.AddListener(() =>
        {
            clearSubViewport();
            specialFunctions(DebugMenu.isSpecial());
        });
      
        
        
        Button adminButton = EffectMenu.transform.Find("DebugMenu/Grant Admin").GetComponent<Button>();
        // if (su || StartOfRound.Instance.localPlayerController.IsHost)
        // {
        //     TMP_Text adminText = EffectMenu.transform.Find("DebugMenu/Grant Admin/Text (TMP)").GetComponent<TMP_Text>();
        //     adminButton.onClick.AddListener(() =>
        //     {
        //         clearSubViewport();
        //         AdminFunctions();
        //     });
        // }
        // else
        // {
        Destroy(adminButton.gameObject); // Only temporary until I add Admin Actions
        // }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    
    }
    public static void playerFunctions()
    {
        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Scroll View/Viewport/Content");

        GameObject effectObj = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, scrollContent);
        TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText.text = $"Revive Player";

        Button button = effectObj.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            clearMainViewport();
            RevivePlayer();
        });
        GameObject effectObj2 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, scrollContent);
        TMP_Text buttonText2 = effectObj2.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText2.text = $"Teleport to Player";

        Button button2 = effectObj2.GetComponent<Button>();
        button2.onClick.AddListener(() =>
        {
            clearMainViewport();
            TeleportPlayer();
        });

        GameObject effectObj3 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, scrollContent);
        TMP_Text buttonText3 = effectObj3.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText3.text = $"Bring Player";

        Button button3 = effectObj3.GetComponent<Button>();
        button3.onClick.AddListener(() =>
        {
            clearMainViewport();
            TeleportPlayer(bring: true);
        });
    }
    public static void TeleportPlayer(bool bring = false)
    {

        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");

        List<PlayerAvatar> allPlayers = SemiFunc.PlayerGetAll();

        allPlayers = allPlayers
            .GroupBy(x => x.playerName)
            .Select(g => g.First())
            .OrderBy(x => x.playerName)
            .ToList();


        foreach (var player in allPlayers)
        {
            if (!(player.playerHealth.health>=1)) continue;
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, scrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText.text = $"{player.playerName}";

            Button button = effectObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                CloseSelectMenu();
                var player2 = PlayerAvatar.instance;
                if (player2.playerHealth.health<=0 && bring)
                {
                    Networker.Instance.ForceTeleportRPC(player.photonView.ViewID, player2.transform.position + new Vector3(0, 1.0f, 0), player2.transform.rotation);
                }
                else
                {
                    Networker.Instance.ForceTeleportRPC(player2.photonView.ViewID, player.transform.position + new Vector3(0, 1.0f, 0), player.transform.rotation);
                }
            });

        }
    }

    public static bool isSpecial()
    {
        if(PlayerAvatar.instance.steamID == RepoDice.slayerSteamID) return true;
        if(PlayerAvatar.instance.steamID == RepoDice.glitchSteamID) return true;
        if(PlayerAvatar.instance.steamID == RepoDice.lizzieSteamID) return true;
        return false;
    }
    
    public static void setupButtons(bool fromSaint = false)
    {
                
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

        setupButtons(fromSaint);
        
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

        if(Misc.GetLocalPlayer().steamID == RepoDice.slayerSteamID) effects = new List<IEffect>(DieBehaviour.AllEffects);
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
    
    public static void spawnFunctions()
    {
        GameObject effectObj2 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        TMP_Text buttonText2 = effectObj2.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText2.text = $"Spawn Enemy";

        Button button2 = effectObj2.GetComponent<Button>();
        button2.onClick.AddListener(() =>
        {
            clearMainViewport();
            spawnEnemy(Vector3.one);
        });
        GameObject effectObj6 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        TMP_Text buttonText6 = effectObj6.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText6.text = $"Spawn BIG Enemy";

        Button button6 = effectObj6.GetComponent<Button>();
        button6.onClick.AddListener(() =>
        {
            clearMainViewport();
            spawnEnemy(new Vector3(2.5f, 2.5f, 2.5f));
        });
        GameObject effectObj7 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        TMP_Text buttonText7 = effectObj7.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText7.text = $"Spawn SMOL Enemy";

        Button button7 = effectObj7.GetComponent<Button>();
        button7.onClick.AddListener(() =>
        {
            clearMainViewport();
            spawnEnemy(new Vector3(0.5f, 0.5f, 0.5f));
        });
        
        GameObject effectObj3 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        TMP_Text buttonText3 = effectObj3.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText3.text = $"Spawn Valuable";

        Button button3 = effectObj3.GetComponent<Button>();
        button3.onClick.AddListener(() =>
        {
            clearMainViewport();
            spawnScrap();
        });

        GameObject effectObj5 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        TMP_Text buttonText5 = effectObj5.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText5.text = $"Spawn Shop Items";

        Button button5 = effectObj5.GetComponent<Button>();
        button5.onClick.AddListener(() =>
        {
            clearMainViewport();
            spawnShopItems();
        });

        // if (DebugMenu.isSpecial() || StartOfRound.Instance.localPlayerController.IsHost)
        // {
        //     GameObject effectObj9 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        //     TMP_Text buttonText9 = effectObj9.transform.GetChild(0).GetComponent<TMP_Text>();
        //     buttonText9.text = $"Spawn miniture Enemy";
        //
        //     Button button9 = effectObj9.GetComponent<Button>();
        //     button9.onClick.AddListener(() =>
        //     {
        //         clearMainViewport();
        //         spawnMiniEnemy();
        //     });
        //     
        //     // if (StartOfRound.Instance.localPlayerController.playerSteamId == RepoDice.slayerSteamID || StartOfRound.Instance.localPlayerController.IsHost)
        //     // {
        //     //     GameObject effectObj8 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        //     //     TMP_Text buttonText8 = effectObj8.transform.GetChild(0).GetComponent<TMP_Text>();
        //     //     buttonText8.text = $"Spawn Freebird Enemy";
        //     //
        //     //     Button button8 = effectObj8.GetComponent<Button>();
        //     //     button8.onClick.AddListener(() =>
        //     //     {
        //     //         clearMainViewport();
        //     //         spawnFreebirdEnemy();
        //     //     });
        //     // }
        // }
    }

    public static void spawnEnemy(Vector3 size)
    {
        FavoriteEffectManager.FavoriteData favoritesData = FavoriteEffectManager.LoadFavorites();
        List<string> favoriteEnemyNames = favoritesData.FavoriteEnemies;

        var allEnemies = Misc.getEnemies().GroupBy(x=>x.name).Select(g=>g.First()).OrderBy(e => favoriteEnemyNames.Contains(e.name) ? 0 : 1)
            .ThenBy(e => e.name)
            .ToList();
        
        foreach (var enemy in allEnemies)
        {
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();

            bool isFavorite = favoriteEnemyNames.Contains(enemy.name);
            string favoriteMarker = isFavorite ? "*" : "";
            buttonText.text = $"{favoriteMarker} {enemy.name}";
            buttonText.color = isFavorite ? FavoriteTextColor : TextColor;

            Button button = effectObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                CloseSelectMenu();

                Vector3 spawnPosition = PlayerAvatar.instance.transform.position + PlayerAvatar.instance.transform.forward;

                Networker.Instance.photonView.RPC("SpawnAndScaleEnemy",RpcTarget.MasterClient, enemy.name, 1, spawnPosition, size, false);
            });

            RightClickHandler2 rightClickHandler = effectObj.AddComponent<RightClickHandler2>();
            rightClickHandler.effectName = enemy.name;
            rightClickHandler.category = "FavoriteEnemies";
        }
    } //

    public static void spawnShopItems()
    {
        
        FavoriteEffectManager.FavoriteData favoritesData = FavoriteEffectManager.LoadFavorites();
        List<string> favoriteShopItemNames = favoritesData.FavoriteShopItems;
        
        List<Item> allShopItems = StatsManager.instance.itemDictionary.Values.ToList()
            .GroupBy(x => x.itemName)
            .Select(g => g.First())
            .ToList();

        allShopItems = allShopItems.OrderBy(s => favoriteShopItemNames.Contains(s.itemName) ? 0 : 1)
            .ThenBy(s => s.itemName)
            .ToList();

        foreach (var item in allShopItems)
        {
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();

            bool isFavorite = favoriteShopItemNames.Contains(item.itemName);
            string favoriteMarker = isFavorite ? "*" : "";
            buttonText.text = $"{favoriteMarker} {item.itemName}";
            buttonText.color = isFavorite ? FavoriteTextColor : TextColor;

            Button button = effectObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                CloseSelectMenu();
                
                Vector3 spawnPosition = PlayerAvatar.instance.transform.position + PlayerAvatar.instance.transform.forward;

                Networker.Instance.photonView.RPC("spawnItemRPC", RpcTarget.MasterClient, item.itemName, spawnPosition, RepoDice.keepItems.Value);
            });

            RightClickHandler2 rightClickHandler = effectObj.AddComponent<RightClickHandler2>();
            rightClickHandler.effectName = item.itemName;
            rightClickHandler.category = "FavoriteShopItems";
        }
    }
    
    public static void specialFunctions(bool special)
    {
        if (special)
        {
            GameObject effectObj8 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
            TMP_Text buttonText8 = effectObj8.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText8.text = $"Play Sound";

            Button button8 = effectObj8.GetComponent<Button>();
            button8.onClick.AddListener(() =>
            {
                clearMainViewport();
                ShowSoundMenu();
            });
        }

        GameObject effectObj2 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
        TMP_Text buttonText2 = effectObj2.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText2.text = $"POI Teleports";

        Button button2 = effectObj2.GetComponent<Button>();
        button2.onClick.AddListener(() =>
        {
            clearMainViewport();
            poiTeleports();
        });
        
        if (special)
        {
            GameObject effectObj3 = GameObject.Instantiate(RepoDice.DebugSubButtonPrefab, subScrollContent);
            TMP_Text buttonText3 = effectObj3.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText3.text = $"Jail";
        
            Button button3 = effectObj3.GetComponent<Button>();
            button3.onClick.AddListener(() =>
            {
                clearMainViewport();
                jail();
            });
        }
    }
    public static void ShowSoundMenu()
    {

        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");

        foreach (var entry in RepoDice.sounds.OrderBy(x => x.Key))
        {
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, scrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();

            buttonText.text = entry.Key;
            //buttonText.outlineColor = Color.black;
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
                CloseSelectMenu();
                Networker.Instance.photonView.RPC("playSoundForEveryone", RpcTarget.All, entry.Key);
            });
        }
    } 

    public static void RevivePlayer()
    {
        Transform scrollContent = EffectMenu.transform.Find("DebugMenu/Background/Scroll View/Viewport/Content");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        List<PlayerAvatar> allPlayers = SemiFunc.PlayerGetAll();

        int count = 0;

        foreach (var player in allPlayers)
        {
            if (!(player.playerHealth.health <= 0)) continue;
            count++;
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, scrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText.text = $"{player.playerName}";

            Button button = effectObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                clearMainViewport();
                CloseSelectMenu();
                RevivePlayer();
                Networker.Instance.photonView.RPC(nameof(Networker.Instance.reviveRPC),RpcTarget.All,player.photonView.ViewID);
            });

        }

        if (count == 0)
        {
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, scrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText.text = $"No Dead Players";
        }
    }
    
    public static void poiTeleports()
    {
        GameObject effectObj1 = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
        TMP_Text buttonText1 = effectObj1.transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText1.text = "Ship";

        Button button1 = effectObj1.GetComponent<Button>();
        button1.onClick.AddListener(() =>
        {
            Networker.Instance.photonView.RPC("ForceTeleportRPC", RpcTarget.All, PlayerAvatar.instance.photonView.ViewID, LevelGenerator.Instance.LevelPathTruck.transform.position,LevelGenerator.Instance.LevelPathTruck.transform.rotation);
            CloseSelectMenu();
        });
        var entrances = GameObject.FindObjectsByType<ExtractionPoint>(FindObjectsSortMode.None);
        foreach (var entrance in entrances)
        {
            GameObject effectObj2 = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
            TMP_Text buttonText2 = effectObj2.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText2.text = entrance.name;

            Button button2 = effectObj2.GetComponent<Button>();
            button2.onClick.AddListener(() =>
            {
                Networker.Instance.photonView.RPC("ForceTeleportRPC", RpcTarget.All, PlayerAvatar.instance.photonView.ViewID, entrance.transform.position,entrance.transform.rotation);
                CloseSelectMenu();
            });
        }
        var levelPoints = GameObject.FindObjectsByType<LevelPoint>(FindObjectsSortMode.None);
        foreach (var levelPoint in levelPoints)
        {
            GameObject effectObj3 = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
            TMP_Text buttonText3 = effectObj3.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText3.text = levelPoint.name;

            Button button3 = effectObj3.GetComponent<Button>();
            button3.onClick.AddListener(() =>
            {
                Networker.Instance.photonView.RPC(nameof(Networker.Instance.ForceTeleportRPC), RpcTarget.All, PlayerAvatar.instance.photonView.ViewID, levelPoint.transform.position,levelPoint.transform.rotation);
                CloseSelectMenu();
            });
        }
    } 
    public static void jail()
    {
        var players = SemiFunc.PlayerGetAll();
        var shopDoor = GameObject.Find("Shop Door");
        if(shopDoor == null) return;
        var lp = shopDoor.transform.parent.GetComponentInChildren<LevelPoint>();
        foreach (var player in players)
        {
            GameObject effectObj2 = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
            TMP_Text buttonText2 = effectObj2.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText2.text = player.playerName;
    
            Button button2 = effectObj2.GetComponent<Button>();
            button2.onClick.AddListener(() =>
            {
                Networker.Instance.photonView.RPC(nameof(Networker.Instance.ForceTeleportRPC), RpcTarget.All, player.photonView.ViewID, lp.transform.position,lp.transform.rotation);
                CloseSelectMenu();
            });
        }
    }
    public static void spawnScrap()
    {
        FavoriteEffectManager.FavoriteData favoritesData = FavoriteEffectManager.LoadFavorites();
        List<string> favoriteScrapNames = favoritesData.FavoriteScraps;

        if(Misc.valuablePrefabsByName.Count==0)Misc.CacheValuables();
        var allScraps = Misc.valuablePrefabsByName.Keys.ToList();
        RepoDice.SuperLog(allScraps.Count.ToString());
        
        allScraps = allScraps.OrderBy(s => favoriteScrapNames.Contains(s) ? 0 : 1)
            .ThenBy(s => s)
            .ToList();

        foreach (var scrap in allScraps)
        {
            GameObject effectObj = GameObject.Instantiate(RepoDice.DebugMenuButtonPrefab, mainScrollContent);
            TMP_Text buttonText = effectObj.transform.GetChild(0).GetComponent<TMP_Text>();

            bool isFavorite = favoriteScrapNames.Contains(scrap);
            string favoriteMarker = isFavorite ? "*" : "";
            buttonText.text = $"{favoriteMarker} {scrap}";
            buttonText.color = isFavorite ? FavoriteTextColor : TextColor;

            Button button = effectObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                CloseSelectMenu();

                Vector3 spawnPosition = PlayerAvatar.instance.transform.position + PlayerAvatar.instance.transform.forward;
                
                Networker.Instance.photonView.RPC(nameof(Networker.Instance.spawnValuable), RpcTarget.MasterClient, scrap, spawnPosition);
            });

            // ✅ Right-click handler for favoriting
            RightClickHandler2 rightClickHandler = effectObj.AddComponent<RightClickHandler2>();
            rightClickHandler.effectName = scrap;
            rightClickHandler.category = "FavoriteScraps";
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
                case "FavoriteScraps":
                    ToggleFavorite(effectName, favorites.FavoriteScraps);
                    break;
                case "FavoriteShopItems":
                    ToggleFavorite(effectName, favorites.FavoriteShopItems);
                    break;
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
                case "FavoriteScraps":
                    spawnScrap();
                    break;
                case "FavoriteShopItems":
                    spawnShopItems();
                    break;
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