using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using static Vault.AspNet.VaultConfigurationSource;

namespace Vault.AspNet;

public sealed class VaultKeyValueConfigurationProvider(
    IVaultClient vaultClient, string path, int? version = null,
    string? mountPoint = null, string? wrapTimeToLive = null) : ConfigurationProvider
{
    public override void Load()
    {
        var secret = vaultClient.V1.Secrets.KeyValue.V2
            .ReadSecretAsync(path, version, mountPoint, wrapTimeToLive)
            .GetAwaiter()
            .GetResult();

        try
        {
            foreach (var kvp in secret.Data.Data)
            {
                if (Data.ContainsKey(kvp.Key))
                {
                    Data[kvp.Key] = kvp.Value.ToString();
                    continue;
                }

                Data.Add(kvp.Key, kvp.Value.ToString());
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load JSON string configuration", ex);
        }
    }
}

public sealed class VaultConfigurationSource(VaultSettings settings) : IConfigurationSource
{
    public sealed class VaultSettings
    {
        public string Token { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int? Version { get; set; }
        public string? MountPoint { get; set; }
        public string? WrapTimeToLive { get; set; }
    }


    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var authMethod = new TokenAuthMethodInfo(settings.Token);
        var vaultClientSettings = new VaultClientSettings(settings.Url, authMethod);
        var test = new VaultClient(vaultClientSettings);
        return new VaultKeyValueConfigurationProvider(test, settings.Path, settings.Version, settings.MountPoint, settings.WrapTimeToLive);
    }
}
