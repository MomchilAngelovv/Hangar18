namespace Hangar18.Services;

public class Logger
{
	public void LogMessage(string message)
	{
		Console.WriteLine($"[{DateTime.UtcNow.ToShortTimeString()}] {message}");
	}
}
