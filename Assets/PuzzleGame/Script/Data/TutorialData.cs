public class TutorialData : DataBase<TutorialData>
{
    public bool isUnlock_ExtraSlot = false;
    public bool isUnlock_MakeHoleGay = false;
    public bool isUnlock_UFO = false;
    public bool isFirstLevelDone = false;

    protected override SaveKey SaveKey => SaveKey.TutorialData;
}
