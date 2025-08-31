namespace OnlineShop;

class Program
{
    static void Main(string[] args)
    {
        var printer = new ConsolePrinter();
        
        Good iPhone12 = new Good("IPhone 12");
        Good iPhone11 = new Good("IPhone 11");
        
        Warehouse warehouse = new Warehouse();
        
        Shop shop = new Shop(warehouse);
        
        warehouse.Delive(iPhone12, 10);
        warehouse.Delive(iPhone11, 1);
        
        //Вывод всех товаров на складе с их остатком
        warehouse.Print(printer);
        Console.WriteLine();
        
        Cart cart = shop.Cart();
        cart.Add(iPhone12, 4);
        // cart.Add(iPhone11, 3);  //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе
        
        //Вывод всех товаров в корзине
        cart.Print(printer);
        Console.WriteLine();
        
        Console.WriteLine(cart.Order().Paylink);
        
        // cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
    }
}