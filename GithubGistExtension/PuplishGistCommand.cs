﻿//------------------------------------------------------------------------------
// <copyright file="PuplishGistCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RestSharp;

namespace GithubGistExtension
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class PuplishGistCommand
    {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("dce27d4e-a5b7-4189-b13d-b7017e3494e7");

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PuplishGistCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PuplishGistCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            var commandService = ServiceProvider.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static PuplishGistCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new PuplishGistCommand(package);
        }

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = GetDTE();
            if (dte.ActiveDocument == null)
            {
                ShowMessageBox("Не получится", "Нет документа");
                return;
            }
            var activeDocument = dte.ActiveDocument;
            var fileName = activeDocument.Name;
            if (activeDocument.Selection == null || ((TextSelection) activeDocument.Selection).IsEmpty)
            {
                CreateGist(fileName, File.ReadAllText(activeDocument.FullName));
            }
            else
            {
                var selection = (TextSelection) activeDocument.Selection;
                CreateGist(fileName, selection.Text);
            }
        }

        private async void CreateGist(string fileName, string text)
        {
            var client = new RestClient("https://api.github.com");
            var request = new RestRequest("gists", Method.POST) {RequestFormat = DataFormat.Json};
            request.AddJsonBody(new JsonObject
            {
                ["files"] =
                    new JsonObject
                    {
                        [fileName] =
                            new JsonObject {["content"] = text}
                    }
            });
            var result = await client.ExecuteTaskAsync(request);

            var gistUrl = ((JsonObject) SimpleJson.DeserializeObject(result.Content))["html_url"] as string;
            Clipboard.SetText(gistUrl);
            ShowMessageBox("Гист создан. Ссылка в клипборде", gistUrl);
        }

        private DTE GetDTE()
        {
            return ServiceProvider.GetService(typeof (DTE)) as DTE;
        }

        private void ShowMessageBox(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}