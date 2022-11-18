using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PRBD_Framework {
    public static class BindingExtensions {
        public class Binding {
            private bool binding;

            public INotifyPropertyChanged Source { get; private set; }
            public string SourcePropName { get; private set; }
            public INotifyPropertyChanged Target { get; private set; }
            public string TargetPropName { get; private set; }
            public bool Bidirectional { get; private set; }

            public Binding(
                INotifyPropertyChanged source, string sourcePropName,
                INotifyPropertyChanged target, string targetPropName,
                bool bidirectional = false) {

                Source = source;
                SourcePropName = sourcePropName;
                Target = target;
                TargetPropName = targetPropName;
                Bidirectional = bidirectional;

                CopyValue(Source, SourcePropName, Target, TargetPropName);

                source.PropertyChanged += Source_PropertyChanged;
                if (bidirectional)
                    target.PropertyChanged += Target_PropertyChanged;
            }

            private void CopyValue(INotifyPropertyChanged source, string sourcePropName, INotifyPropertyChanged target, string targetPropName) {
                if (!binding) {
                    binding = true;
                    var val = source?.GetType().GetProperty(sourcePropName)?.GetValue(source);
                    //Console.WriteLine(sender + " : " + e.PropertyName + " = " + val);
                    target?.GetType().GetProperty(targetPropName)?.SetValue(target, val);
                    binding = false;
                }
            }

            private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName != SourcePropName) return;
                CopyValue(Source, SourcePropName, Target, TargetPropName);
            }

            private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName != TargetPropName) return;
                CopyValue(Target, TargetPropName, Source, SourcePropName);
            }

            public void Unbind() {
                Source.PropertyChanged -= Source_PropertyChanged;
                if (Bidirectional)
                    Target.PropertyChanged -= Target_PropertyChanged;
            }
        }

        private static List<Binding> bindings = new List<Binding>();

        public static void BindOneWay(this INotifyPropertyChanged source, string sourcePropName, INotifyPropertyChanged target, string targetPropName) {
            var binding = new Binding(source, sourcePropName, target, targetPropName, false);
            bindings.Add(binding);
        }

        public static void BindTwoWays(this INotifyPropertyChanged source, string sourcePropName, INotifyPropertyChanged target, string targetPropName) {
            var binding = new Binding(source, sourcePropName, target, targetPropName, true);
            bindings.Add(binding);
        }

        public static void UnbindAll() {
            foreach (var b in bindings.ToList()) {
                bindings.Remove(b);
                b.Unbind();
            }
        }

        public static void Unbind(this INotifyPropertyChanged obj) {
            foreach (var b in bindings.ToList()) {
                if (b.Source == obj || b.Target == obj) {
                    bindings.Remove(b);
                    b.Unbind();
                }
            }
        }

        public static void Unbind(this INotifyPropertyChanged source, string sourePropName) {
            foreach (var b in bindings.ToList()) {
                if (b.Source == source && b.SourcePropName == sourePropName) {
                    bindings.Remove(b);
                    b.Unbind();
                }
            }
        }
    }

}
