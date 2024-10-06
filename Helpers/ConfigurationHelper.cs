using System;
using Microsoft.Extensions.Configuration;

namespace CodeGenerator.Helpers;

public class ConfigurationHelper
{
    private readonly IConfiguration _configuration
   private ConfigurationHelper(string configFilePath) 
   {
        if(!File.Exists(configFilePath))
            throw new FileNotFoundException($"Configuration file '{configFilePath}' not found!");
        _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFilePath)
                .Build();
   }
   public string GetConnectionString(string connectionStringName)
    {
        return _configuration.GetConnectionString(connectionStringName);
    }
}
