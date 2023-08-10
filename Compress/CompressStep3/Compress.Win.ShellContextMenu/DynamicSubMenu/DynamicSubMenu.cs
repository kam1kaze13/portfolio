using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;

namespace DynamicSubMenus
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    [COMServerAssociation(AssociationType.Directory)]
    public class DynamicSubMenuExtension : SharpContextMenu
    {
        private ContextMenuStrip menu = new ContextMenuStrip();

        protected override bool CanShowMenu()
        {
            this.UpdateMenu();
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            menu.Items.Clear();
            var ext = Path.GetExtension(SelectedItemPaths.First());

            if (ext == ".pkg" && SelectedItemPaths.Count() == 1) 
            {
                this.MenuPackage();
            }
            else
            {
                this.MenuAllFiles();
            }

            return menu;
        }

        private void UpdateMenu()
        {
            menu.Dispose();
            menu = CreateMenu();
        }

        protected void MenuAllFiles()
        {
            ToolStripMenuItem CompressMenu;
            CompressMenu = new ToolStripMenuItem
            {
                Text = "Compress",
                Image = Properties.Resources.archive_icon
            };

                ToolStripMenuItem AddSubMenu;
                AddSubMenu = new ToolStripMenuItem
                {
                    Text = "Add to archive...",
                    Image = Properties.Resources.archive_icon
                };
                AddSubMenu.Click += (sender, args) => ShowItemName();

                var AddToSubMenu = new ToolStripMenuItem
                {
                    Text = "Add to " + Path.GetFileNameWithoutExtension(SelectedItemPaths.First()) + ".pkg",
                    Image = Properties.Resources.archive_icon
                };
                AddToSubMenu.Click += (sender, args) => ShowItemName();

            CompressMenu.DropDownItems.Add(AddSubMenu);
            CompressMenu.DropDownItems.Add(AddToSubMenu);

            menu.Items.Clear();
            menu.Items.Add(CompressMenu);
        }

        protected void MenuPackage()
        {
            ToolStripMenuItem CompressMenu;
            CompressMenu = new ToolStripMenuItem
            {
                Text = "Compress",
                Image = Properties.Resources.archive_icon
            };

                ToolStripMenuItem AddSubMenu;
                AddSubMenu = new ToolStripMenuItem
                {
                    Text = "Add to archive...",
                    Image = Properties.Resources.archive_icon
                };
                AddSubMenu.Click += (sender, args) => PackageAction("a",SelectedItemPaths);

                var AddToSubMenu = new ToolStripMenuItem
                {
                    Text = "Add to " + Path.GetFileNameWithoutExtension(SelectedItemPaths.First()) + ".pkg",
                    Image = Properties.Resources.archive_icon
                };
                AddToSubMenu.Click += (sender, args) => ShowItemName();

                var ExtractSubMenu = new ToolStripMenuItem
                {
                    Text = "Extract files ...",
                    Image = Properties.Resources.archive_icon
                };
                ExtractSubMenu.Click += (sender, args) => ShowItemName();

                var ExtractToCurrentFolderSubMenu = new ToolStripMenuItem
                {
                    Text = "Extract to current folder",
                    Image = Properties.Resources.archive_icon
                };
                ExtractToCurrentFolderSubMenu.Click += (sender, args) => ShowItemName();

                var ExtractToSubMenu = new ToolStripMenuItem
                {
                    Text = "Extract to " + Path.GetFileNameWithoutExtension(SelectedItemPaths.First()),
                    Image = Properties.Resources.archive_icon
                };
                ExtractToSubMenu.Click += (sender, args) => ShowItemName();

            CompressMenu.DropDownItems.Add(AddSubMenu);
            CompressMenu.DropDownItems.Add(AddToSubMenu);
            CompressMenu.DropDownItems.Add(ExtractSubMenu);
            CompressMenu.DropDownItems.Add(ExtractToCurrentFolderSubMenu);
            CompressMenu.DropDownItems.Add(ExtractToSubMenu);

            menu.Items.Clear();
            menu.Items.Add(CompressMenu);
        }

        private void ShowItemName()
        {
            FileAttributes attr = File.GetAttributes(SelectedItemPaths.First());
            RegistryKey localMachineKey = Registry.LocalMachine;
            string appPath = localMachineKey.OpenSubKey(@"SOFTWARE\WOW6432Node\AppTime, LLC\Compress")?.GetValue("App path")?.ToString();

            Process.Start(appPath);
        }

        private void PackageAction(string action, IEnumerable<string> paths, string pathTo = "")
        {
            RegistryKey localMachineKey = Registry.LocalMachine;
            string appPath = localMachineKey.OpenSubKey(@"SOFTWARE\WOW6432Node\AppTime, LLC\Compress")?.GetValue("App path")?.ToString();

            string arguments = action + ";";
            foreach (var path in paths)
            {
                arguments += path + ";";
            }
            arguments += pathTo;

            Process.Start(appPath,arguments);
        }
    }
}