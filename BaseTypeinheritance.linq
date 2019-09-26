<Query Kind="Program" />

void Main()
{
	Text myContent = (Text)"blablabla";
	string xy = (string)myContent;
	
	xy.Dump();
}

public class BaseTypeWrapper<T>
{
	private T value;

	public BaseTypeWrapper(T value)
	{
		this.value = value;
	}

	public static implicit operator T(BaseTypeWrapper<T> key)
	{
		if (key == null)
		{
			throw new InvalidCastException();
		}

		return key.value;
	}

	public static implicit operator BaseTypeWrapper<T>(T key)
	{
		if (key == null)
		{
			throw new InvalidCastException();
		}

		return new BaseTypeWrapper<T>(key);
	}
}

public class Text : BaseTypeWrapper<string>
{
	public Text(string value)
		: base(value)
	{
	}

	public static implicit operator Text(string key)
	{
		if (key == null)
		{
			throw new InvalidCastException();
		}

		return new Text(key);
	}
}