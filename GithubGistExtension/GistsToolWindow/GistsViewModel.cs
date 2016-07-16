using System.ComponentModel;
using System.Runtime.CompilerServices;
using GithubGistExtension.Annotations;

namespace GithubGistExtension
{
    public class GistsViewModel : INotifyPropertyChanged
    {



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}