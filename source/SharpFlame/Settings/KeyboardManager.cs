using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Appccelerate.EventBroker;
using Eto.Forms;
using Ninject.Extensions.Logging;

namespace SharpFlame.Settings
{
    public sealed class InvalidKeyException : Exception
    {
        public InvalidKeyException()
        {
        }

        public InvalidKeyException(string message) 
            : base(message)
        {
        }

        public InvalidKeyException(string message,
            Exception innerException): base(message, innerException)
        {
        }
    }

    public class KeyboardEventArgs : EventArgs
    {
        public KeyboardCommand Key;
    }

    public class KeyboardCommand {
        public string Name { get; set; }

        public Keys? Key { get; private set; }

        public bool Repeat { get; set; }
        public bool Active { get; set; }

        public KeyboardCommand(string name, Keys? key = null, bool repeat = false)
        {
            Name = name;
            if( key != null )
            {
                Key = key;
            }
            Active = false;
            Repeat = repeat;
        }

        public override string ToString()
        {
            return Key?.ToString() ?? $"{Name} has no key!";
        }
    }

    public class KeyboardManager
    {
        public event EventHandler<KeyboardEventArgs> KeyUp = delegate {};

        public event EventHandler<KeyboardEventArgs> KeyDown = delegate {};

        public readonly Dictionary<string, KeyboardCommand> Commands = new Dictionary<string, KeyboardCommand>();

        public readonly Dictionary<string, KeyboardCommand> ActiveCommands = new Dictionary<string, KeyboardCommand>();

        private readonly Dictionary<Keys, KeyboardCommand> hookTable = new Dictionary<Keys, KeyboardCommand>();

        private readonly ILogger logger;

        private UITimer watchModifier = new UITimer();


        public KeyboardManager(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();
            watchModifier.Interval = 0.100;
            watchModifier.Elapsed += WatchModifier_Elapsed;
        }

        public void RegisterClearAll()
        {
            this.Commands.Clear();
            this.hookTable.Clear();
            this.ActiveCommands.Clear();
        }

        public bool Register(string name, Eto.Forms.Keys key = Eto.Forms.Keys.None, bool repeat = false)
        {
            if( this.Commands.ContainsKey(name) )
            {
                throw new Exception($"The key '{name}' already exist.");
            }

            var command = new KeyboardCommand(name, key, repeat);

            this.Commands.Add(name, command);

            if( key != Keys.None )
            {
                hookTable.Add(key, command);
            }

            return true;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="etoKey">Key.</param>
        public void RegisterUpdate(string name, Keys etoKey = Keys.None, bool repeat = false)
        {
            if( !this.Commands.ContainsKey(name) )
            {
                throw new Exception($"The key '{name}' does not exist.");
            }

            Commands.Remove(name);

            KeyboardCommand cmd;
            if( hookTable.TryGetValue(etoKey, out cmd) )
            {
                hookTable.Remove(etoKey);
            }

            Register(name, etoKey, repeat);
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            var kd = e.KeyData;
            KeyboardCommand cmd = null;
            if( !hookTable.TryGetValue(kd, out cmd) )
            {
                return;
            }

            if( cmd.Active )
            {
                if( cmd.Repeat )
                {
                    KeyDown(sender, new KeyboardEventArgs { Key = cmd });
                }
            }
            else
            {
                Activate(cmd);
                KeyDown(sender, new KeyboardEventArgs { Key = cmd });
            }

            e.Handled = true;

            logger.Debug($"KeyDown: {kd}, Char: {( e.IsChar ? e.KeyChar.ToString() : "no char" )}, Handled: {e.Handled}");
        }

        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            var kd = e.KeyData;

            KeyboardCommand cmd = null;
            if( !hookTable.TryGetValue(kd, out cmd) )
            {
                return;
            }

            Deactivate(cmd);
            
            KeyUp(sender, new KeyboardEventArgs { Key = cmd });

            e.Handled = true;

            logger.Debug($"KeyUp: {kd}, Char: { ( e.IsChar ? e.KeyChar.ToString() : "no char" ) }, Handled: {e.Handled}");
        }

        private void Activate(KeyboardCommand cmd)
        {
            cmd.Active = true;
            this.ActiveCommands.Add(cmd.Name, cmd);
        }

        private void Deactivate(KeyboardCommand cmd)
        {
            cmd.Active = false;

            if( ActiveCommands.ContainsKey(cmd.Name) )
            {
                ActiveCommands.Remove(cmd.Name);
            }
        }

        private void ActivateModifier(object sender)
        {
            var current = Keyboard.Modifiers;
            var keyeve = new KeyEventArgs(current, KeyEventType.KeyDown);

            HandleKeyDown(sender, keyeve);
        }

        private Keys lastModifier;

        public void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if( e.Modifiers != Keys.None )
            {
                //we have modifer keys.
                lastModifier = e.Modifiers;
                ActivateModifier(sender);
                watchModifier.Start();
            }
        }

        private void WatchModifier_Elapsed(object sender, EventArgs e)
        {
            //reset modifier
            if( Keyboard.Modifiers != Keys.None )
                return;//still going.

            var active = this.ActiveCommands.Values.ToArray()
                .Where( cmd => cmd.Key == lastModifier);

            if( active.Any() )
            {
                active.ForEach(Deactivate);
            }

            watchModifier.Stop();
        }
    }
}

