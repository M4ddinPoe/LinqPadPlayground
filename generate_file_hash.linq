<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Linq.dll</Reference>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

static string CalculateMD5(string filename)
{
	using (var md5 = MD5.Create())
	{
		using (var stream = File.OpenRead(filename))
		{
			var hash = md5.ComputeHash(stream);
			return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
		}
	}
}

void Main()
{
	CalculateMD5(@"C:\Temp\Test.xls").Dump();
	
	CalculateMD5(@"C:\Temp\Test2.xlsx").Dump();
	
	CalculateMD5(@"C:\Temp\Test3.xls").Dump();
}

// Define other methods and classes here
