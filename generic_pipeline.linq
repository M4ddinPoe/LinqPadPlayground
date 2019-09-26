<Query Kind="Program">
  <Reference>C:\dev\Steag\Terra\terra\Packages\Autofac.4.9.2\lib\net45\Autofac.dll</Reference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Autofac</Namespace>
</Query>

interface IFilter<TContext>
	where TContext : class
{
	Task ExecuteAsync(TContext context);
}

class Pipeline<TContext>
	where TContext : class
{
	private List<IFilter<TContext>> filterList = new List<IFilter<TContext>>();

	public Pipeline<TContext> Register<TFilter>()
		where TFilter: IFilter<TContext>
	{
		var type = typeof(TFilter);
		var instance = (TFilter)Activator.CreateInstance(type);
		
		filterList.Add(instance);
		
		return this;
	}

	public async Task ExecuteAsync(TContext context) 
	{
		foreach (IFilter<TContext> filter in this.filterList)
		{
			await filter.ExecuteAsync(context);
		}
	}
}

class MyContext
{
	public int Result { get; set;} = 0;
}

class Add2 : IFilter<MyContext> 
{
	public Task ExecuteAsync(MyContext context) 
	{
		context.Result += 2;
		return Task.CompletedTask;
	}
}

class Sub1 : IFilter<MyContext>
{
	public Task ExecuteAsync(MyContext context)
	{
		context.Result -= 1;
		return Task.CompletedTask;
	}
}

class Mul2 : IFilter<MyContext>
{
	public Task ExecuteAsync(MyContext context)
	{
		context.Result *= 2;
		return Task.CompletedTask;
	}
}

IContainer container;

async void Main()
{
	this.Configure();
	
	await this.Calculate();
}

private void Configure()
{
	var builder = new ContainerBuilder();
	this.RegisterPipelines(builder);
	this.container = builder.Build();
}

private void RegisterPipelines(ContainerBuilder builder)
{
	var firstPipeline = new Pipeline<MyContext>()
		.Register<Add2>()
		.Register<Sub1>()
		.Register<Add2>()
		.Register<Mul2>();

	var secondPipeline = new Pipeline<MyContext>()
		.Register<Add2>()
		.Register<Add2>()
		.Register<Mul2>()
		.Register<Sub1>()
		.Register<Mul2>();

	builder.Register(c => firstPipeline).Named<Pipeline<MyContext>>("calculation1");
	builder.Register(c => secondPipeline).Named<Pipeline<MyContext>>("calculation2");
}

private async Task Calculate() 
{
	var firstPipeline = this.container.ResolveNamed<Pipeline<MyContext>>("calculation1");
	var secondPipeline = this.container.ResolveNamed<Pipeline<MyContext>>("calculation2");

	var context1 = new MyContext() { Result = 5 };
	var context2 = new MyContext();

	await firstPipeline.ExecuteAsync(context1);
	await secondPipeline.ExecuteAsync(context2);

	context1.Result.Dump();
	context2.Result.Dump();
}

// Define other methods and classes here
