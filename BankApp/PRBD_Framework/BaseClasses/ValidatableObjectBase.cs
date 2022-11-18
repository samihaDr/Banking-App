using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace PRBD_Framework {
    public abstract class ValidatableObjectBase : INotifyDataErrorInfo {
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void AddError(string propertyName, string error) {
            if (!_validationErrors.ContainsKey(propertyName)) {
                _validationErrors[propertyName] = new List<string>();
            }

            _validationErrors[propertyName].Add(error);
            RaiseErrorsChanged(propertyName);
        }

        public void SetError(string propertyName, string error) {
            if (!_validationErrors.ContainsKey(propertyName)) {
                _validationErrors[propertyName] = new List<string>();
            }

            _validationErrors[propertyName].Clear();
            _validationErrors[propertyName].Add(error);
            RaiseErrorsChanged(propertyName);
        }

        public void SetErrors(string propertyName, IEnumerable errors) {
            _validationErrors[propertyName] = new List<string>();
            if (errors != null)
                foreach (var s in errors)
                    _validationErrors[propertyName].Add(s.ToString());
            RaiseErrorsChanged(propertyName);
        }

        public void ClearErrors(string propertyName) {
            _validationErrors[propertyName].Clear();
            RaiseErrorsChanged(propertyName);
        }

        public void ClearErrors() {
            foreach (var key in _validationErrors.Keys) {
                _validationErrors[key].Clear();
                RaiseErrorsChanged(key);
            }
        }

        public void RaiseErrors() {
            foreach (var key in _validationErrors.Keys)
                RaiseErrorsChanged(key);
        }

        public void RaiseErrorsChanged(string propertyName) {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName) {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
                return null;
            return _validationErrors[propertyName];
        }

        public void AddErrors(Dictionary<string, ICollection<string>> errors) {
            foreach (var key in errors.Keys) {
                if (!_validationErrors.ContainsKey(key))
                    _validationErrors[key] = new List<string>();

                foreach (var s in errors[key]) {
                    _validationErrors[key].Add(s);
                    //TODO: bonne idée ?
                    RaiseErrorsChanged(key);
                }
            }
        }

        public void SetErrors(Dictionary<string, ICollection<string>> errors) {
            ClearErrors();
            AddErrors(errors);
        }

        public bool HasErrors {
            get {
                foreach (var key in _validationErrors.Keys)
                    if (_validationErrors[key].Count > 0)
                        return true;
                return false;
            }
        }

        public Dictionary<string, ICollection<string>> Errors {
            get => _validationErrors;
        }

        public virtual bool Validate() => true;

        public virtual bool Validate(DbContextBase context) => true;
    }
}
