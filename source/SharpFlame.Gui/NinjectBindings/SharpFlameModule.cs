using Ninject.Modules;
using SharpFlame.Gui.Controls;

namespace SharpFlame.Gui.NinjectBindings
{
    public class SharpFlameModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<GLSurface>()
                .ToSelf()
                .InSingletonScope()
                .Named(Binding.MapView);

            this.Bind<GLSurface>()
                .ToSelf()
                .InSingletonScope()
                .Named(Binding.TextureView);
        }
    }
}