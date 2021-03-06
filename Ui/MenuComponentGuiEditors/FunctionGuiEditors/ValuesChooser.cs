using System;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Function = RWABuilder.Core.MenuComponents.Function;

namespace RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors {
	public class ValuesChooser: Form {
		public ValuesChooser(string[] values, bool multiple)
		{
			Trace.WriteLine( "ValuesChooser: Booting dialog..." );

			if ( values == null
			  || values.Length < 1 )
			{
				Trace.Indent();
				Trace.WriteLine( "ValuesChooser: ERROR: null values" );
				Trace.Unindent();
				throw new ArgumentException( "null values" );
			}

			this.values = values;
			this.multiple = multiple;
			this.Build();
		}

		private void Populate()
		{
			this.lbValues.Items.Clear();
			this.lbValues.Items.AddRange( this.values );
			this.lbValues.SelectedIndex = 0;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown( e );

			this.Populate();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing( e );

			if ( this.DialogResult != DialogResult.OK ) {
				DialogResult result = MessageBox.Show(
					"Changes will be lost. Are you sure?",
					"Discard changes",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Information,
					MessageBoxDefaultButton.Button2 );

				if ( result == DialogResult.No ) {
					e.Cancel = true;
				} else {
					Trace.WriteLine( "ValuesChooser: Closing dialog..." );
				}
			}
				
			return;
		}

		private void BuildIcon()
		{
			Bitmap appIconBmp;
			System.Reflection.Assembly entryAssembly;

			try {
				entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

				appIconBmp = new Bitmap(
					entryAssembly.GetManifestResourceStream( "RWABuilder.Res.appIcon.png" )
				);

			} catch (Exception) {
				throw new ArgumentException( "Unable to load embedded app icon" );
			}

			this.Icon = Icon.FromHandle( appIconBmp.GetHicon() );
		}

		private void BuildToolbar()
		{
			var quitAction = UserAction.LookUp( "quit" );
			var saveAction = UserAction.LookUp( "save" );

			this.tbToolbar = new ToolStrip();
			this.tbToolbar.BackColor = Color.DarkGray;
			this.tbToolbar.Dock = DockStyle.Top;
			this.tbToolbar.ImageList = UserAction.ImageList;

			// Buttons
			this.tbbQuit = new ToolStripButton();
			this.tbbQuit.ImageIndex = quitAction.ImageIndex;
			this.tbbQuit.ToolTipText = quitAction.Text;
			this.tbbQuit.Click += (sender, e) => {
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			};

			this.tbbSave = new ToolStripButton();
			this.tbbSave.ImageIndex = saveAction.ImageIndex;
			this.tbbSave.ToolTipText = saveAction.Text;
			this.tbbSave.Click += (sender, e) =>  {
				this.DialogResult = DialogResult.OK;
				this.Close();
			};

			this.tbToolbar.Items.Add( tbbQuit );
			this.tbToolbar.Items.Add( tbbSave );
		}

		private void BuildListBox()
		{
			this.pnlValues = new GroupBox();
			this.pnlValues.SuspendLayout();
			this.pnlValues.Dock = DockStyle.Fill;
			this.pnlValues.Padding = new Padding( 5 );
			this.pnlValues.Font = new Font( this.pnlValues.Font, FontStyle.Bold );
			this.pnlValues.Text = "Values";

			this.lbValues = new ListBox();

			if ( this.Multiple ) {
				this.lbValues.SelectionMode = SelectionMode.MultiSimple;
			} else {
				this.lbValues.SelectionMode = SelectionMode.One;
			}

			this.lbValues.BackColor = Color.Wheat;
			this.lbValues.Dock = DockStyle.Fill;
			this.lbValues.Font = new Font( FontFamily.GenericMonospace, 12 );

			this.pnlValues.Controls.Add( this.lbValues );
			this.pnlValues.ResumeLayout( false );
		}

		private void Build()
		{
			this.BuildIcon();
			this.BuildToolbar();
			this.BuildListBox();

			// Add components
			this.Controls.Add( this.pnlValues );
			this.Controls.Add( this.tbToolbar );

			// Polish
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = this.MaximizeBox = false;
			this.Text = "Values chooser";
			this.MinimumSize = new Size( 320, 240 );
		}

		/// <summary>
		/// Gets the index of the selected item.
		/// </summary>
		/// <returns>The selected index, as an int.</returns>
		public int[] GetSelectedIndexes()
		{
			ListBox.SelectedIndexCollection selections = this.lbValues.SelectedIndices;
			int[] toret = new int[ selections.Count ];

			for(int i = 0; i < selections.Count; ++i) {
				toret[ i ] = Math.Max( 0, selections[ i ] );
			}

			Trace.WriteLine( "ValuesChooser: selected index: " + toret[0] );
			return toret;
		}

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <returns>The selected item, as string.</returns>
		public string[] GetSelectedItems()
		{
			int[] indexes = this.GetSelectedIndexes();
			string[] toret = new string[ indexes.Length ];

			for (int i = 0; i < indexes.Length; ++i) {
				toret[ i ] = this.Values[ indexes[ i ] ];
			}

			Trace.WriteLine( "ValuesChooser: selected item: " + toret[ 0 ] );
			return toret;
		}

		/// <summary>
		/// Gets the selected items as list.
		/// </summary>
		/// <returns>The selected items as a comma-separated list in a string.</returns>
		/// <param name="separator">The separator to use.</param>
		public string GetSelectedItemsAsList(char separator = ',')
		{
			StringBuilder toret = new StringBuilder();
			string[] values = this.GetSelectedItems();

			for (int i = 0; i < values.Length; ++i) {
				toret.Append( values[ i ] );

				if ( i < ( values.Length - 1 ) ) {
					toret.Append( separator );
				}
			}

			return toret.ToString();
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values, as string[].</value>
		public string[] Values {
			get {
				return this.values;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this
		/// <see cref="RWABuilder.Ui.MenuComponentGuiEditors.FunctionGuiEditors.ValuesChooser"/> allows multiple
		/// selections.
		/// </summary>
		/// <value><c>true</c> if allows multiple selections; otherwise, <c>false</c>.</value>
		public bool Multiple {
			get {
				return this.multiple;
			}
		}

		private bool multiple;
		private ToolStrip tbToolbar;
		private ToolStripButton tbbQuit;
		private ToolStripButton tbbSave;

		private GroupBox pnlValues;
		private ListBox lbValues;

		private string[] values;
	}
}

