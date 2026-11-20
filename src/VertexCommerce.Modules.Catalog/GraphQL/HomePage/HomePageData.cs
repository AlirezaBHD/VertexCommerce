using System.Diagnostics.CodeAnalysis;

namespace VertexCommerce.Modules.Catalog.GraphQL.HomePage;

/// <summary>
/// Root schema marker type for aggregated home page GraphQL queries.
/// </summary>
[SuppressMessage("Major Code Smell", "S2094:Classes should not be empty", Justification = "Marker type for HotChocolate GraphQL schema root node")]
public sealed class HomePageData;
