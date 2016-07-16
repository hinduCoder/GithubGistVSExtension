using System;

namespace GithubGistExtension
{
    public interface ICommand
    {
        int CommandId { get; }
        Guid CommandSet { get; }
        void Handle(object sender);
    }
}