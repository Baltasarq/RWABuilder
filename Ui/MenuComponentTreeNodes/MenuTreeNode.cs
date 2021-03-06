﻿using System;
using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Ui;
using RWABuilder.Core.MenuComponents;
using RWABuilder.Ui.MenuComponentGuiEditors;

namespace RWABuilder.Ui.MenuComponentTreeNodes {
    /// <summary>
    /// Tree node for a menu entry.
    /// </summary>
    public class MenuTreeNode: MenuComponentTreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Ui.TreeNodes.MenuEntryTreeNode"/> class.
		/// </summary>
		/// <param name="menu">The menu component being represented.</param>
		public MenuTreeNode(Core.MenuComponents.Menu menu)
			:base( menu )
		{
			this.Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RWABuilder.Ui.MenuComponentTreeNodes.MenuTreeNode"/> class.
		/// This is internally used in order to created the top root menu.
		/// </summary>
		/// <param name="rme">Rme.</param>
		protected MenuTreeNode(RootMenu rme)
			:base( rme )
		{
            this.Init();
		}

        private void Init()
        {
            this.ImageIndex = this.SelectedImageIndex =
                UserAction.LookUp( "addmenu" ).ImageIndex;
        }

		protected override MenuComponentGuiEditor CreateEditor(Panel pnl)
		{
			return new MenuGuiEditor( pnl, this, this.MenuComponent );
		}
    }
}
