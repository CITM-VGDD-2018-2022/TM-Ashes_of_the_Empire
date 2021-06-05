using System;
using DiamondEngine;

public class HubSkillTreeController : DiamondComponent
{
    public GameObject mando = null;
    public GameObject mando_tree = null;
    public GameObject mando_tree_activation_point = null;
    public GameObject grogu_tree = null;
    public GameObject grogu_tree_activation_point = null;
    public GameObject weapon_tree = null;
    public GameObject weapon_tree_activation_point = null;

    public Skills skill_selected = null;

    public float maximum_distance_to_interact_squared_skill = 0.0f;

    public GameObject mandoText = null;
    public GameObject groguText = null;
    public GameObject weaponText = null;

    public GameObject beskarResourceText = null;
    public GameObject macaronResourceText = null;
    public GameObject scrapResourceText = null;

    private int beskarResource = 0;
    private int macaronResource = 0;
    private int scrapResource = 0;

    private bool firstFrame = true;

    public void Awake()
    {
        beskarResourceText.GetComponent<Text>().text = PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR).ToString();
        macaronResourceText.GetComponent<Text>().text = PlayerResources.GetResourceCount(RewardType.REWARD_MACARON).ToString();
        scrapResourceText.GetComponent<Text>().text = PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP).ToString();
    }

    public void Update()
    {
        if (firstFrame == true && (mando_tree.IsEnabled() || grogu_tree.IsEnabled() || weapon_tree.IsEnabled()))
        {
            Core.instance.BlockInIdle();
            firstFrame = false;
        }

        if (beskarResource!= PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR))
        {
            beskarResource = PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR);
            beskarResourceText.GetComponent<Text>().text = beskarResource.ToString();
        }

        if (macaronResource != PlayerResources.GetResourceCount(RewardType.REWARD_MACARON))
        {
            macaronResource = PlayerResources.GetResourceCount(RewardType.REWARD_MACARON);
            macaronResourceText.GetComponent<Text>().text = macaronResource.ToString();
        }

        if (scrapResource != PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP))
        {
            scrapResource = PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP);
            scrapResourceText.GetComponent<Text>().text = scrapResource.ToString();
        }

        if (Core.instance.gameObject != null)
            mando = Core.instance.gameObject;

        if (mando == null)
            return;

        if (Input.GetGamepadButton(DEControllerButton.A) != KeyState.KEY_DOWN && Input.GetGamepadButton(DEControllerButton.B) != KeyState.KEY_DOWN)
            return;

        bool activate_tree = Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN ? true : false;

        if (activate_tree && ((mando_tree != null && mando_tree.IsEnabled()) || (grogu_tree != null && grogu_tree.IsEnabled()) || (weapon_tree != null && weapon_tree.IsEnabled())))
            return;

        if (!activate_tree)
        {
            Audio.PlayAudio(gameObject, "Play_UI_Skill_Tree_Close");
            Core.instance.LockInputs(false);
            firstFrame = true;

            if (mando_tree != null && mando_tree.IsEnabled())
            {
                mando_tree.EnableNav(false);
                if (mandoText != null)
                    mandoText.Enable(true);
                return;
            }
            if (grogu_tree != null && grogu_tree.IsEnabled())
            {
                grogu_tree.EnableNav(false);
                if (groguText != null)
                    groguText.Enable(true);
                return;
            }
            if (weapon_tree != null && weapon_tree.IsEnabled())
            {
                weapon_tree.EnableNav(false);
                if (weaponText != null)
                    weaponText.Enable(true);
                return;
            }
        }
        if (mando_tree != null && mando_tree_activation_point != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(mando_tree_activation_point.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared_skill)
            {
                mando_tree.EnableNav(true);
                if (mandoText != null)
                    mandoText.Enable(false);
            }
        }
        if (grogu_tree != null && grogu_tree_activation_point != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(grogu_tree_activation_point.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared_skill)
            {
                grogu_tree.EnableNav(true);
                if (groguText != null)
                    groguText.Enable(false);
            }
        }
        if (weapon_tree != null && weapon_tree_activation_point != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(weapon_tree_activation_point.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared_skill)
            {
                weapon_tree.EnableNav(true);
                if (weaponText != null)
                    weaponText.Enable(false);
            }
        }
    }
}