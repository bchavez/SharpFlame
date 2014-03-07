#region License
/*
  The MIT License (MIT)
 
  Copyright (c) 2013-2014 The SharpFlame Authors.
 
  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:
 
  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.
 
  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
  */
#endregion

//Pulled from : ninject / Ninject.Extensions.AppccelerateEventBroker

using System;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace SharpFlame.Gui.Infrastructure
{
    /// <summary>
    /// Module for the event broker extension.
    /// </summary>
    public class EventBrokerModule : NinjectModule
    {
        /// <summary>
        /// The name of the default global event broker
        /// </summary>
        public const string DefaultGlobalEventBrokerName = "GlobalEventBroker";

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            this.Kernel.AddGlobalEventBroker(DefaultGlobalEventBrokerName);
        }

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public override void VerifyRequiredModulesAreLoaded()
        {
            if( !this.Kernel.HasModule(typeof(NamedScopeModule).FullName) )
            {
                throw new InvalidOperationException("The EventBrokerModule requires NamedScopeModule!");
            }

            if( !this.Kernel.HasModule(typeof(ContextPreservationModule).FullName) )
            {
                throw new InvalidOperationException("The EventBrokerModule requires ContextPreservationModule!");
            }
        }
    }
}