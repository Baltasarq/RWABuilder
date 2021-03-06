﻿using System;
using System.Drawing;
using System.Windows.Forms;

using RWABuilder.Core;
using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Ui.MenuComponentGuiEditors {
    /// <summary>
    /// Named component GUI editor.
    /// This editor is the base for many other ones, in which
    /// at least the name of the component is edited.
    /// </summary>
    public abstract class NamedComponentGuiEditor: MenuComponentGuiEditor {
        protected NamedComponentGuiEditor(Panel panel, MenuComponentTreeNode mctn, MenuComponent mc)
            : base( panel, mctn, mc )
        {
			this.Build();
			this.ReadDataFromComponent();
        }

		public override void Show()
		{
			base.Show();

			this.edName.Text = this.MenuComponent.Name;
			this.pnlEdName.Show();
		}

		private void Build()
		{
            this.OnBuilding = true;
            this.Panel.SuspendLayout();
			this.pnlEdName = new Panel();
			this.pnlEdName.SuspendLayout();
			this.pnlEdName.Dock = DockStyle.Top;

			this.lblName = new Label();
			this.lblName.AutoSize = false;
			this.lblName.TextAlign = ContentAlignment.MiddleLeft;
			this.lblName.Dock = DockStyle.Left;
			this.lblName.Text = "Name:";

			this.edName = new TextBox();
			this.edName.Font = new Font( this.edName.Font, FontStyle.Bold );
			this.edName.Dock = DockStyle.Fill;
			this.edName.KeyUp += (sender, e) => this.OnNameModified();

			this.pnlEdName.Controls.Add( this.edName );
			this.pnlEdName.Controls.Add( this.lblName );
			this.pnlEdName.MaximumSize = new Size( int.MaxValue, this.edName.Height );
			this.Panel.Controls.Add( this.pnlEdName );
			this.pnlEdName.ResumeLayout( false );
            this.Panel.ResumeLayout( false );
            this.OnBuilding = false;
		}

		/// <summary>
		/// Gets the name created by the user.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get {
				return this.edName.Text.Trim();
			}
		}

		protected virtual void OnNameModified() {
			string name = this.Name;

			if ( !string.IsNullOrWhiteSpace( name ) ) {
				this.MenuComponent.Name = name;
				this.MenuComponentTreeNode.Text = name;
			}

			return;
		}

		public new void ReadDataFromComponent()
		{
            this.OnBuilding = true;
			this.edName.Text = this.MenuComponent.Name;
            this.OnBuilding = false;
		}

		private Panel pnlEdName;
		private Label lblName;
		private TextBox edName;
    }
}
