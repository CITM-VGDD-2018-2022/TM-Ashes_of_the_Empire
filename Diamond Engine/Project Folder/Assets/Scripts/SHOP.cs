using System;
using DiamondEngine;
using System.Collections.Generic;

public enum ShopItems
{
    SHOP_ITEM_BOON,
    SHOP_ITEM_HEALTHREPLENISHMENT,
}

public enum ShopPrice
{
    SHOP_FREE = 0,
    SHOP_HEALTH = 75,
    SHOP_CHEAP = 150,
    SHOP_AVERAGE = 230,
    SHOP_EXPENSIVE = 310
}

public class SHOP : DiamondComponent
{
    public GameObject shopUI;
    public GameObject textPopUp;

    //Buttons
    public GameObject item1 = null;
    public GameObject item2 = null;
    public GameObject item3 = null;
    public GameObject item4 = null;
    private GameObject[] items = null;

    private ShopButtons[] shopButtons = null;

    public GameObject currencyObject = null;
    public GameObject currencyObject1 = null;
    private Text currencyText = null;
    private Text currencyText1 = null;

    public float interactionRange = 2.0f;
    public bool autoGenerateItems = true;
    public bool opening;
    public GameObject defaultButton = null;
    private GameObject envObj = null;
    bool shopOpen = false;
    private bool start = true;
    private bool toCheck = true;
    public void Awake()
    {
        DebugOptionsHolder.goToNextLevel = false;
        shopOpen = false;
        opening = false;
        start = true;
        toCheck = true;
        if (currencyObject != null && currencyObject1 != null)
        {
            currencyText = currencyObject.GetComponent<Text>();
            currencyText1 = currencyObject1.GetComponent<Text>();
            UpdateCurrency(PlayerResources.GetRunCoins());
        }

        items = new GameObject[] { item1, item2, item3, item4 };

        shopButtons = new ShopButtons[items.Length];
        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                shopButtons[i] = items[i].GetComponent<ShopButtons>();
        }
        envObj = InternalCalls.FindObjectWithName("Environment");
    }

    private void Start()
    {
        if (autoGenerateItems)
            RandomiseItems();

        start = false;
        if (RoomSwitch.currentroom == RoomSwitch.levelLists[(int)RoomSwitch.currentLevelIndicator].preBossScene)
        {
            toCheck = false;
            Audio.SetState("Game_State", "Run_3");
            Audio.SetState("Player_State", "Alive");
            if (MusicSourceLocate.instance != null)
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
        }
        if (RoomSwitch.currentLevelIndicator-1 == RoomSwitch.LEVELS.ONE && toCheck)
        {
            Audio.SetState("Game_State", "Run");
            Audio.SetState("Player_State", "Alive");
            if (MusicSourceLocate.instance != null)
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            if (envObj !=null)
                Audio.PlayAudio(envObj, "Play_Post_Boss_Room_1_Ambience");
        }
        else if (RoomSwitch.currentLevelIndicator-1 == RoomSwitch.LEVELS.TWO && toCheck)
        {
            Audio.SetState("Game_State", "Run_2");
            Audio.SetState("Player_State", "Alive");
            if (MusicSourceLocate.instance != null)
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            if (envObj != null)
                Audio.PlayAudio(envObj, "Play_Post_Boss_Room_2_Ambience");
        }
    }

    public void Update()
    {
        if (start)
        {
            Start();
            start = false;
        }
        if (shopOpen)
        {
            if (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN)
            {
                CloseShop();
            }

            if (Input.GetKey(DEKeyCode.M) == KeyState.KEY_DOWN)
            {
                PlayerResources.AddRunCoins(100);
                UpdateCurrency(PlayerResources.GetRunCoins());
            }
        }
        else
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, interactionRange))
            {
                if (!textPopUp.IsEnabled()) textPopUp.Enable(true);

                if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN)
                {
                    OpenShop();
                    shopUI.SetParent(shopUI.parent);
                }
            }
            else if (textPopUp.IsEnabled())
            {
                textPopUp.Enable(false);
            }
        }

    }

    public bool InRange(Vector3 point, float givenRange)
    {
        return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
    }

    public bool Buy(ShopButtons item)
    {
        bool ret = false;

        if (shopOpen)
        {
            int currency = PlayerResources.GetRunCoins();
            int discount = 0;

            if (Core.instance.ShopDiscount > 0)
            {
                Core.instance.ShopDiscount--;
                Core.instance.GetStatusData(STATUS_TYPE.GREEF_PAYCHECK).severity--;
                discount = item.discount;

                if (Core.instance.ShopDiscount == 0)
                {
                    Core.instance.RemoveStatus(STATUS_TYPE.GREEF_PAYCHECK);
                    ResetShopPrices();
                }
            }

            if (currency >= (int)item.price - discount)
            {
                if (item.itemType == ShopItems.SHOP_ITEM_BOON)
                {
                    if (item.resource != null)
                    {
                        item.resource.Use();
                    }
                }
                else if (item.itemType == ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT)
                {
                    Core.instance.gameObject.GetComponent<PlayerHealth>().HealPercentMax(0.25f);
                }

                currency -= (int)item.price - (int)discount;
                ret = true;
            }

            UpdateCurrency(currency);
        }
        return ret;
    }

    public void UpdateCurrency(int value)
    {
        PlayerResources.SetRunCoins(value);

        if (currencyText != null)
            currencyText.text = PlayerResources.GetRunCoins().ToString();

        if (currencyText1 != null)
            currencyText1.text = PlayerResources.GetRunCoins().ToString();
    }

    public void RandomiseItems()
    {
        List<BOONS> available = new List<BOONS>();
        for (int i = 0; i < BoonDataHolder.boonType.Length; ++i)  //Number of boons
        {
            if (!PlayerResources.CheckBoon((BOONS)i))
                available.Add((BOONS)i);
        }

        Random rand = new Random();
        int item;

        for(int i = 0; i < shopButtons.Length; i++)
        {
            if(shopButtons[i] != null)
            {
                if (available.Count > 0)
                {
                    item = rand.Next(0, available.Count);
                    SetShopItem(shopButtons[i], ShopItems.SHOP_ITEM_BOON, available[item]);
                    available.RemoveAt(item);
                }
                else
                {
                    SetShopItem(item1.GetComponent<ShopButtons>(), ShopItems.SHOP_ITEM_HEALTHREPLENISHMENT, BOONS.BOON_MAX);
                }
            }
        }
    }

    private void SetShopItem(ShopButtons item, ShopItems type, BOONS boon)
    {
        float discount = 0;
        if (Core.instance.ShopDiscount > 0)
        {
            float price = (float)BoonDataHolder.boonType[(int)boon].price;
            discount = price * 0.75f;
        }
 

        if (type == ShopItems.SHOP_ITEM_BOON)
        {
            if (BoonDataHolder.boonType[(int)boon] != null)
            {
                item.SetItem(type, BoonDataHolder.boonType[(int)boon].price, BoonDataHolder.boonType[(int)boon].name, BoonDataHolder.boonType[(int)boon].rewardDescription, (int)discount);
                item.resource = BoonDataHolder.boonType[(int)boon];
            }
        }
        else
        {
            item.SetItem(type, ShopPrice.SHOP_CHEAP , "Health Replenishment", "Heal for 25% of your max life", (int)discount);
        }
    }

    private void ResetShopPrices()
    {
       for(int i = 0; i < items.Length; i++)
        {
            if(shopButtons[i] != null)
            {
                shopButtons[i].SetDefaultPrice();
            }
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        opening = true;
        shopUI.EnableNav(true);
        textPopUp.Enable(false);
        Core.instance.LockInputs(true);
        Audio.PlayAudio(gameObject, "Play_UI_Shop_Open");
        if (defaultButton != null)
            defaultButton.GetComponent<Navigation>().Select();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.EnableNav(false);
        Core.instance.LockInputs(false); 
        textPopUp.Enable(true);
        UpdateCurrency(PlayerResources.GetRunCoins());
    }
}