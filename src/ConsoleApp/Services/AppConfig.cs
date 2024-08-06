using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Services
{
    internal class AppConfig
    {
        private IConfigurationRoot config;

        public AppConfig() 
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);

            config = builder.Build();
        }

        public T? GetValue<T>(string name) where T : class
        {
            return config.GetValue<T>(name);
        }

        public IList<string> GetArray(string name)
        {
            var section = config.GetSection(name);
            var result = new List<string>();
            var contador = 0;

            while(section.GetSection(contador.ToString()).Value != null)
            {
                var sec = section.GetSection(contador.ToString()).Value;

                result.Add(sec);

                contador++;
            }

            return result;
        }
    }
}
