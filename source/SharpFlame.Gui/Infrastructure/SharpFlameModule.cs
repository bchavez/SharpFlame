using Ninject.Modules;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Sections;

namespace SharpFlame.Gui.Infrastructure
{
    public class SharpFlameModule : NinjectModule
    {
        public override void Load()
        {
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

            this.Bind<MainForm>()
                .ToSelf()
                .InSingletonScope();


            //tabs
            this.Bind<TextureTab>().ToSelf().InSingletonScope();
            this.Bind<TerrainTab>().ToSelf().InSingletonScope();
            this.Bind<HeightTab>().ToSelf().InSingletonScope();
            this.Bind<ResizeTab>().ToSelf().InSingletonScope();
            this.Bind<PlaceObjectsTab>().ToSelf().InSingletonScope();
            this.Bind<ObjectTab>().ToSelf().InSingletonScope();
            this.Bind<LabelsTab>().ToSelf().InSingletonScope();

        }
    }
}