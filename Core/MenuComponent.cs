using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Core {
	/// <summary>
	/// Base class for all menu components:
	/// menu entries, functions, separators...
	/// </summary>
	public abstract class MenuComponent {
		public const string PathDelimiter = ": ";

        public MenuComponent(string name, Menu parent)
		{
			this.SetName( name );
			this.parent = parent;
			this.parent.Add( this );
            this.root = null;
		}

		protected MenuComponent(string name)
		{
			this.SetName( name );
			this.parent = null;
		}

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        /// <value>The name, as a string.</value>
        public virtual string Name {
            get {
                return this.name;
            }
            set {
				this.SetName( value );
            }
        }

		private void SetName(string value)
		{
			if ( string.IsNullOrWhiteSpace( value ) ) {
                throw new ArgumentNullException( "invalid name for menu component" );
            }

			value = ( value ?? "" ).Trim();

			if ( value != this.name ) {
				this.name = value;
			}

			return;
		}

		/// <summary>
		/// Gets or sets the parent of this menu component.
		/// </summary>
		/// <value>The parent.</value>
        public Menu Parent {
			get {
				return this.parent;
			}
			set {
				var menu = this as Menu;

				this.root = null;

				if ( menu != null ) {
					foreach ( MenuComponent mc in menu.MenuComponents ) {
						mc.root = null;
					}
				}

				if ( value != this.parent ) {
					if ( this.parent != null ) {
						this.parent.Remove( this );
					}

					this.parent = value;
				}
			}
		}

        /// <summary>
        /// Removes this instance, by calling itself to its parent.Remove().
        /// Its parent is a <see cref="MenuEntry"/> 
        /// </summary>
        public void Remove()
        {
            this.Parent.Remove( this );
        }

        /// <summary>
        /// Swaps this instance, by calling itself to its parent.SwapPrevious().
        /// Its parent is a <see cref="MenuEntry"/> 
        /// </summary>
        public void SwapPrevious()
        {
            this.Parent.SwapPrevious( this );
        }

        /// <summary>
        /// Swaps this instance, by calling itself to its parent.SwapNext().
        /// Its parent is a <see cref="MenuEntry"/> 
        /// </summary>
        public void SwapNext()
        {
            this.Parent.SwapNext( this );
        }

		/// <summary>
		/// Converts this menu component to XML.
		/// </summary>
		public abstract void ToXml(XmlWriter doc);

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <returns>Returns a MenuComponent[] vector, with the ancestors of this node.</returns>
		public MenuComponent[] GetPath()
		{
			MenuComponent mc = this.Parent;
			var path = new List<MenuComponent>();

			while ( mc != null ) {
				path.Insert( 0, mc );
				mc = mc.Parent;
			}

			return path.ToArray();
		}

		/// <summary>
		/// Gets the path to this <see cref="MenuComponent"/> as string.
		/// </summary>
		/// <returns>The path, as a string.</returns>
		/// <seealso cref="GetPath"/>
		public string GetPathAsString()
		{
			int delimiterLength = PathDelimiter.Length;
			var toret = new StringBuilder();
			MenuComponent[] path = this.GetPath();

			// Build the string representing this path
			foreach(MenuComponent mc in path) {
				toret.Append( mc.name );
				toret.Append( ": " );
			}
			// Remove last delimiter
			toret.Remove( toret.Length - delimiterLength, delimiterLength );

			return toret.ToString();
		}

        /// <summary>
        /// Gets the root of the menu this component pertains to.
        /// </summary>
        /// <value>The root, as a <see cref="RootMenu"/>.</value>
        public RootMenu Root {
            get {
                this.GetRoot();
                return this.root;
            }
        }

        private void GetRoot()
        {
            if ( this.root == null ) {
                MenuComponent mc = this;
                RootMenu rootMenu = mc as RootMenu;

                while( mc != null
                    && rootMenu == null )
                {
                    mc = mc.Parent;
                    rootMenu = mc as RootMenu;
                }

                if ( rootMenu != null ) {
                    this.root = rootMenu;
                }
            }

            return;
        }

		/// <summary>
		/// Looks in contents for the given string. Ignores case.
		/// </summary>
		/// <returns><c>true</c>, if in contents there is txt, <c>false</c> otherwise.</returns>
		/// <param name="txt">Text.</param>
		public virtual bool LookInContentsFor(string txt)
		{
			return ( this.Name.ToLower().IndexOf( txt.Trim().ToLower() ) >= 0 );
		}

		/// <summary>
		/// Copies this instance.
		/// </summary>
		/// <param name="newParentOrOwner">
		/// The <see cref="Menu"/> which will be the parent or owner of the copy.
		/// </param>
		/// <returns>
		/// A new <see cref="MenuComponent"/>, which is an exact copy of this one.
		/// </returns>
		public abstract MenuComponent Copy(MenuComponent newParentOrOwner);

		public override string ToString()
		{
			return string.Format( "[MenuComponent: Name={0}]", Name );
		}

        private string name;
		private Menu parent;
        private RootMenu root;
	}
}

