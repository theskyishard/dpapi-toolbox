using System;

namespace dpapi_toolbox
{
    public class ConsoleOutput : IAppOutput
    {
        public void Write(string output)
        {
            Console.WriteLine(output);
        }
    }
}