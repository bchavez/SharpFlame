using Eto.Gl;
using Eto.IO;
using Ninject.Modules;
using SharpFlame.Gui.Actions;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Sections;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Settings;
using SharpFlame.UiOptions;

namespace SharpFlame.Infrastructure
{
    public class NamedBinding
    {
        public const string MapGl = "MapGl";
        public const string TextureGl = "TextureGl";
    }
    public class SharpFlameModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<KeyboardManager>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<SettingsManager>()
                .ToSelf()
                .InSingletonScope();

            //this.Bind<SharpFlameApplication>()
            //    .ToSelf()
            //    .InSingletonScope();

            this.Bind<MainMapView>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<ViewInfo>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<MainForm>()
                .ToSelf()
                .InSingletonScope()
                .RegisterOnGlobalEventBroker();

            this.Bind<Options>()
                .ToSelf()
                .InSingletonScope();

            //this.Bind<MinimapCreator>()
            //    .ToSelf()
            //    .InSingletonScope();

            /*this.Bind<Updater>()
                .ToSelf()
                .InSingletonScope();*/
                
            #if DEBUG
            // This is here to keep a reference of it.
            this.Bind<Keylogger>()
                .ToSelf()
                .InSingletonScope();
            #endif

            //GUI
            this.Bind<TextureTab>().ToSelf().InSingletonScope().RegisterOnGlobalEventBroker();
            this.Bind<TerrainTab>().ToSelf().InSingletonScope();
            //this.Bind<ResizeTab>().ToSelf().InSingletonScope();
            this.Bind<PlaceObjectsTab>().ToSelf().InSingletonScope();
            this.Bind<ObjectTab>().ToSelf().InSingletonScope();
            this.Bind<LabelsTab>().ToSelf().InSingletonScope();
            this.Bind<HeightTab>().ToSelf().InSingletonScope();
            this.Bind<LoadMap>().ToSelf().InSingletonScope().RegisterOnGlobalEventBroker();

            this.Bind<Map>().ToSelf().InTransientScope();
            this.Bind<clsDrawSectorObjects>().ToSelf().InTransientScope();
            this.Bind<Gui.Dialogs.Settings>().ToSelf().InTransientScope();

            //Mapping.IO
            this.Bind<SharpFlame.Mapping.IO.FMap.FMapLoader>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.FMap.FMapSaver>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.Heightmap.HeightmapSaver>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.LND.LNDLoader>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.LND.LNDSaver>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.Minimap.MinimapSaver>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.TTP.TTPLoader>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.TTP.TTPSaver>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.Wz.WzLoader>().ToSelf().InSingletonScope();
            this.Bind<SharpFlame.Mapping.IO.Wz.WzSaver>().ToSelf().InSingletonScope();
        }
    }
}