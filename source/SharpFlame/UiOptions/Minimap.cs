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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SharpFlame.UiOptions
{
    public class Minimap : INotifyPropertyChanged
    {
        #region Properties      
        private bool textures;
        public bool Textures { 
            get { return textures; }
            set { SetField(ref textures, value, () => Textures); }
        }
        private bool heights;
        public bool Heights { 
            get { return heights; }
            set { SetField(ref heights, value, () => Heights); }
        }
        private bool cliffs;
        public bool Cliffs { 
            get { return cliffs; }
            set { SetField(ref cliffs, value, () => Cliffs); }
        }
        private bool objects;
        public bool Objects { 
            get { return objects; }
            set { SetField(ref objects, value, () => Objects); }
        }
        private bool gateways;
        public bool Gateways { 
            get { return gateways; }
            set { SetField(ref gateways, value, () => Gateways); }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
            OnPropertyChanged(body.Member.Name);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }
        #endregion

        public Minimap() {
            Textures = true;
            Heights = true;
            Cliffs = false;
            Objects = true;
            Gateways = false;
        }
    }
}

