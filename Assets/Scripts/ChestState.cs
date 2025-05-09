public abstract class ChestState
{
    protected ChestTileController controller;

    public ChestState(ChestTileController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter() { }
    public virtual void HandleInput() { }
    public virtual void Exit() { }
}
