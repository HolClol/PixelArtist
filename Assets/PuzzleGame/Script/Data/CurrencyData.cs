public class CurrencyData : DataBase<CurrencyData>
{
    public int Coins = 0;
    public int Lives = 5;
    protected override SaveKey SaveKey => SaveKey.CurrencyData;
}
