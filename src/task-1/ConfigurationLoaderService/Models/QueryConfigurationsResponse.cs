namespace ConfigurationLoaderService.Models;

public record QueryConfigurationsResponse(IEnumerable<ConfigurationItemDto> Items, string? PageToken);