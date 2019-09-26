<Query Kind="Program" />

public void SetFailed(Func<object> field)
{
	object o = field();
	o.Dump();
	nameof(o).Dump();
}

public static string GetMemberName(Expression<Func<Item, object>> expression)
{
	return GetMemberName(expression.Body);
}

private static string GetMemberName(Expression expression)
{
	if (expression == null)
	{
		throw new ArgumentException("expressionCannotBeNullMessage");
	}

	if (expression is MemberExpression)
	{
		// Reference type property or field
		var memberExpression = (MemberExpression)expression;
		return memberExpression.Member.Name;
	}

	if (expression is MethodCallExpression)
	{
		// Reference type method
		var methodCallExpression = (MethodCallExpression)expression;
		return methodCallExpression.Method.Name;
	}

	if (expression is UnaryExpression)
	{
		// Property, field of method returning value type
		var unaryExpression = (UnaryExpression)expression;
		return GetMemberName(unaryExpression);
	}

	throw new ArgumentException("invalidExpressionMessage");
}

public class Item
{
	public string Data { get; set; }
}

void Main()
{
	var item = new Item() { Data = "Some data" };
	
	SetFailed(() => item.Data);
	
	string name = GetMemberName(i => i.Data);
	name.Dump();
}

// Define other methods and classes here
