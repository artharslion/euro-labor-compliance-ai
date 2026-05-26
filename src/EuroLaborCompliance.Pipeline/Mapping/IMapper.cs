using EuroLaborCompliance.Pipeline.Models;

namespace EuroLaborCompliance.Pipeline.Mapping;

public interface IMapper
{
    Task<MappingResult> MapAsync(string markdown, string sourceFile);
}
