using System;
using DiamondEngine;
public enum Interaction
{
    NONE,
    BO_KATAN,
    GREEF,
    ASHOKA,
    CARA_DUNE,
    GROGU
}

public class HubTextController : DiamondComponent
{

    public GameObject textController = null;
    public GameObject dialog = null;
    public GameObject mando = null;
    public GameObject bo_katan = null;
    public GameObject greef = null;
    public GameObject ashoka = null;
    public GameObject cara_dune = null;
    public GameObject grogu = null;

    public GameObject boKatanNotification = null;
    public GameObject greefNotification = null;
    public GameObject ashokaNotification = null;
    public GameObject caraDuneNotification = null;
    public GameObject groguNotification = null;

    /*public int bo_katan_portrait_uid = 0;
    public int greef_portrait_uid = 0;
    public int ashoka_portrait_uid = 0;
    public int grogu_portrait_uid = 0;

    public float bo_katan_portrait_pos_x = 0;
    public float bo_katan_portrait_pos_y = 0;
    public float bo_katan_portrait_size_x = 0;
    public float bo_katan_portrait_size_y = 0;

    public float greef_portrait_pos_x = 0;
    public float greef_portrait_pos_y = 0;
    public float greef_portrait_size_x = 0;
    public float greef_portrait_size_y = 0;

    public float ashoka_portrait_pos_x = 0;
    public float ashoka_portrait_pos_y = 0;
    public float ashoka_portrait_size_x = 0;
    public float ashoka_portrait_size_y = 0;

    public float grogu_portrait_pos_x = 0;
    public float grogu_portrait_pos_y = 0;
    public float grogu_portrait_size_x = 0;
    public float grogu_portrait_size_y = 0;*/


    public int total_interactions = 0;
    public int total_stages = 0;

    public float maximum_distance_to_interact_squared = 0.0f;

    public bool insideColliderTextActive = false;

    private static int boKatanStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("boKatanStage") : 1;
    private static int greefStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("greefStage") : 1;
    private static int ashokaStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("ashokaStage") : 1;
    private static int caraStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("caraStage") : 1;
    private static int groguStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("groguStage") : 1;

    private static int boKatanInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("boKatanInteractionNum") : 1;
    private static int greefInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("greefInteractionNum") : 1;
    private static int ashokaInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("ashokaInteractionNum") : 1;
    private static int caraInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("caraInteractionNum") : 1;
    private static int groguInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("groguInteractionNum") : 1;

    private static bool boKatanHasInteracted = true;
    private static bool greefHasInteracted = true;
    private static bool ashokaHasInteracted = true;
    private static bool caraHasInteracted = true;
    private static bool groguHasInteracted = true;

    private int total_interactions_and_stages = 0;
    private bool dialog_finished = false;


    Interaction interaction = Interaction.NONE;
    NPCInteraction npcInteraction = null;
    public void Awake()
    {
        CheckInteractionBools();
        total_interactions_and_stages = total_stages * total_interactions;
        if (DiamondPrefs.ReadBool("reset"))
            return;
        WriteDataToJSon();
    }
    public void Update()
    {
        if (mando == null || Input.GetGamepadButton(DEControllerButton.A) != KeyState.KEY_DOWN || textController == null || dialog == null || textController.IsEnabled() == false || insideColliderTextActive)
        {
            return;
        }

        if (dialog_finished)
        {
            dialog_finished = false;
            return;
        }
        interaction = Interaction.NONE;

        if (bo_katan != null && !boKatanHasInteracted)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(bo_katan.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.BO_KATAN;
                npcInteraction = bo_katan.GetComponent<NPCInteraction>();
                //if (npcInteraction.canUpgrade)
                //{
                //    PlayerResources.SubstractResource(RewardType.REWARD_MILK, 1);
                //    IncreaseStage(Interaction.BO_KATAN);
                //}
            }
        }

        if (interaction == Interaction.NONE && greef != null && !greefHasInteracted)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(greef.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.GREEF;
                npcInteraction = greef.GetComponent<NPCInteraction>();
            }
        }

        if (interaction == Interaction.NONE && ashoka != null && !ashokaHasInteracted)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(ashoka.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.ASHOKA;
                npcInteraction = ashoka.GetComponent<NPCInteraction>();
                //if (npcInteraction.canUpgrade)
                //{
                //    PlayerResources.SubstractResource(RewardType.REWARD_MILK, 1);
                //    IncreaseStage(Interaction.ASHOKA);
                //}
            }
        }

        if (interaction == Interaction.NONE && cara_dune != null && !caraHasInteracted)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(cara_dune.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.CARA_DUNE;
                npcInteraction = cara_dune.GetComponent<NPCInteraction>();
                //if (npcInteraction.canUpgrade)
                //{
                //    PlayerResources.SubstractResource(RewardType.REWARD_MILK, 1);
                //    IncreaseStage(Interaction.CARA_DUNE);
                //}
            }
        }

        if (interaction == Interaction.NONE && grogu != null && !groguHasInteracted)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(grogu.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.GROGU;
                npcInteraction = grogu.GetComponent<NPCInteraction>();
                //if (npcInteraction.canUpgrade)
                //{
                //    PlayerResources.SubstractResource(RewardType.REWARD_MILK, 1);
                //    IncreaseStage(Interaction.GROGU);
                //}
            }
        }

        if (interaction == Interaction.NONE)
        {
            return;
        }


        switch (interaction)
        {
            case Interaction.BO_KATAN:
                /*if (bo_katan_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(bo_katan_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(bo_katan_portrait_pos_x, bo_katan_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(bo_katan_portrait_size_x, bo_katan_portrait_size_y, 0);
                }*/
                textController.GetComponent<TextController>().dialog_index = boKatanInteractionNum;
                if (boKatanInteractionNum % 4 != 0)
                {
                    boKatanInteractionNum++;
                    DiamondPrefs.Write("boKatanInteractionNum", boKatanInteractionNum);
                }
                else if (BoKatanCanUpgrade())
                {
                    IncreaseStage(Interaction.BO_KATAN);
                }
                else
                    return;

                boKatanHasInteracted = true;


                if (npcInteraction != null)
                {
                    npcInteraction.canInteract = BoKatanHasInteractions();
                }

                break;
            case Interaction.GREEF:
                /*if (greef_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(greef_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(greef_portrait_pos_x, greef_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(greef_portrait_size_x, greef_portrait_size_y, 0);
                }*/
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages) + greefInteractionNum;
                if (greefInteractionNum % 4 != 0)
                {
                    greefInteractionNum++;
                    DiamondPrefs.Write("greefInteractionNum", greefInteractionNum);
                }
                else if (GreefCanUpgrade())
                {
                    IncreaseStage(Interaction.GREEF);
                }
                else
                    return;

                greefHasInteracted = true;


                if (npcInteraction != null)
                {
                    npcInteraction.canInteract = GreefHasInteractions();
                }

                break;
            case Interaction.ASHOKA:
                /*if (ashoka_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(ashoka_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(ashoka_portrait_pos_x, ashoka_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(ashoka_portrait_size_x, ashoka_portrait_size_y, 0);
                }*/
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 2) + ashokaInteractionNum;
                if (ashokaInteractionNum % 4 != 0)
                {
                    ashokaInteractionNum++;
                    DiamondPrefs.Write("ashokaInteractionNum", ashokaInteractionNum);
                }
                else if (AshokaCanUpgrade())
                {
                    IncreaseStage(Interaction.ASHOKA);
                }
                else
                    return;

                ashokaHasInteracted = true;


                if (npcInteraction != null)
                {
                    npcInteraction.canInteract = AshokaHasInteractions();
                }

                break;
            case Interaction.CARA_DUNE:
                /*if (ashoka_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(ashoka_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(ashoka_portrait_pos_x, ashoka_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(ashoka_portrait_size_x, ashoka_portrait_size_y, 0);
                }*/
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 3) + caraInteractionNum;

                if (caraInteractionNum % 4 != 0)
                {
                    caraInteractionNum++;
                    DiamondPrefs.Write("caraInteractionNum", caraInteractionNum);
                }
                else if (CaraDuneCanUpgrade())
                {
                    IncreaseStage(Interaction.CARA_DUNE);
                }
                else
                    return;

                caraHasInteracted = true;

                if (npcInteraction != null)
                {
                    npcInteraction.canInteract = CaraDuneHasInteractions();
                }
                break;
            case Interaction.GROGU:
                /*if (grogu_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(grogu_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(grogu_portrait_pos_x, grogu_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(grogu_portrait_size_x, grogu_portrait_size_y, 0);
                }*/
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 4) + groguInteractionNum;
                if (groguInteractionNum % 3 != 0)
                {
                    groguInteractionNum++;
                    DiamondPrefs.Write("groguInteractionNum", groguInteractionNum);
                }
                else if (GroguCanUpgrade())
                {
                    IncreaseStage(Interaction.GROGU);
                }
                else
                    return;

                groguHasInteracted = true;


                if (npcInteraction != null)
                {
                    npcInteraction.canInteract = GroguHasInteractions();
                }
                break;
        }
        dialog_finished = true;
        textController.GetComponent<Navigation>().Select();
        dialog.Enable(true);
    }

    public void IncreaseStage(Interaction interaction_to_increase_stage)
    {
        PlayerResources.SubstractResource(RewardType.REWARD_MILK, 1);
        switch (interaction_to_increase_stage)
        {
            case Interaction.BO_KATAN:
                if (boKatanStage <= total_stages)
                {
                    boKatanInteractionNum++;
                    DiamondPrefs.Write("boKatanInteractionNum", boKatanInteractionNum);
                    ++boKatanStage;
                    DiamondPrefs.Write("boKatanStage", boKatanStage);
                }
                break;
            case Interaction.GREEF:
                if (greefStage <= total_stages)
                {
                    //greefInteractionNum = (greefStage * total_interactions) + 1;
                    greefInteractionNum++;
                    DiamondPrefs.Write("greefInteractionNum", greefInteractionNum);
                    ++greefStage;
                    DiamondPrefs.Write("greefStage", greefStage);
                }
                break;
            case Interaction.ASHOKA:
                if (ashokaStage <= total_stages)
                {
                    ashokaInteractionNum++;
                    DiamondPrefs.Write("ashokaInteractionNum", ashokaInteractionNum);
                    ++ashokaStage;
                    DiamondPrefs.Write("ashokaStage", ashokaStage);
                }
                break;
            case Interaction.CARA_DUNE:
                if (caraStage <= total_stages)
                {
                    caraInteractionNum++;
                    DiamondPrefs.Write("caraInteractionNum", caraInteractionNum);
                    ++caraStage;
                    DiamondPrefs.Write("caraStage", caraStage);
                }
                break;
            case Interaction.GROGU:
                if (groguStage <= total_stages)
                {
                    groguInteractionNum++;
                    DiamondPrefs.Write("groguInteractionNum", groguInteractionNum);
                    ++groguStage;
                    DiamondPrefs.Write("groguStage", groguStage);
                }
                break;
        }
        NPCInteraction interactionOriginal = npcInteraction;
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    npcInteraction = grogu.GetComponent<NPCInteraction>();
                    break;
                case 1:
                    npcInteraction = bo_katan.GetComponent<NPCInteraction>();
                    break;
                case 2:
                    npcInteraction = greef.GetComponent<NPCInteraction>();
                    break;
                case 3:
                    npcInteraction = cara_dune.GetComponent<NPCInteraction>();
                    break;
                case 4:
                    npcInteraction = ashoka.GetComponent<NPCInteraction>();
                    break;
            }
            npcInteraction.UpdateUpgrade();
        }
        npcInteraction = interactionOriginal;

    }

    public void Reset()
    {

        boKatanStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("boKatanStage") : 1;
        greefStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("greefStage") : 1;
        ashokaStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("ashokaStage") : 1;
        caraStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("caraStage") : 1;
        groguStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("groguStage") : 1;

        boKatanInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("boKatanInteractionNum") : 1;
        greefInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("greefInteractionNum") : 1;
        ashokaInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("ashokaInteractionNum") : 1;
        caraInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("caraInteractionNum") : 1;
        groguInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("groguInteractionNum") : 1;

        ResetInteractionBools();

        if (DiamondPrefs.ReadBool("loadData"))
            return;
        WriteDataToJSon();
    }
    private void WriteDataToJSon()
    {
        DiamondPrefs.Write("boKatanStage", boKatanStage);
        DiamondPrefs.Write("greefStage", greefStage);
        DiamondPrefs.Write("ashokaStage", ashokaStage);
        DiamondPrefs.Write("caraStage", caraStage);
        DiamondPrefs.Write("groguStage", groguStage);
        DiamondPrefs.Write("boKatanInteractionNum", boKatanInteractionNum);
        DiamondPrefs.Write("greefInteractionNum", greefInteractionNum);
        DiamondPrefs.Write("ashokaInteractionNum", ashokaInteractionNum);
        DiamondPrefs.Write("caraInteractionNum", caraInteractionNum);
        DiamondPrefs.Write("groguInteractionNum", groguInteractionNum);
    }

    private void ResetInteractionBools()
    {
        boKatanHasInteracted = true;
        greefHasInteracted = true;
        ashokaHasInteracted = true;
        caraHasInteracted = true;
        groguHasInteracted = true;

        CheckInteractionBools();
    }

    private void CheckInteractionBools()
    {
        if (greefInteractionNum % 4 != 0 && greefInteractionNum < 19)
            greefHasInteracted = false;
        if (ashokaInteractionNum % 4 != 0 && ashokaInteractionNum < 19)
            ashokaHasInteracted = false;
        if (groguInteractionNum % 3 != 0 && groguInteractionNum < 13)
            groguHasInteracted = false;
        if (boKatanInteractionNum % 4 != 0 && boKatanInteractionNum < 19)
            boKatanHasInteracted = false;
        if (caraInteractionNum % 4 != 0 && caraInteractionNum < 19)
            caraHasInteracted = false;
    }

    public bool GreefHasInteractions()
    {
        return greefInteractionNum % 4 != 0 && !greefHasInteracted && greefInteractionNum < 19;
    }
    public bool AshokaHasInteractions()
    {
        return ashokaInteractionNum % 4 != 0 && !ashokaHasInteracted && ashokaInteractionNum < 19;
    }
    public bool GroguHasInteractions()
    {
        return groguInteractionNum % 3 != 0 && !groguHasInteracted && groguInteractionNum < 13;
    }
    public bool BoKatanHasInteractions()
    {
        return boKatanInteractionNum % 4 != 0 && !boKatanHasInteracted && boKatanInteractionNum < 19;
    }
    public bool CaraDuneHasInteractions()
    {
        return caraInteractionNum % 4 != 0 && !caraHasInteracted && caraInteractionNum < 19;
    }

    public bool GreefCanUpgrade()
    {
        if (greefInteractionNum % 4 == 0 && PlayerResources.GetResourceCount(RewardType.REWARD_MILK) > 0 && greefStage < 5)
        {
            greefHasInteracted = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool AshokaCanUpgrade()
    {
        if (ashokaInteractionNum % 4 == 0 && PlayerResources.GetResourceCount(RewardType.REWARD_MILK) > 0 && ashokaStage < 5)
        {
            ashokaHasInteracted = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GroguCanUpgrade()
    {
        if (groguInteractionNum % 3 == 0 && PlayerResources.GetResourceCount(RewardType.REWARD_MILK) > 0 && groguStage < 5)
        {
            groguHasInteracted = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool BoKatanCanUpgrade()
    {
        if (boKatanInteractionNum % 4 == 0 && PlayerResources.GetResourceCount(RewardType.REWARD_MILK) > 0 && boKatanStage < 5)
        {
            boKatanHasInteracted = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CaraDuneCanUpgrade()
    {
        if (caraInteractionNum % 4 == 0 && PlayerResources.GetResourceCount(RewardType.REWARD_MILK) > 0 && caraStage < 5)
        {
            caraHasInteracted = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}