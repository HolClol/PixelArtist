public class SettingData : DataBase<SettingData>
{

    public bool music = true;
    public bool sound = true;
    public bool vibration = true;

    protected override SaveKey SaveKey => SaveKey.SettingData;
}
