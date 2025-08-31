namespace OnlineShop;

public interface IReadOnlyGoodContainer
{
    public Good Good { get; }
    public int Amount { get; }
}