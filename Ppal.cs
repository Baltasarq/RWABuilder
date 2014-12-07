using System;
using System.Diagnostics;
using System.Windows.Forms;

using RAppMenu.Core;
using RAppMenu.Ui;

namespace RAppMenu {
	public class Ppal {
		[STAThread]
		public static void Main()
		{
			Form mainForm = null;

            AppInfo.BuildLog();

			try {
				mainForm = new MainWindow();
				Application.EnableVisualStyles();
				Application.Run( mainForm );
			}
			catch(Exception exc) {
                if ( mainForm != null ) {
                    mainForm.Close();
                }

                MessageBox.Show( null, exc.Message, AppInfo.Name );
                Trace.WriteLine( "[CRITICAL] " + DateTime.Now + ": " + exc.Message );
                Trace.WriteLine( exc.StackTrace );
			}
		}
	}
}
