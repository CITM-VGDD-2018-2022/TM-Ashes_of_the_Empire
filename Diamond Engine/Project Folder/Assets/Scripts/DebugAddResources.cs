using System;
using DiamondEngine;

public class DebugAddResources : DiamondComponent
{
    public void OnExecuteButton()
    {
        PlayerResources.AddRunCoins(500);
        PlayerResources.AddResourceByValue(RewardType.REWARD_BESKAR,500);
        PlayerResources.AddResourceByValue(RewardType.REWARD_MACARON, 500);
        PlayerResources.AddResourceByValue(RewardType.REWARD_MILK, 500);
        PlayerResources.AddResourceByValue(RewardType.REWARD_SCRAP, 500);
        if (Core.instance.hud != null)
        {
            Core.instance.hud.GetComponent<HUD>().UpdateCurrency(PlayerResources.GetRunCoins());
        }

    }
}