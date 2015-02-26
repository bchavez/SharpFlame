using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SharpFlame.MouseTools
{
    public class MinimapOpts : INotifyPropertyChanged
    {
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
        
        public MinimapOpts() {
            Textures = true;
            Heights = true;
            Cliffs = false;
            Objects = true;
            Gateways = false;
        }
    }
}

