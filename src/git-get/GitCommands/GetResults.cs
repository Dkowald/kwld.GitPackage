namespace GitGet.GitCommands;

/// <summary>
/// 
/// </summary>
/// <param name="Commit"></param>
/// <param name="TotalItems">Total file count for items in the selected root.</param>
/// <param name="IncludedItemsCount">Total files included out of the total items</param>
/// <param name="IgnoredItemsCount">Total files ignored from the included total</param>
internal record GetResults(string Commit, int TotalItems, int IncludedItemsCount, int IgnoredItemsCount);