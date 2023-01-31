namespace Leo.Subtypes.Settings
{
    public interface ISubTypeSettings
    {
        AutoDiscoverySettings AutoDiscovery { get; set; }

        SubTypeBlobSettings Blob { get; set; }

        SubTypeIOSettings IO { get; set; }
    }
}