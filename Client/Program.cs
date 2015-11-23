using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LianaMailerClient
{
	// Sample program to communicate with LM API endpoint "echoMessage"
	class Program
	{
		static void Main(string[] args)
		{
			Client client = new Client("<REALM>", "<USER ID>", "<SECRET KEY>");

			EchoResponse response = client.EchoMessage("echo this");
			
			Console.WriteLine("succeed: " + response.Succeed);
			Console.WriteLine("result: " + response.Result);
		}
	}
}
