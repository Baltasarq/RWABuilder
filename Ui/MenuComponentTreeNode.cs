using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Ui
{
	public class MenuComponentTreeNode: TreeNode {
		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.MenuComponentTreeNode"/> class.
		/// Each treenode has a corresponding core component.
		/// </summary>
		/// <param name="text">The text for the menu (and the corresponding core component's name)</param>
		/// <param name="mc">The core component for this entry.</param>
		public MenuComponentTreeNode(string text, MenuComponent mc)
			:base( text )
		{
			this.menuComponent = mc;
		}

		/// <summary>
		/// Gets the menu component associated to this tree node.
		/// </summary>
		/// <value>
		/// The menu component, as a <see cref="MenuComponent"/> object.
		/// </value>
		public MenuComponent MenuComponent {
			get {
				return this.menuComponent;
			}
		}

		private MenuComponent menuComponent;
	}
}
