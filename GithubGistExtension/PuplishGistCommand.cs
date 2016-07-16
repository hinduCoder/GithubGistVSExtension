using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using GithubGistExtension.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ninject;
using Task = System.Threading.Tasks.Task;

namespace GithubGistExtension
{
    // ReSharper disable once UnusedMember.Global
    internal sealed class PuplishGistCommand : ICommand
    {
        public int CommandId { get; } = 0x0100;
        public Guid CommandSet { get; } = new Guid("dce27d4e-a5b7-4189-b13d-b7017e3494e7");

        [Inject]
        private DTE DTE { get; set; }
        [Inject]
        private Package Package { get; set; }
        [Inject]
        private GithubGistApi Github { get; set; }

        async void ICommand.Handle(object sender)
        {
            if (DTE.ActiveDocument == null)
            {
                ShowMessageBox("Не получится", "Нет документа");
                return;
            }
            var activeDocument = DTE.ActiveDocument;
            var fileName = activeDocument.Name;
            if (activeDocument.Selection == null || ((TextSelection) activeDocument.Selection).IsEmpty)
            {
                await CreateGist(fileName, File.ReadAllText(activeDocument.FullName));
            }
            else
            {
                var selection = (TextSelection) activeDocument.Selection;
                await CreateGist(fileName, selection.Text);
            }
        }

        private async Task CreateGist(string fileName, string text)
        {
            await Github.AuthorizeAsync();
            var gistUrl = await Github.CreateGistAsync(true, fileName, text);
            Clipboard.SetText(gistUrl);
            ShowMessageBox("Гист создан. Ссылка в клипборде", gistUrl);
        }


        private void ShowMessageBox(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                Package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}