namespace OnlineShop;

public interface IGoodsProvider
{
    public int GetGoodAmount(Good good);
    public void Take(IReadOnlyList<IReadOnlyGoodContainer> goods);
}