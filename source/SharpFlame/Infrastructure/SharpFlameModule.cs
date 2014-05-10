using Eto.Gl;
using Ninject.Modules;
using SharpFlame.Gui.Actions;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Sections;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Settings;
using SharpFlame.UiOptions;

namespace SharpFlame.Infrastructure
{
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

            this.Bind<GLSurface>()
                .ToSelf()
                .InSingletonScope()
                .Named(NamedBinding.MapView);

            this.Bind<GLSurface>()
                .ToSelf()
                .InSingletonScope()
                .Named(NamedBinding.TextureView);

            this.Bind<SharpFlameApplication>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<MainMapView>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<ViewInfo>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<MainForm>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<Options>()
                .ToSelf()
                .InSingletonScope();

            this.Bind<MinimapCreator>()
                .ToSelf()
                .InSingletonScope();
                
            #if DEBUG
            // This is here to keep a reference of it.
            this.Bind<Keylogger>()
                .ToSelf()
                .InSingletonScope();
            #endif
        }
    }
}