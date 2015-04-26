using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace Trains
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmTrains : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbFilename;
		private System.Windows.Forms.Button btnFindSolution;
		private System.Windows.Forms.Button btnShowTracks;
		private TrainSystem trainSystem;
		private System.Windows.Forms.ListBox lbTracks;
		private System.Windows.Forms.ListBox lbSolution;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmTrains()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label4 = new System.Windows.Forms.Label();
			this.cbFilename = new System.Windows.Forms.ComboBox();
			this.btnFindSolution = new System.Windows.Forms.Button();
			this.btnShowTracks = new System.Windows.Forms.Button();
			this.lbTracks = new System.Windows.Forms.ListBox();
			this.lbSolution = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(128, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 23);
			this.label4.TabIndex = 3;
			this.label4.Text = "Track File";
			// 
			// cbFilename
			// 
			this.cbFilename.Location = new System.Drawing.Point(192, 24);
			this.cbFilename.Name = "cbFilename";
			this.cbFilename.Size = new System.Drawing.Size(121, 21);
			this.cbFilename.TabIndex = 5;
			// 
			// btnFindSolution
			// 
			this.btnFindSolution.Location = new System.Drawing.Point(16, 24);
			this.btnFindSolution.Name = "btnFindSolution";
			this.btnFindSolution.Size = new System.Drawing.Size(96, 23);
			this.btnFindSolution.TabIndex = 4;
			this.btnFindSolution.Text = "Find Solution";
			this.btnFindSolution.Click += new System.EventHandler(this.btnFindSolution_Click);
			// 
			// btnShowTracks
			// 
			this.btnShowTracks.Location = new System.Drawing.Point(400, 24);
			this.btnShowTracks.Name = "btnShowTracks";
			this.btnShowTracks.Size = new System.Drawing.Size(80, 23);
			this.btnShowTracks.TabIndex = 7;
			this.btnShowTracks.Text = "Show Tracks";
			this.btnShowTracks.Click += new System.EventHandler(this.btnShowTracks_Click);
			// 
			// lbTracks
			// 
			this.lbTracks.Location = new System.Drawing.Point(400, 96);
			this.lbTracks.Name = "lbTracks";
			this.lbTracks.Size = new System.Drawing.Size(120, 95);
			this.lbTracks.TabIndex = 8;
			// 
			// lbSolution
			// 
			this.lbSolution.Location = new System.Drawing.Point(32, 96);
			this.lbSolution.Name = "lbSolution";
			this.lbSolution.Size = new System.Drawing.Size(232, 264);
			this.lbSolution.TabIndex = 9;
			// 
			// frmTrains
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(608, 442);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lbSolution,
																		  this.lbTracks,
																		  this.btnShowTracks,
																		  this.label4,
																		  this.cbFilename,
																		  this.btnFindSolution});
			this.Name = "frmTrains";
			this.Text = "Trains";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmTrains());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			// Get the filenames in the palindrome executable directory ending in .txt
			string currentDir = Directory.GetCurrentDirectory();
			string [] wordFiles = Directory.GetFiles(currentDir,"*.txt");

			// Strip off the directory from the filenames and add them to the drop-down list
			// of files containing word lists from which to find palindromes
			foreach (string wordFile in wordFiles)
			{
				cbFilename.Items.Add(wordFile.Substring(wordFile.LastIndexOf('\\') + 1));
				cbFilename.SelectedIndex = 0;
			}
		}

		private void btnFindSolution_Click(object sender, System.EventArgs e)
		{
			// First, instantiate the train system object, passing an input file containing the list of tracks.
			trainSystem = new TrainSystem(cbFilename.Text);

			// Variable to hold the distance between two stations
			int distance;

			// Find distance A-B-C
			distance = trainSystem.GetDistance("ABC");
			if (distance == 0)
				lbSolution.Items.Add("Output #1: No Such Route");
			else
				lbSolution.Items.Add("Output #1: " + distance.ToString());
			
			// Find distance A-D
			distance = trainSystem.GetDistance("AD");
			if (distance == 0)
				lbSolution.Items.Add("Output #2: No Such Route");
			else
				lbSolution.Items.Add("Output #2: " + distance.ToString());

			// Find distance A-D-C
			distance = trainSystem.GetDistance("ADC");
			if (distance == 0)
				lbSolution.Items.Add("Output #3: No Such Route");
			else
				lbSolution.Items.Add("Output #3: " + distance.ToString());

			// Find distance A-E-B-C-D
			distance = trainSystem.GetDistance("AEBCD");
			if (distance == 0)
				lbSolution.Items.Add("Output #4: No Such Route");
			else
				lbSolution.Items.Add("Output #4: " + distance.ToString());

			// Find distance A-E-D
			distance = trainSystem.GetDistance("AED");
			if (distance == 0)
				lbSolution.Items.Add("Output #5: No Such Route");
			else
				lbSolution.Items.Add("Output #5: " + distance.ToString());

			// Get number of routes between C and C with less than (or equal to) 3 stops
			int numRoutes = trainSystem.GetAllRoutesWithMaxStops('C', 'C', 3);
			lbSolution.Items.Add("Output #6: " + numRoutes.ToString());

			// Get number of routes between A and C with exactly 4 stops
			numRoutes = trainSystem.GetNumRoutesWithNStops('A', 'C', 4);
			lbSolution.Items.Add("Output #7: " + numRoutes.ToString());

			// Get shortest route between A and C 
			distance = trainSystem.GetShortestRoute('A', 'C');
			lbSolution.Items.Add("Output #8: " + distance.ToString());

			// Get shortest route from B to B 
			distance = trainSystem.GetShortestRoute('B', 'B');
			lbSolution.Items.Add("Output #9: " + distance.ToString());
	
			// Get number of routes between C and C that have a distance less than 30
			distance = trainSystem.GetAllRoutesWithMaxDistance('C', 'C', 30);
			lbSolution.Items.Add("Output #10: " + distance.ToString());

/* Some test cases to exercise other possibilities */
/*
			// Get number of routes between A and B with less than (or equal to) 4 stops
			numRoutes = trainSystem.GetAllRoutesWithMaxStops('A', 'B', 4);
			lbSolution.Items.Add("Output #11: " + numRoutes.ToString());

			// Get number of routes between A and B with exactly 7 stops
			numRoutes = trainSystem.GetNumRoutesWithNStops('A', 'B', 7);
			lbSolution.Items.Add("Output #12: " + numRoutes.ToString());

			// Get shortest route between A and E 
			distance = trainSystem.GetShortestRoute('A', 'E');
			lbSolution.Items.Add("Output #13: " + distance.ToString());

			// Get shortest route from A to E 
			distance = trainSystem.GetShortestRoute('A', 'E');
			lbSolution.Items.Add("Output #14: " + distance.ToString());
	
			// Get number of routes between A and B that have a distance less than 30
			distance = trainSystem.GetAllRoutesWithMaxDistance('A', 'B', 30);
			lbSolution.Items.Add("Output #15: " + distance.ToString());
*/
		}

		private void btnShowTracks_Click(object sender, System.EventArgs e)
		{
			// First, instantiate the train system object, passing an input file containing the list of tracks.
			trainSystem = new TrainSystem(cbFilename.Text);

			// Loop through the tracks and spit 'em out 
			foreach (Track track in trainSystem.mTracks)
				lbTracks.Items.Add(track.Source.Name + " " + track.Target.Name + " " + track.Distance.ToString());
		}
	}
}
