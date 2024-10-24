using GitGet.Model;

namespace GitGet.Actions;

internal interface IAction
{
    Task<int> Run(Args args);
}