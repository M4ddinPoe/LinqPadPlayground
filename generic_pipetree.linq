<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

interface IPipeTreeContext: ICloneable
{
	void Update(object context)
}

interface IFilter<TContext>
	where TContext : class, IPipeTreeContext
{
	Task ExecuteAsync(TContext context);
}


class GenericPipeTreeNode<TContext>
  where TContext : class, IPipeTreeContext
{
	private IFilter<TContext> filter;
	
	public GenericPipeTreeNode(IFilter<TContext> filter)
	{
		this.filter = filter;
	}
	
	public List<GenericPipeTreeNode<TContext>> Children { get; private set; }
	
	public GenericPipeTreeNode<TContext> Add(GenericPipeTreeNode<TContext> node)
	{
		Children.Add(node);
		return this;
	}

	public async Task ExecuteAsync(TContext context)
	{
		await filter.ExecuteAsync(context);
		
		var contextClones = new List<TContext>();
		var tasks = new List<Task>();
		
		// Execute child nodes
		foreach (var child in this.Children)
		{
			TContext contextClone = (TContext)context.Clone();
			contextClones.Add(contextClone);
			
			var task = child.ExecuteAsync(contextClone);
			tasks.Add(task);
		}
		
		await Task.WhenAll(tasks);
		
		// Merge results
		foreach (TContext contextClone in contextClones)
		{
			context.Update(contextClone);
		}
	}
}

class GenericPipeTree<TContext>
	where TContext : class, IPipeTreeContext
{
	public List<GenericPipeTreeNode<TContext>> Children { get; private set; }

	public GenericPipeTree<TContext> Register<TFilter>()
		where TFilter : IFilter<TContext>
	{
		var type = typeof(TFilter);
		var instance = (TFilter)Activator.CreateInstance(type);

		var node = new GenericPipeTreeNode<TContext>(instance);

		Children.Add(node);

		return this;
	}

	public async Task ExecuteAsync(TContext context)
	{
		foreach (GenericPipeTreeNode<TContext> node in this.Children)
		{
			await node.ExecuteAsync(context);
		}
	}
}

class MyContext : IPipeTreeContext
{
	public int Number { get; set; }

	public string Name { get; set; }

	public bool IsDeleted { get; set; }

	public object Clone()
	{
		return this.MemberwiseClone();
	}
	
	public void Update(object context)
	{
		MyContext updateContext = context as MyContext;

		if (updateContext == null)
		{
			return;
		}
		
		this.Number = updateContext.Number;
		this.Name = updateContext.Name;
		this.IsDeleted = updateContext.IsDeleted;
	}
}

class DoeSomething : IFilter<MyContext>
{
	public Task ExecuteAsync(MyContext context)
	{
		return Task.CompletedTask;
	}
}

void Main()
{
	var pipeTree = new GenericPipeTree<MyContext>();
	
	pipeTree
		.Register<DoeSomething>()
		.Register<DoeSomething>()
}

// Define other methods and classes here
