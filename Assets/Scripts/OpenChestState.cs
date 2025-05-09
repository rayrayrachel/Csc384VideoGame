using UnityEngine;

public class OpenChestState : ChestState
{
    public OpenChestState(ChestTileController controller) : base(controller) { }

    public override void Enter()
    {
        controller.GiveReward();
    }
}
