<Query Kind="Program" />

class Item 
{
	public int Id { get; set;}

	public string Name { get; set;}
}

void Main()
{
	var item = FillItem(i => { i.Name = "Test"; });
	item.Dump();
}

Item FillItem(Action<Item> itemFactory)
{
	var item = new Item();
	itemFactory?.Invoke(item);
	
	return item;
}


// Define other methods and classes here
