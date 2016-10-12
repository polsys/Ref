using System;

namespace Polsys.Ref.ViewModels
{
    /// <summary>
    /// The base class for all catalogue entries and subentries.
    /// </summary>
    internal abstract class EntryViewModelBase : ViewModelBase
    {
        // This is in the base class since all entries have a name
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, nameof(Title)); }
        }
        private string _title;

        /// <summary>
        /// Gets whether this entry is read-only or editable.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            protected set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    NotifyPropertyChanged(nameof(IsReadOnly));
                }
            }
        }
        private bool _isReadOnly;

        /// <summary>
        /// Gets or sets whether this is the currently selected entry.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged(nameof(IsSelected));
                }
            }
        }
        private bool _isSelected;

        /// <summary>
        /// Cancels the pending changes and resets the properties to original values.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsReadOnly"/> is true.</exception>
        public virtual void Cancel()
        {
            // Implemented by derived classes
            throw new NotImplementedException();
        }

        /// <summary>
        /// Commits the changed properties to the model.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsReadOnly"/> is true.</exception>
        public virtual void Commit()
        {
            // Implemented by derived classes
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes this entry editable.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsReadOnly"/> is already false.</exception>
        public void Edit()
        {
            if (!IsReadOnly)
                throw new InvalidOperationException("Already in edit mode.");

            IsReadOnly = false;
        }

        /// <summary>
        /// Sets a property, checks for edit mode and raises the PropertyChanged event.
        /// </summary>
        /// <param name="property">A reference to the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyName">The property name that will be passed to the event.</param>
        protected void SetProperty(ref string property, string value, string propertyName)
        {
            if (property != value)
            {
                if (IsReadOnly)
                    throw new InvalidOperationException("Not in edit mode.");

                property = value;
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}
