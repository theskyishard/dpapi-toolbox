using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dpapi_toolbox
{
    class Program
    {
        private static readonly IAppOutput appOutput = new ConsoleOutput();

        static void Main(string[] args)
        {
            appOutput.Write($"Data Protection API Toolbox v{Assembly.GetExecutingAssembly().GetName().Version}");
            var configuration = BuildConfiguration();
            var protector = ConfigureDataProtection(configuration);
            bool shouldExit = false;

            do
            {
                PrintOptions(appOutput);
                var pressedKeyInfo = Console.ReadKey(true);
                appOutput.Write($"You pressed: {pressedKeyInfo.KeyChar}");

                switch (pressedKeyInfo.Key)
                {
                    case ConsoleKey.D0:
                        shouldExit = true;
                        break;
                    case ConsoleKey.D1:
                        var valueToEncrypt = GetInput(appOutput, "Enter value to encrypt:");
                        var encryptedResult = EncryptAsCurrentUser(protector, valueToEncrypt);
                        appOutput.Write("Encrypted value:" + Environment.NewLine);
                        appOutput.Write(encryptedResult);
                        break;
                    case ConsoleKey.D2:
                        var valueToDecrypt = GetInput(appOutput, "Enter value to decrypt:");
                        var decryptedResult = DecryptAsCurrentUser(protector, valueToDecrypt);
                        appOutput.Write("Decrypted value:" + Environment.NewLine);
                        appOutput.Write(decryptedResult);
                        break;
                    default:
                        appOutput.Write($"Unrecognized option '{pressedKeyInfo.KeyChar}'" + Environment.NewLine);
                        break;
                }
            } while (!shouldExit);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
        }

        private static IDataProtector ConfigureDataProtection(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDataProtection()
                             .SetApplicationName(configuration["AppSettings:ApplicationName"]);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var dataProtectionProvider = serviceProvider.GetService<IDataProtectionProvider>();
            var protector = dataProtectionProvider.CreateProtector(configuration["AppSettings:ProtectionPurpose"]);

            return protector;
        }

        private static string GetInput(IAppOutput output, string prompt)
        {
            output.Write(prompt + Environment.NewLine);
            var input = Console.ReadLine();
            return input;
        }

        private static string DecryptAsCurrentUser(IDataProtector protector, string input)
        {
            return protector.Unprotect(input);
        }

        private static string EncryptAsCurrentUser(IDataProtector protector, string input)
        {
            return protector.Protect(input);
        }

        private static void PrintOptions(IAppOutput appOutput)
        {
            var currentUser = Environment.UserName;

            appOutput.Write(Environment.NewLine + "Select an option:" + Environment.NewLine);
            appOutput.Write($"1. Encrypt (current user: {currentUser})");
            appOutput.Write($"2. Decrypt (current user: {currentUser})");
            appOutput.Write("0. Exit" + Environment.NewLine);
        }
    }
}
