using Eto.Forms;

namespace SharpFlame.Gui.Dialogs
{
    public class Dialog2<T> : Dialog<T>
    {
        new public T DataContext
        {
            get { return (T)base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}