using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using Ninject.Activation.Strategies;
using Ninject.Modules;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Extensions;
using SharpFlame.Gui.Actions;
using SharpFlame.Gui.Sections;
using SharpFlame.Mapping;
using SharpFlame.Settings;
using SharpFlame.Util;

namespace SharpFlame.Gui.Forms
{
	public class MainForm : Form
	{
        internal TextureTab TextureTab { get; set; }
        
        internal TerrainTab TerrainTab { get; set; }
        
        internal HeightTab HeightTab { get; set; }
        //[Inject]
        //internal ResizeTab ResizeTab { get; set; }
        
        internal PlaceObjectsTab PlaceObjectsTab { get; set; }
        
        internal ObjectTab ObjectTab { get; set; }
        
        
        internal MapPanel MapPanel { get; set; }

        [Inject]
        internal LoadMapCommand LoadMapCommand { get; set; }

        [Inject]
        internal SettingsManager Settings { get; set; }

	    internal ViewInfo ViewInfo { get; set; }

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        public void SetTitle(string mapName)
	    {
            this.Title = string.Format("{0} - {1} {2}", mapName, Constants.ProgramName, Constants.ProgramVersion());
	    }

	    [EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
	    public void OnMapLoad(Map newMap)
	    {
			if( newMap == null )
	        {
	            this.SetTitle("No Map");
	        }
	        else
	        {
	            var mapName = newMap.InterfaceOptions.FileName;
	            this.SetTitle(mapName);
	        }
	    }

		[EventSubscription(EventTopics.OnMapSave, typeof(OnPublisher))]
		public void OnMapSave(string path)
		{
			var map = MapPanel.MainMap;
			var mapFileTitle = "";

			//menuSaveFMapQuick.Text = "Quick Save fmap";
			//menuSaveFMapQuick.Enabled = false;
			if (map == null)
			{
				mapFileTitle = "No Map";
				//tsbSave.ToolTipText = "No Map";
			}
			else
			{
				if (map.PathInfo == null)
				{
					mapFileTitle = "Unsaved map";
					//tsbSave.ToolTipText = "Save FMap...";
				}
				else
				{
					var splitPath = new sSplitPath(map.PathInfo.Path);
					if (map.PathInfo.IsFMap)
					{
						mapFileTitle = splitPath.FileTitleWithoutExtension;
						var quickSavePath = map.PathInfo.Path;
						//tsbSave.ToolTipText = "Quick save FMap to {0}".Format2(quickSavePath);
						//menuSaveFMapQuick.Text = "Quick Save fmap to \"";
						if (quickSavePath.Length <= 32)
						{
							//menuSaveFMapQuick.Text += quickSavePath;
						}
						else
						{
							//menuSaveFMapQuick.Text += quickSavePath.Substring(0, 10) + "..." + quickSavePath.Substring(quickSavePath.Length - 20, 20);
						}
						//menuSaveFMapQuick.Text += "\"";
						//menuSaveFMapQuick.Enabled = true;
					}
					else
					{
						mapFileTitle = splitPath.FileTitle;
						//tsbSave.ToolTipText = "Save FMap...";
					}
				}
				map.SetTabText();
			}

			var title = mapFileTitle;
			this.SetTitle(title);
		}

	    public MainForm()
	    {
	        Init();
	        App.ShowWarnings(SharpFlameApplication.InitializeResult);
	    }

	    void Init()
	    {
	        ClientSize = new Size(1024, 768);
	        this.SetTitle("No Map");
	        Icon = Resources.ProgramIcon;

            this.MapPanel = new MapPanel();
		    this.MapPanel.OnSaveFMap += (sender, args) => new SaveFMapCommand(this, this.EventBroker).Execute();
	        this.TextureTab = new TextureTab();
	        this.TerrainTab = new TerrainTab();
	        //this.PlaceObjectsTab = new PlaceObjectsTab();
	        //this.ObjectTab = new ObjectTab();


	        var tabControl = new TabControl();
	        tabControl.Pages.Add(new TabPage {Text = "Textures", Content = this.TextureTab});
	        tabControl.Pages.Add(new TabPage {Text = "Terrain", Content = this.TerrainTab});
		    tabControl.Pages.Add(new HeightTab());
	        tabControl.Pages.Add(new ResizeTab());
	        //tabControabPages.Add(new TabPage {Text = "Resize", Content = this.ResizeTab});
		    tabControl.Pages.Add(new PlaceObjectsTab());
	        tabControl.Pages.Add(new ObjectTab());
	        tabControl.Pages.Add(new LabelsTab());

	        tabControl.SelectedIndexChanged += delegate
	            {
                    //ETO 1.0 
	                //tabControl.SelectedPage.Content.OnShown(EventArgs.Empty);
	                tabControl.SelectedPage.Content.Properties.TriggerEvent(ShownEvent, this, EventArgs.Empty);
	            };

            this.Closing += MainForm_Closing;

	        var splitter = new Splitter
	            {
	                Position = 392,
	                FixedPanel = SplitterFixedPanel.Panel1,
	                Panel1 = tabControl,
                    Panel2 = this.MapPanel
	            };

	        // Set the content of the form to use the layout
	        Content = splitter;
            
	        GenerateMenuToolBar();
	        //Maximize();
        }

        void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //App.Kernel.Dispose();
        }

	    private void GenerateMenuToolBar()
		{
			this.Menu = new MenuBar();
			// create standard system menu (e.g. for OS X)

			// add our own items to the menu
			var file = Menu.Items.GetSubmenu("&File", 100);
            file.Items.Add(new Command() { MenuText = "&New Map" });
	        file.Items.AddSeparator();
            file.Items.Add(LoadMapCommand);
            file.Items.AddSeparator();

            var saveMenu = file.Items.GetSubmenu("&Save", 300);
	        saveMenu.Items.Add(new SaveFMapCommand(this, this.EventBroker) {MenuText = "&Map fmap"});
            saveMenu.Items.AddSeparator();
            saveMenu.Items.Add(new Command() { MenuText = "&Quick Save fmap" });
            saveMenu.Items.AddSeparator();
            saveMenu.Items.Add(new Command() { MenuText = "Export map &LND" });
            saveMenu.Items.AddSeparator();
            saveMenu.Items.Add(new Command() { MenuText = "Export &Tile Types" });
            saveMenu.Items.AddSeparator();
            saveMenu.Items.Add(new Command() { MenuText = "Minimap Bitmap" });
            saveMenu.Items.Add(new Command() { MenuText = "Heightmap Bitmap" });

            file.Items.Add(new Command() { MenuText = "&Import" });
            file.Items.AddSeparator();
            file.Items.Add(new CompileMapCommand(this), 500);
            file.Items.AddSeparator();
	        file.Items.Add(new Actions.QuitCommand());

            var toolsMenu = Menu.Items.GetSubmenu("&Tools", 600);
			toolsMenu.Items.GetSubmenu ("Reinterpret Terrain", 100);
			toolsMenu.Items.GetSubmenu ("Water Triangle Correction", 200);
			toolsMenu.Items.AddSeparator ();
			toolsMenu.Items.GetSubmenu ("Flatten Under Oils", 300);
			toolsMenu.Items.GetSubmenu ("Flatten Under Structures", 400);
			toolsMenu.Items.AddSeparator ();
			toolsMenu.Items.GetSubmenu ("Generator", 500);

            var editMenu = Menu.Items.GetSubmenu ("&Edit", 700);
	        editMenu.Items.Add(new Actions.SettingsCommand());

			var help = Menu.Items.GetSubmenu("&Help", 1000);
	        help.Items.Add(new Actions.AboutCommand());

			// optional, removes empty submenus and duplicate separators
			// menu.Items.Trim();

	        var testing = Menu.Items.GetSubmenu("TESTING");
	        testing.Items.GetSubmenu("CMD1").Click += (sender, args) =>
	            {
	                this.ViewInfo.LookAtTile(new XYInt(1, 1));
	            };
	        testing.Items.GetSubmenu("CMD2 - ViewPos").Click += (sender, args) =>
	            {
	                this.ViewInfo.ViewPosChange(new XYZInt(1024, 1024, 1024));
	            };
            testing.Items.GetSubmenu("CMD3 - Check Screen Calculation").Click += (sender, args) =>
            {
                var posWorld = new WorldPos();
                this.ViewInfo.ScreenXyGetTerrainPos(new XYInt(500, 1000), ref posWorld);
            };
            testing.Items.GetSubmenu("CMD4 - MousePos").Click += (sender, args) =>
                {
                    this.ViewInfo.MouseOver = new ViewInfo.clsMouseOver();
                    this.ViewInfo.MouseOver.ScreenPos.X = 500;
                    this.ViewInfo.MouseOver.ScreenPos.Y = 1000;
                    this.ViewInfo.MouseOverPosCalc();
                };
            
		}
	}
}
